#if EXILED
using Exiled.API.Interfaces;
#endif

namespace ChaosRadio
{
    public class Translation 
#if EXILED
        : ITranslation
#endif
    {
        public string ChaosRadioPickupText = "You have picked up the <color=green>Chaos Radio</color>";
        
        public string NtfRadioPickupText = "You have picked up the <color=blue>NTF Radio</color>";
    }
}