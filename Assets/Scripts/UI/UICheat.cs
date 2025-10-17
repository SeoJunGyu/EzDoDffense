using UnityEngine;

public class UICheat : MonoBehaviour
{
    private bool infiniteGold = false;
    private bool infiniteGem = false;
    public PlacementManager placementManager;

    private void Update()
    {
        if (infiniteGold)
        {
            Variables.Gold = 999;
        }
        if (infiniteGem)
        {
            Variables.Gem = 999;
        }
    }

    public void InfiniteGold()
    {
        infiniteGold = !infiniteGold;
    }

    public void InfiniteGem()
    {
        infiniteGem = !infiniteGem;
    }

    public void TreeEnemySpawn()
    {
        var data = DataTableManager.AllyTable.Get(1101011001);
        for (int i = 0; i < 3; i++)
        {
            placementManager.PlaceUnitCheck(data, 0);
        }
    }

    public void OneEnemySpawn()
    {
        var data = DataTableManager.AllyTable.Get(1430011001);
        placementManager.PlaceUnitCheck(data, 0);
    }

    public void OneTestEnemySpawn()
    {
        var data = DataTableManager.AllyTable.Get(1540022001);
        placementManager.PlaceUnitCheck(data, 0);
    }

    public void TimeScaleOne()
    {
        Time.timeScale = 1f;
    }

    public void TimeScaleTwice()
    {
        Time.timeScale = 2f;
    }

    public void TimeScaleFifth()
    {
        Time.timeScale = 3f;
    }

    public void StagePlusOne()
    {
        Variables.Stage += 1;
    }

    public void StagePlusTen()
    {

        Variables.Stage = ((Variables.Stage / 10) + 1) * 10;
    }

    public void StageHundred()
    {
        Variables.Stage = 99;
    }
}
