using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateClient.Dto
{
    public class GateDb
    {
        public GateBaseInfo? GateInfo { get; set; }
        public GateinTask? GateinTask { get; set; }

        // 获取景点id
        public string? GetSpotId()
        {
            return GateinTask?.spotTaskList?.FirstOrDefault()?.spotId;
        }
        // 获取景点名称
        public string? GetSpotName()
        {
            return GateinTask?.spotTaskList?.FirstOrDefault()?.sportName;
        }

        public string? GetAreaType()
        {
            return GateInfo?.areaType;
        }
    }

    public class GateBaseInfo
    {
        //1 车站 2码头  3 景点
        public string areaType { get; set; }
        //所在区域代码
        public string areaCode { get; set; }
        public string areaName { get; set; }
        //1入闸检票  2出闸检票
        public string workMode { get; set; }
        // 0正常运行 1停止运行
        public string status { get; set; }
        //是否打开人脸校验 0打开 1关闭
        public string isOpenFaceVerify { get; set; }
    }

    public class GateinTask
    {
        public Shiptasklist[]? shipTaskList { get; set; }
        public Spottasklist[]? spotTaskList { get; set; }
    }

    public class Shiptasklist
    {
        public string flightinfoId { get; set; }
        public string flightId { get; set; }
        public string flightName { get; set; }
        public string startPortName { get; set; }
        public string endPortName { get; set; }
        public string flightCode { get; set; }
        public string setoffDate { get; set; }
        public string setoffTime { get; set; }
        public string endsetoffTime { get; set; }
        public string shipId { get; set; }
        public string flightShipCode { get; set; }
        //0待开航  1 到港 2 检票中 9结束检票 3 已开航
        public string status { get; set; }
        public string passengerSeatNum { get; set; }
    }

    public class Spottasklist
    {
        public string spotId { get; set; }
        public string sportName { get; set; }
    }
}


