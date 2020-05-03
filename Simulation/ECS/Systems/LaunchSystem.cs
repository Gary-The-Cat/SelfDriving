using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.System;
using SFML.Window;
using System;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class LaunchSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<MovementComponent, LaunchComponent> filter = null;

        private Random random = new Random();

        public void Run()
        {
            foreach(var component in filter)
            {
                var launchComponent = filter.Components2[component];
                var movementComponent = filter.Components1[component];

                if(Keyboard.IsKeyPressed(Keyboard.Key.Space) && launchComponent.CanLaunch)
                {
                    launchComponent.CanLaunch = false;

                    var angle = -Math.PI / 4 - Math.PI * random.NextDouble() / 2;
                    var x = Math.Cos(angle) * 500;
                    var y = Math.Sin(angle) * 500;
                    movementComponent.Velocity = new Vector2f((float)0, (float)y);
                }
            }
        }
    }
}
