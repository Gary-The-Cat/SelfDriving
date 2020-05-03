using System;
using CarSimulation.CollisionData;
using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using CarSimulation.ECS.Entities;
using CarSimulation.Events;
using CarSimulation.ExtensionMethods;
using Leopotam.Ecs;
using SFML.System;
using static CarSimulation.Events.Events;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class CollisionSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<CollisionComponent> filter = null;
        
        private EcsFilter<PositionComponent, CollisionComponent, WallComponent> wallFilter = null;

        // ---------- Paddle Feel Variables ------------ //
        private static float DifferenceScale = 0.6f;
        private static float BallMovementScale = 0.8f;
        private static float PaddleMovementScale = 0.6f;

        public void Run()
        {
            for (int a = 0; a < filter.Components1.Length; a++)
            {
                var collisionComponentA = filter.Components1[a];

                if (collisionComponentA == null)
                {
                    break;
                }

                collisionComponentA.LifeTime += TimeTracker.Time.FrameTime;

                if (collisionComponentA.Cooldown > collisionComponentA.LifeTime)
                {
                    continue;
                }

                    for (int b = a + 1; b < filter.Components1.Length; b++)
                {
                    var collisionComponentB = filter.Components1[b];

                    if (collisionComponentB == null || collisionComponentB.Body == null)
                    {
                        break;
                    }

                    if (collisionComponentB.Cooldown > collisionComponentB.LifeTime)
                    {
                        continue;
                    }

                    var collision = CollisionManager.CheckCollision(collisionComponentA, collisionComponentB);
                    if(collision != null)
                    {
                        collision.EntityA = filter.Entities[a];
                        collision.EntityB = filter.Entities[b];

                        ProcessCollision(collision);
                    }
                }
            }
        }

        private void ProcessCollision(Collision collision)
        {
            if(collision == null)
            {
                return;
            }

            // Paddle Collision
            if(collision.EntityAType == EntityType.Paddle || collision.EntityBType == EntityType.Paddle)
            {
                // Paddle Ball Collision
                if(collision.EntityAType == EntityType.Ball || collision.EntityBType == EntityType.Ball)
                {
                    ProcessPaddleBallCollision(collision);
                }

                // Paddle Powerup Collision
                if (collision.EntityAType == EntityType.PowerUp|| collision.EntityBType == EntityType.PowerUp)
                {
                    ProcessPaddlePowerupCollision(collision);
                }
            }
            else
            {
                // All other collisions
                var collisionComponentA = world.GetComponent<CollisionComponent>(collision.EntityA);
                var collisionComponentB = world.GetComponent<CollisionComponent>(collision.EntityB);

                if (collisionComponentA.Deflect && collisionComponentB.Deflect)
                {
                    var movementComponentA = world.GetComponent<MovementComponent>(collision.EntityA);
                    var positionComponentA = world.GetComponent<PositionComponent>(collision.EntityA);
                    var movementComponentB = world.GetComponent<MovementComponent>(collision.EntityB);
                    var positionComponentB = world.GetComponent<PositionComponent>(collision.EntityB);

                    if (movementComponentA != null)
                    {
                        movementComponentA.Velocity = -(2 * collision.Normal.Dot(movementComponentA.Velocity) * collision.Normal - movementComponentA.Velocity);
                        positionComponentA.Position += collision.Normal * collision.Depth;
                        collisionComponentA.Body.SetPosition(positionComponentA.Position);
                    }

                    if(movementComponentB != null)
                    {
                        movementComponentB.Velocity = -(2 * collision.Normal.Dot(movementComponentB.Velocity) * collision.Normal - movementComponentB.Velocity);
                        positionComponentB.Position += collision.Normal * collision.Depth;
                        collisionComponentB.Body.SetPosition(positionComponentA.Position);
                    }
                }


                var destructableComponentA = world.GetComponent<DestructableComponent>(collision.EntityA);
                var destructableComponentB = world.GetComponent<DestructableComponent>(collision.EntityB);

                if (destructableComponentA != null)
                {
                    destructableComponentA.ToDestroy = true;
                }

                if (destructableComponentB != null)
                {
                    destructableComponentB.ToDestroy = true;
                }
            }
        }

        private void ProcessPaddlePowerupCollision(Collision collision)
        {
            var paddleEntity = collision.EntityAType == EntityType.Paddle ? collision.EntityA : collision.EntityB;
            var powerUpEntity = collision.EntityAType == EntityType.PowerUp ? collision.EntityA : collision.EntityB;

            var powerUpDestructableComponent = world.GetComponent<DestructableComponent>(powerUpEntity);
            var powerUpComponent = world.GetComponent<PowerupComponent>(powerUpEntity);

            powerUpDestructableComponent.ToDestroy = true;

            EventManager.Trigger(new PowerupCollected { Type = powerUpComponent.Type });
        }

        private void ProcessPaddleBallCollision(Collision collision)
        {
            var paddleEntity = collision.EntityAType == EntityType.Paddle ? collision.EntityA : collision.EntityB;
            var ballEntity = collision.EntityAType == EntityType.Ball ? collision.EntityA : collision.EntityB;

            var ballMovementComponent = world.GetComponent<MovementComponent>(ballEntity);
            var ballPositionComponent = world.GetComponent<PositionComponent>(ballEntity);
            var ballCollisionComponent = world.GetComponent<CollisionComponent>(ballEntity);
            ballMovementComponent.Velocity = -(2 * collision.Normal.Dot(ballMovementComponent.Velocity) * collision.Normal - ballMovementComponent.Velocity);
            ballPositionComponent.Position += collision.Normal * collision.Depth;
            ballCollisionComponent.Body.SetPosition(ballPositionComponent.Position);

            // It hit the paddle.
            var paddlePositionComponent = world.GetComponent<PositionComponent>(paddleEntity);
            var paddleSpriteComponent = world.GetComponent<SpriteComponent>(paddleEntity);
            var paddleMovementComponent = world.GetComponent<MovementComponent>(paddleEntity);
            var scale = paddleSpriteComponent.Scale;
            var spriteSize = paddleSpriteComponent.Texture.GetLocalBounds();
            var paddlePosition = new Vector2f(
                paddlePositionComponent.Position.X + spriteSize.Width * scale / 2,
                paddlePositionComponent.Position.Y + spriteSize.Height * scale / 2);

            var difference = ballPositionComponent.Position - paddlePosition;

            difference.X = difference.X / 2;
            difference.Y = difference.Y * 2;

            var n = (
                difference * DifferenceScale +
                ballMovementComponent.Velocity * BallMovementScale +
                paddleMovementComponent.Velocity * PaddleMovementScale).Normalize() * Math.Min(700 + (TimeTracker.Time.TotalTime / 50), Configuration.MaxBallSpeed);


            ballMovementComponent.Velocity += n;

            if(ballMovementComponent.Velocity.Magnitude() > Configuration.MaxBallSpeed)
            {
                ballMovementComponent.Velocity = ballMovementComponent.Velocity.Normalize();
                ballMovementComponent.Velocity *= Configuration.MaxBallSpeed;
            }

            EventManager.Trigger(new BallPaddleCollision
            {
                BallEntity = ballEntity,
                PaddleEntity = paddleEntity
            });
        }
    }
}
