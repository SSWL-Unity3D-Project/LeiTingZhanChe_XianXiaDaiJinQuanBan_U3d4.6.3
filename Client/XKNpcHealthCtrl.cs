using UnityEngine;
using System.Collections;

public class XKNpcHealthCtrl : MonoBehaviour {
	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing; //控制主角所击杀npc的积分逻辑.
	[Range(0, 999999)] public int JiFenVal = 1;
	[Range(0f, 10000f)] public float PlayerDamage = 1f;

	/**
	 * MaxPuTongAmmo[0] -> 单人模式下.
	 * MaxPuTongAmmo[1] -> 双人模式下.
	 * MaxPuTongAmmo[2] -> 三人模式下.
	 * MaxPuTongAmmo[3] -> 四人模式下.
	 */
	[Range(1, 100000)] public int[] MaxPuTongAmmo = {1, 1, 1, 1};
//	[Range(0, 100)] public int MaxAmmoHurtLiZi = 0;
	public GameObject[] HiddenNpcObjArray; //npc死亡时需要立刻隐藏的对象.
//	public GameObject HurtLiZiObj; //飞机npc的受伤粒子.
	public GameObject DeathExplode;
	public Transform DeathExplodePoint;
	[Range(0.1f, 100f)] public float YouTongDamageDis = 10f;
	public bool IsYouTongNpc;
	public bool IsAutoRemoveNpc = true;
	public bool IsCanHitNpc = true;
	float MinDisCamera = 15f;
	float TimeLastVal;
	float DisCamera = 150f;
	Transform GameCameraTr;
	public int PuTongAmmoCount;
	public bool IsOpenCameraShake;
    public bool IsDeathNpc;
	XKNpcMoveCtrl NpcScript;
	XKCannonCtrl CannonScript;
	float TimeHitBoss;
	BoxCollider BoxColCom;
	bool IsSpawnObj;
	void Start()
	{
		CheckNpcRigidbody();
		if (NpcJiFen == NpcJiFenEnum.Boss) {
			XKBossXueTiaoCtrl.GetInstance().SetBloodBossAmount(-1f, this);
		}

		gameObject.layer = LayerMask.NameToLayer(XkGameCtrl.NpcLayerInfo);
		BoxColCom = GetComponent<BoxCollider>();
		NpcDamageCom = GetComponent<XKNpcDamageCtrl>();
		if (MaxPuTongAmmo.Length < 4) {
			MaxPuTongAmmo = new int[4];
		}

		Invoke("CheckDisGameCamera", 2f);
		NpcScript = GetComponentInParent<XKNpcMoveCtrl>();
		if (NpcScript != null && NpcJiFen == NpcJiFenEnum.Boss) {
			NpcScript.SetIsBossNpc(true);
		}
    }
    XKPlayerMvFanWei m_FanWeiHou;
    public bool IsHitFanWeiHou = false;
    //public static int TestNum = 0;
    //int TestNumRecord = 0;

    void Update()
	{
        if (m_XKDaPaoCom != null && m_XKDaPaoCom.SpawnPointScript == null)
        {
            if (Time.frameCount % 15 == 0 && !IsDeathNpc)
            {
                if (m_FanWeiHou != null && !IsHitFanWeiHou)
                {
                    Vector3 posTA = m_FanWeiHou.transform.position;
                    Vector3 posTB = transform.position;
                    posTA.y = posTB.y = 0f;
                    Vector3 vecForward = -m_FanWeiHou.transform.forward;
                    Vector3 vecAB = posTB - posTA;
                    vecForward.y = vecAB.y = 0f;
                    if (Vector3.Dot(vecForward, vecAB) < 0f)
                    {
                        float dis = Vector3.Distance(posTA, posTB);
                        if (dis > 15f && dis < 40f)
                        {
                            //Debug.LogError("======== remove test name =============== " + m_XKDaPaoCom.name);
                            IsHitFanWeiHou = true;
                            m_XKDaPaoCom.OnRemoveCannon(PlayerEnum.Null, 0, 1f);
                            return;
                        }
                    }
                }
            }
        }

        if (NpcScript == null && CannonScript != null)
        {
            if (Time.frameCount % 15 == 0 && !IsDeathNpc)
            {
                if (m_FanWeiHou != null && !IsHitFanWeiHou)
                {
                    Vector3 posTA = m_FanWeiHou.transform.position;
                    Vector3 posTB = transform.position;
                    posTA.y = posTB.y = 0f;
                    Vector3 vecForward = -m_FanWeiHou.transform.forward;
                    Vector3 vecAB = posTB - posTA;
                    vecForward.y = vecAB.y = 0f;
                    if (Vector3.Dot(vecForward, vecAB) < 0f)
                    {
                        if (Vector3.Distance(posTA, posTB) > 15f)
                        {
                            //Debug.LogError("remove test name =============== " + CannonScript.DaPaoCtrlScript.name
                            //        + ", TestNumRecord == " + TestNumRecord);
                            IsHitFanWeiHou = true;
                            CannonScript.OnRemoveCannon(PlayerEnum.Null, 1);
                            return;
                        }
                    }
                }
            }
        }

		if (Time.realtimeSinceStartup - TimeLastVal < 10f) {
			return;
		}
		TimeLastVal = Time.realtimeSinceStartup;

		if (!IsSpawnObj) {
			return;
		}

		if (!IsAutoRemoveNpc) {
			return;
		}

		if (IsDeathNpc) {
			return;
		}

		if (GameCameraTr == null) {
			return;
		}

		Vector3 posA = GameCameraTr.position;
		Vector3 posB = DeathExplodePoint.position;
		posA.y = posB.y = 0f;
		if (Vector3.Distance(posA, posB) < DisCamera) {
			return;
		}

		if (DisCamera == MinDisCamera) {
			Vector3 vecA = GameCameraTr.forward;
			Vector3 vecB = Vector3.zero;
			vecB = posA - posB;
			vecA.y = vecB.y = 0f;
			if (Vector3.Dot(vecA, vecB) <= 0f) {
				return;
			}
		}
		MakeNpcHidden();
	}

	void OnCollisionEnter(Collision collision)
	{
        //Debug.Log("Unity:"+"**********OnCollisionEnter-> collision "+collision.gameObject.name);
		XKPlayerMoveCtrl playerScript = collision.gameObject.GetComponent<XKPlayerMoveCtrl>();
		if (playerScript == null) {
			return;
		}

		if (NpcJiFen == NpcJiFenEnum.Boss || !IsCanHitNpc) {
			if (Time.realtimeSinceStartup - TimeHitBoss < 1f) {
				return;
			}
			TimeHitBoss = Time.realtimeSinceStartup;

			Vector3 pushDir = Vector3.zero;
			Vector3 playerPos = playerScript.transform.position;
			Vector3 hitPos = transform.position;
			playerPos.y = hitPos.y = 0f;
			pushDir = playerPos - hitPos;
			playerScript.PushPlayerTanKe(pushDir);
			if (!playerScript.GetIsWuDiState()) {
				XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
			}
			return;
		}

		if (IsDeathNpc) {
			return;
		}

		if (!playerScript.GetIsWuDiState()) {
			XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
		}

		CheckNpcDeathExplode();
		if (!IsYouTongNpc) {
			XkGameCtrl.GetInstance().AddPlayerKillNpc(playerScript.PlayerIndex, NpcJiFen, JiFenVal);
		}

		if (NpcScript != null) {
			IsDeathNpc = true;
			NpcScript.TriggerRemovePointNpc(1);
		}
		else if (CannonScript != null) {
			IsDeathNpc = true;
			CannonScript.OnRemoveCannon(PlayerEnum.Null, 1);
		}
		CheckHiddenNpcObjArray();
	}

	public void CheckHiddenNpcObjArray()
	{
		if (HiddenNpcObjArray.Length <= 0) {
			return;
		}

		int max = HiddenNpcObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (HiddenNpcObjArray[i] != null && HiddenNpcObjArray[i].activeSelf) {
				XKNpcAnimatorCtrl aniScript = HiddenNpcObjArray[i].GetComponent<XKNpcAnimatorCtrl>();
				if (aniScript != null) {
					aniScript.ResetNpcAnimation();
				}
				HiddenNpcObjArray[i].SetActive(false);
			}
        }

        if (m_XKDaPaoCom != null && m_XKDaPaoCom.SpawnPointScript == null)
        {
            m_XKDaPaoCom.OnRemoveCannon(PlayerEnum.Null, 0, 1f);
        }
    }

	public XKNpcMoveCtrl GetNpcMoveScript()
	{
		return NpcScript;
	}

	public void SetNpcMoveScript(XKNpcMoveCtrl script)
	{
		IsSpawnObj = true;
		NpcScript = script;
		if (NpcScript != null && NpcJiFen == NpcJiFenEnum.Boss) {
			NpcScript.SetIsBossNpc(true);
		}
		NpcNameInfo = script.name;
		ResetNpcHealthInfo();
	}

	public bool GetIsDeathNpc()
	{
		return IsDeathNpc;
	}

	void MakeNpcHidden()
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;
		//Debug.Log("Unity:"+"MakeNpcHidden -> name "+gameObject.name);
		
		if (NpcScript != null) {
			if (CannonScript != null) {
				CannonScript.OnRemoveCannon(PlayerEnum.Null, 0);
			}
			NpcScript.TriggerRemovePointNpc(0, CannonScript);
		}
		else if (CannonScript != null) {
			CannonScript.OnRemoveCannon(PlayerEnum.Null, 1);
		}
	}
	
	void CheckDisGameCamera()
    {
        if (m_FanWeiHou == null)
        {
            m_FanWeiHou = XKPlayerMvFanWei.GetInstanceHou();
        }

        if (DeathExplodePoint == null)
        {
			DeathExplodePoint = transform;
		}

		if (XkPlayerCtrl.GetInstanceFeiJi() != null)
        {
			GameCameraTr = XkPlayerCtrl.GetInstanceFeiJi().transform;
		}

		if (GameCameraTr == null)
        {
			Debug.LogWarning("Unity:"+"CheckDisGameCamera -> GameCameraTr is null");
			return;
		}

		Vector3 vecA = GameCameraTr.forward;
		Vector3 vecB = Vector3.zero;
		Vector3 posA = DeathExplodePoint.position;
		Vector3 posB = GameCameraTr.position;
		posA.y = posB.y = 0f;
		vecB = posA - posB;
		vecA.y = vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) <= 0f)
        {
			return;
		}
		DisCamera = MinDisCamera;
		//Debug.Log("Unity:"+"DisCamera "+DisCamera);
	}

	string NpcNameInfo = "";
	XKNpcDamageCtrl NpcDamageCom;
	int CountActivePlayer;
	public void OnDamageNpc(int damageNpcVal = 1, PlayerEnum playerSt = PlayerEnum.Null,
	                        PlayerAmmoType pAmmoType = PlayerAmmoType.Null)
	{
		if (IsDeathNpc) {
			return;
		}

        switch (NpcJiFen)
        {
            case NpcJiFenEnum.Boss:
                if (!XKBossXueTiaoCtrl.GetInstance().GetIsCanSubXueTiaoAmount())
                {
                    if (NpcDamageCom != null)
                    {
                        NpcDamageCom.PlayNpcDamageEvent();
                    }
                    return;
                }
                break;
        }

        if (CountActivePlayer != XkGameCtrl.PlayerActiveNum) {
			if (CountActivePlayer != 0) {
				//fix PuTongAmmoCount.
				int indexValTmp = CountActivePlayer - 1;
				int puTongAmmoNumTmp = MaxPuTongAmmo[indexValTmp];
				indexValTmp = XkGameCtrl.PlayerActiveNum - 1;
				if (indexValTmp >= 0) {
					float healthPer = (float)PuTongAmmoCount / puTongAmmoNumTmp;
					//int oldPuTongAmmoCount = PuTongAmmoCount;
					PuTongAmmoCount = (int)(healthPer * MaxPuTongAmmo[indexValTmp]);
					/*Debug.Log("Unity:"+"fix npc health -> PuTongAmmoCount "+PuTongAmmoCount
					          +", oldPuTongAmmoCount "+oldPuTongAmmoCount);*/
				}
			}
			CountActivePlayer = XkGameCtrl.PlayerActiveNum;
		}

		if (NpcDamageCom != null) {
			NpcDamageCom.PlayNpcDamageEvent();
		}

		if (NpcScript == null || (NpcScript != null && !NpcScript.GetIsWuDi())) {
			PuTongAmmoCount += damageNpcVal;
		}

		int indexVal = XkGameCtrl.PlayerActiveNum - 1;
		indexVal = indexVal < 0 ? 0 : indexVal;
		int puTongAmmoNum = MaxPuTongAmmo[indexVal];
		if (NpcJiFen == NpcJiFenEnum.Boss) {
			float bossAmount = (float)(puTongAmmoNum - PuTongAmmoCount) / puTongAmmoNum;
			bossAmount = bossAmount < 0f ? 0f : bossAmount;
			XKBossXueTiaoCtrl.GetInstance().SetBloodBossAmount(bossAmount, this);
		}

		/*Debug.Log("Unity:"+"OnDamageNpc -> "
		          +", nameNpc "+NpcNameInfo
		          +", puTongAmmoNum "+puTongAmmoNum);*/
		if (PuTongAmmoCount >= puTongAmmoNum)
        {
            if (NpcScript != null)
            {
                if (NpcScript.IsZhanCheNpc)
                {
                    //战车npc是否可以被击爆的判断.
                    if (XkGameCtrl.GetInstance().IsCaiPiaoHuLuePlayerIndex == false && NpcScript.m_IndexPlayerJiBao != playerSt)
                    {
                        //不是可以击爆战车npc的玩家.
                        return;
                    }

                    if (!XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsChuCaiPiaoByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe))
                    {
                        //战车彩池的彩票积累的不够.
                        return;
                    }
                }

                if (NpcScript.IsJPBossNpc)
                {
                    //JPBoss是否可以被击爆的判断.
                    if (XkGameCtrl.GetInstance().IsCaiPiaoHuLuePlayerIndex == false && NpcScript.m_IndexPlayerJiBao != playerSt)
                    {
                        //不是可以击爆JPBoss的玩家.
                        return;
                    }

                    if (!XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsChuCaiPiaoByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss))
                    {
                        //JPBoss彩池的彩票积累的不够.
                        return;
                    }
                }
            }

			if (IsDeathNpc)
            {
				return;
			}
			IsDeathNpc = true;

			if (IsOpenCameraShake) {
				XKPlayerCamera.GetInstanceFeiJi().HandlePlayerCameraShake();
			}

			if (NpcJiFen == NpcJiFenEnum.Boss && BossXieZiScript != null) {
				BossXieZiScript.ResetBossXieZiShouBiInfo();
			}

			if (BoxColCom != null) {
				BoxColCom.enabled = false;
			}
			CheckSpawnDaoJuCom(playerSt);
			CheckNpcDeathExplode(playerSt);
			CheckHiddenNpcObjArray();

//			bool isAddKillNpcNum = true;
//			if (NpcScript != null && CannonScript != null) {
//				if (NpcScript.GetIsDeathNPC()) {
//					isAddKillNpcNum = false;
//					Debug.Log("Unity:"+"name "+NpcScript.name+", isAddKillNpcNum "+isAddKillNpcNum);
//				}
//			}
			
			if (!IsYouTongNpc) {
				switch (NpcJiFen) {
				case NpcJiFenEnum.Boss:
					if (GameTimeBossCtrl.GetInstance().GetTimeBossResidual() > 0) {
						XkGameCtrl.GetInstance().AddPlayerKillNpc(PlayerEnum.Null, NpcJiFen, JiFenVal);
					}
					break;
				default:
					XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFen, JiFenVal);
					break;
				}
//				if (isAddKillNpcNum) {
//					switch (NpcJiFen) {
//					case NpcJiFenEnum.Boss:
//						if (GameTimeBossCtrl.GetInstance().GetTimeBossResidual() > 0) {
//							XkGameCtrl.GetInstance().AddPlayerKillNpc(PlayerEnum.Null, NpcJiFen, JiFenVal);
//						}
//						break;
//					default:
//						XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFen, JiFenVal);
//						break;
//					}
//				}
			}
			else {
				CheckYouTongDamageNpc();
			}

			if (NpcScript != null) {
				if (CannonScript != null) {
					CannonScript.OnRemoveCannon(playerSt, 1);
				}

				if (NpcJiFen != NpcJiFenEnum.Boss && NpcScript.GetIsBossNpc()) {
					return;
				}
				NpcScript.TriggerRemovePointNpc(1, CannonScript, pAmmoType);

                if (NpcScript.IsCaiPiaoZhanChe)
                {
                    SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe;
                    //彩票boss或战车npc.
                    if (NpcScript.GetIsBossNpc())
                    {
                        deCaiType = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss;
                    }

                    if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
                    {
                        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SubGameDeCaiValByDeCaiState(playerSt, deCaiType);
                    }
                }
            }
			else if (CannonScript != null) {
				CannonScript.OnRemoveCannon(playerSt, 1);
			}
		}
	}

	void CheckSpawnDaoJuCom(PlayerEnum index)
	{
		XKNpcSpawnDaoJu daoJuScript = GetComponent<XKNpcSpawnDaoJu>();
		if (daoJuScript == null) {
			return;
		}
        //daoJuScript.SpawnAllDaoJu();
        daoJuScript.CreatSuiJiDaoJu(index);
    }

	void CheckYouTongDamageNpc()
	{
		if (!IsYouTongNpc) {
			return;
		}

		XKNpcHealthCtrl healthScript = null;
		Transform[] npcArray = XkGameCtrl.GetInstance().GetNpcTranList().ToArray();
		int max = npcArray.Length;
		Vector3 posA = transform.position;
		Vector3 posB = Vector3.zero;
		for (int i = 0; i < max; i++) {
			if (npcArray[i] == null) {
				continue;
			}
			
			posB = npcArray[i].position;
			if (Vector3.Distance(posA, posB) <= YouTongDamageDis) {
				healthScript = npcArray[i].GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null) {
					//Add Damage Npc num to PlayerInfo.
					healthScript.OnDamageNpc(20, PlayerEnum.Null);
				}
			}
		}
	}

    XKDaPaoCtrl m_XKDaPaoCom;
    public void SetXKDaPaoScript(XKDaPaoCtrl script)
	{
        m_XKDaPaoCom = script;
        NpcNameInfo = script.name;
	}

	public void SetCannonScript(XKCannonCtrl script, bool isSpawn = true)
	{
		if (isSpawn) {
			IsSpawnObj = true;
		}
		CannonScript = script;
		ResetNpcHealthInfo();
	}

	public void SetIsDeathNpc(bool isDeath)
	{
		IsDeathNpc = isDeath;
	}

	void ResetNpcHealthInfo()
	{
        IsHitFanWeiHou = false;
        CheckNpcRigidbody();
		XkGameCtrl.GetInstance().AddNpcTranToList(transform);
		if (BoxColCom != null) {
			BoxColCom.enabled = true;
		}

		CountActivePlayer = 0;
		PuTongAmmoCount = 0;
		IsDeathNpc = false;
		int max = HiddenNpcObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (HiddenNpcObjArray[i] != null && !HiddenNpcObjArray[i].activeSelf) {
				HiddenNpcObjArray[i].SetActive(true);
			}
		}
		CheckDisGameCamera();
	}

	void CheckNpcDeathExplode(PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		if (DeathExplode == null) {
			return;
		}

		GameObject objExplode = null;
		objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
		objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(objExplode);

        if (NpcScript != null && NpcScript.IsCaiPiaoZhanChe)
        {
            SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe;
            //彩票boss或战车npc.
            if (NpcScript.GetIsBossNpc())
            {
                deCaiType = SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss;
                AudioBeiJingCtrl.StopGameBeiJingAudio();
            }

            if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
            {
                int value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(deCaiType);
                if (objExplode != null)
                {
                    SSCaiPiaoLiZiManage caiPiaoLiZi = objExplode.GetComponent<SSCaiPiaoLiZiManage>();
                    if (caiPiaoLiZi != null)
                    {
                        caiPiaoLiZi.ShowNumUI(value, indexPlayer);
                    }
                    else
                    {
                        Debug.LogWarning("CheckNpcDeathExplode -> caiPiaoLiZi was null.................");
                    }
                }
            }


            if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe)
            {
                if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
                {
                    //初始化飞出的彩票逻辑.
                    XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitCaiPiaoFly(transform, indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe);
                }
                else
                {
                    Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
                }
            }
            else if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
            {
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    SSUIRoot.GetInstance().m_GameUIManage.InitCaiPiaoAnimation(XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JPBossCaiPiaoFlyDt.TimeLeiJiaVal, indexPlayer);
                }

                if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
                {
                    //初始化烟花粒子的产生.
                    XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitPlayCaiPiaoYanHua();
                }
                else
                {
                    Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
                }
            }
        }
    }

	public string GetNpcName()
	{
		return NpcNameInfo;
	}

	XKBossXieZiCtrl BossXieZiScript;
	public void SetBossXieZiScript(XKBossXieZiCtrl xieZiScript)
	{
		BossXieZiScript = xieZiScript;
	}

	void CheckNpcRigidbody()
	{
		Rigidbody rigCom = GetComponent<Rigidbody>();
		if (rigCom == null) {
			rigCom = gameObject.AddComponent<Rigidbody>();
		}

		SphereCollider spCol = GetComponent<SphereCollider>();
		if (spCol != null) {
			rigCom.isKinematic = false;
			return;
		}
		rigCom.isKinematic = true;
	}

	public float GetBossFillAmount()
	{
		if (NpcJiFen != NpcJiFenEnum.Boss) {
			return 1f;
		}
		float bossAmount = 1f;
		int indexVal = XkGameCtrl.PlayerActiveNum - 1;
		indexVal = indexVal < 0 ? 0 : indexVal;
		int puTongAmmoNum = MaxPuTongAmmo[indexVal];
		bossAmount = (float)(puTongAmmoNum - PuTongAmmoCount) / puTongAmmoNum;
		bossAmount = bossAmount < 0f ? 0f : bossAmount;
		return bossAmount;
	}
}