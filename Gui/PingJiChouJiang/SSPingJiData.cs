using UnityEngine;

public class SSPingJiData : MonoBehaviour
{
    /// <summary>
    /// 评级分数阀值.
    /// </summary>
    public int[] m_PingJiFenShuArray = new int[7] { 0, 100, 200, 300, 400, 500, 600 };
    public enum PingJiLevel
    {
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        S = 4,
        SS = 5,
        SSS = 6,
    }

    private void Start()
    {
        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().m_PingJiData = this;
        }
    }

    internal PingJiLevel GetPlayerPingJiLevel(int fenShuVal)
    {
        if (m_PingJiFenShuArray.Length <= 0 || fenShuVal < m_PingJiFenShuArray[0])
        {
            //最低评级.
            return PingJiLevel.A;
        }

        int length = (int)PingJiLevel.SSS + 1;
        PingJiLevel pingJiLevel = PingJiLevel.A;
        for (int i = 0; i < length; i++)
        {
            if (i == length - 1)
            {
                //评级分数已经到达最高等级.
                pingJiLevel = (PingJiLevel)i;
                break;
            }
            else
            {
                if (fenShuVal >= m_PingJiFenShuArray[i] && fenShuVal < m_PingJiFenShuArray[i + 1])
                {
                    pingJiLevel = (PingJiLevel)i;
                    break;
                }
            }
        }
        return pingJiLevel;
    }
}
