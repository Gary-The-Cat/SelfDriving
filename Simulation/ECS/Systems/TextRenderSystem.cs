using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems.Interfaces;
using CarSimulation.ExtensionMethods;
using Leopotam.Ecs;
using SFML.Graphics;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class TextRenderSystem : IRenderSystem
    {
        private EcsWorld world = null;

        private EcsFilter<TextComponent, PositionComponent> filter = null;

        public RenderWindow Window { get; set; }

        public TextRenderSystem()
        {
        }
        
        public void Run()
        {
            foreach(var component in filter)
            {
                var textComponent = filter.Components1[component];
                var positionComponent = filter.Components2[component];

                Window.DrawString(textComponent.Text, positionComponent.Position);
            }
        }
    }
}
