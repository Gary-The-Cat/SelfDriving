using Leopotam.Ecs;
using SFML.System;

namespace CarSimulation.ECS.Components
{
    public class AttachableComponent
    {
        public EcsEntity AttachedEntity { get; set; }

        public bool Attached { get; set; }

        public Vector2f OffsetFromEntity { get; set; }
    }
}
