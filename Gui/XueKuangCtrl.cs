using UnityEngine;
using System.Collections;

public class XueKuangCtrl : MonoBehaviour
{
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UITexture XueKuangNum;
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

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
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
		HandleXueKuangNum();
        m_WeiXinHead.mainTexture = m_TouMingHead;
    }
	
	public void HandlePlayerXueTiaoInfo(float playerBlood)
	{
		XueTiaoSprite.fillAmount = (XkGameCtrl.KeyBloodUI * playerBlood) + XkGameCtrl.MinBloodUIAmount;
	}

	public void HandleXueKuangNum()
	{
		int indexVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			indexVal = XkGameCtrl.IsActivePlayerOne == true ? 1 : 0;
			break;
			
		case PlayerEnum.PlayerTwo:
			indexVal = XkGameCtrl.IsActivePlayerTwo == true ? 1 : 0;
			break;
			
		case PlayerEnum.PlayerThree:
			indexVal = XkGameCtrl.IsActivePlayerThree == true ? 1 : 0;
			break;
			
		case PlayerEnum.PlayerFour:
			indexVal = XkGameCtrl.IsActivePlayerFour == true ? 1 : 0;
			break;
		}
		CoinDiKuang.mainTexture = CoinDKTexture[indexVal];
        
        if (XKGlobalData.GameVersionPlayer == 0)
        {
            XueKuangNum.mainTexture = XueKuangTexture[indexVal];
        }
        else
        {
            if (PlayerSt == PlayerEnum.PlayerThree || PlayerSt == PlayerEnum.PlayerFour)
            {
                XueKuangNum.mainTexture = XueKuangGmTexture[indexVal];
            }
        }
        
		bool isActiveInfo = indexVal == 1 ? true : false;
		XueTiaoSprite.gameObject.SetActive(isActiveInfo);
		XueTiaoSprite.fillAmount = 1f;
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
}