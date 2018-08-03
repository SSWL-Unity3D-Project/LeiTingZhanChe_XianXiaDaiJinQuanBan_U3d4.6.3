using UnityEngine;

public class SSTriggerSceneManage : MonoBehaviour
{
    /// <summary>
    /// 需要打开的对象数组.
    /// </summary>
    public GameObject[] m_OpenObjArray;
    /// <summary>
    /// 需要关闭的对象数组.
    /// </summary>
    public GameObject[] m_CloseObjArray;
    public AiPathCtrl TestPlayerPath;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XkPlayerCtrl>() == null)
        {
            return;
        }

        if (m_OpenObjArray.Length > 0)
        {
            //显示场景.
            for (int i = 0; i < m_OpenObjArray.Length; i++)
            {
                if (m_OpenObjArray[i] != null)
                {
                    m_OpenObjArray[i].SetActive(true);
                }
            }
        }
        
        if (m_CloseObjArray.Length > 0)
        {
            //关闭场景.
            for (int i = 0; i < m_CloseObjArray.Length; i++)
            {
                if (m_CloseObjArray[i] != null)
                {
                    m_CloseObjArray[i].SetActive(false);
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!XkGameCtrl.IsDrawGizmosObj)
        {
            return;
        }

        if (!enabled)
        {
            return;
        }

        if (TestPlayerPath != null)
        {
            TestPlayerPath.DrawPath();
        }
    }
#endif
}