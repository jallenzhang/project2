using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer.Store
{
    public class user
    {
        public Int64 id { get; set; }
        public string nickname { get; set; }
        public string userid { get; set; }
        public int? usertype { get; set; }
        public string username { get; set; }
        public DateTime? logintime { get; set; }
        public string mail { get; set; }
        public int? role { get; set; }
        public string password { get; set; }
        public Int64? money { get; set; }
        public Int64? exp { get; set; }
        public int? avator { get; set; }
        //筹码
        public Int64? chips { get; set; }
        public int? status { get; set; }
        //胜场数
        public int? wins { get; set; }
        //场景类型
        public int? backgroundtype { get; set; }
        ////背景类型
        //public int backgroundtype { get; set; }
        //房间ID
        public string roomid { get; set; }
        //客厅类型
        public int? livingroomtype { get; set; }

        //胜率
        public decimal? winningpercentage { get; set; }
        //最大赢注
        public Int64? maximumwinnote { get; set; }
        //最好牌型
        public string besthandtype { get; set; }
        //已玩牌局
        public Int64? totalgame { get; set; }

        public string password2 { get; set; }
    }
}



