using System.Collections.Generic;
using UnityEngine;

public class ShapeHolderPool : MonoBehaviour
{
    public static ShapeHolderPool Instance { get; private set; }

    [SerializeField] private ShapeHolder shapeHolderPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<ShapeHolder> pool = new Queue<ShapeHolder>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Tạo các ShapeHolder ban đầu
        for (int i = 0; i < initialPoolSize; i++)
        {
            ShapeHolder holder = Instantiate(shapeHolderPrefab, transform);
            holder.gameObject.SetActive(false);
            pool.Enqueue(holder);
        }
    }

    // Lấy 1 ShapeHolder từ pool
    public ShapeHolder Get()
    {
        if (pool.Count > 0)
        {
            ShapeHolder holder = pool.Dequeue();
            holder.gameObject.SetActive(true);
            return holder;
        }
        else
        {
            // Nếu hết pool thì tạo thêm
            ShapeHolder newHolder = Instantiate(shapeHolderPrefab, transform);
            return newHolder;
        }
    }

    // Trả lại ShapeHolder vào pool
    public void ReturnToPool(ShapeHolder holder)
    {
        holder.gameObject.SetActive(false);
        pool.Enqueue(holder);
    }
}
