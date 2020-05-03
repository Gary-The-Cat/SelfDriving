using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.System;

namespace CarSimulation.Helpers
{
    public enum Side
    {
        Left, 
        Right,
        Top
    }

    public static class WallCreationHelper
    {
        public static void CreateSideWall(EcsWorld world, Side side)
        {
            _ = world.CreateEntityWith<PositionComponent, CollisionComponent, WallComponent>(
                       out _,
                       out var leftCollisionComponent,
                       out var leftWallComponent);

            var xOffset = side == Side.Left ? -50 : (int)Configuration.Width + 50;
            leftCollisionComponent.Body = new Rectangle(xOffset, Configuration.Height / 2, 50, Configuration.Height / 2);
            leftCollisionComponent.Type = ECS.Entities.EntityType.Wall;
            leftWallComponent.Side = side;
        }

        public static void CreateTopWall(EcsWorld world)
        {
            _ = world.CreateEntityWith<PositionComponent, CollisionComponent, WallComponent>(
           out var topPositionComponent,
           out var topCollisionComponent,
           out var topWallComponent);

            topPositionComponent.Position = new Vector2f(-50, -50);
            topCollisionComponent.Body = new Rectangle(Configuration.Width / 2, -50, Configuration.Width, 55);
            topCollisionComponent.Type = ECS.Entities.EntityType.Wall;
            topWallComponent.Side = Side.Top;
        }
    }
}
