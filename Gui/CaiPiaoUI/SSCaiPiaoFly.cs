using UnityEngine;

public class SSCaiPiaoFly : SSGameMono
{
    int IndexCaiPiao;
    PlayerEnum IndexPlayer = PlayerEnum.Null;
    SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState m_DeCaiState;
    SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState m_DaiJinQaunType;
    public void Init(int indexCaiPiao, float timeFly, Vector3[] path, PlayerEnum indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type,
        SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01)
    {
        IndexCaiPiao = indexCaiPiao;
        IndexPlayer = indexPlayer;
        m_DeCaiState = type;
        m_DaiJinQaunType = daiJinQuanType;

        iTween.MoveTo(gameObject, iTween.Hash("path", path,
                                           "time", timeFly,
                                           "orienttopath", false,
                                           "easeType", iTween.EaseType.linear,
                                           "oncomplete", "MoveCaiPiaoOnCompelteITween"));
    }

    void MoveCaiPiaoOnCompelteITween()
    {
        if (IndexCaiPiao == 1)
        {
            if (SSUIRoot.GetInstance().m_GameUIManage != null)
            {
                float time = 1.5f;
                switch (m_DeCaiState)
                {
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe:
                        {
                            time = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_ZhanCheCaiPiaoFlyDt.TimeLeiJiaVal;
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu:
                        {
                            time = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_SuiJiCaiPiaoFlyDt.TimeLeiJiaVal;
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhengChang:
                        {
                            time = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_ZhengChangCaiPiaoFlyDt.TimeLeiJiaVal;
                            break;
                        }
                }
                SSUIRoot.GetInstance().m_GameUIManage.InitCaiPiaoAnimation(time, IndexPlayer, m_DeCaiState, m_DaiJinQaunType);
            }
        }
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}