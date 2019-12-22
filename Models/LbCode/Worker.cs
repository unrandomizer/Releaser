using Releaser.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Releaser.Models.LbCode
{
    public class Worker
    {
        private LbWrapper lbcore;
        private Thread workerThread;
        private ShellViewModel vm;
        public List<UpdateReport> Reports { get; set; }
        private bool IsRunning { get; set; }
        public Worker(ShellViewModel shellvm)
        {
            vm = shellvm;
            Reports = new List<UpdateReport>();
            lbcore = new LbWrapper("", "", "");
            workerThread = new Thread(Loop);
        }
        private void Loop()
        {
            while (IsRunning)
            {
                UpdateReport report = new UpdateReport();
                report.Time = DateTime.Now;
                List<Contact> contacts = lbcore.GetContacts();
                if (contacts != null)
                    report.Success = true;
                report.Contacts = contacts;
                Reports.Add(report);
                vm.OnWorkerIterate();
                Thread.Sleep(4000);
            }
        }
        public void Run()
        {
            IsRunning = true;
            workerThread = new Thread(Loop);
            workerThread.Start();
        }
        public void Stop()
        {
            IsRunning = false;         
        }
        public class UpdateReport
        {
            public DateTime Time { get; set; }
            public bool Success { get; set; }
            public List<Contact> Contacts { get; set; }
        }
    }
}