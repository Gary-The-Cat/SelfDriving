using CarSimulation.ECS.Components;
using CarSimulation.ExtensionMethods;
using Leopotam.Ecs;
using SFML.System;
using SFML.Window;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class AttachedSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<PositionComponent, AttachableComponent> filter = null;

        public void Run()
        {
            foreach(var component in filter)
            {
                var attachComponent = filter.Components2[component];
                if(attachComponent.Attached)
                {
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                    {
                        attachComponent.Attached = false;
                    }

                    var positionComponent = filter.Components1[component];
                    var parentPosition = world.GetComponent<PositionComponent>(attachComponent.AttachedEntity).Position;
                    
                    var expectedPosition = new Vector2f(
                        parentPosition.X + attachComponent.OffsetFromEntity.X,
                        parentPosition.Y + attachComponent.OffsetFromEntity.Y);

                    var offset = positionComponent.Position.Distance(expectedPosition);
                    if (offset < 400)
                    {
                        positionComponent.Position = new Vector2f(
                            parentPosition.X + attachComponent.OffsetFromEntity.X,
                            parentPosition.Y + attachComponent.OffsetFromEntity.Y);
                    }
                }
            }
        }
    }
}
