namespace CarSimulation.TimeTracker
{
    public static class Time
    {
        public static float TotalTime = 0;

        public static float PreviousTotalTime = 0;

        public static float FrameTime = 0;

        public static void TimeStep(float time)
        {
            TotalTime += time;
            FrameTime = time;
        }
    }
}
