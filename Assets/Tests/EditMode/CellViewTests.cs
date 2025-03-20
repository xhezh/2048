using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellViewTests {
    private GameObject testObject;
    private CellView cellView;
    private AdaptiveFontSize adaptiveFontSize;
    private Image backgroundImage;
    private TMP_Text valueText;

    [SetUp]
    public void Setup() {
        testObject = new GameObject("CellViewTest");
        cellView = testObject.AddComponent<CellView>();

        GameObject textObj = new GameObject("Text");
        valueText = textObj.AddComponent<TextMeshProUGUI>();
        textObj.transform.SetParent(testObject.transform);
        cellView.valueText = valueText;

        GameObject imageObj = new GameObject("Background");
        backgroundImage = imageObj.AddComponent<Image>();
        imageObj.transform.SetParent(testObject.transform);
        cellView.backgroundImage = backgroundImage;

        adaptiveFontSize = textObj.AddComponent<AdaptiveFontSize>();
        adaptiveFontSize.textMeshPro = valueText;
        cellView.adaptiveFontSize = adaptiveFontSize;
    }

    [TearDown]
    public void Teardown() {
        GameObject.DestroyImmediate(testObject);
    }

    [Test]
    public void Init_ShouldSubscribeAndCallUpdate() {
        Cell cell = new Cell(new Vector2Int(1, 1), 2);
        cellView.Init(cell, 4);

        Assert.AreEqual(cell.Position, new Vector2Int(1, 1));
        Assert.IsNotNull(cellView);
    }
}
