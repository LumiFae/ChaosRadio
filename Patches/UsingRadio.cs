
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Radio;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.Voice;
using VoiceChat;
using VoiceChat.Networking;
using static HarmonyLib.AccessTools;

namespace ChaosRadio.Patches
{
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public class UsingRadio
    {
        // public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
        //     
        //     int index = newInstructions.FindIndex(instruction =>
        //         instruction.opcode == OpCodes.Callvirt
        //         && (MethodInfo)instruction.operand == Method(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ValidateReceive)));
        //     index += 1;
        //     
        //     newInstructions.InsertRange(index, new CodeInstruction[]
        //     {
        //         new (OpCodes.Ldarg_1),
        //         new (OpCodes.Ldfld, Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
        //         new (OpCodes.Ldloc_3)
        //     });
        //     
        //     foreach (CodeInstruction instruction in newInstructions)
        //         yield return instruction;
        //
        //     ListPool<CodeInstruction>.Shared.Return(newInstructions);
        // }
        
        public static bool Prefix(NetworkConnection conn, VoiceMessage msg)
        {
            if (msg.Channel != VoiceChatChannel.Radio) return true;
            
            if (!Player.TryGet(msg.Speaker, out Player player)) return false;

            bool chaosRadio = player.HasChaosRadio();
            
            if (msg.SpeakerNull || (int) msg.Speaker.netId != (int) conn.identity.netId || !(msg.Speaker.roleManager.CurrentRole is IVoiceRole currentRole1) || !currentRole1.VoiceModule.CheckRateLimit() || VoiceChatMutes.IsMuted(msg.Speaker))
                return false;
            
            VoiceChatChannel channel = currentRole1.VoiceModule.ValidateSend(msg.Channel);
            
            if (channel == VoiceChatChannel.None)
                return false;
            
            currentRole1.VoiceModule.CurrentChannel = channel;
            
            foreach (Player allPlayer in Player.List)
            {
                ReferenceHub allHub = allPlayer.ReferenceHub;

                bool isCRadio = allPlayer.HasChaosRadio();
                if (chaosRadio != isCRadio) continue;
                
                if (allHub.roleManager.CurrentRole is not IVoiceRole currentRole2) continue;
                
                VoiceChatChannel voiceChatChannel = currentRole2.VoiceModule.ValidateReceive(msg.Speaker, channel);
                
                if (voiceChatChannel == VoiceChatChannel.None) continue;
                
                msg.Channel = voiceChatChannel;
                EventInfo eventInfo = typeof(VoiceTransceiver).GetEvent(nameof(VoiceTransceiver.OnVoiceMessageReceiving));
                
                if (eventInfo != null)
                {
                    MethodInfo methodInfo = eventInfo.GetRaiseMethod();
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(null, new object[] { msg, allHub });
                    }
                }
                
                allHub.connectionToClient.Send<VoiceMessage>(msg);
            }
        
            return false;
        }
    }
}