using UnityEngine;

public class XKNpcHealthCtrl : MonoBehaviour {
	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing; //控制主角所击杀npc的积分逻辑.
    /// <summary>
    /// 飘分点.
    /// </summary>
    public Transform m_PiaoFenPoint;
	[Range(0, 999999)] public int JiFenVal = 1;
	[Range(0f, 10000f)] public float PlayerDamage = 1f;

	/**
	 * MaxPuTongAmmo[0] -> 单人模式下.
	 * MaxPuTongAmmo[1] -> 双人模式下.
	 * MaxPuTongAmmo[2] -> 三人模式下.
	 * MaxPuTongAmmo[3] -> 四人模式下.
	 */
	[Range(1, 10000000)] public int[] MaxPuTongAmmo = {1, 1, 1, 1};
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
	internal XKNpcMoveCtrl NpcScript;
	XKCannonCtrl CannonScript;
	float TimeHitBoss;
	BoxCollider BoxColCom;
	bool IsSpawnObj;
	void Start()
	{
		CheckNpcRigidbody();
		if (NpcJiFen == NpcJiFenEnum.Boss) {
            if (XKBossXueTiaoCtrl.GetInstance() != null)
            {
                XKBossXueTiaoCtrl.GetInstance().SetBloodBossAmount(-1f, this);
            }
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

    float m_LastFanWeiHouTime = 0f;
    XKPlayerMvFanWei m_FanWeiHou;
    public bool IsHitFanWeiHou = false;
    //public static int TestNum = 0;
    //int TestNumRecord = 0;

    void Update()
	{
        //if (NpcScript != null && NpcScript.IsCaiPiaoZhanChe)
        //{
        //    //彩票战车或boss不进行检测.
        //    return;
        //}

        if (m_XKDaPaoCom != null && m_XKDaPaoCom.SpawnPointScript == null)
        {
            if (Time.time - m_LastFanWeiHouTime > 1f)
            {
                m_LastFanWeiHouTime = Time.time;
                if (!IsDeathNpc)
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
        }

        if (NpcScript == null && CannonScript != null)
        {
            if (Time.time - m_LastFanWeiHouTime > 1f)
            {
                m_LastFanWeiHouTime = Time.time;
                if (!IsDeathNpc)
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
        }

		if (Time.time - TimeLastVal < 10f) {
			return;
		}
		TimeLastVal = Time.time;

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
            ShowPiaoFen(playerScript.PlayerIndex);
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

    /// <summary>
    /// npc彩票显示组件.
    /// </summary>
    public SSCaiPiaoNpcUI m_CaiPiaoNpcUI;
	public void SetNpcMoveScript(XKNpcMoveCtrl script)
	{
		IsSpawnObj = true;
		NpcScript = script;
		if (NpcScript != null && NpcJiFen == NpcJiFenEnum.Boss) {
			NpcScript.SetIsBossNpc(true);
		}
		NpcNameInfo = script.name;
		ResetNpcHealthInfo();

        if (m_CaiPiaoNpcUI != null)
        {
            if (NpcScript != null)
            {
                if (NpcScript.IsCaiPiaoZhanChe == true)
                {
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                    {
                        //获取获取JPBoss和战车Npc的血值数据.
                        XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetTotalHealData(NpcScript.m_DaiJinQuanState);
                    }

                    if (NpcScript.GetIsBossNpc() == true)
                    {
                        m_CaiPiaoNpcUI.ShowNumUI(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss, this);
                        if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                        {
                            //跟新JPBoss的血值数据.
                            MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurentTotalHealthDt.JPBossHealth.MaxPuTongAmmo;
                        }
                    }
                    else
                    {
                        m_CaiPiaoNpcUI.ShowNumUI(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe, this);
                        if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                        {
                            //跟新战车Npc的血值数据.
                            MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurentTotalHealthDt.ZhanCheHealth.MaxPuTongAmmo;
                        }
                    }

                    //创建代金券npc的血条信息.
                    float perVal = 0.5f;
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                    {
                        perVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurentTotalHealthDt.UIHealthPer;
                    }
                    SSUIRoot.GetInstance().m_GameUIManage.CreatDaiJinQuanNpcXueTiaoUI(perVal);
                }
                NpcScript.m_CaiPiaoNpcUI = m_CaiPiaoNpcUI;
            }
        }
        TimeLastVal = Time.time;
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

    /// <summary>
    /// 获取是否为代金券Npc.
    /// </summary>
    internal bool GetIsDaiJinQuanNpc()
    {
        if (NpcScript != null)
        {
            return NpcScript.IsCaiPiaoZhanChe;
        }
        return false;
    }

	string NpcNameInfo = "";
	XKNpcDamageCtrl NpcDamageCom;
	int CountActivePlayer;
    /// <summary>
    /// 玩家主炮散弹对npc造成伤害的时间记录信息.
    /// </summary>
    float TimeSanDanDamage = 0f;
    /// <summary>
    /// 不允许被击爆的代金券npc血值是否降到10%
    /// </summary>
    bool IsBloodTo_10 = false;
    /// <summary>
    /// 血值降到10%之后的时间记录.
    /// </summary>
    float m_TimeLastBloodTo_10 = 0f;
    /// <summary>
    /// npc的血值信息记录.
    /// </summary>
    float m_BloodAmoutValue = 0f;
	public void OnDamageNpc(int damageNpcVal = 1,
                            PlayerEnum playerSt = PlayerEnum.Null,
	                        PlayerAmmoType pAmmoType = PlayerAmmoType.Null,
                            bool isAiFireAmmo = false)
	{
		if (IsDeathNpc)
        {
			return;
		}

        if (pAmmoType == PlayerAmmoType.SanDanAmmo)
        {
            if (Time.time - TimeSanDanDamage < XKPlayerGlobalDt.GetInstance().TimeShouDongDaoDan)
            {
                //对于玩家的主炮散弹在同一次发射内只计算一次伤害.
                //SSDebug.LogWarning("************* pAmmoType ====== " + pAmmoType);
                return;
            }
            TimeSanDanDamage = Time.time;
        }

        //switch (NpcJiFen)
        //{
        //    case NpcJiFenEnum.Boss:
        //        if (!XKBossXueTiaoCtrl.GetInstance().GetIsCanSubXueTiaoAmount())
        //        {
        //            if (NpcDamageCom != null)
        //            {
        //                NpcDamageCom.PlayNpcDamageEvent();
        //            }
        //            return;
        //        }
        //        break;
        //}


        if (NpcDamageCom != null)
        {
            NpcDamageCom.PlayNpcDamageEvent();
        }

        if (NpcScript != null)
        {
            if (NpcScript.IsCaiPiaoZhanChe == true && NpcScript.IsEnterCameraBox == false)
            {
                //彩票战车和boss没有进入摄像机盒子,不计算伤害.
                return;
            }
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

		if (NpcScript == null || (NpcScript != null && !NpcScript.GetIsWuDi()))
        {
			PuTongAmmoCount += damageNpcVal;
		}

		int indexVal = XkGameCtrl.PlayerActiveNum - 1;
		indexVal = indexVal < 0 ? 0 : indexVal;
		int puTongAmmoNum = MaxPuTongAmmo[indexVal];
        if (NpcScript != null)
        {
            if (NpcScript.IsZhanCheNpc || NpcScript.IsJPBossNpc)
            {
                if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                {
                    //当没有玩家激活游戏时,彩票战车和JPBoss采用原血量的一定比例来计算.
                    puTongAmmoNum = (int)(XkGameCtrl.GetInstance().m_ZhanCheBossBloodPer * puTongAmmoNum);
                }
            }
        }

        /*if (NpcJiFen == NpcJiFenEnum.Boss)
        {
			float bossAmount = (float)(puTongAmmoNum - PuTongAmmoCount) / puTongAmmoNum;
			bossAmount = bossAmount < 0f ? 0f : bossAmount;
            if (XKBossXueTiaoCtrl.GetInstance() != null)
            {
                XKBossXueTiaoCtrl.GetInstance().SetBloodBossAmount(bossAmount, this);
            }
		}*/
        
        if (NpcScript != null)
        {
            if (NpcScript.IsCaiPiaoZhanChe == true)
            {
                float bloodAmount = (float)(puTongAmmoNum - PuTongAmmoCount) / puTongAmmoNum;
                bloodAmount = bloodAmount < 0f ? 0f : bloodAmount;

                bool isCanJiBao = true;
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                {
                    isCanJiBao = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurentTotalHealthDt.IsCanJiBao;
                }

                if (isCanJiBao == false)
                {
                    //不允许被玩家击爆的代金券npc.
                    if (IsBloodTo_10 == false)
                    {
                        if (bloodAmount <= 0.1f)
                        {
                            //强制保留一定的血量.
                            m_BloodAmoutValue = bloodAmount = 0.1f;
                            IsBloodTo_10 = true;
                            m_TimeLastBloodTo_10 = Time.time;
                            PuTongAmmoCount = puTongAmmoNum - 1;
                        }
                    }
                    else
                    {
                        float minBloodAmount = 0.01f; //最小极限血值.
                        if (m_BloodAmoutValue > minBloodAmount)
                        {
                            //血值已经降到10%
                            float dTimeVal = 0.3f;
                            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                            {
                                dTimeVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TimeNoDead;
                            }

                            if (Time.time - m_TimeLastBloodTo_10 >= dTimeVal)
                            {
                                //每次间隔一定时间减少一定血值.
                                m_TimeLastBloodTo_10 = Time.time;
                                float subBloodAmount = 0.01f; //每次减少的血值.
                                if (m_BloodAmoutValue > minBloodAmount)
                                {
                                    m_BloodAmoutValue -= subBloodAmount;
                                    if (m_BloodAmoutValue < minBloodAmount)
                                    {
                                        //强制保护的血值信息.
                                        m_BloodAmoutValue = minBloodAmount;
                                    }
                                }
                            }
                        }
                        bloodAmount = m_BloodAmoutValue;
                        PuTongAmmoCount = puTongAmmoNum - 1; //强制保留一定的血值.
                    }
                }
                //彩票战车和boss的血条UI信息更新.
                SSUIRoot.GetInstance().m_GameUIManage.SetDaiJinQuanNpcXueTiaoAmount(bloodAmount);
            }
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

                    if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                    {
                        //没有激活任何玩家.
                    }
                    else
                    {
                        if (!XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsChuCaiPiaoByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe, NpcScript.m_DaiJinQuanState))
                        {
                            //战车彩池的彩票积累的不够.
                            return;
                        }
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
                    
                    if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                    {
                        //没有激活任何玩家.
                    }
                    else
                    {
                        if (!XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsChuCaiPiaoByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss))
                        {
                            //JPBoss彩池的彩票积累的不够.
                            return;
                        }
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
					if (GameTimeBossCtrl.GetInstance() != null
                            && GameTimeBossCtrl.GetInstance().GetTimeBossResidual() > 0) {
						XkGameCtrl.GetInstance().AddPlayerKillNpc(PlayerEnum.Null, NpcJiFen, JiFenVal);
					}
					break;
				default:
					XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFen, JiFenVal);
                    ShowPiaoFen(playerSt);
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
                        if (isAiFireAmmo == false)
                        {
                            //只有被玩家击爆的代金券npc才允许出彩.
                            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SubGameDeCaiValByDeCaiState(playerSt, deCaiType,
                                SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang, NpcScript.m_DaiJinQuanState);
                        }
                        else
                        {
                            //被Ai坦克击爆的代金券npc不允许出彩.
                            SSDebug.Log("The DaiJinQuan was killed by AiTank");
                        }
                    }
                }
                else
                {
                    //普通npc被击杀.
                    if (XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData != null && DeathExplodePoint != null)
                    {
                        //随机送出正常得彩.
                        XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData.CheckPlayerSongPiaoInfo(playerSt, DeathExplodePoint.position);
                    }
                }
            }
			else if (CannonScript != null)
            {
				CannonScript.OnRemoveCannon(playerSt, 1);
                //炮台类npc被击杀.
                if (XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData != null && DeathExplodePoint != null)
                {
                    //随机送出正常得彩.
                    XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData.CheckPlayerSongPiaoInfo(playerSt, DeathExplodePoint.position);
                }
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
        TimeLastVal = Time.time;
        IsHitFanWeiHou = false;
        IsBloodTo_10 = false;
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
                int value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(deCaiType, SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang,
                    NpcScript.m_DaiJinQuanState);
                if (DeathExplodePoint != null)
                {
                    //Vector3 pos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(objExplode.transform.position);
                    SSUIRoot.GetInstance().m_GameUIManage.CreatZhanCheBossCaiPiaoZhuanPan(indexPlayer, value, DeathExplodePoint.position, deCaiType, DeathExplode);
                    //SSCaiPiaoLiZiManage caiPiaoLiZi = objExplode.GetComponent<SSCaiPiaoLiZiManage>();
                    //if (caiPiaoLiZi != null)
                    //{
                    //    caiPiaoLiZi.ShowNumUI(value, indexPlayer);
                    //}
                    //else
                    //{
                    //    Debug.LogWarning("CheckNpcDeathExplode -> caiPiaoLiZi was null.................");
                    //}
                    if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
                    {
                        //只给boss产生爆炸粒子.
                        GameObject objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
                        objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
                        XkGameCtrl.CheckObjDestroyThisTimed(objExplode);

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
            }
            
            //if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe)
            //{
            //    if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            //    {
            //        //初始化飞出的彩票逻辑.
            //        XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitCaiPiaoFly(transform.position, indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe);
            //    }
            //    else
            //    {
            //        Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
            //    }
            //}
            //else if (deCaiType == SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss)
            //{
            //    if (SSUIRoot.GetInstance().m_GameUIManage != null)
            //    {
            //        SSUIRoot.GetInstance().m_GameUIManage.InitCaiPiaoAnimation(XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JPBossCaiPiaoFlyDt.TimeLeiJiaVal, indexPlayer);
            //    }

            //    if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            //    {
            //        //初始化烟花粒子的产生.
            //        XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitPlayCaiPiaoYanHua();
            //    }
            //    else
            //    {
            //        Debug.LogWarning("CreatLiZi -> m_CaiPiaoFlyData was null............");
            //    }
            //}
        }
        else
        {
            GameObject objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
            objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
            XkGameCtrl.CheckObjDestroyThisTimed(objExplode);
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

    void ShowPiaoFen(PlayerEnum indexPlayer)
    {
        if (JiFenVal <= 0)
        {
            return;
        }

        if (m_PiaoFenPoint == null)
        {
            return;
        }

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.ShowNpcPiaoFenUI(indexPlayer, JiFenVal, m_PiaoFenPoint.position);
        }
    }
}