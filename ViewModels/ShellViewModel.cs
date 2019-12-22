using Caliburn.Micro;
using Releaser.Models;
using Releaser.Models.LbCode;
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
        public BindableCollection<Contact> Contacts { get; set; }
        private Worker worker { get; set; }
        public string ContactsCount => $"{Contacts.Count} contacts";       
        private bool IsRunning { get; set; }
        public Contact SelectedContact { get; set; }
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
            worker = new Worker(this);           
        }
        public void OnWorkerIterate()
        {
            var report = worker.Reports.Last();        
            if (report.Success)
            {               
                Contacts = new BindableCollection<Contact>(report.Contacts);
                NotifyOfPropertyChange(() => Contacts);
                NotifyOfPropertyChange(() => ContactsCount);
            }
        }
        public void ReleaseContact()
        {
            Contact copy = SelectedContact;
            MessageBox.Show($"Release btc to {copy.Username} for {copy.AmountRub} Rub?", "MissclickChecker", MessageBoxButton.YesNo);
            MessageBox.Show($" btc to {copy.Username} has been released");
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