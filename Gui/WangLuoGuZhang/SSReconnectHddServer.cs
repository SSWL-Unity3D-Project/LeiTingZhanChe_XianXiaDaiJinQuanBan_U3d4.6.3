using UnityEngine;

public class SSReconnectHddServer : MonoBehaviour
{
    float m_LastTimeVal = 0f;
    bool IsBackMovie = false;
	// Use this for initialization
	void Start()
    {
        m_LastTimeVal = Time.time;
    }
	
	// Update is called once per frame
	void Update()
    {
        if (Time.time - m_LastTimeVal > 3f && IsBackMovie == false)
        {
            m_LastTimeVal = Time.time;
            IsBackMovie = true;
            BackMovieScene();
        }
    }

    void BackMovieScene()
    {
        if (Application.loadedLevel != (int)GameLevel.Movie)
        {
            XkGameCtrl.ResetGameInfo();
            if (!XkGameCtrl.IsGameOnQuit)
            {
                System.GC.Collect();
                Resources.UnloadUnusedAssets();
                Application.LoadLevel((int)GameLevel.Movie);
            }
        }
    }
}
