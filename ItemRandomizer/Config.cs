using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace ItemRandomizer
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public List<ItemType> ExcludeItems { get; set; } = new List<ItemType>() { ItemType.KeycardJanitor, ItemType.KeycardScientist, ItemType.KeycardScientistMajor, ItemType.KeycardZoneManager };
    }
}