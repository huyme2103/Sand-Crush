using System;
using System.Collections.Generic;
using UnityEngine;

public class SandSimulation : MonoBehaviour
{
    public static SandSimulation instance;
    public static Action<int> OnLineCleared; // truyền vào số hàng/cột bị xóa


    [Header("Elements")]
    private SpriteRenderer renderer;
    private Texture2D texture;


    [Header("Settings")]
    [SerializeField] private int width; // x chạy ngang
    [SerializeField] private int height; // y chạy dọc
    [SerializeField] private Color backgroundColor;
    [SerializeField] private int pixelsPerUnit = 100;

    public static float maxX;
    public static float maxY;

    private bool sandMoved;
    private bool searchedForMatch;

    [Header("Data")]
    private Cell[,] grid;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        GameEvents.shapeDropped += OnShapeDropped;

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameEvents.shapeDropped -= OnShapeDropped;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        grid = new Cell[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                grid[x, y].color = backgroundColor; // duyệt tất cả ô grid thành màu background

        grid[width / 2, height / 2] = new Cell { type = EMaterialType.Sand, color = Color.red };

        UpdateTexture();

        CalcukateBounds();

        renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one / 2, pixelsPerUnit);//new Rect lấy hết ảnh từ (0,0) đến (width,height) độ nhỏ của cát


    }

   

    // Update is called once per frame
    void Update()
    {
        // Sand Fountain
        //grid[width / 2, height / 2] = new Cell { type = EMaterialType.Sand, color = Color.red };
        //HandleInput();

        SimulateSand();

        if (!sandMoved && !searchedForMatch)
        {
            TryFindMatch();

        }

        UpdateTexture();
    }

    private void TryFindMatch()
    {
        searchedForMatch = true;

        for (int i = 0; i < ShapeManager.instance.Colors.Length; i++)
        {
            Color color = ShapeManager.instance.Colors[i];

            if(TexturePathChecker.TryGetConnectedRegion(texture, color, out HashSet<Vector2Int> coords))
            {
                foreach(Vector2Int coord in coords)
                {
                    grid[coord.x, coord.y].type = EMaterialType.Empty;
                    grid[coord.x, coord.y].color = backgroundColor;
                }

                OnLineCleared?.Invoke(coords.Count);//cộng điểm
                UpdateTexture();
                break;
            }
        }
    }

    private void CalcukateBounds()
    {
        maxX = (float)width / 200;
        maxY = (float) height / 200;

    }


    private void OnShapeDropped(ShapeHolder shapeHolder)
    {
        Vector2Int gridCoords = WorldToGrid(shapeHolder.transform.position);

        DropShape(shapeHolder.Shape, shapeHolder.Color, gridCoords);

        //Destroy(shapeHolder.gameObject);
        ShapeHolderPool.Instance.ReturnToPool(shapeHolder);

    }
    //private void HandleInput()
    //{
    //    if (!Input.GetMouseButtonDown(0))
    //        return;

    //    Vector3 worldClickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2Int gridCoords = WorldToGrid(worldClickedPosition); 

    //    Shape randomShape = ShapeManager.instance.Shapes.GetRandom(); // GetRandom code extend
    //    Color randomColor = ShapeManager.instance.Colors.GetRandom();

    //    DropShape(randomShape, randomColor, gridCoords);
    //}

    private void DropShape(Shape shape, Color color, Vector2Int gridCoords)
    {
        for (int y = 0; y < shape.height; y++)
        {
            for (int x = 0; x < shape.width; x++)
            {
                int texX = gridCoords.x - (shape.width / 2) + x;
                int texY = gridCoords.y - (shape.width / 2) + y;

                if (!IsInBounds(new Vector2Int(texX, texY)) || shape.cells[x, y].type == EMaterialType.Empty)
                    continue;


                Cell cell = shape.cells[x, y];
                cell.color = color;
                grid[texX, texY] = cell;
            }
        }
    }

    private bool IsInBounds(Vector2Int coords)
    {
        return coords.x >= 0 && coords.x < width && coords.y >= 0 && coords.y < height;
    }
    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x * 100 + width / 2);
        int y = Mathf.RoundToInt(worldPos.y * 100 + height / 2);
        return new Vector2Int(x, y);
    }
    private void UpdateTexture()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, y, grid[x, y].color); //pixel theo màu của Cell

        texture.Apply();
    }

    private void SimulateSand()
    {
        sandMoved = false;
        // duyệt từ dưới lên
        for (int y = 1; y < height; y++) // khi grid[x, y-1] = grid[x, -1] y sẽ ngoài mảng, height -1 hàng trên cùng
        {
            bool righSide = Time.frameCount % 2 == 0;
            if (righSide)// rơi pixel đều 2 bên
            {
                for (int x = width - 1; x >= 0; x--)
                    if (grid[x, y].type == EMaterialType.Sand)
                        TryMoveSand(x, y);
            }
            else
            {
                for (int x = 0; x < width; x++)
                    if (grid[x, y].type == EMaterialType.Sand)
                    TryMoveSand(x, y);
            }
        }
    }
    private void TryMoveSand(int x, int y) // sand drop
    {
        // cạnh trái và phải màn hình
        if(x==0|| x == width - 1)
        {
            HandleBorder(x, y);
            return;
        }
        //check ô dưới
        if (grid[x,y-1].type == EMaterialType.Empty)
        {
            Swap(x, y, x, y - 1);  // đổi y với vị trí y -1
        }
        //check ô bên dưới bên phải
        else if (grid[x+1,y-1].type == EMaterialType.Empty)
        {
            Swap(x, y, x + 1, y - 1);
        }
        //roi xuong ô bên dưới bên trái
        else if (grid[x - 1, y - 1].type == EMaterialType.Empty)
        {
            Swap(x, y, x - 1, y - 1);
        }

    }

    private void HandleBorder(int x, int y)
    {
        //check ô dưới
        if (grid[x, y - 1].type == EMaterialType.Empty)
        {
            Swap(x, y, x, y - 1);  // đổi y với vị trí y -1
        }
        else
        {
            if (x == 0)
            {
                //check ô bên dưới bên phải
                if (grid[x + 1, y - 1].type == EMaterialType.Empty)
                {
                    Swap(x, y, x + 1, y - 1);
                }
            }
            else if (x == width - 1)
            {
                //roi xuong ô bên dưới bên trái
                if (grid[x - 1, y - 1].type == EMaterialType.Empty)
                {
                    Swap(x, y, x - 1, y - 1);
                }

            }
        }
    
    }
    private void Swap(int x1,int y1, int x2, int y2)// doi vi tri ô
    {
        Cell temp = grid[x1, y1];
        grid[x1, y1] = grid[x2, y2];
        grid[x2, y2] = temp;

        sandMoved = true;
        searchedForMatch = false;
    }

    public bool CanDropShape(ShapeHolder holder)
    {
        Shape shape = holder.Shape;
        Vector2Int gridCoords = WorldToGrid(holder.transform.position);

        for (int y = 0; y < shape.height; y++)
        {
            for (int x = 0; x < shape.width; x++)
            {
                // Bỏ qua pixel trống
                if (shape.cells[x, y].type == EMaterialType.Empty)
                    continue;

                int texX = gridCoords.x - (shape.width / 2) + x;
                int texY = gridCoords.y - (shape.height / 2) + y;

                // Nếu nằm ngoài lưới
                if (!IsInBounds(new Vector2Int(texX, texY)))
                    return false;

                // Nếu trong lưới mà vị trí đó đã có cát
                if (grid[texX, texY].type != EMaterialType.Empty)
                    return false;
            }
        }

        // có thể thả
        return true;
    }


}
