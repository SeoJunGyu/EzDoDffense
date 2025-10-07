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
    public Dictionary<int, int> GradeUpgradeBaseGold = new Dictionary<int, int>();
    public Dictionary<int, int> GradeUpgradeGold = new Dictionary<int, int>();
    public Dictionary<int, int> GradeUpgradeGem = new Dictionary<int, int>();
    public Dictionary<int, int> TypeUpgradeGold = new Dictionary<int, int>();
    public Dictionary<int, int> GradeUpgradeSave = new Dictionary<int, int>();
    public Dictionary<int, int> TypeUpgradeSave = new Dictionary<int, int>();

    public List<int> UnitPrice = new List<int>();

    private bool IsGotcha = false;

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

        for(int i = 1; i < 6; i++)
        {
            if(i == 1)
            {
                GradeUpgradeBaseGold.Add(i, 10);
                GradeUpgradeGold.Add(i, 10);
                continue;
            }
            if (i == 2)
            {
                GradeUpgradeBaseGold.Add(i, 15);
                GradeUpgradeGold.Add(i, 15);
                continue;
            }
            if(i == 3)
            {
                GradeUpgradeBaseGold.Add(i, 20);
                GradeUpgradeGold.Add(i, 20);
                continue;
            }
            if (i == 4)
            {
                GradeUpgradeBaseGold.Add(i, 50);
                GradeUpgradeGold.Add(i, 50);
                continue;
            }
            if (i == 5)
            {
                GradeUpgradeBaseGold.Add(i, 80);
                GradeUpgradeGold.Add(i, 80);
                continue;
            }
        }

        for (int i = 1; i < 4; i++)
        {
            TypeUpgradeGold.Add(i, 50);
        }

        for(int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    UnitPrice.Add(40);
                    break;
                case 1:
                    UnitPrice.Add(60);
                    break;
                case 2:
                    UnitPrice.Add(150);
                    break;
                case 3:
                    UnitPrice.Add(350);
                    break;
                case 4:
                    UnitPrice.Add(500);
                    break;
            }
        }
    }

    public bool FindSameUnit(AllyData data, float rndValue)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId;

            if (IsPlace)
            {
                return TryPlaceOnSlot(slot, data, 50, rndValue);
            }
        }

        return false;
    }

    public void PlaceInSocket(AllyData data, float rndValue)
    {
        foreach (var slot in slots)
        {
            bool IsPlace = slot.UnitId == 0 || (slot.SocketInCount < 3 && Variables.SlotCount > 0 && data.Unit_ID == slot.UnitId);

            if (IsPlace && TryPlaceOnSlot(slot, data, 50, rndValue))
            {
                return ;
            }
        }
    }

    //추후 cost 변경되어 소환해야하는 경우 사용
    /*
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
    */

    public bool TryPlaceOnSlot(Clickable slot, AllyData data, int cost, float rndValue)
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
            var type = TypeUpgradeSave.ContainsKey(data.Unit_Type) ? TypeUpgradeSave[data.Unit_Type] : 0;
            unit.Setup(data, grade, type);
            unit.gameObject.SetActive(true);

            if(data.Unit_Grade >= 4)
            {
                GameManager.Instance.NoticeRareSpawn(data, rndValue, IsGotcha);
                Handheld.Vibrate();
            }

            unit.OnSynthesis += () => Destroy(visualModel);
            unit.OnSynthesis += () => unit.gameObject.SetActive(false);

            AudioManager.Instance.PlaySpawn(unit.transform.position);

            IsGotcha = false;

            return true;
        }

        IsGotcha = false;
        return false;
    }

    public void PlaceAllyAllRandom()
    {
        IsGotcha = true;

        var data = DataTableManager.AllyTable.GetAllRandom();
        float rndValue = DataTableManager.AllRandomTable.Get(data.Unit_ID).Random_P;
        if (!FindSameUnit(data, rndValue))
        {
            PlaceInSocket(data, rndValue);
        }
    }

    public void PlaceAllyNormalRandom()
    {
        IsGotcha = true;

        var data = DataTableManager.AllyTable.GetNormalRandom();
        float rndValue = DataTableManager.AllRandomTable.Get(data.Unit_ID).Random_P;
        if (!FindSameUnit(data, rndValue))
        {
            PlaceInSocket(data, rndValue);
        }
    }

    public void PlaceAllyPiercingRandom()
    {
        IsGotcha = true;

        var data = DataTableManager.AllyTable.GetPiercingRandom();
        float rndValue = DataTableManager.AllRandomTable.Get(data.Unit_ID).Random_P;
        if (!FindSameUnit(data, rndValue))
        {
            PlaceInSocket(data, rndValue);
        }
    }

    public void PlaceAllyMagicalRandom()
    {
        IsGotcha = true;

        var data = DataTableManager.AllyTable.GetMagicalRandom();
        float rndValue = DataTableManager.AllRandomTable.Get(data.Unit_ID).Random_P;
        if (!FindSameUnit(data, rndValue))
        {
            PlaceInSocket(data, rndValue);
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

    private bool TryPayGem(int cost)
    {
        if(Variables.Gem >= cost)
        {
            Variables.Gem -= cost;
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

        AllSetUp();

        AudioManager.Instance.PlaySynthesis(Vector3.zero);

        Variables.SelectedSlot.DeselectThis();
    }

    public void UnitSale()
    {
        int grade = Variables.SelectedSlot.CurrentData.Unit_Grade;

        Variables.SelectedSlot.UnitDelete();

        switch (grade)
        {
            case 1:
                Variables.Gold += 40;
                break;
            case 2:
                Variables.Gold += 60;
                break;
            case 3:
                Variables.Gold += 150;
                break;
            case 4:
                Variables.Gold += 350;
                break;
            case 5:
                Variables.Gold += 500;
                break;
        }

        AllSetUp();

        AudioManager.Instance.PlaySail(Vector3.zero);
    }

    //강화 실행
    public void GradeUpgrade(int grade)
    {
        int level = GradeUpgradeSave.TryGetValue(grade, out var saved) ? saved : 0;
        if (level >= 10) return;

        bool gemStage = IsGemStage(grade, level);

        //골드 결제
        int goldCostNow = gemStage ? 0 : CalcGradeGold(grade, level);
        if (goldCostNow > 0 && !TryPay(goldCostNow))
        {
            return;
        }

        //보석 결제
        if (gemStage)
        {
            int gemCostNow = CalcGem(level);
            if (gemCostNow > 0 && !TryPayGem(gemCostNow))
            {
                return;
            }
        }

        //강화 성공
        level++;
        GradeUpgradeSave[grade] = level;

        AudioManager.Instance.Playenforce();

        //다음 비용
        if (level >= 10)
        {
            GradeUpgradeGold[grade] = 0;
            GradeUpgradeGem[grade] = 0;
        }
        else
        {
            bool nextGemStage = IsGemStage(grade, level);
            GradeUpgradeGold[grade] = nextGemStage ? 0 : CalcGradeGold(grade, level);
            GradeUpgradeGem[grade] = nextGemStage ? CalcGem(level) : 0;
        }

        AllGradeUpgradeSetUp(level, grade);
    }

    public void TypeUpgrade(int type)
    {
        int level = TypeUpgradeSave.TryGetValue(type, out var saved) ? saved : 0;
        if (level >= 10) return;

        //골드 결제
        int goldCostNow = CalcTypeGold(level);
        if (goldCostNow > 0 && !TryPay(goldCostNow))
        {
            return;
        }

        //강화 성공
        level++;
        TypeUpgradeSave[type] = level;

        AudioManager.Instance.Playenforce();

        //다음 비용
        if (level >= 10)
        {
            TypeUpgradeGold[type] = 0;
        }
        else
        {
            TypeUpgradeGold[type] = CalcTypeGold(level);
        }

        AllTypeUpgradeSetUp(level, type);
    }

    private bool IsGemStage(int grade, int level) => (grade == 4 || grade == 5) && level >= 5;

    private int CalcGradeGold(int grade, int level) => Mathf.CeilToInt(GradeUpgradeBaseGold[grade] * Mathf.Pow(1.25f, level));
    private int CalcTypeGold(int level) => Mathf.RoundToInt(50f * Mathf.Pow(1.2f, level));

    private int CalcGem(int level) => (level - 4);

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

    public void AllTypeUpgradeSetUp(int typeUpdate, int type)
    {
        foreach (var slot in slots)
        {
            if (slot.SocketInCount > 0)
            {
                slot.AllTypeUpgradeUnitSetup(type, typeUpdate);
            }
        }
    }

    public void AllSetUp()
    {
        foreach(var slot in slots)
        {
            if(slot.SocketInCount > 0)
            {
                slot.AllSetUp();
            }
        }
    }
}
