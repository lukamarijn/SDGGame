using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    private SpawnerController spawnerController;
    // Start is called before the first frame update

    public void InitializeFood()
    {
        spawnerController.SpawnFood(gameObject);
    }

    public void Initialize(SpawnerController spawnerController)
    {
        this.spawnerController = spawnerController;
    }
}
