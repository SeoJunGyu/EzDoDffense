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
        if(SocketInCount == 0)
        {
            if(selectedSlot == this)
            {
                DeselectThis();
            }

            return;
        }

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

        //다른 슬롯이 선택되어있는 경우
        var from = selectedSlot;
        var to = this;

        if(to.SocketInCount > 0)
        {
            SwapUnits(from, to);
        }
        else
        {
            MoveAllUnits(from, to);
        }

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

    //유닛 이동 배치
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

        int moved = 0;
        foreach(var toSock in to.sockets.Keys)
        {
            if(moved >= fromPairs.Count)
            {
                break;
            }
            if (to.sockets[toSock] != null)
            {
                continue;
            }

            var srcSock = fromPairs[moved].Key;
            var unit = fromPairs[moved].Value;

            from.sockets[srcSock] = null;

            //이동
            SendUnitTo(unit, toSock.position);
            to.sockets[toSock] = unit;

            moved++;
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

        //적은 수를 먼저 교환
        int pairCount = Mathf.Min(fromOcc.Count, toOcc.Count);
        for(int i = 0; i < pairCount; i++)
        {
            var fSock = fromOcc[i].Key;
            var fUnit = fromOcc[i].Value;

            var tSock = toOcc[i].Key;
            var tUnit = toOcc[i].Value;

            if(tUnit != null)
            {
                SendUnitTo(tUnit, fSock.position);
            }
            if(fUnit != null)
            {
                SendUnitTo(fUnit, tSock.position);
            }

            from.sockets[fSock] = tUnit;
            to.sockets[tSock] = fUnit;
        }

        //from 남은 인원 교환
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

                //이동
                SendUnitTo(fUnit, toSock.position);
                to.sockets[toSock] = fUnit;

                idx++;
            }
        }

        //to 남은 인원 교환
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

                //이동
                SendUnitTo(tUnit, fSock.position);
                from.sockets[fSock] = tUnit;

                idx++;
            }
        }
    }

    public void SendUnitTo(GameObject unit, Vector3 dest)
    {
        var ally = unit.GetComponent<AllyUnit>();
        if(ally != null)
        {
            ally.SetTarget(dest);
        }
    }
}
