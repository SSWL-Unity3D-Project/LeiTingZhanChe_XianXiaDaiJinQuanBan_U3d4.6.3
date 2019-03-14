using UnityEngine;

[RequireComponent(typeof(SSGameNumUI3D))]
public class SSChouJiangFenShu3D : MonoBehaviour
{
    SSGameNumUI3D m_SSGameNumUI3D;
    private void Awake()
    {
        m_SSGameNumUI3D = GetComponent<SSGameNumUI3D>();
    }

    void Start()
    {
        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().AddChouJiangFenShu3D(this);
        }
    }

    /// <summary>
    /// 展示数字.
    /// </summary>
    internal void ShowNum(int num)
    {
        if (m_SSGameNumUI3D != null)
        {
            m_SSGameNumUI3D.ShowNumUI(num);
        }
    }
}
