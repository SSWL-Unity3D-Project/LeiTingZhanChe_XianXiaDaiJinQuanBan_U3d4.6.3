using UnityEngine;
using System.Collections;

public class SSPlayerAmmoBaoJi : MonoBehaviour
{
    /// <summary>
    /// 暴击粒子对象.
    /// </summary>
    public ParticleSystem m_BaoJiParticle;

    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init(PlayerEnum indexPlayer)
    {
        if (m_BaoJiParticle != null)
        {
            int baoJiDengJi = 0;
            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
            {
                baoJiDengJi = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetPlayerBaoJiDengJi(indexPlayer);
            }

            if (baoJiDengJi <= 0)
            {
                m_BaoJiParticle.gameObject.SetActive(false);
            }
            else
            {
                SSDebug.Log("Init -> indexPlayer == " + indexPlayer + ", baoJiDengJi ============ " + baoJiDengJi);
                Material mat = null;
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                {
                    mat = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetPlayerBaoJiMaterial(indexPlayer, baoJiDengJi);
                }

                if (mat == null)
                {
                    m_BaoJiParticle.gameObject.SetActive(false);
                }
                else
                {
                    m_BaoJiParticle.renderer.material = mat;
                    m_BaoJiParticle.gameObject.SetActive(true);
                }
            }
        }
    }
}
