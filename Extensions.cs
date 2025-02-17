using InventorySystem.Items.Radio;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace ChaosRadio
{
    public static class Extensions
    {
        public static IEnumerable<Item> GetItems(this Player player)
        {
            return player.Inventory.UserInventory.Items.Values.Select(Item.Get);
        }
        public static bool HasChaosRadio(this Player player)
        {
            return player.GetItems().Any(p => p.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(p.Serial));
        }
        public static bool TryGetRadio(this Player player, out Item item)
        {
            item = player.GetItems().FirstOrDefault(p => p.Type == ItemType.Radio);
            return item != null;
        }
        public static bool IsChaosRadio(this Item item)
        {
            return item.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(item.Serial);
        }
        public static bool IsChaosRadio(this RadioPickup radio)
        {
            return radio.NetworkInfo.ItemId == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(radio.Info.Serial);
        }
        public static bool IsNtfRadio(this Item item)
        {
            return item.Type == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(item.Serial);
        }
        
        public static bool IsNtfRadio(this RadioPickup radio)
        {
            return radio.NetworkInfo.ItemId == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(radio.Info.Serial);
        }

        public static bool HasSerial(this RadioPickup radio)
        {
            return radio.IsChaosRadio() || radio.IsNtfRadio();
        }
    }
}