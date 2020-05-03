using SFML.System;
using System;

namespace Arkanoid_SFML.Maths
{
    public static class MathHelper
    {
        public static float GetDistance(Vector2f a, Vector2f b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
