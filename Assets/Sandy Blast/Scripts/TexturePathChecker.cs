using System.Collections.Generic;
using UnityEngine;

public class TexturePathChecker
{
    public static bool TryGetConnectedRegion(Texture2D tex, Color color, out HashSet<Vector2Int> coords, float tolerance = .01f )
    {
        int width = tex.width;
        int height = tex.height;

        bool[,] visited = new bool[width, height];

        coords = new HashSet<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // luu tat ca pixel bat dau vao cot ben phai
        for (int y = 0; y < height; y++)
        {
            if(IsSameColor(tex.GetPixel(width-1,y), color, tolerance)){
                Vector2Int pos = new Vector2Int(width - 1, y);
                queue.Enqueue(pos);
                coords.Add(pos);

                visited[width - 1, y] = true;
            }
        }

        Vector2Int[] directions =
        {
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up
        };

        bool touchesLeft = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current.x == 0)
                touchesLeft = true;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;

                if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                    continue;

                if (visited[next.x, next.y])
                    continue;

                if (!IsSameColor(tex.GetPixel(next.x, next.y), color, tolerance))
                    continue;

                queue.Enqueue(next);
                visited[next.x, next.y] = true;
                coords.Add(next);
            }

        }
        if (!touchesLeft)
        {
            coords = null;
            return false;
        }
        return true;
    }
    
    public static bool IsSameColor(Color color1, Color color2, float tolerance)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance
            && Mathf.Abs(color1.g - color2.g) < tolerance
            && Mathf.Abs(color1.b - color2.b) < tolerance;
    }
}
