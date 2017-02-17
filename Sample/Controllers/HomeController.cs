using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessIpay()
        {

            //Initiate Payment Process by posting payment details to iPay
            //This will redirect user to iPay site.
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append(@"<body onload=""document.payform.submit()"">");
            sb.Append(@"<h3 style=""text-align:center"">Please wait while redirecting to our payment page.</h3>");
            sb.Append(@"<form name=""payform"" method=""post"" action=""http://138.128.147.229:93/Payment/"">");
            sb.Append(@"<input type=""hidden"" name=""orderno"" value=""1234"" />"); //Your Order No
            sb.Append(@"<input type=""hidden"" name=""merchantid"" value=""939A8CC6-CDA2-47F7-A25A-A1CCF0F45721"" />"); //Your Merchant ID
            sb.Append(@"<input type=""hidden"" name=""amount"" value=""10.00"" />");
            sb.Append(@"<input type=""hidden"" name=""description"" value=""Payment For Something"" />");
            sb.Append(string.Format(@"<input type=""hidden"" name=""returnurl"" value=""{0}"" />", Url.Action("Success", "Home", null, Request.Url.Scheme)));
            sb.Append(@"<input type=""hidden"" name=""currencycode"" value=""NPR"" />");
            sb.Append(string.Format(@"<input type=""hidden"" name=""errorurl"" value=""{0}"" />", Url.Action("Error", "Home", null, Request.Url.Scheme)));
            sb.Append(string.Format(@"<input type=""hidden"" name=""cancelurl"" value=""{0}"" />", Url.Action("Index", "Home", null, Request.Url.Scheme)));
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");

            Response.Write(sb.ToString());
            return null;
        }

        [HttpPost]
        //You will get ordernumber, transactionid and confirmation_code from iPay in POST Variable
        public void Success(string ordernumber, int transactionid, string confirmation_code)
        {
            //verify transaction
            //call verification url
            var webRequest = CommonHelper.MakeRequest("http://138.128.147.229:93/Payment/VerifyTransaction", new Dictionary<string, object>
            {
                {"transactionid", transactionid},
                {"confirmation_code", confirmation_code},
                {"merchantid", "939A8CC6-CDA2-47F7-A25A-A1CCF0F45721"} //Your MerchantID
            });

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(webResponse.GetResponseStream());
                string data = reader.ReadToEnd();
                string[] d = data.Split('|');

                //if success response and amount is ok
                if (d[0] == "Sucess" && Convert.ToDecimal(d[3]) == 10)
                {
                    //complete your order here...
                    //use ordernumber which is posted by iPay to track your order

                    //redirect user to complete page.
                    Response.Redirect(Url.Action("ThankYou", "Home"));
                    Response.End();
                    return;
                }
            }

            //Something wrong happens
            Response.Redirect(Url.Action("Error", "Home"));
        }

        public void Error()
        {
            Response.Write("An Error Occur while processing your request. Please try again.");
        }

        public void ThankYou()
        {
            Response.Write("<h1>Thank You!</h1>");
        }
    }
}
