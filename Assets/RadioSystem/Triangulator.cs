using UnityEngine;

public class Triangulator: MonoBehaviour
{
    // Последняя рассчитанная позиция
    public Vector3 LastEstimatedPosition { get; private set; } = Vector3.zero;
    public Vector3 TrueTarget { get; private set; } = Vector3.zero;

    public void SetTrueTarget(Vector3 target)
    {
        TrueTarget = target;
    }

    // Попытка триангуляции из двух замеров. Возвращает положение и "уверенность" 0..1
    public bool TryTriangulate(Vector3 p1, float az1Deg, Vector3 p2, float az2Deg, out Vector3 estimated, out float confidence)
    {
        estimated = Vector3.zero;
        confidence = 0f;

        // конвертируем к векторам (XZ плоскость)
        Vector3 d1 = AzimuthToDirection(az1Deg);
        Vector3 d2 = AzimuthToDirection(az2Deg);

        // проекция в 2D (XZ)
        Vector2 P1 = new Vector2(p1.x, p1.z);
        Vector2 D1 = new Vector2(d1.x, d1.z).normalized;
        Vector2 P2 = new Vector2(p2.x, p2.z);
        Vector2 D2 = new Vector2(d2.x, d2.z).normalized;

        // if almost parallel -> fail
        float cross = D1.x * D2.y - D1.y * D2.x;
        if (Mathf.Abs(cross) < 1e-3f)
        {
            // почти параллельные
            return false;
        }
        
        float a = D1.x, b = -D2.x, c = D1.y, d = -D2.y;
        float e = P2.x - P1.x, f = P2.y - P1.y;

        float denom = a * d - b * c;
        if (Mathf.Abs(denom) < 1e-6f) return false; // защита

        float t1 = (e * d - b * f) / denom;
        float t2 = (a * f - e * c) / denom;

        Vector2 intersect1 = P1 + D1 * t1;
        Vector2 intersect2 = P2 + D2 * t2;
        Vector2 mid = (intersect1 + intersect2) * 0.5f;

        estimated = new Vector3(mid.x, 0f, mid.y);
        LastEstimatedPosition = estimated;

        // Оценка confidence: чем ближе intersect1 и intersect2 — тем выше
        float separation = Vector2.Distance(intersect1, intersect2);
        float sepNormalized = Mathf.Clamp01(separation / 5.0f); // 5 м — порог
        confidence = 1f - sepNormalized;

        // Дополнительно можно проверить углы относительно реальной цели (если доступна)
        if (TrueTarget != Vector3.zero)
        {
            float err1 = AngleBetweenDirectionToTarget(p1, d1, TrueTarget);
            float err2 = AngleBetweenDirectionToTarget(p2, d2, TrueTarget);
            float angErr = (err1 + err2) * 0.5f;
            // уменьшаем confidence, если большие угловые ошибки
            confidence *= Mathf.Clamp01(1f - (angErr / 45f)); // 45deg жирно
        }

        return true;
    }

    public Vector3 AzimuthToDirection(float azDeg)
    {
        float rad = azDeg * Mathf.Deg2Rad;
        float x = Mathf.Sin(rad);
        float z = Mathf.Cos(rad);
        return new Vector3(x, 0f, z).normalized;
    }

    public float AngleBetweenDirectionToTarget(Vector3 origin, Vector3 dir, Vector3 target)
    {
        Vector3 toTarget = (target - origin);
        toTarget.y = 0f;
        dir.y = 0f;
        if (toTarget.sqrMagnitude < 1e-6f) return 0f;
        float ang = Vector3.Angle(dir, toTarget.normalized);
        return ang;
    }
}
