using CarSimulation.ECS.Components;
using CarSimulation.Events;
using Leopotam.Ecs;
using static CarSimulation.Events.Events;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class DestructionSystem : IEcsRunSystem
    {
        EcsWorld world = null;

        EcsFilter<PositionComponent, SpriteComponent, CollisionComponent, DestructableComponent> filter = null;

        public void Run()
        {
            foreach(var component in filter)
            {
                var destructableComponent = filter.Components4[component];
                var collisionComponent = filter.Components3[component];
                if (destructableComponent.ToDestroy)
                {
                    destructableComponent.PreDestroy();
                    world.RemoveEntity(filter.Entities[component]);

                    if(collisionComponent.Type == Entities.EntityType.Brick)
                    {
                        EventManager.Trigger<BrickDestroyedEvent>();
                    }
                }
            }
        }
    }
}
