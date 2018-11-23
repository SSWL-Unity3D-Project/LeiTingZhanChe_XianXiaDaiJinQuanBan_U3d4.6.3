using UnityEngine;

/**
 * boss来袭UI控制.
 */
public class XKBossLXCtrl : MonoBehaviour
{
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

    public void StartPlayBossLaiXi()
	{
		//Debug.Log("Unity:"+"StartPlayBossLaiXi...");
		//BossZuDangCtrl.GetInstance().SetIsActiveBossZuDang(true);
		TimeLastBossLX = Time.time;
		XKGlobalData.GetInstance().PlayAudioBossLaiXi();
		gameObject.SetActive(true);
        if (m_GameNumUI != null)
        {
            if (XkPlayerCtrl.GetInstanceFeiJi() != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
            {
                int jpBossDaiJinQuan = (int)XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDaiJinQuan;
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
