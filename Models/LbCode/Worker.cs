using Releaser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Releaser.Data;

namespace Releaser.Models.LbCode
{
    public class Worker
    {
        #region Events
        
        public event NewContactHandler OnNewContact;
        public delegate void NewContactHandler(NewContactArgs args);
        public class NewContactArgs : EventArgs
        {
            public List<Contact> NewContacts { get; set; }
        }
        
        public event PriceOvercomeHandler OnPriceOvercome;
        public delegate void PriceOvercomeHandler(PriceOvercomeArgs args);
        public class PriceOvercomeArgs : EventArgs
        {
            
        }
        #endregion
        
        
        private LbWrapper lbcore;
        private Thread workerThread;
        private readonly ShellViewModel.IterateDel _del;
        private RealeaserDbContext db = new RealeaserDbContext();
        public List<UpdateReport> Reports { get; set; }
        private bool IsRunning { get; set; }
        
        
        public Worker(ShellViewModel.IterateDel del)
        {
            _del = del;
            Reports = new List<UpdateReport>();
            lbcore = new LbWrapper("", "", "");
            workerThread = new Thread(Loop);

            OnNewContact += AddContractsToBd;
        }
        private void Loop()
        {
            while (IsRunning)
            {
                CheckNewContract();
                //CheckPriceOvercome();
                _del();
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

        private void CheckNewContract()
        {
            List<Contact> contacts = lbcore.GetContacts();
            UpdateReport report = new UpdateReport()
            {
                Time = DateTime.Now,
                Success = contacts != null,
                Contacts = contacts
            };
            Reports.Add(report);

            List<Contact> newContacts = contacts?.Except(db.Contacts).ToList();
            if(newContacts != null)
                OnNewContact(new NewContactArgs()
                {
                    NewContacts = newContacts
                });
        }
        private void CheckPriceOvercome()
        {

            OnPriceOvercome(new PriceOvercomeArgs());
        }

        private void AddContractsToBd(NewContactArgs args)
        {
            foreach (var contact in args.NewContacts)
            { 
                db.Contacts.Add(contact);
            }

            db.SaveChanges();
        }
        
        public class UpdateReport
        {
            public DateTime Time { get; set; }
            public bool Success { get; set; }
            public List<Contact> Contacts { get; set; }
        }
    }
}