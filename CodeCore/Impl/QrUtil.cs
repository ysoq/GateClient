using System.IO.Ports;
using System.Text;

namespace CodeCore.Impl
{
    internal class QrUtil : IQrUtil
    {

        #region 注册扫码回调事件

        private Action<string> ScanSuccessCallback;
        private string Com;
        SerialPort serialPort = null;

        public string Register(int com, Action<string> success)
        {
            if (com <= 0)
            {
                return "扫码器端口配置错误";
            }
            ScanSuccessCallback = success;
            Com = "com" + com;
            serialPort = new SerialPort(Com);
            serialPort.BaudRate = 115200;
            serialPort.DataReceived += Device1_DataReceived;
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                return $"扫码器{Com}口打开失败";
            }
            return "";
        }
        #endregion

        #region 开始线程扫码
 

        StringBuilder sb = new StringBuilder();
        private void Device1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            sb.Append(indata);
            if (indata.EndsWith("\r") || indata.EndsWith("\r\n"))
            {
                sb.Replace("\r\n", "").Replace("\r", "");
                string code = sb.ToString();
                sb.Clear();
                ScanSuccess(code);
            }
        }
        #endregion

        #region 扫码回调

        private bool IsRun = true;

        public void Run()
        {
            IsRun = true;
        }

        public void Stop()
        {
            IsRun = false;
        }

        private void ScanSuccess(string code)
        {
            if (!IsRun) return;
            ScanSuccessCallback(code);
        }

        #endregion
    }
}
