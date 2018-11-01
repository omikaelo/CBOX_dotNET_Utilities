using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Security;
using System.Net.Http.Headers;

namespace C_Box
{
    public class FazitTransaction
    {
        public string StatusCode
        {
            get; internal set;
        }

        public FazitTransaction()
        {
            StatusCode = "";
        }

        public bool LogIn(string username, string password, string uri, out string token)
        {
            HttpResponseMessage response;
            token = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback)((sender, cert, chain, sslPolicyErrors) => true);
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent($"{{\"username\":\"{username}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json");
                    response = client.PostAsync(uri, content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        StatusCode = response.StatusCode.ToString();
                        return false;
                    }
                    var data = response.Content.ReadAsStringAsync();
                    dynamic jsonData = JObject.Parse(data.Result);
                    token = jsonData["access_token"];
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool SendDataToFazit(string token, string jsonData, string uri)
        {
            HttpResponseMessage response;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    response = client.PostAsync(uri, content).Result;
                    StatusCode = response.StatusCode.ToString();
                    if (!response.IsSuccessStatusCode)
                        return false;
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
