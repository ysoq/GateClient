namespace CodeCore
{
    public interface ICertUtil
    {
        /// <summary>
        /// 注册扫码事件
        /// </summary>
        /// <param name="success">扫码成功事件</param>
        /// <param name="error">扫码、设备初始化错误事件</param>
        string Register(Action<string> success);
    }
}
