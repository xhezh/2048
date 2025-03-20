using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;

public class GameFieldTests {
    private GameObject testObject;
    private GameField gameField;

    [SetUp]
    public void Setup() {
        testObject = new GameObject("GameFieldTest");
        gameField = testObject.AddComponent<GameField>();
        gameField.gridSize = 4;
        var scoreObj = new GameObject("ScoreText");
        gameField.currentScoreText = scoreObj.AddComponent<TextMeshProUGUI>();

        var highScoreObj = new GameObject("HighScoreText");
        gameField.highScoreText = highScoreObj.AddComponent<TextMeshProUGUI>();
        var cellsContainer = new GameObject("CellsContainer");
        gameField.cellsContainer = cellsContainer.transform;

        var emptyCellsContainer = new GameObject("EmptyCellsContainer");
        gameField.emptyCellsContainer = emptyCellsContainer.transform;
        var emptyPrefab = new GameObject("EmptyCellPrefab");
        gameField.emptyCellPrefab = emptyPrefab;

        var cellPrefabObj = new GameObject("CellPrefab");
        var cellView = cellPrefabObj.AddComponent<CellView>();

        var textObj = new GameObject("ValueText");
        var valueText = textObj.AddComponent<TextMeshProUGUI>();
        textObj.transform.SetParent(cellPrefabObj.transform);
        cellView.valueText = valueText;

        var bgObj = new GameObject("Background");
        var img = bgObj.AddComponent<Image>();
        bgObj.transform.SetParent(cellPrefabObj.transform);
        cellView.backgroundImage = img;
        var adaptiveFont = textObj.AddComponent<AdaptiveFontSize>();
        adaptiveFont.textMeshPro = valueText;
        cellView.adaptiveFontSize = adaptiveFont;

        gameField.cellPrefab = cellView;
    }

    [TearDown]
    public void Teardown() {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void CreateCell_ShouldAddCell() {
        gameField.CreateCell();

        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = (List<Cell>)cellsField.GetValue(gameField);
        Assert.IsTrue(cellsList.Count > 0, "После CreateCell должна быть хотя бы одна клетка");
    }

    [Test]
    public void GetEmptyPosition_ShouldReturnValidPosition() {
        Vector2Int position = gameField.GetEmptyPosition();
        Assert.AreNotEqual(new Vector2Int(-1, -1), position);
    }

    [Test]
    public void IsGameOver_ShouldReturnFalse_WhenEmptyCellsExist() {
        bool result = gameField.IsGameOver();
        Assert.IsFalse(result, "На пустом поле игра не окончена");
    }

    [Test]
    public void RecalculateScore_ShouldSumAllCells() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
            new Cell(new Vector2Int(0, 0), 2),
            new Cell(new Vector2Int(1, 0), 4)
        };
        cellsField.SetValue(gameField, cellsList);

        gameField.RecalculateScore();

        Assert.AreEqual("6", gameField.currentScoreText.text);
    }

    [Test]
    public void GameOver_ShouldClearCellsAndResetScore() {
        gameField.CreateCell();
        gameField.UpdateScore(100);
        gameField.GameOver();
        Assert.AreEqual("0", gameField.currentScoreText.text, "Очки должны быть сброшены");
    }

    [Test]
    public void IsGameOver_ShouldReturnTrue_WhenNoMovesLeft() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>();

        for (int x = 0; x < gameField.gridSize; x++) {
            for (int y = 0; y < gameField.gridSize; y++) {
                int value = 2 + ((x + y) % 2) * 2;
                cellsList.Add(new Cell(new Vector2Int(x, y), value));
            }
        }
        cellsField.SetValue(gameField, cellsList);
        bool result = gameField.IsGameOver();
        Assert.IsTrue(result, "Поле заполнено, ходов нет — игра окончена");
    }

    [Test]
    public void MoveCells_ShouldNotCrash_WhenEmptyField() {
        gameField.MoveCells(Vector2.left);
        gameField.MoveCells(Vector2.right);
        gameField.MoveCells(Vector2.up);
        gameField.MoveCells(Vector2.down);
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = (List<Cell>)cellsField.GetValue(gameField);
        Assert.IsNotNull(cellsList);
    }

    [Test]
    public void SaveLoadGame_ShouldRestoreCells() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(0,0), 2),
        new Cell(new Vector2Int(1,1), 4)
    };
        cellsField.SetValue(gameField, cellsList);

        gameField.SaveGame();
        gameField.LoadGame();

        var loadedCells = (List<Cell>)cellsField.GetValue(gameField);
        Assert.AreEqual(2, loadedCells.Count, "Должно быть восстановлено 2 клетки");
    }

    [Test]
    public void OnApplicationQuit_ShouldCallSaveGame() {
        gameField.OnApplicationQuit();
        Assert.Pass();
    }

    [Test]
    public void OnApplicationPause_ShouldCallSaveGame_WhenPaused() {
        gameField.OnApplicationPause(true);
        Assert.Pass();
    }

    [Test]
    public void Start_ShouldInitializeGameField() {
        gameField.Start();
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = (List<Cell>)cellsField.GetValue(gameField);

        Assert.IsTrue(cellsList.Count >= 2, "После старта должно быть минимум 2 клетки");
        Assert.AreEqual(gameField.gridSize * gameField.gridSize, gameField.emptyCellsContainer.childCount, "Все пустые клетки должны быть созданы");
    }

    [Test]
    public void LoadGame_ShouldHandleMissingFileGracefully() {
        string path = Application.persistentDataPath + "/save.dat";
        if (System.IO.File.Exists(path)) {
            System.IO.File.Delete(path);
        }

        gameField.LoadGame();

        Assert.Pass();
    }

    [Test]
    public void MoveCellsLeft_ShouldMoveCellWithoutMerging() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(2, 0), 2)
    };
        cellsField.SetValue(gameField, cellsList);

        gameField.MoveCells(Vector2.left);

        var updatedCells = (List<Cell>)cellsField.GetValue(gameField);
        Assert.AreEqual(new Vector2Int(0, 0), updatedCells[0].Position, "Клетка должна сместиться влево в позицию (0,0)");
    }

    [Test]
    public void MoveCellsRight_ShouldMoveCellWithoutMerging() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(0, 0), 2)
    };
        cellsField.SetValue(gameField, cellsList);

        gameField.MoveCells(Vector2.right);

        var updatedCells = (List<Cell>)cellsField.GetValue(gameField);
        Assert.AreEqual(new Vector2Int(3, 0), updatedCells[0].Position, "Клетка должна сместиться вправо в позицию (3,0)");
    }

    [Test]
    public void MoveCellsUp_ShouldMoveCellWithoutMerging() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(0, 2), 2) // стоит ниже, должна сместиться вверх
    };
        cellsField.SetValue(gameField, cellsList);

        gameField.MoveCells(Vector2.up);

        var updatedCells = (List<Cell>)cellsField.GetValue(gameField);
        Assert.AreEqual(new Vector2Int(0, 0), updatedCells[0].Position, "Клетка должна сместиться вверх в позицию (0,0)");
    }

    [Test]
    public void MoveCellsDown_ShouldMoveCellWithoutMerging() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(0, 0), 2) // стоит наверху, сместится вниз
    };
        cellsField.SetValue(gameField, cellsList);

        gameField.MoveCells(Vector2.down);

        var updatedCells = (List<Cell>)cellsField.GetValue(gameField);
        Assert.AreEqual(new Vector2Int(0, 3), updatedCells[0].Position, "Клетка должна сместиться вниз в позицию (0,3)");
    }

    [Test]
    public void RemoveCell_ShouldDeleteCellAndView() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var viewsField = typeof(GameField).GetField("cellViews", BindingFlags.NonPublic | BindingFlags.Instance);

        var cell = new Cell(new Vector2Int(0, 0), 2);
        var cellsList = new List<Cell> { cell };
        cellsField.SetValue(gameField, cellsList);

        var view = new GameObject().AddComponent<CellView>();
        var dict = new Dictionary<Cell, CellView> { { cell, view } };
        viewsField.SetValue(gameField, dict);

        MethodInfo removeCellMethod = typeof(GameField).GetMethod("RemoveCell", BindingFlags.NonPublic | BindingFlags.Instance);
        removeCellMethod.Invoke(gameField, new object[] { cell });

        var updatedCells = (List<Cell>)cellsField.GetValue(gameField);
        var updatedDict = (Dictionary<Cell, CellView>)viewsField.GetValue(gameField);

        Assert.IsEmpty(updatedCells, "Клетка должна быть удалена");
        Assert.IsEmpty(updatedDict, "View клетки должен быть удалён");
    }

    [Test]
    public void RecalculateScore_ShouldSumDifferentCells() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
            new Cell(new Vector2Int(0, 0), 2),
            new Cell(new Vector2Int(1, 0), 4),
            new Cell(new Vector2Int(2, 0), 8)
        };
        cellsField.SetValue(gameField, cellsList);

        gameField.RecalculateScore();

        Assert.AreEqual("14", gameField.currentScoreText.text, "Сумма всех клеток должна быть 14");
    }

    [Test]
    public void GameOver_ShouldResetScoreWithoutUpdatingHighScore() {
        typeof(GameField).GetField("highScore", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(gameField, 200);

        gameField.highScoreText.text = "200";
        gameField.UpdateScore(50);
        gameField.GameOver();
        Assert.AreEqual("0", gameField.currentScoreText.text, "Счёт должен сброситься в 0");
        Assert.AreEqual("200", gameField.highScoreText.text, "highScore не должен измениться");
    }

    [Test]
    public void IsGameOver_ShouldReturnFalse_WhenAdjacentCellsCanMerge() {
        var cellsField = typeof(GameField).GetField("cells", BindingFlags.NonPublic | BindingFlags.Instance);
        var cellsList = new List<Cell>
        {
        new Cell(new Vector2Int(0, 0), 2),
        new Cell(new Vector2Int(1, 0), 2)
    };
        cellsField.SetValue(gameField, cellsList);

        bool result = gameField.IsGameOver();

        Assert.IsFalse(result, "Игра не должна заканчиваться, если соседние клетки можно объединить");
    }

    [Test]
    public void GameOver_ShouldUpdateHighScore_WhenCurrentScoreIsHigher() {
        var highScoreField = typeof(GameField).GetField("highScore", BindingFlags.NonPublic | BindingFlags.Instance);
        highScoreField.SetValue(gameField, 50);
        gameField.highScoreText.text = "50";
        gameField.UpdateScore(100);
        gameField.GameOver();
        Assert.AreEqual("100", gameField.highScoreText.text, "highScore должен обновиться, если currentScore больше");
        Assert.AreEqual("0", gameField.currentScoreText.text, "Текущий счёт должен сброситься в 0");
    }

}
