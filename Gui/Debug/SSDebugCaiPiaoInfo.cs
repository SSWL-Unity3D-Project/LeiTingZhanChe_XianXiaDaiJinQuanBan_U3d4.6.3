using UnityEngine;

public class SSDebugCaiPiaoInfo : MonoBehaviour
{
    void OnGUI()
    {
        float width = Screen.width - 20f;
        float hight = 25f;

        string info = "gameCaiPiaoInfo: ";
        //一币兑换彩票数.
        info += "coinToCard == " + XKGlobalData.GetInstance().m_CoinToCard;
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
        Rect rect = new Rect(10f, 10f, width, hight);
        GUI.Box(rect, "");
        GUI.Label(rect, info);
    }
}