using UnityEngine;

public class UICheat : MonoBehaviour
{
    private bool infiniteGold = false;
    public PlacementManager placementManager;

    private void Update()
    {
        if (infiniteGold)
        {
            Variables.Gold = 999;
        }
    }

    public void InfiniteGold()
    {
        infiniteGold = !infiniteGold;
    }

    public void TreeEnemySpawn()
    {
        var data = DataTableManager.AllyTable.Get(1101011001);
        for (int i = 0; i < 3; i++)
        {
            if (!placementManager.FindSameUnit(data, 0))
            {
                placementManager.PlaceInSocket(data, 0);
            }
        }
    }

    public void OneEnemySpawn()
    {
        var data = DataTableManager.AllyTable.Get(1101011001);
        if (!placementManager.FindSameUnit(data, 0))
        {
            placementManager.PlaceInSocket(data, 0);
        }
    }

    public void TimeScaleOne()
    {
        Time.timeScale = 1f;
    }

    public void TimeScaleTwice()
    {
        Time.timeScale = 2f;
    }
}
