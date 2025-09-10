using UnityEngine;

public interface IDraggable
{
    public void OnDragStart();
    public void OnDrag(Vector3 worldPos);
    public void OnDragEnd(Transform socket);
}
