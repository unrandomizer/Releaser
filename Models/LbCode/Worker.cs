using Releaser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Releaser.Data;
using Releaser.Models.Reports;
using System.Diagnostics;
using System.Windows;

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
            if (db.Users.Count() == 0)
            {
                MessageBox.Show("Table 'Users' are empty. Program will be closed", "Warning");
                Environment.Exit(0);
            }
            User user = db.Users.First();
            lbcore = new LbWrapper(user.ApiKey, user.ApiSecret, user.Username);
            workerThread = new Thread(Loop);

            OnNewContact += SendMessages;
            OnNewContact += AddContactsToDb;
            OnNewContact += x => Console.WriteLine(x.NewContacts.Count);
        }
        private void Loop()
        {
            while (IsRunning)
            {
                Debug.WriteLine("Loop() iteration. Checking Contacts");
                CheckNewContact();
                //CheckPriceOvercome();
                _del();
                Thread.Sleep(5000);
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
            ReleaseContactReport report = new ReleaseContactReport()
            {
                Time = DateTime.Now,
                Success = lbcore.ReleaseBitcoins(contact.Id),
                Username = contact.Username,
                AmountRub = contact.AmountRub
            };
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
            List<Contact> newContacts = contacts?.Except(db.Contacts).ToList();
            if (newContacts != null)
            {
                OnNewContact?.Invoke(new NewContactArgs()
                {
                    NewContacts = newContacts
                });
            }
            _del();
        }
        private void CheckPriceOvercome()
        {
            // Some Compare and change logic hire.
            OnPriceOvercome?.Invoke(new PriceOvercomeArgs());
        }
        private void SendMessages(NewContactArgs args)
        {
            List<Contact> toSend = db.Contacts.Where(x => !x.IsMessageSent && !x.IsBuying).ToList();
            foreach (Contact contact in toSend)
            {
                bool sendMessageAttempt = lbcore.SendMessage(contact.Id, db.Users.First().Message);
                if (sendMessageAttempt)
                    contact.IsMessageSent = true;
                PostMessageReport report = new PostMessageReport
                {
                    Success = sendMessageAttempt,
                    Time = DateTime.Now,
                    Username = contact.Username
                };
                Reports.Add(report);
            }
        }
        private void AddContactsToDb(NewContactArgs args)
        {
            foreach (Contact contact in args.NewContacts)
            {
                db.Contacts.Add(contact);
            }
            db.SaveChanges();
        }
    }
}