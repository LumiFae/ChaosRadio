using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using InventorySystem.Items.Radio;

namespace ChaosRadio.Items
{
    public class ChaosRadioItem : CustomItem
    {
        public override uint Id { get; set; } = Plugin.Instance.Config.ChaosItemId;
        public override string Name { get; set; } = Plugin.Instance.Translation.ChaosRadioName;
        public override string Description { get; set; } = Plugin.Instance.Translation.ChaosRadioDescription;

        public override float Weight { get; set; }

        public override SpawnProperties SpawnProperties { get; set; }

        public override ItemType Type { get; set; } = ItemType.Radio;
    }
}