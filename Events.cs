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
            if (!ev.SpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory)) return;
            if (ev.Player.IsCHI)
            {
                Plugin.Instance.ChaosRadio.Give(ev.Player);
            } else if (ev.Player.IsNTF || ev.Player.Role == RoleTypeId.FacilityGuard)
            {
                Item radio = ev.Player.Items.FirstOrDefault(i => i.Type == ItemType.Radio);
                if (radio == null) return;
                ev.Player.RemoveItem(radio);
                Plugin.Instance.NtfRadio.Give(ev.Player);
            }
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
            if (ev.Pickup is not RadioPickup pickup) return;
            if (Plugin.Instance.Config.ReplacePercentage == 0) return;
            ev.ShouldInitiallySpawn = false;
            if(UnityEngine.Random.Range(0, 100) <= Plugin.Instance.Config.ReplacePercentage)
            {
                Log.Debug($"Spawning Chaos Radio at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
                Plugin.Instance.ChaosRadio.Spawn(ev.Pickup.Position);
            } else
            {
                Log.Debug($"Spawning NTF Radio at {ev.Pickup.Room.Name} in {ev.Pickup.Room.Zone}");
                Plugin.Instance.NtfRadio.Spawn(ev.Pickup.Position);
            }
        }
    }
}