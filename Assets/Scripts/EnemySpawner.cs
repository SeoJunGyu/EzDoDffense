using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyUnit prefab;
    public List<Transform> wayPoint;
    private Vector3[] way;

    private List<EnemyUnit> enemies = new List<EnemyUnit>();

    private void Awake()
    {
        way = new Vector3[wayPoint.Count];
        for(int i = 0; i < wayPoint.Count; i++)
        {
            way[i] = wayPoint[i].position;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //CreateEnemy();
            var data = DataTableManager.EnemyTable.Get(60050050001);
            Debug.Log(data);
        }
    }
    
    public void CreateEnemy()
    {
        Debug.Log($"{transform.position} / {way[0]}");
        var enemy = Instantiate(prefab, transform.position, transform.rotation);
        Debug.Log($"{enemy.transform.position}");
        enemy.SetTarget(way);

        enemies.Add(enemy);
    }
}
