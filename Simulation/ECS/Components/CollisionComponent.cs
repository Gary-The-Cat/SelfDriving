using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Entities;

namespace CarSimulation.ECS.Components
{
    public class CollisionComponent
    {
        public IShape Body;

        public EntityType Type;

        public bool Deflect = true;

        public float Cooldown = 0;

        public float LifeTime = 0;
    }
}
