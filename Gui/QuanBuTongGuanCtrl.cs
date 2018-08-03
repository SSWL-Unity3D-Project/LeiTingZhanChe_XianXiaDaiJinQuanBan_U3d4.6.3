using UnityEngine;
using System.Collections;

public class QuanBuTongGuanCtrl : MonoBehaviour
{
	static QuanBuTongGuanCtrl _Instance;
	public static QuanBuTongGuanCtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		HiddenQuanBuTongGuan();
	}

	void HiddenQuanBuTongGuan(int key = 0)
	{
		gameObject.SetActive(false);
		if (key != 0) {
			//play gameOver.
			Debug.Log("Unity:"+"play gameOver...");
			//GameOverCtrl.GetInstance().ShowGameOver(1);
		}
	}

	public void ShowQuanBuTongGuan()
	{
		XKGlobalData.GetInstance().PlayAudioQuanBuTongGuan();
		TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
		twPos.from = new Vector3(0f, 500f, 0f);
		twPos.to = Vector3.zero;
		twPos.duration = 0.5f;
		transform.localPosition = twPos.from;
		gameObject.SetActive(true);
		twPos.PlayForward();
		Invoke("DelayCloseQuanBuTongGuan", 5f);
	}

	void DelayCloseQuanBuTongGuan()
	{
		HiddenQuanBuTongGuan(1);
	}
}