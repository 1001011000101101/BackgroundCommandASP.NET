using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace BackgroundCommandASP.NET.Models
{
    //Invoker (Command design pattern)
    public sealed class EventWorker
    {
        private Thread Thread;
        private bool Started;
        private bool NeedStop;
        private int ErrorsLimit = 5;

        private static volatile EventWorker worker;
        private static object locker = new Object();

        Dictionary<string, IWorker> Workers;

        private EventWorker()
        {
            Workers = new Dictionary<string, IWorker>();
            Thread = new Thread(new ThreadStart(WorkerMethod));
        }

        public static EventWorker Worker
        {
            get
            {
                if (worker == null)
                {
                    lock (locker)
                    {
                        if (worker == null)
                            worker = new EventWorker();
                    }
                }

                return worker;
            }
        }

        public bool IsAlive
        {
            get
            {
                bool v = false;
                lock (locker)
                {
                    v = Started && Workers.Any();
                }
                return v;
            }
        }

        public void Start()
        {
            if (Started)
                return;
            lock (locker)
            {
                if (Started)
                    return;
                NeedStop = false;
                if (Thread.ThreadState != ThreadState.Unstarted)
                {
                    Thread = new Thread(new ThreadStart(WorkerMethod));
                }
                Thread.Start();
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                NeedStop = true;
            }
        }
        public void RegisterWorker(IWorker Worker)
        {
            string key = Worker.GetHashCode().ToString();
            this.RegisterWorker(key, Worker);
        }
        public void RegisterWorker(string Key, IWorker Worker)
        {
            Workers[Key] = Worker;
        }

        private void WorkerMethod()
        {
            Started = true;
            try
            {
                while (!NeedStop)
                {
                    List<string> keys = Workers.Keys.ToList();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        IWorker w;
                        if (Workers.TryGetValue(keys[i], out w) && w != null)
                        {
                            if (w.IsBusy)
                            {
                                continue;
                            }
                            try
                            {
                                w.DoWork();
                            }
                            catch (Exception ex)
                            {
                                w.ErrorsCount++;
                            }
                            if (w.ErrorsCount > ErrorsLimit)
                            {
                                Workers.Remove(keys[i]);
                                if (w is IDisposable)
                                {
                                    ((IDisposable)w).Dispose();
                                }

                            }
                        }
                    }


                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Started = false;
            }
        }
    }
}