using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask raycastMask = ~0;
    [SerializeField] float snapRadius = 0.8f;

    private Transform[] sockets;
    private readonly Dictionary<Transform, Transform> occupied = new();

    IClickable hovered;
    IClickable pressed;

    IDraggable dragging;
    Transform draggingTransform;

    private void Awake()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }

        var socketGo = GameObject.FindGameObjectsWithTag("Slot");
        sockets = new Transform[socketGo.Length];
        for(int i = 0; i < sockets.Length; i++)
        {
            sockets[i] = socketGo[i].transform;
        }
    }

    private void Update()
    {
        if (!TryGetPointer(out Vector2 pos, out bool down, out bool up))
        {
            return;
        }

        IClickable hitClickable = null;
        IDraggable hitDraggable = null;
        if(Input.touchCount == 1)
        {
            var ray = cam.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore))
            {
                hitClickable = hit.collider.GetComponent<IClickable>();
            }
        }

        if(hovered != hitClickable)
        {
            if(hovered != null)
            {
                hovered.OnHover(false);
            }

            hovered = hitClickable;

            if (hovered != null)
            {
                hovered.OnHover(true);
            }
        }

        if(down && hovered != null)
        {
            pressed = hovered;
            pressed.OnPress(true);
        }
        if (up)
        {
            if(pressed != null)
            {
                pressed.OnPress(false);
            }
            if(pressed != null && pressed == hovered)
            {
                pressed.OnClick();
            }

            pressed = null;
        }
    }

    private bool TryGetPointer(out Vector2 pos, out bool down, out bool up)
    {
        pos = default;
        down = up = false;

        if(Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            pos = t.position;
            down = t.phase == TouchPhase.Began;
            up = t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled;
            return true;
        }
        else
        {
            pos = Input.mousePosition;
            down = Input.GetMouseButtonDown(0);
            up = Input.GetMouseButtonUp(0);
            return true;
        }
    }
}
