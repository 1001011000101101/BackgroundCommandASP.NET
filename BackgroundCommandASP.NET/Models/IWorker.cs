using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundCommandASP.NET.Models
{
    public interface IWorker
    {
        void DoWork();
        bool IsBusy { get; }
        int ErrorsCount { get; set; }
    }
}
