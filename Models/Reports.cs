using Releaser.Models.LbCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Releaser.Models.Reports
{
    public interface IReport
    {
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public bool Success { get; set; }
    }
    public class PostMessageReport : IReport
    {
        public DateTime Time { get; set; }
        public bool Success { get; set; }
        public string Username { get; set; }        
        public string Description
        {
            get
            {
                if (Success)
                    return $"Message to {Username} has been sent";
                return $"Sending msg to {Username} has been failed";
            }
            set
            {
                return;
            }
        }
    }
    public class ReleaseContactReport : IReport
    {
        public DateTime Time { get; set; }
        public bool Success { get; set; }
        public string Username { get; set; }
        public decimal AmountRub { get; set; }
        public string Description
        {
            get
            {
                if (Success)
                    return $"{AmountRub} RUB to {Username} released!";
                return $"Releasing {AmountRub} RUB to {Username} has been failed";
            }
            set
            {
                return;
            }
        }
    }
    public class CheckNewContactsReport : IReport
    {
        public DateTime Time { get; set; }
        public bool Success { get; set; }
        public List<Contact> Contacts { get; set; }
        public string Description
        {
            get
            {
                if (Success)
                    return "Contacts were loaded";
                return "Contact's loading has been failed";
            }
            set
            {
                return;
            }
        }
    }
}
