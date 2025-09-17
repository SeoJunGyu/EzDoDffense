using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private AllyUnit prefab;
    private List<Clickable> slots = new List<Clickable>();

    private List<AllyUnit> allyUnits = new List<AllyUnit>();

    public event Action OnSynthesis;

    private void Awake()
    {
        var slotGos = GameObject.FindGameObjectsWithTag("Slot");
        foreach (var slot in slotGos)
        {
            slots.Add(slot.GetComponent<Clickable>());
        }
        slots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        for(int i = 0; i < 10; i++)
        {
            allyUnits.Add(CreateUnit());
        }

        //test
        var data = DataTableManager.AllyTable.Get(1202013002);
        for(int i = 0; i < 3; i++)
        {
            if (!FindSameUnit(data, 0))
            {
                PlaceInSocket(data, 0);
            }
        }
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(DataTableManager.AllyTable.Get(1101011001));
        }
    }

    public bool FindSameUnit(AllyData data)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId;

            if (IsPlace)
            {
                return TryPlaceOnSlot(slot, data, 50);
            }
        }

        return false;
    }

    public void PlaceInSocket(AllyData data)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.UnitId == 0 || (slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId);

            if (IsPlace && TryPlaceOnSlot(slot, data, 50))
            {
                return ;
            }
        }
    }

    public bool FindSameUnit(AllyData data, int cost)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId;

            if (IsPlace)
            {
                return TryPlaceOnSlot(slot, data, cost);
            }
        }

        return false;
    }

    public void PlaceInSocket(AllyData data, int cost)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.UnitId == 0 || (slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId);

            if (IsPlace && TryPlaceOnSlot(slot, data, cost))
            {
                return;
            }
        }
    }

    public bool TryPlaceOnSlot(Clickable slot, AllyData data, int cost)
    {
        if (!TryPay(cost))
        {
            return false;
        }

        if(Variables.SlotCount <= 0)
        {
            return false;
        }

        AllyUnit unit = null;
        foreach(var ally in allyUnits)
        {
            if (!ally.gameObject.activeSelf)
            {
                unit = ally;
                break;
            }
        }

        if(unit == null)
        {
            unit = Instantiate(prefab);
            allyUnits.Add(unit);
        }

        if (slot.SetSocket(unit, data))
        {
            var visualModel = Instantiate(data.VisualModel, unit.transform);
            unit.Setup(data);
            unit.gameObject.SetActive(true);

            OnSynthesis += () => Destroy(visualModel);
            OnSynthesis += () => unit.gameObject.SetActive(false);

            Debug.Log($"{slot.name} / {prefab.name} / {data.Unit_Name}");
            return true;
        }

        return false;
    }

    public void PlaceAllyAllRandom()
    {
        var data = DataTableManager.AllyTable.GetAllRandom();

        if (!FindSameUnit(data))
        {
            PlaceInSocket(data);
        }
    }

    public void PlaceAllyNormalRandom()
    {
        var data = DataTableManager.AllyTable.GetNormalRandom();

        if (!FindSameUnit(data))
        {
            PlaceInSocket(data);
        }
    }

    public void PlaceAllyPiercingRandom()
    {
        var data = DataTableManager.AllyTable.GetPiercingRandom();

        if (!FindSameUnit(data))
        {
            PlaceInSocket(data);
        }
    }

    public void PlaceAllyMagicalRandom()
    {
        var data = DataTableManager.AllyTable.GetMagicalRandom();

        if (!FindSameUnit(data))
        {
            PlaceInSocket(data);
        }
    }

    private AllyUnit CreateUnit()
    {
        var unit = Instantiate(prefab);
        unit.gameObject.SetActive(false);
        return unit;
    }

    private bool TryPay(int cost)
    {
        if(Variables.Gold >= cost)
        {
            Variables.Gold -= cost;
            return true;
        }

        return false;
    }

    //유닛 합성
    public void UnitSynthesis()
    {
        if(!Variables.SelectedSlot || Variables.SelectedSlot.SocketInCount < 3)
        {
            return;
        }

        if(Variables.SelectedSlot.CurrentData.Unit_Grade >= 5)
        {
            return;
        }

        OnSynthesis.Invoke(); //비주얼 모델 제거 후 프리펩 비활성화

        //상위등급, 같은 타입 데이터 가져오기 및 배치
        var data = DataTableManager.AllyTable.GetUpgradeRandomId(Variables.SelectedSlot.CurrentData.Unit_Grade, Variables.SelectedSlot.CurrentData.Unit_Type);
        Variables.SelectedSlot.SlotReset(data); //소켓 딕셔너리, 슬롯 카운트, 슬롯 할당 유닛 id -> 바뀐 id로 변경
        if (!FindSameUnit(data, 0))
        {
            PlaceInSocket(data, 0);
        }

        Variables.SelectedSlot.DeselectThis();
    }
}
