using UnityEngine;
using System.Collections.Generic;

public class XKPlayerCamera : MonoBehaviour
{
    /// <summary>
    /// 血包道具产生之后飞向的点.
    /// </summary>
    public Transform m_XuBaoDaoJuPoint;
    /// <summary>
    /// 玩家1彩票移动的终点.
    /// </summary>
    public Transform m_CaiPiaoEndTrP1;
    /// <summary>
    /// 玩家2彩票移动的终点.
    /// </summary>
    public Transform m_CaiPiaoEndTrP2;
    /// <summary>
    /// 玩家3彩票移动的终点.
    /// </summary>
    public Transform m_CaiPiaoEndTrP3;
    /// <summary>
    /// 玩家产生点.
    /// </summary>
    public Transform[] PlayerSpawnPoint;
	public Transform DaoJuSpawnPointArray;
	Transform[] DaoJuSpawnPoint;
	Transform CameraTran;
	Transform AimTran;
	Transform CameraParent;
	float SpeedIntoAim = 0.1f;
	float SpeedOutAim = 1f;
	bool IsOutAim;
	float AimNpcSpeed = 0.1f;
	float LeaveNpcSpeed = 0.1f;
	float GenZongTmpVal = 0.0001f;
	float GenZongCamRotVal = 0.2f;
	bool IsChangeSpeedOutAim;
	public static Transform FeiJiCameraTan;
	public static Transform TanKeCameraTan;
	GameObject CameraObj;
	XkPlayerCtrl PlayerScript;
	GameObject AimNpcObj;
	float TimeCheckAimNpcLast;
	Camera PlayerCamera;
	CameraShake CamShakeCom;
	[Range(0f, 100f)]public float CamShakeVal = 0.5f;
	PlayerTypeEnum PlayerSt = PlayerTypeEnum.FeiJi;
	static XKPlayerCamera _InstanceFeiJi;
	public static XKPlayerCamera GetInstanceFeiJi()
	{
		return _InstanceFeiJi;
	}
	static XKPlayerCamera _InstanceTanKe;
	public static XKPlayerCamera GetInstanceTanKe()
	{
		return _InstanceTanKe;
	}
	static XKPlayerCamera _InstanceCartoon;
	public static XKPlayerCamera GetInstanceCartoon()
	{
		return _InstanceCartoon;
	}

	void Awake()
	{
        if (DaoJuSpawnPointArray != null)
        {
            DaoJuSpawnPointArray.gameObject.SetActive(true);
            List<Transform> daoJuList = new List<Transform>(DaoJuSpawnPointArray.GetComponentsInChildren<Transform>());
            daoJuList.RemoveAt(0);
            DaoJuSpawnPoint = daoJuList.ToArray();
            DaoJuSpawnPointArray.gameObject.SetActive(false);
        }

        XkPlayerCtrl script = GetComponentInParent<XkPlayerCtrl>();
        if (script != null)
        {
            switch (script.PlayerSt)
            {
                case PlayerTypeEnum.FeiJi:
                    _InstanceFeiJi = this;
                    break;

                case PlayerTypeEnum.TanKe:
                    _InstanceTanKe = this;
                    break;

                case PlayerTypeEnum.CartoonCamera:
                    _InstanceCartoon = this;
                    break;
            }
        }
    }

	// Use this for initialization
	void Start()
	{
		CamShakeCom = gameObject.AddComponent<CameraShake>();
		PlayerCamera = camera;
		camera.targetTexture = null;
		CameraTran = transform;
		XkPlayerCtrl script = GetComponentInParent<XkPlayerCtrl>();
		switch (script.PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			_InstanceFeiJi = this;
			PlayerSt = PlayerTypeEnum.FeiJi;
			FeiJiCameraTan = transform;
			gameObject.SetActive(false);
            if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            {
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP1 = m_CaiPiaoEndTrP1;
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP2 = m_CaiPiaoEndTrP2;
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP3 = m_CaiPiaoEndTrP3;
            }
            break;

		case PlayerTypeEnum.TanKe:
			_InstanceTanKe = this;
			PlayerSt = PlayerTypeEnum.TanKe;
			TanKeCameraTan = transform;
			gameObject.SetActive(false);
			break;

		case PlayerTypeEnum.CartoonCamera:
			_InstanceCartoon = this;
			PlayerSt = PlayerTypeEnum.CartoonCamera;
			break;
		}

		CameraObj = gameObject;
		PlayerScript = GetComponentInParent<XkPlayerCtrl>();
		if (PlayerScript != null) {
			PlayerScript.SetPlayerCamera(this);
		}
		
		GameObject obj = new GameObject();
		obj.name = "CameraParent";
		CameraParent = obj.transform;
		CameraParent.parent = CameraTran.parent;
		CameraParent.localPosition = CameraTran.localPosition;
		CameraParent.rotation = CameraTran.rotation;
		CameraTran.parent = null;
		CameraTran.rotation = CameraParent.localRotation;
		SetEnableCamera(false);
        for (int i = 0; i < PlayerSpawnPoint.Length; i++)
        {
            PlayerSpawnPoint[i].gameObject.SetActive(false);
        }
	}

	public void SetActiveCamera(bool isActive)
	{
		CameraObj.SetActive(isActive);
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && !XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isActive = false;
				}
				break;

			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isActive = false;
				}
				break;
			}
		}

		if (isActive && !ScreenDanHeiCtrl.IsStartGame && PlayerSt != PlayerTypeEnum.CartoonCamera) {
			isActive = false;
		}
		//Debug.Log("Unity:"+"SetActiveCamera -> player "+PlayerSt+", isEnable "+isActive);
		PlayerCamera.enabled = isActive;
	}

	public void SetEnableCamera(bool isEnable)
	{
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		//Debug.Log("Unity:"+"SetEnableCamera -> player "+PlayerSt+", isEnable "+isEnable);
		PlayerCamera.enabled = isEnable;
	}

	public void ActivePlayerCamera()
	{
		bool isEnable = true;
		GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		//Debug.Log("Unity:"+"ActivePlayerCamera -> player "+PlayerSt+", isEnable "+isEnable+", jiTai "+jiTai);
		PlayerCamera.enabled = isEnable;

        //if (isEnable)
        //{
        //    XkGameCtrl.TestDelayActivePlayerOne();
        //}
    }

	public bool GetActiveCamera()
	{
		return CameraObj.activeSelf;
	}

	void Update()
	{
		SmothMoveCamera();
		CheckMainCamera();
	}

	void FixedUpdate()
	{
		SmothMoveCamera();
	}

	void LateUpdate()
	{
		SmothMoveCamera();
	}

	void CheckMainCamera()
	{
		if (Camera.main == null || !Camera.main.enabled) {
			if (_InstanceFeiJi != null) {
				_InstanceFeiJi.ActivePlayerCamera();
			}
			else if (_InstanceTanKe != null) {
				_InstanceTanKe.ActivePlayerCamera();
			}
		}
	}

	public void SmothMoveCamera()
	{
		if (XKPlayerHeTiData.IsActiveHeTiPlayer) {
			if (PlayerSt == PlayerTypeEnum.FeiJi || PlayerSt == PlayerTypeEnum.TanKe) {
				this.enabled = false;
				return;
			}
		}

		if (CameraParent == null) {
			return;
		}

		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
		    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
			if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
				CameraTran.position = CameraParent.position;
				CameraTran.rotation = CameraParent.rotation;
			}
			else {
				CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
			}
		}
		else {
			if (!CameraShake.IsCameraShake) {
				//CameraTran.position = CameraParent.position;
				if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
					CameraTran.position = CameraParent.position;
					CameraTran.rotation = CameraParent.rotation;
				}
				else {
					CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
				}
			}
		}
		SmothChangeCameraRot();
		
//		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi) {
//			if (ServerPortCameraCtrl.GetInstanceFJ() != null) {
//				ServerPortCameraCtrl.GetInstanceFJ().CheckCameraFollowTran();
//			}
//		}
//		else if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
//			if (ServerPortCameraCtrl.GetInstanceTK() != null) {
//				ServerPortCameraCtrl.GetInstanceTK().CheckCameraFollowTran();
//			}
//		}
	}

	void SmothChangeCameraRot()
	{
		//CheckAimNpcObj();
		if (AimTran == null) {
			if (IsOutAim) {
				float angle = Quaternion.Angle(CameraTran.rotation, CameraParent.rotation);
				//Debug.Log("Unity:"+"angle ****** "+angle);
				if (angle <= 0.001f) {
					IsChangeSpeedOutAim = true;
				}

				if (IsChangeSpeedOutAim) {
					if (SpeedOutAim > GenZongCamRotVal) {
						SpeedOutAim -= GenZongTmpVal;
					}
					else {
						SpeedOutAim += GenZongTmpVal;
					}

					if (Mathf.Abs(SpeedOutAim - GenZongCamRotVal) <= (GenZongTmpVal * 1.5f)) {
						SpeedOutAim = GenZongCamRotVal;
					}
					IsOutAim = false;
				}
				CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, SpeedOutAim * Time.deltaTime);
			}
			else {
				IsChangeSpeedOutAim = false;
				if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
				    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
					CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, GenZongCamRotVal * Time.deltaTime);
				}
				else {
					if (Quaternion.Angle(CameraTran.rotation, CameraParent.rotation) > 30f) {
						CameraTran.rotation = CameraParent.rotation;
					}
					else {
						CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation,
						                                      CameraParent.rotation,
						                                      GenZongCamRotVal * Time.deltaTime);
					}
				}
			}
		}
		else {
			CheckAimTranObj();
		}
	}

	void CheckAimTranObj()
	{
		if (AimTran == null) {
			return;
		}

		Vector3 endPos = AimTran.position;
		Vector3 startPos = CameraTran.position;
		if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
			endPos.y = startPos.y = 0f;
		}

		Vector3 forwardVal = endPos - startPos;
		if (forwardVal != Vector3.zero) {
			Quaternion rotTmp = Quaternion.LookRotation(forwardVal);
			CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, rotTmp, SpeedIntoAim * Time.deltaTime);
		}
	}

	void ChangeAimTran(Transform aimVal)
	{
//		Debug.Log("Unity:"+"ChangeAimTran...");
		if (aimVal == null) {
			if (AimTran != null) {
				IsOutAim = true;
			}
			else {
				IsOutAim = false;
			}
		}
		else {
			SpeedIntoAim = AimNpcSpeed;
			SpeedOutAim = LeaveNpcSpeed;
		}		
		AimTran = aimVal;
	}

	public void SetAimTranInfo(AiMark markScript)
	{
		if (AimNpcObj != null) {
//			Debug.Log("Unity:"+"SetAimTranInfo -> AimNpcObj should be null");
			return;
		}

		Transform aimVal = markScript.PlayerCamAimTran;
		if (aimVal == null) {
			if (AimTran != null) {
				IsOutAim = true;
			}
			else {
				IsOutAim = false;
			}
		}
		else {
			SpeedIntoAim = markScript.SpeedIntoAim;
			SpeedOutAim = markScript.SpeedOutAim;
			//Debug.Log("Unity:"+"111*************SpeedOutAim "+SpeedOutAim);
		}

		AimTran = aimVal;
	}

	public Transform GetAimTram()
	{
		return AimTran;
	}

	void CheckAimNpcObj()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckAimNpcLast;
		if (dTime < 0.05f) {
			return;
		}
		TimeCheckAimNpcLast = Time.realtimeSinceStartup;
		
		if (PlayerScript == null) {
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() == null) {
			if (AimNpcObj != null) {
				AimNpcObj = null;
				ChangeAimTran(null);
			}
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() != AimNpcObj) {
			AimNpcObj = PlayerScript.GetAimNpcObj(); //改变距离主角最近的npc.
			ChangeAimTran(AimNpcObj.transform);
		}
	}

	public void SetCameraAimNpcSpeed(float aimSpeed, float leaveSpeed)
	{
		AimNpcSpeed = aimSpeed;
		LeaveNpcSpeed = leaveSpeed;
	}
	
	/**
	 * key == 1 -> 使主角摄像机依附于父级摄像机并且停止跟踪.
	 */
	public void SetPlayerCameraTran(int key)
	{
		switch(key) {
		case 1:
			CameraTran.parent = CameraParent;
			CameraTran.localPosition = Vector3.zero;
			CameraTran.localEulerAngles = Vector3.zero;
			CameraParent = null; //stop move player camera pos.
			break;
		
		default:
			CameraTran.position = CameraParent.position;
			CameraTran.rotation = CameraParent.rotation;
			break;
		}
	}
	
	public Transform[] GetDaoJuSpawnPoint()
	{
		return DaoJuSpawnPoint;
	}

	public void HandlePlayerCameraShake()
	{
		CamShakeCom.SetCameraShakeImpulseValue(CamShakeVal);
	}
}