using CarSimulation.ECS.Components;
using Leopotam.Ecs;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class ScreenBoundsEnforcerSystem : IEcsRunSystem
    {
        private EcsWorld world = null;

        private EcsFilter<PositionComponent, SpriteComponent, ScreenBoundsEnforcerComponent> filter = null;

        public void Run()
        {
            if (!Configuration.DebugInput)
            {
                foreach (var component in filter)
                {
                    var pc = filter.Components1[component];
                    var sc = filter.Components2[component];
                    var width = (sc.Texture.GetLocalBounds().Width * sc.Scale);
                    var height = (sc.Texture.GetLocalBounds().Height * sc.Scale);

                    if (pc.Position.X < 0)
                    {
                        pc.Position.X = 0;
                    }

                    if (pc.Position.X > Configuration.Width - width)
                    {
                        pc.Position.X = Configuration.Width - width;
                    }
                }
            }
        }
    }
}
