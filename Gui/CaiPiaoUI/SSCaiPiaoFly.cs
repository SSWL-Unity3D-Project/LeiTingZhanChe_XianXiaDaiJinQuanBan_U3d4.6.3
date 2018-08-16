using UnityEngine;

public class SSCaiPiaoFly : SSGameMono
{
    int IndexCaiPiao;
    PlayerEnum IndexPlayer = PlayerEnum.Null;
    SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState m_DeCaiState;
    public void Init(int indexCaiPiao, float timeFly, Vector3[] path, PlayerEnum indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type)
    {
        IndexCaiPiao = indexCaiPiao;
        IndexPlayer = indexPlayer;
        m_DeCaiState = type;

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
                SSUIRoot.GetInstance().m_GameUIManage.InitCaiPiaoAnimation(time, IndexPlayer);
            }
        }
        Destroy(gameObject);
    }
}