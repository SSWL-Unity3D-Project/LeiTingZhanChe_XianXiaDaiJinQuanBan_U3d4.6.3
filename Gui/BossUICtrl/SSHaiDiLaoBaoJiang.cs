using UnityEngine;

/// <summary>
/// 海底捞爆奖率控制.
/// </summary>
public class SSHaiDiLaoBaoJiang : MonoBehaviour
{
	[System.Serializable]
	public class BaoJiangData
	{
		/// <summary>
		/// 爆奖率.
		/// </summary>
		[Range(0, 100)]
		public int m_BaoJiangLv = 50;
		/// <summary>
		/// 几个人里出一次奖.
		/// </summary>
		int maxPlayer = 1;
		/// <summary>
		/// 当前已经有几个人进行游戏(对同一微信玩家不进行去重).
		/// </summary>
		int currentNum = 0;
		/// <summary>
		/// 是否已经击爆了Npc.
		/// </summary>
		bool IsHaveJiBaoNpc = false;
		internal void SetIsHaveJiBaoNpc(bool isJiBaoNpc)
		{
			IsHaveJiBaoNpc = isJiBaoNpc;
		}

		/// <summary>
		/// 更新游戏在几个人里出一次奖.
		/// </summary>
		internal void UpdateMaxPlayer(int baoJiangLv)
		{
			int addVal = 0;
			if (100 % baoJiangLv != 0)
			{
				addVal = 1;
			}
			maxPlayer = (100 / baoJiangLv) + addVal;
            //SSDebug.LogWarning("UpdateMaxPlayer -> maxPlayer ==== " + maxPlayer + ", baoJiangLv ==== " + baoJiangLv);
		}

		internal void AddCurrentNum()
		{
			currentNum++;
			if (IsHaveJiBaoNpc == true)
			{
				if (currentNum > maxPlayer)
				{
					ResetCurrentNum();
				}
			}
		}

		void ResetCurrentNum()
		{
			IsHaveJiBaoNpc = false;
			currentNum = 0;
		}

		/// <summary>
		/// 获取是否可以击爆Npc.
		/// </summary>
		internal bool GetIsCanJiBaoNpc()
        {
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                //没有玩家激活游戏时,Ai都是可以击爆JPBoss和战车的.
                return true;
            }

            if (IsHaveJiBaoNpc == true)
			{
				//已经有玩家击爆过npc,其余玩家不能在击爆npc了.
				return false;
			}

			if (currentNum >= maxPlayer)
			{
				//当前玩家人数已经积累的足够多了,新来的玩家都可以击爆npc.
				return true;
			}

			bool isCanJiBao = true;
			int randNum = (Random.Range(0, 1000) % 100) + 1;
			if (randNum > m_BaoJiangLv)
			{
				isCanJiBao = false;
			}
			return isCanJiBao;
		}
	}
	/// <summary>
	/// JPBoss爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtJPBoss = new BaoJiangData();
	/// <summary>
	/// 战车01爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtZhanChe01 = new BaoJiangData();
	/// <summary>
	/// 战车02爆奖数据控制.
	/// </summary>
	public BaoJiangData m_BaoJiangDtZhanChe02 = new BaoJiangData();
    static SSHaiDiLaoBaoJiang _Instance = null;
    public static SSHaiDiLaoBaoJiang GetInstance()
    {
        return _Instance;
    }

    private void Awake()
    {
        _Instance = this;
    }

    /// <summary>
    /// 更新爆奖率数据信息.
    /// </summary>
	internal void UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type, int baoJiangLv)
	{
		BaoJiangData baoJiangDt = null;
		switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
        }

        if (baoJiangDt != null)
        {
            baoJiangDt.UpdateMaxPlayer(baoJiangLv);
        }
	}

    /// <summary>
    /// 设置是否已经击爆Npc.
    /// </summary>
    internal void SetIsHaveJiBaoNpc(bool isHaveJiBao, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有玩家激活游戏时不允许设置该属性.
            return;
        }

        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
        }

        if (baoJiangDt != null)
        {
            baoJiangDt.SetIsHaveJiBaoNpc(isHaveJiBao);
        }
    }

    internal bool GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        BaoJiangData baoJiangDt = null;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    baoJiangDt = m_BaoJiangDtJPBoss;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe01;
                }
                break;
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    baoJiangDt = m_BaoJiangDtZhanChe02;
                }
                break;
        }

        bool isCanJiBao = false;
        if (baoJiangDt != null)
        {
            isCanJiBao = baoJiangDt.GetIsCanJiBaoNpc();
        }
        return isCanJiBao;
    }

    internal void AddPlayerNum()
    {
        if (m_BaoJiangDtJPBoss != null)
        {
            m_BaoJiangDtJPBoss.AddCurrentNum();
        }

        if (m_BaoJiangDtZhanChe01 != null)
        {
            m_BaoJiangDtZhanChe01.AddCurrentNum();
        }

        if (m_BaoJiangDtZhanChe02 != null)
        {
            m_BaoJiangDtZhanChe02.AddCurrentNum();
        }
    }
}
