using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyUnit prefab;
    public List<Transform> wayPoint;
    private Vector3[] way;

    private List<EnemyUnit> enemies = new List<EnemyUnit>();
    private int enemyCount = 0;

    private EnemyData currentEnemyData;

    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnTime = 0f;

    private void Awake()
    {
        way = new Vector3[wayPoint.Count];
        for(int i = 0; i < wayPoint.Count; i++)
        {
            way[i] = wayPoint[i].position;
        }

        GetCurrentEnemyData();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //CreateEnemy();
            //var data = DataTableManager.EnemyTable.Get(10050050001);
            //Debug.Log(data);
        }

        spawnTime += Time.deltaTime;
        if(spawnTime > spawnInterval)
        {
            if(enemyCount >= 40)
            {
                EnemyUpgrade();
                GetCurrentEnemyData();

                enemyCount = 0;
            }
            CreateEnemy();
            spawnTime = 0f;
        }
    }
    
    public void CreateEnemy()
    {
        var enemy = Instantiate(prefab, transform.position, transform.rotation);
        Instantiate(currentEnemyData.VisualModel, enemy.transform); //ÇÁ¸®Æé ¸ðµ¨ »ý¼º

        enemy.SetTarget(way);

        enemies.Add(enemy);

        enemyCount++;
        Variables.EnemyTotalCount++;
    }

    public void GetCurrentEnemyData() => currentEnemyData = DataTableManager.EnemyTable.GetStageEnemy(Variables.Stage);

    public void EnemyUpgrade() => Variables.Stage++;
}
