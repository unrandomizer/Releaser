using Caliburn.Micro;
using Releaser.Models;
using Releaser.Models.LbCode;
using Releaser.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Releaser.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        public delegate void IterateDel();
        public BindableCollection<Contact> Contacts { get; set; }
        public BindableCollection<IReport> Reports { get; set; }
        private Worker worker { get; set; }
        public string ContactsCount => $"{Contacts.Count} contacts";
        private bool IsRunning { get; set; }
        private Contact _selectedContact;
        public Contact SelectedContact
        {
            get
            {
                return _selectedContact;
            }
            set
            {
                if (value == null)
                    return;
                _selectedContact = value;
                NotifyOfPropertyChange(() => SelectedContact);
            }
        }
        public string SwitchOnOffContent
        {
            get
            {
                if (IsRunning)
                    return "STOP";
                return "START";
            }
        }
        public ShellViewModel()
        {
            Contacts = new BindableCollection<Contact>();
            worker = new Worker(OnWorkerIterate);
        }
        public void OnWorkerIterate()
        {
            Contacts = new BindableCollection<Contact>(worker.LatestContacts);
            NotifyOfPropertyChange(() => Contacts);
            NotifyOfPropertyChange(() => ContactsCount);
            Reports = new BindableCollection<IReport>(worker.Reports);
            NotifyOfPropertyChange(() => Reports);
        }
        public void ReleaseContact()
        {
            Contact copy = SelectedContact;
            var choice = MessageBox.Show($"Release btc to {copy.Username} for {copy.AmountRub} Rub?", "MissclickChecker", MessageBoxButton.YesNo);
            if (choice == MessageBoxResult.Yes)
            {
                MessageBox.Show($" btc to {copy.Username} is being released");
                worker.ReleaseContact(copy);
            }
            else
            {
                MessageBox.Show($"Releasing cancelled");
            }
        }
        public void SwitchOnOff()
        {
            if (IsRunning)
            {
                worker.Stop();
                IsRunning = false;
            }
            else
            {
                worker.Run();
                IsRunning = true;
            }
            NotifyOfPropertyChange(() => SwitchOnOffContent);
        }
    }
}