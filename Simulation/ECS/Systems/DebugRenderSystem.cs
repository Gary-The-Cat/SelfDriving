using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems.Interfaces;
using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;

namespace CarSimulation.ECS.Systems
{
    [EcsInject]
    public class DebugRenderSystem : IRenderSystem
    {
        public RenderWindow Window { get; set; }
        
        private EcsFilter<CollisionComponent> filter = null;

        public void Run()
        {
            foreach(var item in filter)
            {
                var collisionComponent = filter.Components1[item];

                if(collisionComponent.Body is Circle circle)
                {
                    // Outline
                    var circleShape = new CircleShape(circle.GetRadius());
                    circleShape.Position = circle.GetPosition();
                    circleShape.OutlineThickness = 2;
                    circleShape.OutlineColor = Color.White;
                    circleShape.FillColor = Color.Transparent;
                    Window.Draw(circleShape);

                    // Centre
                    circleShape = new CircleShape(1);
                    circleShape.Position = circle.GetPosition();
                    circleShape.OutlineThickness = 2;
                    circleShape.OutlineColor = Color.Red;
                    circleShape.FillColor = Color.Transparent;
                    Window.Draw(circleShape);
                }

                if (collisionComponent.Body is Rectangle rectangle)
                {
                    // Outline
                    var rectangleShape = new RectangleShape(rectangle.GetSize());
                    rectangleShape.Position = rectangle.GetPosition();
                    rectangleShape.OutlineThickness = 2;
                    rectangleShape.OutlineColor = Color.White;
                    rectangleShape.FillColor = Color.Transparent;
                    Window.Draw(rectangleShape);

                    // Centre
                    rectangleShape = new RectangleShape(new Vector2f(1,1));
                    rectangleShape.Position = rectangle.GetPosition();
                    rectangleShape.OutlineThickness = 2;
                    rectangleShape.OutlineColor = Color.Red;
                    rectangleShape.FillColor = Color.Transparent;
                    Window.Draw(rectangleShape);
                }
            }
        }
    }
}
