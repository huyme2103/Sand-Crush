using UnityEngine;

[System.Serializable]
public class Shape
{
    public Cell[,] cells;
    public int width; // số ô shape
    public int height;
    public Texture2D tex;
    public  Shape(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new Cell[width, height];
    }
}
    

