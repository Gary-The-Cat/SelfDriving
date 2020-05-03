using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkanoid_SFML.Managers
{
    public class CheckpointManager
    {
        public Vector2f CurrentWaypoint { get; set; }

        public Vector2f LastWaypoint { get; set; }

        public float WaypointTolerance = 100f;

        public int CheckpointsPassed { get; set; } = 0;

        private List<Vector2f> waypoints;

        public CheckpointManager(List<Vector2f> waypoints)
        {
            this.waypoints = waypoints;
            if(waypoints != null)
            this.CurrentWaypoint = waypoints.First();
            this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
        }

        private Vector2f GetLastWaypoint(int currentWaypointIndex)
        {
            var previousWaypointIndex = currentWaypointIndex - 2;
            if(previousWaypointIndex < 0)
            {
                previousWaypointIndex = waypoints.Count() - (1 - previousWaypointIndex);
            }

            return waypoints[previousWaypointIndex];
        }

        public bool Update(Vector2f currentPosition)
        {
            if(Maths.MathHelper.GetDistance(CurrentWaypoint, currentPosition) < WaypointTolerance)
            {
                SetNextWaypoint();
            }

            if (Maths.MathHelper.GetDistance(LastWaypoint, currentPosition) < WaypointTolerance)
            {
                return false;
            }

            return true;
        }

        private void SetNextWaypoint()
        {
            CheckpointsPassed++;

            var currentIndex = waypoints.IndexOf(CurrentWaypoint);
            if (currentIndex + 1 < waypoints.Count())
            {
                CurrentWaypoint = waypoints[currentIndex+ 1];
                this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
            } 
            else
            {
                CurrentWaypoint = waypoints.First();
                this.LastWaypoint = GetLastWaypoint(waypoints.IndexOf(CurrentWaypoint));
            }
        }
    }
}
