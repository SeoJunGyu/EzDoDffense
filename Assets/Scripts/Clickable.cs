using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    [SerializeField] Color normal = Color.white;
    [SerializeField] Color press = new Color(0.8f, 0.85f, 1f);
    [SerializeField] Color selectedColor = new Color(0.9f, 0.95f, 1f);
    
    private Renderer targetRenderer;

    private bool isPress;
    private bool isSelected;

    private static Clickable selectedSlot;

    private Dictionary<Transform, GameObject> sockets = new Dictionary<Transform, GameObject>();
    private int count = 0;
    public int SocketInCount
    {
        get
        {
            count = 0;
            foreach(var socket in sockets)
            {
                if(socket.Value != null)
                {
                    count++;
                }
            }
            return count;
        }
    }

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();

        var findGo = GetComponentsInChildren<Transform>();
        foreach(var tr in findGo)
        {
            if(tr.tag == "Socket")
            {
                sockets.Add(tr, null);
            }
        }

        ApplyColor(normal);
    }

    public void OnClick()
    {
        Debug.Log($"{name} Click / SocketInCount : {SocketInCount}");

        if(selectedSlot == null)
        {
            SelectThis();
            return;
        }

        if(selectedSlot == this)
        {
            DeselectThis();
            return;
        }

        //�ٸ� ������ ���õǾ��ִ� ���
        var from = selectedSlot;
        var to = this;

        SwapUnits(from, to);

        from.DeselectThis();
    }

    public void OnPress(bool isDown)
    {
        isPress = isDown;
        Refresh();
    }

    public void Refresh()
    {
        if (isSelected)
        {
            ApplyColor(selectedColor);
        }
        else if (isPress)
        {
            ApplyColor(press);
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

    public bool SetSocket(GameObject prefab)
    {
        foreach(var socket in sockets)
        {
            if(socket.Value == null)
            {
                var go = Instantiate(prefab, socket.Key.position, socket.Key.rotation);
                sockets[socket.Key] = go;
                return true;
            }
        }

        return false;
    }

    public void SelectThis()
    {
        if(selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.DeselectThis();
        }

        selectedSlot = this;
        isSelected = true;
        Refresh();
    }

    public void DeselectThis()
    {
        if(selectedSlot == this)
        {
            selectedSlot = null;
        }

        isSelected = false;
        Refresh();
    }

    //���� �̵� ��ġ
    public void MoveAllUnits(Clickable from, Clickable to)
    {
        var fromPairs = new List<KeyValuePair<Transform, GameObject>>();
        foreach(var kv in from.sockets)
        {
            if(kv.Value != null)
            {
                fromPairs.Add(kv);
            }
        }

        if(fromPairs.Count == 0)
        {
            return;
        }

        var toEmptySockets = new List<Transform>();
        foreach(var kv in to.sockets)
        {
            if(kv.Value == null)
            {
                toEmptySockets.Add(kv.Key);
            }
        }

        int moved = Mathf.Min(fromPairs.Count, toEmptySockets.Count);
        for(int i = 0; i < moved; i++)
        {
            var srcSock = fromPairs[i].Key;
            var unit = fromPairs[i].Value;
            var dstSock = toEmptySockets[i];

            from.sockets[srcSock] = null;

            SendUnitTo(unit, dstSock.position, dstSock.parent.position);

            to.sockets[dstSock] = unit;
        }
        
    }

    public void SwapUnits(Clickable from, Clickable to)
    {
        var fromOcc = new List<KeyValuePair<Transform, GameObject>>();
        var toOcc = new List<KeyValuePair<Transform, GameObject>>();

        foreach (var kv in from.sockets)
        {
            if (kv.Value != null)
            {
                fromOcc.Add(kv);
            }
        }
        foreach (var kv in to.sockets)
        {
            if (kv.Value != null)
            {
                toOcc.Add(kv);
            }
        }

        //���� ���� ���� ��ȯ
        int pairCount = Mathf.Min(fromOcc.Count, toOcc.Count);
        for(int i = 0; i < pairCount; i++)
        {
            var fSock = fromOcc[i].Key;
            var fUnit = fromOcc[i].Value;

            var tSock = toOcc[i].Key;
            var tUnit = toOcc[i].Value;

            if(tUnit != null)
            {
                SendUnitTo(tUnit, fSock.position, fSock.parent.position);
            }
            if(fUnit != null)
            {
                SendUnitTo(fUnit, tSock.position, tSock.parent.position);
            }

            from.sockets[fSock] = tUnit;
            to.sockets[tSock] = fUnit;
        }

        //from ���� �ο� ��ȯ
        if(fromOcc.Count > pairCount)
        {
            int idx = pairCount;

            var toEmpty = new List<Transform>();
            foreach(var kv in to.sockets)
            {
                if(kv.Value == null)
                {
                    toEmpty.Add(kv.Key);
                }
            }

            int e = 0;
            while (idx < fromOcc.Count && e < toEmpty.Count)
            {
                var fSock = fromOcc[idx].Key;
                var fUnit = fromOcc[idx].Value;

                if (from.sockets[fSock] == fUnit)
                {
                    from.sockets[fSock] = null;
                }

                var toSock = toEmpty[e++];

                //�̵�
                SendUnitTo(fUnit, toSock.position, toSock.parent.position);
                to.sockets[toSock] = fUnit;

                idx++;
            }
        }

        //to ���� �ο� ��ȯ
        if (toOcc.Count > pairCount)
        {
            int idx = pairCount;

            var fromEmpty = new List<Transform>();
            foreach (var kv in from.sockets)
            {
                if (kv.Value == null)
                {
                    fromEmpty.Add(kv.Key);
                }
            }

            int e = 0;
            while (idx < toOcc.Count && e < fromEmpty.Count)
            {
                var tSock = toOcc[idx].Key;
                var tUnit = toOcc[idx].Value;

                if (to.sockets[tSock] == tUnit)
                {
                    to.sockets[tSock] = null;
                }

                var fSock = fromEmpty[e++];

                //�̵�
                SendUnitTo(tUnit, fSock.position, fSock.parent.position);
                from.sockets[fSock] = tUnit;

                idx++;
            }
        }
    }

    public void SendUnitTo(GameObject unit, Vector3 dest, Vector3 slotCenter)
    {
        var ally = unit.GetComponent<AllyUnit>();
        if(ally != null)
        {
            ally.SetTarget(dest, slotCenter);
        }
    }
}
