//#define TEST_CLOSE_SPHER_HIT
#define USE_SPHERE_HIT
using UnityEngine;
using System.Collections.Generic;

public class PlayerAmmoCtrl : MonoBehaviour
{
    /// <summary>
    /// 子弹数据信息.
    /// </summary>
    public SSPlayerAmmoData m_AmmmoData;
    [Range(0, 100)]
    public int AmmoIndex = 0;
	public PlayerAmmoType AmmoType = PlayerAmmoType.PuTongAmmo;
	/**
	 * 迫击炮的内核.
	 */
	public GameObject AmmoCore;
	/**
	 * 迫击炮的提示特效.
	 */
	public GameObject PaiJiPaoTiShiPrefab;
    /// <summary>
    /// 子弹爆炸粒子预制.
    /// </summary>
    public GameObject AmmoExplode;
	[Range(1, 1000)] public int DamageNpc = 1;
	[Range(1f, 4000f)] public float MvSpeed = 50f;
	const float MvSpeedMax = 500f;
    /// <summary>
    /// 子弹的伤害范围.
    /// </summary>
    [Range(1f, 1000f)]
    public float AmmoDamageDis = 50f;
	[Range(0.001f, 100f)] public float LiveTime = 4f;
	public GameObject MetalParticle;		//金属.
	public GameObject ConcreteParticle;		//混凝土.
	public GameObject DirtParticle;			//土地.
	public GameObject WoodParticle;			//树木.
	public GameObject WaterParticle;		//水.
	public GameObject SandParticle;			//沙滩.
	public GameObject GlassParticle;		//玻璃.
	GameObject ObjAmmo;
	Transform AmmoTran;
	PlayerEnum PlayerState = PlayerEnum.Null;
	public static LayerMask PlayerAmmoHitLayer;
	public static LayerMask NpcCollisionLayer;
	//Vector3 AmmoStartPos;
	//Vector3 AmmoEndPos;
	bool IsHandleRpc;
	bool IsDestroyAmmo;
	TrailRenderer TrailScript;
	float TrailTime = 3f;
	float MaxDisVal;
	float CosABMin = Mathf.Cos(Mathf.PI*(60f/180f));
	void Awake()
	{
		TrailScript = GetComponentInChildren<TrailRenderer>();
		if (TrailScript != null) {
			TrailScript.castShadows = false;
			TrailScript.receiveShadows = false;
			TrailTime = TrailScript.time;
		}

		AmmoTran = transform;
		ObjAmmo = gameObject;
		AmmoTran.parent = XkGameCtrl.PlayerAmmoArray;
		MaxDisVal = MvSpeed * LiveTime;
	}

    float m_TimeLastAmmoHit = 0f;
	void Update()
	{
		if (!IsHandleRpc) {
			return;
		}

		if (IsDestroyAmmo || IsChuanJiaDanHitCamColForward) {
			return;
		}

        if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
        {
            //冲击波子弹只进行一次范围伤害计算不用进行轮询更新.
            UpdateRemoveChongJiBoAmmo();
            return;
        }

        if (AmmoType == PlayerAmmoType.DaoDanAmmo
            || AmmoType == PlayerAmmoType.ChuanTouAmmo
            || AmmoType == PlayerAmmoType.SanDanAmmo)
        {
            if (AmmoType == PlayerAmmoType.ChuanTouAmmo || AmmoType == PlayerAmmoType.SanDanAmmo)
            {
                if (Time.time - m_TimeLastAmmoHit > 0.05f)
                {
                    m_TimeLastAmmoHit = Time.time;
                    CheckPlayerAmmoForwardHitNpc();
                }
            }
            else
            {
                CheckPlayerAmmoForwardHitNpc();
            }
        }
        else
        {
            if (Time.time - m_TimeLastAmmoHit > 0.1f)
            {
                m_TimeLastAmmoHit = Time.time;
                CheckPlayerAmmoForwardHitNpc();
            }
        }
	}

    /// <summary>
    /// 是否为Ai坦克发射的子弹.
    /// </summary>
    bool IsAiFireAmmo = false;
    internal void SetIsAiFireAmmo(bool isAiFireAmmo)
    {
        IsAiFireAmmo = isAiFireAmmo;
    }


    List<XKNpcHealthCtrl> NpcHealthList;
	bool CheckAmmoHitObj(GameObject hitObjNpc, PlayerEnum playerIndex)
	{
//		BuJiBaoCtrl buJiBaoScript = hitObjNpc.GetComponent<BuJiBaoCtrl>();
//		if (buJiBaoScript != null) {
//			buJiBaoScript.RemoveBuJiBao(playerIndex); //buJiBaoScript
//			return;
//		}

		bool isStopCheckHit = false;
		if (AmmoType != PlayerAmmoType.PaiJiPaoAmmo) {
			XKPlayerCheckCamera checkCam = hitObjNpc.GetComponent<XKPlayerCheckCamera>();
			if (checkCam != null)
            {
                IsHitNpcAmmo = true;
                //SSDebug.LogWarning("MoveAmmoOnCompelteITween ============== 55555555555555555");
                MoveAmmoOnCompelteITween();
				return true;
			}
		}

		XKNpcHealthCtrl healthScript = hitObjNpc.GetComponent<XKNpcHealthCtrl>();
		if (healthScript != null && !healthScript.GetIsDeathNpc()) {
			/*Debug.Log("Unity:"+"CheckAmmoHitObj -> OnDamageNpc: "
			          +"AmmoType "+AmmoType
			          +", AmmoName "+AmmoTran.name
			          +", NpcName "+healthScript.GetNpcName()
			          +", AmmoDamageDis "+AmmoDamageDis);*/
			bool isHitNpc = false;
			switch (AmmoType)
            {
			case PlayerAmmoType.ChuanTouAmmo:
			case PlayerAmmoType.PaiJiPaoAmmo:
                    {
                        if (NpcHealthList == null)
                        {
                            NpcHealthList = new List<XKNpcHealthCtrl>();
                        }

                        if (!NpcHealthList.Contains(healthScript))
                        {
                            NpcHealthList.Add(healthScript);
                            isHitNpc = true;
                        }
                        break;
                    }
			default:
                    {
                        IsHitNpcAmmo = true;
                        //MoveAmmoOnCompelteITween();
                        isStopCheckHit = true;
                        isHitNpc = true;
                        break;
                    }
			}
            
			if (isHitNpc)
            {
                int baoJiDamage = 0;
                if (AmmoType == PlayerAmmoType.ChuanTouAmmo
                    || AmmoType == PlayerAmmoType.DaoDanAmmo
                    || AmmoType == PlayerAmmoType.PaiJiPaoAmmo
                    || AmmoType == PlayerAmmoType.SanDanAmmo
                    || AmmoType == PlayerAmmoType.ChongJiBoAmmo)
                {
                    //获取玩家对代金券npc的暴击伤害.
                    XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.CheckPlayerBaoJiDengJi(AmmoType, PlayerState, healthScript);
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null && healthScript.GetIsDaiJinQuanNpc() == true)
                    {
                        baoJiDamage = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetBaoJiDamage(PlayerState);
                    }
                }

                //if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null && healthScript.GetIsDaiJinQuanNpc() == true)
                //{
                //    if (AmmoType == PlayerAmmoType.ChuanTouAmmo
                //        || AmmoType == PlayerAmmoType.DaoDanAmmo
                //        || AmmoType == PlayerAmmoType.PaiJiPaoAmmo
                //        || AmmoType == PlayerAmmoType.SanDanAmmo)
                //    {
                //        //获取玩家对代金券npc的暴击伤害.
                //        XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.CheckPlayerBaoJiDengJi(AmmoType, PlayerState, healthScript);
                //        baoJiDamage = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetBaoJiDamage(PlayerState);
                //    }
                //}
                healthScript.OnDamageNpc(DamageNpc + baoJiDamage, PlayerState, AmmoType, IsAiFireAmmo);
                SpawnAmmoParticleObj(healthScript);
			}

            if (isStopCheckHit == true)
            {
                //停止子弹的伤害检测.
                if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
                {
                    //冲击波子弹不允许调用MoveAmmoOnCompelteITween.
                }
                else
                {
                    //SSDebug.LogWarning("MoveAmmoOnCompelteITween ============== 44444444444444444");
                    MoveAmmoOnCompelteITween();
                }
            }
		}

		if (hitObjNpc != null) {
			NpcAmmoCtrl npcAmmoScript = hitObjNpc.GetComponent<NpcAmmoCtrl>();
			if (npcAmmoScript != null) {
				npcAmmoScript.MoveAmmoOnCompelteITween();
            }

            if (AmmoType == PlayerAmmoType.DaoDanAmmo
                || AmmoType == PlayerAmmoType.GaoBaoAmmo
                || AmmoType == PlayerAmmoType.PuTongAmmo
                || AmmoType == PlayerAmmoType.SanDanAmmo
                || AmmoType == PlayerAmmoType.ChongJiBoAmmo)
            {
                if (hitObjNpc.layer != LayerMask.NameToLayer("Default"))
                {
                    //SSDebug.Log("hitObjNpc ===================================== " + hitObjNpc.name);
                    IsHitNpcAmmo = true;
                    if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
                    {
                        //冲击波子弹不允许在这里调用MoveAmmoOnCompelteITween,否则会被瞬间删除.
                    }
                    else
                    {
                        //SSDebug.LogWarning("MoveAmmoOnCompelteITween ============== 333333333333333");
                        MoveAmmoOnCompelteITween();
                    }
                    isStopCheckHit = true;
                }
            }
        }
		return isStopCheckHit;
	}

    //bool IsXiaoFeiJiAmmo = false;
    //internal void SetIsXiaoFeiJiAmmo(bool isXiaoFeiJiAmmo)
    //{
    //    IsXiaoFeiJiAmmo = isXiaoFeiJiAmmo;
    //}

    //static int TestChongJiBoCount = 0;
    public void StartMoveAmmo(Vector3 firePos,
        PlayerEnum playerIndex,
        XKPlayerAutoFire autoFireCom = null,
        NpcPathCtrl ammoMovePath = null,
        GameObject hitObjNpc = null)
	{
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有激活任何玩家.
            SetIsAiFireAmmo(true);
        }
        else
        {
            SetIsAiFireAmmo(false);
        }

        float disTmp = Vector3.Distance(firePos, AmmoTran.position);
		Vector3 vecA = firePos - AmmoTran.position;
		if (disTmp < 10f
			|| disTmp > MaxDisVal
		    || AmmoType == PlayerAmmoType.PaiJiPaoAmmo) {
			firePos = AmmoTran.position + (vecA.normalized * MaxDisVal);
			//Debug.Log("Unity:"+"StartMoveAmmo::fix firePos -> "+"disTmp "+disTmp+", disMax "+MaxDisVal);
		}
		IsChuanJiaDanHitCamColForward = false;
        IsCreatAmmoParticle = false;

        m_TimeLast = Time.time;
        ObjAmmo = gameObject;
        if (IsDestroyAmmo == true)
        {
            ObjAmmo.SetActive(true);
            IsDestroyAmmo = false;
            IsHitNpcAmmo = false;
        }

        AmmoTran = transform;
		PlayerState = playerIndex;
        if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
        {
            //冲击波子弹不用运动,只需要检测一次范围伤害.
            if (m_AmmmoData != null && autoFireCom != null)
            {
                m_AmmmoData.SetActiveAmmoCore(autoFireCom.CountAmmoStateZhuPao, autoFireCom.PlayerIndex);
            }
            CheckPlayerAmmoForwardHitNpc();
            //StartCoroutine(DelayRemoveChongJiBoAmmo());
            //TestChongJiBoCount++;
            //SSDebug.LogWarning("TestChongJiBoCount ============== " + TestChongJiBoCount + ", AmmoActive == " + gameObject.activeInHierarchy);
        }
        else
        {
            MoveAmmoByItween(firePos, ammoMovePath, autoFireCom);
        }
		IsHandleRpc = true;
	}

    /// <summary>
    /// 时间记录.
    /// </summary>
    float m_TimeLast = 0f;
    /// <summary>
    /// 持续检测是否可以隐藏冲击波子弹.
    /// </summary>
    void UpdateRemoveChongJiBoAmmo()
    {
        if (Time.time - m_TimeLast >= LiveTime && IsDestroyAmmo == false)
        {
            m_TimeLast = Time.time;
            //SSDebug.LogWarning("MoveAmmoOnCompelteITween ============== 222222222222222");
            MoveAmmoOnCompelteITween();
        }
    }

	void ResetTrailScriptInfo()
	{
		gameObject.SetActive(false);
		if (TrailScript == null) {
			return;
		}
		TrailScript.time = TrailTime;
	}

	Vector3 GetGenZongDanFirePos()
	{
		Vector3 vecA = AmmoTran.forward;
		Vector3 vecB = Vector3.forward;
		Vector3 posA = AmmoTran.position;
		Vector3 posB = Vector3.zero;
		Vector3 firePos = Vector3.zero;

		Transform[] npcArray = XkGameCtrl.GetInstance().GetNpcTranList().ToArray();
		int max = npcArray.Length;
		for (int i = 0; i < max; i++) {
			if (npcArray[i] == null) {
				continue;
			}
			
			posB = npcArray[i].position;
			vecA.y = posA.y = posB.y = 0f;
			vecB = posB - posA;
			if (Vector3.Dot(vecA, vecB.normalized) < CosABMin || vecB.magnitude > 50f) {
				continue;
			}
			firePos = npcArray[i].position;
			break;
		}
		return firePos;
	}
    
	GameObject PaiJiPaoTiShi;
	void MoveAmmoByItween(Vector3 firePos, NpcPathCtrl ammoMovePath, XKPlayerAutoFire autoFireCom)
    {
        //if (autoFireCom != null)
        //{
        //    SSDebug.Log("MoveAmmoByItween -> AmmoType ==== " + AmmoType + ", IsQianHouFire ==== " + autoFireCom.IsQianHouFire);
        //    SSDebug.Log("MoveAmmoByItween -> CountAmmoStateZhuPao ==== " + autoFireCom.CountAmmoStateZhuPao
        //        + ", CountAmmoStateJiQiang ==== " + autoFireCom.CountAmmoStateJiQiang);
        //}

        if (m_AmmmoData != null && autoFireCom != null)
        {
            switch (AmmoType)
            {
                case PlayerAmmoType.PaiJiPaoAmmo:
                case PlayerAmmoType.ChuanTouAmmo:
                case PlayerAmmoType.SanDanAmmo:
                case PlayerAmmoType.ChongJiBoAmmo:
                case PlayerAmmoType.DaoDanAmmo:
                    {
                        if (AmmoType == PlayerAmmoType.DaoDanAmmo)
                        {
                            //普通导弹子弹.
                            m_AmmmoData.SetActiveAmmoCore(0, autoFireCom.PlayerIndex);
                        }
                        else
                        {
                            //其它特殊导弹.
                            m_AmmmoData.SetActiveAmmoCore(autoFireCom.CountAmmoStateZhuPao, autoFireCom.PlayerIndex);
                        }
                        break;
                    }
                case PlayerAmmoType.PuTongAmmo:
                    {
                        if (autoFireCom.IsQianHouFire == true)
                        {
                            //小飞机发射的子弹.
                            m_AmmmoData.SetActiveAmmoCore(autoFireCom.CountAmmoStateJiQiang, autoFireCom.PlayerIndex);
                        }
                        break;
                    }
            }
        }

        if (ammoMovePath == null) {
			Vector3[] posArray = new Vector3[2];
			posArray[0] = AmmoTran.position;
			float lobTime = Vector3.Distance(firePos, posArray[0]) / MvSpeed;
			if (AmmoType == PlayerAmmoType.GenZongAmmo
			    || AmmoType == PlayerAmmoType.PaiJiPaoAmmo) {

				Vector3 posTmp = AmmoType == PlayerAmmoType.GenZongAmmo ? GetGenZongDanFirePos() : firePos;
				if (posTmp != Vector3.zero) {
					firePos = posTmp;
				}

				if (AmmoType == PlayerAmmoType.PaiJiPaoAmmo) {
					RaycastHit hit;
					Vector3 posA = firePos + Vector3.up * 50f;
					Physics.Raycast(posA, Vector3.down, out hit, 500f, XkGameCtrl.GetInstance().LandLayer);
					if (hit.collider != null) {
						firePos = hit.point;
					}
				}
				
				if (AmmoType == PlayerAmmoType.PaiJiPaoAmmo && PaiJiPaoTiShiPrefab != null) {
					PaiJiPaoTiShi = (GameObject)Instantiate(PaiJiPaoTiShiPrefab, firePos, Quaternion.identity);
					if (XkGameCtrl.NpcAmmoArray != null) {
						PaiJiPaoTiShi.transform.parent = XkGameCtrl.NpcAmmoArray;
                    }
                    
                    if (PaiJiPaoTiShi != null)
                    {
                        //迫击炮子弹提示.
                        SSPlayerAmmoTiShiData ammoTiShiDt = PaiJiPaoTiShi.GetComponent<SSPlayerAmmoTiShiData>();
                        if (ammoTiShiDt != null)
                        {
                            ammoTiShiDt.InitPlayerAmmo(PlayerState);
                        }
                    }
                }

				float disMV = Vector3.Distance(firePos , posArray[0]);
				lobTime = disMV / MvSpeed;
				float lobHeight = disMV * XKPlayerGlobalDt.GetInstance().KeyPaiJiPaoValPlayer;
                //lobHeight = lobHeight > 10f ? 10f : lobHeight;
                if (m_AmmmoData != null && autoFireCom != null)
                {
                    GameObject objCore = m_AmmmoData.GetAmmoCore(autoFireCom.CountAmmoStateZhuPao);
                    if (objCore != null)
                    {
                        AmmoCore = objCore;
                    }
                }
                AmmoCore.transform.localPosition = Vector3.zero;
				AmmoCore.transform.localEulerAngles = Vector3.zero;
				iTween.MoveBy(AmmoCore, iTween.Hash("y", lobHeight,
				                                    "time", lobTime/2,
				                                    "easeType", iTween.EaseType.easeOutQuad));
				iTween.MoveBy(AmmoCore, iTween.Hash("y", -lobHeight,
				                                    "time", lobTime/2,
				                                    "delay", lobTime/2,
				                                    "easeType", iTween.EaseType.easeInCubic));     
				//iTween.FadeTo(gameObject, iTween.Hash("delay", 3, "time", .5, "alpha", 0, "onComplete", "CleanUp"));
			}
			posArray[1] = firePos;
			//AmmoStartPos = AmmoTran.position;
			iTween.MoveTo(gameObject, iTween.Hash("position", firePos,
			                                      "time", lobTime,
			                                      "easeType", iTween.EaseType.linear,
			                                      "oncomplete", "MoveAmmoOnCompelteITween"));
		}
		else {
			int countMark = ammoMovePath.transform.childCount;
			Transform[] tranArray = ammoMovePath.transform.GetComponentsInChildren<Transform>();
			List<Transform> nodesTran = new List<Transform>(tranArray){};
			nodesTran.Remove(ammoMovePath.transform);
			transform.position = nodesTran[0].position;
			transform.rotation = nodesTran[0].rotation;
			firePos = nodesTran[countMark-1].position;
			//AmmoStartPos = nodesTran[countMark-2].position;
			iTween.MoveTo(ObjAmmo, iTween.Hash("path", nodesTran.ToArray(),
			                                   "speed", MvSpeed,
			                                   "orienttopath", true,
			                                   "easeType", iTween.EaseType.linear,
			                                   "oncomplete", "MoveAmmoOnCompelteITween"));
		}
		//AmmoEndPos = firePos;
	}

    /// <summary>
    /// 是否创建了子弹的爆炸粒子.
    /// </summary>
    bool IsCreatAmmoParticle = false;
    //static int TestCount = 0;
	void SpawnAmmoParticleObj(XKNpcHealthCtrl healthScript = null)
	{
        if (AmmoType != PlayerAmmoType.ChuanTouAmmo)
        {
            //不是穿甲弹.
            if (IsCreatAmmoParticle == true)
            {
                //已经创建过爆炸粒子.
                return;
            }
            IsCreatAmmoParticle = true;
        }

        //if (AmmoType == PlayerAmmoType.DaoDanAmmo)
        //{
        //    TestCount++;
        //    SSDebug.LogWarning("SpawnAmmoParticleObj -> healthScript ============= " + healthScript
        //        + ", TestCount == " + TestCount
        //        + ", time == " + Time.time.ToString("f3"));
        //}

        //if (TestCount >= 100)
        //{
        //    TestCount = 0;
        //}

#if USE_SPHERE_HIT
        GameObject objParticle = null;
		GameObject hitObj = CheckPlayerAmmoOverlapSphereHit(1);
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

        Vector3 pos = AmmoTran.position;
        Quaternion rot = AmmoTran.rotation;
        if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
        {
            if (healthScript != null)
            {
                //获取距离冲击波子弹最近的爆炸产生点.
                Transform tr = healthScript.GetAmmoLiZiMinDisSpawnPoint(transform);
                if (tr != null)
                {
                    //冲击波子弹.
                    pos = tr.position;
                    rot = tr.rotation;
                }
            }
            else
            {
                //冲击波子弹没有击中npc时不产生自爆粒子.
                return;
            }
        }
		GameObject obj = (GameObject)Instantiate(objParticle, pos, rot);
		Transform tran = obj.transform;
		tran.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);

        SSPlayerAmmoBaoJi baoJiAmmo = obj.GetComponent<SSPlayerAmmoBaoJi>();
        if (baoJiAmmo != null)
        {
            bool isDisplayBaoJi = false;
            if (healthScript != null)
            {
                if (healthScript.GetIsDaiJinQuanNpc() == true)
                {
                    //代金券npc.
                    isDisplayBaoJi = true;
                }
                
                //if (XkGameCtrl.GetInstance().m_SSDebugBaoJi != null)
                //{
                //    //测试爆击代金券npc.
                //    XkGameCtrl.GetInstance().m_SSDebugBaoJi.IsHitDaiJinQuanNpc = healthScript.GetIsDaiJinQuanNpc();
                //}
            }
            //else
            //{
            //    if (XkGameCtrl.GetInstance().m_SSDebugBaoJi != null)
            //    {
            //        //测试爆击代金券npc.
            //        XkGameCtrl.GetInstance().m_SSDebugBaoJi.IsHitDaiJinQuanNpc = false;
            //    }
            //}

            if (isDisplayBaoJi == true)
            {
                //初始化子弹爆炸粒子上的暴击效果.
                baoJiAmmo.Init(PlayerState);
            }
            else
            {
                //隐藏子弹爆炸粒子上的暴击效果.
                baoJiAmmo.HiddenBaoJi();
            }
        }
#else
		GameObject objParticle = null;
		GameObject obj = null;
		Transform tran = null;
		Vector3 hitPos = transform.position;
		RaycastHit hit;
		Vector3 forwardVal = Vector3.Normalize(AmmoEndPos - AmmoStartPos);
		if (AmmoType == PlayerAmmoType.PuTongAmmo) {
			float disVal = Vector3.Distance(AmmoEndPos, AmmoStartPos) + 10f;
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, PlayerAmmoHitLayer);
			if (hit.collider != null) {
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
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, PlayerAmmoHitLayer);
			if (hit.collider != null) {
				string tagHitObj = hit.collider.tag;
				switch (tagHitObj) {
				case "dirt":
					if (DirtParticle != null) {
						objParticle = DirtParticle;
					}
					break;
					
				case "water":
					if (WaterParticle != null) {
						objParticle = WaterParticle;
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
		
		hitPos = explodePos;
		switch (AmmoType) {
		case PlayerAmmoType.DaoDanAmmo:
			Vector3 AmmoPos = transform.position - (transform.forward * 3f);
			Physics.Raycast(AmmoPos, forwardVal, out hit, 13f, XkGameCtrl.GetInstance().LandLayer);
			if (hit.collider != null) {
				Vector3 normalVal = hit.normal;
				Quaternion rotVal = Quaternion.LookRotation(normalVal);
				obj = (GameObject)Instantiate(objParticle, hitPos, rotVal);
				obj.transform.up = normalVal;
			}
			else {
				obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
			}
			break;
		case PlayerAmmoType.ChuanTouAmmo:
			obj = (GameObject)Instantiate(objParticle, explodePos, transform.rotation);
			break;
		default:
			obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
			break;
		}
		tran = obj.transform;
		tran.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);
		
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
	
	void MoveAmmoOnCompelteITween()
	{
		if (IsDestroyAmmo) {
			return;
		}
		IsDestroyAmmo = true;

		if (NpcHealthList != null) {
			NpcHealthList.Clear();
			NpcHealthList = null;
		}

		if (PaiJiPaoTiShi != null) {
			Destroy(PaiJiPaoTiShi);
		}
        
		NpcAmmoCtrl.RemoveItweenComponents(gameObject);
		NpcAmmoCtrl.RemoveItweenComponents(gameObject, 1);
        if (AmmoType == PlayerAmmoType.Null)
        {
        }
        else
        {
            if (IsHitNpcAmmo == false)
            {
                CheckPlayerAmmoOverlapSphereHit();
            }
            //else
            //{
            //    SSDebug.Log("PlayerAmmo have hit npc! ************ AmmoType ===== " + AmmoType);
            //}
		}

        if (AmmoType != PlayerAmmoType.ChuanTouAmmo)
        {
            //创建子弹粒子.
            SpawnAmmoParticleObj();
        }
        DaleyHiddenPlayerAmmo();
    }

    bool _IsHitNpcAmmo = false;
    /// <summary>
    /// 是否击中NPC.
    /// </summary>
    bool IsHitNpcAmmo
    {
        set
        {
            _IsHitNpcAmmo = value;
            //if (_IsHitNpcAmmo == true)
            //{
            //    SSDebug.Log("******************************** IsHitNpcAmmo == " + _IsHitNpcAmmo);
            //}
        }
        get { return _IsHitNpcAmmo; }
    }
	void DaleyHiddenPlayerAmmo()
    {
        //if (AmmoType == PlayerAmmoType.ChongJiBoAmmo)
        //{
            //SSDebug.LogWarning("**** TestChongJiBoCount ============== " + TestChongJiBoCount + ", AmmoActive == " + gameObject.activeInHierarchy);
            //SSDebug.LogWarning("**** TestChongJiBoAmmo AmmoActive == " + gameObject.activeInHierarchy);
        //}
        gameObject.SetActive(false);
    }

	void CheckPlayerAmmoForwardHitNpc()
	{
		if (AmmoType == PlayerAmmoType.GenZongAmmo
		    || AmmoType == PlayerAmmoType.PaiJiPaoAmmo) {
			return;
		}
		CheckPlayerAmmoOverlapSphereHit();
	}
    
	bool IsChuanJiaDanHitCamColForward = false;
	/**
	 * key == 0 -> 检测子弹打中的物体.
	 * key == 1 -> 检测子弹打中的物体, 并且用来调用爆炸特效.
	 */
	GameObject CheckPlayerAmmoOverlapSphereHit(int key = 0)
	{
		GameObject obj = null;
#if !TEST_CLOSE_SPHER_HIT
        bool isBreak = false;
        float disDamage = key == 0 ? AmmoDamageDis : 0.5f;
		XKPlayerMvFanWei playerMvFanWei = null;
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, disDamage, PlayerAmmoHitLayer);
        Collider col = null;
        int length = hits.Length;
        //Debug.Log("length ======================== " + length);
        for (int i = 0; i < length; i++)
        {
            col = hits[i];
            if (col == null)
            {
                continue;
            }

            // Don't collide with triggers
            if (col.isTrigger || IsChuanJiaDanHitCamColForward)
            {
                continue;
            }

            if (AmmoType == PlayerAmmoType.PaiJiPaoAmmo || AmmoType == PlayerAmmoType.ChongJiBoAmmo)
            {
                //范围伤害类型子弹.
                //迫击炮和冲击波子弹需要忽略主角碰撞范围.
            }
            else 
            {
                //非范围伤害类型子弹.
                playerMvFanWei = col.GetComponent<XKPlayerMvFanWei>();
                if (playerMvFanWei != null)
                {
                    if (playerMvFanWei.FanWeiState == PointState.Hou)
                    {
                        continue;
                    }

                    if (playerMvFanWei.FanWeiState == PointState.Qian)
                    {
                        IsHitNpcAmmo = true;
                        if (AmmoType != PlayerAmmoType.ChuanTouAmmo)
                        {
                            isBreak = true;
                            obj = col.gameObject;
                            //SSDebug.LogWarning("MoveAmmoOnCompelteITween ============== 111111111111");
                            MoveAmmoOnCompelteITween();
                            break;
                        }
                        else
                        {
                            IsChuanJiaDanHitCamColForward = true;
                            break;
                        }
                    }
                }
            }

            switch (key)
            {
                case 0:
                    isBreak = CheckAmmoHitObj(col.gameObject, PlayerState);
                    if (isBreak)
                    {
                        break;
                    }
                    break;
                case 1:
                    isBreak = true;
                    obj = col.gameObject;
                    break;
            }
        }
#endif
        return obj;
	}
}