using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.IO;
using Amib.Threading;

namespace sshserver3
{
    public  class ThreadManager
    {
        Object thisLock = new Object();
        
        public  string sshList { set; get; }
        public  int TotalThreads { set; get; }
        public  int TimeoutSeconds { set; get; }
        public  int DefaultPort { set; get; }
        public  int TotalSSH { set; get; }
        public  int TotalDown { set; get; }
        public  int TotalFresh { set; get; }
        public  int CurrentIndex { set; get; }

        public  bool IsRunning = false;

        public  string[] ListSSH;
        public  StringBuilder ListFresh;
        public  StringBuilder ListDown;
        public string sshdown;
        public  string Message { set; get; }

        public  HashSet<int> Ports = new HashSet<int>();

        public  SmartThreadPool STP { set; get; }

        public  void Start()
        {
        
            IsRunning = true;
            TotalSSH = ListSSH.Length;
            TotalDown = 0;
            TotalFresh = 0;
            CurrentIndex = 0;
            ListFresh = new StringBuilder();
            ListDown = new StringBuilder();

            if (STP != null && STP.IsShuttingdown == false) STP.Shutdown(true, 1000);
            STP = new SmartThreadPool(TimeoutSeconds * 10 * 1000, TotalThreads, TotalThreads);

            for (int i = 0; i < TotalThreads; i++)
            {

                SshChecker sc = new SshChecker();
                sc.LineIndex = CurrentIndex;
                sc.row = ListSSH[CurrentIndex];
                sc.TimeoutSeconds = TimeoutSeconds;
                sc.DefaultPort = DefaultPort;
                sc.passControl = new SshChecker.PassControl(CheckFreshCallbank);
                //new Thread(new ThreadStart(sc.CheckFresh)).Start();
                STP.QueueWorkItem(sc.CheckFresh);
                CurrentIndex++;
                if (CurrentIndex == TotalSSH)
                {
                    break;
                }
            }

            new Thread(new ThreadStart(TimerCheckCompleted)).Start();
        }
        public  void Stop()
        {
            Message = "Stopped by User.";
            IsRunning = false;
        }

        private  void TimerCheckCompleted()
        {
            while (IsRunning)
            {
                Thread.Sleep(2000);

                if (CurrentIndex >= TotalSSH)
                {
                    STP.Shutdown(true, TimeoutSeconds * 3 * 1000);
                    if (STP.IsShuttingdown)
                    {
                        Message = "Check Fresh Completed.";
                        IsRunning = false;
                        break;
                    }
                }

                if (CurrentIndex < TotalSSH)
                {
                    for (int i = STP.InUseThreads; i < TotalThreads; i++)
                    {
                        
                        SshChecker sc = new SshChecker();
                        sc.LineIndex = CurrentIndex;
                        sc.row = ListSSH[CurrentIndex];
                        sc.TimeoutSeconds = TimeoutSeconds;
                        sc.DefaultPort = DefaultPort;
                        sc.passControl = new SshChecker.PassControl(CheckFreshCallbank);

                        //new Thread(new ThreadStart(sc.CheckFresh)).Start();
                        STP.QueueWorkItem(sc.CheckFresh);
                    
                        CurrentIndex++;
                        if (CurrentIndex == TotalSSH)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public  void CheckFreshCallbank(int LineIndex, bool IsFresh, string Ssh, string ErrorMessage)
        {

            lock (thisLock)
            { 
            if (IsFresh)
            {
                if (string.IsNullOrWhiteSpace(Ssh)) return;
                TotalFresh++;
                ListFresh.AppendLine(Ssh + "|");
            }
            else
            {
                TotalDown++;
                ListDown.AppendLine(ListSSH[LineIndex] + "|");
            }
            }
        }
        public string getdownssh()
        {
            lock (thisLock)
            {
                string ret=  ListDown.ToString();
                ListDown.Clear();
                return ret;
            }
        }
    }
}
