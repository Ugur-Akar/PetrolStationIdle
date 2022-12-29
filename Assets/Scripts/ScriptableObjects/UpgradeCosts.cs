using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeCosts", order = 1)]
public class UpgradeCosts : ScriptableObject
{
    public int[] sourceSpeedCosts;
    public int[] sourceStackCosts;
    public int[] pumpStackCosts;

}

