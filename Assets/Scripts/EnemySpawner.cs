using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public EnemyUnit testTarget;

    private void Awake()
    {
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
        if (Input.GetMouseButtonDown(0))
        {
            //CreateEnemy();
            //var data = DataTableManager.EnemyTable.Get(10050050001);
            //Debug.Log(data);

            //testTarget.OnDamage(10f);
        }

        spawnTime += Time.deltaTime;
        if(spawnTime > spawnInterval)
        {
            if(enemyCount >= 40)
            {
                GetCurrentEnemyData();

                enemyCount = 0;
            }
            CreateEnemy();
            spawnTime = 0f;
        }
    }
    
    public void CreateEnemy()
    {
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
            enemy = Instantiate(prefab, transform.position, transform.rotation);
            enemies.Add(enemy);
        }

        enemy.Setup(currentEnemyData);
        enemy.gameObject.SetActive(true);
        var visualModel = Instantiate(currentEnemyData.VisualModel, enemy.transform); //ÇÁ¸®Æé ¸ðµ¨ »ý¼º

        enemy.SetTarget(way);

        enemyCount++;
        Variables.EnemyTotalCount++;

        enemy.OnDeath += () => Destroy(visualModel);
        enemy.OnDeath += () => enemy.gameObject.SetActive(false);
    }

    public void GetCurrentEnemyData() => currentEnemyData = DataTableManager.EnemyTable.GetStageEnemy(Variables.Stage);
}
