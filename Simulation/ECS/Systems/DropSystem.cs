using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.System;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class DropSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<DestructableComponent, DroppableComponent> filter = null;

        public void Run()
        {
            foreach(var component in filter)
            {
                var destructableComponent = filter.Components1[component];
                if (destructableComponent.ToDestroy)
                {
                    var droppableComponent = filter.Components2[component];
                    var parentPositionComponent = world.GetComponent<PositionComponent>(droppableComponent.Entity);
                    var spriteComponent = world.GetComponent<SpriteComponent>(droppableComponent.Entity);
                    var size = spriteComponent.Texture.GetLocalBounds();
                    var scale = spriteComponent.Scale;
                    droppableComponent.SpawnChildComponent(
                        world, 
                        droppableComponent.DropObject,
                        new Vector2f(
                            parentPositionComponent.Position.X + size.Width / 2 * scale, 
                            parentPositionComponent.Position.Y + size.Height / 2 * scale));
                }
            }
        }
    }
}
