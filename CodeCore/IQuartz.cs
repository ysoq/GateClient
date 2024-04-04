using Quartz;

namespace CodeCore
{
    public interface IQuartz: IJob
    {

        public void CreateJob(object owner, string token, ITrigger trigger, Action action);

        public void CreateJob(object owner, string token, double seconds, bool startNow, Action action);

        public   void CreateJob(object owner, string token, double seconds, Action action);

        public Task Remove(string key);

        public Task RemoveAll(object owner);


        /// <summary>
        /// 设置当前任务正在执行，锁定后下次任务则会跳过
        /// </summary>
        /// <param name="token"></param>
        public   void Lock(string token);

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="token"></param>
        public void Unlock(string token);
    }
}
