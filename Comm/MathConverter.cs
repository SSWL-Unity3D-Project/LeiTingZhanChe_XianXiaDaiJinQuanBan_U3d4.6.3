using System;

namespace Assets.XKGame.Script.Comm
{
    /// <summary>
    /// 数学转换类
    /// </summary>
    public class MathConverter
    {
        /// <summary>
        /// object 转换 float(转换失败，则尝试将前部分数字转换为float)
        /// </summary>
        /// <param name="obj2Float"></param>
        /// <returns>默认：0.00</returns>
        public static float StringToFloat(string str2Float)
        {
            float result = 0.00f;   //默认值
            if (!float.TryParse(str2Float, out result))     //string直接转换为float,若失败，则获取字符串前部分数字转换为float
            {
                string strNumber = string.Empty;
                foreach (char iChr in str2Float)
                {
                    if (Char.IsNumber(iChr))
                    {
                        strNumber += iChr;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(strNumber))
                {
                    float.TryParse(strNumber, out result);
                }
            }
            return result;
        }
    }
}
