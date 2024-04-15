using System.Runtime.InteropServices;
using System.Text;

namespace CodeCore.Impl
{
    internal class CertUtil : ICertUtil
    {
        public CertUtil(ILogger logger)
        {
            this.logger = logger;
        }
        private readonly ILogger logger;
        #region
        [DllImport("Expand/cert_zk.dll")]
        static extern int InitCommExt();//自动搜索身份证阅读器并连接身份证阅读器 

        [DllImport("Expand/cert_zk.dll")]
        static extern int Authenticate();//判断是否有放卡，且是否身份证 

        [DllImport("Expand/cert_zk.dll")]
        public static extern int Read_Content(int index);//读卡操作,信息文件存储在dll所在下

        [DllImport("Expand/cert_zk.dll")]
        extern public static void getIDNum(byte[] data, int cbData);
        [DllImport("Expand/cert_zk.dll")]
        extern public static int GetBmpPhotoExt();

        [DllImport("Expand/cert_zk.dll")]
        static extern int CloseComm();//断开与身份证阅读器连接 


        [DllImport("Expand/cert_zk.dll")]
        extern public static void getAddress(byte[] data, int cbData);

        [DllImport("Expand/cert_zk.dll")]
        extern public static int IC_ReadData(int iPort, int keyMode, int sector, int idx, String key, byte[] data, int cbdata, ref UInt32 icSnr);

        [DllImport("Expand/cert_zk.dll")]
        extern public static int IC_GetICSnr(int iPort, ref UInt32 icSnr);
        [DllImport("Expand/cert_zk.dll")]
        extern public static void getJPGPhotoBase64(byte[] data, int cbData);
        #endregion

        private bool Init()
        {
            return InitCommExt() > 0;
        }
        const int DataSize = 128;
        CertInfo info = new CertInfo();
        bool getInfoing = false;

        private bool GetInfo()
        {
            if (getInfoing)
            {
                return false;
            }
            try
            {
                getInfoing = true;
                int FindCard = Authenticate();
                if (FindCard != 1) return false;

                int rs = Read_Content(1);
                if (!(rs is 1 or 2 or 3 or 4)) return false;
                info.Cert = "";
                info.Photo = "";

                int cbDataSize = 256;
                byte[] value = new byte[cbDataSize];

                getIDNum(value, DataSize);
                info.Cert = Encoding.GetEncoding("GBK").GetString(value).Replace("\0", "");

                Thread.Sleep(10);
                var ret = GetBmpPhotoExt();
                if (ret == 1)
                {
                    value = new byte[128 * 1024];
                    getJPGPhotoBase64(value, 128 * 1024);
                    string strJpegBase64 = Encoding.GetEncoding("GBK").GetString(value).Replace("\0", ""); ;
                    strJpegBase64 = strJpegBase64.Replace("\0", "");
                    if (strJpegBase64.Length > 0)
                    {
                        info.Photo = strJpegBase64;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                getInfoing = false;
            }
            return false;
        }

        public string Register(Action<CertInfo> success)
        {
            logger.Info("初始化身份证模块");

            if (!Init())
            {
                return "初始化失败";
            }

            Quartzer.CreateJob(this, "ReadCert", 1, () =>
            {
                var card = "";
                // 读身份证
                if (GetInfo())
                {
                    success?.Invoke(info);
                }
            });

            return "";
        }
    }

    public record CertInfo
    {
        public string? Cert { get; set; }
        public string? Photo { get; set; }
    }
}
