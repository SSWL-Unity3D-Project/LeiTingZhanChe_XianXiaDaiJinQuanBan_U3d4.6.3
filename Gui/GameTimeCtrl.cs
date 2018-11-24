#define HIDDEN_GAME_TIME
using UnityEngine;

public class GameTimeCtrl : MonoBehaviour {
	public UISprite[] TimeSprite;
	UISpriteAnimation UIAni;
	static GameTimeCtrl _Instance;
	public static GameTimeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	internal void Init()
	{
		_Instance = this;
		UIAni = GetComponent<UISpriteAnimation>();
		UIAni.enabled = false;
		#if HIDDEN_GAME_TIME
		HiddenGameTime();
		#endif

	}

	// Update is called once per frame
	void Update()
	{
		CheckTimeSpriteVal();
	}

	int[] TimeVal = {0, 0, 0, 0};
	float TimeLast;
	bool IsAddTime;
	bool IsCheckTimeSprite;
	public void ActiveIsCheckTimeSprite()
	{
		#if HIDDEN_GAME_TIME
		IsCheckTimeSprite = true;
		#else
		IsCheckTimeSprite = true;
		UIAni.enabled = true;
		#endif
	}

	public bool GetIsCheckTimeSprite()
	{
		return IsCheckTimeSprite;
	}

	void CheckTimeSpriteVal()
	{
		if (Time.realtimeSinceStartup - TimeLast < 0.03f) {
			return;
		}
		TimeLast = Time.realtimeSinceStartup;

		if (!IsCheckTimeSprite) {
			return;
		}

		if (TimeVal[3] == 0) {
			if (TimeSprite[3].spriteName != "timeJiFen_0") {
				TimeVal[3]++;
			}
			return;
		}

		if (TimeSprite[3].spriteName != "timeJiFen_0") {
			IsAddTime = false;
			return;
		}

		if (IsAddTime) {
			return;
		}
		IsAddTime = true;

		int[] timeArray = {0, 0, 0, 0};
		string timeName = "timeJiFen_";
		TimeVal[2]++;
		if (TimeVal[2] >= 6) {
			TimeVal[2] = 0;
			timeArray[2] = 1;
		}
		TimeSprite[2].spriteName = timeName + TimeVal[2];

		if (TimeVal[2] == 0 && timeArray[2] == 1) {
			TimeVal[1]++;
			if (TimeVal[1] >= 10) {
				TimeVal[1] = 0;
				timeArray[1] = 1;
			}
		}
		TimeSprite[1].spriteName = timeName + TimeVal[1];
		
		if (TimeVal[1] == 0 && timeArray[1] == 1) {
			TimeVal[0]++;
			if (TimeVal[0] >= 10) {
				TimeVal[0] = 0;
			}
		}
		TimeSprite[0].spriteName = timeName + TimeVal[0];
	}

	public void HiddenGameTime()
	{
		GameObject parObj = transform.parent.gameObject;
		parObj.SetActive(false);
	}

	public void OpenGameTime()
	{
		
		#if HIDDEN_GAME_TIME
		GameObject parObj = transform.parent.gameObject;
		parObj.SetActive(false);
		#else
		GameObject parObj = transform.parent.gameObject;
		parObj.SetActive(true);
		#endif
	}

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            _Instance = null;
            Destroy(gameObject);
        }
    }
}