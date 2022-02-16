using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class holding tile states 
 */
public class TileState : MonoBehaviour
{
    public enum State
    {
        Empty,              //1
        //Player,             //2
        //Door,               //3
        //Key,                //4
        Enemy,              //5
        HealthPickup,       //6
        //ItemPickup          //7
    };

    public GameObject tile;
    public State currState;
}
