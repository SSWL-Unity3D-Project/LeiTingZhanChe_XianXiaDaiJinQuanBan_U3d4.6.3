namespace Assets.XKGame.Script.Server.WebSocket
{
    /// <summary>
    /// 服务器对游戏进行的配置数据信息.
    /// </summary>
    public class SSServerConfigData
    {
        /// <summary>
        /// 游戏机台弹幕信息.
        /// </summary>
        internal string GameDanMuInfo = "代金券送不停";

        /// <summary>
        /// 随机道具出票率.
        /// 1元代金券.
        /// </summary>
        internal float SuiJiDaoJuChuPiaoLv = 0.0f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券01低面额.
        /// </summary>
        internal float ZhanCheChuPiaoLv_01 = 0.5f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券02高面额.
        /// </summary>
        internal float ZhanCheChuPiaoLv_02 = 0.3f;
        /// <summary>
        /// JPBoss出票率.
        /// 100元代金券.
        /// </summary>
        internal float JPBossChuPiaoLv = 0.19f;

        /// <summary>
        /// 随机道具出票条件(游戏启动币数乘以该值).
        /// 随机道具代金券.
        /// </summary>
        float SuiJiDaoJuDaiJinQuan = 10f;
        /// <summary>
        /// 战车代金券01出票条件(游戏启动币数乘以该值).
        /// </summary>
        float ZhanCheDaiJinQuan_01 = 5f;
        /// <summary>
        /// 战车代金券02出票条件(游戏启动币数乘以该值).
        /// </summary>
        float ZhanCheDaiJinQuan_02 = 20f;
        /// <summary>
        /// JPBoss出票条件(游戏启动币数乘以该值).
        /// JPBoss代金券200元.
        /// </summary>
        internal float JPBossDaiJinQuan = 200f;

        /// <summary>
        /// 随机道具得彩累积数量.
        /// </summary>
        internal float SuiJiDaoJuDeCai = 0f;
        /// <summary>
        /// 战车得彩累积数量.
        /// 20元代金券累计数量.
        /// </summary>
        internal float ZhanCheDeCai_01 = 0f;
        /// <summary>
        /// 战车得彩累积数量.
        /// 50元代金券累计数量.
        /// </summary>
        internal float ZhanCheDeCai_02 = 0f;
        /// <summary>
        /// JPBoss得彩累积数量.
        /// </summary>
        internal float JPBossDeCai = 0f;

        /// <summary>
        /// 更新所有服务器配置数据.
        /// </summary>
        internal void UpdataAllServerConfigData()
        {
            SSDebug.Log("UpdataAllServerConfigData...");
            if (XKGlobalData.GetInstance() != null)
            {
                //更新游戏弹幕信息.
                XKGlobalData.GetInstance().UpdateDanMuInfo(GameDanMuInfo);
            }

            if (XkPlayerCtrl.GetInstanceFeiJi() != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
            {
                //更新代金券奖池相关数据信息.
                SSCaiPiaoDataManage.GameCaiPiaoData gmCaiPiaoData = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData;
                gmCaiPiaoData.UpdateChuPiaoLvInfo(SuiJiDaoJuChuPiaoLv, ZhanCheChuPiaoLv_01, ZhanCheChuPiaoLv_02, JPBossChuPiaoLv);
                gmCaiPiaoData.UpdateDaiJinQuanInfo(SuiJiDaoJuDaiJinQuan, ZhanCheDaiJinQuan_01, ZhanCheDaiJinQuan_02, JPBossDaiJinQuan);
                gmCaiPiaoData.UpdateDaiJinQuanCaiChiInfo(SuiJiDaoJuDeCai, ZhanCheDeCai_01, ZhanCheDeCai_01, JPBossDeCai);
            }
        }
    }
}
