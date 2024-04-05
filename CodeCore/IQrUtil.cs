using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore
{
    public interface IQrUtil
    {
        /// <summary>
        /// 注册扫码事件
        /// </summary>
        /// <param name="com">扫码器端口号 use类型扫码器无需设置</param>
        /// <param name="success">扫码成功事件</param>
        /// <param name="error">扫码、设备初始化错误事件</param>
        string Register(int com, Action<string> success);

        /// <summary>
        /// 解除Stop方法操作，开始正常扫码
        /// </summary>
        void Run();

        /// <summary>
        /// 临时暂停扫码，此操作不会关闭设备，只是不会调用回调事件
        /// </summary>
        void Stop();
    }
}
