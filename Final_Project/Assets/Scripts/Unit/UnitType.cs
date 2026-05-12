using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Type", menuName = "Unit System/Unit Type")]
public class UnitType : ScriptableObject
{
    [Header("Unit Information")]
    public string unitName = "Unit";
    public string unitDescription = "A combat unit";

    [Header("Health")]
    public int maxHealth = 100;

    [Header("Move Settings")]
    public int moveDistance = 4;

    [Header("Shoot Settings")]
    public bool hasShootAction = false;
    public int shootRange = 5;
    public int shootDamage = 20;

    [Header("Sword Settings")]
    public bool hasSwordAction = false;
    public int swordRange = 1;
    public int swordDamage = 50;

    [Header("Grenade Settings")]
    public bool hasGrenadeAction = false;
    public int grenadeRange = 3;
}
