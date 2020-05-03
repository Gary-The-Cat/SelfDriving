using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.System;
using SFML.Window;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    class UserInputSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<UserInputComponent, MovementComponent, PositionComponent> filter = null;

        private Vector2i PreviousMousePosition = new Vector2i(0, 0);

        public void Run()
        {
            foreach (var component in filter)
            {
                if(!Configuration.DebugInput && !Configuration.MouseInput)
                {
                    var movementComponent = filter.Components2[component];

                    // Process user input.
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    {
                        movementComponent.Velocity = new Vector2f(-Configuration.PaddleMaxSpeed, 0);
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    {
                        movementComponent.Velocity = new Vector2f(Configuration.PaddleMaxSpeed, 0);
                    }
                    else
                    {
                        movementComponent.Velocity = new Vector2f(0, 0);
                    }
                }
                else if(Configuration.MouseInput)
                {
                    var changeInPosition = PreviousMousePosition - Mouse.GetPosition();
                    PreviousMousePosition = Mouse.GetPosition();
                    var positionComponent = filter.Components3[component];
                    positionComponent.Position.X -= changeInPosition.X;
                }
                else
                {
                    var pc = filter.Components3[component];

                    var position = new Vector2f(Mouse.GetPosition().X, Mouse.GetPosition().Y);
                    position.X = (position.X - 1920f / 3840 * (int)Configuration.Width);
                    position.Y = (position.Y / 2160 * (int)Configuration.Height);
                    pc.Position = position;
                }
            }
        }
    }
}
