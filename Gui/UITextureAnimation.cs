using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UITextureAnimation : MonoBehaviour
{
    /// <summary>
    /// 是否检测图片信息.
    /// </summary>
    public bool IsCheckTexture = true;
    /// <summary>
    /// 图片资源.
    /// </summary>
    public Texture mTexture;
    /// <summary>
    /// 图片颜色控制.
    /// </summary>
	public Color mColor = Color.white;
    /// <summary>
    /// 图片展示控制.
    /// </summary>
    UITexture mUITexture;
    /// <summary>
    /// 显示隐藏控制数据.
    /// </summary>
    [System.Serializable]
    public class ShowHiddenData
    {
        /// <summary>
        /// 间隔时间.
        /// </summary>
        [Range(0.1f, 30f)]
        public float timeJianGe = 1f;
        float timeLast = 0f;
        /// <summary>
        /// 是否激活.
        /// </summary>
        bool isEnable = true;
        /// <summary>
        /// 是否打开该功能.
        /// </summary>
        public bool IsOpen = false;
        /// <summary>
        /// 获取是否显示图片.
        /// </summary>
        internal bool GetIsEnableImg()
        {
            if (Time.time - timeLast >= timeJianGe)
            {
                timeLast = Time.time;
                isEnable = !isEnable;
            }
            return isEnable;
        }
    }
    /// <summary>
    /// 图片显示隐藏控制.
    /// </summary>
    public ShowHiddenData m_ShowHiddenData;

    void Start()
    {
        mUITexture = GetComponent<UITexture>();
    }

    void Update()
    {
        if (mUITexture != null)
        {
            UpdateUITextureInfo();
        }
    }

    /// <summary>
    /// 更新UITexture的信息.
    /// </summary>
    void UpdateUITextureInfo()
    {
        //图片控制.
        if (IsCheckTexture == true)
        {
            if (mUITexture.mainTexture != mTexture)
            {
                mUITexture.mainTexture = mTexture;
            }
        }

        //图片颜色控制.
        if (mUITexture.color != mColor)
        {
            mUITexture.color = mColor;
        }

        //图片显示隐藏控制.
        if (m_ShowHiddenData != null && m_ShowHiddenData.IsOpen)
        {
            bool isEnable = m_ShowHiddenData.GetIsEnableImg();
            if (mUITexture.enabled != isEnable)
            {
                mUITexture.enabled = isEnable;
            }
        }
    }

    /// <summary>
    /// 动画事件回调.
    /// </summary>
    public void OnAnimationTrigger(int index)
    {
        //Debug.Log("OnAnimationTrigger -> index is " + index);
        //广播消息.
        SendMessage("OnAnimationEnvent", index, SendMessageOptions.DontRequireReceiver);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (mUITexture == null)
        {
            mUITexture = GetComponent<UITexture>();
        }

        if (mUITexture != null)
        {
            UpdateUITextureInfo();
        }
    }
#endif
}