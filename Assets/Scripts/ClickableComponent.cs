using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask raycastMask = ~0;

    IClickable pressed;

    private void Update()
    {
        if (!TryGetPointer(out Vector2 pos, out bool down, out bool up))
        {
            return;
        }

        IClickable hitClickable = null;
        if (Input.touchCount == 1)
        {
            var ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore))
            {
                hitClickable = hit.collider.GetComponent<IClickable>();
            }
        }

        if(hitClickable == null)
        {
            return;
        }

        if(down)
        {
            pressed = hitClickable;
            pressed.OnPress(true);
        }
        if (up)
        {
            if(pressed != null)
            {
                pressed.OnPress(false);
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
