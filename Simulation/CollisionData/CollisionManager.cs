using CarSimulation.CollisionData.Shapes;
using CarSimulation.ECS.Components;
using CarSimulation.ExtensionMethods;
using SFML.System;

namespace CarSimulation.CollisionData
{
    public static class CollisionManager
    {
        public static Collision CheckCollision(CollisionComponent componentA, CollisionComponent componentB)
        {
            Collision collision = null;
            var shapeA = componentA.Body;
            var shapeB = componentB.Body;

            if (shapeA is Circle && shapeB is Rectangle)
            {
                collision = CheckCollision((Circle)shapeA, (Rectangle)shapeB);
            }

            if (shapeB is Circle && shapeA is Rectangle)
            {
                collision = CheckCollision((Circle)shapeB, (Rectangle)shapeA);
            }

            if (collision != null)
            {
                collision.EntityAType = componentA.Type;
                collision.EntityBType = componentB.Type;
            }
            
            return collision;
        }


        public static Collision CheckCollision(Rectangle a, Rectangle b)
        {
            Collision collision = null;

            // Vector from A to B.
            var n = b.GetCentre() - a.GetCentre();

            // Rectangles collide if they overlap in both x and y axis.
            float xOverlap = a.GetHalfSize().X + b.GetHalfSize().X - MathExtensions.Abs(n.X);
            if (xOverlap > 0)
            {
                float yOverlap = a.GetHalfSize().Y + b.GetHalfSize().Y - MathExtensions.Abs(n.Y);
                if (yOverlap > 0)
                {
                    // Resolve collision along axis of least overlap.
                    if (xOverlap < yOverlap)
                    {
                        collision.Normal = (n.X < 0) ? new Vector2f(-1, 0) : new Vector2f(1, 0);
                        collision.Depth = xOverlap;
                        return collision;
                    }
                    else
                    {
                        collision.Normal = (n.Y < 0) ? new Vector2f(0, -1) : new Vector2f(0, 1);
                        collision.Depth = yOverlap;
                        return collision;
                    }
                }
            }
            return collision;
        }

        public static Collision CheckCollision(Circle a, Circle b)
        {
            Collision collision = null;

            // Vector from A to B.
            var n = b.GetPosition() - a.GetPosition();

            // No collision if distance greater than combined radii.
            float d = n.MagnitudeSquared();
            float r = a.GetRadius() + b.GetRadius();
            if (d > r * r)
            {
                return null;
            }

            // Avoided square root until needed.
            d = MathExtensions.Sqrt(d);

            // Collision resolution.
            if (d == 0)
            {
                // Circles are in the same position, set arbitrary collision normal.
                collision.Normal = new Vector2f(1, 0);
                collision.Depth = a.GetRadius();
            }
            else
            {
                collision.Normal = n / d;
                collision.Depth = r - d;
            }

            return collision;
        }

        public static Collision CheckCollision(Rectangle a, Circle b)
        {
            Collision collision = new Collision();
            // Vector from A to B.
            var n = b.GetPosition() + b.GetHalfSize() - a.GetCentre();

            // Get rectangle-vertex closest to circle center by clamping vector to rectangle bounds.
            var closest = new Vector2f(n.X, n.Y);
            closest.X = MathExtensions.Clamp(closest.X, -a.GetHalfSize().X, a.GetHalfSize().X);
            closest.Y = MathExtensions.Clamp(closest.Y, -a.GetHalfSize().Y, a.GetHalfSize().Y);

            // If clamping vector had no effect, then circle center is inside rectangle.
            bool inside = (n == closest);

            // Recalculate rectangle-vertex closest to circle center.
            if (inside)
            {
                if (MathExtensions.Abs(n.X) > MathExtensions.Abs(n.Y))
                {
                    closest.X = (closest.X > 0) ? a.GetHalfSize().X : -a.GetHalfSize().X;
                }
                else
                {
                    closest.Y = (closest.Y > 0) ? a.GetHalfSize().Y : -a.GetHalfSize().Y;
                }
            }

            // Calculate vector from circle center to closest rectangle-vertex.
            var nn = n - closest;
            float d = nn.MagnitudeSquared();
            float r = b.GetRadius();

            // No collision if vector is greater than circle radius.
            if (d > (r * r) && !inside)
            {
                return null;
            }

            // Avoided square root until needed.
            d = MathExtensions.Sqrt(d);

            // Collision resolution.
            if (inside)
            {
                collision.Normal = -1.0f * nn / d;
                collision.Depth = r + d;
            }
            else
            {
                collision.Normal = nn / d;
                collision.Depth = r - d;
            }

            return collision;
        }

        public static Collision CheckCollision(Circle a, Rectangle b)
        {
            return CheckCollision(b, a);
        }

        public static Vector2f? CheckCollision(Vector2f p0, Vector2f p1, Vector2f p2, Vector2f p3)
        {
            float s1_x = p1.X - p0.X; 
            float s1_y = p1.Y - p0.Y;
            float s2_x = p3.X - p2.X;
            float s2_y = p3.Y - p2.Y;

            float s = (-s1_y * (p0.X - p2.X) + s1_x * (p0.Y - p2.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            float t = (s2_x * (p0.Y - p2.Y) - s2_y * (p0.X - p2.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return new Vector2f(p0.X + (t * s1_x), p0.Y + (t * s1_y));
            }

            return null; // No collision
        }
    }
}
