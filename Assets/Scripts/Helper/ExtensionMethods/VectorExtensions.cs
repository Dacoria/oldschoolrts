using Unity.Mathematics;
using UnityEngine;

public static class VectorExtensions
{
    public static bool IsSameXAndZ(this Vector3 a, Vector3 b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool IsSameVector3(this Vector3 a, Vector3 b)
    {
        var sqrDiff = Vector3.SqrMagnitude(a - b);
        return sqrDiff < 0.001;
    }

    public static bool IsEmptyVector(this Vector3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
    public static bool IsAlmostEmptyVector(this Vector3 a)
    {
        return Mathf.Abs(a.x) <= 0.1 && Mathf.Abs(a.y) <= 0.1 && Mathf.Abs(a.z) <= 0.1;
    }

    public static Vector3 MultiplyVector(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Absolute(this Vector3 a)
    {
        return new Vector3(math.abs(a.x), math.abs(a.y), math.abs(a.z));
    }

    public static Bounds Copy(this Bounds b)
    {
        return new Bounds(new Vector3(b.center.x, b.center.y, b.center.z), new Vector3(b.size.x, b.size.y, b.size.z));
    }

    public static Vector2 Random(int minRange, int maxRange)
    {
        var randomX = UnityEngine.Random.Range(minRange, maxRange);
        var randomZ = UnityEngine.Random.Range(minRange, maxRange);

        var positiveNegativeX = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        var positiveNegativeZ = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

        var x = Mathf.RoundToInt(randomX * positiveNegativeX);
        var z = Mathf.RoundToInt(randomZ * positiveNegativeZ);

        return new Vector2(x, z);
    }
}