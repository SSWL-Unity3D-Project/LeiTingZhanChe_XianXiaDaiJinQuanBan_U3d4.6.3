using UnityEngine;
using System.Collections;

public class SSWangLuoGuZhang : MonoBehaviour
{
    float m_LastTimeVal = 0f;
    internal bool IsLoadingReconnectServerScene = false;
	// Use this for initialization
	void Start()
    {
        m_LastTimeVal = Time.time;
        if (ErWeiMaUI.GetInstance() != null)
        {
            ErWeiMaUI.GetInstance().SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        if (Time.time - m_LastTimeVal >= 150f)
        {
            m_LastTimeVal = Time.time;
            if (DaoJiShiCtrl.GetInstanceOne().IsPlayDaoJishi == true
                || DaoJiShiCtrl.GetInstanceTwo().IsPlayDaoJishi == true
                || DaoJiShiCtrl.GetInstanceThree().IsPlayDaoJishi == true)
            {
                //有玩家正在播放倒计时.
                //Debug.LogWarning("player have play daoJiShi...");
                return;
            }

            if (XkGameCtrl.GetInstance().m_GamePlayerAiData != null
                && XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == false)
            {
                //有激活游戏的玩家.
                //Debug.LogWarning("player have play game...");
                return;
            }
            StartCoroutine(DelayLoadingReconectServerGameScene());
        }
	}
    
    IEnumerator DelayLoadingReconectServerGameScene()
    {
        IsLoadingReconnectServerScene = true;
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.CreateCompanyLogo();
        }

        //if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        //{
        //    //关闭WebSocket
        //    pcvr.GetInstance().m_HongDDGamePadInterface.CloseWebSocket();
        //}

        UITexture uiTexture = gameObject.GetComponent<UITexture>();
        if (uiTexture != null)
        {
            uiTexture.enabled = false;
        }

        float audioVol = 1f;
        do
        {
            yield return new WaitForSeconds(0.1f);
            audioVol -= 0.05f;
            if (audioVol < 0f)
            {
                audioVol = 0f;
            }
            AudioListener.volume = audioVol;

            if (audioVol <= 0f)
            {
                break;
            }
        } while (true);
        yield return new WaitForSeconds(0.2f);
        
        if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
        {
            //重启websocket.
            pcvr.GetInstance().m_HongDDGamePadInterface.OnXiTiaoMsgTimeOutFromWangLuoGuZhang();
        }
        XkGameCtrl.IsLoadingLevel = false;
        XkGameCtrl.GetInstance().LoadingReconnectServerGameScene();
    }
}
