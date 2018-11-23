using UnityEngine;

public class XKPlayerJiSuMove : MonoBehaviour
{
	[Range(0.1f, 10f)]public float PiaoZiTime = 0.5f;
	[Range(10f, 500f)]public float PiaoZiPY = 100f;
	public Vector2 LocalScale = new Vector2(1f, 1f);
	public void SetPlayerJiSuVal(Vector3 startPos)
	{
		startPos.y += XKDaoJuGlobalDt.GetInstance().DaoJuMaoZiPY;
		transform.localPosition = startPos;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = new Vector3(LocalScale.x, LocalScale.y, 1f);
		gameObject.SetActive(true);
		TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
		twPos.from = startPos;
		twPos.to = startPos + new Vector3(0f, PiaoZiPY, 0f);
		twPos.duration = PiaoZiTime;
		twPos.PlayForward();
		
		TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
		twAlp.from = 1f;
		twAlp.to = 0f;
		twAlp.duration = PiaoZiTime;
		twAlp.PlayForward();
		
		EventDelegate.Add(twAlp.onFinished, delegate{
			HiddenPlayerJiSu();
		});
	}
	
	void HiddenPlayerJiSu()
	{
        //TweenPosition twPos = gameObject.GetComponent<TweenPosition>();
        //DestroyObject(twPos);
        //TweenAlpha twAlp = gameObject.GetComponent<TweenAlpha>();
        //DestroyObject(twAlp);
        //gameObject.SetActive(false);
        Destroy(gameObject);
	}
}