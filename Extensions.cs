﻿using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;

namespace ChaosRadio
{
    public static class Extensions
    {
        public static bool HasChaosRadio(this Exiled.API.Features.Player player)
        {
            return player.Items.Any(p => p.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(p.Serial));
        }

        public static bool TryGetRadio(this Exiled.API.Features.Player player, out Item item)
        {
            item = player.Items.FirstOrDefault(p => p.Type == ItemType.Radio);
            return item != null;
        }
        
        public static bool IsChaosRadio(this Item item)
        {
            return item.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(item.Serial);
        }
        public static bool IsChaosRadio(this RadioPickup radio)
        {
            return radio.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(radio.Serial);
        }
        
        public static bool IsNtfRadio(this Item item)
        {
            return item.Type == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(item.Serial);
        }
        
        public static bool IsNtfRadio(this RadioPickup radio)
        {
            return radio.Type == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(radio.Serial);
        }

        public static bool HasSerial(this RadioPickup radio)
        {
            return radio.IsChaosRadio() || radio.IsNtfRadio();
        }
    }
}