using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [SerializeField] private AllyUnit prefab;
    private List<Clickable> slots = new List<Clickable>();

    private List<AllyUnit> allyUnits = new List<AllyUnit>();
    public List<AllyUnit> GetAllyUnits
    {
        get
        {
            return allyUnits;
        }
    }

    public SkillManager skillManager;

    //강화 수치 관리
    public static Dictionary<int, int> GradeUpgradeSave = new Dictionary<int, int>();
    public static Dictionary<int, int> TypeUpgradeSave = new Dictionary<int, int>();

    private void Awake()
    {
        Instance = this;

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
            bool IsPlace = slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId && Variables.SelectedSlot != slot;

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
            var grade = GradeUpgradeSave.ContainsKey(data.Unit_Grade) ? GradeUpgradeSave[data.Unit_Grade] : 0;
            unit.Setup(data, grade);
            unit.gameObject.SetActive(true);

            unit.OnSynthesis += () => Destroy(visualModel);
            unit.OnSynthesis += () => unit.gameObject.SetActive(false);

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

        //상위등급, 같은 타입 데이터 가져오기 및 배치
        var data = DataTableManager.AllyTable.GetUpgradeRandomId(Variables.SelectedSlot.CurrentData.Unit_Grade, Variables.SelectedSlot.CurrentData.Unit_Type);
        Variables.SelectedSlot.SlotReset(data); //소켓 딕셔너리, 슬롯 카운트, 슬롯 할당 유닛 id -> 바뀐 id로 변경
        if (!FindSameUnit(data, 0))
        {
            PlaceInSocket(data, 0);
        }

        Variables.SelectedSlot.DeselectThis();
    }

    public void UnitSale()
    {
        Variables.SelectedSlot.UnitDelete();

        Variables.Gold += 50;
    }

    public void GradeUpgrade(int grade)
    {
        if(GradeUpgradeSave.ContainsKey(grade))
        {
            if(GradeUpgradeSave[grade] >= 10)
            {
                return;
            }

            GradeUpgradeSave[grade]++;
        }
        else
        {
            GradeUpgradeSave.Add(grade, 1);
        }

        AllGradeUpgradeSetUp(GradeUpgradeSave[grade], grade);
        return;
    }

    public void AllGradeUpgradeSetUp(int gradeUpdate, int grade)
    {
        foreach(var slot in slots)
        {
            if(slot.SocketInCount > 0)
            {
                slot.AllGradeUpgradeUnitSetup(grade, gradeUpdate);
            }
        }
    }
}
