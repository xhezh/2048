using NUnit.Framework;
using UnityEngine;
using TMPro;

public class AdaptiveFontSizeTests {
    private GameObject testObject;
    private AdaptiveFontSize adaptiveFontSize;
    private TMP_Text tmpText;

    [SetUp]
    public void Setup() {
        testObject = new GameObject("TestObject");
        tmpText = testObject.AddComponent<TextMeshProUGUI>();
        adaptiveFontSize = testObject.AddComponent<AdaptiveFontSize>();
        adaptiveFontSize.textMeshPro = tmpText;
        adaptiveFontSize.maxFontSize = 100f;
        adaptiveFontSize.minFontSize = 30f;
        adaptiveFontSize.maxDigits = 6;
    }

    [Test]
    public void Start_ShouldAssignTextMeshPro_WhenNull() {
        adaptiveFontSize.textMeshPro = null;
        adaptiveFontSize.Start();
        Assert.IsNotNull(adaptiveFontSize.textMeshPro, "TextMeshPro должен быть назначен автоматически");
    }

    [TearDown]
    public void Teardown() {
        GameObject.DestroyImmediate(testObject);
    }

    [Test]
    public void UpdateNumber_ShouldUpdateTextAndFontSize() {
        int number = 12345;
        adaptiveFontSize.UpdateNumber(number);
        Assert.AreEqual(number.ToString(), tmpText.text, "Текст должен совпадать с числом.");
        Assert.IsTrue(tmpText.fontSize <= adaptiveFontSize.maxFontSize &&
                      tmpText.fontSize >= adaptiveFontSize.minFontSize,
                      "Размер шрифта должен находиться между minFontSize и maxFontSize.");
    }

    [Test]
    public void UpdateNumber_ShouldSetMinFontSize_WhenNumberHasMaxDigits() {
        int number = 999999;
        adaptiveFontSize.UpdateNumber(number);
        Assert.AreEqual(number.ToString(), tmpText.text);
        Assert.AreEqual(adaptiveFontSize.minFontSize, tmpText.fontSize, "При максимальном числе размер должен быть минимальным");
    }

    [Test]
    public void UpdateNumber_ShouldSetMaxFontSize_WhenNumberIsSingleDigit() {
        int number = 5;
        int digitCount = number.ToString().Length;
        float t = Mathf.Clamp01((float)digitCount / adaptiveFontSize.maxDigits);
        float expectedFontSize = Mathf.Lerp(adaptiveFontSize.maxFontSize, adaptiveFontSize.minFontSize, t);
        adaptiveFontSize.UpdateNumber(number);
        Assert.AreEqual(expectedFontSize, tmpText.fontSize, 0.001f, "Размер шрифта должен совпадать с рассчитанным");
    }


}
