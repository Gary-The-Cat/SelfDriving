using CarSimulation.ECS.Systems.Interfaces;
using CarSimulation.ViewTools;
using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;

namespace CarSimulation.Screens
{
    public class Screen
    {
        public Camera Camera { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDraw { get; set; }

        internal RenderWindow window;
        internal EcsWorld world;
        private EcsSystems updateSystems;
        private EcsSystems renderSystems;

        public Screen (RenderWindow window, FloatRect configuration)
        {
            this.window = window;

            Camera = new Camera(configuration);

            world = new EcsWorld();
            updateSystems = new EcsSystems(world);
            renderSystems = new EcsSystems(world);

            IsUpdate = true;
            IsDraw = true;
        }

        public void AddRenderSystem(IRenderSystem system)
        {
            system.Window = this.window;
            renderSystems.Add(system);
        }

        public void AddUpdateSystem(IEcsRunSystem system)
        {
            updateSystems.Add(system);
        }

        public virtual void Update(float deltaT)
        {
            //Camera.Update();
            updateSystems.Run();
        }

        public virtual void Draw(float deltaT)
        {
            renderSystems.Run();
        }

        public virtual void InitializeScreen()
        {

        }

        public void SetInactive()
        {
            IsUpdate = false;
            IsDraw = false;
        }

        public void SetActive()
        {
            IsUpdate = true;
            IsDraw = true;
        }

        public void SetUpdateInactive()
        {
            IsUpdate = false;
        }

        public void SetDrawInactive()
        {
            IsDraw = false;
        }

        public void SetUpdateActive()
        {
            IsUpdate = true;
        }

        public void SetDrawActive()
        {
            IsDraw = true;
        }

        internal void Initialize()
        {
            updateSystems.Initialize();
            renderSystems.Initialize();
        }
    }
}
