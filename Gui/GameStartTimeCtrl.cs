using UnityEngine;

public class GameStartTimeCtrl : MonoBehaviour
{
    //public GameObject m_TiShiUI;
	public Texture[] TimeTexture;
	UITexture StartTimeTexture;
	int TimeCount;
	bool IsInitPlay;
	static GameStartTimeCtrl _Instance;
	public static GameStartTimeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		StartTimeTexture = GetComponent<UITexture>();
		gameObject.SetActive(false);
        //if (m_TiShiUI != null)
        //{
        //    m_TiShiUI.SetActive(false);
        //}
    }

	public void InitPlayStartTimeUI()
	{
		if (IsInitPlay) {
			return;
		}
		IsInitPlay = true;
        //gameObject.SetActive(true);
        //if (m_TiShiUI != null)
        //{
        //    m_TiShiUI.SetActive(true);
        //}
        //PlayStartTimeUI();
        StartPlayGame();
    }

	void PlayStartTimeUI()
	{
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}
		
		tweenScaleCom = gameObject.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = 1.2f;
		tweenScaleCom.from = new Vector3(3f, 3f, 1f);
		tweenScaleCom.to = new Vector3(1f, 1f, 1f);
		EventDelegate.Add(tweenScaleCom.onFinished, delegate{
			ChangeStartTimeUI();
		});
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

	void ChangeStartTimeUI()
	{
		TimeCount++;
		if (TimeCount >= TimeTexture.Length)
        {
			Debug.Log("Unity:"+"ChangeStartTimeUI -> change over!");
            //if (m_TiShiUI != null)
            //{
            //    Destroy(m_TiShiUI);
            //}
			gameObject.SetActive(false);
			ScreenDanHeiCtrl.GetInstance().ActiveGameUiCamera();
			XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
			GameTimeCtrl.GetInstance().ActiveIsCheckTimeSprite();
			return;
		}

		//Debug.Log("Unity:"+"ChangeStartTimeUI -> TimeCount "+TimeCount);
		StartTimeTexture.mainTexture = TimeTexture[TimeCount];
		PlayStartTimeUI();
	}

    void StartPlayGame()
    {
        gameObject.SetActive(false);
        ScreenDanHeiCtrl.GetInstance().ActiveGameUiCamera();
        XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
        GameTimeCtrl.GetInstance().ActiveIsCheckTimeSprite();
    }
}