using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager instance;

    [Header("Data")]
    [SerializeField] private Sprite[] shapeSprites;
    [SerializeField] private Color[] colors;
    public Color[] Colors => colors;
    private Shape[] shapes;
    public Shape[] Shapes => shapes;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateShapes();
        
    }

    private void GenerateShapes()
    {
        shapes = new Shape[shapeSprites.Length];

        for (int i = 0; i < shapes.Length; i++)
        {
            Texture2D tex = ExtractTextureFromSprite(shapeSprites[i]);
            shapes[i] = GenerateShapesFromTexture(tex);
        }
    }


    private Texture2D ExtractTextureFromSprite(Sprite sprite) // tach từng sprite ra khoi atlat
    {
        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        tex.filterMode = FilterMode.Point;

        Color[] colors = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);//lấy pixel từ 1 phần của texture gốc (phần thuộc về sprite).màu bắt đầu từ (x, y) với kích thước (width, height)

        tex.SetPixels(colors);//gán màu trên vào teture trên
        tex.Apply();

        return tex;

    }

    private Shape GenerateShapesFromTexture(Texture2D tex)
    {
        Shape shape = new Shape(tex.width, tex.height);

        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color pixelColor = tex.GetPixel(x, y);

                if(pixelColor.a < .1f) //ixel trong suốt => ô trống
                {
                    shape.cells[x, y] = Cell.Empty;
                }
                else // có màu => ô cát
                {
                    shape.cells[x, y] = new Cell { type = EMaterialType.Sand, color = Color.white };
                }
            }
        }
        return shape;
    }
}
