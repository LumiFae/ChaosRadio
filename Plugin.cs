using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Toys;
using HarmonyLib;
using MEC;

namespace ChaosRadio
{
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }
        
        public override string Name => "ChaosRadio";
        public override string Author => "LumiFae";
        public override string Prefix => "ChaosRadio";
        public override Version Version => new (1, 1, 0);
        public override Version RequiredExiledVersion => new (9, 3, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        private Harmony _harmony;
        
        public List<ushort> ChaosRadios { get; private set; }
        public List<ushort> NtfRadios { get; private set; }
        
        public Dictionary<ushort, Speaker> ChaosSpeakers { get; private set; }
        public Dictionary<ushort, Speaker> NtfSpeakers { get; private set; }
        
        public Dictionary<RadioPickup, CoroutineHandle> RadioCoroutines { get; private set; }
        
        private Events Events { get; set; }

        public override void OnEnabled()
        {
            Log.Debug("Hello!");
            Instance = this;

            try
            {
                _harmony = new($"lumifae.chaosradio.{DateTime.Now.Ticks}");
                _harmony.PatchAll();
                Log.Debug("Patch successful");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to patch: {e}");
            }

            ChaosRadios = new();
            NtfRadios = new();
            ChaosSpeakers = new();
            NtfSpeakers = new();
            RadioCoroutines = new();
            Log.Debug("Lists initialized");

            Events = new();
            Exiled.Events.Handlers.Player.Spawned       += Events.OnSpawn;
            Exiled.Events.Handlers.Player.DroppedItem   += Events.OnDropped;
            Exiled.Events.Handlers.Map.   SpawningItem  += Events.OnSpawningItem;
            Exiled.Events.Handlers.Player.PickingUpItem += Events.OnPickingUpItem;
            Exiled.Events.Handlers.Player.ItemAdded     += Events.OnItemAdded;
            Log.Debug("Events initialized and spawned");
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            ChaosRadios = null;
            NtfRadios = null;
            ChaosSpeakers = null;
            NtfSpeakers = null;
            RadioCoroutines = null;
            Log.Debug("Lists nullified");
            
            Exiled.Events.Handlers.Player.Spawned -= Events.OnSpawn;
            Exiled.Events.Handlers.Player.DroppedItem -= Events.OnDropped;
            Exiled.Events.Handlers.Map.SpawningItem -= Events.OnSpawningItem;
            Exiled.Events.Handlers.Player.PickingUpItem -= Events.OnPickingUpItem;
            Exiled.Events.Handlers.Player.ItemAdded -= Events.OnItemAdded;
            Events = null;
            Log.Debug("Events nullified");
            
            Log.Debug("Goodbye!");
            base.OnDisabled();
        }

        internal IEnumerator<float> DrainBattery(RadioPickup pickup)
        {
            float depletion = InventorySystem.Items.Radio.RadioPickup._radioCache.Ranges[pickup.Base.SavedRange]
                .MinuteCostWhenIdle/60;
            while (pickup.BatteryLevel >= 0)
            {
                yield return Timing.WaitForSeconds(1);
                pickup.BatteryLevel -= depletion;
            }
        }
    }
}