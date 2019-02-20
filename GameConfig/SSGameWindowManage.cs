using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SSGameWindowManage : MonoBehaviour
{
#if UNITY_EDITOR
    // Update is called once per frame
    //void Update ()
    //{
    //    UpdataGameWindows();
    //}
#else
    // Update is called once per frame
    void Update()
    {
        UpdataGameWindows();
    }
#endif

    float m_TimeLast = 0f;
    void UpdataGameWindows()
    {
        if (Time.time - m_TimeLast >= 3f)
        {
            m_TimeLast = Time.time;
            SetGameWindows();
        }
    }

    /// <summary>
    /// 激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。
    /// 在恢复最小化窗口时，应用程序应该指定这个标志。nCmdShow=9
    /// </summary>
    const int SW_RESTORE = 9;
    [DllImport("kernel32.dll")]
    public static extern void SetLastError(uint dwErrCode);
    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);
    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    [DllImport("user32.dll")]
    static extern bool IsIconic(IntPtr hWnd);
    /// <summary>
    /// 获取游戏窗体句柄.
    /// </summary>
    IntPtr GetProcessWnd()
    {
        IntPtr ptrWnd = IntPtr.Zero;
        uint pid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;  // 当前进程 ID  
        bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
        {
            uint id = 0;
            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref id);
                if (id == lParam)    // 找到进程对应的主窗口句柄  
                {
                    ptrWnd = hwnd;   // 把句柄缓存起来  
                    SetLastError(0); // 设置无错误  
                    return false;    // 返回 false 以终止枚举窗口  
                }
            }
            return true;
        }), pid);
        return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
    }

    void SetGameWindows()
    {
        IntPtr ptr = GetProcessWnd();
        bool isMinWindows = IsIconic(ptr);
        if (isMinWindows == true)
        {
            //游戏窗口被设置为最小化了.
            //SSDebug.LogWarning("SetGameWindows -> isMinWindows ====== " + isMinWindows);
            bool isShow = ShowWindow(ptr, SW_RESTORE);//恢复窗口.
            SSDebug.LogWarning("SetGameWindows -> isShow ====== " + isShow);
            StartCoroutine(DelayShowFullScreenGame());
        }
    }

    /// <summary>
    /// 延迟使游戏为全屏展示模式
    /// </summary>
    IEnumerator DelayShowFullScreenGame()
    {
        yield return new WaitForSeconds(1f);
        //游戏为全屏展示模式
        XkGameCtrl.MakeGameToFullScreen();
    }
}

