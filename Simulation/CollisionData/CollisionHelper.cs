using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using SFML.System;

namespace CarSimulation.CollisionData
{
    public static class CollisionHelper
    {
        public static Rectangle GetCollisionRectangle(Vector2f position, SpriteComponent spriteComponent)
        {
            var bounds = spriteComponent.Texture.GetLocalBounds();
            var scale = spriteComponent.Scale;

            return new Rectangle(
                position.X + bounds.Width * scale / 2,
                position.Y + bounds.Height * scale / 2,
                bounds.Width * scale / 2,
                bounds.Height * scale / 2);
        }

        public static Circle GetCollisionCircle(Vector2f position, SpriteComponent spriteComponent)
        {
            var bounds = spriteComponent.Texture.GetLocalBounds();
            var scale = spriteComponent.Scale;

            return new Circle(
                position.X + bounds.Width * scale / 2,
                position.Y + bounds.Height * scale / 2,
                bounds.Width * scale / 2);
        }
    }
}
