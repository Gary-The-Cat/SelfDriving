using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulation.ECS.Components
{
    public enum PowerupType
    {
        GrabBall,
        DuplicataBalls,
        UnstoppableBall
    }

    public class PowerupComponent
    {
        public PowerupType Type { get; set; }
    }
}
