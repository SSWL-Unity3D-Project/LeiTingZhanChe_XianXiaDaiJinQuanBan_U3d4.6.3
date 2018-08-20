using UnityEngine;

/// <summary>
/// 彩票随机道具粒子控制组件.
/// </summary>
public class SSCaiPiaoSuiJiDaoJu : SSGameMono
{
    internal SSCaiPiaoDataManage.SuiJiDaoJuState DaoJuType = SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang;
    /// <summary>
    /// 粒子预制.
    /// </summary>
    public GameObject m_LiZiPrefab;
    /// <summary>
    /// 创建粒子.
    /// </summary>
    public void CreatLiZi(PlayerEnum indexPlayer)
    {
        if (m_LiZiPrefab == null)
        {
            UnityLogWarning("CreatLiZi -> m_LiZiPrefab was null......");
            return;
        }

        GameObject obj = (GameObject)Instantiate(m_LiZiPrefab, XkGameCtrl.NpcAmmoArray, transform);
        if (obj != null)
        {
            if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
            {
                int value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu, DaoJuType);
                SSCaiPiaoLiZiManage caiPiaoLiZi = obj.GetComponent<SSCaiPiaoLiZiManage>();
                if (caiPiaoLiZi != null)
                {
                    caiPiaoLiZi.ShowNumUI(value, indexPlayer);
                }
                else
                {
                    UnityLogWarning("CreatLiZi -> caiPiaoLiZi was null.................");
                }
            }

            if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            {
                //初始化飞出的彩票逻辑.
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitCaiPiaoFly(transform.position, indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu);
            }
            else
            {
                UnityLogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
            }
        }
    }
}