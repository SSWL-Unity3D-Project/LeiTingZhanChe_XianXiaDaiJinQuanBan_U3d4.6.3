using UnityEngine;

public class SSDebugCaiPiaoInfo : MonoBehaviour
{
    void OnGUI()
    {
        float width = 0.8f * Screen.width;
        float hight = 25f;

        string info = "gameCaiPiaoInfo: ";
        //一币兑换彩票数.
        info += "GamePrice == " + XKGlobalData.GetInstance().GameCoinToMoney + " Fen";
        //战车彩池数据.
        float zhanCheJiangChi_20 = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_01;
        info += ", zhanCheJiangChi_01 == " + zhanCheJiangChi_20.ToString("f2");
        float zhanCheJiangChi_50 = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.ZhanCheDeCai_02;
        info += ", zhanCheJiangChi_02 == " + zhanCheJiangChi_50.ToString("f2");
        //JPBoss彩池数据.
        float jpBossJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDeCai;
        info += ", jpBossJiangChi == " + jpBossJiangChi.ToString("f2");
        //jPBoss代金券商户支付数据.
        float jPBossDaiJinQuanShangHuZhiFu = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.JPBossDaiJinQuanShangHuZhiFu;
        info += ", jPBossDaiJinQuanShangHuZhiFu == " + jPBossDaiJinQuanShangHuZhiFu.ToString("f1");
        //随机道具彩池数据.
        float daoJuJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SuiJiDaoJuDeCai;
        info += ", daoJuJiangChi == " + daoJuJiangChi.ToString("f2");
        //JPBoss代金券爆奖率.
        float jpBossBaoJiangLv = XKGlobalData.GetInstance().JPBossBaoJiangLv;
        info += ", jpBossBaoJiangLv == " + jpBossBaoJiangLv.ToString("f2");
        //战车01代金券爆奖率.
        float zhanCheBaoJiangLv_01 = XKGlobalData.GetInstance().ZhanCheBaoJiangLv_01;
        info += ", zhanCheBaoJiangLv_01 == " + zhanCheBaoJiangLv_01.ToString("f2");
        //战车02代金券爆奖率.
        float zhanCheBaoJiangLv_02 = XKGlobalData.GetInstance().ZhanCheBaoJiangLv_02;
        info += ", zhanCheBaoJiangLv_02 == " + zhanCheBaoJiangLv_02.ToString("f2");
        //随机道具爆奖率.
        float daoJuBaoJiangLv = XKGlobalData.GetInstance().SuiJiDaoJuBaoJiangLv;
        info += ", daoJuBaoJiangLv == " + daoJuBaoJiangLv.ToString("f2");
        //预制彩池数据.
        //float yuZhiJiangChi = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameYuZhiCaiPiaoData.YuZhiCaiPiaoVal;
        //info += ", yuZhiJiangChi == " + yuZhiJiangChi.ToString("f2");
        //总投币数据.
        //int value = XKGlobalData.GetInstance().m_TotalInsertCoins;
        //info += ", TotalInsertCoins == " + value;
        //总出票数据.
        //value = XKGlobalData.GetInstance().m_TotalOutPrintCards;
        //info += ", TotalOutPrintCards == " + value;
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