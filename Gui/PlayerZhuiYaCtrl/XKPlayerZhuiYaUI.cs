using UnityEngine;

public class XKPlayerZhuiYaUI : MonoBehaviour
{
	[Range(0.1f, 10f)]public float TimeMove = 0.5f;
	public void MovePlayerZhuiYaUI(Vector3 startPos)
	{
		transform.localPosition = startPos;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.SetActive(true);
		TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
		twPos.from = startPos;
		twPos.to = startPos + new Vector3(0f, 100f, 0f);
		twPos.duration = TimeMove;
		twPos.PlayForward();
		
		TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
		twAlp.from = 1f;
		twAlp.to = 0f;
		twAlp.duration = TimeMove;
		twAlp.PlayForward();
		
		EventDelegate.Add(twAlp.onFinished, delegate{
			HiddenPlayerFenShu();
		});
	}
	
	void HiddenPlayerFenShu()
	{
		//TweenPosition twPos = gameObject.GetComponent<TweenPosition>();
		//DestroyObject(twPos);
		//TweenAlpha twAlp = gameObject.GetComponent<TweenAlpha>();
		//DestroyObject(twAlp);
        Destroy(gameObject);
	}
}