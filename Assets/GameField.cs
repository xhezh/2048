using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameField : MonoBehaviour {
    public int gridSize = 4;
    public CellView cellPrefab;
    private List<Cell> cells = new List<Cell>(); // Объекты Cell
    public GameObject emptyCellPrefab;
    public Transform emptyCellsContainer;
    public Transform cellsContainer;
    public TMP_Text currentScoreText;
    public TMP_Text highScoreText;

    private int currentScore = 0;
    private int highScore = 2048;

    /// <summary>
    /// Возвращает координаты случайной пустой клетки на поле
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetEmptyPosition() {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                if (!cells.Exists(c => c.Position == new Vector2Int(x, y))) {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count == 0) return new Vector2Int(-1, -1); // Нет пустых позиций
        return emptyPositions[Random.Range(0, emptyPositions.Count)];
    }

    /// <summary>
    /// Создает новую клетку в случайной пустой позиции на поле со значением 1 с вероятностью 90% и 2 с 10%. Добавляет эту клетку в список всех клеток, создаёт для нее префаб с компонентом CellView и инициализирует CellView
    /// </summary>
    public void CreateCell() {
        Vector2Int position = GetEmptyPosition();
        if (position == new Vector2Int(-1, -1)) return;

        int value = Random.value < 0.9f ? 2 : 4;
        Cell newCell = new Cell(position, value);
        cells.Add(newCell);

        CellView cellView = Instantiate(cellPrefab, cellsContainer);
        cellView.Init(newCell, gridSize);

        UpdateScore(value);
    }


    private void CreateEmptyCells() {
        for (int i = 0; i < gridSize * gridSize; i++) {
            Instantiate(emptyCellPrefab, emptyCellsContainer);
        }
    }

    public void UpdateScore(int points) {
        currentScore += points;
        currentScoreText.text = currentScore.ToString();

        if (currentScore > highScore) {
            highScore = currentScore;
            highScoreText.text = highScore.ToString();
        }
    }

    private void Start() {
        CreateEmptyCells();
        CreateCell();
        CreateCell();
    }
}
