using Caliburn.Micro;
using Releaser.Models;
using Releaser.Models.LbCode;
using Releaser.Models.Reports;
using Releaser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Releaser.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        public delegate void IterateDel();
        public BindableCollection<Contact> Contacts { get; set; }
        public BindableCollection<Models.LbCode.Message> Messages { get; set; }
        public BindableCollection<IReport> Reports { get; set; }
        private Worker Worker { get; set; }
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
            Worker = new Worker(OnWorkerIterate);
        }
        public void OnWorkerIterate()
        {
            Contacts = new BindableCollection<Contact>(Worker.LatestContacts);
            Messages = new BindableCollection<Models.LbCode.Message>(Worker.RecentMessages);
            NotifyOfPropertyChange(() => Contacts);
            NotifyOfPropertyChange(() => ContactsCount);
            NotifyOfPropertyChange(() => Messages);
            Reports = new BindableCollection<IReport>(Worker.Reports.OrderByDescending(t => t.Time));
            NotifyOfPropertyChange(() => Reports);
        }
        public void ReleaseContact()
        {
            Contact copy = SelectedContact;
            if (copy == null)
                return;
            var choice = MessageBox.Show($"Release btc to {copy.Username} for {copy.AmountRub} Rub?", "MisclickChecker", MessageBoxButton.YesNo);
            if (choice == MessageBoxResult.Yes)
            {
                Worker.ReleaseContact(copy);
            }
            else
            {
                MessageBox.Show($"Releasing cancelled");
            }
        }
        public void LoadRecentMessages()
        {
            Task task = new Task(Worker.MessagesLoop);
            task.Start();
        }
        public void SwitchOnOff()
        {
            if (IsRunning)
            {
                Worker.Stop();
                IsRunning = false;
            }
            else
            {
                Worker.Run();
                IsRunning = true;
            }
            NotifyOfPropertyChange(() => SwitchOnOffContent);
        }
    }
}