using UnityEngine;
using System.Collections;

public class RestartGameCom : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        /*if (SSJingRuiJiaMi.IsOpenJingRuiJiaMi == true)
        {
            PlayerPrefs.SetInt("JiaoYanJingRui", 1);
        }*/
        StartCoroutine(DelayLoadingMovieScene());
    }


    IEnumerator DelayLoadingMovieScene()
    {
        yield return new WaitForSeconds(0.5f);
        RestartGame();
    }

    /// <summary>
    /// 重新启动游戏.
    /// </summary>
    void RestartGame()
    {
        Application.Quit();
        RunCmd("start OpenApp.exe");
    }

    void RunCmd(string command)
    {
        //實例一個Process類，啟動一個獨立進程    
        System.Diagnostics.Process p = new System.Diagnostics.Process();    //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，    
                                                                            //包括了一些屬性和方法，下面我們用到了他的幾個屬性：   
        p.StartInfo.FileName = "cmd.exe";           //設定程序名   
        p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數   
        p.StartInfo.UseShellExecute = false;        //關閉Shell的使用    p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入    p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出   
        p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出    
        p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口    
        p.Start();   //啟動

        //p.WaitForInputIdle();
        //MoveWindow(p.MainWindowHandle, 1000, 10, 300, 200, true);

        //p.StandardInput.WriteLine(command); //也可以用這種方式輸入要執行的命令    
        //p.StandardInput.WriteLine("exit");        //不過要記得加上Exit要不然下一行程式執行的時候會當機    return p.StandardOutput.ReadToEnd();        //從輸出流取得命令執行結果
    }
}
