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
	[Range(1, 10000000)] public int[] MaxPuTongAmmo = { 1, 1, 1, 1 };
    /// <summary>
    /// 血值缓存信息.
    /// </summary>
    int[] MaxPuTongAmmoCache = { 1, 1, 1, 1 };
    /// <summary>
    /// 是否记录了血值信息.
    /// </summary>
    bool IsRecordMaxPuTongAmmo = false;
//	[Range(0, 100)] public int MaxAmmoHurtLiZi = 0;
	public GameObject[] HiddenNpcObjArray; //npc死亡时需要立刻隐藏的对象.
//	public GameObject HurtLiZiObj; //飞机npc的受伤粒子.
    /// <summary>
    /// 死亡爆炸粒子.
    /// </summary>
	public GameObject DeathExplode;
    /// <summary>
    /// 死亡爆炸产生点.
    /// </summary>
	public Transform DeathExplodePoint;
    /// <summary>
    /// 玩家子弹爆炸粒子产生点数组.
    /// </summary>
    public Transform[] AmmoLiZiPointArray;
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

    /// <summary>
    /// 获取积分.
    /// </summary>
    int GetJiFen()
    {
        //1位玩家时目前得分不变,2位玩家时得分变为原来的1.5倍,3位玩家时得分变为原来的2倍.
        float fenZhiBeiLv = 1f;
        switch (XkGameCtrl.PlayerActiveNum)
        {
            case 2:
                {
                    fenZhiBeiLv = 1.5f;
                    break;
                }
            case 3:
                {
                    fenZhiBeiLv = 2f;
                    break;
                }
        }
        int jiFen = (int)(JiFenVal * fenZhiBeiLv);
        return jiFen;
    }

    /// <summary>
    /// 获取积分.
    /// </summary>
    int GetJiFen(int jiFenVal)
    {
        //1位玩家时目前得分不变,2位玩家时得分变为原来的1.5倍,3位玩家时得分变为原来的2倍.
        float fenZhiBeiLv = 1f;
        switch (XkGameCtrl.PlayerActiveNum)
        {
            case 2:
                {
                    fenZhiBeiLv = 1.5f;
                    break;
                }
            case 3:
                {
                    fenZhiBeiLv = 2f;
                    break;
                }
        }
        int jiFen = (int)(jiFenVal * fenZhiBeiLv);
        return jiFen;
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
			XkGameCtrl.GetInstance().AddPlayerKillNpc(playerScript.PlayerIndex, NpcJiFen, GetJiFen());
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
    /// 是否重置了代金券npc的血值.
    /// </summary>
    bool IsBackDaiJinQuanNpcBlood = false;
    /// <summary>
    /// 重置血值的时间记录信息.
    /// </summary>
    float m_TimeLastBackDaiJinQuanNpcBlood = 0f;
    /// <summary>
    /// 恢复代金券npc的血值数据及UI信息.
    /// </summary>
    internal void BackDaiJinQuanNpcBlood()
    {
        if (NpcScript.IsZhanCheNpc || NpcScript.IsJPBossNpc)
        {
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == false)
            {
                //当有玩家激活游戏时,恢复彩票战车和JPBoss的血值信息.
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                {
                    if (NpcScript.GetIsBossNpc() == true)
                    {
                        //跟新JPBoss的血值数据.
                        //MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurentTotalHealthDt.JPBossHealth.MaxPuTongAmmo;
                        //SSDebug.Log("*********************************************************44444444444444444444444444444444");
                        SetJPBossHealthInfo(NpcScript);
                    }
                    else
                    {
                        //跟新战车Npc的血值数据.
                        MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.ZhanCheHealth.MaxPuTongAmmo;
                    }
                }

                //重置代金券npc的伤害数值.
                PuTongAmmoCount = 0;
                IsBackDaiJinQuanNpcBlood = true;
                m_TimeLastBackDaiJinQuanNpcBlood = Time.time;

                //重置代金券npc的血条UI信息.
                float perVal = 0.5f;
                if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                {
                    perVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.UIHealthPer;
                    //重置Boss为不可以被击爆,避免玩家击爆AI坦克刷出的可以被击爆的Boss.
                    XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.IsCanJiBao = false;
                }
                SSUIRoot.GetInstance().m_GameUIManage.BackBloodBossAmount(perVal);
            }
        }
    }

    internal bool IsGetTotalHealthData = false;
    /// <summary>
    /// 重置信息.
    /// </summary>
    internal void ResetIsGetTotalHealthData()
    {
        IsGetTotalHealthData = false;
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
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null && IsGetTotalHealthData == false)
                    {
                        IsGetTotalHealthData = true;
                        //获取获取JPBoss和战车Npc的血值数据.
                        if (NpcScript.IsJPBossNpc == true)
                        {
                            //JPBoss战车.
                            XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetTotalHealthData(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan);
                        }
                        else
                        {
                            //战车01或02.
                            XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.GetTotalHealthData(NpcScript.m_DaiJinQuanState);
                        }
                        //保存代金券npc的血条脚本.
                        XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.SaveDaiJinQuanHealth(this);
                    }
                    
                    SetRecordMaxPuTongAmmo();
                    if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                    {
                        //没有玩家激活游戏,使用游戏记录的血值数据.
                        MaxPuTongAmmo = MaxPuTongAmmoCache;
                    }
                    else
                    {
                        //有玩家正在进行游戏,使用游戏配置的血值数据.
                        if (NpcScript.GetIsBossNpc() == true)
                        {
                            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                            {
                                //跟新JPBoss的血值数据.
                                MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.JPBossHealth.MaxPuTongAmmo;
                                //SSDebug.Log("*********************************************************3333333333333333333333333333333");
                            }
                        }
                        else
                        {
                            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                            {
                                //跟新战车Npc的血值数据.
                                MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.ZhanCheHealth.MaxPuTongAmmo;
                            }
                        }
                    }

                    if (NpcScript.GetIsBossNpc() == true)
                    {
                        m_CaiPiaoNpcUI.ShowNumUI(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss, this);
                    }
                    else
                    {
                        m_CaiPiaoNpcUI.ShowNumUI(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe, this);
                    }

                    //创建代金券npc的血条信息.
                    float perVal = 0.5f;
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                    {
                        perVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.UIHealthPer;
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
    /// 不允许被击爆的代金券npc血值是否降到阶段02.
    /// </summary>
    bool IsBloodToStage02 = false;
    /// <summary>
    /// 不允许被击爆的代金券npc血值是否降到阶段01.
    /// </summary>
    bool IsBloodToStage01 = false;
    /// <summary>
    /// 血值降到某阶段之后的时间记录.
    /// </summary>
    float m_TimeLastBloodToStage = 0f;
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

		if (pAmmoType == PlayerAmmoType.ChuanTouAmmo)
		{
			if (XKPlayerMvFanWei.GetInstanceQian() != null && XKPlayerCamera.GetInstanceFeiJi() != null)
			{
				Vector3 forwardVecCam = XKPlayerCamera.GetInstanceFeiJi().transform.forward;
				Vector3 posA = transform.position;
				Vector3 posB = XKPlayerMvFanWei.GetInstanceQian().transform.position;
				forwardVecCam.y = posA.y = posB.y = 0f;
				Vector3 vecBA = posA - posB;
				if (Vector3.Dot(forwardVecCam, vecBA) >= 0f)
				{
					//npc在镜头前方范围之外,不受玩家子弹伤害.
					return;
				}
			}
		}

        if (IsBackDaiJinQuanNpcBlood == true)
        {
            //代金券npc重置血值后,给一定时间的无敌状态.
            if (Time.time - m_TimeLastBackDaiJinQuanNpcBlood <= 0.5f)
            {
                return;
            }
            else
            {
                IsBackDaiJinQuanNpcBlood = false;
            }
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
                if (XkGameCtrl.GetInstance() != null && XKPlayerFenShuCtrl.GetInstance() != null)
                {
                    if(pAmmoType == PlayerAmmoType.DaoDanAmmo
                        || pAmmoType == PlayerAmmoType.PaiJiPaoAmmo
                        || pAmmoType == PlayerAmmoType.ChuanTouAmmo
                        || pAmmoType == PlayerAmmoType.SanDanAmmo
                        || pAmmoType == PlayerAmmoType.ChongJiBoAmmo)
                    {
                        int jiFenVal = XkGameCtrl.GetInstance().m_DaoDanHitBossJiFen;
                        //XKPlayerFenShuCtrl.GetInstance().ShowPlayerFenShu(playerSt, GetJiFen(jiFenVal));
                        XKPlayerFenShuCtrl.GetInstance().ShowPlayerFenShu(playerSt, jiFenVal);
                    }
                }

                float bloodAmount = (float)(puTongAmmoNum - PuTongAmmoCount) / puTongAmmoNum;
                bloodAmount = bloodAmount < 0f ? 0f : bloodAmount;

                bool isCanJiBao = true;
                if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                {
                    //没有玩家激活游戏时,Ai都认为是可以击爆战车或Boss.
                }
                else
                {
                    //有玩家激活游戏.
                    if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                    {
                        isCanJiBao = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.IsCanJiBao;
                    }
                }
                
                if (isCanJiBao == false)
                {
                    //不允许被玩家击爆的代金券npc.
                    if (IsBloodToStage02 == false)
                    {
                        if (bloodAmount <= 0.24f)
                        {
                            //强制保留一定的血量.
                            m_BloodAmoutValue = bloodAmount = 0.24f;
                            IsBloodToStage02 = true;
                            m_TimeLastBloodToStage = Time.time;
                            PuTongAmmoCount = puTongAmmoNum - 1;
                        }
                    }
                    else if (IsBloodToStage01 == false)
                    {
                        float minBloodAmount = 0.12f; //最小极限血值.
                        if (m_BloodAmoutValue > minBloodAmount)
                        {
                            //血值已经降到最低阶段.
                            float dTimeVal = 0.4f;
                            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                            {
                                dTimeVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TimeNoDead;
                            }

                            if (Time.time - m_TimeLastBloodToStage >= dTimeVal)
                            {
                                //每次间隔一定时间减少一定血值.
                                m_TimeLastBloodToStage = Time.time;
                                float subBloodAmount = 0.02f; //每次减少的血值.
                                if (m_BloodAmoutValue > minBloodAmount)
                                {
                                    m_BloodAmoutValue -= subBloodAmount;
                                    if (m_BloodAmoutValue < minBloodAmount)
                                    {
                                        //强制保护的血值信息.
                                        m_BloodAmoutValue = minBloodAmount;
                                        IsBloodToStage01 = true;
                                    }
                                }
                            }
                        }
                        bloodAmount = m_BloodAmoutValue;
                        PuTongAmmoCount = puTongAmmoNum - 1; //强制保留一定的血值.
                    }
                    else
                    {
                        float minBloodAmount = 0.03f; //最小极限血值.
                        if (m_BloodAmoutValue > minBloodAmount)
                        {
                            //血值已经降到最低阶段.
                            float dTimeVal = 0.4f;
                            if (XkGameCtrl.GetInstance().m_CaiPiaoHealthDt != null)
                            {
                                dTimeVal = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_TimeNoDead;
                            }

                            if (Time.time - m_TimeLastBloodToStage >= dTimeVal)
                            {
                                //每次间隔一定时间减少一定血值.
                                m_TimeLastBloodToStage = Time.time;
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

                if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
                {
                    //没有玩家激活游戏时,Ai都认为是不可以击爆JPBoss特殊武器的.
                }
                else
                {
                    //JPBoss受到玩家子弹伤害.
                    if (NpcScript.IsJPBossNpc == true)
                    {
                        NpcScript.OnDamageJPBossWeapon(bloodAmount);
                    }
                }
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
						XkGameCtrl.GetInstance().AddPlayerKillNpc(PlayerEnum.Null, NpcJiFen, GetJiFen());
					}
					break;
				default:
					XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFen, GetJiFen());
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
                    else
                    {
                        if (XkPlayerCtrl.GetInstanceFeiJi() != null)
                        {
                            XkPlayerCtrl.GetInstanceFeiJi().AddGetZhanCheDaiJinQuanPlayer(XKPlayerMoveCtrl.GetInstance(playerSt));
                        }
                    }

                    if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
                    {
                        if (isAiFireAmmo == false)
                        {
                            //只有被玩家击爆的代金券npc才允许出彩.
                            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.SubGameDeCaiValByDeCaiState(playerSt, deCaiType,
                                SSCaiPiaoDataManage.SuiJiDaoJuState.BaoXiang, NpcScript.m_DaiJinQuanState);
                        }
                        //else
                        //{
                            //被Ai坦克击爆的代金券npc不允许出彩.
                            //SSDebug.Log("The DaiJinQuan was killed by AiTank");
                        //}
                    }
                }
                else
                {
                    //普通npc被击杀.
                    //if (XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData != null && DeathExplodePoint != null)
                    //{
                    //    //随机送出正常得彩.
                    //    XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData.CheckPlayerSongPiaoInfo(playerSt, DeathExplodePoint.position);
                    //}
                }
            }
			else if (CannonScript != null)
            {
				CannonScript.OnRemoveCannon(playerSt, 1);
                //炮台类npc被击杀.
                //if (XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData != null && DeathExplodePoint != null)
                //{
                //    //随机送出正常得彩.
                //    XkGameCtrl.GetInstance().m_PlayerJiChuCaiPiaoData.CheckPlayerSongPiaoInfo(playerSt, DeathExplodePoint.position);
                //}
            }
		}
	}

	void CheckSpawnDaoJuCom(PlayerEnum index)
	{
        if (XkGameCtrl.GetInstance().GetIsActiveAiPlayer() == true)
        {
            //Ai坦克被激活时,npc被击爆后不允许产生道具.
            return;
        }

		XKNpcSpawnDaoJu daoJuScript = GetComponent<XKNpcSpawnDaoJu>();
		if (daoJuScript == null) {
			return;
		}
        //daoJuScript.CreatSuiJiDaoJu(index);

        if (NpcScript != null && NpcScript.IsCaiPiaoZhanChe == false)
        {
            daoJuScript.SpawnAllDaoJu(index);
        }
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
        IsBloodToStage02 = false;
        IsBloodToStage01 = false;
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
                    SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuanType = NpcScript.m_DaiJinQuanState;
                    SSUIRoot.GetInstance().m_GameUIManage.CreatZhanCheBossCaiPiaoZhuanPan(indexPlayer, value, DeathExplodePoint.position, deCaiType, DeathExplode, daiJinQuanType);
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
            SSUIRoot.GetInstance().m_GameUIManage.ShowNpcPiaoFenUI(indexPlayer, GetJiFen(), m_PiaoFenPoint.position);
        }
    }

    /// <summary>
    /// 记录血值信息.
    /// </summary>
    void SetRecordMaxPuTongAmmo()
    {
        if (IsRecordMaxPuTongAmmo == false)
        {
            //记录血值信息.
            IsRecordMaxPuTongAmmo = true;
            MaxPuTongAmmoCache = MaxPuTongAmmo;
        }
    }

    internal void SetJPBossHealthInfo(XKNpcMoveCtrl npcMoveCom)
    {
        if (npcMoveCom != null)
        {
            SetRecordMaxPuTongAmmo();
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                //没有玩家激活游戏,使用游戏记录的血值数据.
                MaxPuTongAmmo = MaxPuTongAmmoCache;
            }
            else
            {
                switch (npcMoveCom.m_TriggerDir)
                {
                    case SSTriggerCaiPiaoBossMove.TriggerDir.Qian:
                    case SSTriggerCaiPiaoBossMove.TriggerDir.Hou:
                        {
                            //SSDebug.Log("*********************************************************1111111111111111111");
                            MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.JPBossHealth.MaxPuTongAmmo;
                            break;
                        }
                    case SSTriggerCaiPiaoBossMove.TriggerDir.Zuo:
                    case SSTriggerCaiPiaoBossMove.TriggerDir.You:
                        {
                            //SSDebug.Log("*********************************************************22222222222222222222222222");
                            MaxPuTongAmmo = XkGameCtrl.GetInstance().m_CaiPiaoHealthDt.m_CurrentTotalHealthDt.JPBossHealthHengXiang.MaxPuTongAmmo;
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// 获取距离子弹最近的爆炸粒子产生点.
    /// </summary>
    public Transform GetAmmoLiZiMinDisSpawnPoint(Transform ammoTr)
    {
        if (ammoTr == null)
        {
            return null;
        }

        Transform point = null;
        float disMax = 10000f;
        float dis = 0f;
        Vector3 posA = Vector3.zero;
        Vector3 posB = Vector3.zero;
        if (DeathExplodePoint != null)
        {
            posA = ammoTr.position;
            posB = DeathExplodePoint.position;
            posA.y = posB.y = 0f;
            dis = Vector3.Distance(posA, posB);
            if (dis < disMax)
            {
                disMax = dis;
                point = DeathExplodePoint;
            }
        }

        if (AmmoLiZiPointArray.Length > 0)
        {
            for (int i = 0; i < AmmoLiZiPointArray.Length; i++)
            {
                if (AmmoLiZiPointArray[i] != null)
                {
                    posA = ammoTr.position;
                    posB = AmmoLiZiPointArray[i].position;
                    posA.y = posB.y = 0f;
                    dis = Vector3.Distance(posA, posB);
                    if (dis < disMax)
                    {
                        disMax = dis;
                        point = AmmoLiZiPointArray[i];
                    }
                }
            }
        }
        return point;
    }
}