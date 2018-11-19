using UnityEngine;
using System.Collections.Generic;

public class XKPlayerGlobalDt : SSGameMono
{
    /// <summary>
    /// 子弹等级粒子特效数据.
    /// </summary>
    [System.Serializable]
    public class AmmoDengJiLiZiData
    {
        /// <summary>
        /// 发射主炮时的爆炸粒子预制.
        /// </summary>
        public GameObject[] LiZiPrefabArray = new GameObject[3];
        /// <summary>
        /// 创建粒子特效.
        /// </summary>
        internal void CreatLiZiTeXiao(int index, XKPlayerAutoFire autoFireCom, Transform tr)
        {
            //SSDebug.Log("CreatLiZiTeXiao -> index ================ " + index);
            if (index < 0 || index >= LiZiPrefabArray.Length)
            {
                return;
            }

            GameObject prefab = LiZiPrefabArray[index];
            if (prefab && autoFireCom != null)
            {
                autoFireCom.Instantiate(prefab, XkGameCtrl.MissionCleanup, tr);
            }
        }
    }
    /// <summary>
    /// 迫击炮子弹等级粒子数据.
    /// </summary>
    public AmmoDengJiLiZiData m_PaiJiPaoAmmoDengJiLiZiDt;
    /// <summary>
    /// 散弹主炮子弹等级粒子数据.
    /// </summary>
    public AmmoDengJiLiZiData m_SanDanZhuPaoAmmoDengJiLiZiDt;
    /// <summary>
    /// 激光炮子弹等级粒子数据.
    /// </summary>
    public AmmoDengJiLiZiData m_JiGuanPaoAmmoDengJiLiZiDt;

    /**
	 * 机枪普通子弹.
	 */
    public GameObject PuTongJQAmmo;
	const int AmmoPuTongJQIndex = 0;
	const PlayerAmmoType AmmoPuTongJQType = PlayerAmmoType.PuTongAmmo;
	/**
	 * 主炮普通导弹.
	 */
	public GameObject DaoDanZPAmmo;
	const int AmmoDaoDanZPIndex = 0;
	const PlayerAmmoType AmmoDaoDanZPType = PlayerAmmoType.DaoDanAmmo;
	/**
	 * 普通子弹开火粒子.
	 */
	public GameObject[] AmmoParticle;
	public GameObject[] SanDanAmmoParticle;
	public GameObject[] PaiJiPaoAmmoParticle;
	public GameObject[] ChuanTouDanAmmoParticle;
	public GameObject[] DaoDanAmmoParticle;
	/**
	 * 主角死亡爆炸预置体.
	 */
	public GameObject DeathExplodPrefab;
	/**
	 * 普通子弹发射频率.
	 * Frequency[0] -> 机枪口.
	 * Frequency[1] -> 主炮口.
	 */
	[Range(0.01f, 500f)]public float[] Frequency = {8f, 0.75f};
	/**
	 * 导弹对npc施加的力.
	 */
	[Range(0f, 10000f)]public float DaoDanPowerNpc = 500f;
	[Range(0f, 100f)]public float MaxMvSpeed = 50f;
	/**
	 * 摇杆版主角移动速度.
	 */
	[Range(0f, 500f)]public float MaxMvSpeedYG = 50f;
	/**
	 * 摇杆版主角横向移动速度倍率.
	 */
	[Range(0f, 500f)]public float HorizontalSpeedYGBL = 0.5f;

	[Range(0f, 500f)]public float DirSpeed = 170f;
	/**
	 * 摇杆版转向速度.
	 */
	[Range(0f, 5000f)]public float DirSpeedYG = 170f;
	[Range(0f, 100f)]public float MaxHitPower = 50f;
	[Range(0f, 90f)]public float MaxFangXiangAngle = 90f;
	/**
	 * 普通机枪发射后坐力.
	 */
	[Range(0f, 100f)]public float JiQiangFirePTPower = 50f;
	/**
	 * 前后发射后坐力.
	 */
	[Range(0f, 100f)]public float QianHouFirePTPower = 50f;
	/**
	 * 长程机枪后坐力.
	 */
	[Range(0f, 100f)]public float ChangChengFirePTPower = 50f;
	/**
	 * 散弹机枪后坐力.
	 */
	[Range(0f, 100f)]public float SanDanFirePTPower = 50f;
	/**
	 * 强击机枪后坐力.
	 */
	[Range(0f, 100f)]public float QiangJiFirePTPower = 50f;
	/**
	 * 主炮后坐力.
	 */
	[Range(0f, 100f)]public float ZhuPaoFireZPPower = 50f;
	/**
	 * 穿甲弹后坐力.
	 */
	[Range(0f, 100f)]public float ChuanJiaDanZPPower = 50f;
	/**
	 * 炮击炮后坐力.
	 */
	[Range(0f, 100f)]public float PaiJiPaoZPPower = 50f;
	/**
	 * 散弹后坐力.
	 */
	[Range(0f, 100f)]public float SanDanZPPower = 50f;
	/**
	 * 火力全开主炮后坐力.
	 */
	[Range(0f, 100f)]public float HuoLiQuanKaiZPPower = 50f;
	/**
	 * 火力全开机枪后坐力.
	 */
	[Range(0f, 100f)]public float HuoLiQuanKaiPTPower = 50f;
	/**
	 * 主角出了镜头范围后减去的血值.
	 */
	[Range(0f, 10000f)]public float DamageFanWeiOut = 100f;
	/**
	 * 主角跳板伤害检测距离.
	 */
	[Range(0f, 100f)]public float DamageDisTiaoBan = 10f;
	/**
	 * 抛物线计算高度的斜率.
	 */
	[Range(0f, 1000f)]public float GDKeyTiaoBanPaoWuXian = 0.5f;
	[Range(0.01f, 10f)]public float FlyTimeTiaoBanPaoWuXian = 0.5f;
	/**
	 * 激活跳板时的爆炸特效.
	 */
	public GameObject TiaoBanExpObjStart;
	/**
	 * 跳板过程结束的爆炸特效.
	 */
	public GameObject TiaoBanExpObjOver;
	/**
	 * 主角迫击炮计算高度的斜率.
	 */
	[Range(0.01f, 100f)]public float KeyPaiJiPaoValPlayer = 0.1f;
	public static List<XKPlayerMoveCtrl> PlayerMoveList;
	static XKPlayerGlobalDt _Instance;
	public static XKPlayerGlobalDt GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		
		CheckPlayerAmmoFrequency();
		for (int i = 0; i < 2; i++) {
			if (AmmoParticle[i] == null) {
				Debug.LogWarning("Unity:"+"AmmoParticle["+i+"] is null");
				AmmoParticle[i].name = "null";
				return;
			}
		}
		
		for (int i = 0; i < 2; i++) {
			if (SanDanAmmoParticle[i] == null) {
				Debug.LogWarning("Unity:"+"SanDanAmmoParticle["+i+"] is null");
				SanDanAmmoParticle[i].name = "null";
				return;
			}
		}

		for (int i = 0; i < 2; i++) {
			if (PaiJiPaoAmmoParticle[i] == null) {
				Debug.LogWarning("Unity:"+"PaiJiPaoAmmoParticle["+i+"] is null");
				PaiJiPaoAmmoParticle[i].name = "null";
				return;
			}
		}

		for (int i = 0; i < 2; i++) {
			if (ChuanTouDanAmmoParticle[i] == null) {
				Debug.LogWarning("Unity:"+"ChuanTouDanAmmoParticle["+i+"] is null");
				ChuanTouDanAmmoParticle[i].name = "null";
				return;
			}
		}

		for (int i = 0; i < 2; i++) {
			if (DaoDanAmmoParticle[i] == null) {
				Debug.LogWarning("Unity:"+"DaoDanAmmoParticle["+i+"] is null");
				DaoDanAmmoParticle[i].name = "null";
				return;
			}
		}

		if (DeathExplodPrefab == null) {
			Debug.LogWarning("Unity:"+"DeathExplodPrefab is null");
			DeathExplodPrefab.name = "null";
			return;
		}

		if (PlayerMoveList != null){
			PlayerMoveList.Clear();
		}
		PlayerAmmoCtrl ammoScript = PuTongJQAmmo.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoPuTongJQIndex;
		ammoScript.AmmoType = AmmoPuTongJQType;

		ammoScript = DaoDanZPAmmo.GetComponent<PlayerAmmoCtrl>();
		ammoScript.AmmoIndex = AmmoDaoDanZPIndex;
		ammoScript.AmmoType = AmmoDaoDanZPType;
	}

	public static void AddPlayerMoveList(XKPlayerMoveCtrl playerScript)
	{
		if (PlayerMoveList == null){
			PlayerMoveList = new List<XKPlayerMoveCtrl>();
		}

		if (PlayerMoveList.Contains(playerScript)) {
			return;
		}
		PlayerMoveList.Add(playerScript);
	}

	void CheckPlayerAmmoFrequency()
	{
		int max = Frequency.Length;
		for (int i = 0; i < max; i++) {
			Frequency[i] = Frequency[i] <= 0f ? 1f : Frequency[i];
		}
		
		/*max = FrequencyGaoBao.Length;
		for (int i = 0; i < max; i++) {
			FrequencyGaoBao[i] = FrequencyGaoBao[i] <= 0f ? 1f : FrequencyGaoBao[i];
		}*/
		
		/*max = FrequencySanDan.Length;
		for (int i = 0; i < max; i++) {
			FrequencySanDan[i] = FrequencySanDan[i] <= 0f ? 1f : FrequencySanDan[i];
		}*/
		
		/*max = FrequencyGenZongDan.Length;
		for (int i = 0; i < max; i++) {
			FrequencyGenZongDan[i] = FrequencyGenZongDan[i] <= 0f ? 1f : FrequencyGenZongDan[i];
		}*/
		
		/*max = FrequencyGaoBao.Length;
		for (int i = 0; i < max; i++) {
			FrequencyChuanTouDan[i] = FrequencyChuanTouDan[i] <= 0f ? 1f : FrequencyChuanTouDan[i];
		}*/
	}
}