using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;

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
            Log.Debug("Lists initialized");

            Events = new();
            Exiled.Events.Handlers.Player.Spawned += Events.OnSpawn;
            Exiled.Events.Handlers.Player.DroppedItem += Events.OnDropped;
            Exiled.Events.Handlers.Map.SpawningItem += Events.OnSpawningItem;
            Exiled.Events.Handlers.Player.PickingUpItem += Events.OnPickingUpItem;
            Exiled.Events.Handlers.Player.ItemAdded += Events.OnItemAdded;
            Log.Debug("Events initialized and spawned");
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            ChaosRadios = null;
            NtfRadios = null;
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
    }
}