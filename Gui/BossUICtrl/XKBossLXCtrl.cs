using UnityEngine;

/**
 * boss来袭UI控制.
 */
public class XKBossLXCtrl : MonoBehaviour
{
    /// <summary>
    /// 商家信息展示UILablel.
    /// 字数最大为5个字.
    /// 最多支持4个商户(一条默认商户信息.).
    /// </summary>
    public UILabel m_ShangJiaInfoLb;
    /// <summary>
    /// 游戏数字UI控制组件.
    /// </summary>
    public SSGameNumUI m_GameNumUI;
    /// <summary>
    /// 显示时间.
    /// </summary>
    public float m_ShowTime = 2f;
    public float m_UIAlpha = 1f;
    public UITexture m_UITexture;
	float TimeLastBossLX;
	void Update()
	{
        UpdateUITextureAlpha();
        if (Time.time - TimeLastBossLX < m_ShowTime)
        {
			return;
		}
		HiddenBossLaiXi();
	}

    /// <summary>
    /// 更新图片alpha.
    /// </summary>
    void UpdateUITextureAlpha()
    {
        if (m_UITexture != null)
        {
            if (m_UITexture.alpha != m_UIAlpha)
            {
                m_UITexture.alpha = m_UIAlpha;
            }
        }
    }

    /// <summary>
    /// 设置商户名称信息.
    /// </summary>
    void SetShangJiaInfo()
    {
        if (m_ShangJiaInfoLb != null)
        {
            string shangHuInfo = "海底捞";
            if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
            {
                shangHuInfo = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingInfo().ShangHuMing;
            }

            if (shangHuInfo.Length > 5)
            {
                //最多支持5个字.
                shangHuInfo = shangHuInfo.Substring(0, 5);
            }
            m_ShangJiaInfoLb.text = shangHuInfo;
        }
    }

    public void StartPlayBossLaiXi(SpawnNpcManage.NpcState type = SpawnNpcManage.NpcState.JPBoss,
        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
	{
		//Debug.Log("Unity:"+"StartPlayBossLaiXi...");
		//BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(true);
		TimeLastBossLX = Time.time;
		XKGlobalData.GetInstance().PlayAudioBossLaiXi();
        SetShangJiaInfo();

        gameObject.SetActive(true);
        if (m_GameNumUI != null)
        {
            if (XkPlayerCtrl.GetInstanceFeiJi() != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
            {
                int jpBossDaiJinQuan = 200;
                switch (type)
                {
                    case SpawnNpcManage.NpcState.JPBoss:
                        {
                            jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDaiJinQuan;
                            break;
                        }
                    case SpawnNpcManage.NpcState.ZhanChe:
                        {
                            if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
                            {
                                jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_01;
                            }
                            else if (daiJinQuanType == SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02)
                            {
                                jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDaiJinQuan_02;
                            }
                            break;
                        }
                }
                //SSDebug.Log("StartPlayBossLaiXi -> jpBossDaiJinQuan ======================== " + jpBossDaiJinQuan);
                m_GameNumUI.ShowNumUI(jpBossDaiJinQuan);
            }
        }
	}

	void HiddenBossLaiXi()
	{
		//XKGlobalData.GetInstance().StopAudioBossLaiXi();
		TweenAlpha twAlpha = GetComponent<TweenAlpha>();
		if (twAlpha != null) {
			DestroyObject(twAlpha);
		}
		gameObject.SetActive(false);

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.RemoveBossLaiXiUI();
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
