using UnityEngine;

public class SSGameObjFlash : MonoBehaviour
{
    float m_TimeLast = 0f;
    float m_TimeUpdate = 1f;
    GameObject m_FlashObj = null;
	// Use this for initialization
	internal void Init(float timeUpdate, GameObject flashObj)
    {
        if (timeUpdate < 0.03f)
        {
            timeUpdate = 0.03f;
        }
        m_TimeUpdate = timeUpdate;
        m_TimeLast = -1000f;
        m_FlashObj = flashObj;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (m_FlashObj != null)
        {
            if (Time.time - m_TimeLast >= m_TimeUpdate)
            {
                m_TimeLast = Time.time;
                m_FlashObj.SetActive(!m_FlashObj.activeInHierarchy);
            }
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            if (m_FlashObj != null)
            {
                m_FlashObj.SetActive(true);
            }
            DestroyObject(this);
        }
    }
}
