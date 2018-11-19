using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKCannonCtrl : MonoBehaviour {
	public GameObject DaPaoAmmo;
	public AudioSource AudioCannonFire;
	public GameObject DaPaoAmmoLiZi;
	public Transform PaoGuan;
	public Transform[] SpawnAmmoPoint;
	[Range(-1f, 1000f)] public float FireDis = 50f;
	[Range(0f, 100f)] public float TimeFireDelay = 0f; //开火延迟时间.
	float TimeStartSpawn;
	//炮管角度控制.
	[Range(-90f, 90f)]public float UpPaoGuanJDVal = -45f;
	[Range(-90f, 90f)]public float DownPaoGuanJDVal = 45f;
	//炮管旋转速度.
	[Range(0.000001f, 100f)]public float PaoGuanSDVal = 2f;
	//炮管伸缩速度.
	[Range(0.03f, 1f)]public float PaoGuanShenSuoSD = 0.03f;
	//炮身角度控制.
	[Range(0f, 180f)]public float MaxPaoShenJDVal = 10f;
	[Range(0f, -180f)]public float MinPaoShenJDVal = -10f;
	//炮身旋转速度.
	[Range(0.000001f, 100f)]public float PaoShenSDVal = 2f;
	[Range(0f, 100f)]public float PaoGuanZhenFu = 0.9f;
	//单管大炮做fire的间隔时间.
	[Range(0.001f, 100f)]public float TimeDanGuanFire = 1f;
	//多管大炮各个炮管产生子弹的间隔时间.
	[Range(0.03f, 10f)] public float TimeAmmoUnit = 0.1f;
	//多管炮弹每次发射炮弹的间隔时间.
	[Range(0.001f, 50f)] public float TimeAmmoWait = 1f;
	//npc的删除延迟时间.
	[Range(0f, 50f)] public float TimeDestroy = 0f;
	public NpcPathCtrl AmmoMovePath; //拍摄循环动画时，使子弹可以做抛物线运动.
	Transform CannonTran;
	bool IsDoFireAnimation;
	bool IsDeathNpc;
	bool IsStopAnimation;
	bool IsDouGuanDaPao;
	bool IsPlayPaoGuanAnimation;
	bool IsOutputError = false;
	XKPlayerMoveCtrl PlayerMoveScript;
	// Use this for initialization
	void Awake()
	{
		//if (pcvr.TKMoveSt == TKMoveState.U_FangXiangPan) {
		//	TimeDanGuanFire *= XkGameCtrl.GetInstance().NpcAmmoTime;
		//	TimeAmmoWait *= XkGameCtrl.GetInstance().NpcAmmoTime;
		//}

		if (AudioCannonFire != null) {
			AudioCannonFire.playOnAwake = false;
			AudioCannonFire.Stop();
		}
	}

	void Start()
	{
		TimeStartSpawn = Time.realtimeSinceStartup;
		if (XkGameCtrl.PlayerActiveNum > 0) {
			GetAimPlayerMoveScript();
		}

		foreach (var item in SpawnAmmoPoint) {
			item.gameObject.SetActive(true);
		}

		if (AudioCannonFire != null) {
			AudioCannonFire.Stop();
		}

		if (SpawnAmmoPoint.Length <= 0) {
			//Debug.LogWarning("Unity:"+"XKCannonCtrl -> SpawnAmmoPoint was wrong!");
		}
		else {
			for (int i = 0; i < SpawnAmmoPoint.Length; i ++) {
				if (SpawnAmmoPoint[i] == null) {
					Debug.LogWarning("Unity:"+"XKCannonCtrl -> SpawnAmmoPoint was wrong! index = "+i);
					IsOutputError = true;
					break;
				}
			}
		}
		CosAngleUp = Mathf.Cos((UpPaoGuanJDVal / 180f) * Mathf.PI);
		CosAngleDown = Mathf.Cos((DownPaoGuanJDVal / 180f) * Mathf.PI);

		CannonTran = transform;
		if (HealthScript == null) {
			HealthScript = GetComponent<XKNpcHealthCtrl>();
		}

		if (HealthScript != null) {
			HealthScript.SetCannonScript(this, false);
			IsYouTongNpc = HealthScript.IsYouTongNpc;
		}

		if (SpawnAmmoPoint.Length > 1) {
			IsDouGuanDaPao = true;
		}
		InitNpcAmmoList();

		NpcMoveScript = GetComponentInParent<XKNpcMoveCtrl>();
		if (IsOutputError) {
			GameObject obj = null;
			obj.name = "null";		
		}
	}

	XKNpcMoveCtrl NpcMoveScript;
	XKNpcHealthCtrl HealthScript;
	bool IsYouTongNpc;
	const float AngleMin = 1f;
	const float AngleMax = 89f;
	float CosAngleMin = Mathf.Cos((AngleMin / 180f) * Mathf.PI);
	float CosAngleDP = Mathf.Cos((45 / 180f) * Mathf.PI);
	float CosAngleUp = 0f;
	float CosAngleDown = 0f;
	Transform NpcFirePlayer;
    [HideInInspector]
    public XKDaPaoCtrl DaPaoCtrlScript;
	void MakePaoGuanAimPlayer(Vector3 playerPos)
	{
		Vector3 posA = playerPos;
		Vector3 posB = PaoGuan.position;
		Vector3 posBTmp = PaoGuan.position;
		Vector3 vecA = Vector3.Normalize(posA - posB);
		posBTmp.y = posA.y;
		
		Vector3 vecC = CannonTran.forward;
		float cosAC = Vector3.Dot(vecA, vecC);
		if (cosAC < CosAngleDP) {
			return;
		}

		Vector3 vecB = Vector3.Normalize(posA - posBTmp);
		float cosAB = Vector3.Dot(vecA, vecB);

		if (cosAB > CosAngleMin) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = posA.y >= posB.y ? (-AngleMin) : AngleMin;
			if (Mathf.Abs(PaoGuan.localEulerAngles.x) > AngleMin) {
				Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
				if (eulerAnglesTmpVal.x > 90f) {
//					Debug.Log("Unity:"+"A "+PaoGuan.localEulerAngles+", B "+AngleMin);
					eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
				}
				eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x != 0f ? eulerAnglesTmpVal.x : eulerAnglesTmp.x;
			
				if (UpPaoGuanJDVal != DownPaoGuanJDVal) {	
					PaoGuan.localEulerAngles = eulerAnglesTmpVal;
				}
			}
			return;
		}

		if (posA.y > posB.y && cosAB < CosAngleUp) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = UpPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
//				Debug.Log("Unity:"+"A "+PaoGuan.localEulerAngles+", B "+AngleMin);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
			
			if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
				PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			}
			return;
		}
		
		if (posA.y < posB.y && cosAB < CosAngleDown) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = DownPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
				//Debug.Log("Unity:"+"A "+PaoGuan.localEulerAngles+", B "+AngleMin);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
			
			if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
				PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			}
			return;
		}
		
		if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
			PaoGuan.forward = Vector3.Lerp(PaoGuan.forward, vecA, Time.deltaTime * PaoGuanSDVal);
		}
		Vector3 eulerAngles = PaoGuan.localEulerAngles;
		eulerAngles.y = eulerAngles.z = 0f;
		if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
			PaoGuan.localEulerAngles = eulerAngles;
		}
	}

	public void SetIsActiveTrigger()
	{
		if (Network.peerType == NetworkPeerType.Client) {
			IsDoFireAnimation = true;
			IsStopAnimation = false;
			Debug.Log("Unity:"+"KaQiuSha -> SetIsActiveTrigger...");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (IsOutputError) {
			return;		
		}

		if (IsYouTongNpc) {
			return;
		}
		
		if (SpawnAmmoPoint.Length <= 0) {
			return;
		}

		if ((JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())
		    || GameOverCtrl.IsShowGameOver) {
			return;
		}
		
		if (XkGameCtrl.CheckNpcIsMoveToCameraBack(transform)) {
			return;
		}

		if (!XkGameCtrl.IsActivePlayerOne
		    && !XkGameCtrl.IsActivePlayerTwo
		    && !XkGameCtrl.IsActivePlayerThree
		    && !XkGameCtrl.IsActivePlayerFour) {
			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
				if (!IsStopAnimation) {
					IsStopAnimation = true;
				}
				return;
			}
		}
		else {
			if (IsStopAnimation) {
				IsStopAnimation = false;
			}
		}

		if (FireDis <= 0f) {
			if (IsDoFireAnimation) {
				IsDoFireAnimation = false;
			}
			return;
		}

		Vector3 posA = Vector3.zero;
		bool isGetCamPos = true;
		if (PlayerMoveScript != null) {

            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                isGetCamPos = false;
                posA = PlayerMoveScript.transform.position;
            }
            else
            {
                if (!PlayerMoveScript.GetIsDeathPlayer())
                {
                    isGetCamPos = false;
                    posA = PlayerMoveScript.transform.position;
                }
            }
		}

		if (isGetCamPos) {
			GetAimPlayerMoveScript();
			return;
		}

		Vector3 posASave = posA;
		Vector3 posB = CannonTran.position;
		posA.y = posB.y = 0f;
		Vector3 forwardVal = posA - posB;
		if (MaxPaoShenJDVal != MinPaoShenJDVal) {
			CannonTran.forward = Vector3.Lerp(CannonTran.forward, forwardVal.normalized, Time.deltaTime * PaoShenSDVal);
		}

		Vector3 eulerAnglesPS = CannonTran.localEulerAngles;
		eulerAnglesPS.x = eulerAnglesPS.z = 0f;
		float angleY = eulerAnglesPS.y > 180 ? -(360 - eulerAnglesPS.y) : eulerAnglesPS.y;
		if (angleY > MaxPaoShenJDVal || angleY < MinPaoShenJDVal) {
			angleY = angleY > MaxPaoShenJDVal ? MaxPaoShenJDVal : angleY;
			eulerAnglesPS.y = angleY > MinPaoShenJDVal ? angleY : MinPaoShenJDVal;
		}

		if (MaxPaoShenJDVal != MinPaoShenJDVal) {
			CannonTran.localEulerAngles = eulerAnglesPS;
		}

		if (!IsPlayPaoGuanAnimation) {
			MakePaoGuanAimPlayer(posASave);
		}

		posA = posASave;
		posA.y = posB.y = 0f;
		if (Vector3.Distance(posA, posB) <= FireDis && !IsDoFireAnimation) {
			IsDoFireAnimation = true;
			if (CountPaoGuanAni <= 1) {
				if (!IsDouGuanDaPao) {
					StartCoroutine(PlayPaoGuanAnimation());
				}
				else {
					StartCoroutine(SpawnDuoPaoAmmo());
				}
			}
		}

		if (Vector3.Distance(posA, posB) > FireDis && IsDoFireAnimation) {
			IsDoFireAnimation = false;
		}
	}

	/********************************************************************
	 * NpcAimPlayerState == 0; -> SpawnPointScript.PointType == SpawnPointType.KongZhong
	 * NpcAimPlayerState == 1; -> SpawnPointScript.PointType == SpawnPointType.DiMian
	 * NpcAimPlayerState == 2; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == true
	 * NpcAimPlayerState == -1; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == false
	 ********************************************************************/
	//int NpcAimPlayerState = -1;
	public void SetCannonSpawnPointInfo(int aimState, float fireDisVal)
	{
		//Debug.Log("Unity:"+"SetCannonSpawnPointInfo -> aimState "+aimState+", fireDisVal "+fireDisVal);
		ResetCannonInfo();
		FireDis = fireDisVal;
		TimeStartSpawn = Time.realtimeSinceStartup;
		if (!gameObject.activeSelf || IsDeathNpc) {
			gameObject.SetActive(true);
			IsDeathNpc = false;
			InitCannonInfo();
		}
	}

	void PlayAudioCannonFire()
	{
		if (AudioCannonFire == null) {
			return;
		}

		if (AudioCannonFire.isPlaying && AudioCannonFire.loop) {
			return;
		}

		if (!AudioCannonFire.enabled) {
			return;
		}

		if (AudioCannonFire.isPlaying) {
			AudioCannonFire.Stop();
		}
		AudioCannonFire.Play();
	}

	void StopAudioCannonFire()
	{
		if (AudioCannonFire == null) {
			return;
		}
		AudioCannonFire.Stop();
	}

	public int CountPaoGuanAni;
	IEnumerator PlayPaoGuanAnimation()
	{
		CountPaoGuanAni++;
		if (CountPaoGuanAni > 1) {
			//Debug.LogWarning("Unity:"+"PlayPaoGuanAnimation -> CountPaoGuanAni "+CountPaoGuanAni);
			yield break;
		}

		int count = 0;
		int maxCount = 1;
		float speed = PaoGuanZhenFu / maxCount;
		bool isBackPaoGuan = false;
		bool isFireAmmo = false;
		IsPlayPaoGuanAnimation = true;
		do {
			if (IsDeathNpc || GameOverCtrl.IsShowGameOver) {
				yield break;
			}

			if (IsStopAnimation
			    || !IsDoFireAnimation
			    || Time.realtimeSinceStartup - TimeStartSpawn < TimeFireDelay
			    || JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				yield return new WaitForSeconds(0.1f);
				continue;
			}
            
            if (XkGameCtrl.GetInstance().IsCreatAmmoOnBoss == false)
            {
                if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetIsHaveCaiPiaoBoss() == true)
                {
                    if (NpcMoveScript != null && NpcMoveScript.IsCaiPiaoZhanChe == true)
                    {
                    }
                    else
                    {
                        //有彩票boss产生,不再继续发射子弹.
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                }
            }

            if (!isBackPaoGuan) {
				PaoGuan.position -= PaoGuan.forward * speed;
				count++;
				if (count >= maxCount) {
					isBackPaoGuan = true;
				}

				if (count == 1 && !isFireAmmo) {
					isFireAmmo = true;
					PlayAudioCannonFire();
					
					PlayerAmmoCtrl ammoPlayerScript = DaPaoAmmo.GetComponent<PlayerAmmoCtrl>();
					if (ammoPlayerScript != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) {
						yield break;
					}

					GameObject obj = GetNpcAmmoFromList(SpawnAmmoPoint[0]);
					if (obj == null) {
						yield break;
					}

					NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
					Transform tran = obj.transform;
					tran.parent = XkGameCtrl.NpcAmmoArray;
					if (AmmoScript != null) {
						AmmoScript.SetIsAimFeiJiPlayer(false);
					}
					else {
						PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
						if (ammoScript != null) {
							Vector3 startPos = tran.position;
							Vector3 firePos = tran.position;
							Vector3 ammoForward = tran.forward;
							firePos = Random.Range(300f, 400f) * ammoForward + startPos;
							float fireDisVal = Vector3.Distance(firePos, startPos);
							RaycastHit hit;
							LayerMask FireLayer = XkGameCtrl.GetInstance().PlayerAmmoHitLayer;
							if (Physics.Raycast(startPos, ammoForward, out hit, fireDisVal, FireLayer.value)) {
								//Debug.Log("Unity:"+"npc fire PlayerAmmo, fire obj -> "+hit.collider.name);
								firePos = hit.point;
								XKNpcHealthCtrl healthScript = hit.collider.GetComponent<XKNpcHealthCtrl>();
								if (healthScript != null) {
									healthScript.OnDamageNpc(ammoScript.DamageNpc, PlayerEnum.Null);
								}
								
								BuJiBaoCtrl buJiBaoScript = hit.collider.GetComponent<BuJiBaoCtrl>();
								if (buJiBaoScript != null) {
									buJiBaoScript.RemoveBuJiBao(PlayerEnum.Null); //buJiBaoScript
								}
							}
							ammoScript.StartMoveAmmo(firePos, PlayerEnum.Null, null, AmmoMovePath);
						}
					}

					obj = (GameObject)Instantiate(DaPaoAmmoLiZi, SpawnAmmoPoint[0].position, SpawnAmmoPoint[0].rotation);
					tran = obj.transform;
					XkGameCtrl.CheckObjDestroyThisTimed(obj);
					tran.parent = SpawnAmmoPoint[0];
				}
			}
			else {
				PaoGuan.position += PaoGuan.forward * speed;
				count--;
				if (count <= 0) {
					IsPlayPaoGuanAnimation = false;
					isBackPaoGuan = false;
					isFireAmmo = false;
					count = 0;

					yield return new WaitForSeconds(TimeDanGuanFire);
					IsPlayPaoGuanAnimation = true;
					continue;
				}
			}
			yield return new WaitForSeconds(PaoGuanShenSuoSD);
		} while (true);
	}

	IEnumerator SpawnDuoPaoAmmo()
	{
		CountPaoGuanAni++;
		if (CountPaoGuanAni > 1) {
			//Debug.LogWarning("Unity:"+"PlayPaoGuanAnimation -> CountPaoGuanAni "+CountPaoGuanAni);
			yield break;
		}

		if (!IsDouGuanDaPao) {
			yield break;
		}

		int countMax = SpawnAmmoPoint.Length;
		int count = countMax;
		do {
			if (IsDeathNpc || GameOverCtrl.IsShowGameOver) {
				Debug.Log("Unity:"+this.name+" -> IsDeathNpc "+IsDeathNpc
				          +", IsShowGameOver "+GameOverCtrl.IsShowGameOver);
				yield break;
			}

			if (!IsDoFireAnimation
			    || IsStopAnimation
			    || Time.realtimeSinceStartup - TimeStartSpawn < TimeFireDelay
			    || JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				yield return new WaitForSeconds(0.1f);
				continue;
			}
            
            if (XkGameCtrl.GetInstance().IsCreatAmmoOnBoss == false)
            {
                if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetIsHaveCaiPiaoBoss() == true)
                {
                    if (NpcMoveScript != null && NpcMoveScript.IsCaiPiaoZhanChe == true)
                    {
                    }
                    else
                    {
                        //有彩票boss产生,不再继续发射子弹.
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                }
            }

            count--;
			if (count < 0 || count >= SpawnAmmoPoint.Length) {
				yield break;
			}

			PlayAudioCannonFire();
			GameObject obj = GetNpcAmmoFromList(SpawnAmmoPoint[count]);
			if (obj == null) {
				Debug.Log("Unity:"+this.name+" is not find ammo!");
				yield break;
			}

			NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
			AmmoScript.SetIsAimFeiJiPlayer(false);
			obj.transform.parent = XkGameCtrl.NpcAmmoArray;

			obj = (GameObject)Instantiate(DaPaoAmmoLiZi, SpawnAmmoPoint[count].position, SpawnAmmoPoint[count].rotation);
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
			obj.transform.parent = SpawnAmmoPoint[count];

			yield return new WaitForSeconds(TimeAmmoUnit);
			if (count <= 0) {
				yield return new WaitForSeconds(TimeAmmoWait);
				count = countMax;
				continue;
			}
		} while (true);
	}

	public void CallOtherPortDeath()
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;
		ResetCannonInfo();
	}

	public void OnRemoveCannon(PlayerEnum playerSt, int key)
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(CannonTran);

		if (TimeDestroy <= 0) {
			gameObject.SetActive(false);
			ResetCannonInfo();
		}

		if (DaPaoCtrlScript == null) {
			DaPaoCtrlScript = GetComponentInParent<XKDaPaoCtrl>();
		}

		if (key == 1 && DaPaoCtrlScript != null && NpcMoveScript == null) {
			//Debug.Log("Unity:"+"XKDaPaoCtrl -> OnRemoveCannon...");
			DaPaoCtrlScript.OnRemoveCannon(PlayerEnum.Null, 1, TimeDestroy);
		}
	}

	public void SetSpawnPointScript(XKDaPaoCtrl daPaoScript)
	{
		DaPaoCtrlScript = daPaoScript;
	}

	/// <summary>
	/// The ammo list.
	/// </summary>
	List<NpcAmmoCtrl> AmmoList;
	bool IsClearNpcAmmo;
	void InitNpcAmmoList()
	{
		if (AmmoList != null) {
			AmmoList.Clear();
		}
		AmmoList = new List<NpcAmmoCtrl>(5);
	}
	
	void HandleAmmoList(NpcAmmoCtrl scriptAmmo)
	{
		if (AmmoList.Contains(scriptAmmo)) {
			return;
		}
		AmmoList.Add(scriptAmmo);
	}

	public void ClearNpcAmmoList()
	{
		if (IsClearNpcAmmo) {
			return;
		}
		IsClearNpcAmmo = true;

//		Debug.Log("Unity:"+"XKCannonCtrl::ClearNpcAmmoList -> NpcAmmoCount "+AmmoList.Count);
		if (AmmoList == null || AmmoList.Count < 1) {
			return;
		}

		NpcAmmoCtrl[] ammoListArray = AmmoList.ToArray();
		int max = ammoListArray.Length;
		for (int i = 0; i < max; i++) {
			if (ammoListArray[i] != null) {
				ammoListArray[i].MakeNpcAmmoDestory();
			}
		}
		AmmoList.Clear();
	}
	
	GameObject GetNpcAmmoFromList(Transform spawnPoint)
	{
		if (IsClearNpcAmmo) {
			return null;
		}

		if (spawnPoint == null) {
			return null;
		}
		
		GameObject objAmmo = null;
		int max = AmmoList.Count;
		for (int i = 0; i < max; i++) {
			if (AmmoList[i] != null && !AmmoList[i].gameObject.activeSelf) {
				objAmmo = AmmoList[i].gameObject;
				break;
			}
		}
		
		if (objAmmo == null) {
			objAmmo = SpawnNpcAmmo(spawnPoint);
			HandleAmmoList( objAmmo.GetComponent<NpcAmmoCtrl>() );
		}
		
		if (objAmmo != null) {
			Transform tranAmmo = objAmmo.transform;
			tranAmmo.position = spawnPoint.position;
			tranAmmo.rotation = spawnPoint.rotation;
		}
		return objAmmo;
	}
	
	GameObject SpawnNpcAmmo(Transform spawnPoint)
	{
		return (GameObject)Instantiate(DaPaoAmmo, spawnPoint.position, spawnPoint.rotation);
	}

	/// <summary>
	/// Resets the cannon info.
	/// </summary>
	void ResetCannonInfo()
	{
		CountPaoGuanAni = 0;
		IsClearNpcAmmo = false;
		IsPlayPaoGuanAnimation = false;
		IsStopAnimation = false;
		IsDoFireAnimation = false;
	}

	void InitCannonInfo()
	{
		if (HealthScript != null) {
			HealthScript.SetCannonScript(this);
		}

		if (AudioCannonFire != null) {
			AudioCannonFire.Stop();
		}
		InitNpcAmmoList();
	}
	
	void GetAimPlayerMoveScript()
	{
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            PlayerMoveScript = null;
            int count = 0;
            int randVal = Random.Range(0, 100) % 2;
            do
            {
                switch (randVal)
                {
                    case 0:
                        PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePOne();
                        break;

                    case 1:
                        PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePTwo();
                        break;

                    case 2:
                        PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePThree();
                        break;

                    case 3:
                        PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePFour();
                        break;
                }

                if (PlayerMoveScript != null)
                {
                    break;
                }

                randVal = Random.Range(0, 100) % 2;
                count++;
                if (count > 8)
                {
                    break;
                }
            } while (PlayerMoveScript == null);
        }
        else
        {
            PlayerMoveScript = null;
            if (XkGameCtrl.PlayerActiveNum <= 0)
            {
                return;
            }

            int count = 0;
            int randVal = Random.Range(0, 100) % 4;
            do
            {
                switch (randVal)
                {
                    case 0:
                        if (XkGameCtrl.IsActivePlayerOne)
                        {
                            PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePOne();
                        }
                        break;

                    case 1:
                        if (XkGameCtrl.IsActivePlayerTwo)
                        {
                            PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePTwo();
                        }
                        break;

                    case 2:
                        if (XkGameCtrl.IsActivePlayerThree)
                        {
                            PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePThree();
                        }
                        break;

                    case 3:
                        if (XkGameCtrl.IsActivePlayerFour)
                        {
                            PlayerMoveScript = XKPlayerMoveCtrl.GetInstancePFour();
                        }
                        break;
                }

                if (PlayerMoveScript != null)
                {
                    break;
                }
                randVal = Random.Range(0, 100) % 4;
                count++;
                if (count > 8)
                {
                    break;
                }
            } while (PlayerMoveScript == null);
        }
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
}