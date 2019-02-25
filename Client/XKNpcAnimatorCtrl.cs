using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKNpcAnimatorCtrl : MonoBehaviour {

	internal Animator AnimatorCom;
	public GameObject AmmoPrefab;
	public GameObject AmmoLiZiPrefab;
	public Transform AmmoSpawnTran;
	public Transform[] AmmoSpawnArray;
	public AudioSource AudioNpcFire;
	XKNpcMoveCtrl NpcScript;
//	XkNpcZaiTiCtrl ZaiTiScriptVal;
	bool IsStopAnimation;
	int CountFireAction;
	[Range(0, 100)]public int CountFirePL = 1;
	[Range(0f, 100f)]public float TimeRootAni = 0.2f;
	int CountHuanDan = 10;
	float TimeRootVal = 1f;
//	string AnimationNameCur = "";
	bool IsDoHuanDanAction;
	bool IsDoRunFireAction;
	int CountFireRunVal;
	int CountFireRun = 5;
	bool IsAimFeiJiPlayer;
	GameObject AmmoLiZiObj;
	// Use this for initialization
	void Awake()
	{
		InitNpcAmmoList();
		SetCountHuanDan(CountFirePL, TimeRootAni);
		AnimatorCom = GetComponent<Animator>();
		NpcScript = GetComponentInParent<XKNpcMoveCtrl>();
        for (int i = 0; i < AmmoSpawnArray.Length; i++)
        {
            if (AmmoSpawnArray[i] != null)
            {
                AmmoSpawnArray[i].gameObject.SetActive(false);
            }
        }

        if (AmmoSpawnTran != null)
        {
            AmmoSpawnTran.gameObject.SetActive(false);
        }
	}

//	void Update()
//	{
//		if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
//			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
//				if (!IsStopAnimation) {
//					IsStopAnimation = true;
//					AnimatorCom.speed = 0f;
//				}
//			}
//		}
//		else {
//			if (IsStopAnimation) {
//				IsStopAnimation = false;
//				AnimatorCom.speed = 1f;
//			}
//		}

//		if (AnimatorCom != null
//		    && !AnimatorCom.enabled
//		    && AnimatorCom.runtimeAnimatorController != null
//		    && NpcScript != null
//		    && !NpcScript.GetIsDeathNPC()) {
//			AnimatorCom.enabled = true;
//			if (!NpcScript.IsAniMove) {
//				AnimatorCom.enabled = true;
//			}
//		}
//	}

	public void PlayNpcAnimatoin(string aniName)
	{
		//Debug.Log("Unity:"+"aniName "+aniName);
		if (aniName == "") {
			return;
		}

		if (AnimatorCom == null || AnimatorCom.runtimeAnimatorController == null) {
			//Debug.LogWarning("Unity:"+"AnimatorCom or runtimeAnimatorController is null, name "+gameObject.name);
			return;
		}

		if (!gameObject.activeSelf) {
			gameObject.SetActive(true);
		}

		if (!AnimatorCom.enabled) {
			AnimatorCom.enabled = true;
		}

		bool isHuanDan = aniName.StartsWith("HuanDan");
		if (!isHuanDan) {
			ResetNpcAnimation();
//			AnimationNameCur = aniName;
		}

        if (AnimatorCom.speed == 0f)
        {
            //Debug.LogWarning("PlayNpcAnimatoin -> Reset AniSpeed, SpeedActionCur ====== " + SpeedActionCur);
            AnimatorCom.speed = SpeedActionCur;
        }
		AnimatorCom.SetBool(aniName, true);
	}

	public void ResetIsDoRunFireAction()
	{
		IsDoRunFireAction = false;
	}

	public void ResetNpcAnimation()
	{
		if (!gameObject.activeSelf) {
			return;
		}

		if (AnimatorCom == null || AnimatorCom.runtimeAnimatorController == null) {
//			Debug.LogWarning("Unity:"+"AnimatorCom is null, name " + gameObject.name);
//			AnimatorCom.name = "null";
			return;
		}

		AnimatorCom.SetBool("Root1", false);
//		AnimatorCom.SetBool("Root2", false);
//		AnimatorCom.SetBool("Root3", false);
//		AnimatorCom.SetBool("Root4", false);
		AnimatorCom.SetBool("Run1", false);
		AnimatorCom.SetBool("Run2", false);
		AnimatorCom.SetBool("Run3", false);
		AnimatorCom.SetBool("Run4", false);
		AnimatorCom.SetBool("Fire1", false);
		AnimatorCom.SetBool("Fire2", false);
//		AnimatorCom.SetBool("Fire3", false);
//		AnimatorCom.SetBool("Fire4", false);
//		AnimatorCom.SetBool("Fire5", false);
//		AnimatorCom.SetBool("Fire6", false);
//		AnimatorCom.SetBool("HuanDan1", false);
//		AnimatorCom.SetBool("HuanDan2", false);
//		AnimatorCom.SetBool("HuanDan3", false);
//		AnimatorCom.SetBool("HuanDan4", false);
//		AnimatorCom.SetBool("HuanDan5", false);
//		AnimatorCom.SetBool("HuanDan6", false);
	}

//	public void SetAmmoPrefabVal(XKNpcMoveCtrl scriptVal)
//	{
//		if (ZaiTiScriptVal == null) {
//			ZaiTiScriptVal = GetComponent<XkNpcZaiTiCtrl>();
//		}
//
//		if (ZaiTiScriptVal != null) {
//			AmmoPrefab = ZaiTiScriptVal.AmmoPrefab;
//			AmmoLiZiPrefab = ZaiTiScriptVal.AmmoLiZiPrefab;
//			AmmoSpawnTran = ZaiTiScriptVal.AmmoSpawnTran;
//			AudioNpcFire = ZaiTiScriptVal.AudioNpcFire;
//		}
		//NpcScript = scriptVal;
		//SetCountHuanDan(scriptVal.CountHuanDan);

		//SetSpawnPointScript( NpcScript.GetSpawnPointScript() );
//	}

	public void ResetFireAnimationSpeed()
	{
//		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
//			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
//				return;
//			}
//		}

		if (AnimatorCom.speed == SpeedActionCur) {
			return;
		}
		AnimatorCom.speed = SpeedActionCur;
	}

	void OnTriggerFireAimAnimation()
	{
		if (!NpcScript.GetIsAimPlayerByFire()) {
			return;
		}
		//Debug.Log("Unity:"+"OnTriggerFireAimAnimation -> name "+gameObject.name);
		SpeedActionCur = AnimatorCom.speed;
		AnimatorCom.speed = 0f;
	}

	public void SetIsAimFeiJiPlayer(bool isAim)
	{
		IsDoHuanDanAction = false;
		IsAimFeiJiPlayer = isAim;
	}

	void OnTriggerFireAnimation()
	{
		//return; //test
		//Debug.Log("Unity:"+"OnTriggerFireAnimation**NpcName "+AnimatorCom.name);
		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				return;
			}
		}
				
		if ((JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())
		    || GameOverCtrl.IsShowGameOver) {
			return;
		}

        if (XkGameCtrl.GetInstance().IsCreatAmmoOnBoss == false)
        {
            if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.GetIsHaveCaiPiaoBoss() == true
                && Random.Range(0, 100) % 2 == 0)
            {
                //有彩票boss产生,不再继续发射子弹.
                return;
            }
        }

        //Debug.Log("************************11111 time " + Time.time);
        int rv = AddCountFireAction();
		if (rv != -1) {
			StartSpawnNpcAmmo();
		}
	}

	void StartSpawnNpcAmmo()
	{
		if (XkGameCtrl.CheckNpcIsMoveToCameraBack(transform)) {
			return;
		}

		if (AudioNpcFire != null) {
			if (AudioNpcFire.isPlaying) {
				AudioNpcFire.Stop();
			}
			AudioNpcFire.Play();
		}

		GameObject obj = null;
		Transform tran = null;
		if (AmmoLiZiPrefab != null && AmmoLiZiObj == null) {
			obj = (GameObject)Instantiate(AmmoLiZiPrefab, AmmoSpawnTran.position, AmmoSpawnTran.rotation);
			tran = obj.transform;
			tran.parent = XkGameCtrl.NpcAmmoArray;
			AmmoLiZiObj = obj;
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
		}

		if (AmmoPrefab == null) {
			return;		
		}

		PlayerAmmoCtrl ammoPlayerScript = AmmoPrefab.GetComponent<PlayerAmmoCtrl>();
		if (ammoPlayerScript != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		obj = GetNpcAmmoFromList(AmmoSpawnTran);
		if (obj == null) {
			return;
		}

		tran = obj.transform;
		tran.parent = XkGameCtrl.NpcAmmoArray;
		NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
		if (AmmoScript != null) {
			AmmoScript.SetNpcScriptInfo(NpcScript);
			AmmoScript.SetIsAimFeiJiPlayer(IsAimFeiJiPlayer);
			for (int i = 0; i < AmmoSpawnArray.Length; i++) {
				if (AmmoSpawnArray[i] != null) {
					obj = (GameObject)Instantiate(AmmoPrefab,
					                              AmmoSpawnArray[i].position,
					                              AmmoSpawnArray[i].rotation);
					tran = obj.transform;
					tran.parent = XkGameCtrl.NpcAmmoArray;
					AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
					AmmoScript.SetNpcScriptInfo(NpcScript);
					AmmoScript.SetIsAimFeiJiPlayer(IsAimFeiJiPlayer);
				}
			}
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
				ammoScript.StartMoveAmmo(firePos, PlayerEnum.Null);
			}
		}
	}
	
	void OnTriggerHuanDanAnimation()
	{
//		Debug.Log("Unity:"+"OnTriggerHuanDanAnimation...");
		RandomPlayNpcFireAction();
		IsDoHuanDanAction = false;
	}

	void RandomPlayNpcFireAction()
	{
		ResetHuanDanAnimation();
//		if (ZaiTiScriptVal != null) {
//			ZaiTiScriptVal.MakeNpcDoFireAnimation();
//		}
//		else {
//			NpcScript.MakeNpcDoFireAnimation();
//		}
	}

	void ResetHuanDanAnimation()
	{
//		AnimatorCom.SetBool("HuanDan1", false);
//		AnimatorCom.SetBool("HuanDan2", false);
//		AnimatorCom.SetBool("HuanDan3", false);
//		AnimatorCom.SetBool("HuanDan4", false);
//		AnimatorCom.SetBool("HuanDan5", false);
//		AnimatorCom.SetBool("HuanDan6", false);
	}

	public void SetCountHuanDan(int valCount = 0, float valTime = 0f)
	{
		CountHuanDan = valCount == 0 ? Random.Range(1, 5) : valCount;
		TimeRootVal = valTime == 0f ? Random.Range(0.5f, 3f) : valTime;
	}

	int AddCountFireAction()
	{
		if (IsDoHuanDanAction) {
//			Debug.LogWarning("Unity:"+"IsDoHuanDanAction is true");
			return -1;
		}
		
		if (IsDoRunFireAction) {
//			Debug.LogWarning("Unity:"+"IsDoRunFireAction is true");
			return -1;
		}
		
		CountFireAction++;
		FirePoint firePointScript = NpcScript.GetFirePointScript();
		if (firePointScript != null) {
			CountFireRun = firePointScript.CountFire;
			CountFireRunVal++;
//			Debug.Log("Unity:"+"CountFireAction "+CountFireAction+", CountFireRunVal "+CountFireRunVal);
			if (CountFireRun <= CountFireRunVal) {
				//Play Run_Fire Action
				CountFireRunVal = 0;
				MakeNpcDoActionRun3();
				NpcScript.MakeNpcMoveFirePoint();
				return 0;
			}
		}

		NpcMark markScript = NpcScript.GetMarkScriptVal();
		if (NpcScript != null && markScript != null && markScript.IsDoFireAction) {
			CountFireRun = markScript.FireCount;
			CountFireRunVal++;
			if (CountFireRun <= CountFireRunVal) {
				CountFireRunVal = 0;
				MakeNpcDoActionRun3();
				NpcScript.MoveNpcByItween();
				return 0;
			}
		}

		if (CountHuanDan <= 0) {
			return 0;
		}

		if (CountHuanDan <= CountFireAction) {
			//Stop Play Fire Action.
			CountFireAction = 0;
			DelayPlayFireAction();
			SetCountHuanDan(CountFirePL, TimeRootAni);
//			PlayNPCHuanDanAction();
		}
		return 0;
	}

	float SpeedActionCur;
	void DelayPlayFireAction()
	{
		SpeedActionCur = AnimatorCom.speed;
		AnimatorCom.speed = 0f;
		Invoke("ResetFireAnimationSpeed", TimeRootVal);
	}

	void MakeNpcDoActionRun3()
	{
		IsDoHuanDanAction = false;
		IsDoRunFireAction = true;
		ResetNpcAnimation();
		NpcScript.NetNpcPlayAnimation(this, AnimatorNameNPC.Run3.ToString());

        //Debug.Log("************************22222 time " + Time.time);
    }

	void PlayNPCHuanDanAction()
	{
		IsDoHuanDanAction = true;
		string aniName = "";
//		if (AnimationNameCur == AnimatorNameNPC.Fire1.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan1.ToString();
//		}
//		else if (AnimationNameCur == AnimatorNameNPC.Fire2.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan2.ToString();
//		}
//		else if (AnimationNameCur == AnimatorNameNPC.Fire3.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan3.ToString();
//		}
//		else if (AnimationNameCur == AnimatorNameNPC.Fire4.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan4.ToString();
//		}
//		else if (AnimationNameCur == AnimatorNameNPC.Fire5.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan5.ToString();
//		}
//		else if (AnimationNameCur == AnimatorNameNPC.Fire6.ToString()) {
//			aniName = AnimatorNameNPC.HuanDan6.ToString();
//		}

		NpcScript.NetNpcPlayAnimation(this, aniName);
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
//		Debug.Log("Unity:"+"XKNpcAnimatorCtrl::ClearNpcAmmoList -> NpcAmmoCount "+AmmoList.Count);

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
		if (AmmoPrefab == null) {
			return null;
		}

		if (IsClearNpcAmmo) {
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
			HandleAmmoList( objAmmo.GetComponent<NpcAmmoCtrl>() );  //npc子弹列表.
        }
		
		if (objAmmo != null) {
			Transform tranAmmo = objAmmo.transform;
			tranAmmo.position = spawnPoint.position;
			tranAmmo.rotation = spawnPoint.rotation;
//			if (gameObject.name == "kuanggong") {
//				Debug.LogError("Unity:"+"***********************11");
//			}
		}
		return objAmmo;
	}
	
	GameObject SpawnNpcAmmo(Transform spawnPoint)
	{
		return (GameObject)Instantiate(AmmoPrefab, spawnPoint.position, spawnPoint.rotation);
	}
}