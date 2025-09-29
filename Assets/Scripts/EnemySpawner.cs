using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    public EnemyUnit prefab;
    public List<Transform> wayPoint;
    private Vector3[] way;

    private List<EnemyUnit> enemies = new List<EnemyUnit>();
    private int enemyCount = 0;
    public List<EnemyUnit> GetEnemies
    {
        get
        {
            return enemies;
        }
    }

    private EnemyData currentEnemyData;
    public EnemyData CurrentEnemyData { get => currentEnemyData; }

    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnTime = 0f;

    private float currentStage = 1;

    private void Awake()
    {
        Instance = this;

        way = new Vector3[wayPoint.Count];
        for(int i = 0; i < wayPoint.Count; i++)
        {
            way[i] = wayPoint[i].position;
        }

        GetCurrentEnemyData();

        for(int i = 0; i < 10; i++)
        {
            var enemy = Instantiate(prefab, transform.position, transform.rotation);
            enemies.Add(enemy);
            enemy.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Variables.Boss)
        {
            if (Variables.Boss.IsDead)
            {
                Variables.Boss = null;
            }
            else
            {
                return;
            }
        }

        if(Variables.Stage >= 100 && Variables.IsBoss)
        {
            return;
        }

        spawnTime += Time.deltaTime;
        if(spawnTime > spawnInterval)
        {
            if(currentStage != Variables.Stage)
            {
                currentStage = Variables.Stage;
                GetCurrentEnemyData();

                if(Variables.Stage < 100)
                {
                    Variables.BossSummoned = false;
                }
                enemyCount = 0;
            }
            CreateEnemy();
            spawnTime = 0f;
        }
    }
    
    public void CreateEnemy()
    {
        if(Variables.Stage >= 100 && Variables.BossSummoned)
        {
            return;
        }
        EnemyUnit enemy = null;
        foreach(var enem in enemies)
        {
            if (!enem.gameObject.activeSelf)
            {
                enemy = enem;
                enemy.transform.position = transform.position;
                enemy.transform.rotation = transform.rotation;
                break;
            }
        }

        if(enemy == null)
        {
            if(!Variables.BossSummoned && Variables.Boss == null)
            {
                enemy = Instantiate(prefab, transform.position, transform.rotation);
                enemies.Add(enemy);
            }
        }

        enemy.Setup(currentEnemyData);
        enemy.gameObject.SetActive(true);
        var visualModel = Instantiate(currentEnemyData.VisualModel, enemy.transform); //ÇÁ¸®Æé ¸ðµ¨ »ý¼º

        enemy.SetTarget(way);

        enemyCount++;
        Variables.EnemyTotalCount++;

        SetOnDeathEvent(enemy, visualModel);

        if(Variables.Stage % 10 == 0)
        {
            visualModel.transform.localScale *= 2f;
            Variables.Boss = enemy;
            Variables.BossSummoned = true;
        }
    }

    public void GetCurrentEnemyData() => currentEnemyData = DataTableManager.EnemyTable.GetStageEnemy(Variables.Stage);

    private void SetOnDeathEvent(EnemyUnit enemy, GameObject visualModel)
    {
        enemy.OnDeath += () => Destroy(visualModel);
        enemy.OnDeath += () => enemy.gameObject.SetActive(false);
    }
}
