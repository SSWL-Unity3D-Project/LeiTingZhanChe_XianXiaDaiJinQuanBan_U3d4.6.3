using UnityEngine;
using System.Collections.Generic;

public class XKNpcSpawnDaoJu : SSGameMono
{
    /// <summary>
    /// 是否产生随机道具.
    /// </summary>
    public bool IsCreatSuiJiDaoJu = false;
    public bool IsSpawnDJ = true;
    /// <summary>
    /// 掉落道具的预制列表.
    /// </summary>
	public GameObject[] DaoJuArray;
    /// <summary>
    /// 掉落道具的概率.
    /// </summary>
    [Range(0, 100)]
    public int[] DaoJuGaiLv;
    /// <summary>
    /// 大血包数据.
    /// </summary>
    [System.Serializable]
    public class BigXueBaoData
    {
        /// <summary>
        /// 血包预制.
        /// </summary>
        public GameObject XueBaoPrefab;
        /// <summary>
        /// 掉落概率.
        /// </summary>
        [Range(0, 100)]
        public int GaiLv = 0;
    }
    /// <summary>
    /// 大血包道具数据.
    /// 如果产生了大血包道具就不再产生其它道具.
    /// </summary>
    public BigXueBaoData m_BigXueBaoDt;

    /// <summary>
    /// 创建随机道具.
    /// </summary>
    public void CreatSuiJiDaoJu(PlayerEnum indexPlayer)
    {
        if (!IsCreatSuiJiDaoJu)
        {
            return;
        }

        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            return;
        }

        //if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PlayerCoinData[indexVal].XuBiVal <= 0)
        //{
        //    //不是续币玩家.
        //    return;
        //}

        if (!XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetIsChuCaiPiaoByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu))
        {
            //随机道具彩池的彩票积累的不够.
            return;
        }

        if (SSGameLogoData.m_GameDaiJinQuanMode == SSGameLogoData.GameDaiJinQuanMode.HDL_CaiPinQuan)
        {
            //海底捞菜品券版本游戏.
            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
            {
                if (SSHaiDiLaoBaoJiang.GetInstance().GetIsCanJiBaoNpc(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan) == false)
                {
                    //不允许发出随机道具.
                    return;
                }
            }
        }

        bool isUseOldMethod = false;
        if (isUseOldMethod == true)
        {
            if (Random.Range(0f, 100f) / 100f > 0.4f)
            {
                //没有随机上产生随机道具.
                return;
            }

            if (XkGameCtrl.GetInstance().GetIsCreateSuiJiDaoJu() == false)
            {
                //产生随机道具的间隔时间未到.
                return;
            }
        }
        else
        {
            if (XkGameCtrl.GetInstance().GetIsCreateSuiJiDaoJu(indexPlayer) == false)
            {
                //产生随机道具的间隔时间未到.
                return;
            }
        }
        //UnityLog("CreatSuiJiDaoJu....................");

        GameObject suiJiDaoJuPrefab = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.GetSuiJiDaoJuPrefab(indexPlayer);
        if (suiJiDaoJuPrefab != null)
        {
            GameObject obj = (GameObject)Instantiate(suiJiDaoJuPrefab, XkGameCtrl.GetInstance().DaoJuArray, transform);
            if (obj != null)
            {
                BuJiBaoCtrl buJiBao = obj.GetComponent<BuJiBaoCtrl>();
                if (buJiBao != null)
                {
                    buJiBao.IsCaiPiaoDaoJu = true;
                    buJiBao.SetIsSpawnDaoJu(indexPlayer);
                    buJiBao.DelayRemoveSelf(indexPlayer);
                }

                //SSCaiPiaoSuiJiDaoJu suiJiDaoJu = obj.GetComponent<SSCaiPiaoSuiJiDaoJu>();
                //if (suiJiDaoJu != null)
                //{
                //    suiJiDaoJu.CreatLiZi(indexPlayer);
                //}
                //else
                //{
                //    UnityLogWarning("CreatSuiJiDaoJu -> SSCaiPiaoSuiJiDaoJu was null..........");
                //}
            }
        }
        else
        {
            UnityLogWarning("CreatSuiJiDaoJu -> suiJiDaoJuPrefab was null!");
        }
    }

    /// <summary>
    /// 产生道具的时间记录信息.
    /// </summary>
    static float m_TimeLastCreatDaoJu = 0f;
	public void SpawnAllDaoJu(PlayerEnum playerIndex)
	{
        float timeLengQue = 60f;
        if (XkGameCtrl.GetInstance() != null)
        {
            timeLengQue = XkGameCtrl.GetInstance().m_NpcDiaoDaoJuTimeLengQue;
        }

        if (Time.time - m_TimeLastCreatDaoJu < timeLengQue)
        {
            //间隔时间小于x秒则不产生补给包.
            return;
        }
        m_TimeLastCreatDaoJu = Time.time;

        if (!IsSpawnDJ)
        {
			return;
		}

        bool isCreateBigXueBao = CreateBigXueBaoDaoJu(playerIndex);
        if (isCreateBigXueBao == true)
        {
            //如果产生了大血包道具就不再产生其它道具.
            return;
        }

        PointList = new List<Transform>();
		CheckDaoJuSpawnPointList();

		int randVal = 0;
		int max = DaoJuArray.Length;
		Transform trEndPoint = null;
		for(int i = 0; i < max; i++) {
			randVal = Random.Range(0, 10000) % 100;
			if (DaoJuGaiLv.Length > i && randVal >= DaoJuGaiLv[i]) {
				continue;
			}

            if (DaoJuArray[i] == null)
            {
                continue;
            }

            trEndPoint = null;
            BuJiBaoCtrl buJiBaoCom = DaoJuArray[i].GetComponent<BuJiBaoCtrl>();
            if (buJiBaoCom != null && buJiBaoCom.BuJiBao == BuJiBaoType.BigYiLiaoBaoDJ || buJiBaoCom.BuJiBao == BuJiBaoType.YiLiaoBaoDJ)
            {
                //医疗包道具.
                if (XKPlayerMoveCtrl.GetInstance(playerIndex) != null)
                {
                    trEndPoint = XKPlayerMoveCtrl.GetInstance(playerIndex).transform;
                }
            }

            if (trEndPoint == null)
            {
                trEndPoint = GetDaoJuSpawnPoint(i);
            }

			if (trEndPoint == null)
            {
				continue;
			}

            GameObject daoJuObj = (GameObject)Instantiate(DaoJuArray[i], transform.position, transform.rotation);
			BuJiBaoCtrl buJiScript = daoJuObj.GetComponent<BuJiBaoCtrl>();
			buJiScript.SetIsSpawnDaoJu(playerIndex);
            buJiScript.MoveDaoJuToPoint(trEndPoint);
		}
	}

    /// <summary>
    /// 创建大血包道具.
    /// </summary>
    bool CreateBigXueBaoDaoJu(PlayerEnum playerIndex)
    {
        bool isCreate = false;
        if (m_BigXueBaoDt != null && m_BigXueBaoDt.XueBaoPrefab != null)
        {
            int randVal = 0;
            randVal = Random.Range(0, 10000) % 100;
            if (randVal >= m_BigXueBaoDt.GaiLv)
            {
                //没有随机上.
            }
            else
            {
                Transform trEndPoint = null;
                //PointList = new List<Transform>();
                //CheckDaoJuSpawnPointList();
                //trEndPoint = GetDaoJuSpawnPoint(0);
                if (XKPlayerMoveCtrl.GetInstance(playerIndex) != null)
                {
                    trEndPoint = XKPlayerMoveCtrl.GetInstance(playerIndex).transform;
                }

                if (trEndPoint == null)
                {
                    //没有找到道具落点.
                }
                else
                {
                    //产生道具.
                    GameObject daoJuObj = (GameObject)Instantiate(m_BigXueBaoDt.XueBaoPrefab, transform.position, transform.rotation);
                    BuJiBaoCtrl buJiScript = daoJuObj.GetComponent<BuJiBaoCtrl>();
                    if (buJiScript != null)
                    {
                        isCreate = true;
                        buJiScript.SetIsSpawnDaoJu(playerIndex);
                        //血包道具自动吸附到玩家位置.
                        buJiScript.MoveDaoJuToPoint(trEndPoint);
                    }
                }
            }
        }
        return isCreate;
    }

	List<Transform> PointList;
	Transform GetDaoJuSpawnPoint(int indexVal)
	{
		int max = PointList.Count;
		if (max <= 0) {
			return null;
		}

		int indexValTmp = indexVal % max;
		return PointList[indexValTmp];
	}

	[Range(0.01f, 100f)]public float DisDaoJuVal = 15f;
	void CheckDaoJuSpawnPointList()
	{
		Transform[] trPointArray = XKPlayerCamera.GetInstanceFeiJi().GetDaoJuSpawnPoint();
		int max = trPointArray.Length;
		if (max <= 0) {
			return;
		}

		float disVal = DisDaoJuVal;
		Vector3 posA = transform.position;
		Vector3 posB = Vector3.zero;
		posA.y = 0f;
		for (int i = 0; i < max; i++) {
			posB = trPointArray[i].position;
			posB.y = 0f;
			if (Vector3.Distance(posA, posB) <= disVal && Vector3.Distance(posA, posB) > 1f) {
				PointList.Add(trPointArray[i]);
			}
		}
	}
}