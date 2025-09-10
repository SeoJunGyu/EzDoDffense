using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    [SerializeField] Color normal = Color.white;
    [SerializeField] Color hover = new Color(0.9f, 0.95f, 1f);
    [SerializeField] Color press = new Color(0.8f, 0.85f, 1f);

    private Renderer targetRenderer;

    bool isHover;
    bool isPress;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    public void OnClick()
    {
        Debug.Log($"{name} Click");
    }

    public void OnHover(bool isOver)
    {
        isHover = isOver;
        Refresh();
    }

    public void OnPress(bool isDown)
    {
        isPress = isDown;
        Refresh();
    }

    public void Refresh()
    {
        if (isPress)
        {
            ApplyColor(press);
        }
        else if (isHover)
        {
            ApplyColor(hover);
        }
        else
        {
            ApplyColor(normal);
        }
    }

    public void ApplyColor(Color c)
    {
        if(targetRenderer != null && targetRenderer.material != null)
        {
            targetRenderer.material.color = c;
        }
    }
}
