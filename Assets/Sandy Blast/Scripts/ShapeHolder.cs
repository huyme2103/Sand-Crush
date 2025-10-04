using UnityEngine;

public class ShapeHolder : MonoBehaviour
{
    [Header(("Elements"))]
    [SerializeField] private SpriteRenderer renderer;
  public void Configure(Shape shape, Color color)
    {
        Texture2D tex = shape.tex;

        Texture2D newTex = new Texture2D(tex.width, tex.height);
        newTex.filterMode = FilterMode.Point;

        Color[] colors = tex.GetPixels();

        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a > .1f)
                colors[i] = color;
        }

        newTex.SetPixels(colors);
        newTex.Apply();

        renderer.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height),Vector2.one/2,100);
    }
}
