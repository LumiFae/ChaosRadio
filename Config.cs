﻿using System.ComponentModel;
using Exiled.API.Interfaces;

namespace ChaosRadio
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("The ID of the custom item that will be used for the chaos radio.")]
        public uint ChaosItemId { get; set; } = 333;
        [Description("The ID of the custom item that will be used for the NTF radio. Made so people can differentiate between the two.")]
        public uint NtfItemId { get; set; } = 334;
        
        [Description("Due to a game limitations, you can't split radio pickups, enabling this will turn off all radio pickups.")]
        public bool DisableRadioPickups { get; set; } = true;

        [Description("The percentage of spawned radios that you want to be Chaos radios. The rest will be NTF. Set to 100 to only spawn Chaos radios.")]
        public int ReplacePercentage { get; set; } = 50;
        
        [Description("If true, all items given to a player via remote admin will default to NTF, if not then Chaos.")]
        public bool GivenItemsDefaultToNtf { get; set; } = true;
    }
}