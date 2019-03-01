using UnityEngine;
using System.Collections.Generic;

public class XKNpcSpawnDaoJu : SSGameMono
{
    /// <summary>
    /// 是否产生随机道具.
    /// </summary>
    public bool IsCreatSuiJiDaoJu = false;
    public bool IsSpawnDJ = true;
	public GameObject[] DaoJuArray;
	public int[] DaoJuGaiLv;

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
                    buJiBao.SetIsSpawnDaoJu();
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
	public void SpawnAllDaoJu()
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

        if (!IsSpawnDJ) {
			return;
		}
		PointList = new List<Transform>();
		CheckDaoJuSpawnPointList();

		int randVal = 0;
		int max = DaoJuArray.Length;
		Transform trEndPoint = null;
		for(int i = 0; i < max; i++) {
			randVal = Random.Range(0, 10000) % 100;
			if (randVal >= DaoJuGaiLv[i]) {
				continue;
			}

			trEndPoint = GetDaoJuSpawnPoint(i);
			if (trEndPoint == null) {
				continue;
			}
			GameObject daoJuObj = (GameObject)Instantiate(DaoJuArray[i], transform.position, transform.rotation);
			BuJiBaoCtrl buJiScript = daoJuObj.GetComponent<BuJiBaoCtrl>();
			buJiScript.SetIsSpawnDaoJu();
			buJiScript.MoveDaoJuToPoint(trEndPoint);
		}
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