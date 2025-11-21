using UnityEngine;
using UnityEngine.EventSystems;

public class Radio : MonoBehaviour
{
    public float rotationSpeed = 10f; // Чувствительность вращения
    public float minAngle = -90f; // Минимальный угол вращения
    public float maxAngle = 90f; // Максимальный угол вращения
    public Vector3 rotationAxis = Vector3.forward; // Ось вращения (например, Vector3.up или Vector3.forward)

    private float currentAngle;
    private Vector3 previousMousePosition;
    private bool isDragging = false;

    void Start()
    {
        // Инициализация текущего угла на основе начального вращения
        currentAngle = transform.localEulerAngles.z; // Замените Z на нужную ось
    }

    void OnMouseDown()
    {
        // Вызывается при нажатии кнопки мыши на коллайдере объекта
        isDragging = true;
        // Запоминаем начальную позицию мыши в мировых координатах
        previousMousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // Вызывается при отпускании кнопки мыши
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            // Получаем текущую позицию мыши
            Vector3 currentMousePosition = Input.mousePosition;
            // Вычисляем разницу (дельту) в позиции мыши между кадрами
            Vector3 deltaMousePosition = currentMousePosition - previousMousePosition;

            // Определяем, как движение мыши влияет на вращение.
            // Для "крутилки" обычно используется горизонтальное движение (X)
            float rotationAmount = deltaMousePosition.x * rotationSpeed * Time.deltaTime;

            // Обновляем текущий угол
            currentAngle += rotationAmount;

            // Ограничиваем угол в заданном диапазоне с помощью Mathf.Clamp
            // Примечание: Clamp работает лучше для углов в диапазоне 0-360,
            // или же можно использовать более сложную логику для углов вроде -90/90.
            // Для простоты примера оставим так, как есть.
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

            // Применяем вращение к объекту вокруг заданной оси
            // Quaternion.Euler создает вращение из углов Эйлера
            transform.localRotation = Quaternion.Euler(rotationAxis * currentAngle);

            // Обновляем предыдущую позицию мыши для следующего кадра
            previousMousePosition = currentMousePosition;
        }
    }
}