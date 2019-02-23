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
    [Range(0.1f, 10f)]
    public float XuHuaTime = 0.5f;
    UISprite m_SpriteUI;
    //	[Range(0, 999999)]public int FenShuTest = 123456;
    // Update is called once per frame
    void Update()
    {
        //SetPlayerFenShuVal(FenShuTest);
        UpdateRemovePiaoFen();
    }

    float m_TimeLast = 0f;
    void UpdateRemovePiaoFen()
    {
        if (Time.time - m_TimeLast > 4f)
        {
            m_TimeLast = Time.time;
            HiddenFenShu();
        }
    }

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
            if (fenShuAtlas != null)
            {
                FenShuSprite[i].atlas = fenShuAtlas;
            }

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

        if (m_SpriteUI == null)
        {
            m_SpriteUI = gameObject.GetComponent<UISprite>();
        }

        if (m_SpriteUI != null)
        {
            m_SpriteUI.alpha = 1f;
        }

        m_TimeLast = Time.time;
        transform.localPosition = startPos;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.SetActive(true);
        TweenPosition twPos = gameObject.AddComponent<TweenPosition>();
        twPos.from = startPos;
        twPos.to = startPos + new Vector3(0f, 50f, 0f);
        twPos.duration = PiaoFenTime;
        twPos.PlayForward();
        EventDelegate.Add(twPos.onFinished, delegate {
            StartXuHuaUI();
        });
    }

    void StartXuHuaUI()
    {
        TweenAlpha twAlp = gameObject.AddComponent<TweenAlpha>();
        twAlp.from = 1f;
        twAlp.to = 0f;
        twAlp.duration = XuHuaTime;
        twAlp.PlayForward();

        EventDelegate.Add(twAlp.onFinished, delegate {
            HiddenFenShu();
        });
    }

    void HiddenFenShu()
    {
        TweenPosition twPos = gameObject.GetComponent<TweenPosition>();
        if (twPos != null)
        {
            DestroyObject(twPos);
        }

        TweenAlpha twAlp = gameObject.GetComponent<TweenAlpha>();
        if (twAlp != null)
        {
            DestroyObject(twAlp);
        }
        gameObject.SetActive(false);
    }
}