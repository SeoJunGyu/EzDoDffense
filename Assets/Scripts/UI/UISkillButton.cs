using UnityEngine;
using UnityEngine.EventSystems;

public class UISkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UIUnitInfo uiInfo;
    public int skillIndex;

    private bool isPressed;

    private void Update()
    {
        if (isPressed)
        {
            uiInfo.ActiveSkillInfo(skillIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        uiInfo.SkillInfo.gameObject.SetActive(false);
    }
}
