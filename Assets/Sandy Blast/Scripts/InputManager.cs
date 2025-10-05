using System;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private LayerMask shapeHolderMask;
    [SerializeField] private Vector2 moveSpeed;
    private ShapeHolder currentShapeHolder;

    private Vector3 clickedPosition;
    private Vector3 shapeClickedPosition;
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
        if (!hit.collider.TryGetComponent(out ShapeHolder shapeHolder))
            return;

        currentShapeHolder = shapeHolder;
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

        currentShapeHolder.transform.position = Vector3.Lerp(currentShapeHolder.transform.position, targetPosition, Time.deltaTime * 60 * .3f);
    }


    private void HandleMouseUp()
    {
       
    }




}
