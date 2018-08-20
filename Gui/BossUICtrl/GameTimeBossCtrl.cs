using UnityEngine;
using System.Collections;

public class GameTimeBossCtrl : MonoBehaviour
{
	public GameObject BossTimeObj;
	public UISprite[] TimeSprite;
	UISpriteAnimation UIAni;
	static GameTimeBossCtrl _Instance;
	public static GameTimeBossCtrl GetInstance()
	{
		return _Instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		UIAni = GetComponent<UISpriteAnimation>();
		UIAni.enabled = false;
		HiddenGameTime();
	}
	
	// Update is called once per frame
	void TmpUpdate()
	{
//		if (Input.GetKeyUp(KeyCode.P)) {
//			ActiveIsCheckTimeSprite(740); //test
//		}
		CheckTimeSpriteVal();
	}
	
	public int[] TimeVal = {0, 0, 5, 0};
	float TimeLast;
	bool IsAddTime = true;
	bool IsCheckTimeSprite;
	int TimeBossVal;
	int TimeBossUse;
	string TimeMiaoGeWeiStr = "";
	/**
	 * 获取boss战的剩余时间.
	 */
	public int GetTimeBossResidual()
	{
		int timeVal = TimeBossVal - TimeBossUse;
		/*Debug.Log("Unity:"+"TimeBossUse *** "+TimeBossUse
		          +", TimeBossVal *** "+TimeBossVal);*/
		timeVal = timeVal < 0 ? 0 : timeVal;
        //Debug.Log("Unity:"+"GetTimeBossResidual -> timeVal "+timeVal);
        //return timeVal;
        return 1;
	}

	public void ActiveIsCheckTimeSprite(int miaoShuVal = 180)
	{
		UIAni.enabled = true;
		miaoShuVal = (miaoShuVal / 10) * 10 + 9;
		TimeBossVal = miaoShuVal;
		TimeBossUse = 0;
		//Debug.Log("Unity:"+"miaoShuVal "+miaoShuVal);
		TimeVal[3] = 9;
		TimeVal[2] = (miaoShuVal / 10) % 6;
		TimeVal[1] = (miaoShuVal / 60) % 10;
		TimeVal[0] = (miaoShuVal / 600) % 10;
		//Debug.Log("Unity:"+"TimeVal[0] "+TimeVal[0]+", TimeVal[1] "+TimeVal[1]
		          //+", TimeVal[2] "+TimeVal[2]+", TimeVal[3] "+TimeVal[3]);
		
		TimeMiaoGeWeiStr = "timeJiFenBoss_0";
		TimeSprite[3].spriteName = TimeMiaoGeWeiStr;

		string timeName = "p1_";
		TimeSprite[2].spriteName = timeName + TimeVal[2];
		TimeSprite[1].spriteName = timeName + TimeVal[1];
		TimeSprite[0].spriteName = timeName + TimeVal[0];
		BossTimeObj.SetActive(true);
		IsCheckTimeSprite = true;
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

		if (TimeSprite[3].spriteName != TimeMiaoGeWeiStr) {
			TimeMiaoGeWeiStr = TimeSprite[3].spriteName;
			TimeBossUse++;
		}

		if (TimeSprite[3].spriteName == "timeJiFenBoss_9"
		    && TimeSprite[2].spriteName == "p1_0"
		    && TimeSprite[1].spriteName == "p1_0"
		    && TimeSprite[0].spriteName == "p1_0") {
			XKBossXueTiaoCtrl.GetInstance().SetBloodBossAmount(0f);
			Debug.Log("Unity:"+"bossTimeOver...");
			return;
		}

		if (TimeSprite[3].spriteName != "timeJiFenBoss_0") {
			IsAddTime = false;
			return;
		}
		
		if (IsAddTime) {
			return;
		}
		IsAddTime = true;
		
		int[] timeArray = {0, 0, 0, 0};
		string timeName = "p1_";
		TimeVal[2]--;
		if (TimeVal[2] < 0) {
			TimeVal[2] = 5;
			timeArray[2] = 1;
		}
		TimeSprite[2].spriteName = timeName + TimeVal[2];
		
		if (TimeVal[2] == 5 && timeArray[2] == 1) {
			TimeVal[1]--;
			if (TimeVal[1] < 0) {
				TimeVal[1] = 9;
				timeArray[1] = 1;
			}
		}
		TimeSprite[1].spriteName = timeName + TimeVal[1];
		
		if (TimeVal[1] == 9 && timeArray[1] == 1) {
			TimeVal[0]--;
			if (TimeVal[0] < 0) {
				TimeVal[0] = 9;
			}
		}
		TimeSprite[0].spriteName = timeName + TimeVal[0];
	}
	
	public void HiddenGameTime()
	{
		BossTimeObj.SetActive(false);
	}
}