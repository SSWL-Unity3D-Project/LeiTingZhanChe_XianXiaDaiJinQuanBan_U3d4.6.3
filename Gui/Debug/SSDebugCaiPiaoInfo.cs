using UnityEngine;

public class SSDebugCaiPiaoInfo : MonoBehaviour
{
    void OnGUI()
    {
        float width = 0.8f * Screen.width;
        float hight = 25f;

        string info = "gameCaiPiaoInfo: ";
        //一币兑换彩票数.
        info += "coinToCard == " + XKGlobalData.GetInstance().m_CoinToCard;
        //战车彩池数据.
        float zhanCheJiangChi_20 = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_01;
        info += ", zhanCheJiangChi_20 == " + zhanCheJiangChi_20.ToString("f2");
        float zhanCheJiangChi_50 = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_02;
        info += ", zhanCheJiangChi_50 == " + zhanCheJiangChi_50.ToString("f2");
        //JPBoss彩池数据.
        float jpBossJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDeCai;
        info += ", jpBossJiangChi == " + jpBossJiangChi.ToString("f2");
        //随机道具彩池数据.
        float daoJuJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SuiJiDaoJuDeCai;
        info += ", daoJuJiangChi == " + daoJuJiangChi.ToString("f2");
        //预制彩池数据.
        int yuZhiJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.YuZhiCaiPiaoVal;
        info += ", yuZhiJiangChi == " + yuZhiJiangChi;
        //总投币数据.
        int value = XKGlobalData.GetInstance().m_TotalInsertCoins;
        info += ", TotalInsertCoins == " + value;
        //总出票数据.
        value = XKGlobalData.GetInstance().m_TotalOutPrintCards;
        info += ", TotalOutPrintCards == " + value;
        //info = "bufLen == " + info.Length + ", " + info;

        if (width < info.Length * 8f)
        {
            //动态修改高度.
            hight = ((info.Length * 8f) / width) * hight;
        }
        Rect rect = new Rect(10f, 40f, width, hight);
        GUI.Box(rect, "");
        GUI.Label(rect, info);
    }
}