
using System;
using System.Collections;
using UnityEngine;

public class WPCountdown: MonoBehaviour {

    public event Action NoTimeLeft;
    public event Action<float> OnTimeChange;

    public float seconds;
    private float _currSeconds;
    
    private void Start() {
        StartCoroutine(nameof(StartCountdown));
    }

    private IEnumerator StartCountdown() {
        _currSeconds = seconds;

        while (_currSeconds >= 0) {
            OnTimeChange.Invoke(_currSeconds);
            yield return new WaitForSeconds(1.0f);
			_currSeconds--;
		}
        
        NoTimeLeft.Invoke();
        yield return null;

    }
}
