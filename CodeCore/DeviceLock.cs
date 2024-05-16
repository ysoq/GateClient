using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore
{
    /// <summary>
    /// 防连刷
    /// </summary>
    public class DeviceLock
    {
        public static TimeSpan ThresholdValue = TimeSpan.FromSeconds(3);

        /// <summary>
        /// 扫码、证件读取当前是否可继续
        /// </summary>
        /// <returns></returns>
        public static bool Security(string code)
        {
            if (!_record.ContainsKey(code))
            {
                return true;
            }
            DateTime dateTime = _record[code];
            if (dateTime == DateTime.MinValue || TimeInThresholdValue(dateTime))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检验一个时间是否在安全阈值内
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static bool TimeInThresholdValue(DateTime dateTime)
        {
            return dateTime.Add(ThresholdValue) > DateTime.Now;
        }

        static Dictionary<string, DateTime> _record = new Dictionary<string, DateTime>();

        /// <summary>
        /// 设备行为点记录
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public static void RecordPoint(DeviceLock.Point point, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return;
            }
            lock (_record)
            {
                if (point == Point.NetworkRequest)
                {
                    _record[code] = DateTime.MinValue;
                }
                else if (point == Point.NetworkResponse)
                {
                    _record[code] = DateTime.Now;


                    var delKeys = _record.Where(x => !TimeInThresholdValue(x.Value)).Select(x => x.Key).ToList();
                    foreach (var key in delKeys)
                    {
                        _record.Remove(key);
                    }
                }
            }
        }

        public enum Point
        {
            NetworkRequest,
            NetworkResponse,
        }
    }

}
