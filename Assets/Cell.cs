using System;
using UnityEngine;

public class Cell {
    // Хранит позицию клетки на поле
    public Vector2Int Position { get; private set; }

    // Хранит значение клетки
    public int Value { get; private set; }

    // Событие вызывается при изменении значения клетки
    public event Action<int> OnValueChanged;

    // Событие вызывается при изменении позиции клетки
    public event Action<Vector2Int> OnPositionChanged;

    public Cell(Vector2Int position, int value = 2) {
        Position = position;
        Value = value;
    }

    public void SetValue(int newValue) {
        Value = newValue;
        OnValueChanged?.Invoke(Value);
    }

    public void SetPosition(Vector2Int newPosition) {
        Position = newPosition;
        OnPositionChanged?.Invoke(Position);
    }
}
