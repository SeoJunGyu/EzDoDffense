using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ClickableComponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask raycastMask = ~0;

    IClickable pressed;

    private Camera cam;

    public UIManager uiManager;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!TryGetPointer(out Vector2 pos, out bool down, out bool up))
        {
            return;
        }

        IClickable hitClickable = null;
        EnemyUnit enemy = null;

        if(Input.touchCount == 1)
        {
            var ray = cam.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore))
            {
                hitClickable = hit.collider.GetComponent<IClickable>();

                if(hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    enemy = hit.collider.gameObject.GetComponent<EnemyUnit>();
                }
            }
        }

        if (down && hitClickable == null)
        {
            if(Variables.SelectedSlot != null)
            {
                Variables.SelectedSlot.OnPress(false);
                Variables.SelectedSlot.DeselectThis();
                Variables.SelectedSlot = null;
            }

            pressed = null;
        }
        else if(down && enemy == null)
        {
            if(Variables.SelectedEnemy != null)
            {
                Variables.SelectedEnemy = null;
            }
        }


        if (down && hitClickable != null)
        {
            pressed = hitClickable;
            pressed.OnPress(true);
        }
        else if(down && enemy != null)
        {
            Variables.SelectedEnemy = enemy;
        }

        if (up)
        {
            if (pressed != null)
            {
                pressed.OnPress(false);
                pressed.OnClick();
            }

            pressed = null;
        }

        if (Variables.SelectedSlot)
        {
            uiManager.ActiveInfoPanel(Variables.SelectedSlot.CurrentData);
        }
        else if(Variables.SelectedEnemy)
        {
            uiManager.ActiveInfoPanel(Variables.SelectedEnemy);
        }
        else
        {
            uiManager.UnActiveInfoPanel();
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
