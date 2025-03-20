using NUnit.Framework;
using UnityEngine;
using System;

public class CellTests {
    [Test]
    public void SetValue_ShouldUpdateValueAndTriggerEvent() {
        Vector2Int position = new Vector2Int(0, 0);
        Cell cell = new Cell(position, 2);
        bool eventFired = false;
        int eventValue = 0;
        cell.OnValueChanged += (newVal) => {
            eventFired = true;
            eventValue = newVal;
        };
        cell.SetValue(4);

        Assert.AreEqual(4, cell.Value, "Значение клетки должно обновиться до 4.");
        Assert.IsTrue(eventFired, "Событие OnValueChanged должно быть вызвано.");
        Assert.AreEqual(4, eventValue, "В событии должно передаваться новое значение 4.");
    }

    [Test]
    public void SetPosition_ShouldUpdatePositionAndTriggerEvent() {
        Vector2Int initialPosition = new Vector2Int(0, 0);
        Cell cell = new Cell(initialPosition);
        bool eventFired = false;
        Vector2Int eventPosition = Vector2Int.zero;
        cell.OnPositionChanged += (newPos) => {
            eventFired = true;
            eventPosition = newPos;
        };
        Vector2Int newPosition = new Vector2Int(1, 2);
        cell.SetPosition(newPosition);
        Assert.AreEqual(newPosition, cell.Position, "Позиция клетки должна обновиться.");
        Assert.IsTrue(eventFired, "Событие OnPositionChanged должно быть вызвано.");
        Assert.AreEqual(newPosition, eventPosition, "В событии должно передаваться новое значение позиции.");
    }
}
