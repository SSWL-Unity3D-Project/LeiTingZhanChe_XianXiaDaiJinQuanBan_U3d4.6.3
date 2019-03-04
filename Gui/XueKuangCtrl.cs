using UnityEngine;

public class XueKuangCtrl : MonoBehaviour
{
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UITexture XueKuangNum;
    /// <summary>
    /// 头像数字UI.
    /// </summary>
    public GameObject m_TouXiangNum;
    /// <summary>
    /// 微信头像.
    /// </summary>
    public UITexture m_WeiXinHead;
    public Texture m_TouMingHead;
    /// <summary>
    /// 血框数字图片的大小.
    /// XueKuangNumWH[0] - 未激活.
    /// XueKuangNumWH[1] - 激活.
    /// </summary>
    //public Vector2[] XueKuangNumWH = new Vector2[2];
    /// <summary>
    /// 血框数字图片的位置.
    /// XueKuangNumPos[0] - 未激活.
    /// XueKuangNumPos[1] - 激活.
    /// </summary>
    //public Vector2[] XueKuangNumPos = new Vector2[2];
    /**
	 * 四人版血框数字.
	 */
    public Texture[] XueKuangTexture;
	/**
	 * 双人版血框数字.
	 */
	public Texture[] XueKuangGmTexture;
	public UISprite XueTiaoSprite;
	public UITexture CoinDiKuang;
	public Texture[] CoinDKTexture;
	static XueKuangCtrl _InstanceOne;
	public static XueKuangCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}
	
	static XueKuangCtrl _InstanceTwo;
	public static XueKuangCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}
	
	static XueKuangCtrl _InstanceThree;
	public static XueKuangCtrl GetInstanceThree()
	{
		return _InstanceThree;
	}
	
	static XueKuangCtrl _InstanceFour;
	public static XueKuangCtrl GetInstanceFour()
	{
		return _InstanceFour;
    }
    public static XueKuangCtrl GetInstance(PlayerEnum indexPlayer)
    {
        XueKuangCtrl xueKuangCom = null;
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    xueKuangCom = _InstanceOne;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    xueKuangCom = _InstanceTwo;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    xueKuangCom = _InstanceThree;
                    break;
                }
            case PlayerEnum.PlayerFour:
                {
                    xueKuangCom = _InstanceFour;
                    break;
                }
        }
        return xueKuangCom;
    }

    // Use this for initialization
    void Start()
    {
        switch (PlayerSt)
        {
            case PlayerEnum.PlayerOne:
                _InstanceOne = this;
                break;

            case PlayerEnum.PlayerTwo:
                _InstanceTwo = this;
                break;

            case PlayerEnum.PlayerThree:
                _InstanceThree = this;
                break;

            case PlayerEnum.PlayerFour:
                _InstanceFour = this;
                break;
        }

        bool isActive = XkGameCtrl.GetIsActivePlayer(PlayerSt);
        HandleXueKuangNum(isActive);
        m_WeiXinHead.mainTexture = m_TouMingHead;
    }

    //SSGameObjFlash m_GameObjFlash = null;
    public void HandlePlayerXueTiaoInfo(float playerBlood)
	{
        if (XueTiaoSprite == null)
        {
            return;
        }

        float amount = (XkGameCtrl.KeyBloodUI * playerBlood) + XkGameCtrl.MinBloodUIAmount;
        amount = Mathf.Clamp01(amount);
        XueTiaoSprite.fillAmount = amount;
        //if (XueTiaoSprite.fillAmount <= 0.4f)
        //{
            //if (XueTiaoSprite.fillAmount <= 0f)
            //{
                //if (m_GameObjFlash != null)
                //{
                //    m_GameObjFlash.RemoveSelf();
                //    m_GameObjFlash = null;
                //}
            //}
            //else
            //{
                //if (m_GameObjFlash == null)
                //{
                //    m_GameObjFlash = gameObject.AddComponent<SSGameObjFlash>();
                //    m_GameObjFlash.Init(0.25f, XueTiaoSprite.gameObject);
                //}
            //}
        //}
        //else
        //{
            //if (m_GameObjFlash != null)
            //{
            //    m_GameObjFlash.RemoveSelf();
            //    m_GameObjFlash = null;
            //}
        //}
	}

	public void HandleXueKuangNum(bool isActive)
	{
		//int indexVal = 0;
        //switch (PlayerSt) {
        //case PlayerEnum.PlayerOne:
        //	indexVal = XkGameCtrl.IsActivePlayerOne == true ? 1 : 0;
        //	break;

        //case PlayerEnum.PlayerTwo:
        //	indexVal = XkGameCtrl.IsActivePlayerTwo == true ? 1 : 0;
        //	break;

        //case PlayerEnum.PlayerThree:
        //	indexVal = XkGameCtrl.IsActivePlayerThree == true ? 1 : 0;
        //	break;

        //case PlayerEnum.PlayerFour:
        //	indexVal = XkGameCtrl.IsActivePlayerFour == true ? 1 : 0;
        //	break;
        //}
        int indexVal = isActive == true ? 1 : 0;
        CoinDiKuang.mainTexture = CoinDKTexture[indexVal];

        if (pcvr.IsHongDDShouBing)
        {
            if (XKGlobalData.GameVersionPlayer == 0)
            {
                XueKuangNum.mainTexture = XueKuangTexture[indexVal];
                if (indexVal == 1)
                {
                    if (m_WeiXinHead != null)
                    {
                        int indexUrl = (int)PlayerSt - 1;
                        SetActiveWeiXinHead(true);
                        string url = pcvr.GetInstance().m_HongDDGamePadInterface.GetPlayerHeadUrl(indexUrl);
                        XkGameCtrl.GetInstance().m_AsyImage.LoadPlayerHeadImg(url, m_WeiXinHead);
                    }
                }
                else
                {
                    if (m_WeiXinHead != null)
                    {
                        m_WeiXinHead.mainTexture = m_TouMingHead;
                        SetActiveWeiXinHead(false);
                    }
                }
            }
            else
            {
                if (PlayerSt == PlayerEnum.PlayerThree || PlayerSt == PlayerEnum.PlayerFour)
                {
                    XueKuangNum.mainTexture = XueKuangGmTexture[indexVal];
                }
            }
        }
        else
        {
            if (XKGlobalData.GameVersionPlayer == 0)
            {
                XueKuangNum.mainTexture = XueKuangTexture[indexVal];
                SetActiveWeiXinHead(indexVal == 1 ? true : false);
            }
            else
            {
                if (PlayerSt == PlayerEnum.PlayerThree || PlayerSt == PlayerEnum.PlayerFour)
                {
                    XueKuangNum.mainTexture = XueKuangGmTexture[indexVal];
                }
            }
        }
        
		//bool isActiveInfo = indexVal == 1 ? true : false;
		//XueTiaoSprite.gameObject.SetActive(isActiveInfo);
		XueTiaoSprite.gameObject.SetActive(isActive);
        XueTiaoSprite.fillAmount = 1f;
        //if (m_GameObjFlash != null)
        //{
        //    m_GameObjFlash.RemoveSelf();
        //    m_GameObjFlash = null;
        //}
    }

	public static XueKuangCtrl GetXueKuangCtrl(PlayerEnum playerIndex)
	{
		if (playerIndex == PlayerEnum.Null) {
			return null;
		}

		XueKuangCtrl xueKuangScript = null;
		switch (playerIndex) {
		case PlayerEnum.PlayerOne:
			xueKuangScript = _InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			xueKuangScript = _InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			xueKuangScript = _InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			xueKuangScript = _InstanceFour;
			break;
		}
		return xueKuangScript;
	}

    void SetActiveWeiXinHead(bool isActive)
    {
        if (m_WeiXinHead != null)
        {
            m_WeiXinHead.gameObject.SetActive(isActive);
        }

        if (m_TouXiangNum != null)
        {
            m_TouXiangNum.SetActive(!isActive);
        }
    }
}