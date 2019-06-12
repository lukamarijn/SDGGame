using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int scoreBoundary;
	public Text scoreDisplay;
    
    public event Action OnScoreReachedBoundary;
    public event Action<int> OnScoreChanged;

    private void Update()
	{
		scoreDisplay.text = score.ToString();
	}

    public void UpdateScore()
    {
        score++;
        Debug.Log("Score: "  + score);
        OnScoreChanged?.Invoke(score);

        if (score >= 10)
        {
            OnScoreReachedBoundary?.Invoke();
        }
    }
    
    
}
