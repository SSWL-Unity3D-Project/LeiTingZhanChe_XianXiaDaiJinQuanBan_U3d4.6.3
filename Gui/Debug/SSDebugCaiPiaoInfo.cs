using UnityEngine;

public class SSDebugCaiPiaoInfo : MonoBehaviour
{
    void OnGUI()
    {
        Rect rect = new Rect(10f, 10f, Screen.width - 20f, 25f);
        GUI.Box(rect, "");

        string info = "gameCaiPiaoInfo: ";
        //一币兑换彩票数.
        info += "coinToCard == " + XKGlobalData.GetInstance().m_CoinToCaiPiao;
        //战车彩池数据.
        int zhanCheJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai;
        info += ", zhanCheJiangChi == " + zhanCheJiangChi;
        //JPBoss彩池数据.
        int jpBossJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDeCai;
        info += ", jpBossJiangChi == " + jpBossJiangChi;
        //随机道具彩池数据.
        int daoJuJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SuiJiDaoJuDeCai;
        info += ", daoJuJiangChi == " + daoJuJiangChi;
        //预制彩池数据.
        int yuZhiJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.YuZhiCaiPiaoVal;
        info += ", yuZhiJiangChi == " + yuZhiJiangChi;
        GUI.Label(rect, info);
    }
}