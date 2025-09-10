using UnityEngine;

public interface IClickable
{
    public void OnHover(bool isOver);
    public void OnPress(bool isDown);
    public void OnClick();
}
