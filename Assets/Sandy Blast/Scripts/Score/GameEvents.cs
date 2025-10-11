using System;

public static class GameEvents
{
    // Khi điểm thay đổi
    public static Action<int> OnScoreChanged;

    // Khi combo thay đổi
    public static Action<int> OnComboChanged;

    public static Action<ShapeHolder> shapeDropped;
}
