using UnityEngine;

public class SSCaiPiaoDaJiang : SSGameMono
{
    /// <summary>
    /// 大奖数字信息控制组件.
    /// </summary>
    public SSGameNumUI m_GameNumUI;
    public UITexture m_PlayerIndexUI;
    public Texture[] m_PlayerIndexUIArray = new Texture[3];
    /// <summary>
    /// 代金券商户信息5个字.
    /// </summary>
    public UILabel m_DaiJinQuanShangHuInfo;
    internal void ShowDaJiangCaiPiaoNum(PlayerEnum indexPlayer, int num)
    {
        if (m_GameNumUI != null)
        {
            m_GameNumUI.ShowNumUI(num);
        }
        
        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_PlayerIndexUI != null && m_PlayerIndexUIArray.Length > index)
            {
                //设置中大奖玩家的UI索引信息.
                m_PlayerIndexUI.mainTexture = m_PlayerIndexUIArray[index];
            }
        }
    }

    /// <summary>
    /// 显示代金券商户信息5个字.
    /// </summary>
    internal void ShowDaiJinQuanShangHuInfo(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type,
        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
    {
        if (m_DaiJinQuanShangHuInfo != null)
        {
            string info = "";
            if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
            {
                switch (type)
                {
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss:
                        {
                            info = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetJPBossShangHuMingDt();
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe:
                        {
                            info = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingDt(daiJinQuanType).ShangHuMing;
                            break;
                        }
                }
            }

            if (info != "")
            {
                m_DaiJinQuanShangHuInfo.text = info;
            }
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