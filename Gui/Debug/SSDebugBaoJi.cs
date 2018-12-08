using UnityEngine;

/// <summary>
/// 测试玩家爆击信息.
/// </summary>
public class SSDebugBaoJi : MonoBehaviour
{
    internal bool IsHitDaiJinQuanNpc = false;
	void OnGUI ()
    {
        if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt == null)
        {
            return;
        }
        int baoJiDengJi = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetPlayerBaoJiDengJi(PlayerEnum.PlayerOne) + 1;
        string info = "baoJiP1 == " + baoJiDengJi;

        baoJiDengJi = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetPlayerBaoJiDengJi(PlayerEnum.PlayerTwo) + 1;
        info += ", baoJiP2 == " + baoJiDengJi;

        baoJiDengJi = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetPlayerBaoJiDengJi(PlayerEnum.PlayerThree) + 1;
        info += ", baoJiP3 == " + baoJiDengJi;
        GUI.Box(new Rect(15f, 0f, 300f, 20f), info);

        if (IsHitDaiJinQuanNpc == true)
        {
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "");
        }
    }
}
