using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UITextureAnimation : MonoBehaviour
{
    /// <summary>
    /// 是否检测图片信息.
    /// </summary>
    public bool IsCheckTexture = true;
    public Texture mTexture;
	public Color mColor = Color.white;
    UITexture mUITexture;
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
        if (IsCheckTexture == true)
        {
            if (mUITexture.mainTexture != mTexture)
            {
                mUITexture.mainTexture = mTexture;
            }
        }

        if (mUITexture.color != mColor)
        {
            mUITexture.color = mColor;
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