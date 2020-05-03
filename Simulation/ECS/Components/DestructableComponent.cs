using System;

namespace CarSimulation.ECS.Components
{
    public class DestructableComponent
    {
        public bool ToDestroy { get; set; }

        public Action PreDestroy { get; set; }
    }
}
