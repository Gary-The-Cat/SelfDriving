using System;
using CarSimulation.ExtensionMethods;
using SFML.System;

namespace CarSimulation.CollisionData.Shapes
{
    public class Circle : IShape
    {
        private float radius;

        private Vector2f center;

        public Circle()
        {
            radius = 0;
            center = new Vector2f(0, 0);
        }

        public Circle(float x, float y, float radius)
        {
            this.radius = radius;
            center = new Vector2f(x, y);
        }

        public void SetPosition(float x, float y)
        {
            center.X = x;
            center.Y = y;
        }
        
        public void SetPosition(Vector2f position)
        {
            this.center = position;
        }

        public Vector2f GetPosition()
        {
            return center;
        }

        public Vector2f GetUpperLeftPosition()
        {
            return new Vector2f(center.X - radius, center.Y - radius);
        }

        public void Move(Vector2f delta)
        {
            this.center += delta;
        }

        public float GetRadius()
        {
            return radius;
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public bool Contains(Vector2f point)
        {
            return center.DistanceSquared(point) < radius * radius;
        }

        public  Vector2f GetHalfSize()
        {
            return new Vector2f(radius, radius);
        }
    }
}
