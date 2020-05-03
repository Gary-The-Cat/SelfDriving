using Leopotam.Ecs;

namespace CarSimulation.Events
{
    public interface IEntityEvent
    {
        EcsEntity Entity { get; set; } 
    }
}
