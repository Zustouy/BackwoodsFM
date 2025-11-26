using UnityEngine;

public static class TriangulationUtils
{
    /// <summary>
    /// Азимут (градусы, 0 = север, по часовой) → направление в мире (XZ плоскость)
    /// </summary>
    public static Vector3 AzimuthToDirection(float azDeg)
    {
        float rad = azDeg * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad);
        float z = Mathf.Cos(rad);
        return new Vector3(x, 0f, z); // уже нормализовано (Sin² + Cos² = 1)
    }

    /// <summary>
    /// Угол между текущим направлением антенны и направлением на цель (в градусах)
    /// </summary>
    public static float AngleToTarget(Vector3 antennaPos, Vector3 antennaDir, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - antennaPos;
        toTarget.y = 0f; // игнорируем высоту

        if (toTarget.sqrMagnitude < 0.01f) return 0f;

        toTarget.Normalize();
        antennaDir.y = 0f;
        antennaDir.Normalize();

        // Vector3.Angle всегда возвращает 0..180
        return Vector3.Angle(antennaDir, toTarget);
    }

    /// <summary>
    /// То же самое, но с круговым углом (signed): положительный = цель справа, отрицательный = слева
    /// </summary>
    public static float SignedAngleToTarget(Vector3 antennaPos, Vector3 antennaDir, Vector3 targetPos)
    {
        Vector3 toTarget = (targetPos - antennaPos);
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.01f) return 0f;

        return Vector3.SignedAngle(antennaDir, toTarget.normalized, Vector3.up);
    }

    /// <summary>
    /// Прогресс от 0 (плохо) до 1 (идеально) для индикатора
    /// </summary>
    public static float GetAccuracyProgress(float angleError, float maxAcceptableError = 15f)
    {
        return Mathf.Clamp01(1f - angleError / maxAcceptableError);
    }
}