using UnityEngine;

public class StubGameField : GameField {
    public bool moveCalled = false;
    public Vector2 lastDirection;
    public bool SkipStart = false;

    public override void MoveCells(Vector2 direction) {
        moveCalled = true;
        lastDirection = direction;
    }

    public override void Start() {
        if (SkipStart) return;
        base.Start();
    }
}
