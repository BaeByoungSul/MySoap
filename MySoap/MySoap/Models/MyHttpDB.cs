using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace BBS
{
    public enum DBAction
    {
        ExecNonQuery,
        GetDataSet
    }

    public enum MyEndpointName
    {
        BasicHttpBinding_IDBService,
        WSHttpBinding_IDBServiceWs
    }


    public class ReqResult
    {
        public string ReturnCD { get; set; }
        public string ReturnMsg { get; set; }
        public DataSet Ds { get; set; }
    }
    public class SvcResult
    {
        public string ReturnCD { get; set; }
        public string ReturnMsg { get; set; }
        public string ReturnStr { get; set; }
    }
    public class MyHttpDB
    {
        //private readonly string pEndpointConfigName = "BasicHttpBinding_IDBService";
        public readonly string DB_EndpointAddress = "http://20.227.136.125:9099/DBServiceHttp";

        private HttpWebRequest phttpReq { get; set; }
        private DBAction pEnumAction { get; set; }
        public MyHttpDB( DBAction action )
        {
            phttpReq = CreateHttpRequest(action);
            pEnumAction = action;
        }

        /// <summary>
        /// DB 서비스 요청 생성
        /// </summary>
        /// <param name="dbAction"></param>
        /// <returns></returns>
        private  HttpWebRequest CreateHttpRequest(DBAction dbAction)
        {

            //string sUri = MyStatic.DB_EndpointAddress;
            string sUri = DB_EndpointAddress;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sUri);

            webRequest.Method = "POST";
            webRequest.ProtocolVersion = HttpVersion.Version11;

            // 통신 데이터를 압축
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            webRequest.ContentType = "text/xml;charset=UTF-8";

            string sAction = string.Empty;
            if (dbAction == DBAction.ExecNonQuery)
            {
                sAction = "http://nakdong.wcf.service/IDBService/ExecNonQuery";
            }
            else if (dbAction == DBAction.GetDataSet)
            {
                sAction = "http://nakdong.wcf.service/IDBService/GetDataSetXml";
            }
            webRequest.Headers.Add("SOAPAction", sAction);

            // True이면: 하나의 연결에 다수의 request/response를 허용함
            // HTTP/1.1: 기본값 true
            webRequest.KeepAlive = true;

            // Connection Timeout : ? 초 & maximum # of request in a single connection
            // 서버설정에 따르는게 좋을 듯: Apache httpd(1.3 & 2.0) default 값은 15초
            // webRequest.Headers["Keep-Alive"] = "timeout=30, max=50";

            // True이면 : Header만 보내고 서버응답시 다시 데이터 보낸다.

            return webRequest;
        }
        public SvcResult GetResponse( XmlDocument reqXmlDoc)
        {
            try
            {
                //InsertSoapEnvelopeIntoWebRequest
                using (Stream stream = phttpReq.GetRequestStream())
                {
                    reqXmlDoc.Save(stream);
                }

                XDocument xDoc = new XDocument();
                using (WebResponse response = phttpReq.GetResponse())
                {
                    //using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    //{
                    //    xDoc = XDocument.Load(rd);
                    //}
                    using (Stream stream = response.GetResponseStream())
                    {
                        xDoc = XDocument.Load(stream);
                    }
                }

                string sNs = @"{http://nakdong.wcf.service}";

                XElement soapResult;
                if (pEnumAction == DBAction.GetDataSet)
                {
                    soapResult = xDoc.Root.Descendants(sNs + "GetDataSetXmlResponse").Elements(sNs + "GetDataSetXmlResult").FirstOrDefault();
                }
                else
                {
                    soapResult = xDoc.Root.Descendants(sNs + "ExecNonQueryResponse").Elements(sNs + "ExecNonQueryResult").FirstOrDefault();
                }
                SvcResult svc = new SvcResult();
                svc.ReturnCD = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnCD").Value;
                svc.ReturnMsg = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnMsg").Value;
                svc.ReturnStr = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnStr").Value;

                return svc;

                //return xDoc;
                //return HttpResponseParser(xDoc, T);

            }
            catch (WebException wex)
            {
                if (wex.Response == null) throw new Exception(wex.ToString());
                else if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string sErrorMsg = reader.ReadToEnd();
                            throw new Exception(sErrorMsg);
                        }
                    }
                }
                throw new Exception(wex.ToString());


                //string pageContent = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd().ToString();
                //throw new Exception(pageContent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;

            }

        }



        private ReqResult HttpResponseParser(XDocument xDoc)
        {

            string sNs = @"{http://nakdong.wcf.service}";

            XElement soapResult;
            if (pEnumAction == DBAction.GetDataSet )
            {
                soapResult = xDoc.Root.Descendants(sNs + "GetDataSetXmlResponse").Elements(sNs + "GetDataSetXmlResult").FirstOrDefault();
            }
            else
            {
                soapResult = xDoc.Root.Descendants(sNs + "ExecNonQueryResponse").Elements(sNs + "ExecNonQueryResult").FirstOrDefault();
            }

            //            XElement soapBody = xDoc.Descendants("{http://schemas.xmlsoap.org/soap/envelope/}Body").FirstOrDefault();
            //            XElement soapResponse = soapBody.Element("{http://nakdong.wcf.service}GetDataSetXmlResponse");
            //            XElement soapResult = soapResponse.Element("{http://nakdong.wcf.service}GetDataSetXmlResult");


            SvcResult svc = new SvcResult();
            svc.ReturnCD = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnCD").Value;
            svc.ReturnMsg = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnMsg").Value;
            svc.ReturnStr = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnStr").Value;

            StringReader theReader = new StringReader(svc.ReturnStr);
            DataSet ds = new DataSet();
            ds.ReadXml(theReader, XmlReadMode.ReadSchema);

            return new ReqResult
            {
                ReturnCD = svc.ReturnCD,
                ReturnMsg = svc.ReturnMsg,
                Ds = ds
            };
        }


    }

   

}