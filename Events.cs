using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles;
using VoiceChat;
using VoiceChat.Networking;

namespace ChaosRadio
{
    public class Events
    {
        public void OnSpawn(SpawnedEventArgs ev)
        {
            if (ev.Player.TryGetRadio(out Item item))
            {
                Plugin.Instance.NtfRadios.Add(item.Serial);
                Log.Debug($"Player {ev.Player.Nickname} spawned with NTF Radio");
            }

            if (!ev.Player.IsCHI || !ev.SpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory)) return;
            Item radio = Item.Create(ItemType.Radio);
            Plugin.Instance.ChaosRadios.Add(radio.Serial);
            ev.Player.AddItem(radio);
            Log.Debug($"Given player {ev.Player.Nickname} a Chaos Radio");
        }

        public void OnDropped(DroppedItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup pickup || !Plugin.Instance.Config.DisableRadioPickups) return;
            bool isChaosRadio = pickup.IsChaosRadio();
            if (pickup.IsEnabled)
            {
                Speaker speaker = Speaker.Create(pickup.Position, pickup.Rotation.eulerAngles, pickup.Scale, true);
                speaker.Base.transform.parent = pickup.Transform;
                speaker.ControllerId = (byte)pickup.Serial;
                if (isChaosRadio)
                    Plugin.Instance.ChaosSpeakers.Add(pickup.Serial, speaker);
                else
                    Plugin.Instance.NtfSpeakers.Add(pickup.Serial, speaker);
            }
            pickup.IsEnabled = false;
            Log.Debug($"{(isChaosRadio ? "Chaos" : "NTF")} Radio dropped at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
        }

        public void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup pickup) return;
            if (Plugin.Instance.Config.ReplacePercentage == 0) return;
            if(UnityEngine.Random.Range(0, 100) <= Plugin.Instance.Config.ReplacePercentage)
            {
                Log.Debug($"Spawning Chaos Radio at {pickup.Room.Name} in {pickup.Room.Zone}");
                Plugin.Instance.ChaosRadios.Add(pickup.Serial);
            } else
            {
                Log.Debug($"Spawning NTF Radio at {pickup.Room.Name} in {pickup.Room.Zone}");
                Plugin.Instance.NtfRadios.Add(pickup.Serial);
            }
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup pickup) return;
            if (pickup.IsChaosRadio())
            {
                Log.Debug($"Player {ev.Player.Nickname} picked up a Chaos Radio");
                ev.Player.ShowHint(Plugin.Instance.Translation.ChaosRadioPickupText);
                Plugin.Instance.ChaosSpeakers.Remove(pickup.Serial);
            }
            else
            {
                Log.Debug($"Player {ev.Player.Nickname} picked up a NTF Radio");
                ev.Player.ShowHint(Plugin.Instance.Translation.NtfRadioPickupText);
                Plugin.Instance.NtfSpeakers.Remove(pickup.Serial);
            }

            pickup.IsEnabled = true;
        }

        public void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (ev.Pickup != null || ev.Item is not Radio radio) return;
            if (Plugin.Instance.Config.GivenItemsDefaultToNtf)
            {
                Log.Debug($"Player {ev.Player.Nickname} was given a radio, and it defaulted to NTF");
                Plugin.Instance.NtfRadios.Add(radio.Serial);
            }
            else
            {
                Log.Debug($"Player {ev.Player.Nickname} was given a radio, and it defaulted to Chaos");
                Plugin.Instance.ChaosRadios.Add(radio.Serial);
            }
        }

        // public void OnVoiceChatting(VoiceChattingEventArgs ev)
        // {
        //     if (ev.VoiceMessage.Channel != VoiceChatChannel.Radio) return;
        //     if (!ev.Player.TryGetRadio(out Item item)) return;
        //     Log.Debug(item);
        //     bool isChaos = item.IsChaosRadio();
        //     Log.Debug(isChaos);
        //     
        //     OpusHandler opusHandler = OpusHandler.Get(ev.Player);
        //     float[] decodedBuffer = new float[480];
        //     opusHandler.Decoder.Decode(ev.VoiceMessage.Data, ev.VoiceMessage.DataLength, decodedBuffer);
        //
        //     byte[] encodedData = new byte[512];
        //     int dataLen = opusHandler.Encoder.Encode(decodedBuffer, encodedData);
        //     Dictionary<ushort, Speaker> speakers = isChaos ? Plugin.Instance.ChaosSpeakers : Plugin.Instance.NtfSpeakers;
        //     Log.Debug(speakers);
        //     foreach (KeyValuePair<ushort, Speaker> speakerPair in speakers)
        //     {
        //         AudioMessage audioMessage = new (speakerPair.Value.ControllerId, encodedData, dataLen);
        //         foreach (Player player in Player.List)
        //         {
        //             if(player.Role is not IVoiceRole voiceRole  || voiceRole.VoiceModule.ValidateReceive(player.ReferenceHub, VoiceChatChannel.Proximity) == VoiceChatChannel.None) continue;
        //             
        //             player.ReferenceHub.connectionToClient.Send(audioMessage);
        //         }
        //     }
        // }
        //
        // public void OnTransmitting(TransmittingEventArgs ev)
        // {
        //     Log.Debug("Transmitting");
        // }
    }
}