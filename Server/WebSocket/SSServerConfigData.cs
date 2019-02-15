namespace Assets.XKGame.Script.Server.WebSocket
{
    /// <summary>
    /// 服务器对游戏进行的配置数据信息.
    /// </summary>
    public class SSServerConfigData
    {
        /// <summary>
        /// 游戏一币等于多少人民币的信息.
        /// 单位是分.
        /// </summary>
        internal int GameCoinToMoney = 200;
        /// <summary>
        /// 免费试玩次数信息.
        /// </summary>
        internal int MianFeiShiWanCount = 0;
        /// <summary>
        /// 游戏机台弹幕信息.
        /// </summary>
        internal string GameDanMuInfo = "代金券送不停";

        /// <summary>
        /// 游戏彩池返奖率数据信息.
        /// </summary>
        internal float CaiChiFanJiangLv = 0.5f;

        /// <summary>
        /// 随机道具出票率.
        /// 1元代金券.
        /// </summary>
        internal float SuiJiDaoJuChuPiaoLv = 0f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券01低面额.
        /// </summary>
        internal float ZhanCheChuPiaoLv_01 = 0f;
        /// <summary>
        /// 战车得彩出票率.
        /// 战车代金券02高面额.
        /// </summary>
        internal float ZhanCheChuPiaoLv_02 = 0f;
        /// <summary>
        /// JPBoss出票率.
        /// 100元代金券.
        /// </summary>
        internal float JPBossChuPiaoLv = 1f;
        
        /// <summary>
        /// 随机道具爆奖率.
        /// 1元代金券.
        /// </summary>
        internal float SuiJiDaoJuBaoJiangLv = 0f;
        /// <summary>
        /// 战车得彩爆奖率.
        /// 战车代金券01低面额.
        /// </summary>
        internal float ZhanCheBaoJiangLv_01 = 0f;
        /// <summary>
        /// 战车得彩爆奖率.
        /// 战车代金券02高面额.
        /// </summary>
        internal float ZhanCheBaoJiangLv_02 = 0f;
        /// <summary>
        /// JPBoss爆奖率.
        /// 100元代金券.
        /// </summary>
        internal float JPBossBaoJiangLv = 0.3f;

        /// <summary>
        /// 随机道具出票条件(游戏启动币数乘以该值).
        /// 随机道具代金券.
        /// </summary>
        internal float SuiJiDaoJuDaiJinQuan = 10f;
        /// <summary>
        /// 战车代金券01出票条件(游戏启动币数乘以该值).
        /// </summary>
        internal float ZhanCheDaiJinQuan_01 = 5f;
        /// <summary>
        /// 战车代金券02出票条件(游戏启动币数乘以该值).
        /// </summary>
        internal float ZhanCheDaiJinQuan_02 = 20f;
        /// <summary>
        /// JPBoss出票条件(游戏启动币数乘以该值).
        /// JPBoss代金券200元.
        /// </summary>
        internal float JPBossDaiJinQuan = 200f;
        /// <summary>
        /// JPBoss代金券商户支付金额.
        /// </summary>
        internal float JPBossDaiJinQuanShangHuZhiFu = 150f;

        /// <summary>
        /// 随机道具代金券奖池是否无穷大.
        /// </summary>
        internal bool IsWuQiongDaJiangChiSuiJiDaoJuDaiJinQuan = true;
        /// <summary>
        /// 战车代金券01奖池是否无穷大.
        /// </summary>
        internal bool IsWuQiongDaJiangChiZhanCheDaiJinQuan_01 = true;
        /// <summary>
        /// 战车代金券02奖池是否无穷大.
        /// </summary>
        internal bool IsWuQiongDaJiangChiZhanCheDaiJinQuan_02 = true;
        /// <summary>
        /// JPBoss代金券奖池是否无穷大.
        /// </summary>
        internal bool IsWuQiongDaJiangChiJPBossDaiJinQuan = false;

        /// <summary>
        /// 无穷大奖池.
        /// </summary>
        float WuQiongDaJiangChi = 999999;

        float _SuiJiDaoJuDeCai = 0f;
        /// <summary>
        /// 随机道具得彩累积数量.
        /// </summary>
        internal float SuiJiDaoJuDeCai
        {
            get
            {
                if (IsWuQiongDaJiangChiSuiJiDaoJuDaiJinQuan == true)
                {
                    _SuiJiDaoJuDeCai = WuQiongDaJiangChi;
                }
                return _SuiJiDaoJuDeCai;
            }
            set
            {
                _SuiJiDaoJuDeCai = value;
            }
        }

        float _ZhanCheDeCai_01 = 0f;
        /// <summary>
        /// 战车得彩累积数量.
        /// 20元代金券累计数量.
        /// </summary>
        internal float ZhanCheDeCai_01
        {
            get
            {
                if (IsWuQiongDaJiangChiZhanCheDaiJinQuan_01 == true)
                {
                    _ZhanCheDeCai_01 = WuQiongDaJiangChi;
                }
                return _ZhanCheDeCai_01;
            }
            set
            {
                _ZhanCheDeCai_01 = value;
            }
        }

        float _ZhanCheDeCai_02 = 0f;
        /// <summary>
        /// 战车得彩累积数量.
        /// 50元代金券累计数量.
        /// </summary>
        internal float ZhanCheDeCai_02
        {
            get
            {
                if (IsWuQiongDaJiangChiZhanCheDaiJinQuan_02 == true)
                {
                    _ZhanCheDeCai_02 = WuQiongDaJiangChi;
                }
                return _ZhanCheDeCai_02;
            }
            set
            {
                _ZhanCheDeCai_02 = value;
            }
        }

        float _JPBossDeCai = 0f;
        /// <summary>
        /// JPBoss得彩累积数量.
        /// </summary>
        internal float JPBossDeCai
        {
            get
            {
                if (IsWuQiongDaJiangChiJPBossDaiJinQuan == true)
                {
                    _JPBossDeCai = WuQiongDaJiangChi;
                }
                return _JPBossDeCai;
            }
            set
            {
                _JPBossDeCai = value;
            }
        }

        /// <summary>
        /// 更新所有服务器配置数据.
        /// </summary>
        internal void UpdataAllServerConfigData()
        {
            //SSDebug.Log("UpdataAllServerConfigData...");
            if (XKGlobalData.GetInstance() != null)
            {
                //更新游戏弹幕信息.
                if (GameDanMuInfo.Length > 10)
                {
                    GameDanMuInfo = GameDanMuInfo.Substring(0, 10);
                }
                XKGlobalData.GetInstance().UpdateDanMuInfo(GameDanMuInfo);
            }

            if (XkPlayerCtrl.GetInstanceFeiJi() != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null
                && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData != null)
            {
                //更新代金券奖池相关数据信息.
                SSCaiPiaoDataManage.GameCaiPiaoData gmCaiPiaoData = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData;
                float chuCaiLv = SuiJiDaoJuChuPiaoLv + ZhanCheChuPiaoLv_01 + ZhanCheChuPiaoLv_02 + JPBossChuPiaoLv;
                if (chuCaiLv > 1f)
                {
                    //出票率(返奖率)之和必须为1f.
                    JPBossChuPiaoLv = 1f;
                    ZhanCheChuPiaoLv_01 = 0f;
                    ZhanCheChuPiaoLv_01 = 0f;
                    SuiJiDaoJuChuPiaoLv = 0f;
                }
                gmCaiPiaoData.UpdateChuPiaoLvInfo(SuiJiDaoJuChuPiaoLv, ZhanCheChuPiaoLv_01, ZhanCheChuPiaoLv_02, JPBossChuPiaoLv);
                gmCaiPiaoData.UpdateDaiJinQuanInfo(SuiJiDaoJuDaiJinQuan, ZhanCheDaiJinQuan_01, ZhanCheDaiJinQuan_02, JPBossDaiJinQuan);
                gmCaiPiaoData.UpdateDaiJinQuanCaiChiInfo(SuiJiDaoJuDeCai, ZhanCheDeCai_01, ZhanCheDeCai_02, JPBossDeCai);
                gmCaiPiaoData.UpdateJPBossDaiJinQuanShangHuZhiFu(JPBossDaiJinQuanShangHuZhiFu);

                if (XKGlobalData.GetInstance() != null)
                {
                    XKGlobalData.GetInstance().SetIsWuXianJiangChi(IsWuQiongDaJiangChiJPBossDaiJinQuan, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan);
                    XKGlobalData.GetInstance().SetIsWuXianJiangChi(IsWuQiongDaJiangChiZhanCheDaiJinQuan_01, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
                    XKGlobalData.GetInstance().SetIsWuXianJiangChi(IsWuQiongDaJiangChiZhanCheDaiJinQuan_02, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
                    XKGlobalData.GetInstance().SetIsWuXianJiangChi(IsWuQiongDaJiangChiSuiJiDaoJuDaiJinQuan, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan);
                }
            }
            
            if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
            {
                //更新游戏免费试玩信息.
                pcvr.GetInstance().m_HongDDGamePadInterface.UpdateMianFeiCountInfo(MianFeiShiWanCount);
                XKGlobalData.GetInstance().SetMianFeiShiWanCount(MianFeiShiWanCount);

                //更新游戏一币等于多少人民币的信息.
                pcvr.GetInstance().m_HongDDGamePadInterface.UpdateGameCoinToMoney(GameCoinToMoney);
            }

            //设置游戏彩池返奖率信息.
            XKGlobalData.GetInstance().SetCaiChiFanJiangLv(CaiChiFanJiangLv);

            //设置游戏彩池爆奖率信息.
            XKGlobalData.GetInstance().SetCaiChiBaoJiangLv(SuiJiDaoJuBaoJiangLv, ZhanCheBaoJiangLv_01, ZhanCheBaoJiangLv_02, JPBossBaoJiangLv);
            if (SSHaiDiLaoBaoJiang.GetInstance() != null)
            {
                SSHaiDiLaoBaoJiang.GetInstance().UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan, (int)JPBossBaoJiangLv);
                SSHaiDiLaoBaoJiang.GetInstance().UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01, (int)ZhanCheBaoJiangLv_01);
                SSHaiDiLaoBaoJiang.GetInstance().UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02, (int)ZhanCheBaoJiangLv_02);
                SSHaiDiLaoBaoJiang.GetInstance().UpdateBaoJiangDt(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan, (int)SuiJiDaoJuBaoJiangLv);
            }
        }
    }
}
