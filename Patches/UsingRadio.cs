
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Radio;
using Mirror;
using PlayerRoles.Voice;
using VoiceChat;
using VoiceChat.Networking;

namespace ChaosRadio.Patches
{
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public class UsingRadio
    {
        public static bool Prefix(NetworkConnection conn, VoiceMessage msg)
        {
            if (msg.Channel != VoiceChatChannel.Radio) return true;
            if (!Player.TryGet(msg.Speaker, out Player player)) return false;

            bool chaosRadio = player.Items.Any(i => Plugin.Instance.ChaosRadio.Check(i));
            
            if (msg.SpeakerNull || (int) msg.Speaker.netId != (int) conn.identity.netId || !(msg.Speaker.roleManager.CurrentRole is IVoiceRole currentRole1) || !currentRole1.VoiceModule.CheckRateLimit() || VoiceChatMutes.IsMuted(msg.Speaker))
                return false;
            VoiceChatChannel channel = currentRole1.VoiceModule.ValidateSend(msg.Channel);
            if (channel == VoiceChatChannel.None)
                return false;
            currentRole1.VoiceModule.CurrentChannel = channel;
            foreach (Player allPlayer in Player.List)
            {
                ReferenceHub allHub = allPlayer.ReferenceHub;
                bool isCRadio = allPlayer.Items.Any(i => Plugin.Instance.ChaosRadio.Check(i));
                if (chaosRadio != isCRadio) continue;
                if (allHub.roleManager.CurrentRole is IVoiceRole currentRole2)
                {
                    VoiceChatChannel voiceChatChannel = currentRole2.VoiceModule.ValidateReceive(msg.Speaker, channel);
                    if (voiceChatChannel != VoiceChatChannel.None)
                    {
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
                }
            }

            return false;
        }
    }
}