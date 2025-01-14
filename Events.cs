using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

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
            if (ev.Pickup is RadioPickup pickup && Plugin.Instance.Config.DisableRadioPickups)
            {
                pickup.IsEnabled = false;
                if (!Plugin.Instance.Config.Debug) return;
                bool isChaosRadio = pickup.IsPickupChaosRadio();
                Log.Debug($"{(isChaosRadio ? "Chaos" : "NTF")} Radio dropped at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
            }
        }

        public void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup) return;
            if (Plugin.Instance.Config.ReplacePercentage == 0) return;
            if(UnityEngine.Random.Range(0, 100) <= Plugin.Instance.Config.ReplacePercentage)
            {
                Log.Debug($"Spawning Chaos Radio at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
                Plugin.Instance.ChaosRadios.Add(ev.Pickup.Serial);
            } else
            {
                Log.Debug($"Spawning NTF Radio at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
                Plugin.Instance.NtfRadios.Add(ev.Pickup.Serial);
            }
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup is RadioPickup pickup)
            {
                if (pickup.IsPickupChaosRadio())
                {
                    Log.Debug($"Player {ev.Player.Nickname} picked up a Chaos Radio");
                    ev.Player.ShowHint(Plugin.Instance.Translation.ChaosRadioPickupText);
                }
                else
                {
                    Log.Debug($"Player {ev.Player.Nickname} picked up a NTF Radio");
                    ev.Player.ShowHint(Plugin.Instance.Translation.NtfRadioPickupText);
                }
            }
        }

        public void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (ev.Pickup is RadioPickup pickup)
            {
                if (Plugin.Instance.Config.GivenItemsDefaultToNtf)
                {
                    Log.Debug($"Player {ev.Player.Nickname} was given a radio, and it defaulted to NTF");
                    Plugin.Instance.NtfRadios.Add(pickup.Serial);
                }
                else
                {
                    Log.Debug($"Player {ev.Player.Nickname} was given a radio, and it defaulted to Chaos");
                    Plugin.Instance.ChaosRadios.Add(pickup.Serial);
                }
            }
        }
    }
}