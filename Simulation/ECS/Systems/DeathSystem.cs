using CarSimulation.ECS.Components;
using CarSimulation.Events;
using Leopotam.Ecs;
using static CarSimulation.Events.Events;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class DeathSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<PositionComponent, AttachableComponent> filter = null;

        private EcsFilter<PositionComponent, GrabBallComponent> paddleFilter = null;

        public void Run()
        {
            foreach(var component in filter)
            {
                var pc = filter.Components1[component];
                if(pc.Position.Y > Configuration.Height)
                {
                    var ac = filter.Components2[component];
                    ac.Attached = true;
                    var parentPosition = paddleFilter.Components1[0].Position;
                    pc.Position = new SFML.System.Vector2f(
                         parentPosition.X + ac.OffsetFromEntity.X,
                        parentPosition.Y + ac.OffsetFromEntity.Y);

                    EventManager.Trigger(new DeathOccuredEvent { Entity = filter.Entities[component] });
                }
            }
        }
    }
}
