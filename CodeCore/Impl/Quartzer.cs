
using Microsoft.Extensions.DependencyInjection;

namespace CodeCore.Impl
{
    public class TaskDispatch
    {
        private static Dictionary<string, System.Timers.Timer> TimerList { get; } = new();
        private static Dictionary<string, DelayedExecution> OnceList { get; } = new();
        private static Dictionary<string, bool> LockData { get; } = new();

        public static void CreateJob(string token, double seconds, Action action)
        {
            if (TimerList.ContainsKey(token))
            {
                return;
            }
            lock (TimerList)
            {
                var timer = new System.Timers.Timer(seconds * 1000);
                timer.Elapsed += (sender, e) =>
                {
                    if (!LockData.ContainsKey(token) || !LockData[token])
                    {
                        try { action(); }
                        catch (Exception ex)
                        {
                            Util.Injection.GetService<ILogger>()?.Error(ex);
                        }
                    }
                };
                timer.Enabled = true;
                TimerList.Add(token, timer);
            }
        }

        public static void Once(string token, double seconds, Action action)
        {
            try
            {
                lock (OnceList)
                {
                    DelayedExecution delayed;
                    if (OnceList.ContainsKey(token))
                    {
                        delayed = OnceList[token];
                    }
                    else
                    {
                        delayed = new DelayedExecution();
                        OnceList.Add(token, delayed);
                    }
                    delayed.Execute(seconds, action);
                }
            }
            catch (Exception ex)
            {
                Util.Injection.GetService<ILogger>()?.Error(ex);
            }
        }

        public static void Remove(string key)
        {
            try
            {
                lock (TimerList)
                {
                    if (TimerList.ContainsKey(key))
                    {
                        TimerList[key].Stop();
                        TimerList[key].Dispose();
                        TimerList.Remove(key);
                    }
                }

            }
            catch (Exception ex)
            {
                Util.Injection.GetService<ILogger>()?.Error(ex);
            }
        }

        /// <summary>
        /// 设置当前任务正在执行，锁定后下次任务则会跳过
        /// </summary>
        /// <param name="token"></param>
        public static void Lock(string token)
        {
            LockData[token] = true;
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="token"></param>
        public static void Unlock(string token)
        {
            LockData.Remove(token);
        }

        private class DelayedExecution
        {
            private CancellationTokenSource _cancellationTokenSource;

            public void Execute(double seconds, Action action)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                Task.Delay((int)(seconds * 1000), token).ContinueWith(t =>
                {
                    if (!t.IsCanceled)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            Util.Injection.GetService<ILogger>()?.Error(ex);
                        }
                    }
                }, token);
            }
        }
    }
}