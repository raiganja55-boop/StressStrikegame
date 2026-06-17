using System;

public class Test
{
    // Simplified Quaternion implementation for testing
    public struct Quat
    {
        public float x, y, z, w;
        public Quat(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static Quat Identity = new Quat(0, 0, 0, 1);
        public static Quat Inverse(Quat q) { return new Quat(-q.x, -q.y, -q.z, q.w); }
        public static Quat operator *(Quat a, Quat b) {
            return new Quat(
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y + a.y * b.w + a.z * b.x - a.x * b.z,
                a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
            );
        }
        public void ToAngleAxis(out float angle, out float ax, out float ay, out float az) {
            angle = (float)(2.0 * Math.Acos(w)) * 180f / (float)Math.PI;
            float s = (float)Math.Sqrt(1.0 - w * w);
            if (s < 0.001f) { ax = 1; ay = 0; az = 0; }
            else { ax = x / s; ay = y / s; az = z / s; }
        }
        public override string ToString() => $"({x:F2}, {y:F2}, {z:F2}, {w:F2})";
    }

    public static void Main()
    {
        // Target: 0 degrees
        Quat initial = Quat.Identity;
        // Current: +10 degrees around X. sin(5) = 0.087, cos(5) = 0.996
        Quat current = new Quat(0.087f, 0, 0, 0.996f);
        
        Quat delta = initial * Quat.Inverse(current);
        delta.ToAngleAxis(out float angle, out float ax, out float ay, out float az);
        
        if (angle > 180f) angle -= 360f;
        
        Console.WriteLine($"Delta: {delta}");
        Console.WriteLine($"Angle: {angle:F2}, Axis: ({ax:F2}, {ay:F2}, {az:F2})");
        
        float torqueX = ax * angle;
        Console.WriteLine($"Torque X: {torqueX:F2}");
    }
}
