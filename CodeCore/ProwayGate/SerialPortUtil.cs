using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeCore.ProwayGate
{
    public class SerialPortUtil
    {
        private SerialPort serialPort;

        private GateUtil gateUtil;

        private const string passInCmd = ":0102PF0001";

        private const string passInTimeout = ":0102PF8000";

        private const string passOutCmd = ":0102NF0001";

        private const string passOutTimeout = ":0102NF8000";

        private const string clearPassIn = ":0101PF0000";

        private const string clearPassOut = ":0101NF0000";

        private const string fireInOpen = ":0103PF0001";

        private const string fireInClose = ":0103PF0000";

        private const string fireOutOpen = ":0103NF0001";

        private const string fireOutClose = ":0103NF0000";

        private const string currentInOpen = ":0104PF0001";

        private const string currentInClose = ":0104PF0000";

        private const string currentOutOpen = ":0104NF0001";

        private const string currentOutClose = ":0104NF0000";

        private const string openCheckIn = ":0105PF0000";

        private const string stopCheckIn = ":0105PF0001";

        private const string openCheckOut = ":0105NF0000";

        private const string stopCheckOut = ":0105NF0001";

        private const string passModel_0 = ":0113LF0000";

        private const string passModel_1 = ":0113LF0001";

        private const string passModel_2 = ":0113LF0002";

        private const string passModel_3 = ":0113LF0003";

        private const string passModel_4 = ":0113LF0004";

        private const string passModel_5 = ":0113LF0005";

        private readonly ILogger logger;

        public SerialPortUtil(GateUtil util, string com)
        {
            serialPort = new SerialPort();
            serialPort.NewLine = "\r\n";
            serialPort.RtsEnable = true;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.PortName = com;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(this.gateReceive);
            serialPort.BaudRate = 9600;
            try
            {
                serialPort.Open();
                gateUtil = util;
            }
            catch (Exception)
            {
                throw new Exception("打开串口失败");
            }

            logger = Util.Injection.GetService<ILogger>()!;
        }

        private void gateReceive(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(30);
            int len = serialPort.BytesToRead;
            byte[] buf = new byte[len];
            serialPort.Read(buf, 0, len);
            string retCmd = Encoding.Default.GetString(buf).Trim().Replace("\r", "").Replace("\n", "");
            handleCommand(retCmd);
        }

        //处理返回
        private void handleCommand(string cmd)
        {
            logger.Info("<<<<", cmd);
            switch (cmd)
            {
                case passInCmd:
                    if (Constant.readCardDirect == 2)
                    {
                        if (Constant.comeInCount > 0)
                        {
                            Constant.comeInCount -= 1;
                        }
                        else
                        {
                            Constant.comeInCount = 0;
                        }
                        callback(PassResult.LPass);
                    }
                    else
                    {
                        if (Constant.comeOutCount > 0)
                        {
                            Constant.comeOutCount -= 1;
                        }
                        else
                            Constant.comeOutCount = 0;
                        callback(PassResult.RPass);
                    }
                    break;
                case passInTimeout:
                    if (Constant.readCardDirect == 2)
                    {
                        Constant.comeInCount = 0;
                        callback(PassResult.LFail);
                    }
                    else
                    {
                        Constant.comeOutCount = 0;
                        callback(PassResult.RFail);
                    }
                    break;
                case passOutCmd:
                    if (Constant.readCardDirect == 2)
                    {
                        if (Constant.comeOutCount > 0)
                        {
                            Constant.comeOutCount -= 1;
                        }
                        else
                            Constant.comeOutCount = 0;
                        callback(PassResult.RPass);
                    }
                    else
                    {
                        if (Constant.comeInCount > 0)
                        {
                            Constant.comeInCount -= 1;
                        }
                        else
                            Constant.comeInCount = 0;
                        callback(PassResult.LPass);
                    }
                    break;
                case passOutTimeout:
                    if (Constant.readCardDirect == 2)
                    {
                        Constant.comeOutCount = 0;
                        callback(PassResult.RFail);
                    }
                    else
                    {
                        Constant.comeInCount = 0;
                        callback(PassResult.LFail);
                    }
                    break;
                case clearPassIn:
                    if (Constant.readCardDirect == 2)
                    {
                        Constant.comeInCount = 0;
                    }
                    else
                    {
                        Constant.comeOutCount = 0;
                    }

                    break;
                case clearPassOut:
                    if (Constant.readCardDirect == 2)
                    {
                        Constant.comeOutCount = 0;
                    }
                    else
                    {
                        Constant.comeInCount = 0;
                    }
                    break;
                case fireInOpen:
                    Constant.fireModel = true;
                    Constant.comeInCount = 0;
                    Constant.comeOutCount = 0;
                    break;
                case fireOutOpen:
                    Constant.fireModel = true;
                    Constant.comeInCount = 0;
                    Constant.comeOutCount = 0;
                    break;
                case fireOutClose:
                    Constant.fireModel = false;
                    break;
                case fireInClose:
                    Constant.fireModel = false;
                    break;
                case currentInOpen:
                    Constant.isNormalOpen = true;
                    Constant.comeInCount = 0;
                    Constant.comeOutCount = 0;
                    break;
                case currentOutOpen:
                    Constant.isNormalOpen = true;
                    Constant.comeInCount = 0;
                    Constant.comeOutCount = 0;
                    break;
                case currentInClose:
                    Constant.isNormalOpen = false;
                    break;
                case currentOutClose:
                    Constant.isNormalOpen = false;
                    break;
                case openCheckIn:
                    Constant.isStopCheck = false;
                    break;
                case openCheckOut:
                    Constant.isStopCheck = false;
                    break;
                case stopCheckOut:
                    Constant.isStopCheck = true;
                    break;
                case stopCheckIn:
                    Constant.isStopCheck = true;
                    break;
                case passModel_0:
                    if (Constant.readCardDirect == 2)
                        Constant.passModel = 0;
                    else
                        Constant.passModel = 2;
                    break;
                case passModel_1:
                    Constant.passModel = 1;
                    break;
                case passModel_2:
                    if (Constant.readCardDirect == 2)
                        Constant.passModel = 2;
                    else
                        Constant.passModel = 0;
                    break;
                case passModel_3:
                    if (Constant.readCardDirect == 2)
                        Constant.passModel = 3;
                    else
                        Constant.passModel = 5;
                    break;
                case passModel_4:
                    Constant.passModel = 4;
                    break;
                case passModel_5:
                    if (Constant.readCardDirect == 2)
                        Constant.passModel = 5;
                    else
                        Constant.passModel = 3;
                    break;

            }

        }

        //设置通行模式
        public bool setPassModel(int num)
        {
            string cmd = ":0113LW000" + num;
            if (Constant.readCardDirect != 1) return writeCommand(cmd);
            cmd = num switch
            {
                0 => ":0113LW0002",
                1 => ":0113LW0001",
                2 => ":0113LW0000",
                3 => ":0113LW0005",
                4 => ":0113LW0004",
                5 => ":0113LW0003",
                _ => cmd
            };

            return writeCommand(cmd);
        }

        public void queryPassModel()
        {
            string cmd = ":0113LR0000";
            serialPort.WriteLine(cmd);
        }

        //设置消防
        public bool setFireModel(bool isOpen)
        {
            return writeCommand(isOpen ? ":0103PW0001" : ":0103PW0000");
        }

        /// <summary>
        /// 闸机开闸
        /// </summary>
        /// <param name="direct">1:进 2:出</param>
        /// <param name="num">开门次数</param>
        public bool openDoor(int direct, int num)
        {
            if (num < 0)
                return false;

            if (Constant.isNormalOpen || Constant.isStopCheck || Constant.fireModel)
                return false;

            if (direct == 1)
            {
                if (Constant.passModel is 0 or 1 or 2 or 5)
                    return false;
                Constant.comeInCount += num;
            }
            else
            {
                if (Constant.passModel is 0 or 1 or 2 or 3)
                    return false;
                Constant.comeOutCount += num;
            }

            if (num == 0)
            {
                return Constant.readCardDirect == 2
                    ? writeCommand(direct == 1 ? ":0101PC0000" : ":0101NC0000")
                    : writeCommand(direct == 2 ? ":0101PC0000" : ":0101NC0000");
            }

            string cmd = ":0101PW";
            if (Constant.readCardDirect == 2)
            {
                cmd = direct == 1 ? ":0101PW" : ":0101NW";
            }
            else
            {
                cmd = direct == 1 ? ":0101NW" : ":0101PW";
            }

            string rs = Convert.ToString(num, 16).ToUpper();
            cmd = rs.Length switch
            {
                1 => cmd + "000" + rs,
                2 => cmd + "00" + rs,
                3 => cmd + "0" + rs,
                _ => cmd + rs
            };

            return writeCommand(cmd);
        }

        //发送指令
        public bool writeCommand(string cmd)
        {
            if (!serialPort.IsOpen) return false;

            logger.Info(">>>>", cmd);
            serialPort.WriteLine(cmd);
            return true;
        }

        public bool SetTimeout(int time)
        {
            if (time <= 0 || time > 60)
                return false;

            byte one = (byte)(time % 16);
            byte two = (byte)(time / 16);

            string cmd = ":0111LW" + two.ToString().PadLeft(2, '0');
            cmd += one.ToString().PadLeft(2, '0');
            return writeCommand(cmd);
        }

        private void callback(PassResult result)
        {
            gateUtil.handleResult(result);
        }
    }
}
