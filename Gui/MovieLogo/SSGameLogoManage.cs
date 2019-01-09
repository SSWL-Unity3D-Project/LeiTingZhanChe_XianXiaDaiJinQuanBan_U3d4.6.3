using UnityEngine;

public class SSGameLogoManage : MonoBehaviour
{
	// Use this for initialization
	void Awake()
    {
        Init();
    }

    void Init()
    {
        string imgPath = XKGlobalData.GetInstance().GetLogoImgPath();
        Texture img = (Texture)Resources.Load(imgPath);
        if (img != null)
        {
            UITexture uiTexture = GetComponent<UITexture>();
            if (uiTexture != null)
            {
                uiTexture.mainTexture = img;
            }
        }
    }
}
