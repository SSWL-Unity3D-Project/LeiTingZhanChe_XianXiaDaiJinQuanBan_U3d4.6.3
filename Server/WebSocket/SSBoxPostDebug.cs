using UnityEngine;

public class SSBoxPostDebug : MonoBehaviour
{
    /// <summary>
    /// 没有找到本地连接.
    /// 需要将电脑主板网卡名称修改为"本地连接".
    /// </summary>
    bool IsNotFindLocalMac = false;
    internal void SetIsNotFindLocalMac()
    {
        IsNotFindLocalMac = true;
        m_GuiStyle = new GUIStyle();
        m_GuiStyle.fontSize = 50;
    }

    GUIStyle m_GuiStyle = null;
    void OnGUI()
    {
        if (IsNotFindLocalMac == true)
        {
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "");
            GUI.color = Color.red;
            GUI.Label(new Rect(30f, 50f, Screen.width - 60f, 25f), "请将电脑主板网卡名称修改为\"本地连接\"", m_GuiStyle);
        }
    }
}
