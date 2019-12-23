using LocalBitcoins;
using System;
using System.Collections.Generic;
using System.Text;

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
            List<Contact> result = new List<Contact>();
            Random rand = new Random();
            int dice = rand.Next(0, 10);
            Contact contact1 = new Contact();
            contact1.AmountRub = 888;
            contact1.Username = "stainlessBuyer";
            contact1.MarkedAsPaid = true;
            result.Add(contact1);
            if (dice < 7)
            {
                Contact contact = new Contact();
                contact.AmountRub = 1337;
                contact.Username = "pahanDrinksCola";
                contact.MarkedAsPaid = true;
                result.Add(contact);
            }
            if (dice < 5)
            {
                Contact contact = new Contact();
                contact.AmountRub = 2200;
                contact.Username = "2gBuds";
                contact.MarkedAsPaid = false;
                result.Add(contact);
            }
            if (dice < 3)
                return null;

            return result;
            //try
            //{
            //    List<Contact> result = new List<Contact>();
            //    var smth = client.GetDashboard().Result;
            //    IEnumerable<dynamic> list = smth.data.contact_list;
            //    foreach (dynamic x in list)
            //    {
            //        Contact contact = new Contact();
            //        contact.Id = x.data.contact_id;
            //        contact.CreatedAt = x.data.created_at;
            //        contact.AmountBtc = x.data.amount_btc;
            //        contact.AmountRub = x.data.amount;
            //        contact.IsBuying = x.data.is_buying;
            //        if (x.data.payment_completed_at != null)
            //        {
            //            contact.MarkedAsPaid = true;
            //        }
            //        result.Add(contact);
            //    }
            //    return result;
            //}
            //catch (Exception e)
            //{
            //    // logging 
            //    return null;
            //}


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
                string msg = resp.Result.data.message;
                // log resp
                return true;
            }
            catch (Exception e)
            {
                //log exception
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
    public class Contact
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string Username { get; set; }
        public string RefCode { get; set; }
        public decimal AmountRub { get; set; }
        public decimal AmountBtc { get; set; }
        public bool IsBuying { get; set; }
        public bool MarkedAsPaid { get; set; }
        public bool IsMessageSent { get; set; }
    }
}