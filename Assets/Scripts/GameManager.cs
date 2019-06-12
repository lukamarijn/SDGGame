using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private SnakePartsController snakePartsController;
	[SerializeField] private SnakeController snakeController;
	[SerializeField] private SpawnerController spawnerController;
	[SerializeField] private Background background;
	[SerializeField] private FoodController foodController;
	[SerializeField] private ScoreManager scoreManager;
	[SerializeField] private UIManager uiManager;
	private float snakeSpeed;
	private float initialWaitTime = 1.0f;
	private float snakeSpeedIncrease = 0.075f;
	
	
	private void Start()
	{
		snakeSpeed = 0.5f;
		spawnerController.Initialize(background);
		foodController.Initialize(spawnerController);
		snakeController.Initialize(snakePartsController, spawnerController, foodController, scoreManager);
//		TODO your own method instead of RestartGame?
		snakeController.OnPlayerHitWallOrSnake += RestartGame;
		scoreManager.OnScoreReachedBoundary += GameWon;

		scoreManager.OnScoreChanged += score => ChangeSnakeSpeed(score);
		
//		InvokeRepeating("MoveSnakeHeadDirection", 1.0f, 0.5f);
//
//		while (true)
//		{
//			yield return new WaitForSeconds(snakeSpeed);
//			MoveSnakeHeadDirection();
//			
//		}

//		WaitFunction func = Wait;
//		StartCoroutine(func(2));
//		

		StartCoroutine(MoveSnakeInInterval(initialWaitTime));

	}

	private void ChangeSnakeSpeed(int score)
	{
		if (score % 2 == 0)
		{
			snakeSpeed -= snakeSpeedIncrease;
		}
	}

	IEnumerator MoveSnakeInInterval(float f)
	{
		yield return new WaitForSeconds(f);
		Debug.Log(f + " " + Time.time);
		while( true )
		{
            MoveSnakeHeadDirection();
			Debug.Log ("current snake speed is "+ snakeSpeed);
			yield return new WaitForSeconds(snakeSpeed) ;
		}
	}

  //TODO Opposite direction possible if step keys are spammed fast enough
  public void SnakeMoveNorth()
  {
    snakeController.Move((int) SnakeController.headDirections.North);
  }

  public void SnakeMoveEast()
  {
    snakeController.Move((int) SnakeController.headDirections.East);
  }

  public void SnakeMoveSouth()
  {
    snakeController.Move((int) SnakeController.headDirections.South);
  }

  public void SnakeMoveWest()
  {
    snakeController.Move((int) SnakeController.headDirections.West);
  }


  // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
	        snakeController.Move((int) SnakeController.headDirections.North);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
	        snakeController.Move((int) SnakeController.headDirections.East);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
	        snakeController.Move((int) SnakeController.headDirections.South);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
	        snakeController.Move((int) SnakeController.headDirections.West);
        }
    }

    private void MoveSnakeHeadDirection()
    {
	    snakeController.Move();
    }

//    TODO Restart game/ win/lose screen?
    private void RestartGame()
    {
	    Time.timeScale = 0;
	    uiManager.SetLoseScreen();
    }

    private void GameWon()
    {
	    Time.timeScale = 0;
	    uiManager.SetWinScreen();
    }
}

