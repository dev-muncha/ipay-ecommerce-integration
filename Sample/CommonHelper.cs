using System.Collections.Generic;
using System.Text;

namespace Sample
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        public static System.Net.WebRequest MakeRequest(string url, Dictionary<string, object> data, bool addCookieContainer = false)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            if (addCookieContainer)
            {
                ((System.Net.HttpWebRequest)request).CookieContainer = new System.Net.CookieContainer();
            }

            StringBuilder parameter = new StringBuilder();
            foreach (var item in data)
            {
                parameter.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameter.ToString().Trim('&'));
            request.ContentLength = bytes.Length;

            using (System.IO.Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length); //Push data
                stream.Close();
            }

            return request;
        }
    }
}
