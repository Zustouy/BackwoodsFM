using UnityEngine;

public class RealisticAerodynamics : MonoBehaviour
{
    public float airDensity = 1.225f; // Плотность воздуха (кг/м^3) на уровне моря
    public float dragCoefficient = 0.47f; // Коэффициент сопротивления
    public float liftCoefficient = 0.3f; // Коэффициент подъемной силы
    public float area = 1.0f; // Площадь поперечного сечения объекта (м^2)
    public float angularDragCoefficient = 0.1f; // Коэффициент углового сопротивления
    public Vector3 randomTorqueRange = new Vector3(1f, 1f, 1f); // Диапазон случайного вращения

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.maxAngularVelocity = 10f;
        ApplyRandomTorque();
    }

    void FixedUpdate()
    {
        ApplyAerodynamicForces();
        ApplyAngularDrag();
    }

    void ApplyAerodynamicForces()
    {
        // Скорость объекта
        Vector3 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed < 0.01f)
            return; // Если скорость очень мала, не применять силы

        // Угол атаки
        float angleOfAttack = Vector3.Angle(velocity, transform.forward) * Mathf.Deg2Rad;

        // Изменение коэффициентов сопротивления и подъемной силы в зависимости от угла атаки
        float currentDragCoefficient = dragCoefficient * (1 + 0.5f * Mathf.Sin(angleOfAttack));
        float currentLiftCoefficient = liftCoefficient * Mathf.Sin(2 * angleOfAttack);

        // Расчет силы сопротивления
        Vector3 dragForce = 0.5f * airDensity * speed * speed * currentDragCoefficient * area * -velocity.normalized;

        // Расчет подъемной силы
        Vector3 liftDirection = Vector3.Cross(velocity, transform.right).normalized; // Перпендикулярная к направлению движения
        Vector3 liftForce = 0.5f * airDensity * speed * speed * currentLiftCoefficient * area * liftDirection;

        // Применение сил к объекту
        rb.AddForce(dragForce);
        rb.AddForce(liftForce);

        // Расчет момента силы сопротивления для вращения объекта
        Vector3 aerodynamicTorque = Vector3.Cross(transform.forward, velocity).normalized * speed * currentDragCoefficient * area;
        rb.AddTorque(aerodynamicTorque);
    }

    void ApplyAngularDrag()
    {
        // Угловая скорость объекта
        Vector3 angularVelocity = rb.angularVelocity;
        float angularSpeed = angularVelocity.magnitude;

        if (angularSpeed < 0.01f)
            return; // Если угловая скорость очень мала, не применять угловое сопротивление

        // Расчет углового сопротивления
        Vector3 angularDragTorque = angularDragCoefficient * angularSpeed * angularSpeed * -angularVelocity.normalized;

        // Применение углового сопротивления
        rb.AddTorque(angularDragTorque*angularDragCoefficient);
    }

    void ApplyRandomTorque()
    {
        Vector3 randomTorque = new Vector3(
            Random.Range(-randomTorqueRange.x, randomTorqueRange.x),
            Random.Range(-randomTorqueRange.y, randomTorqueRange.y),
            Random.Range(-randomTorqueRange.z, randomTorqueRange.z)
        );

        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }
}
