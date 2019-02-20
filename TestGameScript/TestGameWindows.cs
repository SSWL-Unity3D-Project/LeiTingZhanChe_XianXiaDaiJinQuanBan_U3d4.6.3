using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestGameWindows : MonoBehaviour
{   
	// Update is called once per frame
	void Update ()
    {
        UpdataGameWindows();
    }

    float m_TimeLast = 0f;
    void UpdataGameWindows()
    {
        if (Time.time - m_TimeLast >= 3f)
        {
            m_TimeLast = Time.time;
            SetGameWindows();
        }
    }

    [DllImport("user32")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    const int SWP_NOSIZE = 0x0001;
    const int SWP_NOMOVE = 0x0002;
    const int HWND_NOTOPMOST = 0xffffffe;
    const int HWND_TOPMOST = -1;
    [DllImport("kernel32.dll")]
    public static extern void SetLastError(uint dwErrCode);
    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);
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
        IntPtr ms_hMainDlg = GetProcessWnd();
        SetWindowPos(ms_hMainDlg, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        SetWindowPos(ms_hMainDlg, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        //SSDebug.LogWarning("SetGameWindows...");
    }
}
