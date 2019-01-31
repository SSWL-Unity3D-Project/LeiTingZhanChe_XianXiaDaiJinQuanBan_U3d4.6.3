using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

public class XKNpcMoveCtrl : MonoBehaviour
{
    /// <summary>
    /// 彩票boss的产生方位信息.
    /// </summary>
    internal SSTriggerCaiPiaoBossMove.TriggerDir m_TriggerDir = SSTriggerCaiPiaoBossMove.TriggerDir.Center;
    /// <summary>
    /// 可以被哪个玩家击爆.
    /// </summary>
    public PlayerEnum m_IndexPlayerJiBao;
    /// <summary>
    /// 彩票战车npc的代金券变量.
    /// 主要用来区分是战车01还是战车02类型代金券.
    /// </summary>
    internal SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState m_DaiJinQuanState = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01;
    bool _IsZhanCheNpc = false;
    /// <summary>
    /// 是否是战车npc.
    /// </summary>
    public bool IsZhanCheNpc
    {
        set
        {
            _IsZhanCheNpc = value;
            if (value == true)
            {
                m_IndexPlayerJiBao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetPlayerIndexByJiBaoGaiLv(SpawnNpcManage.NpcState.ZhanChe, m_DaiJinQuanState);
            }
        }
        get { return _IsZhanCheNpc; }
    }
    bool _IsJPBossNpc = false;
    /// <summary>
    /// 是否是JPBoss.
    /// </summary>
    public bool IsJPBossNpc
    {
        set
        {
            _IsJPBossNpc = value;
            m_IndexPlayerJiBao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetPlayerIndexByJiBaoGaiLv(SpawnNpcManage.NpcState.JPBoss);
        }
        get { return _IsJPBossNpc; }
    }

	float SpawnTimeVal;
	NpcType NpcState = NpcType.LandNpc;
//	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing; //控制npc的运动.
	public NpcJiFenEnum NpcMoveType = NpcJiFenEnum.ShiBing; //控制npc的运动.
	public Transform RealNpcTran;
	public XKNpcAnimatorCtrl NpcAniScript;
//	public GameObject DeathExplode;
	[Range(0f, 1f)]public float DoFire1 = 0.1f;
//	[Range(0f, 1f)]public float DoFire2 = 0.2f;
	public bool IsMoveByCar;
	public bool IsZaiTiNpc;
	//public bool IsAniMove; //用动画来控制运动.
	//CountHuanDan小于1时，不做换弹动画. 否则做换弹动画.
	//[Range(-1, 100)] public int CountHuanDan = 10;
	[Range(0f, 100f)]public float RemoveNpcTime = 2f;
	public GameObject TestSpawnPoint;
	Rigidbody BuWaWaRigidbody;
	Animator AnimatorCom;
	GameObject NpcObj;
	Transform NpcTran;
	Transform NpcPathTran;
	internal float MvSpeed;
	int MarkCount;
	string CurrentRunAnimation;
	bool IsDeathNPC;
	bool IsDoFireAnimation;
	bool IsFireMove;
	bool IsChangeNpcForward;
	Vector3 ForwardValNpc;
	float FireDistance = 0f;
	bool IsAimPlayer;
	XKSpawnNpcPoint SpawnPointScript;
	bool IsFangZhenNpc;
	XKNpcFangZhenCtrl NpcFangZhenScript;
//	bool IsDoFireAniZaiTi;
	Animator[] ZaiTiNpcAni;
//	XKNpcAnimatorCtrl[] ZaiTiNpcAniScript;
	Rigidbody[] ZaiTiNpcBuWaWa;
	iTween ITweenScriptNpc;
	bool IsWuDi;
	bool IsHuoCheNpc;
//	float TimeMinFire = 0.1f;
//	float TimeMaxFire = 2f;
//	int ZaiTiNpcFireCount;
	bool IsAimPlayerByFire;
//	bool IsDelayFireAction;
	Transform MarkTranAim;
	Vector3 MarkTranAimForward;
	[Range(0.001f, 1f)]public float CheLiangRotSpeed = 0.5f;
    /// <summary>
    /// boss碰上中心停止镜头触发器后的特殊运动速度.
    /// </summary>
    public float m_BossTeShuMoveSpeed = 1f;
	NpcPathCtrl NpcPathScript;
//	int IndexNpc;
	int IndexFirePointGroup;
//	bool IsIntoJingJieState;
	FirePoint FirePointScript;
	bool IsMoveFirePoint;
	NpcFireAction FireAnimation;
	//Transform MarkNpcMove;
    Vector3 MarkNpcMovePos;
	bool IsMoveToMarkPoint;
	NetworkView NetViewCom;
	public XKPlayerMoveCtrl PlayerMoveScript;
	bool IsCheLiangMoveType;
	float TimeFire = 1f; //npc开火持续时间.
	float TimeRun = 1f; //npc开火后奔跑时间.
	public float DisPlayerVal; //测试Npc和主角的距离.
	public bool IsTestDrawFireDis;
	Rigidbody RigCom;

	void Awake()
	{
		switch (NpcMoveType) {
		case NpcJiFenEnum.Boss:
		case NpcJiFenEnum.CheLiang:
		case NpcJiFenEnum.ChuanBo:
			if (!IsMoveByCar && MoveStyle != UITweener.Style.Loop) {
				IsCheLiangMoveType = true;
			}
			break;
		}

		if (XkGameCtrl.PlayerActiveNum > 0) {
			GetAimPlayerMoveScript();
		}

		SpawnTimeVal = Time.realtimeSinceStartup;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (TestSpawnPoint != null) {
				XKSpawnNpcPoint spawnPoint = TestSpawnPoint.GetComponent<XKSpawnNpcPoint>();
				if (spawnPoint != null) {
					SetSpawnNpcInfo(spawnPoint);
				}
			}
		}
        
		NetViewCom = GetComponent<NetworkView>();
		if (Network.peerType == NetworkPeerType.Disconnected && NetViewCom != null) {
            Destroy(NetViewCom);
		}
		InitNpcInfo();
		//XkGameCtrl.GetInstance().AddNpcTranToList(NpcTran);
		MakeLandNpcMoveToLand();
		Invoke("DelayChangeNpcParent", 0.2f);
    }

	void DelayChangeNpcParent()
	{
		if (!IsHuoCheNpc) {
			if (transform.parent == null) {
				transform.parent = XkGameCtrl.NpcObjArray;
			}
		}
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}

		if (!IsTestDrawFireDis) {
			return;
		}

		if (FireDistance <= 0f) {
			return;
		}
		Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.3f);
		Gizmos.DrawSphere(transform.position, FireDistance);
		
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, FireDistance);
	}
#endif

    void Update()
	{
		CheckNpcIsRemove();
		if (IsDeathNPC) {
			return;
		}
		
		MakeLandNpcMoveToLand();
		CheckMoveNpcOnCompelteITween();

		if (IsChangeNpcForward && !IsMoveByCar) {
			NpcTran.forward = Vector3.Lerp(NpcTran.forward, ForwardValNpc, 0.05f);
			if (Vector3.Distance(NpcTran.forward, ForwardValNpc) <= 0.02f) {
				NpcTran.forward = ForwardValNpc;
				IsChangeNpcForward = false;
			}
		}

        if (IsCheLiangMoveType && RealNpcTran != null)
        {
            //RealNpcTran.forward = Vector3.Slerp(RealNpcTran.forward, MarkTranAimForward, CheLiangRotSpeed);
            RealNpcTran.forward = Vector3.MoveTowards(RealNpcTran.forward, MarkTranAimForward, CheLiangRotSpeed);
        }

        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            if (!IsDoFireAnimation
                && !IsFangZhenNpc
                && FireDistance > 0f
                && XkGameCtrl.PlayerActiveNum > 0
                && NpcState != NpcType.FlyNpc)
            {
                Vector3 posA = NpcTran.position;
                Vector3 posB = XkPlayerCtrl.PlayerTranFeiJi.position;

                bool isActiveFire = false;
                for (int i = 0; i < 2; i++)
                {
                    switch (i)
                    {
                        case 0:
                            posB = XKPlayerMoveCtrl.GetInstancePOne().transform.position;
                            posA.y = posB.y = 0f;
                            DisPlayerVal = Vector3.Distance(posA, posB);
                            if (DisPlayerVal <= FireDistance)
                            {
                                isActiveFire = true;
                                PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePOne();
                            }
                            break;
                        case 1:
                            posB = XKPlayerMoveCtrl.GetInstancePTwo().transform.position;
                            posA.y = posB.y = 0f;
                            DisPlayerVal = Vector3.Distance(posA, posB);
                            if (DisPlayerVal <= FireDistance)
                            {
                                isActiveFire = true;
                                PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePTwo();
                            }
                            break;
                    }

                    if (isActiveFire)
                    {
                        MakeNpcDoFireAnimation();
                        break;
                    }
                }
            }
        }
        else
        {
            if (!IsDoFireAnimation
                && !IsFangZhenNpc
                && FireDistance > 0f
                && XkGameCtrl.PlayerActiveNum > 0
                && NpcState != NpcType.FlyNpc)
            {
                Vector3 posA = NpcTran.position;
                Vector3 posB = XkPlayerCtrl.PlayerTranFeiJi.position;

                bool isActiveFire = false;
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (XkGameCtrl.IsActivePlayerOne)
                            {
                                posB = XKPlayerMoveCtrl.GetInstancePOne().transform.position;
                                posA.y = posB.y = 0f;
                                DisPlayerVal = Vector3.Distance(posA, posB);
                                if (DisPlayerVal <= FireDistance)
                                {
                                    isActiveFire = true;
                                    PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePOne();
                                }
                            }
                            break;
                        case 1:
                            if (XkGameCtrl.IsActivePlayerTwo)
                            {
                                posB = XKPlayerMoveCtrl.GetInstancePTwo().transform.position;
                                posA.y = posB.y = 0f;
                                DisPlayerVal = Vector3.Distance(posA, posB);
                                if (DisPlayerVal <= FireDistance)
                                {
                                    isActiveFire = true;
                                    PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePTwo();
                                }
                            }
                            break;
                        case 2:
                            if (XkGameCtrl.IsActivePlayerThree)
                            {
                                posB = XKPlayerMoveCtrl.GetInstancePThree().transform.position;
                                posA.y = posB.y = 0f;
                                DisPlayerVal = Vector3.Distance(posA, posB);
                                if (DisPlayerVal <= FireDistance)
                                {
                                    isActiveFire = true;
                                    PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePThree();
                                }
                            }
                            break;
                        case 3:
                            if (XkGameCtrl.IsActivePlayerFour)
                            {
                                posB = XKPlayerMoveCtrl.GetInstancePFour().transform.position;
                                posA.y = posB.y = 0f;
                                DisPlayerVal = Vector3.Distance(posA, posB);
                                if (DisPlayerVal <= FireDistance)
                                {
                                    isActiveFire = true;
                                    PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePFour();
                                }
                            }
                            break;
                    }

                    if (isActiveFire)
                    {
                        MakeNpcDoFireAnimation();
                        break;
                    }
                }
            }
        }

		if (IsAimPlayer && IsDoFireAnimation && !IsFireMove && !IsMoveFirePoint) {
			CheckNpcAimPlayer();
		}
	}

	void CheckNpcAimPlayer()
	{
		if (IsMoveByCar) {
			return;
		}

		if (IsZaiTiNpc) {
			return;
		}

		Vector3 pos = Vector3.zero;
		if (XkPlayerCtrl.PlayerTranFeiJi != null) {
			bool isGetCamPos = true;
			if (PlayerMoveScript != null) {
				if (!PlayerMoveScript.GetIsDeathPlayer()) {
					isGetCamPos = false;
					pos = PlayerMoveScript.transform.position;
				}
			}
			
			if (isGetCamPos) {
				GetAimPlayerMoveScript();
				//pos = XkPlayerCtrl.PlayerTranFeiJi.position;
				return;
			}
		}
		else {
			return;
		}
		
		pos.y = RealNpcTran.position.y;
		RealNpcTran.LookAt(pos);
	}
    
	public FirePoint GetFirePointScript()
	{
		if (SpawnPointScript == null || SpawnPointScript.FirePointNpc == null) {
			return null;
		}
		FirePointScript = SpawnPointScript.FirePointNpc.GetFirePoint();
		return FirePointScript;
	}
	
	public void SetIndexNpc(int val)
	{
//		IndexNpc = val;
		SetFirePointScript();
	}

	void SetFirePointScript()
	{
		FirePointScript = null;
		IndexFirePointGroup = 0;
//		if (SpawnPointScript.FirePointGroup.Length < 1) {
//			FirePointScript = null;
//			IndexFirePointGroup = 0;
//			return;
//		}

//		if (SpawnPointScript.FirePointGroup.Length <= IndexFirePointGroup) {
//			if (!SpawnPointScript.IsLoopFirePoint || SpawnPointScript.FirePointGroup.Length == 1) {
//				FirePointScript = null;
//				return;
//			}
//			else {
//				IndexFirePointGroup = 0;
//			}
//		}
//		Debug.Log("Unity:"+"SetFirePointScript...");

//		FirePointCtrl pointCtrl = SpawnPointScript.FirePointGroup[IndexFirePointGroup];
//		Transform pointCtrlTran = pointCtrl.transform;
//		Debug.Log("Unity:"+"IndexNpc "+IndexNpc+", childCount "+pointCtrlTran.childCount);
//		if (IndexNpc < pointCtrlTran.childCount) {
//			FirePointScript = pointCtrlTran.GetChild(IndexNpc).GetComponent<FirePoint>();
//		}
//		else {
//			int rv = Random.Range(0, pointCtrlTran.childCount);
//			FirePointScript = pointCtrlTran.GetChild(rv).GetComponent<FirePoint>();
//		}
	}

	public void MakeNpcMoveFirePoint()
    {
        //Debug.Log("**************************move 3333333333333333333333");
        if (IsDeathNPC) {
			return;
		}

		if (IsMoveFirePoint) {
			return;
		}
		IsMoveFirePoint = true;
		IsChangeNpcForward = false;
		RealNpcTran.localEulerAngles = Vector3.zero;

		FireAnimation = FirePointScript.AniFireName;
		float mvSpeed = SpawnPointScript.SpeedFangZhenFireRun > 0f ? SpawnPointScript.SpeedFangZhenFireRun : 1f;
		Vector3 firePos = FirePointScript.transform.position;
		Vector3[] posArray = new Vector3[2];
		posArray[0] = NpcTran.position;
		posArray[1] = firePos;
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			Destroy(itweenScript);
			//Debug.Log("*************************2222 name  " + gameObject.name);
		}
		
		iTween.MoveTo(NpcObj, iTween.Hash("path", posArray,
		                                  "speed", mvSpeed,
		                                  "orienttopath", true,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "OnCompelteMakeNpcMoveFirePoint"));

		IndexFirePointGroup++;
		SetFirePointScript();
	}

	void OnCompelteMakeNpcMoveFirePoint()
	{
		IsMoveFirePoint = false;
		SetIsDoFireAnimation(true);
		RandomPlayFireAction(NpcAniScript, false); //play fire action
	}

	public void ActiveIsFangZhenNpc()
	{
		IsFangZhenNpc = true;
	}

	public void SetIsFangZhenNpc(XKNpcFangZhenCtrl fangZhenScript, int index)
	{
		IsFangZhenNpc = true;
		NpcFangZhenScript = fangZhenScript;
		XkNpcZaiTiCtrl script = GetComponentInChildren<XkNpcZaiTiCtrl>();
		if (script != null) {
			script.SetFangZhenZaiTiNpcAni(index);
		}
	}

	public void SetNpcForwardVal(Vector3 val)
	{
		if (IsDeathNPC) {
			return;
		}
		IsChangeNpcForward = true;
		ForwardValNpc = val;
	}

	bool IsInitNpcInfo;
	void InitNpcInfo()
	{
		if (IsInitNpcInfo) {
			return;
		}
		IsInitNpcInfo = true;

		if (IsDeathNPC) {
			NpcObj = null;
			IsDeathNPC = false;
			RealNpcTran.gameObject.SetActive(true);
            NpcTran.SetParent(XkGameCtrl.NpcObjArray);
        }

		if (NpcObj != null)
        {
            if (NpcAniScript != null)
            {
                NpcAniScript.gameObject.SetActive(true);
                XKNpcHealthCtrl healthScriptTmp = NpcAniScript.gameObject.GetComponent<XKNpcHealthCtrl>();
                if (healthScriptTmp != null)
                {
                    healthScriptTmp.SetNpcMoveScript(this);
                }
            }
            return;
		}
		NpcObj = gameObject;
		NpcTran = transform;

//		NpcAniScript = RealNpcTran.gameObject.GetComponent<XKNpcAnimatorCtrl>();
//		if (NpcAniScript == null) {
//			NpcAniScript = RealNpcTran.gameObject.AddComponent<XKNpcAnimatorCtrl>();
//		}
//		NpcAniScript.SetAmmoPrefabVal(this);

		AnimatorCom = RealNpcTran.GetComponent<Animator>();
		if (AnimatorCom != null) {
			AnimatorCom.enabled = true;
		}
		BoxCollider boxCol = NpcObj.GetComponent<BoxCollider>();
		if (boxCol == null) {
			boxCol = NpcObj.AddComponent<BoxCollider>();
			boxCol.center = new Vector3(0f, 0.5f, 0f);
			boxCol.size = new Vector3(0.2f, 0.2f, 0.2f);
		}
		boxCol.enabled = true;

		if (IsZaiTiNpc) {
			InitZaiTiNpcInfo();
//			int max = ZaiTiNpcAni.Length;
//			ZaiTiNpcAniScript = new XKNpcAnimatorCtrl[max];
//			for (int i = 0; i < max; i++) {
//				if (XkGameCtrl.GetInstance().IsCartoonShootTest && ZaiTiNpcAni[i] == null) {
//					continue;
//				}
//
//				if (ZaiTiNpcAni[i].gameObject.GetComponent<XKNpcAnimatorCtrl>() != null) {
//					//Debug.LogWarning("Unity:"+"XKNpcAnimatorCtrl is not null, name "+gameObject.name);
//					continue;
//				}
//				ZaiTiNpcAniScript[i] = ZaiTiNpcAni[i].gameObject.AddComponent<XKNpcAnimatorCtrl>();
//				ZaiTiNpcAniScript[i].SetAmmoPrefabVal(this);
//			}

			XKNpcHealthCtrl healthScript = RealNpcTran.GetComponent<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
			else {
				healthScript = GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null)
                {
					healthScript.SetNpcMoveScript(this);
				}
                else
                {
                    if (NpcAniScript != null)
                    {
                        NpcAniScript.gameObject.SetActive(true);
                        healthScript = NpcAniScript.gameObject.GetComponent<XKNpcHealthCtrl>();
                        if (healthScript != null)
                        {
                            healthScript.SetNpcMoveScript(this);
                        }
                    }
                }
			}
		}
		else {
			XKNpcHealthCtrl healthScript = GetComponentInChildren<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
			
			XkNpcZaiTiCtrl zaiTiScript = GetComponentInChildren<XkNpcZaiTiCtrl>();
			BuWaWaRigidbody = zaiTiScript.ZaiTiNpcBuWaWa;
		}

		if (MoveStyle == UITweener.Style.Loop) {
			SetNpcCheLunIsRun(false);
		}
	}

	public void SetNpcSpawnScriptInfo(XKSpawnNpcPoint spawnScript)
	{
		if (IsDeathNPC) {
			IsDeathNPC = false;
			if (AnimatorCom == null) {
				AnimatorCom = RealNpcTran.GetComponent<Animator>();
			}

			if (AnimatorCom != null) {
				AnimatorCom.enabled = true;
			}
			RealNpcTran.gameObject.SetActive(true);
        }

        if (RealNpcTran != null)
        {
            XKNpcHealthCtrl healthScript = RealNpcTran.GetComponent<XKNpcHealthCtrl>();
            if (healthScript != null)
            {
                healthScript.SetNpcMoveScript(this);
            }
        }

        SpawnPointScript = spawnScript;

		TimeFire = SpawnPointScript.TimeFire;
		TimeRun = SpawnPointScript.TimeRun;
		if (IsZaiTiNpc) {
			IsFireMove = true;		
		} else {
			IsFireMove = SpawnPointScript.IsFireMove;
		}
		IsAimPlayer = SpawnPointScript.IsAimPlayer;
		NpcPathTran = SpawnPointScript.NpcPath;
		if (!IsDoFireAnimation) {
			if ( (NpcPathTran == null && (int)SpawnPointScript.AniRunName < (int)AnimatorNameNPC.Run1
			      && (int)SpawnPointScript.AniRunName > (int)AnimatorNameNPC.Run4)
			    || NpcPathTran != null )
			{
				PlayNpcAnimation(SpawnPointScript.AniRunName);
			}
		}
		MvSpeed = SpawnPointScript.MvSpeed;
		FireDistance = SpawnPointScript.FireDistance;

		SetFirePointScript();
	}
	
	void SetNpcIsAimFeiJiPlayer(bool isAim)
	{
		XKNpcAnimatorCtrl[] npcAniScript = GetComponentsInChildren<XKNpcAnimatorCtrl>();
		for (int i = 0; i < npcAniScript.Length; i++) {
			npcAniScript[i].SetIsAimFeiJiPlayer(isAim);
		}

		XkNpcZaiTiCtrl[] zaiTiScript = GetComponentsInChildren<XkNpcZaiTiCtrl>();
		for (int i = 0; i < zaiTiScript.Length; i++) {
			zaiTiScript[i].SetIsAimFeiJiPlayer(isAim);
		}
	}
	

	XKCannonCtrl[] CannonScript;
	WaypointProgressTracker WaypointCom;
	public void SetSpawnNpcInfo(XKSpawnNpcPoint spawnScript)
	{
        AddPathNodeData(spawnScript.NpcPath);
		SetNpcSpawnScriptInfo(spawnScript);
		IsHuoCheNpc = spawnScript.GetIsHuoCheNpc();
		TestSpawnPoint = spawnScript.gameObject;
		if (spawnScript.NpcPath != null) {
			NpcPathScript = spawnScript.NpcPath.GetComponent<NpcPathCtrl>();
		}

		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = GetComponentsInChildren<XKCannonCtrl>();
		}
		if (CannonScript.Length > 0) {
			int max = CannonScript.Length;
			//Debug.Log("Unity:"+"SetSpawnNpcInfo -> max "+max);
			for (int i = 0; i < max; i++) {
				CannonScript[i].SetSpawnPointScript(null);
			}
			SetCannonAimPlayerState();
		}

		InitNpcInfo();
		if (SpawnPointScript.TimeRootAni > 0f) {
			if (!IsDoFireAnimation) {
					PlayNpcAnimation(SpawnPointScript.AniRootName);
			}
		}
		
		RigCom = GetComponent<Rigidbody>();
		if (NpcPathTran != null) {
			if (RigCom != null) {
				RigCom.isKinematic = true;
			}

			if (!IsMoveByCar) {
				StartCoroutine(StartMoveNpcByItween(SpawnPointScript.AniRunName, SpawnPointScript.TimeRootAni));
			}
			else {
				if (WaypointCom == null) {
					WaypointCom = GetComponent<WaypointProgressTracker>();
				}

				if (AiCarCom == null) {
					AiCarCom = GetComponent<XKAiCarMoveCtrl>();
				}
                AiCarCom.SetActiveHiddenAiCarObj(true);
                AiCarCom.SetAiCarTopMoveSpeed(SpawnPointScript.MvSpeed);
				WaypointCom.SetCarPathInfo(NpcPathTran.GetComponent<WaypointCircuit>());
			}
		}
	}

	void SetCannonAimPlayerState()
	{
		SetCannonNpcInfo(0, SpawnPointScript.FireDistance);
	}
	
	void SetCannonNpcInfo(int valAim, float valFireDis)
	{
		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = GetComponentsInChildren<XKCannonCtrl>();
		}

		int max = CannonScript.Length;
		for (int i = 0; i < max; i++) {
			CannonScript[i].SetCannonSpawnPointInfo(valAim, valFireDis);
		}
	}

	void DelayCheckNpcAniController()
	{
		Animator aniCom = GetComponent<Animator>();
		//aniCom.runtimeAnimatorController = SpawnPointScript.AniController;
		aniCom.enabled = true;
	}
	
	IEnumerator StartMoveNpcByItween(AnimatorNameNPC aniVal, float rootTime)
	{
		yield return new WaitForSeconds(rootTime);
		if (IsDeathNPC || (IsDoFireAnimation && !IsFireMove)) {
			yield break;
		}
		
		AnimatorNameNPC runAni = AnimatorNameNPC.Run1;
		if (aniVal == AnimatorNameNPC.Run1
		    || aniVal == AnimatorNameNPC.Run2
		    || aniVal == AnimatorNameNPC.Run3
		    || aniVal == AnimatorNameNPC.Run4) {
			runAni = aniVal;
		}
		else {
			runAni = XKNpcFangZhenCtrl.GetRandRunAniName();
		}

		if (!IsDoFireAnimation) {
			PlayNpcAnimation(runAni);
		}
		MoveNpcByItween();
		yield break;
	}

	public void PlayNpcAnimation(AnimatorNameNPC aniVal)
	{
		if (IsZaiTiNpc || IsDeathNPC) {
			return;
		}

		if (aniVal != AnimatorNameNPC.Null) {
			if (aniVal == AnimatorNameNPC.Run1
			    || aniVal == AnimatorNameNPC.Run2
			    || aniVal == AnimatorNameNPC.Run3
			    || aniVal == AnimatorNameNPC.Run4) {
				CurrentRunAnimation = aniVal.ToString();
                if (NpcAniScript != null && NpcAniScript.AnimatorCom != null && NpcAniScript.AnimatorCom.speed == 0f)
                {
                    NpcAniScript.ResetFireAnimationSpeed();
                }
            }

			if (NpcAniScript != null) {
				NetNpcPlayAnimation(NpcAniScript, aniVal.ToString());
			}
		}
	}

    /// <summary>
    /// 是否为彩票boss或战车Npc.
    /// </summary>
    internal bool IsCaiPiaoZhanChe = false;
    internal void SetIsCaiPiaoZhanChe(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        IsEnterCameraBox = false;
        IsCaiPiaoZhanChe = true;

        //int rv = Random.Range(0, 100) % 2;
        //if (rv == 0)
        //{
        //    m_DaiJinQuanState = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01;
        //}
        //else
        //{
        //    m_DaiJinQuanState = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02;
        //}
        m_DaiJinQuanState = type;

        //彩票战车npc.
        IsZhanCheNpc = true;
    
        if (NpcMoveType == NpcJiFenEnum.FeiJi)
        {
        }
        else
        {
            //彩票战车或boss设置为车辆运动.
            IsCheLiangMoveType = true;
        }

        if (RealNpcTran != null)
        {
            XKNpcHealthCtrl healthScript = RealNpcTran.GetComponent<XKNpcHealthCtrl>();
            if (healthScript != null)
            {
                healthScript.SetNpcMoveScript(this);
            }
        }

        Rigidbody rig = gameObject.GetComponent<Rigidbody>();
        if (rig == null)
        {
            rig = gameObject.AddComponent<Rigidbody>();
            rig.isKinematic = true;
            rig.freezeRotation = true;
        }
    }
    
    public void MoveNpcByItween()
	{
        //Debug.Log("**************************move 1111111111111111");
        if (IsDeathNPC) {
			return;
		}

		if (NpcPathTran == null) {
			//Debug.Log("Unity:"+"The npc has no path! name "+gameObject.name);
			return;
		}

        if (NpcState == NpcType.FlyNpc)
        {
            if (NpcPathTran != null)
            {
                NpcPathTran.gameObject.SetActive(true);
                List<Transform> tranList = new List<Transform>(NpcPathTran.GetComponentsInChildren<Transform>()) { };
                tranList.Remove(NpcPathTran);

                NpcMark markScript = tranList[0].GetComponent<NpcMark>();
                if (markScript != null)
                {
                    SetIsDoFireAnimation(markScript.IsFireFeiJiNpc);
                }
                //MarkTranAim = tranList[0];
                //MarkNpcMove = MarkTranAim;
                MarkNpcMovePos = tranList[0].position;
                MarkCount++;
                iTween.MoveTo(NpcObj, iTween.Hash("path", tranList.ToArray(),
                                                  "speed", MvSpeed,
                                                  "orienttopath", true,
                                                  "easeType", iTween.EaseType.linear));

                NpcPathTran.gameObject.SetActive(false);
            }
			return;
		}

		//Transform[] tranArray = new Transform[2];
        Vector3[] nodesArray = new Vector3[2];
        //tranArray[0] = NpcTran;
        nodesArray[0] = NpcTran.position;
        //if (IsCaiPiaoZhanChe)
        //{
        //    //彩票boss或战车类型npc获取路径最后一个点.
        //    MarkCount = NpcPathTran.childCount - 1;
        //}
        //MarkCount = NpcPathTran.childCount - 1; //test.

        if (MarkCount >= NpcPathTran.childCount || MarkCount < 0) {
			MarkCount = 0; //fixed MarkCount
		}
		//tranArray[1] = NpcPathTran.GetChild(MarkCount);
        nodesArray[1] = m_PathNodeList[MarkCount];

        MarkCount++;
		if (IsHuoCheNpc) {
			//StartCoroutine(MoveNpcByLocalPos(tranArray[1], MvSpeed));
		}
		else {
			//MarkNpcMove = tranArray[1];
            MarkNpcMovePos = nodesArray[1];
            IsMoveToMarkPoint = false;

			bool isOrienttopath = true;
//			if (NpcJiFen == NpcJiFenEnum.FeiJi  || NpcJiFen == NpcJiFenEnum.ChuanBo
			if (IsCheLiangMoveType) {
				isOrienttopath = false;
				//MarkTranAim = tranArray[1];
				MarkTranAimForward = nodesArray[1] - nodesArray[0];
                MarkTranAimForward.y = 0f;
                MarkTranAimForward = MarkTranAimForward.normalized;
            }

            if (IsCaiPiaoZhanChe)
            {
                isOrienttopath = false;
                //MarkTranAim = tranArray[1];
                MarkTranAimForward = nodesArray[1] - nodesArray[0];
                MarkTranAimForward.y = 0f;
                MarkTranAimForward = MarkTranAimForward.normalized;
                //if (RealNpcTran != null)
                //{
                //    RealNpcTran.forward = MarkTranAimForward;
                //}
                //Debug.Log("********************************************* boss *************** isOrienttopath == " + isOrienttopath);
            }

			if (MoveStyle == UITweener.Style.Loop) {
				isOrienttopath = false;
			}

			//Debug.Log("********************************* name " + gameObject.name);
            iTween.MoveTo(NpcObj, iTween.Hash("path", nodesArray,
			                                  "speed", MvSpeed,
			                                  "orienttopath", isOrienttopath,
			                                  "easeType", iTween.EaseType.linear));

			if (MoveStyle == UITweener.Style.Loop) {
				SetNpcCheLunIsRun(true);
			}
		}
	}

	void CheckMoveNpcOnCompelteITween()
	{
		if (IsMoveByCar) {
			return;
		}

		if (IsMoveToMarkPoint) {
			return;
		}

		if (NpcPathTran == null) {
			return;
		}

        if (m_CaiPiaoBossMoveCom != null
            && m_CaiPiaoBossMoveCom.IsMoveNpc == true)
        {
            //彩票boss正在进行特殊移动.
            return;
        }

        Vector3 posA = NpcObj.transform.position;
        Vector3 posB = MarkNpcMovePos;
        if (IsCaiPiaoZhanChe)
        {
            //彩票战车或boss的高度值归零.
            posA.y = posB.y = 0f;
        }
        float dis = Vector3.Distance(posA, posB);
		float disMin = MvSpeed * Time.deltaTime;
		if (MarkCount >= NpcPathTran.childCount) {
			float disMinTmp = 0.5f;
			disMin = disMin > disMinTmp ? disMinTmp : disMin;
		}

		if (dis > disMin) {
			return;
		}
		//Debug.Log("Unity:"+"CheckMoveNpcOnCompelteITween -> npc has moved to markPoint. npcName ==== " + gameObject.name);
		IsMoveToMarkPoint = true;


        if (IsCaiPiaoZhanChe == true)
        {
            if (IsBossNpc == false)
            {
                //Debug.Log("Unity:"+ "CheckMoveNpcOnCompelteITween -> MarkCount ==== " + MarkCount
                //    + ", NpcPathTran.childCount ======== " + NpcPathTran.childCount
                //    + ", npcName ====== " + gameObject.name);
                //彩票战车
                if (MarkCount == 2)
                {
                    //战车npc走到路径第2个点时停止镜头移动.
                    XkGameCtrl.GetInstance().SetGameCameraIsMoveing(false, NpcJiFenEnum.CheLiang);
                }

                if (MarkCount == NpcPathTran.childCount - 1)
                {
                    //战车npc走到路径倒数第2个点时打开镜头移动.
                    XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.CheLiang);
                }
            }
        }

        if (NpcState == NpcType.FlyNpc)
        {
			if (MarkCount >= NpcPathTran.childCount)
            {
                if (IsCaiPiaoZhanChe == false)
                {
                    MakeNpcLeaveCamera();
                }
				return;
			}
			//Debug.Log("Unity:"+"MarkCount***"+MarkCount);

			//MarkTranAim = NpcPathTran.GetChild(MarkCount);
			NpcMark markScript = NpcPathTran.GetChild(MarkCount).GetComponent<NpcMark>();
			if (markScript != null)
            {
				SetIsDoFireAnimation(markScript.IsFireFeiJiNpc);
			}
			//MarkNpcMove = MarkTranAim;
            MarkNpcMovePos = m_PathNodeList[MarkCount];
            MarkCount++;
			IsMoveToMarkPoint = false;
			return;
		}

		MoveNpcOnCompelteITween();
	}

    /// <summary>
    /// 远离摄像机.
    /// </summary>
    public bool IsLeaveCamera = false;
    /// <summary>
    /// 使npc远离摄像机.
    /// </summary>
    void MakeNpcLeaveCamera()
    {
        if (IsLeaveCamera == true)
        {
            return;
        }

        IsLeaveCamera = true;
        Vector3[] path = new Vector3[2];
        path[0] = transform.position;
        //Vector3 forwardVal = transform.forward;
        //forwardVal.y = 0f;
        //Vector3 pos = path[0] + forwardVal.normalized * 80f;

        //路的宽度80米.
        Vector3 posA = path[0];
        Vector3 posB = Camera.main.transform.position;
        posA.y = posB.y = 0f;
        if (Vector3.Distance(posA, posB) <= 40f)
        {
            //npc停在了路径中,使npc向左右运动离开路径.
            Vector3 vecBA = posA - posB;
            float sign = Vector3.Dot(vecBA, Camera.main.transform.right) >= 0 ? 1f : -1f;
            Vector3 forwardVal = Camera.main.transform.right;
            forwardVal.y = 0f;
            Vector3 pos = path[0] + (sign * forwardVal * 80f);
            path[1] = pos;
            iTween.MoveTo(NpcObj, iTween.Hash("path", path,
                                              "speed", MvSpeed,
                                              "orienttopath", true,
                                              "easeType", iTween.EaseType.linear));
        }
    }

	IEnumerator MoveNpcByLocalPos(Transform endTran, float speed)
	{
		bool isMoveNpc = true;
		float timeVal = 0.03f;
		float speedVal = speed * timeVal;
		Vector3 endPos = NpcTran.position;
		Vector3 startPos = endTran.position;
		endPos.y = startPos.y = 0f;
		Vector3 forwardVal = Vector3.Normalize(endPos - startPos);
		Vector3 vecSpeed = forwardVal * speedVal;
		do {
			if (!IsHuoCheNpc || IsDeathNPC) {
//				Debug.LogWarning("Unity:"+"MoveNpcByLocalPos -> IsHuoCheNpc "+IsHuoCheNpc
//				          +", IsDeathNPC "+IsDeathNPC+", npcName "+NpcObj.name);
				yield break;
			}

			endPos = NpcTran.position;
			startPos = endTran.position;
			if (Vector3.Distance(endPos, startPos) > speedVal) {
				endPos.y = startPos.y = 0f;
				forwardVal = Vector3.Normalize(endPos - startPos);
				vecSpeed = forwardVal * speedVal;
			}

			float spLX = vecSpeed.z;
			float spLY = vecSpeed.x;
			float spLZ = vecSpeed.y;
			vecSpeed.x = spLY;
			vecSpeed.y = spLZ;
			vecSpeed.z = spLX;
			NpcTran.localPosition += vecSpeed;
			NpcTran.right = Vector3.Lerp(NpcTran.forward, forwardVal, 0.5f);
			if (Vector3.Distance(endPos, startPos) < 2f*speedVal) {
				isMoveNpc = false;
				break;
			}
			yield return new WaitForSeconds(0.03f);
		} while (isMoveNpc);

		OnFinishedMoveNpcByLocalPos();
	}

	void OnFinishedMoveNpcByLocalPos()
	{
		if (MarkCount >= NpcPathTran.childCount) {
			if (NpcPathScript != null && NpcPathScript.IsMoveEndFire) {
				MakeNpcDoFireAnimation(); //play fire animation
			}
			else {
				MakeNpcPlayRootAnimation();
			}
			return;
		}

		if (IsDoFireAnimation && !IsFireMove) {
			return;
		}
		Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		IsWuDi = markScript.IsWuDi;
		/*if (!IsDoFireAnimation) {
			PlayNpcAnimation(markScript.AniName);
		}*/
		MoveNpcByItween();
		//Debug.Log("Unity:"+"MoveNpcOnCompelteITween...npc is "+NpcObj.name);
//		if (markScript.AnimatorTime > 0f && markScript.AniName != AnimatorNameNPC.Null) {
//			Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
//		}
//		else {
//			MoveNpcByItween();
//		}
	}

	void InitZaiTiNpcInfo()
	{
		XkNpcZaiTiCtrl[] zaiTiScript = GetComponentsInChildren<XkNpcZaiTiCtrl>();
		int max = zaiTiScript.Length;
		List<Animator> zaiTiAniList = new List<Animator>(max);
		List<Rigidbody> zaiTiRigList = new List<Rigidbody>(max);
		for (int i = 0; i < zaiTiScript.Length; i++) {
			zaiTiScript[i].SetIsZaiNpc();
			zaiTiAniList.Add(zaiTiScript[i].ZaiTiNpcAni);
			zaiTiRigList.Add(zaiTiScript[i].ZaiTiNpcBuWaWa);
		}
		ZaiTiNpcAni = zaiTiAniList.ToArray();
		ZaiTiNpcBuWaWa = zaiTiRigList.ToArray();
	}

	public bool GetIsAimPlayerByFire()
	{
		return IsAimPlayerByFire;
	}

	void SetIsAimPlayerByFire(bool isAim)
	{
		IsAimPlayerByFire = isAim;
		if (!isAim) {
			//Play npc fire action
			if (!IsZaiTiNpc) {
				NpcAniScript.ResetFireAnimationSpeed();
			}
//			else {
//				Debug.Log("Unity:"+"ZaiTiNpcFireCount *** "+ZaiTiNpcFireCount);
//				if (ZaiTiNpcAniScript[ZaiTiNpcFireCount] == null) {
//					return;
//				}
//				ZaiTiNpcAniScript[ZaiTiNpcFireCount].ResetFireAnimationSpeed();
//				ZaiTiNpcFireCount++;
//			}
		}
	}

	IEnumerator DelayResetIsAimPlayerByFire(float timeVal)
	{
		yield return new WaitForSeconds(timeVal);
		SetIsAimPlayerByFire(false);
//		Debug.Log("Unity:"+"DelayResetIsAimPlayerByFire***name "+gameObject.name);
	}
    
	void DelayMakeNpcMoveDoRun3()
	{
		if (ITweenScriptNpc != null)
        {
            //Debug.Log("************************33333 time " + Time.time);
            PlayNpcAnimation(AnimatorNameNPC.Run3);
			RealNpcTran.localEulerAngles = Vector3.zero;
			ITweenScriptNpc.isRunning = true;
			ITweenScriptNpc.isPaused = false;
			IsFireMove = true;

			if (TimeFire > 0f) {
				if (IsFangZhenNpc) {
					TimeFire = Random.Range(TimeFire, TimeRun+5f);
				}

				if (IsInvoking("DelayMakeNpcDoFireAni")) {
					CancelInvoke("DelayMakeNpcDoFireAni");
				}
				Invoke("DelayMakeNpcDoFireAni", TimeRun);
			}
		}
		else {
			DelayMakeNpcDoFireAni();
		}
	}

	void DelayMakeNpcDoFireAni()
	{
		IsFireMove = false;
		MakeNpcDoFireAnimation();
	}

	public void MakeNpcDoFireAnimation()
	{
		//Debug.Log("Unity:"+"MakeNpcDoFireAnimation****");
		if (IsDeathNPC) {
			return;
		}

		if (!IsFireMove) {
			if (IsMoveByCar) {
				AiCarCom.SetIsStopMoveCar(true);
			}

			if (MarkScriptVal == null || !MarkScriptVal.IsDoFireAction) {
				iTween itweenScript = GetComponent<iTween>();
				if (itweenScript != null) {
					itweenScript.isRunning = false;
					itweenScript.isPaused = true;
					ITweenScriptNpc = itweenScript;
					
					if (TimeRun > 0f) {
						if (IsFangZhenNpc) {
							TimeRun = Random.Range(TimeRun, TimeRun+5f);
						}
						
						if (IsInvoking("DelayMakeNpcMoveDoRun3")) {
							CancelInvoke("DelayMakeNpcMoveDoRun3");
						}
						Invoke("DelayMakeNpcMoveDoRun3", TimeRun);
					}
//					itweenScript.enabled = false;
//					DestroyObject(itweenScript);
				}
			}
		}
		else {
			if (NpcMoveType == NpcJiFenEnum.ShiBing && !IsZaiTiNpc) {
				//Debug.Log("Unity:"+"***********play run4");
				PlayNpcAnimation(AnimatorNameNPC.Run4);
				return;
			}
		}

		SetIsDoFireAnimation(true);
		RandomPlayFireAction(NpcAniScript);
//		DelayPlayFireAction();
//		NpcAniScript.SetCountHuanDan(CountHuanDan);
	}

	public void SetIsDoFireAnimation(bool isDoFire)
	{
		//Debug.Log("Unity:"+"SetIsDoFireAnimation...isDoFire "+isDoFire);
		if (isDoFire == IsDoFireAnimation) {
			return;
		}
		IsDoFireAnimation = isDoFire;
		//SetClientNpcIsDoFireAnimation(isDoFire);
	}

	void RandomPlayFireAction(XKNpcAnimatorCtrl aniScript, bool isRandom = true)
	{
		if (aniScript == null) {
			return;
		}

		float randVal = Random.Range(0f, 1f);
		AnimatorNameNPC aniName = AnimatorNameNPC.Fire1;
		aniScript.ResetIsDoRunFireAction();
		if (!isRandom) {
			switch (FireAnimation) {
			case NpcFireAction.Fire1_4:
				aniName = AnimatorNameNPC.Fire1;
				break;

			case NpcFireAction.Fire2_5:
				aniName = AnimatorNameNPC.Fire2;
				break;
			default:
				aniName = AnimatorNameNPC.Fire1;
				break;
			}
		}
		else {
			if (randVal < DoFire1) {
				aniName = AnimatorNameNPC.Fire1;
			}
			else {
				aniName = AnimatorNameNPC.Fire2;
			}
		}
		NetNpcPlayAnimation(aniScript, aniName.ToString());
	}

	public bool GetIsDoFireAnimation()
	{
		return IsDoFireAnimation;
	}

	public bool GetIsWuDi()
	{
		return IsWuDi;
	}
	
	void MakeNpcPlayRootAnimation()
	{
		PlayNpcAnimation(AnimatorNameNPC.Root1);
	}

	NpcMark MarkScriptVal;
	public NpcMark GetMarkScriptVal()
	{
		return MarkScriptVal;
	}

	void MoveNpcOnCompelteITween()
	{
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isPaused = true;
			itweenScript.isRunning = false;
			itweenScript.enabled = false;
			DestroyObject(itweenScript);
			//Debug.Log("*************************4444 name  " + gameObject.name);
		}

		NpcMark markScript = null;
		MarkScriptVal = null;
		if (MoveStyle != UITweener.Style.Loop) {
			if (MarkCount <= 0) {
				return;
			}
			
			if (MarkCount >= NpcPathTran.childCount)
            {
                if (IsCaiPiaoZhanChe == false)
                {
                    MakeNpcLeaveCamera();
                    return;
                }

                if (NpcPathScript != null && NpcPathScript.IsMoveEndFire)
                {
					MakeNpcDoFireAnimation(); //play fire animation
				}
				else
                {
					MakeNpcPlayRootAnimation();
				}
				return;
			}
			
			if (IsDoFireAnimation && !IsFireMove) {
				return;
			}
			
			Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
			markScript = markTran.GetComponent<NpcMark>();
            if (IsCaiPiaoZhanChe == false)
            {
                //普通npc获取路径点上的速度.
                MvSpeed = markScript.MvSpeed;
			    IsWuDi = markScript.IsWuDi;

                if (!IsDoFireAnimation)
                {
                    PlayNpcAnimation(markScript.AniName);
                }

                if (markScript.IsDoFireAction)
                {
                    MarkScriptVal = markScript;
                    MakeNpcDoFireAnimation();
                    return;
                }
            }
		}
		else {
			Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
			markScript = markTran.GetComponent<NpcMark>();
		}

        //Debug.Log("Unity:"+"MoveNpcOnCompelteITween...npc is "+NpcObj.name);
        if (IsCaiPiaoZhanChe == false)
        {
            if (markScript.AnimatorTime > 0f)
            {
                switch (MoveStyle)
                {
                    case UITweener.Style.Loop:
                        if (MoveStyle == UITweener.Style.Loop)
                        {
                            SetNpcCheLunIsRun(false);
                        }
                        Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
                        break;
                    default:
                        if (markScript.AniName != AnimatorNameNPC.Null)
                        {
                            Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
                        }
                        break;
                }
                return;
            }
        }
		MoveNpcByItween();
	}

	public void SetFeiJiMarkInfo(NpcMark script)
	{
		IsWuDi = script.IsWuDi;
//		if (IsWuDi) {
//			Debug.Log("Unity:"+"SetFeiJiMarkInfo ********* IsWuDi "+IsWuDi);
//		}
	}

	/**
	 * MoveStyle == UITweener.Style.Loop -> 控制boss循环运动.
	 */
	public UITweener.Style MoveStyle = UITweener.Style.Once;
	void DelayMoveNpcWaitAnimationEnd()
	{
		switch (MoveStyle) {
		case UITweener.Style.Loop:
			MoveNpcByItween();
			break;
		default:
			if (IsDoFireAnimation && !IsFireMove) {
				return;
			}
			
			if (!IsZaiTiNpc && !IsDoFireAnimation) {
				NetNpcPlayAnimation(NpcAniScript, CurrentRunAnimation);
			}
			MoveNpcByItween();
			break;
		}
	}

	public void NetNpcPlayAnimation(XKNpcAnimatorCtrl aniScript, string aniName)
	{
		aniScript.PlayNpcAnimatoin(aniName);
	}

	public bool GetIsDeathNPC()
	{
		return IsDeathNPC;
	}

	void DestroyNpc()
	{
//		if (IsAniMove) {
//			StartCoroutine(DestroyNetNpcObj(NpcObj));
//		}
//		else {
//			StartCoroutine(DestroyNetNpcObj(NpcObj, RemoveNpcTime));
//		}
		StartCoroutine(DestroyNetNpcObj(NpcObj, RemoveNpcTime));
	}

    //void CallServerRemoveNpc()
    //{
    //}

    //void CallServerRemoveCannon(int cannonIndex)
    //{
    //}

    /// <summary>
    /// 彩票boss特殊移动控制脚本.
    /// </summary>
    internal SSCaiPiaoBossMove m_CaiPiaoBossMoveCom;

    /// <summary>
    /// npc彩票显示组件.
    /// </summary>
    internal SSCaiPiaoNpcUI m_CaiPiaoNpcUI;
    public void TriggerRemovePointNpc(int key, XKCannonCtrl cannonScriptVal = null,
	                                  PlayerAmmoType pAmmoType = PlayerAmmoType.Null)
	{
        //Debug.LogWarning("Unity: TriggerRemovePointNpc -> ************* key == " + key + ", npcName == " + gameObject.name);
		if (IsDeathNPC)
        {
			return;
		}
		IsDeathNPC = true;
		HandleNpcDeathInfo();

        if (IsCaiPiaoZhanChe == true)
        {
            IsEnterCameraBox = false;
            if (IsBossNpc == true)
            {
                //彩票boss被删除.
                //使镜头继续移动.
                XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.Boss);
                if (m_CaiPiaoBossMoveCom != null)
                {
                    m_CaiPiaoBossMoveCom.ResetInfo();
                }
            }
            else
            {
                //彩票战车被删除.
                //使镜头继续移动.
                XkGameCtrl.GetInstance().SetGameCameraIsMoveing(true, NpcJiFenEnum.CheLiang);
            }

            if (m_CaiPiaoNpcUI != null)
            {
                m_CaiPiaoNpcUI.HiddenNumUI();
            }

            //删除彩票战车和boss的血条UI.
            SSUIRoot.GetInstance().m_GameUIManage.RemoveDaiJinQuanNpcXueTiaoUI();
        }

		//XkGameCtrl.ClearNpcSpawnAllAmmo(gameObject);
		//CallServerRemoveNpc();
		CancelInvoke("DelayMoveNpcWaitAnimationEnd");
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			Destroy(itweenScript);
			//Debug.Log("*************************1111 name  " + gameObject.name);
		}

		if (AnimatorCom != null) {
			AnimatorCom.enabled = false;
		}

		if (key == 0)
        {
			StartCoroutine(DestroyNetNpcObj(NpcObj));
		}
		else
        {
			if (NpcTran.parent != XkGameCtrl.MissionCleanup) {
				NpcTran.parent = XkGameCtrl.NpcObjArray;
			}

			if (Vector3.Distance(Vector3.zero, RealNpcTran.localPosition) > 0.1f
			    && Network.peerType == NetworkPeerType.Disconnected) {
				//Debug.Log("Unity:"+"fix realnpc localPosition, name "+NpcObj.name);
				Vector3 posTmpVal = RealNpcTran.position;
				RealNpcTran.parent = XkGameCtrl.NpcObjArray;
				NpcObj.transform.position = posTmpVal;
				RealNpcTran.parent = NpcObj.transform;
				RealNpcTran.localPosition = Vector3.zero;
			}
			
			if (BuWaWaRigidbody != null && !IsZaiTiNpc) {
				if (RigCom == null) {
					RigCom = NpcObj.AddComponent<Rigidbody>();
				}
				RigCom.isKinematic = false;
				//RigCom.AddForce(NpcTran.forward * 5f, ForceMode.Impulse);
				BuWaWaRigidbody.gameObject.SetActive(true);
				//BuWaWaRigidbody.AddForce(NpcTran.forward * 95f, ForceMode.Impulse);
				switch (pAmmoType) {
				case PlayerAmmoType.DaoDanAmmo:
				case PlayerAmmoType.ChuanTouAmmo:
				case PlayerAmmoType.PaiJiPaoAmmo:
				case PlayerAmmoType.GenZongAmmo:
				case PlayerAmmoType.JianSuAmmo:
				case PlayerAmmoType.SanDanAmmo:
					float powerVal = XKPlayerGlobalDt.GetInstance().DaoDanPowerNpc;
					RigCom.AddForce(-NpcTran.forward * powerVal, ForceMode.Impulse);
					BuWaWaRigidbody.AddForce(-NpcTran.forward * powerVal, ForceMode.Impulse);
					break;
				default:
					RigCom.AddForce(NpcTran.forward * 5f, ForceMode.Impulse);
					BuWaWaRigidbody.AddForce(NpcTran.forward * 95f, ForceMode.Impulse);
					break;
				}
				DestroyNpc();
			}
			else if (IsZaiTiNpc) {
				if (RigCom == null) {
					RigCom = NpcObj.AddComponent<Rigidbody>();
				}
				RigCom.isKinematic = false;
				RigCom.AddForce(NpcTran.forward * 5f, ForceMode.Impulse);

				XkNpcZaiTiCtrl zaiTiScript = null;
				int max = ZaiTiNpcAni.Length;
				for (int i = 0; i < max; i++) {
					if (ZaiTiNpcAni[i] == null) {
						continue;
					}

					zaiTiScript = ZaiTiNpcAni[i].GetComponent<XkNpcZaiTiCtrl>();
					if (ZaiTiNpcAni[i] != null && !zaiTiScript.GetIsDeathNPC()) {
						//ZaiTiNpcAni[i].transform.parent = XkGameCtrl.NpcObjArray;
//						if (ZaiTiNpcAni[i].gameObject.GetComponent<Rigidbody>() == null) {
//							ZaiTiNpcAni[i].gameObject.AddComponent<Rigidbody>();
//						}
						ZaiTiNpcAni[i].speed = 0f;
						ZaiTiNpcAni[i].enabled = false;
						//zaiTiScript.SetIsDeathNPC();
						zaiTiScript.SetIsZaiNpc();
						zaiTiScript.RemoveNpcObj();
						//StartCoroutine(DestroyNetNpcObj(ZaiTiNpcAni[i].gameObject, 2f));
					}
				}

				max = ZaiTiNpcBuWaWa.Length;
				for (int i = 0; i < max; i++) {
					if (ZaiTiNpcBuWaWa[i] != null) {
						ZaiTiNpcBuWaWa[i].gameObject.SetActive(true);
						ZaiTiNpcBuWaWa[i].AddForce(NpcTran.forward * 95f, ForceMode.Impulse);
					}
				}
				DestroyNpc();
			}
			else {
				StartCoroutine(DestroyNetNpcObj(NpcObj));
			}

			if (IsFangZhenNpc) {
				NpcFangZhenScript.SubFangZhenNpcCount();
			}

//			if (XKTriggerHuoJianDan.IsActivePanXuanPath) {
//				XKTriggerHuoJianDan.GetInstance().AddKillNpcNum();
//			}
		}
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(NpcTran);


        if (m_PathNodeList != null)
        {
            //清理路径坐标信息.
            m_PathNodeList.Clear();
        }
    }

	bool IsRemoveNpcObj;
	public void SetIsRemoveNpcObj()
	{
		//Debug.Log("Unity:"+"SetIsRemoveNpcObj -> npcName "+NpcObj.name);
		IsRemoveNpcObj = true;
	}

	IEnumerator DestroyNetNpcObj(GameObject obj, float timeVal = 0f)
	{
		if (IsMoveByCar) {
			AiCarCom.SetIsStopMoveCar(true);
            AiCarCom.SetActiveHiddenAiCarObj(false);
        }

		if (timeVal > 0f) {
			yield return new WaitForSeconds(timeVal);
		}

		bool isHiddenNpcTest = true;
		if (IsRemoveNpcObj) {
			isHiddenNpcTest = false;
		}

        if (NpcAniScript != null && NpcAniScript.gameObject != null)
        {
            NpcAniScript.gameObject.SetActive(false);
        }

		if (!isHiddenNpcTest) {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(NpcObj);
				Destroy(obj);
			}
			else {
				if (Network.peerType == NetworkPeerType.Server) {
					XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(NpcObj);
					if (NetworkServerNet.GetInstance() != null) {
						NetworkServerNet.GetInstance().RemoveNetworkObj(obj);
					}
				}
			}
		}
		else {
			ResetNpcInfo();
		}
        
        if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null)
        {
            //检测是否有战车、JPBoss和SuperJPBoss的数据需要清理.
            bool isReturn = false;
            isReturn = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_ZhanCheJPBossData.ZhanCheData.RemoveNpcFromList(obj);
            if (isReturn)
            {
                //战车被删除,重置战车数据信息.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.ResetCreatNpcInfo(SpawnNpcManage.NpcState.ZhanChe);
            }

            isReturn = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_ZhanCheJPBossData.JPBossData.RemoveNpcFromList(obj);
            if (isReturn)
            {
                //JPBoss被删除,重置JPBoss数据信息.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.ResetCreatNpcInfo(SpawnNpcManage.NpcState.JPBoss);
            }

            isReturn = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_ZhanCheJPBossData.SuperJPBossData.RemoveNpcFromList(obj);
            if (isReturn)
            {
                //SuperJPBoss被删除,重置SuperJPBoss数据信息.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.ResetCreatNpcInfo(SpawnNpcManage.NpcState.SuperJPBoss);
            }
        }
    }

	public void MoveFZNpcToFirePoint(Vector3 firePos, float mvSpeed)
    {
        //Debug.Log("**************************move 2222222222222222222");
        if (IsDeathNPC)
        {
		    return;
		}

		IsChangeNpcForward = false;
		Vector3[] posArray = new Vector3[2];
		posArray[0] = NpcTran.position;
		posArray[1] = firePos;
		
		iTween.MoveTo(NpcObj, iTween.Hash("path", posArray,
		                                  "speed", mvSpeed,
		                                  "orienttopath", true,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "MoveFZNpcOnCompelteITween"));
	}

	void MoveFZNpcOnCompelteITween()
	{
		MakeNpcDoFireAnimation();
	}

	public bool GetIsAimPlayer()
	{
		return IsAimPlayer;
	}

	public XKSpawnNpcPoint GetSpawnPointScript()
	{
		return SpawnPointScript;
	}

	public void SetSpawnPointScript(XKSpawnNpcPoint script)
	{
		SpawnPointScript = script;
	}
	
	void MakeLandNpcMoveToLand()
	{
		if (IsMoveByCar) {
			return;
		}

		if (NpcMoveType == NpcJiFenEnum.FeiJi) {
			return;
		}

		RaycastHit hitInfo;
		Vector3 startPos = RealNpcTran.position + Vector3.up * 5f;
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 35f, XkGameCtrl.GetInstance().LandLayer);
		if (hitInfo.collider != null){
			RealNpcTran.position = hitInfo.point;
		}
	}

	public void SetFireDistance(float val)
	{
		//Debug.Log("Unity:"+"valDis "+val);
		FireDistance = val;
		IsDoFireAnimation = true;
	}

	public void SetHuoCheNpcInfo(int indexPoint)
	{
	}

	IEnumerator DelaySetHuoCheNpcInfo(int indexPoint)
	{
		yield return new WaitForSeconds(0.5f);
//		Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo -> indexPoint "+indexPoint+", name "+transform.name);
		if (Network.peerType == NetworkPeerType.Server) {
			yield break;
		}
		
		XKHuoCheCtrl huoCheScript = XKHuoCheCtrl.Instance;
		if (huoCheScript == null) {
			yield break;
		}
		
		XKSpawnNpcPoint[] pointScript = huoCheScript.gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
//		Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo -> pointScript.Len "+pointScript.Length);
		if (pointScript.Length > 0) {
			for (int i = 0; i < pointScript.Length; i++) {
				if (i == indexPoint) {
					IsHuoCheNpc = true;
//					Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo... IsHuoCheNpc "+IsHuoCheNpc+", parentName "
//					          +pointScript[i].transform.parent.name);
					CancelInvoke("DelayChangeNpcParent");
					transform.parent = pointScript[i].transform.parent;
					transform.localPosition = pointScript[i].transform.localPosition;
					transform.localEulerAngles = pointScript[i].transform.localEulerAngles;
					yield break;
				}
			}
		}
	}
	
	IEnumerator DelaySetFeiJiNpcInfo(int indexPoint)
	{
		yield return new WaitForSeconds(0.5f);
//		Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo -> indexPoint "+indexPoint+", name "+transform.name);
		if (Network.peerType == NetworkPeerType.Server) {
			yield break;
		}
		
		XKHuoCheCtrl huoCheScript = XKHuoCheCtrl.Instance;
		if (huoCheScript == null) {
			yield break;
		}
		
		XKSpawnNpcPoint[] pointScript = huoCheScript.gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
//		Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo -> pointScript.Len "+pointScript.Length);
		if (pointScript.Length > 0) {
			for (int i = 0; i < pointScript.Length; i++) {
				if (i == indexPoint) {
					IsHuoCheNpc = true;
//					Debug.Log("Unity:"+"XKNpcSendSetHuoCheNpcInfo... IsHuoCheNpc "+IsHuoCheNpc+", parentName "
//					          +pointScript[i].transform.parent.name);
					CancelInvoke("DelayChangeNpcParent");
					transform.parent = pointScript[i].transform.parent;
					transform.localPosition = pointScript[i].transform.localPosition;
					transform.localEulerAngles = pointScript[i].transform.localEulerAngles;
					yield break;
				}
			}
		}
	}

	void CheckNpcIsRemove()
	{
		if (Time.realtimeSinceStartup - SpawnTimeVal < 180f) {
			return;
		}
		
		if (Time.frameCount % 100 != 0) {
			return;
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			return;
		}

        if (IsCaiPiaoZhanChe == true)
        {
            //彩票战车或boss不用进行检测.
            //有镜头触发器来控制它们的删除逻辑.
            return;
        }
		
		XkPlayerCtrl scriptFJ = XkPlayerCtrl.GetInstanceFeiJi();
		XkPlayerCtrl scriptTK = XkPlayerCtrl.GetInstanceTanKe();
		if (scriptFJ == null && scriptTK == null) {
			return;
		}
		
		float disFJ = 0f;
		float disTK = 0f;
		float minDis = 400f;
		Vector3 posA = Vector3.zero;
		Vector3 posB = Vector3.zero;
		Vector3 posC = Vector3.zero;
		if (scriptFJ != null && scriptTK != null) {
			posA = NpcTran.position;
			posB = scriptFJ.transform.position;
			posC = scriptTK.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disFJ = Vector3.Distance(posB, posA);
			disTK = Vector3.Distance(posC, posA);
			if (disFJ > minDis && disTK > minDis) {
				//Debug.Log("Unity:"+"npcMoveTest*******************111");
				TriggerRemovePointNpc(0);
			}
			return;
		}
		
		if (scriptFJ != null) {
			posA = NpcTran.position;
			posB = scriptFJ.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disFJ = Vector3.Distance(posB, posA);
			if (disFJ > minDis) {
				//Debug.Log("Unity:"+"npcMoveTest*******************222");
				TriggerRemovePointNpc(0);
			}
			return;
		}
		
		if (scriptTK != null) {
			posA = NpcTran.position;
			posC = scriptTK.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disTK = Vector3.Distance(posC, posA);
			if (disTK > minDis) {
				//Debug.Log("Unity:"+"npcMoveTest*******************333");
				TriggerRemovePointNpc(0);
			}
			return;
		}
	}

	/// <summary>
	/// Resets the npc info.
	/// </summary>
	void ResetNpcInfo()
	{
        IsLeaveCamera = false;
        IsInitNpcInfo = false;
		IsMoveToMarkPoint = false;
		
		IsMoveFirePoint = false;
//		IsIntoJingJieState = false;
//		IsDelayFireAction = false;
		
		IsAimPlayerByFire = false;
		IsHuoCheNpc = false;
		IsWuDi = false;
		
//		IsDoFireAniZaiTi = false;
		IsFangZhenNpc = false;
		IsAimPlayer = false;

		IsDoFireAnimation = false;
		IsChangeNpcForward = false;
		//IsDeathNPC = false;

		IndexFirePointGroup = 0;
		MarkCount = 0;

		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			itweenScript.enabled = false;
			DestroyObject(itweenScript);
			//Debug.Log("*************************3333 name  " + gameObject.name);
		}

		XkNpcZaiTiCtrl zaiTiScript = GetComponentInChildren<XkNpcZaiTiCtrl>();
		if (zaiTiScript != null && zaiTiScript.ZaiTiNpcBuWaWa != null) {
			zaiTiScript.ResetNpcZaiTiSomeInfo();
		}

		RealNpcTran.gameObject.SetActive(false);
		RealNpcTran.localPosition = Vector3.zero;
		RealNpcTran.localEulerAngles = Vector3.zero;
		if (BuWaWaRigidbody != null) {
			Transform buWaWaTran = BuWaWaRigidbody.transform;
			buWaWaTran.localPosition = Vector3.zero;
			buWaWaTran.localEulerAngles = Vector3.zero;
		}

		if (RigCom != null && !RigCom.isKinematic && !IsMoveByCar) {
			Destroy(RigCom);
		}

		Rigidbody rigCom = RealNpcTran.GetComponent<Rigidbody>();
		if (rigCom != null && !rigCom.isKinematic) {
			Destroy(rigCom);
		}
		NpcTran.position = new Vector3(-18000f, -18000f, 0f);
        NpcTran.SetParent(XkGameCtrl.GetInstance().NpcObjHiddenArray);
	}
    
	XKAiCarMoveCtrl AiCarCom;
	void HandleNpcDeathInfo()
	{
		//if (IsZaiTiNpc || IsAniMove) {
		if (IsZaiTiNpc) {
			return;
		}

		if (NpcAniScript != null) {
			BoxCollider boxCol = NpcAniScript.GetComponent<BoxCollider>();
			if (boxCol == null || !boxCol.enabled) {
				return;
			}
			boxCol.enabled = false;
			NpcAniScript.ResetNpcAnimation();
		}
	}

	void GetAimPlayerMoveScript()
	{
		GameObject playerObj = XkGameCtrl.GetInstance().GetRandAimPlayerObj();
		PlayerMoveScript = playerObj == null ? null : playerObj.GetComponent<XKPlayerMoveCtrl>();
		//Debug.Log("Unity:"+"GetAimPlayerMoveScript -> player "+PlayerMoveScript.name);
		
		if (IsInvoking("ResetPlayerMoveScript")) {
			CancelInvoke("ResetPlayerMoveScript");
		}
		Invoke("ResetPlayerMoveScript", Random.Range(3f, 8f));
	}
	
	void ResetPlayerMoveScript()
	{
		PlayerMoveScript = null;
	}

	XKNpcCheLunCtrl[] NpcCheLunArray;
	void SetNpcCheLunIsRun(bool isRun)
	{
		if (NpcCheLunArray == null) {
			NpcCheLunArray = GetComponentsInChildren<XKNpcCheLunCtrl>();
		}

		foreach (var item in NpcCheLunArray) {
			item.SetCheLunIsRun(isRun);
		}
	}

    /// <summary>
    /// 是否是boss.
    /// </summary>
	bool IsBossNpc;
	public void SetIsBossNpc(bool isBoss)
	{
		IsBossNpc = isBoss;
        if (isBoss)
        {
            IsZhanCheNpc = false;
            //彩票JPBoss.
            IsJPBossNpc = true;
            m_DaiJinQuanState = SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan;

            if (m_CaiPiaoBossMoveCom == null)
            {
                //添加彩票Boss特殊移动脚本.
                m_CaiPiaoBossMoveCom = gameObject.AddComponent<SSCaiPiaoBossMove>();
                m_CaiPiaoBossMoveCom.m_NpcMoveCom = this;
            }

            if (m_CaiPiaoBossMoveCom != null)
            {
                //重置彩票Boss的运动信息.
                m_CaiPiaoBossMoveCom.ResetInfo();
            }
        }
	}
	
	public bool GetIsBossNpc()
	{
		return IsBossNpc;
	}

    /// <summary>
    /// 路径坐标信息.
    /// </summary>
    List<Vector3> m_PathNodeList = new List<Vector3>();
    /// <summary>
    /// 组织npc路径坐标信息.
    /// </summary>
    void AddPathNodeData(Transform pathTr)
    {
        if (m_PathNodeList == null)
        {
            m_PathNodeList = new List<Vector3>();
        }

        if (m_PathNodeList.Count > 0)
        {
            m_PathNodeList.Clear();
        }

        if (pathTr != null)
        {
            pathTr.gameObject.SetActive(true);
            for (int i = 0; i < pathTr.childCount; i++)
            {
                m_PathNodeList.Add(pathTr.GetChild(i).position);
            }
            pathTr.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 关闭boss的正常移动.
    /// </summary>
    public void CloseBossMoveing()
    {
        iTween tweenMove = gameObject.GetComponent<iTween>();
        if (tweenMove != null)
        {
            tweenMove.isRunning = false;
            tweenMove.isPaused = true;
            Destroy(tweenMove);
        }
    }

    /// <summary>
    /// 恢复boss的正常运动.
    /// </summary>
    public void RestartMoveingBoss()
    {
        //Debug.Log("**************************move 1111111111111111");
        if (IsDeathNPC)
        {
            return;
        }

        if (NpcPathTran == null)
        {
            //Debug.Log("Unity:"+"The npc has no path! name "+gameObject.name);
            return;
        }
        
        Vector3[] nodesArray = new Vector3[2];
        nodesArray[0] = NpcTran.position;
        if (MarkCount >= NpcPathTran.childCount || MarkCount < 0)
        {
            MarkCount = 0; //fixed MarkCount
        }
        nodesArray[1] = m_PathNodeList[MarkCount];
        
        MarkNpcMovePos = nodesArray[1];
        IsMoveToMarkPoint = false;

        bool isOrienttopath = true;
        if (IsCaiPiaoZhanChe)
        {
            isOrienttopath = false;
            MarkTranAimForward = nodesArray[1] - nodesArray[0];
            MarkTranAimForward.y = 0f;
            MarkTranAimForward = MarkTranAimForward.normalized;
            //Debug.Log("********************************************* boss *************** isOrienttopath == " + isOrienttopath);
        }

        //Debug.Log("********************************* name " + gameObject.name);
        iTween.MoveTo(NpcObj, iTween.Hash("path", nodesArray,
                                          "speed", MvSpeed,
                                          "orienttopath", isOrienttopath,
                                          "easeType", iTween.EaseType.linear));
    }

    /// <summary>
    /// 是否进入摄像机盒子.
    /// </summary>
    internal bool IsEnterCameraBox = false;
    public void SetIsEnterCameraBox()
    {
        StartCoroutine(DelaySetIsEnterCameraBox());
    }

    IEnumerator DelaySetIsEnterCameraBox()
    {
        yield return new WaitForSeconds(3f);
        IsEnterCameraBox = true;
    }

    internal void SetJPBossHealthInfo()
    {
        if (NpcAniScript != null)
        {
            XKNpcHealthCtrl healthScriptTmp = NpcAniScript.GetComponent<XKNpcHealthCtrl>();
            if (healthScriptTmp != null)
            {
                healthScriptTmp.SetJPBossHealthInfo(this);
            }
        }
    }
}

public enum NpcType
{
	WaterNpc,
	LandNpc,
	FlyNpc,
}