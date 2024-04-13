using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore.ProwayGate
{
    public class GateUtil
    {
        public event PassHandler handler;

        private SerialPortUtil serialPortUtil;

        private bool isInit = false;

        public void handleResult(PassResult passResult)
        {
            if (handler != null)
            {
                handler(passResult);
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="pCom"></param>
        /// <returns></returns>
        public bool OpenCom(int pCom)
        {
            string com = "COM" + pCom;
            try
            {
                serialPortUtil = new SerialPortUtil(this, com);

                //加载文件
                Constant.loadFile();
                //查询通行模式
                serialPortUtil.queryPassModel();

                isInit = true;
                return isInit;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置闸机刷开方向
        /// </summary>
        /// <param name="direct"></param>
        /// <returns></returns>
        public bool SetDirection(int direct)
        {
            if (!isInit)
                return false;

            Constant.readCardDirect = direct;
            Constant.reWrite("readCardDirect:" + direct);
            return isInit;
        }

        /// <summary>
        /// 设置通行模式
        /// </summary>
        /// <param name="pModel"></param>
        /// <returns></returns>
        public bool SetModel(int pModel)
        {
            return serialPortUtil.setPassModel(pModel);
        }

        bool fireModel = false;
        public bool SetFiremodel(bool pModel)
        {
            fireModel = pModel;
            return serialPortUtil.setFireModel(pModel);
        }

        //设置正向开闸
        public bool SetIntimes(int pTimes)
        {
            if (!Util.Accredit)
            {
                return false;
            }
            if (!isInit)
                return false;
            var ok = serialPortUtil.openDoor(1, pTimes);
            if(fireModel)
            {
                return false;
            }
            return ok;
        }

        public bool SetOuttimes(int pTimes)
        {
            if (!isInit)
                return false;
            return serialPortUtil.openDoor(2, pTimes);
        }

        public int GetInRemaintimes()
        {
            return Constant.comeInCount;
        }

        public int GetOutRemaintimes()
        {
            return Constant.comeOutCount;
        }

        public bool SendCommand(string cmd)
        {
            return serialPortUtil.writeCommand(cmd);
        }

        public bool SetTimeout(int time)
        {
            if (!isInit)
                return false;
            return serialPortUtil.SetTimeout(time);
        }

    }
}
