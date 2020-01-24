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
                    return $"{Username} message SENT";
                return $"{Username} sending FAILED";
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
                    return $"{AmountRub} RUB to {Username} RELEASED";
                return $"{AmountRub} RUB to {Username} releasing FAILED";
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
                    return "CONTACTS LOADED";
                return "CONTACTS FAILED";
            }
            set
            {
                return;
            }
        }
    }
    public class LoadRecentMessagesReport : IReport
    {
        public DateTime Time { get; set; }
        public bool Success { get; set; }       
        public string Description
        {
            get
            {
                if (Success)
                    return "RECENT MESSAGES LOADED";
                return "LOADING MSGs FAILED";
            }
            set
            {
                return;
            }
        }
    }
}
