using Releaser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Releaser.Data;
using Releaser.Models.Reports;

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
        public List<Contact> LatestContacts { get; set; }
        public List<IReport> Reports { get; set; }
        private bool IsRunning { get; set; }


        public Worker(ShellViewModel.IterateDel del)
        {
            _del = del;
            Reports = new List<IReport>();
            LatestContacts = new List<Contact>();
            lbcore = new LbWrapper("", "", "");
            workerThread = new Thread(Loop);

            OnNewContact += AddContactsToDb;
        }
        private void Loop()
        {
            while (IsRunning)
            {
                CheckNewContact();
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
        public void ReleaseContact(Contact contact)
        {
            ReleaseContactReport report = new ReleaseContactReport();
            report.Time = DateTime.Now;
            bool result = lbcore.ReleaseBitcoins(contact.Id);
            report.Success = result;
            report.Username = contact.Username;
            report.AmountRub = contact.AmountRub;
            Reports.Add(report);
            _del();
        }
        private void CheckNewContact()
        {
            List<Contact> contacts = lbcore.GetContacts();
            CheckNewContactsReport report = new CheckNewContactsReport()
            {
                Time = DateTime.Now,
                Success = contacts != null,
                Contacts = contacts
            };
            Reports.Add(report);
            if (contacts != null)
                LatestContacts = contacts;

            List<Contact> contactsInDb = db.Contacts.ToList();
            List<Contact> newContacts = contacts?.Except(contactsInDb).ToList();
            if (newContacts != null)
                OnNewContact(new NewContactArgs()
                {
                    NewContacts = newContacts
                });
            _del();
        }
        private void CheckPriceOvercome()
        {
            OnPriceOvercome(new PriceOvercomeArgs());
        }

        private void AddContactsToDb(NewContactArgs args)
        {
            using (var dbn = new RealeaserDbContext())
            {
                foreach (var contact in args.NewContacts)
                {
                    dbn.Contacts.Add(contact);
                }
                dbn.SaveChanges();
            }
        }
    }
}