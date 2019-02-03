using UnityEngine;

public class SSGameHddSreenId : SSGameMono
{
    public SSGameNumUI m_GameNumUI;
    internal void Init(int screenId)
    {
        UnityLog("Init -> screenId =================== " + screenId);
        if (m_GameNumUI != null)
        {
            m_GameNumUI.ShowNumUI(screenId);
        }

        if (m_PosY == 0f)
        {
            m_PosY = transform.localPosition.y;
        }
    }

    float m_PosY = 0f;
    float m_NewPosY = -35f;
    internal void OnSetActiveHddErWeiMa(bool isActive)
    {
        Vector3 pos = transform.localPosition;
        if (isActive == true)
        {
            //将屏幕码放到二维码的下面位置.
            if (m_PosY != 0f)
            {
                pos.y = m_PosY;
            }
        }
        else
        {
            //将屏幕码向上移动.
            pos.y = m_NewPosY;
        }
        transform.localPosition = pos;
    }
}
