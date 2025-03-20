using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellView : MonoBehaviour {
    public TMP_Text valueText;
    public Image backgroundImage;

    private Cell cell;
    private int gridSize;

    public Color startColor = Color.white;
    public Color endColor = Color.red;

    public AdaptiveFontSize adaptiveFontSize;

    /// <summary>
    /// Метод Init, принимающий объект класса Cell, подписывающийся на его события изменения позиции и значения двумя методами UpdateValue и UpdatePosition
    /// </summary>
    /// <param name="newCell"></param>
    /// <param name="gridSize"></param>
    public void Init(Cell newCell, int gridSize) {
        cell = newCell;
        this.gridSize = gridSize;
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;

        UpdateValue(cell.Value);
        UpdatePosition(cell.Position);
    }

    /// <summary>
    /// Выставляет значение текста на префабе равным Math.Pow(Cell.Value, 2)
    /// </summary>
    /// <param name="newValue"></param>
    private void UpdateValue(int newValue) {
        adaptiveFontSize.UpdateNumber(newValue);
        // Вычисляем коэффициент: чем больше newValue, тем больше значение коэффициента
        // Например, для newValue == 2 -> t = 0, для newValue == 2048 -> t = 1
        // Подберите нормировку в зависимости от диапазона значений
        float t = Mathf.Clamp01((Mathf.Log(newValue, 2) - 1) / 10f);

        // Интерполируем между startColor и endColor
        backgroundImage.color = Color.Lerp(startColor, endColor, t);
    }

    /// <summary>
    /// Изменяет позицию префаба в соответствии с позицией клетки на поле.
    /// </summary>
    /// <param name="newPosition"></param>
    private void UpdatePosition(Vector2Int newPosition) {
        float cellSize = 130f;
        float spacing = 10f;

        float totalWidth = (cellSize + spacing) * gridSize - spacing;
        float startX = -totalWidth / 2 + cellSize / 2;
        float startY = totalWidth / 2 - cellSize / 2;

        float xPos = startX + newPosition.x * (cellSize + spacing);
        float yPos = startY - newPosition.y * (cellSize + spacing);

        transform.localPosition = new Vector3(xPos, yPos, 0);
    }
}
