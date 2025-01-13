using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;

namespace ChaosRadio.Items
{
    public class NtfRadioItem : CustomItem
    {
        public override uint Id { get; set; } = Plugin.Instance.Config.NtfItemId;
        public override string Name { get; set; } = Plugin.Instance.Translation.NtfRadioName;
        public override string Description { get; set; } = Plugin.Instance.Translation.NtfRadioDescription;

        public override float Weight { get; set; }

        public override SpawnProperties SpawnProperties { get; set; }

        public override ItemType Type { get; set; } = ItemType.Radio;
    }
}