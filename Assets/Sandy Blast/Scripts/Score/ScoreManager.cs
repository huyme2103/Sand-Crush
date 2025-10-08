using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore;
    private int comboCount;
    private const string SCORE_KEY = "PlayerScore";
    public static Action<int> OnScoreChanged;
    public static Action<int> OnComboChanged;


    private void Awake()
    {

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        LoadScore();
    }

    private void OnEnable()
    {
        // Đăng ký lắng nghe event từ InputManager và SandSimulation
        InputManager.shapeDropped += OnShapeDropped;
        SandSimulation.OnLineCleared += OnLineCleared;
    }

    private void OnDisable()
    {
        InputManager.shapeDropped -= OnShapeDropped;
        SandSimulation.OnLineCleared -= OnLineCleared;
    }

    // +10 điểm mỗi block thả xuống
    private void OnShapeDropped(ShapeHolder holder)
    {
        AddScore(10);
        comboCount = 0; // reset combo khi thả khối mới

        OnComboChanged?.Invoke(comboCount); // reset combo hiển thị
    }

    // +100 điểm mỗi vùng match, thêm bonus theo kích thước và combo
    private void OnLineCleared(int clearedCount)
    {
        if (clearedCount <= 0) return;

        int baseScore = 100;

        // Nếu xóa nhiều pixel hơn, thưởng theo % (vd: +50% nếu >50 pixel)
        float sizeBonus = clearedCount > 50 ? 0.5f : 0f;
        baseScore += Mathf.RoundToInt(baseScore * sizeBonus);

        // Combo bonus (chuỗi liên tiếp)
        comboCount++;
        float comboMultiplier = 1f + comboCount * 0.1f; // +10% mỗi combo
        int total = Mathf.RoundToInt(baseScore * comboMultiplier);

        AddScore(total);

        OnComboChanged?.Invoke(comboCount); //  Báo cho UI combo
    }

    private void AddScore(int value)
    {
        currentScore += value;
        Debug.Log($"[SCORE] +{value} | Total = {currentScore}");
        OnScoreChanged?.Invoke(currentScore); // Báo cho UI cập nhật
        SaveScore();
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt(SCORE_KEY, currentScore);
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        currentScore = PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    public int GetScore() => currentScore;
}
