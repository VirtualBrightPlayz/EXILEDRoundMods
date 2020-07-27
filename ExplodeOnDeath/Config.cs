using Exiled.API.Interfaces;

namespace ExplodeOnDeath
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public float Delay { get; set; } = 0f;
    }
}