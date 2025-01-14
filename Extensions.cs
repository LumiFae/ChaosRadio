namespace ChaosRadio
{
    public static class Extensions
    {
        public static bool HasChaosRadio(this Exiled.API.Features.Player player)
        {
            return player.Items.Any(p => p.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(p.Serial));
        }

        public static bool HasNtfRadio(this Exiled.API.Features.Player player)
        {
            return player.Items.Any(p => p.Type == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(p.Serial));
        }

        public static bool HasRadio(this Exiled.API.Features.Player player)
        {
            return player.Items.Any(p => p.Type == ItemType.Radio);
        }
        
        public static bool TryGetChaosRadio(this Exiled.API.Features.Player player, out Exiled.API.Features.Items.Item item)
        {
            item = player.Items.FirstOrDefault(p => p.Type == ItemType.Radio && Plugin.Instance.ChaosRadios.Contains(p.Serial));
            return item != null;
        }
        
        public static bool TryGetNtfRadio(this Exiled.API.Features.Player player, out Exiled.API.Features.Items.Item item)
        {
            item = player.Items.FirstOrDefault(p => p.Type == ItemType.Radio && Plugin.Instance.NtfRadios.Contains(p.Serial));
            return item != null;
        }

        public static bool TryGetRadio(this Exiled.API.Features.Player player, out Exiled.API.Features.Items.Item item)
        {
            item = player.Items.FirstOrDefault(p => p.Type == ItemType.Radio);
            return item != null;
        }
    }
}