using Leopotam.Ecs;
using SFML.Graphics;

namespace CarSimulation.ECS.Systems.Interfaces
{
    public interface IRenderSystem : IEcsRunSystem
    {
        RenderWindow Window {get;set;}
    }
}
