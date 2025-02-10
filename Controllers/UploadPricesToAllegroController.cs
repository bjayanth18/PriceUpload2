using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ZemaPriceUpload.Controllers
{
    [EnableCors("AllowAll")] // Enables CORS for this route
    [ApiController]
    [Route("UploadPricesToAllegro")]
    public class UploadPricesToAllegroController : ControllerBase
    {
        private readonly ILogger<UploadPricesToAllegroController> _logger;
        private readonly IConfiguration _configuration;

        public UploadPricesToAllegroController(IConfiguration configuration, ILogger<UploadPricesToAllegroController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Authorize] //Authorize using Okta
        [HttpPost(Name = "PostUploadPricesToAllegro")]
        public async Task<string> Post([FromBody] ZemaPrice[] prices)
        {
            string priceindex = string.Empty;
            try
            {
                priceindex = prices[0].priceindex;
                string filename = prices[0].priceindex + "_inputfromzema_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                System.IO.File.WriteAllText("requestlog/" + filename + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(prices));
            }
            catch (Exception) { }

            return SerializeAndPostToAllegro(prices, priceindex);
        }

        private string SerializeAndPostToAllegro(ZemaPrice[] prices, string priceindex)
        {
            string resultString;
            try
            {
                string pricesXml = SerializeToXml(prices);
                resultString = PostToAllegro(GetSoapXml(pricesXml), priceindex);
            }
            catch (Exception ex)
            {
                resultString = "Exception occured while uploading prices to Allegro. " + Environment.NewLine + "Message:" + ex.Message;
            }

            return resultString;
        }

        private string SerializeToXml(ZemaPrice[] prices)
        {
            //Convert prices to XML format
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ZemaPrice[]));
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            string pricesXml = string.Empty;

            using (var strWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(strWriter, settings))
                {
                    xsSubmit.Serialize(xmlWriter, prices, emptyNamespaces);
                    pricesXml = strWriter.ToString();
                }
            }

            return pricesXml;
        }

        private string PostToAllegro(string pricesXml, string priceindex)
        {
            string url = _configuration["HorizonPriceWS"] ?? string.Empty;
            string soapAction = _configuration["HttpSoapAction"] ?? string.Empty;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(soapAction))
                return "Missing URL and SOAP Action configuration";

            //Create HttpWebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add(soapAction);
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.UseDefaultCredentials = true;
            request.Timeout = 1200000;

            //Add PricesXml to the stream
            using (Stream stream = request.GetRequestStream())
            {
                XmlDocument reqBody = new XmlDocument();
                reqBody.LoadXml(pricesXml);
                reqBody.Save(stream);
            }

            string resultString = string.Empty;
            //Invoke the Allegro web service
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    resultString = String.Format("POST failed with HTTP {0}", response.StatusCode);
                }
                else
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        //reading stream
                        var ServiceResult = rd.ReadToEnd();
                        //writing stream result on console
                        resultString = ServiceResult;
                    }
                }

                try
                {
                    string filename = priceindex + "_outputfromallegro_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    System.IO.File.WriteAllText("responselog/" + filename + ".xml", resultString);
                }
                catch (Exception ex) { }
            }

            return resultString;
        }

        private string GetSoapXml(string pricesXml)
        {

            //Convert XML to the SOAP Request format
            pricesXml = pricesXml.Replace("ArrayOfZemaPrice", "prices");
            pricesXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                       <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                         <soap:Body>
                                           <UploadPrices xmlns=""http://allegrodevelopment.com/"">
                                            " + pricesXml + @"
                                            </UploadPrices>
                                         </soap:Body>
                                        </soap:Envelope>";
            return pricesXml;
        }
    }
}
