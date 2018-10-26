using Assets.XKGame.Script.GamePay;

public class SSGamePayUI : SSGameMono
{
    public UITextList m_TextList;
    /// <summary>
    /// 总收入.
    /// </summary>
    public UILabel m_TotalRevenue;
    /// <summary>
    /// 总支出.
    /// </summary>
    public UILabel m_TotalRebate;
    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init()
    {
        UnityEngine.Screen.showCursor = true;
        SSGameWXPayDataManage payDataManage = new SSGameWXPayDataManage();
        SSGameWXPayData[] payDataArray = payDataManage.GetGamePayDataInfo();

        if (m_TextList != null)
        {
            int revenues = 0;
            int rebates = 0;
            string head = "";
            string info = "";
            for (int i = 0; i < payDataArray.Length; ++i)
            {
                head = (i % 2 == 0) ? "[FFFFFF]" : "[000000]";
                info = head + payDataArray[i].Time
                    + GetSpaceInfo(payDataArray[i].Revenue) + payDataArray[i].Revenue
                    + GetSpaceInfo(payDataArray[i].Rebate) + payDataArray[i].Rebate + "[-]";
                m_TextList.Add(info);

                revenues += System.Convert.ToInt32(payDataArray[i].Revenue);
                rebates += System.Convert.ToInt32(payDataArray[i].Rebate);
            }

            m_TotalRevenue.text = revenues + "元";
            m_TotalRebate.text = rebates + "元";
        }
    }

    string GetSpaceInfo(string value)
    {
        string space = "";
        int length = 20 - (value.Length * 2);
        for (int i = 0; i < length; i++)
        {
            space += " ";
        }
        return space;
    }

    bool IsRemoveSelf = false;
    public void OnCloseBt()
    {
        RemoveSelf();
    }

    internal void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            UnityEngine.Screen.showCursor = false;
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}
