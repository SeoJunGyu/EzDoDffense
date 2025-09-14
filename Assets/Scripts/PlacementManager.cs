using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private List<Clickable> slots = new List<Clickable>();

    private void Awake()
    {
        var slotGos = GameObject.FindGameObjectsWithTag("Slot");
        foreach (var slot in slotGos)
        {
            slots.Add(slot.GetComponent<Clickable>());
        }
        slots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }

    public void PlaceAlly()
    {
        foreach (var slot in slots)
        {
            if (slot.SocketInCount < 3)
            {
                slot.SetSocket(prefab);
                Debug.Log($"{slot.name} / {prefab.name}");
                break;
            }
        }
    }
}
