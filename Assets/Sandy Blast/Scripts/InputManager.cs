using System;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private LayerMask shapeHolderMask;
    [SerializeField] private Vector2 moveSpeed;
    [SerializeField] private float dropYThreshold;

    private ShapeHolder currentShapeHolder;

    private Vector3 clickedPosition;
    private Vector3 shapeClickedPosition;

    [Header("Actions")]
    public static Action<ShapeHolder> shapeDropped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            HandleldMouseDown();
        else if (Input.GetMouseButton(0) && currentShapeHolder != null)
            HandleDrag();
        else if (Input.GetMouseButtonUp(0) && currentShapeHolder != null)
            HandleMouseUp();
    }

    private void HandleldMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero, Mathf.Infinity, shapeHolderMask);
        if (hit.collider == null || !hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
            return;

        currentShapeHolder = shapeHolder;
        currentShapeHolder.Pickup();

        clickedPosition = Input.mousePosition;
        shapeClickedPosition = currentShapeHolder.transform.position;
    }
    private void HandleDrag()
    {
        Vector2 delta = Input.mousePosition - clickedPosition;

        delta.x /= Screen.width;
        delta.y /= Screen.height;

        delta *= moveSpeed;
        Vector2 targetPosition = (Vector2)shapeClickedPosition + delta;

        Bounds shapeBounds = currentShapeHolder.Bounds;
        float maxX = SandSimulation.maxX - shapeBounds.extents.x;
        float maxY = SandSimulation.maxY - shapeBounds.extents.y;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -maxX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, targetPosition.y, maxY);

        currentShapeHolder.transform.position = Vector3.Lerp(currentShapeHolder.transform.position, targetPosition, Time.deltaTime * 60 * .3f);
    }


    private void HandleMouseUp()
    {
       //có thể bỏ shape ?
       //nếu y thấp quá, đưa về vị trí ban đầu
       // drop

        if(currentShapeHolder.transform.position.y < dropYThreshold ||
            !SandSimulation.instance.CanDropShape(currentShapeHolder))
        {
            MoveShapeBack();
        }
        else
        {
            shapeDropped?.Invoke(currentShapeHolder);
            currentShapeHolder = null;//

        }
    }

    private void MoveShapeBack()
    {
        currentShapeHolder.PutBack();

        LeanTween.move(currentShapeHolder.gameObject, shapeClickedPosition, .1f);
        currentShapeHolder = null;//
    }
}
