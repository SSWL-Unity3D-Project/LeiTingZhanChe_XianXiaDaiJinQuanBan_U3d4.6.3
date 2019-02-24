#define USE_PLAYER_WX_HEAD
using UnityEngine;

public class PlayerXueTiaoCtrl : MonoBehaviour
{
    /// <summary>
    /// 头像.
    /// </summary>
    public GameObject m_HeadObj;
    public Texture m_PlayerNumImg;
    [HideInInspector]
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public Renderer NengLiangRenderer;
    /// <summary>
    /// 头像材质球.
    /// </summary>
    public Material m_MatNum;
	Transform CameraTran;
	Transform NengLianTran;
	Transform NengLianParentTr;
	public Vector3 OffsetXT;
	static PlayerXueTiaoCtrl _InstanceOne;
	public static PlayerXueTiaoCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}
	
	static PlayerXueTiaoCtrl _InstanceTwo;
	public static PlayerXueTiaoCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}
	
	static PlayerXueTiaoCtrl _InstanceThree;
	public static PlayerXueTiaoCtrl GetInstanceThree()
	{
		return _InstanceThree;
	}
	
	static PlayerXueTiaoCtrl _InstanceFour;
	public static PlayerXueTiaoCtrl GetInstanceFour()
	{
		return _InstanceFour;
	}

	public static PlayerXueTiaoCtrl GetInstance(PlayerEnum indexPlayer)
	{
		PlayerXueTiaoCtrl playerXT = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			playerXT = _InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			playerXT = _InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			playerXT = _InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			playerXT = _InstanceFour;
			break;
		}
		return playerXT;
	}
	// Use this for initialization
	void Start()
	{
		CameraTran = Camera.main != null ? Camera.main.transform : null;
#if !USE_PLAYER_WX_HEAD
        if (m_PlayerNumImg != null && m_MatNum != null)
        {
            m_MatNum.mainTexture = m_PlayerNumImg;
        }
#else
        if (m_TouMingHead != null)
        {
            //m_HeadUrl = "";
            m_MatNum.mainTexture = m_TouMingHead;
        }
#endif
    }

    // Update is called once per frame
    void Update()
	{
		if (CameraTran == null)
        {
			CameraTran = Camera.main != null ? Camera.main.transform : null;
			return;
		}

        if (Time.frameCount % 3 == 0)
        {
            if (XkGameCtrl.GetIsActivePlayer(PlayerSt) == false || XkGameCtrl.GetIsDeathPlayer(PlayerSt) == true)
            {
                //gameObject.SetActive(false);
                SetActivePlayerXuTiao(false);
                return;
            }
        }
		CheckPlayerHitCol();

		Vector3 pos = Vector3.zero;
		Vector3 forwardVal = Vector3.zero;
		switch (KeyHitSt) {
		case 0:
            //血条在后.
			forwardVal = CameraTran.forward;
			forwardVal.y = 0f;
			NengLianTran.forward = forwardVal;
			pos = NengLianParentTr.position;
			pos += forwardVal * OffsetXT.z;
			//pos.x += OffsetXT.x;
			//pos.y += OffsetXT.y;
			NengLianTran.position = pos;
			break;
		case 1:
            //血条在左.
			forwardVal = CameraTran.right;
			forwardVal.y = 0f;
			NengLianTran.forward = forwardVal;
			pos = NengLianParentTr.position;
			pos += forwardVal * (OffsetXT.z + 3f);
			//pos.x += OffsetXT.x;
			//pos.y += OffsetXT.y;
			NengLianTran.position = pos;
			break;
		case 2:
            //血条在右.
            forwardVal = -CameraTran.right;
			forwardVal.y = 0f;
			NengLianTran.forward = forwardVal;
			pos = NengLianParentTr.position;
			pos += forwardVal * (OffsetXT.z + 3f);
			//pos.x += OffsetXT.x;
			//pos.y += OffsetXT.y;
			NengLianTran.position = pos;
			break;
		case 3:
            //血条在前.
			forwardVal = CameraTran.forward;
			forwardVal.y = 0f;
			NengLianTran.forward = forwardVal;
			pos = NengLianParentTr.position;
			pos -= forwardVal * (OffsetXT.z - 3.5f);
			//pos.x += OffsetXT.x;
			//pos.y += OffsetXT.y;
			NengLianTran.position = pos;
			break;
		}
	}

    /// <summary>
    /// 微信头像url.
    /// </summary>
    string m_HeadUrl = "";
    public Texture m_TouMingHead;
	public void HandlePlayerXueTiaoInfo(float fillVal)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有玩家激活游戏.
            return;
        }
#if !USE_PLAYER_WX_HEAD
        if (m_PlayerNumImg != null && m_MatNum != null)
        {
            m_MatNum.mainTexture = m_PlayerNumImg;
        }
#else
        if (pcvr.IsHongDDShouBing)
        {
            int indexVal = (int)PlayerSt - 1;
            string wxHeadUrl = pcvr.GetInstance().m_HongDDGamePadInterface.GetPlayerHeadUrl(indexVal);
            if (wxHeadUrl == "")
            {
                m_MatNum.mainTexture = m_PlayerNumImg;
            }
            else
            {
                if (m_HeadUrl != wxHeadUrl)
                {
                    m_HeadUrl = wxHeadUrl;
                    XkGameCtrl.GetInstance().m_AsyImage.LoadPlayerHeadImg(m_HeadUrl, m_MatNum);
                }
            }
        }
        else
        {
            m_MatNum.mainTexture = m_PlayerNumImg;
        }
#endif
        
        if (fillVal <= 0.4f)
        {
            if (fillVal <= 0f)
            {
                //if (m_GameObjFlash != null)
                //{
                //    m_GameObjFlash.RemoveSelf();
                //    m_GameObjFlash = null;
                //}
                SetActiveXueTiaoFlash(false);
            }
            else
            {
                //if (m_GameObjFlash == null)
                //{
                //    m_GameObjFlash = gameObject.AddComponent<SSGameObjFlash>();
                //    m_GameObjFlash.Init(0.25f, NengLiangRenderer.gameObject);
                //}
                SetActiveXueTiaoFlash(true);
            }
        }
        else
        {
            //if (m_GameObjFlash != null)
            //{
            //    m_GameObjFlash.RemoveSelf();
            //    m_GameObjFlash = null;
            //}
            SetActiveXueTiaoFlash(false);
        }

        float xueLiangVal = 1f - fillVal;
		xueLiangVal = Mathf.Clamp01(xueLiangVal);
		NengLiangRenderer.materials[0].SetTextureOffset("_MainTex", new Vector2(xueLiangVal, 0f));
		//bool isActiveXT = fillVal <= 0f ? false : true;
  //      if (isActiveXT == true)
  //      {
  //          isActiveXT = XkGameCtrl.GetIsActivePlayer(PlayerSt);
  //          if (XkGameCtrl.GetIsDeathPlayer(PlayerSt) == true)
  //          {
  //              isActiveXT = false;
  //          }
  //      }
		//gameObject.SetActive(isActiveXT);
        //SetActiveHead(isActiveXT);
    }

    internal void SetActivePlayerXuTiao(bool isActive)
    {
        gameObject.SetActive(isActive);
        SetActiveHead(isActive);
        if (isActive == false)
        {
            SetActiveXueTiaoFlash(false);
        }
    }

    //SSGameObjFlash m_GameObjFlash = null;
    /// <summary>
    /// 玩家血条外发光闪烁.
    /// </summary>
    public GameObject m_PlayerXueTiaoFlash = null;
    void SetActiveXueTiaoFlash(bool isActive)
    {
        if (m_PlayerXueTiaoFlash != null && m_PlayerXueTiaoFlash.activeInHierarchy != isActive)
        {
            m_PlayerXueTiaoFlash.SetActive(isActive);
        }
    }
	
	public void SetPlayerIndex(PlayerEnum playerIndex)
	{
		//bool isActiveXT = XkGameCtrl.GetIsActivePlayer(playerIndex);
		PlayerSt = playerIndex;
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

        //Debug.Log("Unity: SetPlayerIndex -> isActiveXT === " + isActiveXT + ", playerIndex ==== " + playerIndex);
		NengLianTran = transform;
		OffsetXT = NengLianTran.localPosition;
		NengLianParentTr = NengLianTran.parent;
		NengLianTran.parent = XkGameCtrl.MissionCleanup;
		//gameObject.SetActive(isActiveXT);
        //SetActiveHead(isActiveXT);
    }

    void SetActiveHead(bool isActive)
    {
        if (m_HeadObj != null && m_HeadObj.activeInHierarchy != isActive)
        {
            m_HeadObj.SetActive(isActive);
        }
    }

	/**
	 * KeyHitSt == 0 -> 血条在后.
	 * KeyHitSt == 1 -> 血条在左.
	 * KeyHitSt == 2 -> 血条在右.
	 * KeyHitSt == 3 -> 血条在前.
	 */
	byte KeyHitSt = 0;
	void CheckPlayerHitCol()
	{
		if (CameraTran == null || (Time.frameCount % 30) != 0) {
			return;
		}
		/**
		 * keyHit[0] -> 检测后面.
		 * keyHit[1] -> 检测左面.
		 * keyHit[2] -> 检测右面.
		 * keyHit[3] -> 检测前面.
		 * keyHit == 0 -> 没有检测到碰撞.
		 * keyHit == 1 -> 有检测到碰撞.
		 */
		byte[] keyHit = new byte[4];
		RaycastHit hitInfo;
		Vector3 startPos = NengLianParentTr.position + (Vector3.up * 3f);
		Vector3 forwardVal = -CameraTran.forward;
		float disVal = 5f;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, XkGameCtrl.GetInstance().XueTiaoCheckLayer)){
			keyHit[0] = 1;
		}

		forwardVal = -CameraTran.right;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, XkGameCtrl.GetInstance().XueTiaoCheckLayer)){
			keyHit[1] = 1;
		}
		
		forwardVal = CameraTran.right;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, XkGameCtrl.GetInstance().XueTiaoCheckLayer)){
			keyHit[2] = 1;
		}

		forwardVal = CameraTran.forward;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, XkGameCtrl.GetInstance().XueTiaoCheckLayer)){
			keyHit[3] = 1;
		}

		byte keyHitVal = (byte)((keyHit[0] << 3) + (keyHit[1] << 2) + (keyHit[2] << 1) + keyHit[3]);
		byte keyHitStVal = 0;
		switch (keyHitVal) {
		case 0x00:
			keyHitStVal = 0;
			break;
		case 0x08:
			keyHitStVal = 3;
			break;
		}

		if ((keyHitVal & 0x04) == 0x04) {
			keyHitStVal = 2;
		}
		
		if ((keyHitVal & 0x02) == 0x02) {
			keyHitStVal = 1;
		}

		if (KeyHitSt != keyHitStVal) {
			KeyHitSt = keyHitStVal;
//#if UNITY_EDITOR
//			switch (KeyHitSt) {
//			case 0:
//				Debug.Log("Unity:"+"CheckPlayerHitCol -> KeyHitSt is back! player "+PlayerSt);
//				break;
//			case 1:
//				Debug.Log("Unity:"+"CheckPlayerHitCol -> KeyHitSt is left! player "+PlayerSt);
//				break;
//			case 2:
//				Debug.Log("Unity:"+"CheckPlayerHitCol -> KeyHitSt is right! player "+PlayerSt);
//				break;
//			case 3:
//				Debug.Log("Unity:"+"CheckPlayerHitCol -> KeyHitSt is forward! player "+PlayerSt);
//				break;
//			}
//#endif
		}
	}
}