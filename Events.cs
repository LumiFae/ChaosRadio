using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using MEC;
using RadioItem = InventorySystem.Items.Radio.RadioItem;
using SpeakerToy = LabApi.Features.Wrappers.SpeakerToy;

namespace ChaosRadio
{
    public class Events
    {
        public void OnReceivedLoadout(PlayerReceivedLoadoutEventArgs ev)
        {
            if (ev.Player.TryGetRadio(out Item item))
            {
                Plugin.Instance.NtfRadios.Add(item.Serial);
            }

            if (!ev.Player.IsChaos) return;
            Item radio = ev.Player.AddItem(ItemType.Radio);
            if (radio == null) return;
            Plugin.Instance.ChaosRadios.Add(radio.Serial);
        }

        public void OnDropped(PlayerDroppedItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup pickup || !Plugin.Instance.Config.DisableRadioPickups) return;
            bool isChaosRadio = pickup.IsChaosRadio();
            if (pickup.IsEnabled)
            {
                SpeakerToy speaker = SpeakerToy.Create(pickup.Transform);
                speaker.ControllerId = (byte)pickup.Serial;
                if (isChaosRadio)
                    Plugin.Instance.ChaosSpeakers.Add(pickup.Serial, speaker);
                else
                    Plugin.Instance.NtfSpeakers.Add(pickup.Serial, speaker);
                
                Plugin.Instance.RadioCoroutines.Add(pickup, Timing.RunCoroutine(Plugin.Instance.DrainBattery(pickup)));
            }
            pickup.IsEnabled = false;
        }

        public void OnSpawnedItem(ItemSpawnedEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup) return;
            if (Plugin.Instance.Config.ReplacePercentage == 0) return;
            if(UnityEngine.Random.Range(0, 100) <= Plugin.Instance.Config.ReplacePercentage)
            {
                Plugin.Instance.ChaosRadios.Add(ev.Pickup.Serial);
            } else
            {
                Plugin.Instance.NtfRadios.Add(ev.Pickup.Serial);
            }
        }

        public void OnPickingUpItem(PlayerPickingUpItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup pickup) return;
            bool chaosRadio = pickup.IsChaosRadio();
            if (chaosRadio)
            {
                ev.Player.SendHint(Plugin.Instance.Translation.ChaosRadioPickupText);
            }
            else
            {
                ev.Player.SendHint(Plugin.Instance.Translation.NtfRadioPickupText);
            }

            if (!Plugin.Instance.RadioCoroutines.TryGetValue(pickup, out CoroutineHandle handle)) return;
            Timing.KillCoroutines(handle);
            pickup.IsEnabled = true;
            if (chaosRadio)
                Plugin.Instance.ChaosSpeakers.Remove(ev.Pickup.Serial);
            else 
                Plugin.Instance.NtfSpeakers.Remove(ev.Pickup.Serial);
        }

        public void OnItemAdded(ReferenceHub refHub, InventorySystem.Items.ItemBase item, InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            if (item.ServerAddReason != InventorySystem.Items.ItemAddReason.AdminCommand) return;
            if (pickup != null || item is not RadioItem radio) return;
            if (Plugin.Instance.Config.GivenItemsDefaultToNtf)
            {
                Plugin.Instance.NtfRadios.Add(radio.ItemSerial);
            }
            else
            {
                Plugin.Instance.ChaosRadios.Add(radio.ItemSerial);
            }
        }
    }
}