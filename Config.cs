using System.ComponentModel;
using Exiled.API.Interfaces;

namespace ChaosRadio
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Due to a game limitations, you can't split radio pickups, enabling this will turn off all radio pickups.")]
        public bool DisableRadioPickups { get; set; } = true;

        [Description("The percentage of spawned radios that you want to be Chaos radios. The rest will be NTF. Set to 100 to only spawn Chaos radios.")]
        public int ReplacePercentage { get; set; } = 50;
        
        [Description("If true, all items given to a player via remote admin will default to NTF, if not then Chaos.")]
        public bool GivenItemsDefaultToNtf { get; set; } = true;
    }
}