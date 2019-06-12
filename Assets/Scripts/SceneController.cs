using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Vuforia;

public class SceneController : MonoBehaviour, ITrackableEventHandler {
     
    private TrackableBehaviour mTrackableBehaviour;
     
     
    void Start () {
        Time.timeScale = 1;
        
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
     
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)
        {
            switch( mTrackableBehaviour.TrackableName ){
                case "SnakeGameQR" :
                    SceneManager.LoadScene("SnakeScene");
                break;

                case "WaterGameQR" :
                    SceneManager.LoadScene("MenuScene");
                break;
            }
        }
    }

    public void StartScan()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void BackToMainScrene()
    {
        SceneManager.LoadScene("VuforiaScene");
    }
}