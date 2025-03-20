using TMPro;
using UnityEngine;

public class AdaptiveFontSize : MonoBehaviour {
    public TMP_Text textMeshPro;
    public float maxFontSize = 100f;
    public float minFontSize = 30f;
    public int maxDigits = 6;

    private void Start() {
        if (textMeshPro == null) {
            textMeshPro = GetComponent<TMP_Text>();
        }
    }

    /// <summary>
    /// Обновляет текст и размер шрифта в зависимости от количества цифр.
    /// </summary>
    /// <param name="number">Новое число для отображения.</param>
    public void UpdateNumber(int number) {
        string numberText = number.ToString();
        int digitCount = numberText.Length;

        float t = Mathf.Clamp01((float)digitCount / maxDigits);
        float newFontSize = Mathf.Lerp(maxFontSize, minFontSize, t);

        textMeshPro.fontSize = newFontSize;
        textMeshPro.text = numberText;
    }
}
