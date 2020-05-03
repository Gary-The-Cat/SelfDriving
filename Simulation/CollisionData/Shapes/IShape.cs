using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulation.CollisionData.Shapes
{
    public interface IShape
    {
        void SetPosition(float x, float y);

        Vector2f GetPosition();

        void SetPosition(Vector2f position);
    }
}
