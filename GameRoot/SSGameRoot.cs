using UnityEngine;

public class SSGameRoot : SSGameMono
{
    public enum JingRuiJiaMiState
    {
        /// <summary>
        /// 不进行精锐加密校验.
        /// </summary>
        NoJiaoYan = 0,
        /// <summary>
        /// 精锐4加密校验.
        /// </summary>
        JiaMiJiaoYan = 1,
    }
    /// <summary>
    /// 精锐加密校验状态.
    /// </summary>
    public JingRuiJiaMiState m_JingRuiJiaMiState = JingRuiJiaMiState.JiaMiJiaoYan;

    void Start()
    {
        bool isLoadingMovieScene = false;
        switch (m_JingRuiJiaMiState)
        {
            case JingRuiJiaMiState.NoJiaoYan:
                {
                    isLoadingMovieScene = true;
                    break;
                }
            case JingRuiJiaMiState.JiaMiJiaoYan:
                {
                    isLoadingMovieScene = true;
                    break;
                }
        }

        if (isLoadingMovieScene)
        {
            //加载循环动画关卡.
            Application.LoadLevel((int)GameLevel.Movie);
        }
    }
}