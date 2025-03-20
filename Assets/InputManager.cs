using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public GameField gameField;

    private Vector2 swipeStart;
    private bool swipeStarted = false;

    public void Update() {
        HandleKeyboardInput();
        HandleMouseSwipe();
        HandleTouchSwipe();
    }

    /// <summary>
    /// Обрабатываем ввод с клавиатуры (WASD и стрелки)
    /// </summary>
    protected virtual void HandleKeyboardInput() {
        if (Keyboard.current == null || gameField == null) return;

        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) {
            Debug.Log("Input: Up");
            gameField.MoveCells(Vector2.up);
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame) {
            Debug.Log("Input: Down");
            gameField.MoveCells(Vector2.down);
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) {
            Debug.Log("Input: Left");
            gameField.MoveCells(Vector2.left);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) {
            Debug.Log("Input: Right");
            gameField.MoveCells(Vector2.right);
        }
    }


    /// <summary>
    /// Обрабатываем свайп мышью.
    /// При нажатии левой кнопки мыши фиксируем стартовую позицию,
    /// при отпускании — вычисляем смещение и определяем направление.
    /// </summary>
    protected virtual void HandleMouseSwipe() {
        if (Mouse.current == null || gameField == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            swipeStart = Mouse.current.position.ReadValue();
            swipeStarted = true;
        }
        if (swipeStarted && Mouse.current.leftButton.wasReleasedThisFrame) {
            Vector2 swipeEnd = Mouse.current.position.ReadValue();
            Vector2 swipeDelta = swipeEnd - swipeStart;
            DetectSwipe(swipeDelta);
            swipeStarted = false;
        }
    }

    /// <summary>
    /// Обрабатываем свайп на сенсорном экране.
    /// Используем тот же алгоритм, что и для мыши.
    /// </summary>
    protected virtual void HandleTouchSwipe() {
        if (Touchscreen.current == null || gameField == null) return;

        if (Touchscreen.current != null) {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
                swipeStart = Touchscreen.current.primaryTouch.position.ReadValue();
                swipeStarted = true;
            }
            if (swipeStarted && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) {
                Vector2 swipeEnd = Touchscreen.current.primaryTouch.position.ReadValue();
                Vector2 swipeDelta = swipeEnd - swipeStart;
                DetectSwipe(swipeDelta);
                swipeStarted = false;
            }
        }
    }

    /// <summary>
    /// Определяет направление свайпа и вызывает движение клеток.
    /// Если длина свайпа меньше порога (например, 50 пикселей) — не реагируем.
    /// </summary>
    /// <param name="swipeDelta">Вектор смещения свайпа</param>
    public void DetectSwipe(Vector2 swipeDelta) {
        if (swipeDelta.magnitude < 50) { // порог чтобы случайные движения не обрабатывались как свайп
            return;
        }
        // Определяем по какому направлению свайп доминирует
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) {
            if (swipeDelta.x > 0) {
                Debug.Log("Input: Swipe Right");
                gameField.MoveCells(Vector2.right);
            }
            else {
                Debug.Log("Input: Swipe Left");
                gameField.MoveCells(Vector2.left);
            }
        }
        else {
            if (swipeDelta.y > 0) {
                Debug.Log("Input: Swipe Up");
                gameField.MoveCells(Vector2.up);
            }
            else {
                Debug.Log("Input: Swipe Down");
                gameField.MoveCells(Vector2.down);
            }
        }
    }
}
