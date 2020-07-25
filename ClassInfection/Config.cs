using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInfection
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("SCP health multiply when infected by SCP")]
        public float SCPHealthMultiply { get; set; } = 0.1f;
    }
}
