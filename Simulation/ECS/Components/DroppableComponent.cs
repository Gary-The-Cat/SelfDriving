using Leopotam.Ecs;
using SFML.System;
using System;

namespace CarSimulation.ECS.Components
{
    public class DroppableComponent
    {
        public Action<EcsWorld, string, Vector2f> SpawnChildComponent;

        public EcsEntity Entity { get; set; }

        public string DropObject { get; set; }
    }
}
