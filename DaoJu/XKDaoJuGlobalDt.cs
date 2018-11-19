using UnityEngine;

public class XKDaoJuGlobalDt : MonoBehaviour
{
    /**
	 * 极速道具配置信息.
	 */
    [Range(1f, 100f)]public float JiSuSpeedVal = 60f;
	/**
	 * 极速道具摇杆版移动速度.
	 */
	[Range(1f, 500f)]public float JiSuSpeedYGVal = 60f;
	/**
	 * 极速道具摇杆版横向移动速度倍率.
	 */
	[Range(0f, 500f)]public float HorizontalSpeedYGBL = 0.5f;
	[Range(1f, 100f)]public float JiSuTimeVal = 3f;
	/**
	 * 医疗包道具配置信息.
	 */
	[Range(1f, 10000f)]public float YiLiaoBaoXueLiangVal = 100f;
	/**
	 * 双倍分数配置信息.
	 */
	[Range(1f, 100f)]public float TimeShuangBeiVal = 10f;
	/**
	 * 长程机枪配置信息.
	 * 长程机枪子弹.
	 */
	public GameObject AmmoChangCheng;
	const int AmmoChangChengIndex = 1;
	const PlayerAmmoType AmmoChangChengType = PlayerAmmoType.PuTongAmmo;
	/**
	 * 强击机枪配置信息.
	 * 强击机枪子弹.
	 */
	public GameObject AmmoQiangJi;
	const int AmmoQiangJiIndex = 2;
	const PlayerAmmoType AmmoQiangJiType = PlayerAmmoType.PuTongAmmo;
	/**
	 * 迫击炮道具配置信息.
	 * 迫击炮主炮子弹.
	 */
	public GameObject AmmoPaiJiPao;
	const int AmmoPaiJiPaoIndex = 0;
	const PlayerAmmoType AmmoPaiJiPaoType = PlayerAmmoType.PaiJiPaoAmmo;
	/**
	 * 散弹主炮道具配置信息.
	 * 散弹主炮子弹.
	 */
	public GameObject AmmoSanDanZP;
	const int AmmoSanDanZPIndex = 1;
	const PlayerAmmoType AmmoSanDanZPType = PlayerAmmoType.SanDanAmmo;
	/**
	 * 穿甲弹主炮道具配置信息.
	 * 穿甲弹主炮子弹.
	 */
	public GameObject AmmoChuanJiaDanZP;
	const int AmmoChuanJiaDanZPIndex = 0;
	const PlayerAmmoType AmmoChuanJiaDanZPType = PlayerAmmoType.ChuanTouAmmo;
	/**
	 * 火力全开道具配置信息.
	 * 机枪子弹.
	 */
	public GameObject AmmoHuoLiOpenJQ;
	const int AmmoHuoLiOpenJQIndex = 3;
	const PlayerAmmoType AmmoHuoLiOpenJQType = PlayerAmmoType.PuTongAmmo;
	/**
	 * 火力全开道具配置信息.
	 * 主炮火力全开子弹.
	 */
	public GameObject AmmoHuoLiOpenZP;
	const int AmmoHuoLiOpenZPIndex = 1;
	const PlayerAmmoType AmmoHuoLiOpenZPType = PlayerAmmoType.DaoDanAmmo;
	[Range(1f, 100f)]public float HuoLiAllOpenTime = 5f;
	[Range(0f, 30f)]public float CiLiDaoJuDis = 3f; //磁力检测距离.
	[Range(0.1f, 50f)]public float CiLiDaoJuSpeed = 10f; //磁力激活后道具飞行的速度.
	[Range(0f, 1000f)]public float DaoJuMaoZiPY = 0f; //道具冒字时高度的偏移量.
	static XKDaoJuGlobalDt _Instance;
	public static XKDaoJuGlobalDt GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
		PlayerAmmoCtrl ammoScript = AmmoChangCheng.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoChangChengIndex;
		ammoScript.AmmoType = AmmoChangChengType;

		ammoScript = AmmoQiangJi.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoQiangJiIndex;
		ammoScript.AmmoType = AmmoQiangJiType;

		ammoScript = AmmoPaiJiPao.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoPaiJiPaoIndex;
		ammoScript.AmmoType = AmmoPaiJiPaoType;

		ammoScript = AmmoSanDanZP.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoSanDanZPIndex;
		ammoScript.AmmoType = AmmoSanDanZPType;

		ammoScript = AmmoChuanJiaDanZP.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoChuanJiaDanZPIndex;
		ammoScript.AmmoType = AmmoChuanJiaDanZPType;

		ammoScript = AmmoHuoLiOpenZP.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoHuoLiOpenZPIndex;
		ammoScript.AmmoType = AmmoHuoLiOpenZPType;
		
		ammoScript = AmmoHuoLiOpenJQ.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoHuoLiOpenJQIndex;
		ammoScript.AmmoType = AmmoHuoLiOpenJQType;
	}

	public static int[] FenShuBeiLv = {1, 1, 1, 1};
	public static void SetTimeFenShuBeiLv(PlayerEnum indexPlayer, int beiLv = 2)
	{
		int indexVal = (int)indexPlayer - 1;
		//Debug.Log("Unity:"+"FenShuBeiLv ************** "+FenShuBeiLv[indexVal]);
		if (FenShuBeiLv[indexVal] > 1) {
			FenShuBeiLv[indexVal] = FenShuBeiLv[indexVal] < 10 ? (FenShuBeiLv[indexVal]+1) : 10;
			return;
		}
		FenShuBeiLv[indexVal] = beiLv;
	}

	public static void ResetPlayerFenShuBeiLv(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		FenShuBeiLv[indexVal] = 1;
	}
	
	public static void SetPlayerQianHouFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsQianHouFire(true);
	}
	
	public static void SetPlayerChangChengFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsChangChengFire(true);
	}

	public static void SetPlayerJiQiangSanDanFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsJiQiangSanDanFire(true);
	}

	public static void SetPlayerQiangJiFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsQiangJiFire(true);
	}

	public static void SetPlayerIsPaiJiPaoFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsPaiJiPaoFire(true);
	}

	public static void SetPlayerIsSanDanZPFire(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		moveScript.SetIsSanDanZPFire(true);
	}

	public static void SetPlayerIsHuoLiAllOpen(PlayerEnum indexPlayer, bool isFire = true)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		if (moveScript == null) {
			return;
		}
		moveScript.SetIsHuoLiAllOpen(isFire);
	}

	public static bool GetPlayerIsHuoLiAllOpen(PlayerEnum indexPlayer)
	{
		XKPlayerMoveCtrl moveScript = XKPlayerMoveCtrl.GetXKPlayerMoveCtrl(indexPlayer);
		if (moveScript == null) {
			return false;
		}
		return moveScript.GetIsHuoLiAllOpen();
	}
}