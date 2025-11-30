using NUnit.Framework;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<Enemy> enemiesInTrigger = new List<Enemy>();
    public List<Boss> bossesInTrigger = new List<Boss>();

    public void AddEnemy(Enemy enemy)
    {
        enemiesInTrigger.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemiesInTrigger.Remove(enemy);
    }

    public void AddBoss(Boss boss)
    {
        bossesInTrigger.Add(boss);
    }

    public void RemoveBoss(Boss boss)
    {
        bossesInTrigger.Remove(boss);
    }
}
