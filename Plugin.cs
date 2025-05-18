using AdminToys;
#if EXILED
using Exiled.API.Enums;
using Exiled.API.Features;
#endif
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Radio;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader;
using MEC;
using Mirror;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace ChaosRadio
{
    public class Plugin : 
#if EXILED
        Plugin<Config, Translation>
#else
    LabApi.Loader.Features.Plugins.Plugin
#endif
    {
        public static Plugin Instance { get; private set; }
        
        public override string Name => "ChaosRadio";
        public override string Author => "LumiFae";
#if EXILED
        public override string Prefix => "ChaosRadio";
        public override Version RequiredExiledVersion => new (9, 3, 0);
        public override PluginPriority Priority => PluginPriority.Default;
#else
        public override string Description { get; } = "Adds a secondary radio channel, given to chaos insurgency.";
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        
        public Config Config { get; private set; }
        public Translation Translation { get; private set; }
#endif
        public override Version Version => new (1, 3, 0);

        private Harmony _harmony;
        
        public List<ushort> ChaosRadios { get; private set; }
        public List<ushort> NtfRadios { get; private set; }
        
        public Dictionary<ushort, SpeakerToy> ChaosSpeakers { get; private set; }
        public Dictionary<ushort, SpeakerToy> NtfSpeakers { get; private set; }
        
        public Dictionary<RadioPickup, CoroutineHandle> RadioCoroutines { get; private set; }
        
        private SpeakerToy SpeakerPrefab { get; set; }
        
        private Events Events { get; set; }

#if EXILED
        public override void OnEnabled()
#else
        public override void Enable()
#endif
        {
            Instance = this;

            try
            {
                _harmony = new($"lumifae.chaosradio.{DateTime.Now.Ticks}");
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
#if EXILED
                Log.Error($"Failed to patch: {e}");
#else
                Logger.Error($"Failed to patch: {e}");
#endif
            }

            ChaosRadios = new();
            NtfRadios = new();
            ChaosSpeakers = new();
            NtfSpeakers = new();
            RadioCoroutines = new();

            Events = new();
            PlayerEvents.ReceivedLoadout += Events.OnReceivedLoadout;
            PlayerEvents.DroppedItem += Events.OnDropped;
            ServerEvents.ItemSpawned += Events.OnSpawnedItem;
            PlayerEvents.PickingUpItem += Events.OnPickingUpItem;
            InventoryExtensions.OnItemAdded += Events.OnItemAdded;
#if EXILED
            base.OnEnabled();
#endif
        }

#if EXILED
        public override void OnDisabled()
#else
        public override void Disable()
#endif
        {
            _harmony.UnpatchAll();
            _harmony = null;
            
            ChaosRadios = null;
            NtfRadios = null;
            ChaosSpeakers = null;
            NtfSpeakers = null;
            RadioCoroutines = null;
            
            PlayerEvents.ReceivedLoadout -= Events.OnReceivedLoadout;
            PlayerEvents.DroppedItem -= Events.OnDropped;
            ServerEvents.ItemSpawned -= Events.OnSpawnedItem;
            PlayerEvents.PickingUpItem -= Events.OnPickingUpItem;
            InventoryExtensions.OnItemAdded -= Events.OnItemAdded;
            Events = null;
#if EXILED
            base.OnDisabled();
#endif
        }
        
#if LABAPI
        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config config);
            this.TryLoadConfig("translation.yml", out Translation translation);
            Config = config ?? new();
            Translation = translation ?? new();
        }
#endif

        internal IEnumerator<float> DrainBattery(RadioPickup pickup)
        {
            float depletion = RadioPickup._radioCache.Ranges[pickup.SavedRange]
                .MinuteCostWhenIdle/60;
            while (pickup.SavedBattery >= 0)
            {
                yield return Timing.WaitForSeconds(1);
                pickup.SavedBattery -= depletion;
            }
        }

        internal SpeakerToy GetSpeakerPrefab()
        {
            if(SpeakerPrefab != null) return SpeakerPrefab;
            foreach (GameObject prefabsValue in NetworkClient.prefabs.Values)
            {
                if (!prefabsValue.TryGetComponent(out SpeakerToy toy)) continue;
                SpeakerPrefab = toy;
                return toy;
            }

            return null;
        }
    }
}