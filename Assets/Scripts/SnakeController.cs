using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeController : MonoBehaviour
{
    private Vector2Int headPosition;
    private int snakeHeadDirection;
    private List<GameObject> snakeParts = new List<GameObject>();
    private SnakePartsController snakePartsController;
    private Deque<GameObject> snakeBody = new Deque<GameObject>();
    private SpawnerController spawnerController;
    private FoodController foodController;
    private ScoreManager scoreManager;
    private SnakeHead snakeHead;
    private float oldSnakeHeadX;
    private float oldSnakeHeadY;
    public event Action OnPlayerHitWallOrSnake;
    private bool foodEaten;

    private Dictionary<int, int[]> snakeStepMapping = new Dictionary<int, int[]>
    {
        [0] = new[] {0, 1},
        [1] = new[] {1, 0},
        [2] = new[] {0, -1},
        [3] = new[] {-1, 0}
    };

    public enum headDirections
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
    
    private void Start()
    {
        headPosition = new Vector2Int(0, 8);
        snakeHeadDirection = (int) headDirections.North;

    }

    public void Move(int direction)
    {
        if (Math.Abs(direction - snakeHeadDirection) != 2)
        {

            snakeHeadDirection = direction;

        }
        
        switch (snakeHeadDirection)
        {
            case 0:
                snakeHead.transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
            case 1:
                snakeHead.transform.rotation = Quaternion.Euler(Vector3.forward * 270);
                break;
            case 2:
                snakeHead.transform.rotation = Quaternion.Euler(Vector3.forward * 180);
                break;
            case 3:
                snakeHead.transform.rotation = Quaternion.Euler(Vector3.forward * 90);
                break;
        }
        
    }

    public void Move()
    {
        GameObject snakeHead = snakeBody.RemoveBack();
            Vector3 oldSnakeHeadVector = snakeHead.transform.position;
            oldSnakeHeadX = oldSnakeHeadVector.x;
            oldSnakeHeadY = oldSnakeHeadVector.y;

//            update Snakehead to new coordinates
            var newSnakeHeadX = oldSnakeHeadX + snakeStepMapping[snakeHeadDirection][0];
            var newSnakeHeadY = oldSnakeHeadY + snakeStepMapping[snakeHeadDirection][1];
            Vector3 newSnakeHeadVector = new Vector3(newSnakeHeadX, newSnakeHeadY);
            

//                put prepareToSpawnSnakeBodyPart() after setting position of new snake head
//                otherwise foodEaten cannot be set to true through collision event before snake body part is spawned
            snakeHead.transform.position = newSnakeHeadVector;
            
            spawnerController.AddEmptyCell(oldSnakeHeadVector);
            spawnerController.RemoveEmptyCell(newSnakeHeadVector);
            
            PrepareToSpawnSnakeBodyPart();
            snakeBody.AddBack(snakeHead);
            
//          TODO  check if snake ate first
            RemoveSnakeTail();
            SetFoodEaten(false);
    }

    private void InstantiateSnakeHead()
    {
        GameObject snakeHead = snakeParts.Find(i => i.CompareTag("SnakeHead"));
        Vector3 snakeHeadPos = new Vector3(headPosition.x, headPosition.y);
        spawnerController.SpawnSnakeHead(snakeHead, snakeHeadPos);
        spawnerController.RemoveEmptyCell(snakeHeadPos);
        
        
        foodController.InitializeFood();
        
    }

    public void Initialize(SnakePartsController snakePartsController, SpawnerController spawnerController,
        FoodController foodController, ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
        this.snakePartsController = snakePartsController;
		foreach (Transform snakePart in this.snakePartsController.transform)
		{
			snakeParts.Add(snakePart.gameObject);	
		}

        this.foodController = foodController;
            
        this.spawnerController = spawnerController;
        this.spawnerController.OnSnakeHeadSpawned += snakeHead => SnakeHeadCreated(snakeHead);
        this.spawnerController.OnSnakeBodyPartSpawned += snakeBodyPart => PersistSnakePart(snakeBodyPart);
        
        InstantiateSnakeHead();
    }

    private void SnakeHitSomething()
    {
        OnPlayerHitWallOrSnake.Invoke();
    }

    private void SnakeHeadCreated(GameObject gameObject)
    {
        snakeHead = gameObject.GetComponent<SnakeHead>();
        snakeHead.OnBorderSnakeCollision += SnakeHitSomething;
        snakeHead.OnSnakeAteFood += snakeAteFood =>
        {
            SetFoodEaten(snakeAteFood);
            foodController.InitializeFood();
            scoreManager.UpdateScore();
            
        };
        PersistSnakePart(gameObject);
        
    }

    private void SetFoodEaten(bool foodEaten)
    {
        this.foodEaten = foodEaten;
    }

    private void PersistSnakePart(GameObject snakePart)
    {
        snakeBody.AddBack(snakePart);
    }

    private void PrepareToSpawnSnakeBodyPart()
    {
//        make sure to spawn a new body part at the old snake head position
//        only if the snake body is longer than just the head or the snake ate food
//        TODO Also check if food has been eaten with an OR! Otherwise snake never gets longer
        if (snakeBody.Count > 0 || foodEaten)
        {
            Vector3 snakeBodyPartPos = new Vector3(oldSnakeHeadX, oldSnakeHeadY);
            GameObject  snakeBodyPart = snakeParts.Find(i => i.CompareTag("SnakeBody"));
            spawnerController.SpawnSnakeBodyPart(snakeBodyPart, snakeBodyPartPos);
            spawnerController.RemoveEmptyCell(snakeBodyPartPos);
        }
        
    }

    private void RemoveSnakeTail()
    {
        if (snakeBody.Count > 1 && foodEaten != true)
        {
            GameObject snakeTail = snakeBody.RemoveFront();
            Destroy(snakeTail);
            spawnerController.AddEmptyCell(snakeTail.transform.position);
        }
    }
}
