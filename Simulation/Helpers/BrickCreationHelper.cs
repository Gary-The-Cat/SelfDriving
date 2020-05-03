using CarSimulation.CollisionData;
using CarSimulation.ECS.Components;
using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;
using System;

namespace CarSimulation.Helpers
{
    public static class BrickCreationHelper
    {
        public static float HeightOffset = 120;
        public static float BrickWidth => 160 * Scale;
        public static float BrickHeight => 80 * Scale;
        public static float Scale = 0.4f;

        public static void FromImage(EcsWorld world, string filePath, uint sampleRate)
        {
            Image image = new Image(filePath);

            // Calculate Scale
            var imageWidth = image.Size.X;
            var stepSize = imageWidth / sampleRate;
            var desiredBrickWidth = Configuration.Width / sampleRate;
            var random = new Random();
            Scale = desiredBrickWidth/160f;
            bool spawnPossible = true;

            for (uint x = 0; x < imageWidth; x+= stepSize)
            {
                spawnPossible = true;
                uint yPixel = 0;
                for (uint y = 0; y < image.Size.Y * 2; y++)
                {
                    var pixel = image.GetPixel(x, yPixel);
                    if(pixel.A > 50)
                    {
                        var spawn = spawnPossible && random.NextDouble() > 0.2;
                        AddBrick(world, (x / stepSize) * BrickWidth, HeightOffset + y * BrickHeight, Scale, spawn, pixel);
                        spawnPossible &= !spawn;
                    }

                    if(y % 2 == 1)
                    {
                        yPixel++;
                    }
                }
            }
        }

        public static void BasicLevel(EcsWorld world, int bricksWide, int bricksDeep)
        {
            var desiredBrickWidth = Configuration.Width / bricksWide;
            var random = new Random();
            Scale = desiredBrickWidth / 160f;

            for (uint x = 0; x < bricksWide; x++)
            {
                for (uint y = 0; y < bricksDeep; y++)
                {
                    var shade = random.Next(128);
                    var spawn = random.NextDouble() < 1.0 / bricksWide;
                    AddBrick(world, x * BrickWidth, HeightOffset + y * BrickHeight, Scale, spawn, new Color((byte)(128 + shade), (byte)(128 + shade), (byte)(128 + shade)));
                }
            }
        }

        public static void StaggeredLevel(EcsWorld world, int bricksWide, int bricksDeep)
        {
            var desiredBrickWidth = Configuration.Width / bricksWide;
            var random = new Random();
            Scale = desiredBrickWidth / 160f;
            for (uint x = 0; x < bricksWide; x++)
            {
                for (uint y = 0; y < bricksDeep; y++)
                {
                    var shade = random.Next(128);
                    if (y % 2 == 0)
                    {
                        if(x < bricksWide-1)
                        {
                            var spawn = random.NextDouble() < 1.0 / bricksWide;
                            AddBrick(
                                world,
                                x * BrickWidth + BrickWidth / 2, 
                                HeightOffset + y * BrickHeight, 
                                Scale,
                                spawn,
                               new Color((byte)(128 + shade), (byte)(128 + shade), (byte)(128 + shade)));
                        }
                    }
                    else
                    {
                        var spawn = random.NextDouble() < 1.0 / bricksWide;
                        AddBrick(
                            world, 
                            x * BrickWidth, 
                            HeightOffset + y * BrickHeight, 
                            Scale, 
                            spawn,
                            new Color((byte)(128 + shade), (byte)(128 + shade), (byte)(128 + shade)));
                    }
                }
            }
        }

        private static void AddBrick(EcsWorld world, float x, float y, float scale, bool addDrop, Color tint)
        {
            var brick = world.CreateEntityWith<PositionComponent, SpriteComponent, CollisionComponent, DestructableComponent, DroppableComponent>(
                       out var positionComponent,
                       out var spriteComponent,
                       out var collisionComponent,
                       out var destructableComponent,
                       out var droppableComponent);

            var position = new Vector2f(x, y);
            positionComponent.Position = position;
            spriteComponent.Scale = scale;
            var texture = new Texture("Resources/Brick.png");
            spriteComponent.Texture = new Sprite(texture);
            spriteComponent.Texture.Color = tint;
            if (addDrop)
            {
                droppableComponent.SpawnChildComponent = SpawnChild;
            }
            else
            {
                droppableComponent.SpawnChildComponent = DoNothing;
            }

            droppableComponent.Entity = brick;
            droppableComponent.DropObject = "Resources/ballPowerup.png";
            destructableComponent.PreDestroy = () =>
            {
                spriteComponent.Texture = null;
                collisionComponent.Body = null;
                droppableComponent.SpawnChildComponent = null;
            };

            collisionComponent.Body = CollisionHelper.GetCollisionRectangle(
                position,
                spriteComponent);

            collisionComponent.Type = ECS.Entities.EntityType.Brick;
        }

        private static void DoNothing(EcsWorld arg1, string arg2, Vector2f arg3)
        { }

        private static void SpawnChild(EcsWorld world, string filePath, Vector2f position)
        {
            _ = world.CreateEntityWith<PositionComponent, MovementComponent, SpriteComponent, CollisionComponent, DestructableComponent, PowerupComponent>(
                        out var positionComponent,
                        out var movementComponent,
                        out var spriteComponent,
                        out var collisionComponent,
                        out var destructableComponent,
                        out var powerupComponent);
            
            var texture = new Texture(filePath);
            spriteComponent.Texture = new Sprite(texture);
            movementComponent.Velocity = new Vector2f(0, 80);

            spriteComponent.Scale = 0.25f;

            var size = spriteComponent.Texture.GetLocalBounds();
            var scale = spriteComponent.Scale;

            var pos = new Vector2f(position.X - size.Width / 2 * scale, position.Y - size.Height / 2 * scale);

            positionComponent.Position = pos;

            collisionComponent.Body = CollisionHelper.GetCollisionCircle(
                position,
                spriteComponent);

            collisionComponent.Type = ECS.Entities.EntityType.PowerUp;
            collisionComponent.Deflect = false;
            collisionComponent.Cooldown = 0.5f;

            destructableComponent.PreDestroy = () =>
            {
                spriteComponent.Texture = null;
                collisionComponent.Body = null;
            };

            destructableComponent.ToDestroy = false;
        }
    }
}
