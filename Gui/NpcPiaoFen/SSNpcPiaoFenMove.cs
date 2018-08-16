using UnityEngine;

public class SSNpcPiaoFenMove : MonoBehaviour
{
    /// <summary>
    /// 加号UI.
    /// </summary>
    public UITexture m_JiaHaoUI;
    public UISprite[] FenShuSprite;
    [Range(0.1f, 10f)]
    public float PiaoFenTime = 0.5f;
    //	[Range(0, 999999)]public int FenShuTest = 123456;
    // Update is called once per frame
    //	void Update()
    //	{
    //		SetPlayerFenShuVal(FenShuTest);
    //	}

    public void ShowFenShuVal(int fenShuVal, Vector3 startPos, UIAtlas fenShuAtlas, Texture jiaHaoImg)
    {
        if (fenShuVal <= 0)
        {
            return;
        }

        if (m_JiaHaoUI != null && jiaHaoImg != null)
        {
            m_JiaHaoUI.mainTexture = jiaHaoImg;
        }

        int max = FenShuSprite.Length;
        int numVal = fenShuVal;
        int valTmp = 0;
        int powVal = 0;
        bool isShowZero = false;
        for (int i = 0; i < max; i++)
        {
            powVal = (int)Mathf.Pow(10, max - i - 1);
            valTmp = numVal / powVal;
            FenShuSprite[i].enabled = true;
            FenShuSprite[i].atlas = fenShuAtlas;
            if (!isShowZero)
            {
                if (valTmp != 0)
                {
                    isShowZero = true;
                }
                else
                {
                    FenShuSprite[i].enabled = false;
                }
            }
            FenShuSprite[i].spriteName = valTmp.ToString();
            numVal -= valTmp * powVal;
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

        TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
        twAlp.from = 1f;
        twAlp.to = 0f;
        twAlp.duration = PiaoFenTime;
        twAlp.PlayForward();

        EventDelegate.Add(twAlp.onFinished, delegate {
            HiddenFenShu();
        });
    }

    void HiddenFenShu()
    {
        TweenPosition twPos = gameObject.GetComponent<TweenPosition>();
        DestroyObject(twPos);
        TweenAlpha twAlp = gameObject.GetComponent<TweenAlpha>();
        DestroyObject(twAlp);
        gameObject.SetActive(false);
    }
}