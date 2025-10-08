using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = comboText.transform.localScale;
    }
    private void Start()
    {
        // Khởi tạo text ban đầu
        UpdateScoreUI(ScoreManager.Instance.GetScore());
        UpdateComboUI(0);


    }
    private void OnEnable()
    {
        // Đăng ký lắng nghe các event từ ScoreManager
        ScoreManager.OnScoreChanged += UpdateScoreUI;
        ScoreManager.OnComboChanged += UpdateComboUI;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= UpdateScoreUI;
        ScoreManager.OnComboChanged -= UpdateComboUI;
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    private void UpdateComboUI(int combo)
    {
        if (combo <= 0)
        {
            comboText.text = "";
            return;
        }

        comboText.text = $"Combo x{combo}";

        // Reset lại scale cũ
        comboText.transform.localScale = originalScale;

        // Hủy các tween cũ đang chạy trên comboText (tránh xung đột)
        LeanTween.cancel(comboText.gameObject);

        // Hiệu ứng phóng to và thu nhỏ mượt
        LeanTween.scale(comboText.rectTransform, originalScale * 1.3f, 0.15f)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(comboText.rectTransform, originalScale, 0.15f).setEaseInBack();
            });

        // Hiệu ứng đổi màu nhẹ (ví dụ từ trắng → vàng → trắng)
        Color baseColor = Color.white;
        LeanTween.value(comboText.gameObject, baseColor, Color.yellow, 0.15f)
            .setOnUpdate((Color c) => comboText.color = c)
            .setOnComplete(() =>
            {
                LeanTween.value(comboText.gameObject, Color.yellow, baseColor, 0.15f)
                    .setOnUpdate((Color c) => comboText.color = c);
            });
    }
}
