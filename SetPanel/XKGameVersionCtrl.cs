using UnityEngine;

public class XKGameVersionCtrl : MonoBehaviour
{
    UILabel VersionLB;
    static string _GameVersion = "Version: 20190131.01";
    public static string GameVersion
    {
        get
        {
            string val = "";
/*#if UNITY_ANDROID
            val = _GameVersion + "_Apk";
#endif
#if UNITY_STANDALONE_WIN
            val = _GameVersion + "_Win";
#endif*/
            val = _GameVersion;
            return val;
        }
    }

	// Use this for initialization
	void Start()
	{
        VersionLB = GetComponent<UILabel>();
        if (VersionLB != null)
        {
            VersionLB.text = GameVersion;
        }
    }

    bool IsInit = false;
    float m_LastDrawTime = 0f;
    public void Init()
    {
        if (IsInit == true)
        {
            return;
        }
        IsInit = true;
        m_LastDrawTime = Time.time;
    }

    static bool IsSetGameVersion = false;
    /// <summary>
    /// 设置红点点正式服务器版本信息.
    /// </summary>
    public static void SetReleaseGameVersion()
    {
        if (IsSetGameVersion == false)
        {
            IsSetGameVersion = true;
            _GameVersion += "_Release"; //红点点正式服务器版本.
        }
    }

    void OnGUI()
    {
        if (IsInit == true)
        {
            if (Time.time - m_LastDrawTime < 15f)
            {
                Rect rect = new Rect(15f, 15f, 260f, 25f);
                GUI.Box(rect, "");
                GUI.color = Color.red;
                GUI.Label(rect, GameVersion);
            }
            else
            {
                IsInit = false;
                Destroy(this);
            }
        }
    }
}