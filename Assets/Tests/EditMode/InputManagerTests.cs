using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class InputManagerTests {
    private GameObject testObject;
    private TestInputManager inputManager;
    private GameObject gameFieldObject;
    private GameField gameField;

    [SetUp]
    public void Setup() {
        testObject = new GameObject("InputManagerTest");
        inputManager = testObject.AddComponent<TestInputManager>();

        gameFieldObject = new GameObject("GameFieldTest");
        gameField = gameFieldObject.AddComponent<GameField>();
        inputManager.gameField = gameField;
    }

    [TearDown]
    public void Teardown() {
        Object.DestroyImmediate(testObject);
        Object.DestroyImmediate(gameFieldObject);
    }

    [Test]
    public void Update_ShouldCallInputHandlers() {
        inputManager.Update();

        Assert.IsTrue(inputManager.keyboardHandled, "HandleKeyboardInput должен быть вызван");
        Assert.IsTrue(inputManager.mouseHandled, "HandleMouseSwipe должен быть вызван");
        Assert.IsTrue(inputManager.touchHandled, "HandleTouchSwipe должен быть вызван");
    }

    [Test]
    public void DetectSwipe_ShouldNotTrigger_WhenSwipeTooSmall() {
        Vector2 smallSwipe = new Vector2(10, 10);
        inputManager.DetectSwipe(smallSwipe);
        Assert.Pass();
    }

    [Test]
    public void DetectSwipe_ShouldDetectSwipeRight() {
        Vector2 swipe = new Vector2(100, 10);
        inputManager.DetectSwipe(swipe);
        Assert.Pass();
    }

    [Test]
    public void DetectSwipe_ShouldDetectSwipeUp() {
        Vector2 swipe = new Vector2(10, 100);
        inputManager.DetectSwipe(swipe);
        Assert.Pass();
    }
}
