using UnityEngine;

public class XKPlayerFenShuMove : MonoBehaviour
{
	public GameObject ShuangBeiObj;
	public Texture[] BeiLvTexture;
	UITexture BeiLvUITexture;
	public UISprite[] FenShuSprite;
	[Range(0.1f, 10f)]public float PiaoFenTime = 0.5f;
//	[Range(0, 999999)]public int FenShuTest = 123456;
	// Update is called once per frame
//	void Update()
//	{
//		SetPlayerFenShuVal(FenShuTest);
//	}

    public void Init(UIAtlas atlas)
    {
        for (int i = 0; i < FenShuSprite.Length; i++)
        {
            if (FenShuSprite[i] != null && atlas != null)
            {
                FenShuSprite[i].atlas = atlas;
            }
        }
    }

	public void SetPlayerFenShuVal(int fenShuVal, Vector3 startPos, int indexPlayer)
	{
		if (fenShuVal <= 0) {
			return;
		}

		bool isShowShuangBeiUI = XKDaoJuGlobalDt.FenShuBeiLv[indexPlayer] >= 2 ? true : false;
		if (isShowShuangBeiUI) {
			int indexBeiLv = XKDaoJuGlobalDt.FenShuBeiLv[indexPlayer] - 2;
			indexBeiLv = indexBeiLv > 8 ? 8 : indexBeiLv;
			//Debug.Log("Unity:"+"SetPlayerFenShuVal -> indexBeiLv "+indexBeiLv+", indexPlayer "+indexPlayer);
			if (BeiLvUITexture == null) {
				BeiLvUITexture = ShuangBeiObj.GetComponent<UITexture>();
			}
			BeiLvUITexture.mainTexture = BeiLvTexture[indexBeiLv];
		}
		ShuangBeiObj.SetActive(isShowShuangBeiUI);

		int max = FenShuSprite.Length;
		int numVal = fenShuVal;
		int valTmp = 0;
		int powVal = 0;
		bool isShowZero = false;
		for (int i = 0; i < max; i++) {
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			FenShuSprite[i].enabled = true;
			if (!isShowZero) {
				if (valTmp != 0) {
					isShowZero = true;
				}
				else {
					FenShuSprite[i].enabled = false;
				}
			}
			FenShuSprite[i].spriteName = valTmp.ToString();
			numVal -= valTmp * powVal;
		}

        UISprite uiSprite = gameObject.GetComponent<UISprite>();
        if (uiSprite != null)
        {
            uiSprite.alpha = 1f;
        }
        
        TweenPosition twPosTmp = gameObject.GetComponent<TweenPosition>();
        if (twPosTmp != null)
        {
            DestroyObject(twPosTmp);
        }

        TweenAlpha twAlpTmp = gameObject.GetComponent<TweenAlpha>();
        if (twAlpTmp != null)
        {
            DestroyObject(twAlpTmp);
        }

        transform.localPosition = startPos;
		transform.localEulerAngles = Vector3.zero;
		transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.SetActive(true);
		TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
		twPos.from = startPos;
		twPos.to = startPos + new Vector3(0f, 50f, 0f);
		twPos.duration = PiaoFenTime;
		twPos.PlayForward();
        EventDelegate.Add(twPos.onFinished, delegate
        {
            OpenTweenAlpha();
        });
    }

    void OpenTweenAlpha()
    {
        TweenPosition twPos = gameObject.GetComponent<TweenPosition>();
        if (twPos != null)
        {
            DestroyObject(twPos);
        }

        TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
        twAlp.from = 1f;
        twAlp.to = 0f;
        twAlp.duration = PiaoFenTime;
        twAlp.PlayForward();

        EventDelegate.Add(twAlp.onFinished, delegate {
            HiddenPlayerFenShu();
        });
    }

	void HiddenPlayerFenShu()
	{
		TweenAlpha twAlp = gameObject.GetComponent<TweenAlpha>();
        if (twAlp != null)
        {
            DestroyObject(twAlp);
        }
		gameObject.SetActive(false);
	}
}