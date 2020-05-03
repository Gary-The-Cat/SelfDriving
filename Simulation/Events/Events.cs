using CarSimulation.ECS.Components;
using Leopotam.Ecs;

namespace CarSimulation.Events
{
    public class Events
    {
        public class PowerupCollected
        {
            public PowerupType Type { get; set; } 
        }

        public class BallPaddleCollision
        {
            public EcsEntity BallEntity { get; set; }

            public EcsEntity PaddleEntity { get; set; }
        }

        public class DeathOccuredEvent
        {
            public EcsEntity Entity { get; set; }
        }

        public class BrickDestroyedEvent
        {

        }
    }
}
