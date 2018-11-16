using UnityEngine;

public class HuDunCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt;
	UISprite HuDunSprite;
	static HuDunCtrl _InstanceOne;
    public static HuDunCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}
	
	static HuDunCtrl _InstanceTwo;
	public static HuDunCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}
	
	static HuDunCtrl _InstanceThree;
	public static HuDunCtrl GetInstanceThree()
	{
		return _InstanceThree;
	}
	
	static HuDunCtrl _InstanceFour;
	public static HuDunCtrl GetInstanceFour()
	{
		return _InstanceFour;
	}

	public static HuDunCtrl GetInstance(PlayerEnum indexPlayer)
	{
		HuDunCtrl playerHD = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			playerHD = _InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			playerHD = _InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			playerHD = _InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			playerHD = _InstanceFour;
			break;
		}
		return playerHD;
	}

	// Use this for initialization
	void Start()
	{
		HuDunSprite = GetComponent<UISprite>();
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			break;
			
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			break;
			
		case PlayerEnum.PlayerThree:
			_InstanceThree = this;
			break;
			
		case PlayerEnum.PlayerFour:
			_InstanceFour = this;
			break;
		}
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		CheckHuDunUISprite();
	}

	float TimeStart;
	void CheckHuDunUISprite()
	{
		float keyHD = -HuDunTime;
		float fillVal = ((Time.time - TimeStart) / keyHD) + 1f;
		HuDunSprite.fillAmount = Mathf.Clamp01(fillVal);
		if (fillVal <= 0f) {
			HiddenHuDunUI();
		}
	}

	float HuDunTime;
	public void ShowHuDunUI(float timeVal = 1f)
	{
        //HuDunTime = timeVal;
        IsCanResetHuDunTime = true;
        HuDunTime = 60f;
        TimeStart = Time.time;
		HuDunSprite.fillAmount = 1f;
		HuDunSprite.enabled = true;
		gameObject.SetActive(true);
	}

    internal bool IsCanResetHuDunTime = false;
    public void ResetHunDunTimeInfo()
    {
        if (IsCanResetHuDunTime == true && gameObject.activeInHierarchy == true)
        {
            IsCanResetHuDunTime = false;
            HuDunTime = 3f;
            TimeStart = Time.time;
            HuDunSprite.fillAmount = 1f;
        }
    }

	void HiddenHuDunUI()
    {
        IsCanResetHuDunTime = false;
        gameObject.SetActive(false);
		XKPlayerMoveCtrl playerMoveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(PlayerSt);
		if (playerMoveScript == null) {
			return;
		}
		playerMoveScript.ResetIsWuDiState();
	}
}