using CarSimulation.ExtensionMethods;
using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.Graphics;
using CarSimulation.ECS.Systems.Interfaces;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    class SpriteRenderSystem : IRenderSystem
    {
        private EcsWorld world = null;

        private EcsFilter<SpriteComponent, PositionComponent> filter = null;

        public RenderWindow Window { get; set; }

        public SpriteRenderSystem()
        {
        }

        public void Run()
        {
            foreach(var component in filter)
            {
                var spriteComponent = filter.Components1[component];
                var positionComponent = filter.Components2[component];

                Window.Draw(spriteComponent.Texture, spriteComponent.Scale, positionComponent.Position);
            }
        }
    }
}
