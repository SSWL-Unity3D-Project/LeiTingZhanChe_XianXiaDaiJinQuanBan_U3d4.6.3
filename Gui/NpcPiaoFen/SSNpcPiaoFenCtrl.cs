using UnityEngine;
using System.Collections.Generic;

public class SSNpcPiaoFenCtrl : MonoBehaviour
{
    /// <summary>
    /// 玩家1加号.
    /// </summary>
    public Texture m_JiaHaoP1;
    public Texture m_JiaHaoP2;
    public Texture m_JiaHaoP3;
    /// <summary>
    /// 玩家1分数图集.
    /// </summary>
    public UIAtlas m_FenShuAtlasP1;
    public UIAtlas m_FenShuAtlasP2;
    public UIAtlas m_FenShuAtlasP3;
    List<SSNpcPiaoFenMove> FenShuList;
    int MaxPlayerFS = 36;
    
    // Use this for initialization
    public void Init()
    {
        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/NpcPiaoFen/NpcPiaoFen");
        if (gmDataPrefab == null)
        {
            Debug.LogWarning("Unity:Init -> gmDataPrefab was null..............");
            return;
        }

        FenShuList = new List<SSNpcPiaoFenMove>();
        GameObject obj = null;
        for (int i = 0; i < MaxPlayerFS; i++)
        {
            obj = (GameObject)Instantiate(gmDataPrefab);
            obj.transform.parent = transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            FenShuList.Add(obj.GetComponent<SSNpcPiaoFenMove>());
            obj.SetActive(false);
        }
    }

    //	public Transform TestPlayerTr;
    //	public Transform TestFenShuTr;
    //	[Range(1, 999999)]public int FenShuTest = 1;
    //	public Vector3 TestPos;
    //	void OnGUI()
    //	{
    //		GUI.Box(new Rect(0f, 0f, 500f, 25f), TestPos.ToString());
    //	}
    // Update is called once per frame
    //	void Update()
    //	{
    //		if (Camera.main == null) {
    //			return;
    //		}
    //		TestPlayerTr = XkPlayerCtrl.GetInstanceFeiJi().TestCubeTr;
    //		Vector3 startPos = Camera.main.WorldToScreenPoint(TestPlayerTr.position);
    //		startPos.z = 0f;
    //		startPos.x = startPos.x < 0f ? 0f : startPos.x;
    //		startPos.x = startPos.x > Screen.width ? Screen.width : startPos.x;
    //		startPos.y = startPos.y < 0f ? 0f : startPos.y;
    //		startPos.y = startPos.y > Screen.height ? Screen.height : startPos.y;
    //
    //		startPos.x *= (XkGameCtrl.ScreenWidth / Screen.width);
    //		startPos.y *= (XkGameCtrl.ScreenHeight / Screen.height);
    //		
    //		startPos.x = startPos.x < 0f ? 0f : startPos.x;
    //		startPos.x = startPos.x > XkGameCtrl.ScreenWidth ? XkGameCtrl.ScreenWidth : startPos.x;
    //		startPos.y = startPos.y < 0f ? 0f : startPos.y;
    //		startPos.y = startPos.y > XkGameCtrl.ScreenHeight ? XkGameCtrl.ScreenHeight : startPos.y;
    //
    //		int fenShuLen = FenShuTest.ToString().Length;
    //		startPos.x += 9f * (fenShuLen - 1);
    //		TestPos = startPos;
    //		TestFenShuTr.localPosition = startPos;

    //		if (Input.GetKeyDown(KeyCode.P)) {
    //			//ShowPlayerFenShu(PlayerEnum.Null, FenShuTest);
    //			ShowPlayerFenShu(PlayerEnum.PlayerOne, Random.Range(1, 999999));
    //		}
    //	}

    SSNpcPiaoFenMove GetFenShuMoveCom()
    {
        GameObject obj = null;
        int valTmp = 0;
        for (int i = 0; i < MaxPlayerFS; i++)
        {
            obj = FenShuList[i].gameObject;
            if (obj.activeSelf)
            {
                continue;
            }
            valTmp = i;
            break;
        }
        return FenShuList[valTmp];
    }

    public void ShowFenShuInfo(PlayerEnum indexPlayer, int fenShuVal, Vector3 piaoFenPos)
    {
        //int indexPlayer = (int)indexPlayer - 1;
        //indexPlayer = (indexPlayer < 0 || indexPlayer > 3) ? 0 : indexPlayer;
        //int fenShuValTmp = (fenShuVal * XKDaoJuGlobalDt.FenShuBeiLv[indexPlayer]);
        //if (fenShuValTmp <= 0)
        //{
        //    return;
        //}

        //XkGameCtrl.PlayerJiFenArray[indexPlayer] += fenShuValTmp;
        //XKPlayerScoreCtrl.ChangePlayerScore(indexVal);

        SSNpcPiaoFenMove fenShuMoveCom = GetFenShuMoveCom();
        if (fenShuMoveCom == null)
        {
            return;
        }

        Texture jiaHaoImg = null;
        UIAtlas atlas = null;
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    atlas = m_FenShuAtlasP1;
                    jiaHaoImg = m_JiaHaoP1;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    atlas = m_FenShuAtlasP2;
                    jiaHaoImg = m_JiaHaoP2;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    atlas = m_FenShuAtlasP3;
                    jiaHaoImg = m_JiaHaoP3;
                    break;
                }
            default:
                {
                    return;
                }
        }

        Vector3 startPos = XkGameCtrl.GetInstance().GetWorldObjToScreenPos(piaoFenPos);
        //int fenShuLen = fenShuVal.ToString().Length;
        //startPos.x += 9f * (fenShuLen - 1);
        fenShuMoveCom.ShowFenShuVal(fenShuVal, startPos, atlas, jiaHaoImg);
    }
}