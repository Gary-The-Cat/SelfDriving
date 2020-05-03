using System;
using CarSimulation.ECS.Components;
using CarSimulation.Events;
using Leopotam.Ecs;
using SFML.System;
using static CarSimulation.Events.Events;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class GrabBallSystem : IEcsRunSystem
    {
        
        private EcsWorld world = null;

        private EcsFilter<GrabBallComponent> grabBallFilter = null;

        public GrabBallSystem()
        {
            EventManager.Listen<PowerupCollected>(ProcessPowerup);

            EventManager.Listen<BallPaddleCollision>(ProcessBallPaddleCollision);

            EventManager.Listen<DeathOccuredEvent>(DeathOccured);
        }

        private void DeathOccured(DeathOccuredEvent e)
        {
            foreach(var component in grabBallFilter)
            {
                var grabBallComponent = grabBallFilter.Components1[component];
                grabBallComponent.IsPowerupEnabled = false;
            }
        }

        private void ProcessBallPaddleCollision(BallPaddleCollision collision)
        {
            var ballAttachComponent = world.GetComponent<AttachableComponent>(collision.BallEntity);
            var paddleGrabComponent = world.GetComponent<GrabBallComponent>(collision.PaddleEntity);

            if (!ballAttachComponent.Attached && paddleGrabComponent.IsPowerupEnabled)
            {
                // Get the current paddle and ball position
                var ballPositionComponent = world.GetComponent<PositionComponent>(collision.BallEntity);
                var paddlePositionComponent = world.GetComponent<PositionComponent>(collision.PaddleEntity);
                var paddleComponent = world.GetComponent<SpriteComponent>(collision.PaddleEntity);
                var paddleBounds = paddleComponent.Texture.GetLocalBounds();
                var ballSprite = world.GetComponent<SpriteComponent>(collision.BallEntity);
                var ballWidth = ballSprite.Texture.GetLocalBounds().Width * ballSprite.Scale;

                ballAttachComponent.OffsetFromEntity = new Vector2f(
                    ballPositionComponent.Position.X - paddlePositionComponent.Position.X,
                    -paddleBounds.Height * paddleComponent.Scale + ballWidth / 2);

                ballAttachComponent.AttachedEntity = collision.PaddleEntity;
                ballAttachComponent.Attached = true;
            }
        }

        private void ProcessPowerup(PowerupCollected powerup)
        {
            foreach(var component in grabBallFilter)
            {
                grabBallFilter.Components1[component].IsPowerupEnabled = true;
            }
        }


        public void Run()
        {
        }
    }
}
