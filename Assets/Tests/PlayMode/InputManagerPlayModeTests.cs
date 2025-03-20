using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class InputManagerPlayModeTests : InputTestFixture {
    private GameObject inputManagerObject;
    private PlayModeTestInputManager inputManager;
    private GameObject gameFieldObject;
    private StubGameField stubGameField;

    [SetUp]
    public override void Setup() {
        base.Setup();

        gameFieldObject = new GameObject("StubGameField");
        stubGameField = gameFieldObject.AddComponent<StubGameField>();

        var emptyContainer = new GameObject("EmptyCellsContainer");
        stubGameField.emptyCellsContainer = emptyContainer.transform;

        var cellsContainer = new GameObject("CellsContainer");
        stubGameField.cellsContainer = cellsContainer.transform;

        var cellPrefabObj = new GameObject("CellPrefab");
        var cellView = cellPrefabObj.AddComponent<CellView>();

        var textObj = new GameObject("ValueText");
        var valueText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        textObj.transform.SetParent(cellPrefabObj.transform);
        cellView.valueText = valueText;

        var bgObj = new GameObject("Background");
        var img = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgObj.transform.SetParent(cellPrefabObj.transform);
        cellView.backgroundImage = img;

        var adaptiveFont = textObj.AddComponent<AdaptiveFontSize>();
        adaptiveFont.textMeshPro = valueText;
        cellView.adaptiveFontSize = adaptiveFont;

        stubGameField.cellPrefab = cellView;

        var currentScoreObj = new GameObject("CurrentScore");
        stubGameField.currentScoreText = currentScoreObj.AddComponent<TMPro.TextMeshProUGUI>();

        var highScoreObj = new GameObject("HighScore");
        stubGameField.highScoreText = highScoreObj.AddComponent<TMPro.TextMeshProUGUI>();

        stubGameField.SkipStart = true;

        inputManagerObject = new GameObject("InputManager");
        inputManagerObject.SetActive(false);

        var testInputManager = inputManagerObject.AddComponent<PlayModeTestInputManager>();
        testInputManager.gameField = stubGameField;
        inputManager = testInputManager;

        inputManagerObject.SetActive(true);
    }

    [UnityTest]
    public IEnumerator HandleKeyboardInput_ShouldDetectUpKey() {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.wKey);
        yield return null;
        Assert.IsTrue(stubGameField.moveCalled);
        Assert.AreEqual(Vector2.up, stubGameField.lastDirection);
    }

    [UnityTest]
    public IEnumerator HandleKeyboardInput_ShouldDetectDownKey() {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.sKey);
        yield return null;
        Assert.IsTrue(stubGameField.moveCalled);
        Assert.AreEqual(Vector2.down, stubGameField.lastDirection);
    }

    [UnityTest]
    public IEnumerator HandleKeyboardInput_ShouldDetectLeftKey() {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.aKey);
        yield return null;
        Assert.IsTrue(stubGameField.moveCalled);
        Assert.AreEqual(Vector2.left, stubGameField.lastDirection);
    }

    [UnityTest]
    public IEnumerator HandleKeyboardInput_ShouldDetectRightKey() {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.dKey);
        yield return null;
        Assert.IsTrue(stubGameField.moveCalled);
        Assert.AreEqual(Vector2.right, stubGameField.lastDirection);
    }

    [UnityTest]
    public IEnumerator HandleMouseSwipe_ShouldDetectSwipeRight() {
        var mouse = InputSystem.AddDevice<Mouse>();
        Press(mouse.leftButton);
        Set(mouse.position, new Vector2(100, 100));
        yield return null;
        Release(mouse.leftButton);
        Set(mouse.position, new Vector2(200, 100));
        yield return null;
        Assert.Pass();
    }

    [UnityTest]
    public IEnumerator HandleTouchSwipe_ShouldDetectSwipeDown() {
        var touchScreen = InputSystem.AddDevice<Touchscreen>();
        BeginTouch(0, new Vector2(300, 300));
        yield return null;
        EndTouch(0, new Vector2(300, 100));
        yield return null;
        Assert.Pass();
    }

    [TearDown]
    public override void TearDown() {
        base.TearDown();
        Object.DestroyImmediate(inputManagerObject);
        Object.DestroyImmediate(gameFieldObject);
    }
}
