using CarSimulation.ECS.Entities;
using Leopotam.Ecs;
using SFML.System;

namespace CarSimulation.CollisionData
{
    public class Collision
    {
        public float Depth { get; set; }

        public Vector2f Normal { get; set; }

        public EcsEntity EntityA { get; set; }

        public EntityType EntityAType { get; set; }

        public EcsEntity EntityB { get; set; }

        public EntityType EntityBType { get; set; }
    }
}
