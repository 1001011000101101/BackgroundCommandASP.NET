using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundCommandASP.NET.Models
{
    //Command (Command design pattern)
    public class CleanFiles : IWorker
    {
        private FilesCleaner FilesCleaner;

        public int ErrorsCount { get; set; }
        public bool IsBusy { get; private set; }

        public CleanFiles(FilesCleaner FilesCleaner)
        {
            this.FilesCleaner = FilesCleaner;
        }

        public void DoWork()
        {
            FilesCleaner.DeleteFiles();
        }
    }
}