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
    public bool IsSelected { get; set; }

    private Dictionary<Transform, AllyUnit> sockets = new Dictionary<Transform, AllyUnit>();
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

    public long UnitId { get; set; } = 0;

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

        if(Variables.SelectedSlot == null)
        {
            SelectThis();
            return;
        }

        if(Variables.SelectedSlot == this)
        {
            DeselectThis();
            return;
        }

        //다른 슬롯이 선택되어있는 경우
        var from = Variables.SelectedSlot;
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
        if (IsSelected)
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

    public bool SetSocket(AllyUnit prefab, AllyData data)
    {
        foreach(var socket in sockets)
        {
            if(socket.Value == null)
            {
                if(UnitId == 0)
                {
                    UnitId = data.Unit_ID;
                    Variables.SlotCount--;
                }

                var go = Instantiate(prefab, socket.Key.position, socket.Key.rotation);
                sockets[socket.Key] = go;

                go.Center = transform.position;

                try
                {
                    if(data.VisualModel == null)
                    {
                        throw new System.NullReferenceException("VisualModel null 뜸");
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.LogError(
                        $"[Instantiate Error] Unit_Name = {data.Unit_Name}");
                }

                Instantiate(data.VisualModel, go.transform);
                go.Setup(data);

                return true;
            }
        }

        return false;
    }

    public void SelectThis()
    {
        if(Variables.SelectedSlot != null && Variables.SelectedSlot != this)
        {
            Variables.SelectedSlot.DeselectThis();
        }

        Variables.SelectedSlot = this;
        IsSelected = true;
        Refresh();
    }

    public void DeselectThis()
    {
        if(Variables.SelectedSlot == this)
        {
            Variables.SelectedSlot = null;
        }

        IsSelected = false;
        Refresh();
    }

    //유닛 이동 배치
    public void MoveAllUnits(Clickable from, Clickable to)
    {
        var fromPairs = new List<KeyValuePair<Transform, AllyUnit>>();
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

            unit.Center = to.transform.position;
            SendUnitTo(unit, dstSock.position);

            to.sockets[dstSock] = unit;
        }
        
    }

    public void SwapUnits(Clickable from, Clickable to)
    {
        var fromOcc = new List<KeyValuePair<Transform, AllyUnit>>();
        var toOcc = new List<KeyValuePair<Transform, AllyUnit>>();

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
                tUnit.Center = from.transform.position;
                SendUnitTo(tUnit, fSock.position);
            }
            if(fUnit != null)
            {
                fUnit.Center = to.transform.position;
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
                fUnit.Center = to.transform.position;
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
                tUnit.Center = from.transform.position;
                SendUnitTo(tUnit, fSock.position);
                from.sockets[fSock] = tUnit;

                idx++;
            }
        }
    }

    public void SendUnitTo(AllyUnit unit, Vector3 dest)
    {
        var ally = unit.GetComponent<AllyUnit>();
        if(ally != null)
        {
            ally.SetTarget(dest);
        }
    }
}
