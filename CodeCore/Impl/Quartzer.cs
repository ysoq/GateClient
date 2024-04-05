using Quartz.Impl;
using Quartz;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCore.Impl
{
    public class Quartzer : IJob
    {
        public static Dictionary<string, Action> ActionList { get; } = new();
        public static Dictionary<string, bool> LockData { get; } = new();
        public static Dictionary<object, List<string>> OwnerKey { get; } = new();

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    JobDataMap dataMap = context.JobDetail.JobDataMap;
                    string token = dataMap.GetString("token")!;

                    if (LockData.TryGetValue(token, out bool isLock) && isLock)
                    {
                        return;
                    }

                    if (ActionList.TryGetValue(token, out Action? action))
                    {
                        action?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    var logger = Util.Injection.GetService<ILogger>();
                    logger?.Error(ex);
                }
            });
        }


        public static void CreateJob(object owner, string token, ITrigger trigger, Action action)
        {
            try
            {
                lock (ActionList)
                {
                    if (!OwnerKey.ContainsKey(owner))
                    {
                        OwnerKey[owner] = new List<string>();
                    }
                    OwnerKey[owner].Add(token);

                    if (ActionList.ContainsKey(token))
                    {
                        Remove(token);
                    }
                    var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                    if (!scheduler.IsStarted)
                    {
                        scheduler.Start().Wait();
                    }
                    ActionList.Add(token, action);
                    IJobDetail job = JobBuilder.Create<CodeCore.Impl.Quartzer>()
                                     .WithIdentity(new JobKey(token)) // name "myJob", group "group1"
                                     .UsingJobData("token", token)
                                     .Build();

                    scheduler.ScheduleJob(job, trigger).Wait();      //把作业，触发器加入调度器。
                }
            }
            catch (Exception ex)
            {
                var logger = Util.Injection.GetService<ILogger>();
                logger?.Error(ex);
            }
        }

        public static void CreateJob(object owner, string token, double seconds, bool startNow, Action action)
        {
            // 创建触发器
            TriggerBuilder triggerBuilder = TriggerBuilder.Create();

            if (!startNow)
            {
                triggerBuilder.StartAt(SystemTime.UtcNow().AddSeconds(seconds));
            }

            var trigger = triggerBuilder.WithSimpleSchedule(x => x
                              .WithInterval(TimeSpan.FromMilliseconds(seconds * 1000))
                              .RepeatForever())
                              .Build();

            CreateJob(owner, token, trigger, action);
        }

        public static void CreateJob(object owner, string token, double seconds, Action action)
        {
            CreateJob(owner, token, seconds, true, action);
        }

        public static void Once(object owner, string token, double seconds, Action action)
        {
            // 创建触发器
            TriggerBuilder triggerBuilder = TriggerBuilder.Create().StartAt(SystemTime.UtcNow().AddSeconds(seconds));

            var trigger = triggerBuilder.WithSimpleSchedule(x => 
                                                            x.WithInterval(TimeSpan.FromMilliseconds(seconds * 1000))
                                                             .WithRepeatCount(0)
                                                            )
                              .Build();

            CreateJob(owner, token, trigger, action);
        }

        public static async Task Remove(string key)
        {
            try
            {
                if (ActionList.ContainsKey(key))
                {
                    var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                    await scheduler.DeleteJob(new JobKey(key));
                    ActionList.Remove(key);
                }
            }
            catch (Exception ex)
            {
                var logger = Util.Injection.GetService<ILogger>();
                logger?.Error(ex);
            }
        }

        public static async void RemoveAll(object owner)
        {
            try
            {
                if (OwnerKey.TryGetValue(owner, out List<string>? list))
                {
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            await Remove(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = Util.Injection.GetService<ILogger>();
                logger?.Error(ex);
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
    }

}