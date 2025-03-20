using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameField : MonoBehaviour {
    public int gridSize = 4;
    public CellView cellPrefab;
    public GameObject emptyCellPrefab;
    public Transform emptyCellsContainer;
    public Transform cellsContainer;
    public TMP_Text currentScoreText;
    public TMP_Text highScoreText;

    private List<Cell> cells = new List<Cell>();
    private Dictionary<Cell, CellView> cellViews = new Dictionary<Cell, CellView>();

    private int currentScore = 0;
    private int highScore = 0;

    #region Создание и Сохранение

    /// <summary>
    /// Возвращает координаты случайной пустой клетки на поле
    /// </summary>
    public Vector2Int GetEmptyPosition() {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                if (!cells.Exists(c => c.Position == new Vector2Int(x, y))) {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count == 0) return new Vector2Int(-1, -1);
        return emptyPositions[Random.Range(0, emptyPositions.Count)];
    }

    /// <summary>
    /// Создает новую клетку в случайной пустой позиции.
    /// С вероятностью 20% клетка со значением 4, иначе 2.
    /// </summary>
    public void CreateCell() {
        Vector2Int position = GetEmptyPosition();
        if (position == new Vector2Int(-1, -1)) return;

        int value = Random.value < 0.2f ? 4 : 2;
        Cell newCell = new Cell(position, value);
        cells.Add(newCell);

        CellView cellView = Instantiate(cellPrefab, cellsContainer);
        cellView.Init(newCell, gridSize);
        cellViews[newCell] = cellView;

        UpdateScore(value);
    }

    public void UpdateScore(int points) {
        currentScore += points;
        currentScoreText.text = currentScore.ToString();

        if (currentScore > highScore) {
            highScore = currentScore;
            highScoreText.text = highScore.ToString();
        }
    }

    /// <summary>
    /// Пересчитывает счет как сумму значений всех клеток
    /// </summary>
    public void RecalculateScore() {
        int total = 0;
        foreach (Cell cell in cells) {
            total += cell.Value;
        }
        currentScore = total;
        currentScoreText.text = currentScore.ToString();

        if (currentScore > highScore) {
            highScore = currentScore;
            highScoreText.text = highScore.ToString();
        }
    }

    #endregion

    #region Передвижение и слияние

    /// <summary>
    /// Вызывается при вводе, определяет направление движения.
    /// </summary>
    /// <param name="direction">/param>
    public virtual void MoveCells(Vector2 direction) {
        bool moved = false;
        if (direction == Vector2.left) {
            moved = MoveCellsLeft();
        }
        else if (direction == Vector2.right) {
            moved = MoveCellsRight();
        }
        else if (direction == Vector2.up) {
            moved = MoveCellsUp();
        }
        else if (direction == Vector2.down) {
            moved = MoveCellsDown();
        }

        if (moved) {
            CreateCell();
            if (IsGameOver()) {
                GameOver();
                CreateCell();
                CreateCell();
            }
        }
    }

    /// <summary>
    /// Движение влево.
    /// Обрабатываем каждую строку, начиная с левого края.
    /// </summary>
    private bool MoveCellsLeft() {
        bool moved = false;
        for (int y = 0; y < gridSize; y++) {
            List<Cell> rowCells = cells.FindAll(c => c.Position.y == y);
            rowCells.Sort((a, b) => a.Position.x.CompareTo(b.Position.x));
            List<Cell> newRow = new List<Cell>();
            int i = 0;
            while (i < rowCells.Count) {
                if (i < rowCells.Count - 1 && rowCells[i].Value == rowCells[i + 1].Value) {
                    rowCells[i].SetValue(rowCells[i].Value * 2);
                    RemoveCell(rowCells[i + 1]);
                    newRow.Add(rowCells[i]);
                    i += 2;
                    moved = true;
                }
                else {
                    newRow.Add(rowCells[i]);
                    i++;
                }
            }
            for (int newX = 0; newX < newRow.Count; newX++) {
                Cell cell = newRow[newX];
                if (cell.Position.x != newX) {
                    cell.SetPosition(new Vector2Int(newX, y));
                    moved = true;
                }
            }
        }
        if (moved) RecalculateScore();
        return moved;
    }

    /// <summary>
    /// Движение вправо.
    /// Обрабатываем каждую строку, начиная с правого края.
    /// </summary>
    private bool MoveCellsRight() {
        bool moved = false;
        for (int y = 0; y < gridSize; y++) {
            List<Cell> rowCells = cells.FindAll(c => c.Position.y == y);
            rowCells.Sort((a, b) => b.Position.x.CompareTo(a.Position.x));
            List<Cell> newRow = new List<Cell>();

            int i = 0;
            while (i < rowCells.Count) {
                if (i < rowCells.Count - 1 && rowCells[i].Value == rowCells[i + 1].Value) {
                    rowCells[i].SetValue(rowCells[i].Value * 2);
                    RemoveCell(rowCells[i + 1]);
                    newRow.Add(rowCells[i]);
                    i += 2;
                    moved = true;
                }
                else {
                    newRow.Add(rowCells[i]);
                    i++;
                }
            }
            for (int iRow = 0; iRow < newRow.Count; iRow++) {
                int newX = gridSize - 1 - iRow;
                Cell cell = newRow[iRow];
                if (cell.Position.x != newX) {
                    cell.SetPosition(new Vector2Int(newX, y));
                    moved = true;
                }
            }
        }
        if (moved) RecalculateScore();
        return moved;
    }

    /// <summary>
    /// Движение вверх.
    /// Обрабатываем каждую колонку, начиная сверху.
    /// </summary>
    private bool MoveCellsUp() {
        bool moved = false;
        for (int x = 0; x < gridSize; x++) {
            List<Cell> colCells = cells.FindAll(c => c.Position.x == x);
            colCells.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));
            List<Cell> newCol = new List<Cell>();
            int i = 0;
            while (i < colCells.Count) {
                if (i < colCells.Count - 1 && colCells[i].Value == colCells[i + 1].Value) {
                    colCells[i].SetValue(colCells[i].Value * 2);
                    RemoveCell(colCells[i + 1]);
                    newCol.Add(colCells[i]);
                    i += 2;
                    moved = true;
                }
                else {
                    newCol.Add(colCells[i]);
                    i++;
                }
            }
            for (int newY = 0; newY < newCol.Count; newY++) {
                Cell cell = newCol[newY];
                if (cell.Position.y != newY) {
                    cell.SetPosition(new Vector2Int(x, newY));
                    moved = true;
                }
            }
        }
        if (moved) RecalculateScore();
        return moved;
    }

    /// <summary>
    /// Движение вниз.
    /// Обрабатываем каждую колонку, начиная снизу.
    /// </summary>
    private bool MoveCellsDown() {
        bool moved = false;
        for (int x = 0; x < gridSize; x++) {
            List<Cell> colCells = cells.FindAll(c => c.Position.x == x);
            colCells.Sort((a, b) => b.Position.y.CompareTo(a.Position.y));
            List<Cell> newCol = new List<Cell>();

            int i = 0;
            while (i < colCells.Count) {
                if (i < colCells.Count - 1 && colCells[i].Value == colCells[i + 1].Value) {
                    colCells[i].SetValue(colCells[i].Value * 2);
                    RemoveCell(colCells[i + 1]);
                    newCol.Add(colCells[i]);
                    i += 2;
                    moved = true;
                }
                else {
                    newCol.Add(colCells[i]);
                    i++;
                }
            }
            for (int iCol = 0; iCol < newCol.Count; iCol++) {
                int newY = gridSize - 1 - iCol;
                Cell cell = newCol[iCol];
                if (cell.Position.y != newY) {
                    cell.SetPosition(new Vector2Int(x, newY));
                    moved = true;
                }
            }
        }
        if (moved) RecalculateScore();
        return moved;
    }

    #endregion

    #region Утилиты для удаления клеток

    /// <summary>
    /// Удаляет клетку из списка и её визуальное представление.
    /// </summary>
    private void RemoveCell(Cell cell) {
        cells.Remove(cell);
        DestroyCellView(cell);
    }

    /// <summary>
    /// Ищет и уничтожает объект-представление для данной клетки.
    /// </summary>
    private void DestroyCellView(Cell cell) {
        if (cellViews.ContainsKey(cell)) {
            #if UNITY_EDITOR
            DestroyImmediate(cellViews[cell].gameObject);
            #else
            Destroy(cellViews[cell].gameObject);
            #endif
            cellViews.Remove(cell);
        }
    }


    #endregion

    #region Сохранение и загрузка игры

    [System.Serializable]
    public class GameData {
        public List<CellData> cells;
        public int currentScore;
        public int highScore;
    }

    [System.Serializable]
    public class CellData {
        public int x;
        public int y;
        public int value;
    }

    public void SaveGame() {
        GameData data = new GameData();
        data.cells = new List<CellData>();
        foreach (Cell cell in cells) {
            data.cells.Add(new CellData {
                x = cell.Position.x,
                y = cell.Position.y,
                value = cell.Value
            });
        }
        data.currentScore = currentScore;
        data.highScore = highScore;

        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.dat";
        FileStream file = File.Create(path);
        bf.Serialize(file, data);
        file.Close();

        Debug.Log("Game saved to " + path);
    }

    public void LoadGame() {
        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            foreach (Cell cell in new List<Cell>(cells)) {
                RemoveCell(cell);
            }
            cells.Clear();
            cellViews.Clear();

            foreach (CellData cd in data.cells) {
                Cell newCell = new Cell(new Vector2Int(cd.x, cd.y), cd.value);
                cells.Add(newCell);
                CellView cellView = Instantiate(cellPrefab, cellsContainer);
                cellView.Init(newCell, gridSize);
                cellViews[newCell] = cellView;
            }

            currentScore = data.currentScore;
            highScore = data.highScore;
            currentScoreText.text = currentScore.ToString();
            highScoreText.text = highScore.ToString();

            Debug.Log("Game loaded from " + path);
        }
        else {
            Debug.Log("No save file found at " + path);
        }
    }

    #endregion

    #region Проигрыш и сброс сессии

    /// <summary>
    /// Проверяет есть ли свободные клетки или возможность объединения
    /// </summary>
    public bool IsGameOver() {
        if (GetEmptyPosition() != new Vector2Int(-1, -1))
            return false;

        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                Cell current = cells.Find(c => c.Position == new Vector2Int(x, y));
                if (current == null) continue;

                Cell right = cells.Find(c => c.Position == new Vector2Int(x + 1, y));
                Cell down = cells.Find(c => c.Position == new Vector2Int(x, y + 1));
                Cell left = cells.Find(c => c.Position == new Vector2Int(x - 1, y));
                Cell up = cells.Find(c => c.Position == new Vector2Int(x, y - 1));

                if ((right != null && right.Value == current.Value) ||
                    (up != null && up.Value == current.Value)) {
                    return false;
                }

                if ((right != null && right.Value == current.Value) ||
                    (down != null && down.Value == current.Value)) {
                    return false;
                }

                if ((left != null && left.Value == current.Value) ||
                    (up != null && up.Value == current.Value)) {
                    return false;
                }

                if ((left != null && left.Value == current.Value) ||
                    (down != null && down.Value == current.Value)) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Обрабатывает ситуацию проигрыша: сохраняет результат, сбрасывает поле и запускает новую игру
    /// </summary>
    public void GameOver() {
        Debug.Log("Game Over!");

        if (currentScore > highScore) {
            highScore = currentScore;
            SaveGame();
        }

        foreach (Cell cell in new List<Cell>(cells)) {
            RemoveCell(cell);
        }
        cells.Clear();
        cellViews.Clear();

        currentScore = 0;
        currentScoreText.text = currentScore.ToString();
    }

    #endregion

    public void OnApplicationQuit() {
        SaveGame();
    }

    public void OnApplicationPause(bool pause) {
        if (pause) {
            SaveGame();
        }
    }

    public virtual void Start() {
        LoadGame();
        CreateEmptyCells();

        if (cells == null || cells.Count == 0) {
            CreateCell();
            CreateCell();
        }
    }

    private void CreateEmptyCells() {
        for (int i = 0; i < gridSize * gridSize; i++) {
            Instantiate(emptyCellPrefab, emptyCellsContainer);
        }
    }
}

