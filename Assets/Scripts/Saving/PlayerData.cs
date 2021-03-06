using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int money;
    public int weapon;
    public int stageNumber;
    public int sceneNumber;


    public PlayerData (string saveString)
    {
        string[] tempSave = saveString.Split('|'); //  PlayerPrefs.GetString("SaveState").Split('|');

        money = int.Parse(tempSave[0]);
        // inventory = Inventory.Parse(tempSave[0]);
        health = 10; // int.Parse(tempSave[4]);
        stageNumber = 1; // int.Parse(tempSave[5]);
        sceneNumber = 1; // int.Parse(tempSave[6]);

    }
}
