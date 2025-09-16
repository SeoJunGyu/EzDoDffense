using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private AllyUnit prefab;
    private List<Clickable> slots = new List<Clickable>();

    private List<AllyUnit> allyUnits = new List<AllyUnit>();

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

            //slot.OnSynthesis += () => Destroy(visualModel);
            //slot.OnSynthesis += () => unit.gameObject.SetActive(false);

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
}
