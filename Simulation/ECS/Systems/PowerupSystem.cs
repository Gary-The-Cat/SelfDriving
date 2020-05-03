using CarSimulation.ECS.Components;
using CarSimulation.Events;
using Leopotam.Ecs;
using static CarSimulation.Events.Events;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class PowerupSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<GrabBallComponent, AttachableComponent> grabBallFilter = null;

        private EcsFilter<SplitBallComponent, PositionComponent, MovementComponent> splitBallFilter = null;

        private EcsFilter<UnstoppableBallComponent, CollisionComponent> filter = null;

        public PowerupSystem()
        {
            EventManager.Listen<PowerupCollected>(ProcessPowerup);
        }

        private void ProcessPowerup(PowerupCollected powerup)
        {
            if(powerup.Type == PowerupType.GrabBall)
            {

            }
        }

        public void Run()
        {
        }
    }
}
