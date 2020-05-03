using SFML.Graphics;
using static SFML.Window.Keyboard;

namespace CarSimulation
{
    public static class Configuration
    {
        // --------------- Dom & Mike ---------------- //
        // Adds user controllable car
        public static bool IsRace = false;
        
        // Shows car rays & waypoints
        public static bool ShowDebug = true;

        // Writes each frame to file (WARNING: Slow)
        public static bool RecordToFile = false;

        // The maximum amount of time each car can be alive (@ 60fps == 2 minutes)
        public static int MaxRuntime = 7200;
        
        public static bool IsDebugFrameTime = true;
        public static float DebugFrameTime = (1 / 60f);
        public static float Scale = 2f;
        public static uint Height = (uint)(1080 * Scale);
        public static uint Width = (uint)(1920 * Scale);

        // ---------- Leave these ----------- //
        public static bool IsFullScreen = false;
        public static bool DebugInput = false;
        public static bool SoundEnabled = true;
        public static bool MouseInput = false;
        public static float MaxBallSpeed = 1000;
        public static float PaddleMaxSpeed = 1000;

        public static Color Background => new Color(0x23, 0x23, 0x23);
        public static bool AllowCameraMovement => true;
        public static Key PanLeft => Key.A;
        public static Key PanRight => Key.D;
        public static Key PanUp => Key.W;
        public static Key PanDown => Key.S;
        public static Key ZoomIn => Key.Z;
        public static Key ZoomOut => Key.X;
        public static Key RotateRight => Key.Num1;
        public static Key RotateLeft => Key.Num2;

        public static FloatRect SinglePlayer => new FloatRect(0, 0, 1, 1);
        public static FloatRect TwoPlayerLeft => new FloatRect(0, 0, 0.5f, 1);
        public static FloatRect TwoPlayerRight => new FloatRect(0.5f, 0, 0.5f, 1);
        public static FloatRect FourPlayerTopLeft => new FloatRect(0, 0, 0.5f, 0.5f);
        public static FloatRect FourPlayerTopRight => new FloatRect(0.5f, 0, 0.5f, 0.5f);
        public static FloatRect FourPlayerBottomLeft => new FloatRect(0, 0.5f, 0.5f, 0.5f);
        public static FloatRect FourPlayerBottomRight => new FloatRect(0.5f, 0.5f, 0.5f, 0.5f);
    }
}