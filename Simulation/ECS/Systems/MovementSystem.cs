using CarSimulation.ECS.Components;
using CarSimulation.TimeTracker;
using Leopotam.Ecs;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class MovementSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<PositionComponent, MovementComponent> filter = null;

        private EcsFilter<PositionComponent, MovementComponent, CollisionComponent, SpriteComponent> collisionFilter = null;

        public void Run()
        {
            foreach(var component in filter)
            {
                var mc = filter.Components2[component];
                var pc = filter.Components1[component];

                pc.Position += mc.Velocity * Time.FrameTime;
            }

            foreach(var component in collisionFilter)
            {
                var pc = collisionFilter.Components1[component];
                var cc = collisionFilter.Components3[component];
                var sc = collisionFilter.Components4[component];
                var size = sc.Texture.GetLocalBounds();
                cc.Body.SetPosition(pc.Position.X, pc.Position.Y);
            }
        }
    }
}
