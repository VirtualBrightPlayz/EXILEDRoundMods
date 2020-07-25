using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPBoss
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("The SCP Boss Health multiplier")]
        public float BaseHPMultiply { get; set; } = 1.5f;
        [Description("The Role to replace non boss SCPs with !!!DO NOT MAKE IT AN SCP!!!")]
        public RoleType scpReplaceType { get; set; } = RoleType.ClassD;
    }
}
