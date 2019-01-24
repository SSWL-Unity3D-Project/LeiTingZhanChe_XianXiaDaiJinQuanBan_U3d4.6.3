#define USE_SPHERE_HIT
using UnityEngine;

/// <summary>
/// npc的子弹若要能被玩家子弹击爆,必须给npc的子弹添加Cube碰撞,否则不要添加碰撞.
/// </summary>
public class NpcAmmoCtrl : MonoBehaviour {
	public bool IsChangeAmmoSpeed = false;
	public PlayerAmmoType AmmoType = PlayerAmmoType.PuTongAmmo;
	[Range(1f, 4000f)] public float MvSpeed = 50f; //km/h
	[Range(0f, 100f)] public float LifeTime = 0f;
	[Range(0f, 10000f)] public float PlayerDamage = 1f;
	public GameObject AmmoExplode;
	public GameObject MetalParticle;		//金属.
	public GameObject ConcreteParticle;		//混凝土.
	public GameObject DirtParticle;			//土地.
	public GameObject WoodParticle;			//树木.
	public GameObject WaterParticle;		//水.
	public GameObject SandParticle;			//沙滩.
	public GameObject GlassParticle;		//玻璃.
	public GameObject TestSpawnNpc;
	#if !USE_SPHERE_HIT
	Vector3 AmmoEndPos;
	Vector3 AmmoStartPos;
	#endif
	GameObject ObjAmmo;
	Transform AmmoTran;
	XKNpcMoveCtrl NpcScript;
	//bool IsAimFeiJiPlayer;
//	public bool IsAimPlayer = true;
	bool IsRemoveAmmo;
//	float TimeScaleVal;
	bool IsOnFinishMove;
	bool IsCannotAddNpcAmmoList;
	public static LayerMask NpcAmmoHitLayer;
	bool IsDestoryNpcAmmo;
	float TimeTrail;
	TrailRenderer TrailScript;
	// Use this for initialization
	void Awake()
	{
		//Debug.Log("Unity:"+"*************** TKMoveSt "+pcvr.TKMoveSt);
		//if (pcvr.TKMoveSt == TKMoveState.U_FangXiangPan && IsChangeAmmoSpeed) {
		//	MvSpeed *= XkGameCtrl.GetInstance().NpcAmmoSpeed;
		//}

		if (AmmoType == PlayerAmmoType.GenZongAmmo) {
			if (GetComponent<BoxCollider>() == null) {
				BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
				boxCol.size = new Vector3(0.2f, 0.2f, 0.2f);
			}
			gameObject.layer = LayerMask.NameToLayer(XkGameCtrl.NpcLayerInfo);
		}
		SpawnTime = Time.realtimeSinceStartup;
		ObjAmmo = gameObject;
		AmmoTran = transform;
		SetGenZongDanInfo();
		
		//MovePaiJiPaoAmmo(); //test
//		if (LifeTime <= 0) {
//			Invoke("DelayRemoveNpcAmmo", 10f);
//		}
//		else {
//			Invoke("DelayRemoveNpcAmmo", LifeTime);
//		}
//		Invoke("DelayAddNpcAmmoList", 0.05f);
	}

	void Update()
	{
		if (AmmoType == PlayerAmmoType.GenZongAmmo) {
			AmmoGenZongDanUpdate();
		}
		else {
			UpdateAmmoHit();
		}
	}

	void UpdateAmmoHit()
	{
		if (AmmoType == PlayerAmmoType.PaiJiPaoAmmo) {
			return;
		}

		bool isHitObj = false;
		XKPlayerMoveCtrl playerScript = null;
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, HitRadius, NpcAmmoHitLayer);
		foreach (Collider c in hits) {
			if (c.isTrigger) {
				continue;
			}
			
			playerScript = c.GetComponent<XKPlayerMoveCtrl>();
			isHitObj = true;
			break;
		}
		
		Vector3 posA = AmmoTran.position;
		Vector3 posB = AmmoTran.position;
		if (!isHitObj) {
			for (int i = 0; i < 4; i++) {
				playerScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl((PlayerEnum)(i+1));
				if (playerScript == null || playerScript.GetIsDeathPlayer()) {
					continue;
				}
				posB = playerScript.transform.position;
				posA.y = posB.y = 0f;
				
				if (Vector3.Distance(posA, posB) <= HitRadius) {
					isHitObj = true;
					break;
				}
			}
		}

		if (isHitObj) {
			if (playerScript != null && !playerScript.GetIsDeathPlayer())
            {
                if (AmmoType == PlayerAmmoType.GenZongAmmo && IsOnlyHitTarget == true
                    && TargetObject != null && TargetObject != playerScript.GenZongDanAimPoint)
                {
                    //只对跟踪目标造成伤害的跟踪弹.
                }
                else
                {
                    XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
                }
			}
			MoveAmmoOnCompelteITween();
		}
	}

	public void SetNpcScriptInfo(XKNpcMoveCtrl scriptVal)
	{
		if (scriptVal == null) {
			return;
		}

		NpcScript = scriptVal;
//		IsAimPlayer = NpcScript.GetIsAimPlayer();
		TestSpawnNpc = NpcScript.gameObject;
	}
	
	XKNpcAmmoSiSanCtrl AmmoSiSanScript;
	public void SetIsAimFeiJiPlayer(bool isAimFeiJi)
	{
		if (ObjAmmo == null) {
			ObjAmmo = gameObject;
		}

		if (!ObjAmmo.activeSelf) {
			ObjAmmo.SetActive(true);
			if (IsInvoking("CheckNpcAmmoState")) {
				CancelInvoke("CheckNpcAmmoState");
			}
		}

		bool isReturn = false;
		switch (AmmoType) {
		case PlayerAmmoType.PaiJiPaoAmmo:
			DestroyNpcAmmo(PaiJiPaoTiShi);
			MovePaiJiPaoAmmo();
			isReturn = true;
			break;
		case PlayerAmmoType.GenZongAmmo:
			isReturn = true;
			break;
		}
		
		if (isReturn) {
			return;
		}

		if (IsInvoking("DelayRemoveNpcAmmo")) {
			CancelInvoke("DelayRemoveNpcAmmo");
		}

		if (LifeTime <= 0) {
			Invoke("DelayRemoveNpcAmmo", 10f);
		}
		else {
			Invoke("DelayRemoveNpcAmmo", LifeTime);
		}

		//IsAimFeiJiPlayer = isAimFeiJi;
		//DelayAddNpcAmmoList();
		MoveAmmoByItween();
	}

//	public void SetIsAimPlayer(bool isAim)
//	{
//		IsAimPlayer = isAim;
//	}

	const float SpeedMaxVal = 150f;
	void MoveAmmoByItween()
	{
		float disVal = 500f;
		RaycastHit hitInfo;
		Vector3[] posArray = new Vector3[2];
		Vector3 startPos = AmmoTran.position;
		Vector3 forwardVal = AmmoTran.forward;
		//Instantiate(testObjAmmo, AmmoTran.position, AmmoTran.rotation); //test
		Vector3 firePos = startPos + (forwardVal * disVal);
		startPos += forwardVal * 3f;
		//优化处理,当npo子弹速度足够大时采用.
		if (MvSpeed > SpeedMaxVal) {
			Physics.Raycast(startPos, forwardVal, out hitInfo, disVal, NpcAmmoHitLayer);
			if (hitInfo.collider != null){
				firePos = hitInfo.point;
				//Debug.Log("Unity:"+"*****npcAmmoHitObj "+hitInfo.collider.name);
				XKPlayerMoveCtrl playerScript = hitInfo.collider.GetComponent<XKPlayerMoveCtrl>();
				if (playerScript != null && !playerScript.GetIsWuDiState())
                {
                    if (AmmoType == PlayerAmmoType.GenZongAmmo && IsOnlyHitTarget == true
                        && TargetObject != null && TargetObject != playerScript.GenZongDanAimPoint)
                    {
                        //只对跟踪目标造成伤害的跟踪弹.
                    }
                    else
                    {
                        XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
                    }
				}
			}
		}

//		TimeScaleVal = Time.timeScale;
		posArray[0] = AmmoTran.position;
		posArray[1] = firePos;
		#if !USE_SPHERE_HIT
		AmmoStartPos = posArray[0];
		AmmoEndPos = firePos;
		#endif
		iTween.MoveTo(ObjAmmo, iTween.Hash("path", posArray,
		                                   "speed", MvSpeed,
		                                   "orienttopath", true,
		                                   "easeType", iTween.EaseType.linear,
		                                   "oncomplete", "MoveAmmoOnCompelteITween"));
	}
	
	GameObject CheckPlayerAmmoOverlapSphereHit()
	{
		GameObject obj = null;
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, 1f, NpcAmmoHitLayer);
		foreach (Collider c in hits) {
			if (c.isTrigger) {
				continue;
			}
			obj = c.gameObject;
		}
		return obj;
	}

	void SpawnAmmoParticleObj()
	{
		#if USE_SPHERE_HIT
		GameObject objParticle = null;
		GameObject hitObj = CheckPlayerAmmoOverlapSphereHit();
		if (hitObj == null) {
			switch (AmmoType) {
			case PlayerAmmoType.PuTongAmmo:
				break;
			default:
				objParticle = AmmoExplode;
				break;
			}
		}
		else {
			string tagHitObj = hitObj.tag;
			switch (AmmoType) {
			case PlayerAmmoType.PuTongAmmo:
				XKAmmoParticleCtrl ammoParticleScript = hitObj.GetComponent<XKAmmoParticleCtrl>();
				if (ammoParticleScript != null && ammoParticleScript.PuTongAmmoLZ != null) {
					objParticle = ammoParticleScript.PuTongAmmoLZ;
				}
				else {
					switch (tagHitObj) {
					case "metal":
						objParticle = MetalParticle;
						break;
						
					case "concrete":
						objParticle = ConcreteParticle;
						break;
						
					case "dirt":
						objParticle = DirtParticle;
						break;
						
					case "wood":
						objParticle = WoodParticle;
						break;
						
					case "water":
						objParticle = WaterParticle;
						break;
						
					case "sand":
						objParticle = SandParticle;
						break;
						
					case "glass":
						objParticle = GlassParticle;
						break;
					default:
						objParticle = AmmoExplode;
						break;
					}
				}
				break;
			default:
				switch (tagHitObj) {
				case "dirt":
					objParticle = DirtParticle;
					break;
				case "water":
					objParticle = WaterParticle;
					break;
				default:
					objParticle = AmmoExplode;
					break;
				}
				break;
			}
		}

		if (objParticle == null) {
			return;
		}
		
		GameObject obj = (GameObject)Instantiate(objParticle, AmmoTran.position, AmmoTran.rotation);
		Transform tran = obj.transform;
		tran.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);
		#else
		GameObject objParticle = null;
		GameObject obj = null;
		Transform tran = null;
		Vector3 hitPos = transform.position;
		
		RaycastHit hit;
		Vector3 forwardVal = Vector3.Normalize(AmmoEndPos - AmmoStartPos);
		if (AmmoType == PlayerAmmoType.PuTongAmmo) {
			float disVal = Vector3.Distance(AmmoEndPos, AmmoStartPos) + 10f;
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, NpcAmmoHitLayer);
			if (hit.collider != null) {
				hitPos = hit.point;
				XKAmmoParticleCtrl ammoParticleScript = hit.collider.GetComponent<XKAmmoParticleCtrl>();
				if (ammoParticleScript != null && ammoParticleScript.PuTongAmmoLZ != null) {
					objParticle = ammoParticleScript.PuTongAmmoLZ;
				}
				else {
					string tagHitObj = hit.collider.tag;
					switch (tagHitObj) {
					case "metal":
						if (MetalParticle != null) {
							objParticle = MetalParticle;
						}
						break;
						
					case "concrete":
						if (ConcreteParticle != null) {
							objParticle = ConcreteParticle;
						}
						break;
						
					case "dirt":
						if (DirtParticle != null) {
							objParticle = DirtParticle;
						}
						break;
						
					case "wood":
						if (WoodParticle != null) {
							objParticle = WoodParticle;
						}
						break;
						
					case "water":
						if (WaterParticle != null) {
							objParticle = WaterParticle;
						}
						break;
						
					case "sand":
						if (SandParticle != null) {
							objParticle = SandParticle;
						}
						break;
						
					case "glass":
						if (GlassParticle != null) {
							objParticle = GlassParticle;
						}
						break;
					}
					
					if (objParticle == null) {
						objParticle = AmmoExplode;
					}
				}
			}
			else {
				objParticle = AmmoExplode;
			}
		}
		else {
			float disVal = Vector3.Distance(AmmoEndPos, AmmoStartPos) + 10f;
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, NpcAmmoHitLayer);
			if (hit.collider != null) {
				hitPos = hit.point;
				string tagHitObj = hit.collider.tag;
				switch (tagHitObj) {
				case "dirt":
					if (DirtParticle != null) {
						objParticle = DirtParticle;
					}
					break;
				}
				
				if (objParticle == null) {
					objParticle = AmmoExplode;
				}
			}
			else {
				objParticle = AmmoExplode;
			}
		}
		
		if (objParticle == null) {
			return;
		}
		
		if (AmmoType == PlayerAmmoType.DaoDanAmmo) {
			Vector3 AmmoPos = transform.position - (transform.forward * 3f);
			Physics.Raycast(AmmoPos, forwardVal, out hit, 13f, XkGameCtrl.GetInstance().LandLayer);
			if (hit.collider != null) {
				hitPos = hit.point;
				Vector3 normalVal = hit.normal;
				Quaternion rotVal = Quaternion.LookRotation(normalVal);
				obj = (GameObject)Instantiate(objParticle, hitPos, rotVal);
				obj.transform.up = normalVal;
			}
			else {
				obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
			}
		}
		else {
			obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
		}
		
		tran = obj.transform;
		tran.parent = XkGameCtrl.NpcAmmoArray;
		
		XkAmmoTieHuaCtrl tieHuaScript = obj.GetComponent<XkAmmoTieHuaCtrl>();
		if (tieHuaScript != null && tieHuaScript.TieHuaTran != null) {
			Transform tieHuaTran = tieHuaScript.TieHuaTran;
			Vector3 AmmoPos = transform.position - (transform.forward * 3f);
			Physics.Raycast(AmmoPos, forwardVal, out hit, 13f, XkGameCtrl.GetInstance().PlayerAmmoHitLayer);
			if (hit.collider != null) {
				tieHuaTran.up = hit.normal;
			}
		}
		#endif
	}

	/**
	 * key == 0 -> 删除当前对象的Itween.
	 * key == 1 -> 删除当前子对象的Itween.
	 */
	public static void RemoveItweenComponents(GameObject obj, int key = 0)
	{
		iTween[] itweenCom = null;
		switch (key) {
		case 0:
			itweenCom = obj.GetComponents<iTween>();
			break;
		case 1:
			itweenCom = obj.GetComponentsInChildren<iTween>();
			break;
		}

		for (int i = 0; i < itweenCom.Length; i++) {
			itweenCom[i].isRunning = false;
			itweenCom[i].isPaused = true;
			itweenCom[i].enabled = false;
			Destroy(itweenCom[i]);
		}
	}

	public void MoveAmmoOnCompelteITween()
	{
		//Debug.Log("Unity:"+"MoveAmmoOnCompelteITween...");
		RemoveItweenComponents(gameObject);

		if (AmmoType == PlayerAmmoType.GenZongAmmo) {
			AmmoHealth -= 1f;
			if (AmmoHealth <= 0f) {
				if (AmmoExplode != null) {
					GameObject expObj = (GameObject)Instantiate(AmmoExplode, AmmoTran.position, AmmoTran.rotation);
					if (XkGameCtrl.NpcAmmoArray != null) {
						expObj.transform.parent = XkGameCtrl.NpcAmmoArray;
					}
				}
				DestroyNpcAmmo(ObjAmmo);
			}
			return;
		}

		if (IsOnFinishMove) {
			return;
		}
		IsOnFinishMove = true;

		SpawnAmmoParticleObj();
		CancelInvoke("DelayRemoveNpcAmmo");
		RemoveAmmo();
	}

	public void RemoveAmmo(int key = 0)
	{
		if (IsRemoveAmmo) {
			return;
		}
		IsRemoveAmmo = true;
		RemoveItweenComponents(gameObject, 1);

		if (key == 1) {
			RemoveItweenComponents(gameObject);
			if (AmmoType == PlayerAmmoType.GenZongAmmo) {
				AmmoHealth -= 1f;
				if (AmmoHealth <= 0f) {
					if (AmmoExplode != null) {
						GameObject expObj = (GameObject)Instantiate(AmmoExplode, AmmoTran.position, AmmoTran.rotation);
						if (XkGameCtrl.NpcAmmoArray != null) {
							expObj.transform.parent = XkGameCtrl.NpcAmmoArray;
						}
					}
					DestroyNpcAmmo(ObjAmmo);
				}
				return;
			}
		}

		//XkGameCtrl.RemoveNpcAmmoList(gameObject);
		if (PaiJiPaoTiShi != null) {
			IsDestoryNpcAmmo = true;
			DestroyNpcAmmo(PaiJiPaoTiShi);
		}

		if (IsDestoryNpcAmmo) {
            Invoke("DelayDestroySelf", 0.05f);
            //Destroy(ObjAmmo, 0.05f);
		}
		else {
			if (TrailScript == null) {
				TrailScript = GetComponentInChildren<TrailRenderer>();
			}
			
			if (TrailScript != null) {
				TimeTrail = TrailScript.time;
				TrailScript.time = 0f;
			}
			Invoke("DelayHiddenNpcAmmo", 0.05f);
		}
	}

    void DelayDestroySelf()
    {
        if (ObjAmmo != null)
        {
            Destroy(ObjAmmo);
            ObjAmmo = null;
        }
    }

	public void MakeNpcAmmoDestory()
	{
		if (IsDestoryNpcAmmo) {
			return;
		}
		IsDestoryNpcAmmo = true;

		if (!ObjAmmo.activeSelf) {
			DestroyNpcAmmo(ObjAmmo);
		}
	}

	public void SetIsDestoryNpcAmmo()
	{
		IsDestoryNpcAmmo = true;
	}

	void DelayHiddenNpcAmmo()
	{
		IsOnFinishMove = false;
		IsRemoveAmmo = false;
		if (TrailScript != null) {
			TrailScript.time = TimeTrail;
		}
		ObjAmmo.SetActive(false);
//		if (IsInvoking("CheckNpcAmmoState")) {
//			CancelInvoke("CheckNpcAmmoState");
//		}
//		Invoke("CheckNpcAmmoState", 10f);
	}

	void CheckNpcAmmoState()
	{
		if (ObjAmmo == null) {
			return;
		}

		if (ObjAmmo.activeSelf) {
			return;
		}
		DestroyNpcAmmo(ObjAmmo);
	}

	void DelayRemoveNpcAmmo()
	{
		RemoveAmmo();
	}

	public void GameNeedRemoveAmmo()
	{
		if (IsRemoveAmmo) {
			return;
		}
//		Debug.Log("Unity:"+"GameNeedRemoveAmmo...ammo "+gameObject.name);

		CancelInvoke("DelayRemoveNpcAmmo");
		RemoveAmmo();
	}
	
	public void SetIsCannotAddNpcAmmoList()
	{
		IsCannotAddNpcAmmoList = true;
	}

	void DelayAddNpcAmmoList()
	{
		if (IsCannotAddNpcAmmoList) {
			return;
		}
		//XkGameCtrl.AddNpcAmmoList(gameObject);
	}

	[Range(0f, 1000f)]public float AmmoHealth = 0f;
	/**
	 * MuBiaoXuanDing == 0 -> 随机选择.
	 * MuBiaoXuanDing == 1 -> 当前血量最高玩家.
	 */
	[Range(0, 1)]public int MuBiaoXuanDing = 0;
	[Range(0f, 90f)]public float ShanXingJiaoDu = 45f;
	float CosJiaoDuGZ;
	public LayerMask IgnoreLayers;
	const float HitRadius = 1.0f;
	//延迟跟踪转向时间.
	float SeekPrecision = 1.3f;
	Vector3 DirAmmo;
	float SpawnTime;
	GameObject TargetObject;
//	public GameObject TestTarget;
	float forceAmount = 10f;

    /// <summary>
    /// 是否只对跟踪目标造成伤害.
    /// </summary>
    bool IsOnlyHitTarget;
    /// <summary>
    /// 攻击已经得到战车代金券的玩家的跟踪弹.
    /// </summary>
    public void SetGenZongDanAimTarget(XKPlayerMoveCtrl playerCom)
    {
        if (AmmoType != PlayerAmmoType.GenZongAmmo)
        {
            return;
        }

        if (TargetObject != null || playerCom == null)
        {
            return;
        }
        IsOnlyHitTarget = true;
        TargetObject = playerCom.GenZongDanAimPoint;
    }

	void SetGenZongDanInfo()
	{
		if (AmmoType != PlayerAmmoType.GenZongAmmo)
        {
			return;
		}

        if (TargetObject != null)
        {
            return;
        }

		CosJiaoDuGZ = Mathf.Cos(ShanXingJiaoDu);
		GameObject playerObj = null;
		if (XkGameCtrl.GetInstance() != null) {
			switch (MuBiaoXuanDing) {
			case 1:
				playerObj = XkGameCtrl.GetInstance().GetMaxHealthPlayer();
				break;
			default:
				playerObj = XkGameCtrl.GetInstance().GetRandAimPlayerObj();
				break;
			}
		}
		TargetObject = playerObj;
		//TargetObject = TestTarget; //test
	}

	void AmmoGenZongDanUpdate()
	{
		if (Time.realtimeSinceStartup > SpawnTime + LifeTime) {
			DestroyNpcAmmo(ObjAmmo);
			return;
		}

		bool isGenZong = false;
		if (TargetObject) {
			Vector3 targetPos = TargetObject.transform.position;
			float cosVal = 0f;
			Vector3 vecA = AmmoTran.forward;
			Vector3 vecB = targetPos - AmmoTran.position;
			vecB.y = vecA.y = 0f;
			cosVal = Vector3.Dot(vecA, vecB.normalized);
			if (cosVal > CosJiaoDuGZ) {
				isGenZong = true;
				//targetPos += transform.right * (Mathf.PingPong (Time.time, 1.0f) - 0.5f) * noise;
				Vector3 targetDirAmmo = (targetPos - AmmoTran.position);
				float targetDist = targetDirAmmo.magnitude;
				targetDirAmmo /= targetDist;
				
				DirAmmo = Vector3.Slerp (DirAmmo, targetDirAmmo, Time.deltaTime * SeekPrecision);
				AmmoTran.rotation = Quaternion.LookRotation(DirAmmo);
				AmmoTran.position += (DirAmmo * MvSpeed) * Time.deltaTime;
			}
		}

		if (!isGenZong) {
			AmmoTran.position += (AmmoTran.forward * MvSpeed) * Time.deltaTime;
		}
		
		// Check if this one hits something
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, HitRadius, ~IgnoreLayers.value);
		bool collided = false;
		foreach (Collider c in hits) {
			// Don't collide with triggers
			if (c.isTrigger) {
				continue;
			}
			
			XKPlayerMoveCtrl playerScript = c.GetComponent<XKPlayerMoveCtrl>();
			if (playerScript != null && !playerScript.GetIsWuDiState()) {
                if (AmmoType == PlayerAmmoType.GenZongAmmo && IsOnlyHitTarget == true
                    && TargetObject != null && TargetObject != playerScript.GenZongDanAimPoint)
                {
                    //只对跟踪目标造成伤害的跟踪弹.
                }
                else
                {
                    XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
                }
			}
			// Get the rigidbody if any
			if (c.rigidbody) {
				// Apply force to the target object
				Vector3 force = AmmoTran.forward * forceAmount;
				force.y = 0f;
				c.rigidbody.AddForce (force, ForceMode.Impulse);
			}
			collided = true;
		}
		
		if (collided) {
			if (AmmoExplode != null) {
				GameObject expObj = (GameObject)Instantiate(AmmoExplode, AmmoTran.position, AmmoTran.rotation);
				if (XkGameCtrl.NpcAmmoArray != null) {
					expObj.transform.parent = XkGameCtrl.NpcAmmoArray;
				}
			}
			DestroyNpcAmmo(ObjAmmo);
		}
	}

	//迫击炮.
	public GameObject PaiJiPaoTiShiPrefab;
	public GameObject AmmoCore;
	[Range(0.1f, 100f)]public float PaiJiPaoMvTime = 1f;
	[Range(0.1f, 10f)]public float PaiJiPaoDamageDis = 1f;
	[Range(0.1f, 100f)]public float MaxPaiJiPaoDis = 8f;
	[Range(0.1f, 100f)]public float MinPaiJiPaoDis = 7f;
	/**
	 * 迫击炮高度计算斜率.
	 */
	[Range(0f, 1000f)]public float PaiJiPaoGDKey = 0.5f;
	/**
	 * 迫击炮最小高度.
	 */
	[Range(0f, 100f)]public float PaiJiPaoGDMin = 0.5f;
	GameObject PaiJiPaoTiShi;
	void MovePaiJiPaoAmmo()
	{
		Vector3 firePos = Vector3.zero;
		Vector3 vecA = AmmoTran.forward;
		vecA.y = 0f;
		MinPaiJiPaoDis = MinPaiJiPaoDis < MaxPaiJiPaoDis ? MinPaiJiPaoDis : MaxPaiJiPaoDis;
		float paoDanMVDis = Random.Range(MinPaiJiPaoDis, MaxPaiJiPaoDis);
		firePos = vecA * paoDanMVDis + AmmoTran.position;
		
		RaycastHit hit;
		int layerVal = XkGameCtrl.GetInstance() != null ? XkGameCtrl.GetInstance().LandLayer.value : 0;
		Physics.Raycast(firePos, Vector3.down, out hit, 100f, layerVal);
		if (hit.collider != null) {
			firePos = hit.point;
		}

		if (PaiJiPaoTiShiPrefab != null) {
			PaiJiPaoTiShi = (GameObject)Instantiate(PaiJiPaoTiShiPrefab, firePos, Quaternion.identity);
			if (XkGameCtrl.NpcAmmoArray != null) {
				PaiJiPaoTiShi.transform.parent = XkGameCtrl.NpcAmmoArray;
			}
		}

		float lobHeight = PaiJiPaoGDKey * paoDanMVDis + PaiJiPaoGDMin;
		float lobTime = PaiJiPaoMvTime;
		iTween.MoveBy(AmmoCore, iTween.Hash("y", lobHeight,
		                                    "time", lobTime * 0.5f,
		                                    "easeType", iTween.EaseType.easeOutQuad));
		iTween.MoveBy(AmmoCore, iTween.Hash("y", -lobHeight,
		                                    "time", lobTime * 0.5f,
		                                    "delay", lobTime * 0.5f,
		                                    "easeType", iTween.EaseType.easeInCubic));
		iTween.MoveTo(ObjAmmo, iTween.Hash("position", firePos,
		                                      "time", lobTime,
		                                      "easeType", iTween.EaseType.linear,
		                                      "oncomplete", "MovePaiJiPaoAmmoOnCompelteITween"));
	}

	void MovePaiJiPaoAmmoOnCompelteITween()
	{
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, PaiJiPaoDamageDis, ~IgnoreLayers.value);
		foreach (Collider c in hits) {
			if (c.isTrigger) {
				continue;
			}
			
			XKPlayerMoveCtrl playerScript = c.GetComponent<XKPlayerMoveCtrl>();
			if (playerScript != null && !playerScript.GetIsWuDiState()) {
				XkGameCtrl.GetInstance().SubGamePlayerHealth(playerScript.PlayerIndex, PlayerDamage);
			}
		}
		DestroyNpcAmmo(PaiJiPaoTiShi);
		
		if (AmmoSiSanScript == null) {
			AmmoSiSanScript = GetComponent<XKNpcAmmoSiSanCtrl>();
		}
		if (AmmoSiSanScript != null) {
			AmmoSiSanScript.SpawnNpcAmmo();
		}

		if (AmmoExplode != null) {
			GameObject expObj = (GameObject)Instantiate(AmmoExplode, AmmoTran.position, AmmoTran.rotation);
			if (XkGameCtrl.NpcAmmoArray != null) {
				expObj.transform.parent = XkGameCtrl.NpcAmmoArray;
			}
			XkGameCtrl.CheckObjDestroyThisTimed(expObj);
		}
		DestroyNpcAmmo(ObjAmmo);
	}

	void DestroyNpcAmmo(GameObject ammoObj)
	{
		if (ammoObj != null) {
			Destroy(ammoObj);
		}
	}
}