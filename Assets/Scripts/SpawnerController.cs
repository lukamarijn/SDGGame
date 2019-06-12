using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SpawnerController : MonoBehaviour
{

    private Background background;
    public HashSet<Vector2Int> emptyCells = new HashSet<Vector2Int>();



    public event Action<GameObject> OnSnakeHeadSpawned;

    public event Action<GameObject> OnSnakeBodyPartSpawned;

    public void SpawnSnakeHead(GameObject gameObject, Vector3 vector3)
    {
        GameObject snakeHead = Instantiate(gameObject, vector3, Quaternion.identity);
        OnSnakeHeadSpawned?.Invoke(snakeHead);
            
    }

    public void SpawnSnakeBodyPart(GameObject gameObject, Vector3 vector3)
    {
        GameObject snakeBodyPart = Instantiate(gameObject, vector3, Quaternion.identity);
        OnSnakeBodyPartSpawned?.Invoke(snakeBodyPart);
            
    }

    public void SpawnFood(GameObject gameObject)
    {
        Random randomizer = new Random();
        Vector2Int[] asArray = emptyCells.ToArray();
        Vector2Int randomEmptyCellVector = asArray[randomizer.Next(asArray.Length)];
        Vector3 vector3 = new Vector3(randomEmptyCellVector.x, randomEmptyCellVector.y);
        Instantiate(gameObject, vector3, Quaternion.identity);

        emptyCells.Remove(randomEmptyCellVector);
    }

    public void Initialize(Background background)
    {
        this.background = background;
        int borderOffset = 1;
        int backgroundMinX = (int) this.background.GetComponent<SpriteRenderer>().bounds.min.x + borderOffset;
        int backgroundMaxX = (int) this.background.GetComponent<SpriteRenderer>().bounds.max.x;
        int backgroundMinY = (int) this.background.GetComponent<SpriteRenderer>().bounds.min.y + borderOffset;
        int backgroundMaxY = (int) this.background.GetComponent<SpriteRenderer>().bounds.max.y;

        Debug.Log("Min x: " + backgroundMinX);
        Debug.Log("Min y: " + backgroundMinY);
        Debug.Log("Max y: " + backgroundMaxY);
        Debug.Log("Max x: " + backgroundMaxX);
        
//        start i and j index at 1 instead of 0 and go until second last value of x and y to add a padding of 1 around the grid
        for (int i = backgroundMinX; i < backgroundMaxX; i++)
        {
            for (int j = backgroundMinY; j < backgroundMaxY; j++)
            {
                emptyCells.Add(new Vector2Int(i, j));
            }
        }

    }

    public void AddEmptyCell(Vector3 vector3)
    {
        Vector2Int vector2 = new Vector2Int((int) vector3.x, (int) vector3.y);
        emptyCells.Add(vector2);
    }

    public void RemoveEmptyCell(Vector3 vector3)
    {
        Vector2Int vector2 = new Vector2Int((int) vector3.x, (int) vector3.y);
        emptyCells.Remove(vector2);
    }

    
}
