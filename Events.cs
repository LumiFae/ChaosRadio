using Exiled.API.Enums;
using Exiled.API.Extensions;
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
            }

            if (!ev.Player.IsCHI || !ev.SpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory)) return;
            Item radio = Item.Create(ItemType.Radio);
            Plugin.Instance.ChaosRadios.Add(radio.Serial);
            ev.Player.AddItem(radio);
        }

        public void OnDropped(DroppedItemEventArgs ev)
        {
            if (ev.Pickup is RadioPickup pickup && Plugin.Instance.Config.DisableRadioPickups)
            {
                pickup.IsEnabled = false;
            }
        }

        public void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if (ev.Pickup is not RadioPickup) return;
            if (Plugin.Instance.Config.ReplacePercentage == 0) return;
            ev.ShouldInitiallySpawn = false;
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
    }
}