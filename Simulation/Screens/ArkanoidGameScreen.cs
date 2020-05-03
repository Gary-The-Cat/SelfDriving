using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarSimulation.CollisionData;
using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using CarSimulation.ECS.Systems;
using CarSimulation.Events;
using CarSimulation.Helpers;
using CarSimulation.Managers;
using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;
using static CarSimulation.Events.Events;

namespace CarSimulation.Screens
{
    public class ArkanoidGameScreen : Screen
    {
        private ScoreManager scoreManager;

        private List<string> levels;

        public ArkanoidGameScreen(RenderWindow window, ScoreManager scoreManager, FloatRect config) : base(window, config)
        {
            this.scoreManager = scoreManager;

            this.levels = new List<string>();

            this.LoadLevels();

            this.SpawnSystems();

            var paddle = this.SpawnCharacter();

            this.SpawnBall(paddle);

            this.SpawnBlocks();

            this.SpawnWalls();

            base.Initialize();
        }

        private void LoadLevels()
        {
            levels = Directory.GetFiles($"Resources\\Levels").ToList();
        }

        private void SpawnWalls()
        {
            // LEFT WALL
            WallCreationHelper.CreateSideWall(world, Side.Left);
            
            // RIGHT WALL
            WallCreationHelper.CreateSideWall(world, Side.Right);

            // TOP WALL
            WallCreationHelper.CreateTopWall(world);
        }

        private void SpawnBlocks()
        {
            var random = new Random();
            var level = random.Next(3);

            switch (level)
            {
                case 0:
                    BrickCreationHelper.BasicLevel(world, 5 + random.Next(15), 5 + random.Next(10));
                    break;
                case 1:
                    BrickCreationHelper.StaggeredLevel(world, 5 + random.Next(15), 5 + random.Next(10));
                    break;
                case 2:
                    BrickCreationHelper.FromImage(world, levels[random.Next(levels.Count())], 16);
                    break;
            }
        }

        private void SpawnSystems()
        {
            AddUpdateSystem(new UserInputSystem());
            AddUpdateSystem(new MovementSystem());

            if (Configuration.ShowDebug)
            {
                AddRenderSystem(new DebugRenderSystem());
            }

            AddUpdateSystem(new ScreenBoundsEnforcerSystem());
            AddUpdateSystem(new AttachedSystem());
            AddUpdateSystem(new LaunchSystem());
            AddUpdateSystem(new CollisionSystem());
            AddUpdateSystem(new GrabBallSystem());
            AddUpdateSystem(new DropSystem());
            AddUpdateSystem(new DestructionSystem());
            AddUpdateSystem(new DeathSystem());
            AddRenderSystem(new SpriteRenderSystem());
        }

        private void DoSomething()
        {
        }

        private EcsEntity SpawnCharacter()
        {
            var paddle = world.CreateEntityWith<PositionComponent, MovementComponent, SpriteComponent, UserInputComponent, ScreenBoundsEnforcerComponent, CollisionComponent, GrabBallComponent>(
                out var positionComponent,
                out _,
                out var spriteComponent,
                out _,
                out _,
                out var collisionComponent,
                out _);

            var texture = new Texture("Resources/player.png");
            spriteComponent.Texture = new Sprite(texture);
            spriteComponent.Scale = 0.5f;

            var chatacterSize = spriteComponent.Texture.GetGlobalBounds();
            positionComponent.Position.Y = Configuration.Height - chatacterSize.Height;
            positionComponent.Position.X = Configuration.Width / 2 - chatacterSize.Width * spriteComponent.Scale / 2;

            collisionComponent.Body = CollisionHelper.GetCollisionRectangle(positionComponent.Position, spriteComponent);
            collisionComponent.Type = ECS.Entities.EntityType.Paddle;

            return paddle;
        }

        private void SpawnBall(EcsEntity paddle)
        {
            var ball = world.CreateEntityWith<PositionComponent, MovementComponent, SpriteComponent, AttachableComponent, CollisionComponent, LaunchComponent, DeathComponent>(
                out var positionComponent,
                out _,
                out var ballSpriteComponent,
                out var attachableComponent,
                out var collisionComponent,
                out _,
                out _);

            ballSpriteComponent.Texture = new Sprite(new Texture("Resources/ball.png"));
            attachableComponent.AttachedEntity = paddle;
            var paddleComponent = world.GetComponent<SpriteComponent>(paddle);
            var paddlePositionComponent = world.GetComponent<PositionComponent>(paddle);
            var paddleBounds = paddleComponent.Texture.GetLocalBounds();
            var ballSale = 0.15f;
            var ballWidth = ballSpriteComponent.Texture.GetLocalBounds().Width * ballSale;
            positionComponent.Position = new Vector2f(
                paddlePositionComponent.Position.X + paddleBounds.Width * paddleComponent.Scale / 2 - ballWidth / 2,
                paddlePositionComponent.Position.Y - paddleBounds.Height * paddleComponent.Scale);
            attachableComponent.Attached = true;
            attachableComponent.OffsetFromEntity = new Vector2f(
                paddleBounds.Width * paddleComponent.Scale / 2 - ballWidth / 2, 
                -paddleBounds.Height * paddleComponent.Scale + ballWidth / 2);
            ballSpriteComponent.Scale = ballSale;
            collisionComponent.Body = CollisionHelper.GetCollisionCircle(new Vector2f(0, 0), ballSpriteComponent);
            collisionComponent.Type = ECS.Entities.EntityType.Ball;
        }
    }
}
