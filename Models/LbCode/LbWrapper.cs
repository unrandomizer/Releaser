using LocalBitcoins;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Releaser.Models.LbCode
{
    public class LbWrapper
    {
        LocalBitcoinsClient client;
        public string Username { get; set; }
        public LbWrapper(string apiKey, string apiSecret, string username)
        {
            Username = username;
            client = new LocalBitcoinsClient(apiKey, apiSecret);
        }
        public List<OwnAd> GetOwnAds()
        {
            try
            {
                List<OwnAd> result = new List<OwnAd>();
                var smth = client.GetOwnAds().Result;
                IEnumerable<dynamic> list = smth.data.ad_list;
                foreach (dynamic x in list)
                {
                    OwnAd ad = new OwnAd();
                    ad.Equation = x.data.price_equation;
                    ad.Id = x.data.ad_id;
                    ad.MinOrderAmount = x.data.min_amount;
                    ad.IsActive = x.data.visible;
                    string type = x.data.trade_type;
                    if (type.Equals("ONLINE_SELL"))
                    {
                        ad.IsSell = true;
                    }
                    result.Add(ad);
                }
                return result;
            }
            catch (Exception e)
            {
                // logging 
                return null;
            }
        }
        public List<Contact> GetContacts()
        {
            try
            {
                List<Contact> result = new List<Contact>();
                var smth = client.GetDashboard().Result;
                IEnumerable<dynamic> list = smth.data.contact_list;
                foreach (dynamic x in list)
                {
                    Contact contact = new Contact();
                    contact.Id = x.data.contact_id;
                    contact.RefCode = x.data.reference_code;
                    contact.CreatedAt = x.data.created_at;
                    contact.AmountBtc = x.data.amount_btc;
                    contact.AmountRub = x.data.amount;
                    contact.IsBuying = x.data.is_buying;
                    if (contact.IsBuying)
                    {
                        contact.Username = x.data.seller.name;
                        contact.FeedbackScore = x.data.seller.feedback_score;
                    }
                    else
                    {
                        contact.Username = x.data.buyer.name;
                        contact.FeedbackScore = x.data.buyer.feedback_score;
                    }
                    if (x.data.payment_completed_at != null)
                    {
                        contact.MarkedAsPaid = true;
                    }
                    result.Add(contact);
                }
                return result;
            }
            catch (Exception e)
            {
                // logging 
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        public List<Message> GetMessagesForContactId(int contactId)
        {
            try
            {
                List<Message> result = new List<Message>();
                var smth = client.GetContactMessages(contactId.ToString()).Result;
                IEnumerable<dynamic> list = smth.data.message_list;
                foreach (dynamic x in list)
                {
                    Message msg = new Message
                    {
                        ContactId = x.contact_id,
                        Msg = x.msg,
                        IsAdmin = x.is_admin,
                        CreatedAt = x.created_at
                    };
                    if (!msg.IsAdmin)
                        msg.SenderUsername = x.sender.username;
                    result.Add(msg);
                }
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<Message> GetRecentMessages()
        {
            try
            {
                List<Message> result = new List<Message>();               
                var smth = client.GetRecentMessages().Result;
                IEnumerable<dynamic> list = smth.data.message_list;
                foreach (dynamic x in list)
                {
                    Message msg = new Message
                    {
                        ContactId = x.contact_id,
                        Msg = x.msg,
                        IsAdmin = x.is_admin,
                        CreatedAt = x.created_at
                    };
                    if (!msg.IsAdmin)
                        msg.SenderUsername = x.sender.username;
                    result.Add(msg);
                }
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool SendMessage(int contactId, string msg)
        {           
            try
            {
                dynamic resp = client.PostMessageToContact(contactId.ToString(), msg).Result;
                // log response
                if (resp.data.message != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                // logging
                return false;
            }
        }
        public bool UpdatePrice(int adId, decimal price)
        {
            try
            {
                var response = client.EditAdPriceEquation(adId.ToString(), price).Result;
                // log response
                return true;
            }
            catch (Exception e)
            {
                // log exception
                return false;
            }
        }
        public List<Ad> GetSellAds()
        {
            try
            {
                List<Ad> result = new List<Ad>();
                var something = client.PublicMarket_BuyBitcoinsOnlineByCurrency("RUB", "transfers-with-specific-bank").Result.data.ad_list;
                foreach (var ad in something)
                {
                    Ad alienAd = new Ad
                    {
                        Advertiser = ad.data.profile.username,
                        MinAmount = ad.data.min_amount,
                        Price = ad.data.temp_price,
                        BankName = ad.data.bank_name
                    };
                    result.Add(alienAd);
                }
                return result;
            }
            catch (Exception e)
            {
                // exception logging 
                return null;
            }
        }
        public List<Ad> GetBuyAds()
        {
            try
            {
                List<Ad> result = new List<Ad>();
                var something = client.PublicMarket_SellBitcoinsOnlineByCurrency("RUB", "transfers-with-specific-bank").Result.data.ad_list;
                foreach (var ad in something)
                {
                    Ad alienAd = new Ad
                    {
                        Advertiser = ad.data.profile.username,
                        MinAmount = ad.data.min_amount,
                        Price = ad.data.temp_price,
                        BankName = ad.data.bank_name
                    };
                    result.Add(alienAd);
                }
                return result;
            }
            catch (Exception e)
            {
                // exception logging
                return null;
            }
        }
        public bool ReleaseBitcoins(int contactId)
        {
            try
            {
                var resp = client.ContactRelease(contactId.ToString()).Result;
                string msg = resp.data.message;
                Debug.WriteLine(msg);
                // log resp
                return true;
            }
            catch (Exception e)
            {
                //log exception
                if (e.InnerException != null)
                    Debug.WriteLine(e.InnerException.Message);
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
    public class OwnAd
    {
        public int Id { get; set; }
        public bool IsSell { get; set; }
        public bool IsActive { get; set; }
        public decimal Equation { get; set; }
        public decimal MinOrderAmount { get; set; }
    }
    public class Ad
    {
        public decimal Price { get; set; }
        public decimal? MinAmount { get; set; }
        public string Advertiser { get; set; }
        public string BankName { get; set; }
    }
    public class Message
    {       
        public string Msg { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public string SenderUsername { get; set; }
        public int ContactId { get; set; }
    }
    public class Contact : IEquatable<Contact>
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [NotMapped]
        public string Elapsed
        {
            get
            {
                TimeSpan timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.TotalSeconds < 60)
                { return $"{timeSpan.TotalSeconds.ToString("00")}s ago"; }
                return $"{timeSpan.TotalMinutes.ToString("0")}m ago";
            }
        }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string Username { get; set; }
        public string RefCode { get; set; }
        public decimal AmountRub { get; set; }
        [NotMapped]
        public string AmountKiloRub
        {
            get
            {
                string result =((int)AmountRub / 1000).ToString(CultureInfo.GetCultureInfo("en-US"));
                return result;
            }
        }
        public decimal AmountBtc { get; set; }
        [NotMapped]
        public int FeedbackScore { get; set; }
        [NotMapped]
        public string UsernameColor
        {
            get
            {
                if (FeedbackScore < 96)
                    return "#ff9900";
                return "#b3b3b3";
            }
        }
        public bool IsBuying { get; set; }
        [NotMapped]
        public string IsBuyingString => IsBuying ? "BUY" : "SELL";
        [NotMapped]
        public string IsBuyingStringColor => IsBuying ? "#cc0000" : "#8c8c8c";
        public bool MarkedAsPaid { get; set; }
        [NotMapped]
        public string MarkedAsPaidStringColor => MarkedAsPaid ? "#00cc00" : "#595959";
        public bool IsMessageSent { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Contact)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public bool Equals(Contact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
    }
}