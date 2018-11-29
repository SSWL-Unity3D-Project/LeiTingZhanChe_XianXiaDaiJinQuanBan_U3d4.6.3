using UnityEngine;

public class SSDaiJinQuanXuanTiao : MonoBehaviour
{
    public UISprite BossXueTiaoSprite;
    public UISprite BossXueTiaoHongSprite;
    /**
	 * 填充血条的速度.
	 */
    [Range(0.001f, 10f)]
    public float SpeedFillXueTiao = 0.5f;
    /**
	 * 减少红色血条的速度.
	 */
    [Range(0.001f, 10f)]
    public float SpeedSubXueTiao = 0.1f;
    bool IsFillBossXueTiao;
    //static SSDaiJinQuanXuanTiao _Instance;
    //public static SSDaiJinQuanXuanTiao GetInstance()
    //{
    //    return _Instance;
    //}

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            //_Instance = null;
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    internal void Init(float maxFillAmount)
    {
        //_Instance = this;
        //HiddenBossXueTiao();
        m_MaxFillAmount = maxFillAmount;
        OpenBossXueTiao();
    }

    void Update()
    {
        if (IsFillBossXueTiao)
        {
            FillBossXueTiaoSprite();
        }

        if (IsCanSubXueTiaoAmount)
        {
            SubBossXueTiaoHongSprite();
        }
    }

    //XKNpcHealthCtrl BossHealthScript;
    public void SetBloodBossAmount(float bloodAmount)
    {
        //不去设置boss血条UI.
        //return;
        //if (bossHealth != null && bloodAmount == -1f)
        //{
        //    BossHealthScript = bossHealth; //存储Boss血量脚本.
        //}

        //bloodAmount [0, 1]
        //realAmount [0, m_MaxFillAmount]
        float key = m_MaxFillAmount;
        //(realBloodAmount - 0) / (bloodAmount - 0) = key;
        //realBloodAmount = bloodAmount * key;
        float realBloodAmount = bloodAmount * key;
        bloodAmount = realBloodAmount;

        if (!IsCanSubXueTiaoAmount)
        {
            return;
        }

        if (!gameObject.activeSelf)
        {
            return;
        }

        bloodAmount = Mathf.Clamp(bloodAmount, 0f, 1f);
        //bloodAmount = bloodAmount > 1f ? 1f : bloodAmount;
        //bloodAmount = bloodAmount < 0f ? 0f : bloodAmount;
        BossXueTiaoSprite.fillAmount = bloodAmount;
        if (bloodAmount <= 0f)
        {
            //JiFenJieMianCtrl.GetInstance().ShowFinishTaskInfo();
            //IsWuDiPlayer = true;
            XkGameCtrl.BossRemoveAllNpcAmmo();
            //AudioBeiJingCtrl.StopGameBeiJingAudio();
            //if (BossHealthScript != null && bossHealth == null)
            //{
            //    BossHealthScript.OnDamageNpc(99999999, PlayerEnum.Null);
            //}
            HiddenBossXueTiao();
        }
    }

    public void HiddenBossXueTiao()
    {
        //BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(false);
        //gameObject.SetActive(false);
        //删除代金券npc的血条UI.
        SSUIRoot.GetInstance().m_GameUIManage.RemoveDaiJinQuanNpcXueTiaoUI();
    }

    void OpenBossXueTiao()
    {
        //timeVal = 150;
        //timeVal = 10; //test.
        IsCanSubXueTiaoAmount = false;
        BossXueTiaoHongSprite.fillAmount = 0f;
        BossXueTiaoSprite.fillAmount = 0f;
        //BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(true);
        //XKTriggerStopMovePlayer.IsActiveTrigger = true;

        //if (GameTimeCtrl.GetInstance() != null)
        //{
        //    GameTimeCtrl.GetInstance().HiddenGameTime();
        //}
        //BossXueTiaoSprite.fillAmount = 1f;
        gameObject.SetActive(true);
        TweenAlpha TwAlpha = gameObject.AddComponent<TweenAlpha>();
        TwAlpha.from = 0f;
        TwAlpha.to = 1f;
        TwAlpha.duration = 0.1f;
        EventDelegate.Add(TwAlpha.onFinished, delegate {
            ChangeBossXTAlphaEnd();
        });
        TwAlpha.PlayForward();
    }

    void ChangeBossXTAlphaEnd()
    {
        //if (GameTimeBossCtrl.GetInstance() != null)
        //{
        //    GameTimeBossCtrl.GetInstance().ActiveIsCheckTimeSprite(timeVal);
        //}
        StartFillBossXueTiao();
    }

    void StartFillBossXueTiao()
    {
        IsFillBossXueTiao = true;
    }

    void CloseFillBossXueTiao()
    {
        IsFillBossXueTiao = false;
        IsCanSubXueTiaoAmount = true;
    }

    bool IsCanSubXueTiaoAmount;
    public bool GetIsCanSubXueTiaoAmount()
    {
        return IsCanSubXueTiaoAmount;
    }

    /// <summary>
    /// 最大填充血条数值.
    /// </summary>
    float m_MaxFillAmount = 1f;
    void FillBossXueTiaoSprite()
    {
        float startAmount = BossXueTiaoSprite.fillAmount;
        float addAmount = 0.03f * SpeedFillXueTiao;
        startAmount += addAmount;
        startAmount = Mathf.Clamp(startAmount, 0f, m_MaxFillAmount);
        if (startAmount >= m_MaxFillAmount)
        {
            BossXueTiaoSprite.fillAmount = BossXueTiaoHongSprite.fillAmount = m_MaxFillAmount;
            CloseFillBossXueTiao();
            return;
        }

        //Debug.Log("Unity:"+"startVec "+startVec);
        BossXueTiaoSprite.fillAmount = BossXueTiaoHongSprite.fillAmount = startAmount;
    }

    void SubBossXueTiaoHongSprite()
    {
        if (BossXueTiaoHongSprite.fillAmount <= BossXueTiaoSprite.fillAmount)
        {
            return;
        }

        float fillAmount = BossXueTiaoHongSprite.fillAmount - (Time.deltaTime * SpeedSubXueTiao);
        if (fillAmount < BossXueTiaoSprite.fillAmount)
        {
            fillAmount = BossXueTiaoSprite.fillAmount;
        }
        BossXueTiaoHongSprite.fillAmount = fillAmount;
    }
}
