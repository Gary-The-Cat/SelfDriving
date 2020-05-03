using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulation.CollisionData.Shapes
{
    public class Rectangle : IShape
    {
        private Vector2f centre;

        private Vector2f halfSize;

        public Rectangle()
        {
            centre = new Vector2f(0, 0);
            halfSize = new Vector2f(0, 0);
        }

        public Rectangle(float centreX, float centreY, float halfSizeX, float halfSizeY)
        {
            centre = new Vector2f(centreX, centreY);
            halfSize = new Vector2f(halfSizeX, halfSizeY);
        }

        public Vector2f GetPosition()
        {
            return centre - halfSize;
        }

        public void SetPosition(Vector2f position)
        {
            centre = position + halfSize;
        }

        public void SetPosition(float x, float y)
        {
            centre.X = x + halfSize.X;
            centre.Y = y + halfSize.Y;
        }

        public Vector2f GetCentre()
        {
            return centre;
        }

        public void SetCentre(Vector2f position)
        {
            centre = position;
        }

        public void SetCentre(float x, float y)
        {
            centre.X = x;
            centre.Y = y;
        }

        public void Move(Vector2f delta)
        {
            centre += delta;
        }

        public Vector2f GetSize()
        {
            return halfSize * 2;
        }

        public void SetSize(Vector2f size)
        {
            halfSize = size / 2;
        }

        public void SetSize(float width, float height)
        {
            halfSize.X = width / 2;
            halfSize.Y = height / 2;
        }

        public Vector2f GetHalfSize()
        {
            return halfSize;
        }

        public float Top()
        {
            return centre.Y - halfSize.Y;
        }

        public float Bottom()
        {
            return centre.Y + halfSize.Y;
        }

        public float Left()
        {
            return centre.X - halfSize.X;
        }

        public float Right()
        {
            return centre.X + halfSize.X;
        }

        public Vector2f Min()
        {
            return centre - halfSize;
        }

        public Vector2f Max()
        {
            return centre + halfSize;
        }

        public bool Intersects(Rectangle r)
        {
            if (this.Left() > r.Right() || r.Left() > this.Right()) return false;
            if (this.Top() > r.Bottom() || r.Top() > this.Bottom()) return false;
            return true;
        }

        public bool Contains(Vector2f point)
        {
            return point.X > Left() && point.X < Right() && point.Y > Top() && point.Y < Bottom();
        }
    }
}
