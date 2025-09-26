using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        bool hitAny = false;
        IClickable hitClickable = null;
        EnemyData enemy = null;

        if(Input.touchCount == 1)
        {
            var ray = cam.ScreenPointToRay(pos);

            hitAny = TryRaycastUI(pos, out var uiHit); //뭔가는 충돌되었다.

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore))
            {
                hitClickable = hit.collider.GetComponent<IClickable>();

                if(hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    enemy = EnemySpawner.Instance.CurrentEnemyData;
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(pos);

            hitAny = TryRaycastUI(pos, out var uiHit); //뭔가는 충돌되었다.

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore))
            {
                hitClickable = hit.collider.GetComponent<IClickable>();

                if (hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    enemy = EnemySpawner.Instance.CurrentEnemyData;
                }
            }
        }
#endif

        if (down && !hitAny && hitClickable == null)
        {
            if(Variables.SelectedSlot != null)
            {
                Variables.SelectedSlot.OnPress(false);
                Variables.SelectedSlot.DeselectThis();
                Variables.SelectedSlot = null;

                Variables.SelectedEnemy = null;
            }

            pressed = null;
        }
        if(down && !hitAny && enemy == null)
        {
            if(Variables.SelectedEnemy != null)
            {
                Variables.SelectedEnemy = null;
            }
        }


        if (down && hitClickable != null && !hitAny)
        {
            pressed = hitClickable;
            pressed.OnPress(true);
        }
        else if(down && enemy != null && !hitAny)
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
        else if(Variables.SelectedEnemy != null)
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

    private bool TryRaycastUI(Vector2 screenPos, out GameObject uiHit, List<RaycastResult> hitsBuffer = null)
    {
        uiHit = null;

        if(EventSystem.current == null)
        {
            return false;
        }

        var data = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        var results = hitsBuffer ?? new List<RaycastResult>();
        results.Clear();

        EventSystem.current.RaycastAll(data, results);

        if(results.Count > 0)
        {
            uiHit = results[0].gameObject;
            return true;
        }

        return false;
    }
}
