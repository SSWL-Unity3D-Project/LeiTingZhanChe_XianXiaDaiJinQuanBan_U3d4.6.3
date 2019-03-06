using UnityEngine;

public class SSDaiJinQuanXuanTiao : MonoBehaviour
{
    /// <summary>
    /// 血条倒计时UI控制组件.
    /// </summary>
    public SSGameNumUI m_TimeNumUI;
    /// <summary>
    /// 血条倒计时毫秒UI控制组件.
    /// </summary>
    public SSGameNumUI m_TimeHaoMiaoNumUI;
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

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    internal void Init(float maxFillAmount)
    {
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
            UpdateDaoJiShiValue();
        }
    }

    /// <summary>
    /// 恢复代金券npc的血条信息.
    /// </summary>
    internal void BackBloodBossAmount(float amount)
    {
        BossXueTiaoSprite.fillAmount = BossXueTiaoHongSprite.fillAmount = amount;
    }

    public void SetBloodBossAmount(float bloodAmount)
    {
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
        BossXueTiaoSprite.fillAmount = bloodAmount;
        if (bloodAmount <= 0f)
        {
            XkGameCtrl.BossRemoveAllNpcAmmo();
            HiddenBossXueTiao();
        }
    }

    public void HiddenBossXueTiao()
    {
        //删除代金券npc的血条UI.
        SSUIRoot.GetInstance().m_GameUIManage.RemoveDaiJinQuanNpcXueTiaoUI();
    }

    void OpenBossXueTiao()
    {
        IsCanSubXueTiaoAmount = false;
        BossXueTiaoHongSprite.fillAmount = 0f;
        BossXueTiaoSprite.fillAmount = 0f;

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

    //*****************************************************************************************************//
    /// <summary>
    /// 倒计时数值.
    /// </summary>
    int m_DaoJiShiVal = 0;
    float m_TimeLastDaoJiShi = 0f;
    internal void InitTimeInfo(int timeVal)
    {
        //SSDebug.Log("InitTimeInfo -> timeVal ============================== " + timeVal);
        m_DaoJiShiVal = timeVal;
        m_TimeLastDaoJiShi = Time.time;
        ShowTimeNum(m_DaoJiShiVal);
    }

    /// <summary>
    /// 更新倒计时数字UI.
    /// </summary>
    void UpdateDaoJiShiValue()
    {
        if (m_DaoJiShiVal <= 0)
        {
            return;
        }

        float dTime = Time.time - m_TimeLastDaoJiShi;
        if (Time.time - m_TimeLastDaoJiShi >= 1f)
        {
            m_TimeLastDaoJiShi = Time.time;
            m_DaoJiShiVal--;
            ShowTimeNum(m_DaoJiShiVal);
            ShowTimeHaoMiaoNum(0);
            //SSDebug.Log("UpdateDaoJiShiValue -> m_DaoJiShiVal ============================== " + m_DaoJiShiVal);
        }
        else
        {
            int val = 100 - ((int)(dTime * 100f) % 100);
            ShowTimeHaoMiaoNum(val);
        }
    }


    /// <summary>
    /// 显示血条倒计时UI数字.
    /// </summary>
    void ShowTimeNum(int val)
    {
        if (m_TimeNumUI != null && val > -1)
        {
            m_TimeNumUI.ShowNumUI(val);
        }
    }

    /// <summary>
    /// 显示毫秒数字.
    /// </summary>
    void ShowTimeHaoMiaoNum(int val)
    {
        if (m_TimeHaoMiaoNumUI != null && val > -1)
        {
            m_TimeHaoMiaoNumUI.ShowNumUI(val);
        }
    }

    /// <summary>
    /// 隐藏倒计时UI.
    /// </summary>
    void HiddenTimeNum()
    {
        if (m_TimeNumUI != null)
        {
            m_TimeNumUI.SetActive(false);
        }
    }
}
