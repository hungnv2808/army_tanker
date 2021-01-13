using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataTankChampion", menuName = "ScriptableObject/DataTankChampion", order = 2)]
public class DataTankChampion : ScriptableObject
{
    public TankChampion[] Champions;
}
[System.Serializable]
public class TankChampion {
    public int Index;
    public TankProperty[] Healthy;
    public TankProperty[] MoveSpeed;
    public TankProperty[] Damage;
}
[System.Serializable]
public class TankProperty {
    public int Stat;
    public int Price;
}
