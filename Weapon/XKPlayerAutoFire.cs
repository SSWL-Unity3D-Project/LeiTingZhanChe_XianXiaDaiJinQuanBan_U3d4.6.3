using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerAmmoType
{
	Null = -1,
	PuTongAmmo,		//普通子弹(前后发射-长程机枪-强击机枪).
	GaoBaoAmmo,		//高爆弹.
	DaoDanAmmo,		//导弹(主炮导弹).
	SanDanAmmo,		//散弹.
	GenZongAmmo,	//跟踪弹(主角迫击炮).
	ChuanTouAmmo,	//穿甲弹(主角穿甲弹).
	JianSuAmmo,		//减速弹.
	PaiJiPaoAmmo,	//迫击炮(npc迫击炮子弹).
}

public class XKPlayerAutoFire : MonoBehaviour
{
	/**
	 * PaoTaRealObj[0] -> 默认炮塔.
	 * PaoTaRealObj[1] -> 主炮穿甲弹炮塔.
	 * PaoTaRealObj[2] -> 主炮散弹炮塔.
	 * PaoTaRealObj[3] -> 主炮迫击炮炮塔.
	 */
	public GameObject[] PaoTaRealObj;
	/**
	 * IsLockPaoTa == true -> 子弹向镜头正前方打,炮塔转向不跟随坦克机身的转向.
	 * IsLockPaoTa == false -> 子弹向子弹产生点方向打,炮塔转向跟随坦克机身的转向.
	 */
	public bool IsLockPaoTa;
	public Transform PaoTaTr;
	/**
	 * 炮塔开火点控制.
	 */
	public Transform PaoTaFireTr;
	Transform CameraTran;
	internal LayerMask FireLayer;
	/// <summary>
	/// 主角向前发射子弹的起始点.
	/// AmmoStartPosOne[0 - 3] -> 左前,右前,左后,右后.
	/// AmmoStartPosOne[4 - x] -> 四散机枪.
	/// </summary>
	public Transform[] AmmoStartPosOne;
	/// <summary>
	/// 主角主炮发射子弹的起始点.
	/// </summary>
	public Transform[] AmmoStartPosZP;
//	Transform[] DaoDanAmmoPosOne;
//	Transform[] DaoDanAmmoPosTwo;
	//普通子弹开火粒子.
	GameObject[] AmmoParticle;
	GameObject[] SanDanAmmoParticle;
	GameObject[] PaiJiPaoAmmoParticle;
	GameObject[] ChuanTouDanAmmoParticle;
	GameObject[] DaoDanAmmoParticle;
	//public GameObject[] GaoBaoAmmoParticle;
	//public GameObject[] GenZongDanAmmoParticle;
	//public GameObject[] JianSuDanAmmoParticle;
	/**
	 * 机枪普通子弹.
	 */
	public GameObject PuTongAmmo;
	/**
	 * 主炮普通导弹.
	 */
	public GameObject DaoDanZPAmmo;
	//public GameObject GaoBaoDanAmmo;
	//public GameObject SanDanAmmo;
	//public GameObject GenZongDanAmmo;
	GameObject ChuanTouDanAmmo;
	//public GameObject JianSuDanAmmo;
	GameObject AmmoSanDanZP;
	Transform TanKeTran;
	
	//GameObject[] AmmoParticleObj = new GameObject[2];
	//GameObject[] SanDanAmmoParticleObj = new GameObject[14];
	//GameObject[] GenZongDanAmmoParticleObj = new GameObject[2];
	//GameObject[] ChuanTouDanAmmoParticleObj = new GameObject[2];
	//GameObject[] JianSuDanAmmoParticleObj = new GameObject[2];
	//GameObject[] GaoBaoDanAmmoParticleObj = new GameObject[2];

	/**
	 * Frequency[0] -> 机枪口.
	 * Frequency[1] -> 主炮口.
	 */
	[Range(0.01f, 500f)] float[] Frequency = {10f, 10f};				//普通子弹发射频率.
	//[Range(0.01f, 500f)] public float[] FrequencyGaoBao = {10f, 10f};		//高爆弹发射频率.
	//[Range(0.01f, 500f)] public float[] FrequencySanDan = {10f, 10f};		//散弹发射频率.
	//[Range(0.01f, 500f)] public float[] FrequencyGenZongDan = {10f, 10f};	//跟踪弹发射频率.
	//[Range(0.01f, 500f)] public float[] FrequencyChuanTouDan = {10f, 10f};	//穿透弹发射频率.
	//[Range(0.01f, 500f)] public float[] FrequencyJianSuDan = {10f, 10f};	//减速弹发射频率.

//	[Range(1f, 500f)] public float DaoDanTimeMin = 1.5f; //导弹冷却时间.
	bool IsActiveFireBtJQ; //机枪开火.
	bool IsActiveFireBtZP; //主炮开火.
	float LastFireTimeJiQiang = -1f;
	float LastFireTimeZhuPao = -1f;
	public PlayerAmmoType AmmoStateJiQiang = PlayerAmmoType.PuTongAmmo;
	public PlayerAmmoType AmmoStateZhuPao = PlayerAmmoType.DaoDanAmmo;
	/*public static PlayerAmmoType AmmoStatePOne = PlayerAmmoType.PuTongAmmo;
	public static PlayerAmmoType AmmoStatePTwo = PlayerAmmoType.PuTongAmmo;
	public static PlayerAmmoType AmmoStatePThree = PlayerAmmoType.PuTongAmmo;
	public static PlayerAmmoType AmmoStatePFour = PlayerAmmoType.PuTongAmmo;
	public static PlayerAmmoType AmmoStateZPOne = PlayerAmmoType.DaoDanAmmo;
	public static PlayerAmmoType AmmoStateZPTwo = PlayerAmmoType.DaoDanAmmo;
	public static PlayerAmmoType AmmoStateZPThree = PlayerAmmoType.DaoDanAmmo;
	public static PlayerAmmoType AmmoStateZPFour = PlayerAmmoType.DaoDanAmmo;*/
	float OffsetForward = 30f;
	float FirePosValTmp = 100f;
	float FireRayDirLen = 100f;
	public static int MaxAmmoCount = 30;
//	float[] DaoDanTimeVal = {0f, 0f};
	public static List<PlayerAmmoCtrl> AmmoList_TK;				//普通子弹.
	public static List<PlayerAmmoCtrl> AmmoGaoBaoList_TK;		//高爆子弹.
	public static List<PlayerAmmoCtrl> AmmoDaoDanList_TK;		//高爆子弹.
	public static List<PlayerAmmoCtrl> AmmoSanDanList_TK;		//散弹.
	public static List<PlayerAmmoCtrl> AmmoGenZongDanList_TK;	//跟踪弹.
	public static List<PlayerAmmoCtrl> AmmoPaiJiPaoList_TK;		//迫击炮子弹.
	public static List<PlayerAmmoCtrl> AmmoChuanTouDanList_TK;	//穿透弹.
	public static List<PlayerAmmoCtrl> AmmoJianSuDanList_TK;	//减速弹.

	/**
PlayerFireAudio[0] -> 主角机枪开枪音效.
PlayerFireAudio[1] -> 主角机枪前后发射音效.
PlayerFireAudio[2] -> 主角机枪长程机枪音效.
PlayerFireAudio[3] -> 主角机枪散弹机枪音效.
PlayerFireAudio[4] -> 主角机枪强击机枪音效.
PlayerFireAudio[5] -> 主角主炮导弹音效.
PlayerFireAudio[6] -> 主角主炮穿甲弹音效.
PlayerFireAudio[7] -> 主角主炮炮击跑音效.
PlayerFireAudio[8] -> 主角主炮散弹音效.
PlayerFireAudio[9] -> 主角主炮火力全开音效.
	 */
	public AudioSource[] PlayerFireAudio;
	bool IsPSAutoFire;
	public static bool IsAimPlayerPOne;
	public static bool IsAimPlayerPTwo;
	PlayerAmmoType PSAmmoTypeVal = PlayerAmmoType.Null;
	float TimeAimPlayerPOne;
	float TimeAimPlayerPTwo;
	internal PlayerEnum PlayerIndex = PlayerEnum.PlayerOne;
	XKPlayerMoveCtrl PlayerMoveScript;
	const int JI_QIANG_INDEX = 0;
	const int ZHU_PAO_INDEX = 1;
	static XKPlayerAutoFire _InstanceOne;
	static XKPlayerAutoFire _InstanceTwo;
	static XKPlayerAutoFire _InstanceThree;
	static XKPlayerAutoFire _InstanceFour;
	public static XKPlayerAutoFire GetInstanceAutoFire(PlayerEnum indexPlayer)
	{
		XKPlayerAutoFire fireScript = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			fireScript = _InstanceOne; 
			break;
		case PlayerEnum.PlayerTwo:
			fireScript = _InstanceTwo; 
			break;
		case PlayerEnum.PlayerThree:
			fireScript = _InstanceThree; 
			break;
		case PlayerEnum.PlayerFour:
			fireScript = _InstanceFour; 
			break;
		}
		return fireScript;
	}

	void Awake()
	{
		PlayerMoveScript = GetComponent<XKPlayerMoveCtrl>();
		if (PlayerMoveScript != null) {
			PlayerIndex = PlayerMoveScript.PlayerIndex;
		}

		switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this; 
			break;
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this; 
			break;
		case PlayerEnum.PlayerThree:
			_InstanceThree = this; 
			break;
		case PlayerEnum.PlayerFour:
			_InstanceFour = this; 
			break;
		}
		ChangePlayerPaoTaObj(PlayerAmmoType.Null);
	}

	// Use this for initialization
	void Start()
	{
		TanKeTran = transform;
		AmmoParticle = XKPlayerGlobalDt.GetInstance().AmmoParticle;
		SanDanAmmoParticle = XKPlayerGlobalDt.GetInstance().SanDanAmmoParticle;
		PaiJiPaoAmmoParticle = XKPlayerGlobalDt.GetInstance().PaiJiPaoAmmoParticle;
		ChuanTouDanAmmoParticle = XKPlayerGlobalDt.GetInstance().ChuanTouDanAmmoParticle;
		DaoDanAmmoParticle = XKPlayerGlobalDt.GetInstance().DaoDanAmmoParticle;

		PuTongAmmo = XKPlayerGlobalDt.GetInstance().PuTongJQAmmo;
		DaoDanZPAmmo = XKPlayerGlobalDt.GetInstance().DaoDanZPAmmo;
		ChuanTouDanAmmo = XKDaoJuGlobalDt.GetInstance().AmmoChuanJiaDanZP;
		AmmoSanDanZP = XKDaoJuGlobalDt.GetInstance().AmmoSanDanZP;
		PaiJiPaoAmmo = XKDaoJuGlobalDt.GetInstance().AmmoPaiJiPao;
		ChangChengAmmo = XKDaoJuGlobalDt.GetInstance().AmmoChangCheng;
		QiangJiAmmo = XKDaoJuGlobalDt.GetInstance().AmmoQiangJi;
		AmmoHuoLiOpenJQ = XKDaoJuGlobalDt.GetInstance().AmmoHuoLiOpenJQ;
		AmmoHuoLiOpenZP = XKDaoJuGlobalDt.GetInstance().AmmoHuoLiOpenZP;
		Frequency = XKPlayerGlobalDt.GetInstance().Frequency;

		//AmmoParticleList = new List<AmmoParticleDt>(6);
		FireLayer = XkGameCtrl.GetInstance().PlayerAmmoHitLayer;
		AmmoStateJiQiang = PlayerAmmoType.PuTongAmmo;
		AmmoStateZhuPao = PlayerAmmoType.DaoDanAmmo;

		int fireAudioLen = PlayerFireAudio.Length;
		for (int i = 0; i < fireAudioLen; i++) {
			if (PlayerFireAudio[i] == null) {
				continue;
			}
			PlayerFireAudio[i].loop = false;
			PlayerFireAudio[i].Stop();
		}

		InitPlayerAmmoList();
		switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtEvent;
			InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickFireDaoDanBtEvent;
			break;
		case PlayerEnum.PlayerTwo:
			InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtEvent;
			InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickFireDaoDanBtEvent;
			break;
		case PlayerEnum.PlayerThree:
			InputEventCtrl.GetInstance().ClickFireBtThreeEvent += ClickFireBtEvent;
			InputEventCtrl.GetInstance().ClickDaoDanBtThreeEvent += ClickFireDaoDanBtEvent;
			break;
		case PlayerEnum.PlayerFour:
			InputEventCtrl.GetInstance().ClickFireBtFourEvent += ClickFireBtEvent;
			InputEventCtrl.GetInstance().ClickDaoDanBtFourEvent += ClickFireDaoDanBtEvent;
			break;
		}
        CreatPlayerXiaoFeiJi();
    }

	void InitPlayerAmmoList()
	{
		if (AmmoList_TK != null) {
			AmmoList_TK.Clear();
		}
		AmmoList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount); //普通子弹.

		if (AmmoGaoBaoList_TK != null) {
			AmmoGaoBaoList_TK.Clear();
		}
		AmmoGaoBaoList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount); //高爆子弹.

		if (AmmoDaoDanList_TK != null) {
			AmmoDaoDanList_TK.Clear();
		}
		AmmoDaoDanList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount); //导弹子弹.

		if (AmmoSanDanList_TK != null) {
			AmmoSanDanList_TK.Clear();
		}
		AmmoSanDanList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount);
		
		if (AmmoGenZongDanList_TK != null) {
			AmmoGenZongDanList_TK.Clear();
		}
		AmmoGenZongDanList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount);

		if (AmmoPaiJiPaoList_TK != null) {
			AmmoPaiJiPaoList_TK.Clear();
		}
		AmmoPaiJiPaoList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount);

		if (AmmoChuanTouDanList_TK != null) {
			AmmoChuanTouDanList_TK.Clear();
		}
		AmmoChuanTouDanList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount);
		
		if (AmmoJianSuDanList_TK != null) {
			AmmoJianSuDanList_TK.Clear();
		}
		AmmoJianSuDanList_TK = new List<PlayerAmmoCtrl>(MaxAmmoCount);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (IsLockPaoTa) {
			UpdatePaoTaRot();
		}

		if (XKTriggerClosePlayerUI.IsClosePlayerUI) {
			return;
		}

		CheckPlayerJiQiangFireBt();
		CheckPlayerZhuPaoFireBt();
//		CheckPSTriggerAutoFire();
	}

	GameObject SpawnPlayerAmmo(GameObject ammoPrefab, Vector3 ammoPos, Quaternion ammoRot)
	{
		return (GameObject)Instantiate(ammoPrefab, ammoPos, ammoRot);
	}

	bool CheckIsActivePlayer()
	{
		switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			if (XkGameCtrl.IsActivePlayerOne) {
				return true;
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			if (XkGameCtrl.IsActivePlayerTwo) {
				return true;
			}
			break;
			
		case PlayerEnum.PlayerThree:
			if (XkGameCtrl.IsActivePlayerThree) {
				return true;
			}
			break;
			
		case PlayerEnum.PlayerFour:
			if (XkGameCtrl.IsActivePlayerFour) {
				return true;
			}
			break;
		}
		return false;
	}

	/// <summary>
	/// Checks the is spawn player ammo.
	/// paoKou == 0 -> 主角机枪炮口.
	/// paoKou == 1 -> 主角主炮炮口.
	/// </summary>
	bool CheckIsSpawnPlayerAmmo(int paoKou)
	{
		bool isSpawnAmmo = true;
		PlayerAmmoType ammoType = PlayerAmmoType.Null;
		float lastFireTime = 0f;
		float frequencyVal = 1f;
		switch (paoKou) {
		case JI_QIANG_INDEX:
			ammoType = AmmoStateJiQiang;
			lastFireTime = LastFireTimeJiQiang;
			break;
			
		case ZHU_PAO_INDEX:
			ammoType = AmmoStateZhuPao;
			lastFireTime = LastFireTimeZhuPao;
			break;
		}

		switch (ammoType) {
		case PlayerAmmoType.PuTongAmmo:
			frequencyVal = Frequency[paoKou];
			break;
		default:
			frequencyVal = Frequency[paoKou];
			break;
		}

		if (Time.time < lastFireTime + (1f / frequencyVal)) {
			isSpawnAmmo = false;
		}
		return isSpawnAmmo;
	}
	
	void SpawnJiQiangAmmo(int indexVal)
	{
		Vector3 ammoSpawnForward = AmmoStartPosOne[indexVal].forward;
		Vector3 ammoSpawnPos = AmmoStartPosOne[indexVal].position;
		Quaternion ammoSpawnRot = AmmoStartPosOne[indexVal].rotation;
		GameObject obj = null;
		CheckFireAudioPlayerJiQiang();
		
		obj = SpawnPlayerAmmoByAmmoType(JI_QIANG_INDEX, ammoSpawnPos, ammoSpawnRot);
		if (obj == null) {
			return;
		}
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
		
		Vector3 mousePosInput = Input.mousePosition;
		//if (pcvr.bIsHardWare) {
			//mousePosInput = pcvr.CrossPositionOne;
		//}
		
		RaycastHit hit;
		Vector3 firePos = Vector3.zero;
		Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
		Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 ammoForward = Vector3.Normalize( posTmp - ammoSpawnPos );
		if (!IsPSAutoFire) {
			firePos = FirePosValTmp * ammoSpawnForward + ammoSpawnPos;
			FireRayDirLen = ammoScript.MvSpeed * ammoScript.LiveTime;
			if (Physics.Raycast(ammoSpawnPos, ammoSpawnForward, out hit, FireRayDirLen, FireLayer.value)) {
				//Debug.Log("Unity:"+"Player fire obj -> "+hit.collider.name);
				if (ammoScript.AmmoType != PlayerAmmoType.ChuanTouAmmo) {
					firePos = hit.point;
				}
			}
		}
		else {
			ammoForward = obj.transform.forward;
			firePos = FirePosValTmp * ammoForward + ammoSpawnPos;
			if (Physics.Raycast(ammoSpawnPos, ammoForward, out hit, FireRayDirLen, FireLayer.value)) {
				//Debug.Log("Unity:"+"Player fire obj -> "+hit.collider.name);
				if (ammoScript.AmmoType != PlayerAmmoType.ChuanTouAmmo) {
					firePos = hit.point;
				}
			}
		}
		ammoScript.StartMoveAmmo(firePos, PlayerIndex);
	}

	BuJiBaoType JiQiangAmmoSt = BuJiBaoType.Null;
	BuJiBaoType ZhuPaoAmmoSt = BuJiBaoType.DaoDan;
    float m_LastAiJiQianTime = 0f;
    float m_AiJiQianTime = 2f;
    bool IsAiPlayerFire = false;

	void CheckPlayerJiQiangFireBt()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //激活主角Ai坦克状态.
            if (XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType == AiPathGroupCtrl.MoveState.YuLe)
            {
                if (Random.Range(0, 100) % 50 != 0)
                {
                    return;
                }
            }
            else
            {
                //if (Random.Range(0, 100) % 10 != 0)
                //{
                //    return;
                //}

                if (Time.time - m_LastAiJiQianTime >= m_AiJiQianTime)
                {
                    if (IsAiPlayerFire == false)
                    {
                        m_AiJiQianTime = Random.Range(1f, 3f);
                    }
                    else
                    {
                        m_AiJiQianTime = Random.Range(0.5f, 2.5f);
                    }
                    IsAiPlayerFire = !IsAiPlayerFire;
                    m_LastAiJiQianTime = Time.time;
                }

                if (IsAiPlayerFire == false)
                {
                    return;
                }
            }
        }
        else
        {
            if (!CheckIsActivePlayer())
            {
                return;
            }

            if (!IsActiveFireBtJQ)
            {
                return;
            }
        }

        bool isSpawnAmmo = CheckIsSpawnPlayerAmmo(JI_QIANG_INDEX);
        if (!isSpawnAmmo)
        {
            return;
        }
        LastFireTimeJiQiang = Time.time;

        if (isSpawnAmmo == true)
        {
            if (IsQianHouFire == true)
            {
                MakeAllXiaoFeiJiFire();
            }
        }

        //if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi())
        //{
        //    return;
        //}

        if (Camera.main == null)
        {
			return;
		}

		if (GameOverCtrl.IsShowGameOver || JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			if (GameOverCtrl.IsShowGameOver) {
				SetIsActiveFireBtJQ(false);
			}
			return;
		}
		CheckPlayerHouZuoLi(JiQiangAmmoSt, JI_QIANG_INDEX);

		if (!IsJiQiangSanDanFire) {
			SpawnJiQiangAmmo(0);
			SpawnJiQiangAmmo(1);
		}

        SpawnJiQiangAmmo(2);
        SpawnJiQiangAmmo(3);
        if (IsQianHouFire || IsJiQiangSanDanFire) {
			//if (!IsJiQiangSanDanFire) {
			//	SpawnJiQiangAmmo(2);
			//	SpawnJiQiangAmmo(3);
			//}

			if (IsJiQiangSanDanFire) {
				int max = AmmoStartPosOne.Length;
				if (max > 4) {
					for (int i = 4; i < max; i++) {
						SpawnJiQiangAmmo(i);
					}
				}
			}
		}
	}

    bool _IsQianHouFire = false;
    /// <summary>
    /// 前后发射子弹道具.
    /// 改装为添加2个小飞机给玩家坦克.
    /// </summary>
	bool IsQianHouFire
    {
        set
        {
            _IsQianHouFire = value;
            if (_IsQianHouFire == true)
            {
                ShowAllXiaoFeiJi();
            }
            else
            {
                HiddenAllXiaoFeiJi();
            }
        }
        get
        {
            return _IsQianHouFire;
        }
    }
	public void SetIsQianHouFire(bool isFire)
	{
		if (IsQianHouFire == isFire) {
			return;
		}
		IsQianHouFire = isFire;
		//Debug.Log("Unity:"+"SetIsQianHouFire -> isFire "+isFire);

		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.QianHouFireDJ : BuJiBaoType.Null;
		JiQiangAmmoSt = daoJuTypeVal;
		
		if (isFire) {
			IsJiQiangSanDanFire = false;
			IsChangChengFire = false;
			//IsQianHouFire = false;
			IsQiangJiFire = false;
		}
	}

	bool IsJiQiangSanDanFire = false;
	public void SetIsJiQiangSanDanFire(bool isFire)
	{
		if (IsJiQiangSanDanFire == isFire) {
			return;
		}
		//Debug.Log("Unity:"+"SetIsJiQiangSanDanFire -> isFire "+isFire);
		IsJiQiangSanDanFire = isFire;
		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.SanDanJiQiang : BuJiBaoType.Null;
		JiQiangAmmoSt = daoJuTypeVal;

		if (isFire) {
			//IsJiQiangSanDanFire = false;
			IsChangChengFire = false;
			IsQianHouFire = false;
			IsQiangJiFire = false;
		}
	}

	bool IsChangChengFire = false;
	public void SetIsChangChengFire(bool isFire)
	{
		if (IsChangChengFire == isFire) {
			return;
		}
		IsChangChengFire = isFire;
		//Debug.Log("Unity:"+"SetIsChangChengFire -> isFire "+isFire);
		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.ChangChengJiQiang : BuJiBaoType.Null;
		JiQiangAmmoSt = daoJuTypeVal;
		
		if (isFire) {
			IsJiQiangSanDanFire = false;
			//IsChangChengFire = false;
			IsQianHouFire = false;
			IsQiangJiFire = false;
		}
	}

	bool IsQiangJiFire = false;
	public void SetIsQiangJiFire(bool isFire)
	{
		if (IsQiangJiFire == isFire) {
			return;
		}
		IsQiangJiFire = isFire;
		//Debug.Log("Unity:"+"SetIsQiangJiFire -> isFire "+isFire);
		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.QiangJiJiQiang : BuJiBaoType.Null;
		JiQiangAmmoSt = daoJuTypeVal;

		if (isFire) {
			IsJiQiangSanDanFire = false;
			IsChangChengFire = false;
			IsQianHouFire = false;
			//IsQiangJiFire = false;
		}
	}

	bool IsPaiJiPaoFire = false;
	public void SetIsPaiJiPaoFire(bool isFire)
	{
		if (IsPaiJiPaoFire == isFire) {
			return;
		}
		IsPaiJiPaoFire = isFire;
		ChangePlayerPaoTaObj(isFire == true ? PlayerAmmoType.PaiJiPaoAmmo : PlayerAmmoType.Null);
		PlayerAmmoType ammoTypeVal = isFire == true ? PlayerAmmoType.PaiJiPaoAmmo : PlayerAmmoType.DaoDanAmmo;
		XKPlayerAutoFire.GetInstanceAutoFire(PlayerIndex).SetAmmoStateZhuPao(ammoTypeVal);
		//Debug.Log("Unity:"+"SetIsPaiJiPaoFire -> isFire "+isFire);
		
		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.PaiJiPaoDJ : BuJiBaoType.DaoDan;
		ZhuPaoAmmoSt = daoJuTypeVal;
		if (isFire) {
			IsSanDanZPFire = false;
			//IsPaiJiPaoFire = false;
		}
	}
	
	bool IsSanDanZPFire = false;
	public void SetIsSanDanZPFire(bool isFire)
	{
		if (IsSanDanZPFire == isFire) {
			return;
		}
		IsSanDanZPFire = isFire;
		ChangePlayerPaoTaObj(isFire == true ? PlayerAmmoType.SanDanAmmo : PlayerAmmoType.Null);
		PlayerAmmoType ammoTypeVal = isFire == true ? PlayerAmmoType.SanDanAmmo : PlayerAmmoType.DaoDanAmmo;
		XKPlayerAutoFire.GetInstanceAutoFire(PlayerIndex).SetAmmoStateZhuPao(ammoTypeVal);
		//Debug.Log("Unity:"+"SetIsSanDanZPFire -> isFire "+isFire);

		BuJiBaoType daoJuTypeVal = isFire == true ? BuJiBaoType.ZhuPaoSanDanDJ : BuJiBaoType.DaoDan;
		ZhuPaoAmmoSt = daoJuTypeVal;
		if (isFire) {
			//IsSanDanZPFire = false;
			IsPaiJiPaoFire = false;
		}
	}

	bool IsHuoLiAllOpen;
	public bool GetIsHuoLiAllOpen()
	{
		return IsHuoLiAllOpen;
	}

	public void SetIsHuoLiAllOpen(bool isFire)
	{
		//Debug.Log("Unity:"+"SetIsHuoLiAllOpen -> isFire "+isFire);
		if (isFire) {
			XKPlayerHuoLiAllOpenUICtrl huoLiUIScript = XKPlayerHuoLiAllOpenUICtrl.GetInstanceHuoLiOpen(PlayerIndex);
			if (huoLiUIScript != null) {
				huoLiUIScript.ShowHuoLiOpenUI(XKDaoJuGlobalDt.GetInstance().HuoLiAllOpenTime);
			}
		}

		if (IsHuoLiAllOpen == isFire) {
			return;
		}
		IsHuoLiAllOpen = isFire;
		//Debug.Log("Unity:"+"SetIsSanDanZPFire -> isFire "+isFire);
		
		BuJiBaoType daoJuTypeValZP = isFire == true ? BuJiBaoType.HuoLiAllOpenDJ : BuJiBaoType.DaoDan;
		BuJiBaoType daoJuTypeValJQ = isFire == true ? BuJiBaoType.HuoLiAllOpenDJ : BuJiBaoType.Null;
		ZhuPaoAmmoSt = daoJuTypeValZP;
		JiQiangAmmoSt = daoJuTypeValJQ;
	}

	/**
	 * paoKou == 0 -> 机枪炮口.
	 * paoKou == 1 -> 主炮炮口.
	 */
	void CheckPlayerHouZuoLi(BuJiBaoType daoJuType, int paoKou)
	{
		float pushPower = 50f;
		switch (daoJuType) {
		case BuJiBaoType.QianHouFireDJ:
			pushPower = XKPlayerGlobalDt.GetInstance().QianHouFirePTPower;
			break;
		case BuJiBaoType.ChangChengJiQiang:
			pushPower = XKPlayerGlobalDt.GetInstance().ChangChengFirePTPower;
			break;
		case BuJiBaoType.SanDanJiQiang:
			pushPower = XKPlayerGlobalDt.GetInstance().SanDanFirePTPower;
			break;
		case BuJiBaoType.QiangJiJiQiang:
			pushPower = XKPlayerGlobalDt.GetInstance().QiangJiFirePTPower;
			break;
		case BuJiBaoType.ChuanTouDan:
			pushPower = XKPlayerGlobalDt.GetInstance().ChuanJiaDanZPPower;
			break;
		case BuJiBaoType.PaiJiPaoDJ:
			pushPower = XKPlayerGlobalDt.GetInstance().PaiJiPaoZPPower;
			break;
		case BuJiBaoType.ZhuPaoSanDanDJ:
			pushPower = XKPlayerGlobalDt.GetInstance().SanDanZPPower;
			break;
		case BuJiBaoType.HuoLiAllOpenDJ:
			if (paoKou == JI_QIANG_INDEX) {
				pushPower = XKPlayerGlobalDt.GetInstance().HuoLiQuanKaiPTPower;
			}

			if (paoKou == ZHU_PAO_INDEX) {
				pushPower = XKPlayerGlobalDt.GetInstance().HuoLiQuanKaiZPPower;
			}
			break;
		default:
			if (paoKou == JI_QIANG_INDEX) {
				pushPower = XKPlayerGlobalDt.GetInstance().JiQiangFirePTPower;
			}
			
			if (paoKou == ZHU_PAO_INDEX) {
				pushPower = XKPlayerGlobalDt.GetInstance().ZhuPaoFireZPPower;
			}
			break;
		}

		Vector3 pushDir = IsLockPaoTa == false ? -TanKeTran.forward : -PaoTaTr.forward;
		if (IsLockPaoTa && CameraTran != null) {
			Vector3 veA = CameraTran.forward;
			Vector3 veB = TanKeTran.forward;
			veA.y = veB.y = 0f;
			float angleVal = Mathf.Clamp(Vector3.Angle(veA, veB), 0f, 90f);
			if (angleVal > 5f) {
				return;
			}
		}
		PlayerMoveScript.PushPlayerTanKe(pushDir, pushPower);
	}

	void CheckPlayerZhuPaoFireBt()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}
		
//		if (!XkGameCtrl.IsActivePlayerTwo) {
//			return;
//		}

        
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //激活主角Ai坦克状态.
            if (XkGameCtrl.GetInstance().m_AiPathGroup.m_CameraMoveType == AiPathGroupCtrl.MoveState.YuLe)
            {
                if (Random.Range(0, 100) % 3 != 0)
                {
                    return;
                }
            }
            else
            {
                if (Random.Range(0, 100) % 30 != 0)
                {
                    return;
                }
            }
        }
        else
        {
            if (!IsActiveFireBtZP)
            {
                return;
            }

            if (!CheckIsActivePlayer())
            {
                return;
            }
        }
		
//		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
//			return;
//		}

		if (Camera.main == null) {
			return;
		}
		
		if (GameOverCtrl.IsShowGameOver || JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			if (GameOverCtrl.IsShowGameOver) {
				IsActiveFireBtZP = false;
			}
			return;
		}

		bool isSpawnAmmo = CheckIsSpawnPlayerAmmo(ZHU_PAO_INDEX);
		if (!isSpawnAmmo) {
			return;
		}
		LastFireTimeZhuPao = Time.time;
		CheckPlayerHouZuoLi(ZhuPaoAmmoSt, ZHU_PAO_INDEX);

		Vector3 ammoSpawnForward = AmmoStartPosZP[0].forward;
		Vector3 ammoSpawnPos = AmmoStartPosZP[0].position;
		Quaternion ammoSpawnRot = AmmoStartPosZP[0].rotation;
		GameObject obj = null;
		CheckFireAudioPlayerZhuPao();
		//pcvr.OpenZuoYiQiNang(PlayerIndex);
		//pcvr.GetInstance().ActiveFangXiangDouDong(PlayerIndex, false);

		obj = SpawnPlayerAmmoByAmmoType(ZHU_PAO_INDEX, ammoSpawnPos, ammoSpawnRot);
		if (obj == null) {
			return;
		}
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;

		PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
		Vector3 mousePosInput = Input.mousePosition;
		//if (pcvr.bIsHardWare) {
			//mousePosInput = pcvr.CrossPositionTwo;
		//}
		
		Vector3 firePos = Vector3.zero;
		Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
		Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 ammoForward = Vector3.Normalize( posTmp - ammoSpawnPos );
		RaycastHit hit;
		if (!IsPSAutoFire) {
			firePos = FirePosValTmp * ammoSpawnForward + ammoSpawnPos;
			FireRayDirLen = ammoScript.MvSpeed * ammoScript.LiveTime;
			if (Physics.Raycast(ammoSpawnPos, ammoSpawnForward, out hit, FireRayDirLen, FireLayer.value)) {
				//Debug.Log("Unity:"+"Player fire obj -> "+hit.collider.name);
				if (ammoScript.AmmoType != PlayerAmmoType.ChuanTouAmmo) {
					firePos = hit.point;
				}
			}
		}
		else {
			ammoForward = obj.transform.forward;
			firePos = FirePosValTmp * ammoForward + ammoSpawnPos;
			if (Physics.Raycast(ammoSpawnPos, ammoForward, out hit, FireRayDirLen, FireLayer.value)) {
				//Debug.Log("Unity:"+"Player fire obj -> "+hit.collider.name);
				if (ammoScript.AmmoType != PlayerAmmoType.ChuanTouAmmo) {
					firePos = hit.point;
				}
			}
		}
		ammoScript.StartMoveAmmo(firePos, PlayerIndex);
	}

	public static int PlayerAmmoNumTest;
	GameObject ChangChengAmmo;
	GameObject QiangJiAmmo;
	GameObject PaiJiPaoAmmo;
	GameObject AmmoHuoLiOpenJQ;
	GameObject AmmoHuoLiOpenZP;
	GameObject GetPlayerAmmo(PlayerAmmoType ammoType, Vector3 ammoPos, Quaternion ammoRot, int ammoIndex = 0)
	{
		int max = 0;
		GameObject objAmmo = null;
		GameObject ammoPrefab = PuTongAmmo;
		PlayerAmmoCtrl ammoScript = null;
		PlayerAmmoCtrl ammoPrefabScript = null;
		bool isFindAmmo = false;
		//Debug.Log("Unity:"+"ammoType "+ammoType+", ammoIndex "+ammoIndex+", GetPlayerAmmo");
		switch (ammoType) {
		case PlayerAmmoType.PuTongAmmo:
			max = AmmoList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoList_TK[i].gameObject.activeSelf) {
					ammoPrefabScript = PuTongAmmo.GetComponent<PlayerAmmoCtrl>();
					if (IsChangChengFire) {
						ammoPrefabScript = ChangChengAmmo.GetComponent<PlayerAmmoCtrl>();
					}

					if (IsQiangJiFire) {
						ammoPrefabScript = QiangJiAmmo.GetComponent<PlayerAmmoCtrl>();
					}

					if (IsHuoLiAllOpen) {
						ammoPrefabScript = AmmoHuoLiOpenJQ.GetComponent<PlayerAmmoCtrl>();
					}

					ammoScript = AmmoList_TK[i].gameObject.GetComponent<PlayerAmmoCtrl>();
					if (ammoPrefabScript.AmmoIndex == ammoScript.AmmoIndex) {
						objAmmo = AmmoList_TK[i].gameObject;
						isFindAmmo = true;
						break;
					}
				}
			}

			if (!isFindAmmo) {
				objAmmo = null;
			}
			
			if (objAmmo == null) {
				if (IsChangChengFire) {
					ammoPrefab = ChangChengAmmo;
				}

				if (IsQiangJiFire) {
					ammoPrefab = QiangJiAmmo;
				}

				if (IsHuoLiAllOpen) {
					ammoPrefab = AmmoHuoLiOpenJQ;
				}
				objAmmo = SpawnPlayerAmmo(ammoPrefab, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;
		
		case PlayerAmmoType.DaoDanAmmo:
			max = AmmoDaoDanList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoDaoDanList_TK[i].gameObject.activeSelf) {
					ammoPrefabScript = DaoDanZPAmmo.GetComponent<PlayerAmmoCtrl>();
					if (IsHuoLiAllOpen) {
						ammoPrefabScript = AmmoHuoLiOpenZP.GetComponent<PlayerAmmoCtrl>();
					}
					
					ammoScript = AmmoDaoDanList_TK[i].gameObject.GetComponent<PlayerAmmoCtrl>();
					if (ammoPrefabScript.AmmoIndex == ammoScript.AmmoIndex) {
						objAmmo = AmmoDaoDanList_TK[i].gameObject;
						isFindAmmo = true;
						break;
					}
				}
			}
			
			if (!isFindAmmo) {
				objAmmo = null;
			}
			
			if (objAmmo == null) {
				ammoPrefab = DaoDanZPAmmo;
				if (IsHuoLiAllOpen) {
					ammoPrefab = AmmoHuoLiOpenZP;
				}

				objAmmo = SpawnPlayerAmmo(ammoPrefab, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;

		/*case PlayerAmmoType.GaoBaoAmmo:
			max = AmmoGaoBaoList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoGaoBaoList_TK[i].gameObject.activeSelf) {
					objAmmo = AmmoGaoBaoList_TK[i].gameObject;
					break;
				}
			}

			if (objAmmo == null) {
				objAmmo = SpawnPlayerAmmo(GaoBaoDanAmmo, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;*/
			
		case PlayerAmmoType.SanDanAmmo:
			max = AmmoSanDanList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoSanDanList_TK[i].gameObject.activeSelf) {
					if (ammoIndex == 1) {
						ammoPrefabScript = AmmoSanDanZP.GetComponent<PlayerAmmoCtrl>();
					}
					ammoScript = AmmoSanDanList_TK[i].gameObject.GetComponent<PlayerAmmoCtrl>();

					if (ammoPrefabScript.AmmoIndex == ammoScript.AmmoIndex) {
						objAmmo = AmmoSanDanList_TK[i].gameObject;
						isFindAmmo = true;
						break;
					}
				}
			}

			if (!isFindAmmo) {
				objAmmo = null;
			}

			if (objAmmo == null) {
				if (ammoIndex == 1) {
					objAmmo = SpawnPlayerAmmo(AmmoSanDanZP, ammoPos, ammoRot);
				}
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;
			
		/*case PlayerAmmoType.GenZongAmmo:
			max = AmmoGenZongDanList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoGenZongDanList_TK[i].gameObject.activeSelf) {
					objAmmo = AmmoGenZongDanList_TK[i].gameObject;
					break;
				}
			}
			
			if (objAmmo == null) {
				objAmmo = SpawnPlayerAmmo(GenZongDanAmmo, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;*/

		case PlayerAmmoType.PaiJiPaoAmmo:
			max = AmmoPaiJiPaoList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoPaiJiPaoList_TK[i].gameObject.activeSelf) {
					objAmmo = AmmoPaiJiPaoList_TK[i].gameObject;
					break;
				}
			}
			
			if (objAmmo == null) {
				objAmmo = SpawnPlayerAmmo(PaiJiPaoAmmo, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;

		case PlayerAmmoType.ChuanTouAmmo:
			max = AmmoChuanTouDanList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoChuanTouDanList_TK[i].gameObject.activeSelf) {
					objAmmo = AmmoChuanTouDanList_TK[i].gameObject;
					break;
				}
			}
			
			if (objAmmo == null) {
				objAmmo = SpawnPlayerAmmo(ChuanTouDanAmmo, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;
			
		/*case PlayerAmmoType.JianSuAmmo:
			max = AmmoJianSuDanList_TK.Count;
			for (int i = 0; i < max; i++) {
				if (!AmmoJianSuDanList_TK[i].gameObject.activeSelf) {
					objAmmo = AmmoJianSuDanList_TK[i].gameObject;
					break;
				}
			}
			
			if (objAmmo == null) {
				objAmmo = SpawnPlayerAmmo(JianSuDanAmmo, ammoPos, ammoRot);
				HandleAmmoList( objAmmo.GetComponent<PlayerAmmoCtrl>() );
			}
			break;*/
		}

		if (objAmmo != null) {
			objAmmo.transform.position = ammoPos;
			objAmmo.transform.rotation = ammoRot;
		}
		return objAmmo;
	}

	void HandleAmmoList(PlayerAmmoCtrl scriptAmmo)
	{
		PlayerAmmoType ammoType = scriptAmmo.AmmoType;
		switch (ammoType) {
		case PlayerAmmoType.PuTongAmmo:
			if (AmmoList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoList_TK.Add(scriptAmmo);
			break;

		case PlayerAmmoType.DaoDanAmmo:
			if (AmmoDaoDanList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoDaoDanList_TK.Add(scriptAmmo);
			break;

		case PlayerAmmoType.GaoBaoAmmo:
			if (AmmoGaoBaoList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoGaoBaoList_TK.Add(scriptAmmo);
			break;
			
		case PlayerAmmoType.SanDanAmmo:
			if (AmmoSanDanList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoSanDanList_TK.Add(scriptAmmo);
			break;
			
		case PlayerAmmoType.GenZongAmmo:
			if (AmmoGenZongDanList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoGenZongDanList_TK.Add(scriptAmmo);
			break;

		case PlayerAmmoType.PaiJiPaoAmmo:
			if (AmmoPaiJiPaoList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoPaiJiPaoList_TK.Add(scriptAmmo);
			break;
			
		case PlayerAmmoType.ChuanTouAmmo:
			if (AmmoChuanTouDanList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoChuanTouDanList_TK.Add(scriptAmmo);
			break;
			
		case PlayerAmmoType.JianSuAmmo:
			if (AmmoJianSuDanList_TK.Contains(scriptAmmo)) {
				return;
			}
			AmmoJianSuDanList_TK.Add(scriptAmmo);
			break;
		}
	}

	void ClickFireBtEvent(pcvr.ButtonState state)
	{
		//Debug.Log("Unity:"+"ClickFireBtOneEvent***state "+state);
		if (state == pcvr.ButtonState.DOWN)
        {
			SetIsActiveFireBtJQ(true);
			LastFireTimeJiQiang = -100f;
			CheckPlayerJiQiangFireBt();
            //if (pcvr.IsHongDDShouBing)
            //{
            //    ClickFireDaoDanBtEvent(pcvr.ButtonState.UP);
            //}
        }
		else
        {
			SetIsActiveFireBtJQ(false);
		}
    }

    IEnumerator DelayResetFire()
    {
        //Debug.Log("Unity:"+"DelayResetFire...111");
        yield return new WaitForSeconds(0.5f);
        ClickFireBtEvent(pcvr.ButtonState.UP);
        //Debug.Log("Unity:"+"DelayResetFire...222");
    }

    void ClickFireDaoDanBtEvent(pcvr.ButtonState state)
	{
		//Debug.Log("Unity:"+"ClickFireBtOneEvent***state "+state);
		if (state == pcvr.ButtonState.DOWN)
        {
            IsActiveFireBtZP = true;
            //if (pcvr.IsHongDDShouBing)
            //{
            //    ClickFireBtEvent(pcvr.ButtonState.UP);
            //}
        }
		else
        {
			IsActiveFireBtZP = false;
		}
	}

    IEnumerator DelayResetFireDaoDan()
    {
        //Debug.Log("Unity:"+"DelayResetFireDaoDan...111");
        yield return new WaitForSeconds(0.1f);
        ClickFireDaoDanBtEvent(pcvr.ButtonState.UP);
        //Debug.Log("Unity:"+"DelayResetFireDaoDan...222");
    }

	public void SpawnPlayerDaoDan(Transform ammoTran, GameObject playerDaoDan)
	{
		//Debug.Log("Unity:"+"SpawnPlayerDaoDan***");
		Vector3 ammoSpawnPos = ammoTran.position;
		GameObject obj = SpawnPlayerAmmo(playerDaoDan, ammoSpawnPos, ammoTran.rotation);
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
		RaycastHit hitInfo;
		float disVal = Random.Range(300f, 500f);
		Vector3 forwardVal = ammoTran.forward;
		Vector3 firePos = ammoSpawnPos + (forwardVal * disVal);
		Physics.Raycast(ammoSpawnPos, forwardVal, out hitInfo, disVal, FireLayer.value);
		if (hitInfo.collider != null){
			firePos = hitInfo.point;
		}
		ammoScript.StartMoveAmmo(firePos, PlayerEnum.Null);
	}

	/**
	 * JiQiangFireAudioState[index] == 0 -> 表示没有开枪.
	 * JiQiangFireAudioState[index] == 1 -> 表示有开枪.
	 */
	static int[] JiQiangFireAudioState = {0, 0, 0, 0};
	bool CheckJiQiangFireAudioState()
	{
		bool isPlay = true;
		int max = (int)PlayerIndex - 1;
		for (int i = 0; i < max; i++) {
			if (JiQiangFireAudioState[i] == 1) {
				isPlay = false;
				//Debug.Log("Unity:"+"CheckJiQiangFireAudioState -> PlayerIndex "+PlayerIndex);
				break;
			}
		}
		return isPlay;
	}

	bool CheckIsCanPlayFireAudioJQ()
	{
		if (!IsActiveFireBtJQ) {
			return false;
		}

		bool isPlay = false;
		switch (PlayerIndex) {
		case PlayerEnum.PlayerOne:
			isPlay = true;
			break;
		case PlayerEnum.PlayerTwo:
		case PlayerEnum.PlayerThree:
		case PlayerEnum.PlayerFour:
			isPlay = CheckJiQiangFireAudioState();
			break;
		}
		return isPlay;
	}

	void CheckFireAudioPlayerJiQiang()
	{
		if (!CheckIsCanPlayFireAudioJQ()) {
			return;
		}

		AudioSource audioSoureCom = PlayerFireAudio[0];
		if (IsQianHouFire) {
			audioSoureCom = PlayerFireAudio[1];
		}

		if (IsChangChengFire) {
			audioSoureCom = PlayerFireAudio[2];
		}

		if (IsJiQiangSanDanFire) {
			audioSoureCom = PlayerFireAudio[3];
		}
		
		if (IsQiangJiFire) {
			audioSoureCom = PlayerFireAudio[4];
		}

		if (IsHuoLiAllOpen) {
			audioSoureCom = PlayerFireAudio[9];
		}
		PlayFireAudio(audioSoureCom);
	}
	
	void CheckFireAudioPlayerZhuPao()
	{
		if (!IsActiveFireBtZP) {
			return;
		}
		
		AudioSource audioSoureCom = PlayerFireAudio[5];
		if (ZhuPaoAmmoSt == BuJiBaoType.ChuanTouDan) {
			audioSoureCom = PlayerFireAudio[6];
		}
		
		if (IsPaiJiPaoFire) {
			audioSoureCom = PlayerFireAudio[7];
		}
		
		if (IsSanDanZPFire) {
			audioSoureCom = PlayerFireAudio[8];
		}
		
		if (IsHuoLiAllOpen) {
			audioSoureCom = PlayerFireAudio[9];
		}
		PlayFireAudio(audioSoureCom);
	}

	void CheckPSTriggerAutoFire()
	{
		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		PlayerAmmoType ammoType = PSTriggerCamera.AutoFirePlayerAmmoTypeVal;
		if (PSAmmoTypeVal == ammoType) {
			switch (PSAmmoTypeVal) {
			case PlayerAmmoType.PuTongAmmo:
			case PlayerAmmoType.GaoBaoAmmo:
				if (!IsActiveFireBtJQ) {
					SetIsActiveFireBtJQ(true);
				}

				if (!IsActiveFireBtZP) {
					IsActiveFireBtZP = true;
				}

				if (PSAmmoTypeVal == PlayerAmmoType.GaoBaoAmmo) {
					XkGameCtrl.GaoBaoDanNumPOne = 9999;
					XkGameCtrl.GaoBaoDanNumPTwo = 9999;
				}
				else {
					XkGameCtrl.GaoBaoDanNumPOne = 0;
					XkGameCtrl.GaoBaoDanNumPTwo = 0;
				}
				break;

			case PlayerAmmoType.DaoDanAmmo:
//				ClickDaoDanBtOneEvent(pcvr.ButtonState.DOWN);
//				ClickDaoDanBtTwoEvent(pcvr.ButtonState.DOWN);
				break;

			default:
				if (IsActiveFireBtJQ) {
					SetIsActiveFireBtJQ(false);
				}
				
				if (IsActiveFireBtZP) {
					IsActiveFireBtZP = false;
				}
				break;
			}
			return;
		}
		PSAmmoTypeVal = ammoType;

		switch (PSAmmoTypeVal) {
		case PlayerAmmoType.DaoDanAmmo:
		case PlayerAmmoType.GaoBaoAmmo:
		case PlayerAmmoType.PuTongAmmo:
			IsPSAutoFire = true;
			break;
		default:
			IsPSAutoFire = false;
			break;
		}
	}

	void PlayFireAudio(AudioSource audioVal)
	{
		if (audioVal == null) {
			return;
		}

		if (audioVal.isPlaying) {
			audioVal.Stop();
		}
		audioVal.Play();
	}

	void SetFireAudioVolume(AudioSource audioVal, float volumeVal = 1)
	{
		if (audioVal == null) {
			return;
		}

		if (audioVal.volume == volumeVal) {
			return;
		}
		audioVal.volume = volumeVal;
	}

	void SpawnPlayerAmmoParticle(GameObject ammoParticle, Vector3 pos, Quaternion rot)
	{
		if (ammoParticle == null) {
			return;
		}
		GameObject obj = (GameObject)Instantiate(ammoParticle, pos, rot);
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);
	}

	/**
	 * ammoType == 0 -> 主角机枪.
	 * ammoType == 1 -> 主炮炮管.
	 */
	GameObject SpawnPlayerAmmoByAmmoType(int ammoIndex, Vector3 ammoSpawnPos, Quaternion ammoSpawnRot)
	{
		GameObject obj = null;
		GameObject ammoParticle = null;
		PlayerAmmoType ammoType = PlayerAmmoType.PuTongAmmo;
		switch (ammoIndex) {
		case JI_QIANG_INDEX:
			ammoType = AmmoStateJiQiang;
			break;
		case ZHU_PAO_INDEX:
			ammoType = AmmoStateZhuPao;
			break;
		}

		if (ammoIndex == ZHU_PAO_INDEX) {
			switch (ammoType) {
			case PlayerAmmoType.ChuanTouAmmo:
				ZhuPaoAmmoSt = BuJiBaoType.ChuanTouDan;
				break;
			case PlayerAmmoType.DaoDanAmmo:
				ZhuPaoAmmoSt = BuJiBaoType.DaoDan;
				break;
			}
		}

		//Debug.Log("Unity:"+"ammoType "+ammoType+", PlayerIndex "+PlayerIndex+", SpawnPlayerAmmoByAmmoType");
		switch (ammoType) {
		case PlayerAmmoType.PuTongAmmo:
			ammoParticle = AmmoParticle[ammoIndex];
			break;

			/*case PlayerAmmoType.GaoBaoAmmo:
			ammoParticle = GaoBaoAmmoParticle[ammoIndex];
			//XkGameCtrl.GetInstance().SubGaoBaoDanNum(PlayerIndex);
			break;*/
			
		case PlayerAmmoType.SanDanAmmo:
			if (ammoIndex == 0) {
				return obj;
			}
			ammoParticle = SanDanAmmoParticle[ammoIndex];
			//XkGameCtrl.GetInstance().SubSanDanNum(PlayerIndex);
			SpawnPlayerAllSanDanAmmo(ammoIndex);
			break;
			
			/*case PlayerAmmoType.GenZongAmmo:
			ammoParticle = GenZongDanAmmoParticle[ammoIndex];
			//XkGameCtrl.GetInstance().SubGenZongDanNum(PlayerIndex);
			break;*/
			
		case PlayerAmmoType.PaiJiPaoAmmo:
			ammoParticle = PaiJiPaoAmmoParticle[ammoIndex];
			break;
			
			/*case PlayerAmmoType.JianSuAmmo:
			ammoParticle = JianSuDanAmmoParticle[ammoIndex];
			//XkGameCtrl.GetInstance().SubJianSuDanNum(PlayerIndex);
			break;*/
			
		case PlayerAmmoType.ChuanTouAmmo:
			ammoParticle = ChuanTouDanAmmoParticle[ammoIndex];
			//XkGameCtrl.GetInstance().SubChuanTouDanNum(PlayerIndex);
			break;
			
		case PlayerAmmoType.DaoDanAmmo:
			ammoParticle = DaoDanAmmoParticle[ammoIndex];
			break;
		}
		SpawnPlayerAmmoParticle(ammoParticle, ammoSpawnPos, ammoSpawnRot);
		obj = GetPlayerAmmo(ammoType, ammoSpawnPos, ammoSpawnRot, ammoIndex);
		return obj;
	}

	void SpawnPlayerAllSanDanAmmo(int ammoIndex)
	{
		//GameObject obj = null;
		int max = 7;
		switch (ammoIndex) {
			/*case JI_QIANG_INDEX:
			max = AmmoStartPosOne.Length;
			for (int i = 1; i < max; i++) {
				SpawnPlayerAmmoParticle(SanDanAmmoParticle[ammoIndex],
				                        AmmoStartPosZP[i].position,
				                        AmmoStartPosZP[i].rotation);
				SpawnPlayerSanDanAmmo(AmmoStartPosOne[i]);
			}
			break;*/
			
		case ZHU_PAO_INDEX:
			max = AmmoStartPosZP.Length;
			for (int i = 1; i < max; i++) {
				/*SpawnPlayerAmmoParticle(SanDanAmmoParticle[ammoIndex],
				                        AmmoStartPosZP[i].position,
				                        AmmoStartPosZP[i].rotation);*/
				SpawnPlayerSanDanAmmo(AmmoStartPosZP[i], 1);
			}
			break;
		}
	}

	/**
	 * ammoIndex == 0 -> 普通机枪散弹.
	 * ammoIndex == 1 -> 主炮散弹.
	 */
	void SpawnPlayerSanDanAmmo(Transform AmmoStartTran, int ammoIndex = 0)
	{
		if (AmmoStartTran == null) {
			return;
		}
		Vector3 ammoSpawnForward = AmmoStartTran.forward;
		Vector3 ammoSpawnPos = AmmoStartTran.position;
		Quaternion ammoSpawnRot = AmmoStartTran.rotation;
		GameObject obj = GetPlayerAmmo(PlayerAmmoType.SanDanAmmo, ammoSpawnPos, ammoSpawnRot, ammoIndex);
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
		PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
		
		RaycastHit hit;
		Vector3 firePos = Vector3.zero;
		firePos = FirePosValTmp * ammoSpawnForward + ammoSpawnPos;
		if (Physics.Raycast(ammoSpawnPos, ammoSpawnForward, out hit, FireRayDirLen, FireLayer.value)) {
			//Debug.Log("Unity:"+"Player fire obj -> "+hit.collider.name);
			firePos = hit.point;
		}
		ammoScript.StartMoveAmmo(firePos, PlayerIndex);
	}
	
	public void SetAmmoStateJiQiang(PlayerAmmoType ammoType)
	{
		AmmoStateJiQiang = ammoType;
	}

	public void SetAmmoStateZhuPao(PlayerAmmoType ammoType)
	{
		AmmoStateZhuPao = ammoType;
		if (ammoType == PlayerAmmoType.ChuanTouAmmo) {
			IsSanDanZPFire = false;
			IsPaiJiPaoFire = false;
			ChangePlayerPaoTaObj(ammoType);
		}
		else if (ammoType == PlayerAmmoType.SanDanAmmo) {
			IsPaiJiPaoFire = false;
			ChangePlayerPaoTaObj(ammoType);
		}
		else if (ammoType == PlayerAmmoType.PaiJiPaoAmmo) {
			IsPaiJiPaoFire = false;
			ChangePlayerPaoTaObj(ammoType);
		}
		else {
			ChangePlayerPaoTaObj(PlayerAmmoType.Null);
		}
	}

	void SetIsActiveFireBtJQ(bool isFire)
	{
		IsActiveFireBtJQ = isFire;
		int indexVal = (int)PlayerIndex - 1;
		JiQiangFireAudioState[indexVal] = IsActiveFireBtJQ == true ? 1 : 0;
	}
	
	void UpdatePaoTaRot()
	{
		if (!IsLockPaoTa) {
			return;
		}
		
		if (CameraTran == null) {
			CameraTran = Camera.main != null ? Camera.main.transform : null;
			return;
		}
		Vector3 forwardVal = CameraTran.forward;
		forwardVal.y = 0f;
		PaoTaFireTr.forward = forwardVal;

		PaoTaTr.forward = forwardVal;
		Vector3 locAngle = PaoTaTr.localEulerAngles;
		locAngle.x = locAngle.z = 0f;
		PaoTaTr.localEulerAngles = locAngle;
	}

	void ChangePlayerPaoTaObj(PlayerAmmoType ammoType)
	{
		int indexJH = 0;
		switch (ammoType) {
		case PlayerAmmoType.ChuanTouAmmo:
			indexJH = 1;
			break;
		case PlayerAmmoType.SanDanAmmo:
			indexJH = 2;
			break;
		case PlayerAmmoType.PaiJiPaoAmmo:
			indexJH = 3;
			break;
		}

//		#if UNITY_EDITOR
//		Debug.Log("Unity:"+"ChangePlayerPaoTaObj -> indexJH "+indexJH);
//		#endif
		
		for (int i = 0; i < 4; i++) {
			if (PaoTaRealObj.Length >= 4 && PaoTaRealObj[i] != null) {
				PaoTaRealObj[i].SetActive(indexJH == i);
			}
		}
	}

    public void ResetInfo()
    {
        IsHuoLiAllOpen = false;
        IsPaiJiPaoFire = false;
        IsQiangJiFire = false;
        IsQianHouFire = false;
        IsSanDanZPFire = false;
    }

    public Transform[] m_XiaoFeiJiFollowPoints = new Transform[2];
    /// <summary>
    /// 玩家小飞机数据列表.
    /// </summary>
    List<SSPlayerXiaoFeiJi> m_PlayerXiaoFeiJiList = new List<SSPlayerXiaoFeiJi>();
    /// <summary>
    /// 创建玩家小飞机.
    /// </summary>
    void CreatPlayerXiaoFeiJi()
    {
        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/Player/PlayerXiaoFeiJi");
        if (gmDataPrefab != null)
        {
            GameObject obj = null;
            SSPlayerXiaoFeiJi xiaoFeiJi = null;
            int length = m_XiaoFeiJiFollowPoints.Length;
            for (int i = 0; i < length; i++)
            {
                if (m_XiaoFeiJiFollowPoints[i] != null)
                {
                    obj = (GameObject)Instantiate(gmDataPrefab);
                    xiaoFeiJi = obj.GetComponent<SSPlayerXiaoFeiJi>();
                    if (xiaoFeiJi != null)
                    {
                        xiaoFeiJi.Init(this, m_XiaoFeiJiFollowPoints[i]);
                        m_PlayerXiaoFeiJiList.Add(xiaoFeiJi);
                    }
                    else
                    {
                        Debug.LogWarning("Unity: CreatPlayerXiaoFeiJi -> xiaoFeiJi was null");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 显示所有小飞机.
    /// </summary>
    void ShowAllXiaoFeiJi()
    {
        int length = m_PlayerXiaoFeiJiList.Count;
        for (int i = 0; i < length; i++)
        {
            if (m_PlayerXiaoFeiJiList[i] != null)
            {
                m_PlayerXiaoFeiJiList[i].ShowSelf();
            }
        }
    }

    /// <summary>
    /// 隐藏所有小飞机.
    /// </summary>
    void HiddenAllXiaoFeiJi()
    {
        int length = m_PlayerXiaoFeiJiList.Count;
        for (int i = 0; i < length; i++)
        {
            if (m_PlayerXiaoFeiJiList[i] != null)
            {
                m_PlayerXiaoFeiJiList[i].HiddenSelf();
            }
        }
    }

    /// <summary>
    /// 使所有小飞机发射子弹.
    /// </summary>
    void MakeAllXiaoFeiJiFire()
    {
        int length = m_PlayerXiaoFeiJiList.Count;
        for (int i = 0; i < length; i++)
        {
            if (m_PlayerXiaoFeiJiList[i] != null)
            {
                m_PlayerXiaoFeiJiList[i].CreatFeiJiAmmo();
            }
        }
    }
}