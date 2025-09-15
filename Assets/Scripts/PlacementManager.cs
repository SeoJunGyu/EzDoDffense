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
            var IsPlace = false;
            if (slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId)
            {
                IsPlace = true;
            }

            if (IsPlace && Variables.Gold >= 50)
            {
                Variables.Gold -= 50;

                slot.SetSocket(prefab, data);
                Debug.Log($"{slot.name} / {prefab.name} / {data.Unit_Name}");

                return true;
            }
        }

        return false;
    }

    public void PlaceInSocket(AllyData data)
    {
        foreach (var slot in slots)
        {
            var IsPlace = false;
            if (slot.UnitId == 0 || (slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId))
            {
                IsPlace = true;
            }

            if (IsPlace && Variables.Gold >= 50)
            {
                Variables.Gold -= 50;

                slot.SetSocket(prefab, data);
                Debug.Log($"{slot.name} / {prefab.name} / {data.Unit_Name}");
                break;
            }
        }
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
}
