using UnityEngine;

public class XKPlayerScoreCtrl : MonoBehaviour
{
	public PlayerEnum PlayerIndex;
    /// <summary>
    /// 距抽奖分数信息UI.
    /// </summary>
    public SSGameNumUI m_JuChouJiangScore;
    /// <summary>
    /// 原先玩家实际得分展示.
    /// </summary>
	public UISprite[] PlayerJF;
	static XKPlayerScoreCtrl _InstanceP1;
	static XKPlayerScoreCtrl _InstanceP2;
	static XKPlayerScoreCtrl _InstanceP3;
	static XKPlayerScoreCtrl _InstanceP4;
	public static XKPlayerScoreCtrl GetInstance(PlayerEnum indexPlayer)
	{
		XKPlayerScoreCtrl instanceVal = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			instanceVal = _InstanceP1;
			break;
		case PlayerEnum.PlayerTwo:
			instanceVal = _InstanceP2;
			break;
		case PlayerEnum.PlayerThree:
			instanceVal = _InstanceP3;
			break;
		case PlayerEnum.PlayerFour:
			instanceVal = _InstanceP4;
			break;
		}
		return instanceVal;
	}

    private void Start()
    {
        Init();
    }

    bool IsInit = false;
    // Use this for initialization
    internal void Init()
	{
        if (IsInit == true)
        {
            return;
        }
        IsInit = true;

        switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			_InstanceP1 = this;
			break;
		case PlayerEnum.PlayerTwo:
			_InstanceP2 = this;
			break;
		case PlayerEnum.PlayerThree:
			_InstanceP3 = this;
			break;
		case PlayerEnum.PlayerFour:
			_InstanceP4 = this;
			break;
		}
		ZuiGaoFenObj.SetActive(false);
		//SetScoreSprite();

		if (!XkGameCtrl.GetIsActivePlayer(PlayerIndex)) {
			SetActivePlayerScore(false);
		}
	}

	public static void ShowPlayerScore(PlayerEnum indexPlayer)
	{
		if (indexPlayer == PlayerEnum.Null) {
			if (XkGameCtrl.IsActivePlayerOne) {
				_InstanceP1.SetActivePlayerScore(true);
			}

			if (XkGameCtrl.IsActivePlayerTwo) {
				_InstanceP2.SetActivePlayerScore(true);
			}
			
			if (XkGameCtrl.IsActivePlayerThree) {
				_InstanceP3.SetActivePlayerScore(true);
			}
			
			if (XkGameCtrl.IsActivePlayerFour) {
				_InstanceP4.SetActivePlayerScore(true);
			}
			return;
		}

		XKPlayerScoreCtrl instanceVal = GetInstance(indexPlayer);
		if (instanceVal == null)
        {
			return;
		}
		instanceVal.SetActivePlayerScore(true);
        //instanceVal.SetScoreSprite();
    }
	
	public static void HiddenPlayerScore(PlayerEnum indexPlayer)
	{
		if (indexPlayer == PlayerEnum.Null) {
			_InstanceP1.SetActivePlayerScore(false);
			_InstanceP2.SetActivePlayerScore(false);
			_InstanceP3.SetActivePlayerScore(false);
			_InstanceP4.SetActivePlayerScore(false);
			return;
		}

		XKPlayerScoreCtrl instanceVal = GetInstance(indexPlayer);
		if (instanceVal == null) {
			return;
		}
		instanceVal.SetActivePlayerScore(false);
	}

	void SetActivePlayerScore(bool isActive)
	{
		if (!isActive)
        {
			//SetScoreSprite();
			ZuiGaoFenObj.SetActive(false);
		}

		if (isActive != gameObject.activeSelf) {
			CheckPlayerZuiGaoFen(true);
            if (isActive == true)
            {
                //显示玩家距抽奖还差多少分数.
                SSPlayerScoreManage playerScoreManage = SSPlayerScoreManage.GetInstance(PlayerIndex);
                if (playerScoreManage != null)
                {
                    playerScoreManage.OnDisplayPlayerScore();
                }

                //设置距抽奖还差的最少分值信息.
                int score = 40000;
                if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
                {
                    score = XkGameCtrl.GetInstance().m_PingJiData.GetChouJiangMinScore();
                }
                SetJuChouJiangScore(score);
            }
		}
		OnEndMakeScoreToSmall();
		gameObject.SetActive(isActive);
	}

	public static void ChangePlayerScore(PlayerEnum indexPlayer)
	{
		XKPlayerScoreCtrl instanceVal = GetInstance(indexPlayer);
		if (instanceVal == null) {
			return;
		}
		//instanceVal.MakeScoreToBig();
		CheckPlayerZuiGaoFen();


        SSPlayerScoreManage playerScoreManage = SSPlayerScoreManage.GetInstance(indexPlayer);
        if (playerScoreManage != null && playerScoreManage.IsOnPlayerFenShuZuGou == false)
        {
            //玩家有新获得积分.
            int minChouJiangScore = 40000;
            if (XkGameCtrl.GetInstance() != null && XkGameCtrl.GetInstance().m_PingJiData != null)
            {
                minChouJiangScore = XkGameCtrl.GetInstance().m_PingJiData.GetChouJiangMinScore();
            }

            int playerScore = XkGameCtrl.GetPlayerJiFenValue(indexPlayer);
            //还差多少分数.
            int haiChaScoreVal = minChouJiangScore - playerScore;
            if (haiChaScoreVal <= 0)
            {
                haiChaScoreVal = 0;
            }

            XKPlayerScoreCtrl playerScoreCom = GetInstance(indexPlayer);
            if (playerScoreCom != null)
            {
                //设置还差多少分数.
                playerScoreCom.SetJuChouJiangScore(haiChaScoreVal);
            }
        }
    }

	bool IsToBigScore;
	float TimeLast;
	public GameObject ZuiGaoFenObj;
	static float TimeLastMaxScore;
	static void CheckPlayerZuiGaoFen(bool isHuLueTime = false)
	{
		//XkGameCtrl.CheckPlayerActiveNum();
		if (XkGameCtrl.PlayerActiveNum <= 0) {
			return;
		}

		if (Time.time - TimeLastMaxScore < 0.5f) {
			return;
		}
		TimeLastMaxScore = Time.time;

		int maxScore = 0;
		int indexVal = -1;
        PlayerEnum indexPlayer = PlayerEnum.Null;
		for (int i = 0; i < 4; i++) {
            indexPlayer = (PlayerEnum)(i + 1);
            if (XkGameCtrl.GetIsActivePlayer(indexPlayer) == false)
            {
                continue;
            }

            if (XkGameCtrl.GetIsDeathPlayer(indexPlayer) == true)
            {
                XKPlayerScoreCtrl playerScoreCom = GetInstance(indexPlayer);
                if (playerScoreCom != null)
                {
                    playerScoreCom.SetActiveZuiGaoFen(false);
                }
                continue;
            }
            //switch (i) {
            //case 0:
            //	if (!XkGameCtrl.IsActivePlayerOne) {
            //		continue;
            //	}
            //	break;
            //case 1:
            //	if (!XkGameCtrl.IsActivePlayerTwo) {
            //		continue;
            //	}
            //	break;
            //case 2:
            //	if (!XkGameCtrl.IsActivePlayerThree) {
            //		continue;
            //	}
            //	break;
            //case 3:
            //	if (!XkGameCtrl.IsActivePlayerFour) {
            //		continue;
            //	}
            //	break;
            //}

            if (XkGameCtrl.PlayerJiFenArray[i] > maxScore) {
				maxScore = XkGameCtrl.PlayerJiFenArray[i];
				indexVal = i;
			}
		}
		//Debug.Log("Unity:"+"CheckPlayerZuiGaoFen -> maxScore "+maxScore+", index "+indexVal);

        if (indexVal == -1)
        {
            return;
        }

		XKPlayerScoreCtrl playerScore = null;
		switch (indexVal) {
		case 0:
			playerScore = _InstanceP1;
			break;
		case 1:
			playerScore = _InstanceP2;
			break;
		case 2:
			playerScore = _InstanceP3;
			break;
		case 3:
			playerScore = _InstanceP4;
			break;
		}
		if (playerScore.ZuiGaoFenObj.activeSelf) {
			return;
		}

		if (_InstanceP1 != null) {
			_InstanceP1.SetActiveZuiGaoFen(playerScore==_InstanceP1);
		}

		if (_InstanceP2 != null) {
			_InstanceP2.SetActiveZuiGaoFen(playerScore==_InstanceP2);
		}

		if (_InstanceP3 != null) {
			_InstanceP3.SetActiveZuiGaoFen(playerScore==_InstanceP3);
		}

		if (_InstanceP4 != null) {
			_InstanceP4.SetActiveZuiGaoFen(playerScore==_InstanceP4);
		}
	}

	void SetActiveZuiGaoFen(bool isActive)
	{
		if (isActive) {
            //if ((JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())
            //    || JiFenJieMianCtrl.GetInstance() == null) {
            if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())
            {
                isActive = false;
			}
            else if (gameObject.activeInHierarchy == false)
            {
                isActive = false;
            }
		}
		ZuiGaoFenObj.SetActive(isActive);
	}

	void MakeScoreToBig()
	{
		if (Time.time - TimeLast > 0.5f && IsToBigScore) {
			IsToBigScore = false;
		}

		if (IsToBigScore) {
			SetScoreSprite();
			return;
		}
		IsToBigScore = true;
		TimeLast = Time.time;
		
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}
		
		tweenScaleCom = gameObject.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = 0.15f;
		tweenScaleCom.from = new Vector3(1f, 1f, 1f);
		tweenScaleCom.to = new Vector3(1.15f, 1.15f, 1f);
		EventDelegate.Add(tweenScaleCom.onFinished, delegate{
			OnEndMakeScoreToBig();
		});
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

	void SetScoreSprite()
	{
		int indexVal = (int)PlayerIndex - 1;
		int max = PlayerJF.Length;
		int numVal = XkGameCtrl.PlayerJiFenArray[indexVal];
        int maxVal = (int)Mathf.Pow(10, max) - 1;
        if (numVal > maxVal)
        {
            //分数大于UI可以展示的最大数值时.
            numVal = maxVal;
        }

		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("Unity:"+"valTmp *** "+valTmp);
			PlayerJF[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
	}

	void OnEndMakeScoreToBig()
	{
		SetScoreSprite();
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}
		tweenScaleCom = gameObject.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = 0.15f;
		tweenScaleCom.from = new Vector3(1.15f, 1.15f, 1f);
		tweenScaleCom.to = new Vector3(1f, 1f, 1f);
		EventDelegate.Add(tweenScaleCom.onFinished, delegate{
			OnEndMakeScoreToSmall();
		});
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

	void OnEndMakeScoreToSmall()
	{
		IsToBigScore = false;
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}
	}
    
    /// <summary>
    /// 设置距抽奖还差多少分.
    /// </summary>
    void SetJuChouJiangScore(int score)
    {
        //SSDebug.LogWarning("SetJuChouJiangScore -> score == " + score + ", PlayerIndex == " + PlayerIndex);
        if (m_JuChouJiangScore != null)
        {
            if (score <= 0)
            {
                //距离获得游戏抽奖还差0分.
                m_JuChouJiangScore.ShowNumUI(0);
                //玩家已经获得游戏抽奖机会.
                SSPlayerScoreManage playerScoreManage = SSPlayerScoreManage.GetInstance(PlayerIndex);
                if (playerScoreManage != null)
                {
                    playerScoreManage.OnPlayerFenShuZuGouChouJiang();
                }
            }
            else
            {
                m_JuChouJiangScore.ShowNumUI(score);
            }
        }
    }
}
