using SFML.System;

namespace CarSimulation.ECS.Components
{
    public class MovementComponent
    {
        public MovementComponent()
        {
            Velocity = new Vector2f(0, 0);
        }

        public Vector2f Velocity;
    }
}
