using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoapClientManager
{
    public class Sample2
    {
       
        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public static string ExecuteSoapRequest(string url, string targetNameSpace, string soapAction, Dictionary<string, string> soapParams)
        {
            HttpWebRequest request = CreateWebRequest(url);

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(targetNameSpace, soapAction, soapParams);
                
            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    return soapResult;
                }
            }
        }

        private static XmlDocument CreateSoapEnvelope(string xmlns, string action, Dictionary<string,string> parameters)
        {
           // var soapParameters = CreateParameters(parameters);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.AppendLine(@"
                <Envelope xmlns=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <Body>");

            sb.AppendLine($@"<{action} xmlns=""{xmlns}"">");
            foreach (var item in parameters)
            {
                sb.Append($@"<{item.Key}>");
                sb.Append($"{item.Value}");
                sb.Append($@"</{item.Key}>");
                sb.AppendLine("");

            }
            sb.AppendLine($@"</{action}>");
            sb.AppendLine("</Body>");
            sb.AppendLine($@"</Envelope>");
            var soapEnv = sb.ToString();
            soapEnvelopeXml.LoadXml(soapEnv);
            return soapEnvelopeXml;
        }

        private static XmlDocument CreateParameters(Dictionary<string,string> soapParamters)
        {
            StringBuilder sb = new StringBuilder();
    
            foreach (var item in soapParamters)
            {
                sb.Append($@"<{item.Key}>");
                sb.Append($"{item.Value}");
                sb.Append($@"</{item.Key}>");
                sb.AppendLine("");

            }

            XmlDocument soapBodyParameters = new XmlDocument();
            soapBodyParameters.LoadXml(sb.ToString());
            sb.Clear();
            sb = null;
            return soapBodyParameters;
        }

        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        public static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

    }
}
