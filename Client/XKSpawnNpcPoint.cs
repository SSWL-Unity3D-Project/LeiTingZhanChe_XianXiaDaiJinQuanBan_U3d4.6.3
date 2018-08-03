using UnityEngine;
using System.Collections.Generic;

public enum NpcSpawnType
{
	PlayerNum_1,
	PlayerNum_2,
	PlayerNum_3,
	PlayerNum_4,
}

public class XKSpawnNpcPoint : MonoBehaviour
{
	public NpcSpawnType PlayerNumSt = NpcSpawnType.PlayerNum_1; //激活几个主角时产生npc.
//	public SpawnPointType PointType = SpawnPointType.KongZhong;
	/// <summary>
	/// 延迟一定时间产生npc.
	/// </summary>
	[Range(0f, 100f)]public float DelayTimeVal = 0f;
//	public bool IsAimFeiJiPlayer;
//	public Transform HuoCheNpcTran;
	public GameObject NpcObj;
	public Transform NpcPath;
	public FirePointCtrl FirePointNpc;
//	public FirePointCtrl[] FirePointGroup;
//	public bool IsLoopFirePoint;
	bool IsHuoCheNpc;
	public bool IsAimPlayer; //fire时,是否瞄准主角.
	public bool IsFireMove; //是否边移动边开火.
	[Range(0.1f, 100f)] public float MvSpeed = 1f;
	[Range(-1f, 1000f)] public float FireDistance = 1f;
	[Range(0f, 100f)] public float TimeFire = 1f; //npc开火持续时间.
	[Range(0f, 100f)] public float TimeRun = 1f; //npc开火后奔跑时间.
	public AnimatorNameNPC AniRunName = AnimatorNameNPC.Run1; //Run animation.
	public AnimatorNameNPC AniRootName = AnimatorNameNPC.Root1;
	[Range(0f, 30f)] public float TimeRootAni = 0f;
	[Range(1f, 100f)] public float LoopSpawnTime = 1f; //miao
	public GameObject[] NpcSpawnLoop; //循环产生npc的数组.
	[Range(1, 30)] public int SpawnMaxNpc = 1;
//	public bool IsDoublePlayer;
	public Transform[] ChildSpawnPoint; //用于方阵npc的产生.
	public GameObject NpcFangZhen; //用于方阵npc的攻击点逻辑.
	public AnimatorNameNPC AniFangZhenFireRun; //方阵npc进入攻击点时的run动画.
	[Range(0.1f, 100f)] public float SpeedFangZhenFireRun; //方阵npc进入攻击点时移动的速度.
//	[Range(0f, 30f)] public float TimeMinFire = 0.1f;
//	[Range(0f, 30f)] public float TimeMaxFire = 2f;
	//public RuntimeAnimatorController AniController; //FlyNpc动画控制运动的Controller.
	public LayerMask TerrainLayer;
	public bool IsMakeObjToTerrain;
	public List<XKTriggerSpawnNpc> TestTriggerSpawnNpc;
	public List<XKTriggerRemoveNpc> TestTriggerRemoveNpc;
	int SpawnNpcCount;
	XKNpcMoveCtrl[] NpcScript;
	XKNpcFangZhenCtrl NpcFangZhenScript;
	bool IsRemoveSpawnPointNpc;
	//XKHuoCheCtrl HuoCheScript;
	XKCannonCtrl CannonScript;
	XKDaPaoCtrl DaPaoScript;
//	bool IsSpawnPointNpc;
	GameObject NpcLoopObj;
	bool IsRemoveTrigger;
	bool IsSpawnTrigger;
	bool IsPlayerAimTrigger;
	bool IsPlayerLeaveTrigger;
	public List<XKSpawnNpcPointCheck> SpawnPointCheckList;
	public bool AddTriggerSpawnScript(XKTriggerSpawnNpc script)
	{
		if (SpawnPointCheckList == null) {
			SpawnPointCheckList = new List<XKSpawnNpcPointCheck>();
			for (int i = 0; i < 10; i++) {
				SpawnPointCheckList.Add(new XKSpawnNpcPointCheck());
			}
		}

		bool isWrong = false;
		for (int i = 0; i < SpawnPointCheckList.Count; i++) {
			if (SpawnPointCheckList[i] != null) {
				if (SpawnPointCheckList[i].TriggerSpawnScript == null) {
					SpawnPointCheckList[i].TriggerSpawnScript = script;
					SpawnPointCheckList[i].Count++;
					if (SpawnPointCheckList[i].Count > 1) {
						isWrong = true;
					}
					break;
				}
				else {
					if (SpawnPointCheckList[i].TriggerSpawnScript == script) {
						SpawnPointCheckList[i].Count++;
						if (SpawnPointCheckList[i].Count > 1) {
							isWrong = true;
						}
						break;
					}
				}
			}
		}
		return isWrong;
	}

	public void ClearSpawnPointCheckList()
	{
		SpawnPointCheckList.Clear();
		SpawnPointCheckList = null;
	}

	// Use this for initialization
	void Awake()
	{
        SSCreatNpcData npcDataCom = gameObject.GetComponent<SSCreatNpcData>();
        if (NpcObj == null && npcDataCom == null)
        {
			if (!XkGameCtrl.GetInstance().IsCartoonShootTest)
            {
				Debug.LogWarning("Unity:"+"NpcObj was null");
				GameObject obj = null;
				obj.name = "null";
			}
			return;
		}

        if (NpcObj != null)
        {
            XKNpcMoveCtrl npcScript = NpcObj.GetComponent<XKNpcMoveCtrl>();
            if (npcScript != null)
            {
                if (IsFireMove && IsAimPlayer)
                {
                    Debug.LogWarning("Unity:" + "SpawnPoint.IsFireMove and SpawnPoint.IsAimPlayer is true!");
                    GameObject obj = null;
                    obj.name = "null";
                }
            }
        }
        
		if (NpcPath != null) {
		    if (NpcPath.childCount < 1) {
				Debug.LogWarning("Unity:"+"NpcPath.childCount was wrong! childCount = "+NpcPath.childCount);
				GameObject obj = null;
				obj.name = "null";
			}

			if (NpcPath.GetComponent<NpcPathCtrl>() == null) {
				Debug.LogWarning("Unity:"+"NpcPath was wrong! cannot find NpcPathCtrl script");
				GameObject obj = null;
				obj.name = "null";
			}
		}

		if (NpcSpawnLoop.Length > 0) {
			for (int i = 0; i < NpcSpawnLoop.Length; i++) {
				if (NpcSpawnLoop[i] == null) {
					Debug.LogWarning("Unity:"+"NpcSpawnLoop was wrong! index "+(i+1));
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}
		}

		if (ChildSpawnPoint.Length > 0) {
			for (int i = 0; i < ChildSpawnPoint.Length; i++) {
				if (ChildSpawnPoint[i] == null) {
					Debug.LogWarning("Unity:"+"ChildSpawnPoint was wrong! index "+(i+1));
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}
		}

		if (ChildSpawnPoint.Length > 0 && NpcFangZhen == null) {
			Debug.LogWarning("Unity:"+"NpcFangZhen is null! fangZhenLength "+ChildSpawnPoint.Length);
			GameObject obj = null;
			obj.name = "null";
		}
        //Invoke("CheckIsRemoveTrigger", 1f);

        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Destroy(mesh);
        }

        MeshFilter meshFt = gameObject.GetComponent<MeshFilter>();
        if (meshFt != null)
        {
            Destroy(meshFt);
        }
    }

	int IndexFeiJiPoint;
	public int GetIndexFeiJiPoint()
	{
		return IndexFeiJiPoint;
	}

	public static void HandleFeiJiNpcSpawnInfo(XKNpcMoveCtrl npcScript, int indexVal)
	{
		if (FiJiNpcPointList == null) {
			return;
		}

		if (indexVal >= FiJiNpcPointList.Count) {
			return;
		}

		//Debug.Log("Unity:"+"HandleFeiJiNpcSpawnInfo -> indexVal "+indexVal);
		XKSpawnNpcPoint spawnScript = FiJiNpcPointList[indexVal];
		spawnScript.SaveFeiJiNpcSpawnInfo(npcScript);
	}

	public void SaveFeiJiNpcSpawnInfo(XKNpcMoveCtrl npcScript)
	{
		NpcLoopObj = npcScript.gameObject;
		npcScript.TestSpawnPoint = gameObject;
	}

	public static List<XKSpawnNpcPoint> FiJiNpcPointList;
	void SetFeiJiSpawnPointInfo()
	{
		if (FiJiNpcPointList == null) {
			FiJiNpcPointList = new List<XKSpawnNpcPoint>();
		}

		if (FiJiNpcPointList.Contains(this)) {
			return;
		}
		IndexFeiJiPoint = FiJiNpcPointList.Count;
		FiJiNpcPointList.Add(this);
	}

	public static void ClearFiJiNpcPointList()
	{
		if (FiJiNpcPointList == null) {
			return;
		}
		FiJiNpcPointList.Clear();
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		CheckTestTriggerSpawnNpc();
		CheckTestTriggerRemoveNpc();

		/*if (NpcObj != null) {
			XKNpcMoveCtrl npcScript = NpcObj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null && npcScript.NpcState == NpcType.FlyNpc && npcScript.NpcJiFen != NpcJiFenEnum.FeiJi) {
				npcScript.NpcJiFen = NpcJiFenEnum.FeiJi;
			}
		}*/

		if ((int)AniFangZhenFireRun < (int)AnimatorNameNPC.Run1
		    || (int)AniFangZhenFireRun > (int)AnimatorNameNPC.Run2) {
			AniFangZhenFireRun = AnimatorNameNPC.Run1;
		}

		if ((int)AniRootName != (int)AnimatorNameNPC.Root1) {
			AniRootName = AnimatorNameNPC.Root1;
		}

		if (FireDistance > 0f) {
			Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.3f);
			Gizmos.DrawSphere(transform.position, FireDistance);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, FireDistance);
		}

		if (NpcPath != null && NpcPath.childCount > 0) {
			Transform[] tranArray =  new Transform[2];
			tranArray[0] = transform;
			tranArray[1] = NpcPath.GetChild(0);
			iTween.DrawPath(tranArray, Color.blue);
		}
		MakeObjMoveToLand();
	}
#endif

    /// <summary>
    /// 创建战车、JPBoss和SuperJPBoss.
    /// </summary>
    public GameObject CreatPointNpc()
    {
        if (NpcObj == null)
        {
            return null;
        }

        //if (NpcObj.name != "boss1")
        //{
        //    return null;
        //}

        GameObject obj = null;
        IsRemoveSpawnPointNpc = false;
        InitSpawnPointInfo();
        StartSpawnNpc();
        obj = NpcLoopObj;
        return obj;
    }

	public void SpawnPointAllNpc()
	{
        //if ("TestNpcFireRunPoint" != gameObject.name)
        //{
        //    //test.
        //    return;
        //}
        //return; //test.
		if (XkGameCtrl.GetInstance().IsCartoonShootTest && NpcObj == null) {
			return;
		}
		
		if (!enabled || !gameObject.activeSelf) {
			return;
		}

        //test
        //XKNpcMoveCtrl npc = NpcObj.GetComponent<XKNpcMoveCtrl>();
        //if (npc != null)
        //{
        //    if (npc.NpcMoveType == NpcJiFenEnum.ShiBing)
        //    {
        //        return;
        //    }
        //}
        //test


//		if (IsSpawnPointNpc) {
//			return;
//		}
//		IsSpawnPointNpc = true;
		IsRemoveSpawnPointNpc = false;
		InitSpawnPointInfo();
		Invoke("StartSpawnNpc", DelayTimeVal);
	}

	//获取当前产生点产生的Npc.
	public GameObject GetNpcLoopObj()
	{
		return NpcLoopObj;
	}

	void StartSpawnNpc()
	{
		if (!enabled || !gameObject.activeSelf) {
			return;
		}

		if (ScreenDanHeiCtrl.IsStartGame) {
//			if ((XkGameCtrl.GameModeVal == GameMode.DanJiFeiJi && PointType == SpawnPointType.DiMian)
//			    || (XkGameCtrl.GameModeVal == GameMode.DanJiTanKe && PointType == SpawnPointType.KongZhong)) {
//				return;
//			}
			
//			if (IsDoublePlayer && (!XkGameCtrl.IsActivePlayerOne || !XkGameCtrl.IsActivePlayerTwo)) {
//				return;
//			}

			switch (PlayerNumSt) {
			case NpcSpawnType.PlayerNum_2:
				if (XkGameCtrl.PlayerActiveNum < 2) {
					return;
				}
				break;
				
			case NpcSpawnType.PlayerNum_3:
				if (XkGameCtrl.PlayerActiveNum < 3) {
					return;
				}
				break;
				
			case NpcSpawnType.PlayerNum_4:
				if (XkGameCtrl.PlayerActiveNum < 4) {
					return;
				}
				break;
			}
		}
		
		GameObject obj = null;
		//XKHuoCheCtrl hcScript = NpcObj.GetComponent<XKHuoCheCtrl>();
		//if (hcScript != null) {
		//	obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
		//	if (obj == null) {
		//		//Debug.Log("Unity:"+"StartSpawnNpc -> Cannot spawn HuoCheNpc!");
		//		return;
		//	}

		//	obj.transform.parent = XkGameCtrl.NpcObjArray;
		//	HuoCheScript = obj.GetComponent<XKHuoCheCtrl>();
		//	HuoCheScript.SetHuoCheInfo(this);
		//	//HuoCheScript.StartMoveHuoChe(NpcPath);
		//	NpcLoopObj = obj;
		//	return;
		//}

		DaPaoScript = NpcObj.GetComponent<XKDaPaoCtrl>();
		if (DaPaoScript != null) {
//			Debug.Log("Unity:"+"Spawn Cannon... ");
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			if (obj == null) {
				//Debug.Log("Unity:"+"StartSpawnNpc -> Cannot spawn DaPaoNpc!");
				return;
			}

			if (!IsHuoCheNpc) {
				obj.transform.parent = XkGameCtrl.NpcObjArray;
			}
			else {
				obj.transform.parent = transform.parent;
			}

			//XkGameCtrl.GetInstance().AddNpcTranToList(obj.transform);
			DaPaoScript = obj.GetComponent<XKDaPaoCtrl>();
			DaPaoScript.SetSpawnPointScript(this);
			NpcLoopObj = obj;
			return;
		}

		if (IsRemoveSpawnPointNpc) {
			return;
		}

		//Debug.Log("Unity:"+"SpawnPointAllNpc...NpcObj is "+NpcObj.name+", SpawnNpcCount "+SpawnNpcCount);
		if (ChildSpawnPoint.Length > 0) {
			//spawn fangZhenNpc
			GameObject fangZhenObj = SpawnPointNpc(NpcFangZhen, transform.position, transform.rotation);
			if (fangZhenObj == null) {
				//Debug.LogError("Unity:"+"StartSpawnNpc -> Cannot spawn FangZhenNpc! NpcFangZhen "+NpcFangZhen.name);
				//fangZhenObj.name = "null";
				return;
			}
			NpcFangZhenScript = fangZhenObj.GetComponent<XKNpcFangZhenCtrl>();
			//NpcFangZhenScript.ActiveFangZhenNpc();
			
			XKNpcMoveCtrl npcScript = null;
			Transform fangZhenTran = fangZhenObj.transform;
			fangZhenTran.parent = XkGameCtrl.NpcObjArray;
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			if (obj == null) {
				//Debug.Log("Unity:"+"StartSpawnNpc -> Cannot spawn FangZhenChildNpc --- 1");
				return;
			}

			npcScript = obj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null) {
				npcScript.ActiveIsFangZhenNpc();
				npcScript.SetNpcSpawnScriptInfo(this);
			}
			obj.transform.parent = fangZhenTran;

			for (int i = 0; i < ChildSpawnPoint.Length; i++) {
				obj = SpawnPointNpc(NpcObj, ChildSpawnPoint[i].position, ChildSpawnPoint[i].rotation);
				if (obj == null) {
					//Debug.LogWarning("Unity:"+"StartSpawnNpc -> Cannot spawn FangZhenChildNpc --- index "+i);
					break;
				}

				npcScript = obj.GetComponent<XKNpcMoveCtrl>();
				if (npcScript != null) {
					npcScript.ActiveIsFangZhenNpc();
					npcScript.SetNpcSpawnScriptInfo(this);
				}
				obj.transform.parent = fangZhenTran;
			}

			NpcFangZhenScript.SetSpawnNpcInfo(this);
			NpcLoopObj = fangZhenObj;
			return;
		}

//		if (NpcLoopObj != null && SpawnMaxNpc > 1) {
//			if (IsRemoveSpawnPointNpc) {
//				return;
//			}
//
//			XKNpcMoveCtrl npcScript = NpcLoopObj.GetComponent<XKNpcMoveCtrl>();
//			if (npcScript != null && !npcScript.GetIsDeathNPC()) {
//				Invoke("StartSpawnNpc", LoopSpawnTime);
//				return;
//			}
//		}

		if (SpawnNpcCount > 0) {
			int maxVal = NpcSpawnLoop.Length;
			int randVal = Random.Range(0, (maxVal+1));
			if (randVal == 0) {
				obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			}
			else {
				randVal = randVal > maxVal ? maxVal : randVal;
				obj = SpawnPointNpc(NpcSpawnLoop[randVal - 1], transform.position, transform.rotation);
			}
		}
		else {
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
		}

		if (obj == null) {
			//Debug.Log("Unity:"+"StartSpawnNpc -> Cannot spawn PuTongNpc!");
			return;
		}

		if (!IsHuoCheNpc) {
			obj.transform.parent = XkGameCtrl.NpcObjArray;
		}
		else {
			obj.transform.parent = transform.parent;
		}

		NpcScript[SpawnNpcCount] = obj.GetComponent<XKNpcMoveCtrl>();
		if (NpcScript[SpawnNpcCount] != null) {
			NpcScript[SpawnNpcCount].SetSpawnNpcInfo(this);
		}
		NpcLoopObj = obj;

		SpawnNpcCount++;
		if (SpawnNpcCount >= SpawnMaxNpc || SpawnMaxNpc <= 1) {
			return;
		}
		Invoke("StartSpawnNpc", LoopSpawnTime);
	}

	public void SetIsHuoCheNpc()
	{
		IsHuoCheNpc = true;
	}

	public bool GetIsHuoCheNpc()
	{
		return IsHuoCheNpc;
	}

	GameObject SpawnPointNpc(GameObject objPrefab, Vector3 pos, Quaternion rot)
	{
		GameObject obj = XKNpcSpawnListCtrl.GetInstance().GetNpcObjFromNpcDtList(this, objPrefab, pos, rot);
//		Debug.Log("Unity:"+"SpawnPointNpc -> objPrefab "+objPrefab.name);
//		if (obj == null) {
//			Debug.Log("Unity:"+"SpawnPointNpc -> obj is null");
//		}
//		else {
//			Debug.Log("Unity:"+"SpawnPointNpc -> obj is "+obj.name);
//		}

//		if (Network.peerType == NetworkPeerType.Disconnected) {
//			obj = (GameObject)Instantiate(objPrefab, pos, rot);
//		}
//		else {
//			int playerID = int.Parse(Network.player.ToString());
//			obj = (GameObject)Network.Instantiate(objPrefab, pos, rot, playerID);
//			if (NetworkServerNet.GetInstance() != null) {
//				NetworkServerNet.GetInstance().AddNpcObjList(obj);
//			}
//		}
		return obj;
	}

	public void RemovePointAllNpc()
	{
		if (!XkGameCtrl.GetMissionCleanupIsActive()) {
			return;
		}

		if (NpcObj == null) {
			Debug.LogError("Unity:"+"NpcObj is null");
			return;
		}

//		if (ScreenDanHeiCtrl.IsStartGame) {
//			if (!gameObject.activeSelf) {
//				return;
//			}
//			gameObject.SetActive(false);
//			
//			if (IsRemoveSpawnPointNpc) {
//				return;
//			}
//			IsRemoveSpawnPointNpc = true;
//		}
//		else {
//			//reset spawnPoint info
//			IsSpawnPointNpc = false;
//		}
		
		if (IsRemoveSpawnPointNpc) {
			return;
		}
		IsRemoveSpawnPointNpc = true;
		//reset spawnPoint info
//		IsSpawnPointNpc = false;

		if (IsInvoking("StartSpawnNpc")){
			CancelInvoke("StartSpawnNpc");
		}

		if (NpcFangZhenScript != null) {
			if (NpcFangZhenScript.TestSpawnPoint != gameObject) {
				//Debug.LogWarning("Unity:"+"RemovePointAllNpc -> Cannot remove fangZhenNpc");
				return;
			}
			NpcFangZhenScript.TriggerRemovePointNpc(0);
		}
		else if (DaPaoScript != null) {
			if (DaPaoScript.TestSpawnPoint != gameObject) {
				//Debug.LogWarning("Unity:"+"RemovePointAllNpc -> Cannot remove daPaoNpc");
				return;
			}
			DaPaoScript.OnRemoveCannon(PlayerEnum.Null, 0);
		}
		//else if (HuoCheScript != null) {
		//	HuoCheScript.OnRemoveHuoCheObj();
		//}
		else if (NpcScript != null) {
			int max = NpcScript.Length;
			for (int i = 0; i < max; i++) {
				if(NpcScript[i] != null) {
					if (NpcScript[i].TestSpawnPoint != gameObject) {
						//Debug.LogWarning("Unity:"+"RemovePointAllNpc -> Cannot remove puTongNpc");
						continue;
					}
					NpcScript[i].TriggerRemovePointNpc(0);
				}
			}
		}
	}
	
	void InitSpawnPointInfo()
	{
//		if (NpcScript != null) {
//			return;
//		}

		SpawnNpcCount = 0;
		NpcObj.SetActive(true);
		NpcLoopObj = null;
		NpcScript = new XKNpcMoveCtrl[SpawnMaxNpc];
	}

	public void SetIsRemoveTrigger()
	{
		IsRemoveTrigger = true;
	}

	public void SetIsSpawnTrigger()
	{
		IsSpawnTrigger = true;
	}
	
	void DelayCheckAimLeaveTrigger()
	{
		if (IsPlayerAimTrigger && !IsPlayerLeaveTrigger) {
			//该产生点有瞄准触发器，但是它没有加离开触发器.
			Debug.LogWarning("Unity:"+"The SpawnPoint has not XKTriggerPlayerAimRemove!");
			GameObject obj = null;
			obj.name = "null";
		}
	}

	void CheckIsRemoveTrigger()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		if (XkGameCtrl.IsDonotCheckError) {
			return; //test
		}

		DelayCheckAimLeaveTrigger();
		if (!IsSpawnTrigger) {
			return; //没有使用该产生点.
		}

		if (IsRemoveTrigger && IsSpawnTrigger) {
			return;
		}

		//使用了该产生点，但是没有在删除触发器中调用.
		Debug.LogWarning("Unity:"+"This spawnPoint has no removeTrigger!");
		GameObject obj = null;
		obj.name = "null";
	}

    void Start()
    {
        Invoke("CheckIsSpawnTrigger", 1.5f);
    }
    
    void CheckIsSpawnTrigger()
    {
        if (!IsSpawnTrigger)
        {
            SSCreatNpcData creatNpcData = GetComponent<SSCreatNpcData>();
            if (creatNpcData == null)
            {
                //没有使用该产生点.
                Destroy(gameObject);
            }
        }
    }

	public void SetIsPlayerAimTrigger()
	{
		IsPlayerAimTrigger = true;
	}

	public void SetIsPlayerLeaveTrigger()
	{
		IsPlayerLeaveTrigger = true;
	}

	public void AddTestTriggerSpawnNpc(XKTriggerSpawnNpc script)
	{
		bool isFind = TestTriggerSpawnNpc.Contains(script);
		if (isFind) {
			return;
		}
		TestTriggerSpawnNpc.Add(script);
	}

	void CheckTestTriggerSpawnNpc()
	{
		bool isFind = false;
		int max = TestTriggerSpawnNpc.Count;
		for (int i = 0; i < max; i++) {
			if (TestTriggerSpawnNpc[i] != null) {
				int maxPoint = TestTriggerSpawnNpc[i].SpawnPointArray.Length;
				for (int j = 0; j < maxPoint; j++) {
					if (TestTriggerSpawnNpc[i].SpawnPointArray[j] == this) {
						isFind = true;
					}
				}

				if (!isFind) {
					TestTriggerSpawnNpc.Remove(TestTriggerSpawnNpc[i]);
					break;
				}
				isFind = false;
			}
		}
	}

	public void AddTestTriggerRemoveNpc(XKTriggerRemoveNpc script)
	{
		bool isFind = TestTriggerRemoveNpc.Contains(script);
		if (isFind) {
			return;
		}
		TestTriggerRemoveNpc.Add(script);
	}
	
	void CheckTestTriggerRemoveNpc()
	{
		bool isFind = false;
		int max = TestTriggerRemoveNpc.Count;
		for (int i = 0; i < max; i++) {
			if (TestTriggerRemoveNpc[i] != null) {
				int maxPoint = TestTriggerRemoveNpc[i].SpawnPointArray.Length;
				for (int j = 0; j < maxPoint; j++) {
					if (TestTriggerRemoveNpc[i].SpawnPointArray[j] == this) {
						isFind = true;
					}
				}
				
				if (!isFind) {
					TestTriggerRemoveNpc.Remove(TestTriggerRemoveNpc[i]);
					break;
				}
				isFind = false;
			}
		}
	}
	
	void MakeObjMoveToLand()
	{
		if (!IsMakeObjToTerrain) {
			return;
		}
		
		RaycastHit hitInfo;
		Vector3 startPos = transform.position;
		Vector3 forwardVal = Vector3.down;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, 10f, TerrainLayer.value)){
			transform.position = hitInfo.point;
		}
	}
}

public enum SpawnPointType
{
	Null,
	KongZhong,
	DiMian,
}