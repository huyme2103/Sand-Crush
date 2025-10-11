using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager instance;

    [Header("Elements")]
    //[SerializeField] private ShapeHolder shapeHolderPrefab;
    [SerializeField] private Transform slotsParent;

    [Header("Data")]
    [SerializeField] private Sprite[] shapeSprites;
    [SerializeField] private Color[] colors;
    public Color[] Colors => colors;
    private Shape[] shapes;
    public Shape[] Shapes => shapes;

    private int shapeDroppedCounter;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        GameEvents.shapeDropped += OnShapeDropped;

    }

   
    private void OnDestroy()
    {
        GameEvents.shapeDropped -= OnShapeDropped;
    }
 


    private void Start()
    {
        GenerateShapes();
        PopulateSlots();

    }

    private void OnShapeDropped(ShapeHolder holder)
    {
        shapeDroppedCounter++;
        if(shapeDroppedCounter >= 3)
        {
            shapeDroppedCounter = 0;
            PopulateSlots(); 
        }
    }
    private void GenerateShapes()
    {
        shapes = new Shape[shapeSprites.Length];

        for (int i = 0; i < shapes.Length; i++)
        {
            Texture2D tex = ExtractTextureFromSprite(shapeSprites[i]);
            shapes[i] = GenerateShapesFromTexture(tex);
            shapes[i].tex = tex; //lưu textủe gốc vào vào mỗi shapes
        }
    }

    private void PopulateSlots()
    {
        for (int i = 0; i < slotsParent.childCount; i++) // gan spite vao tung o con
        {
            //ShapeHolder holder = Instantiate(shapeHolderPrefab, slotsParent.GetChild(i).position, Quaternion.identity, transform);
            ShapeHolder holder = ShapeHolderPool.Instance.Get();
            holder.transform.position = slotsParent.GetChild(i).position;
            

            // setup hình để giữ 
            Shape shape = shapes.GetRandom();
            Color color = colors.GetRandom();

            holder.Configure(shape, color);

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

    private Shape GenerateShapesFromTexture(Texture2D tex)  // spite 2d -> shape 
    {
        Shape shape = new Shape(tex.width, tex.height);

        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color pixelColor = tex.GetPixel(x, y); 

                if(pixelColor.a < .1f) //pixel trong suốt => ô trống
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
