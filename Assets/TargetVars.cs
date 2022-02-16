using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class holding all target variables for learning 
 */
public class TargetVars : MonoBehaviour
{
    [HideInInspector] public int maxEnemies = 4;
    [HideInInspector] public int minEnemies = 1;
    [HideInInspector] public int maxHealthPickups = 3;
    [HideInInspector] public int minHealthPickups = 1;
    [HideInInspector] public int minDoors = 1;
    [HideInInspector] public int maxDoors = 3;

}
