using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore
{
    public interface ILogger
    {
        void Info(params string[] args);
        void IfInfo(bool yes, params string[] args);
        void Debug(params string[] args);
        void Error(params string[] msg);
        void Error(Exception ex, params string[] msg);
    }
}
