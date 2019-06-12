using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{

    public event Action OnBorderSnakeCollision;
    public event Action<bool> OnSnakeAteFood;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Border") || other.CompareTag("SnakeBody"))
        {
            OnBorderSnakeCollision?.Invoke();
        }
        else if (other.CompareTag("Food"))
        {

            Destroy(other.gameObject);
            OnSnakeAteFood?.Invoke(true);
        }

    }
    
}
