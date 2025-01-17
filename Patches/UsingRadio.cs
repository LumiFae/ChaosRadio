using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Toys;
using HarmonyLib;
using Mirror;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

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

        private static void GroundedRadiosTransmit(VoiceMessage msg, Player player, bool isChaos)
        {
            if(!player.TryGetRadio(out Item item)) return;
            if (item is not Radio radioItem) return;
            OpusHandler opusHandler = OpusHandler.Get(player);
            float[] decodedBuffer = new float[480];
            opusHandler.Decoder.Decode(msg.Data, msg.DataLength, decodedBuffer);

            for (int i = 0; i < decodedBuffer.Length; i++)
            {
                decodedBuffer[i] *= 0.8f;
            }
            
            byte[] encodedData = new byte[512];
            int dataLen = opusHandler.Encoder.Encode(decodedBuffer, encodedData);
            Dictionary<ushort, Speaker> speakers = isChaos ? Plugin.Instance.ChaosSpeakers : Plugin.Instance.NtfSpeakers;
            foreach (KeyValuePair<ushort, Speaker> speakerPair in speakers)
            {
                if(Pickup.List.FirstOrDefault(p => p is RadioPickup pickup && pickup.Serial == speakerPair.Key) is not RadioPickup radio) continue;
                int savedRange = radioItem.RangeSettings.MaxRange;
                bool isRadioInRange = Vector3.Distance(player.Position, radio.Position) <= savedRange;
                if (!isRadioInRange) continue;
                speakerPair.Value.Play(encodedData, dataLen);
            }
        }
        
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
            
            GroundedRadiosTransmit(msg, player, chaosRadio);
        
            return false;
        }
    }
}