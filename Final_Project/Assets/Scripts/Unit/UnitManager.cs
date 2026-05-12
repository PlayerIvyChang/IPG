using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Units> unitList;
    private List<Units> friendlyUnitList;
    private List<Units> enemyUnitList;

    private void Awake()
    {
        unitList = new List<Units>();
        friendlyUnitList = new List<Units>();
        enemyUnitList = new List<Units>();

        if (Instance != null)
        {
            Debug.LogError("There is more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {

        Units.OnAnyUnitSpawned += Units_OnAnyUnitSpawned;
        Units.OnAnyUnitDead += Units_OnAnyUnitDead;
    }
    
    private void Units_OnAnyUnitSpawned(object sender, System.EventArgs e)
    {
        Units unit = sender as Units;
        unitList.Add(unit);
        if (unit.IsEnemyUnit())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Units_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Units unit = sender as Units;
        unitList.Remove(unit);
        if (unit.IsEnemyUnit())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Units> GetUnitList()
    {
        return unitList;
    }
    public List<Units> GetEnemyUnitList()
    {
        return enemyUnitList;
    }
    public List<Units> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }
}
