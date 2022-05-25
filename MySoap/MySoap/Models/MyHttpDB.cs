using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace BBS
{
    public enum DBAction
    {
        ExecNonQuery,
        GetDataSet
    }

    public class MyHttpDB
    {
        // WCF End Point Address In Azure Virtual Machine 
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
        public SvcReturn GetResponse( XmlDocument reqXmlDoc)
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
                SvcReturn svc = new SvcReturn();
                svc.ReturnCD = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnCD").Value;
                svc.ReturnMsg = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnMsg").Value;
                svc.ReturnStr = soapResult.Element("{http://schemas.datacontract.org/2004/07/BBS}ReturnStr").Value;

                return svc;
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

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;

            }

        }
        public XmlDocument GetDataDB_HttpReq(ReqCommand reqCmd)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;  // 필수

            XmlWriter w = XmlWriter.Create(sb, settings);

            w.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            w.WriteAttributeString("xmlns", "nak", null, "http://nakdong.wcf.service");
            w.WriteAttributeString("xmlns", "bbs", null, "http://schemas.datacontract.org/2004/07/BBS");

            w.WriteStartElement("soap", "Header", null);
            w.WriteEndElement(); // End Of soapenv:Header

            w.WriteStartElement("soap", "Body", null);

            // 변경해줘야 할 부분 
            w.WriteStartElement("nak", "GetDataSetXml", null);

            // 공통부분 Strart command
            w.WriteStartElement("nak", "cmd", null);


            {
                w.WriteElementString("bbs", "CommandName", null, reqCmd.CommandName);
                w.WriteElementString("bbs", "ConnectionName", null, reqCmd.ConnectionName);
                w.WriteElementString("bbs", "CommandType", null, Convert.ToInt32(reqCmd.CommandType).ToString());
                w.WriteElementString("bbs", "CommandText", null, reqCmd.CommandText);

                // parameter
                w.WriteStartElement("bbs", "Parameters", null);
                foreach (var para in reqCmd.Parameters)
                {
                    w.WriteStartElement("bbs", "MyPara", null);
                    w.WriteElementString("bbs", "ParameterName", null, para.ParameterName);
                    w.WriteElementString("bbs", "DbDataType", null, para.DbDataType.ToString());
                    w.WriteElementString("bbs", "Direction", null, Convert.ToInt32(para.Direction).ToString());
                    w.WriteEndElement(); // Parameters
                }
                w.WriteEndElement(); // Parameters

                // parameter value : ArrayOfArray
                w.WriteStartElement("bbs", "ParaValues", null);
                foreach (Dictionary<string, object> paraDic in reqCmd.ParameterValues)
                {
                    w.WriteStartElement("bbs", "ArrayOfMyParaValue", null);

                    foreach (KeyValuePair<string, object> paraPair in paraDic)
                    {
                        w.WriteStartElement("bbs", "MyParaValue", null);
                        w.WriteElementString("bbs", "ParameterName", null, paraPair.Key);
                        w.WriteElementString("bbs", "ParaValue", null, paraPair.Value.ToString());
                        w.WriteEndElement();  // MyParaValue

                        Console.WriteLine("Key:{0} Value: {1}", paraPair.Key, paraPair.Value);
                    }
                    w.WriteEndElement(); // ArrayOfMyParaValue
                }
                w.WriteEndElement(); // ParaValues

            }

            w.WriteEndElement(); // End Of Command


            w.WriteEndElement(); // End Of 사용자 GetDataSetXml
            w.WriteEndElement(); // End Of soapenv:Body
            w.WriteEndElement(); // End Of First Start
            w.Close();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sb.ToString());
            return xmlDoc;

        }

        public XmlDocument NonExecQueryHttpReq(List<ReqCommand> lstReqCmd)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;  // 필수

            XmlWriter w = XmlWriter.Create(sb, settings);


            w.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            w.WriteAttributeString("xmlns", "nak", null, "http://nakdong.wcf.service");
            w.WriteAttributeString("xmlns", "bbs", null, "http://schemas.datacontract.org/2004/07/BBS");

            w.WriteStartElement("soap", "Header", null);
            w.WriteEndElement(); // End Of soapenv:Header

            w.WriteStartElement("soap", "Body", null);

            // 변경해줘야 할 부분 
            w.WriteStartElement("nak", "ExecNonQuery", null);

            // 공통부분 Strart command
            w.WriteStartElement("nak", "cmd", null);

            foreach (var reqCmd in lstReqCmd)
            {
                w.WriteStartElement("bbs", "MyCommand", null);
                {
                    w.WriteElementString("bbs", "CommandName", null, reqCmd.CommandName);
                    w.WriteElementString("bbs", "ConnectionName", null, reqCmd.ConnectionName);
                    w.WriteElementString("bbs", "CommandType", null, Convert.ToInt32(reqCmd.CommandType).ToString());
                    w.WriteElementString("bbs", "CommandText", null, reqCmd.CommandText);

                    // parameter
                    w.WriteStartElement("bbs", "Parameters", null);
                    foreach (var para in reqCmd.Parameters)
                    {
                        w.WriteStartElement("bbs", "MyPara", null);

                        w.WriteElementString("bbs", "ParameterName", null, para.ParameterName);
                        w.WriteElementString("bbs", "DbDataType", null, para.DbDataType.ToString());
                        w.WriteElementString("bbs", "Direction", null, Convert.ToInt32(para.Direction).ToString());
                        w.WriteElementString("bbs", "HeaderCommandName", null, para.HeaderCommandName);
                        w.WriteElementString("bbs", "HeaderParameter", null, para.HeaderParameter);

                        w.WriteEndElement(); // Parameters
                    }
                    w.WriteEndElement(); // Parameters

                    // parameter value : ArrayOfArray
                    w.WriteStartElement("bbs", "ParaValues", null);
                    foreach (Dictionary<string, object> paraDic in reqCmd.ParameterValues)
                    {
                        w.WriteStartElement("bbs", "ArrayOfMyParaValue", null);

                        foreach (KeyValuePair<string, object> paraPair in paraDic)
                        {
                            w.WriteStartElement("bbs", "MyParaValue", null);
                            w.WriteElementString("bbs", "ParameterName", null, paraPair.Key);
                            if (paraPair.Value == null)
                                w.WriteElementString("bbs", "ParaValue", null, String.Empty);
                            else
                                w.WriteElementString("bbs", "ParaValue", null, paraPair.Value.ToString());

                            w.WriteEndElement();  // MyParaValue

                            Console.WriteLine("Key:{0} Value: {1}", paraPair.Key, paraPair.Value);
                        }
                        w.WriteEndElement(); // ArrayOfMyParaValue
                    }
                    w.WriteEndElement(); // ParaValues
                }

                w.WriteEndElement(); // End Of bbs:MyCommand
            }
            w.WriteEndElement(); // End Of nak:cmd



            w.WriteEndElement(); // End Of 사용자 ExecNonQuery
            w.WriteEndElement(); // End Of soapenv:Body
            w.WriteEndElement(); // End Of First Start
            w.Close();



            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sb.ToString());
            return xmlDoc;

        }

        public XmlDocument GetDataDB_HttpReq_MyCmd(MyCommand myCmd)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;  // 필수

            XmlWriter w = XmlWriter.Create(sb, settings);

            w.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            w.WriteAttributeString("xmlns", "nak", null, "http://nakdong.wcf.service");
            w.WriteAttributeString("xmlns", "bbs", null, "http://schemas.datacontract.org/2004/07/BBS");

            w.WriteStartElement("soap", "Header", null);
            w.WriteEndElement(); // End Of soapenv:Header

            w.WriteStartElement("soap", "Body", null);

            // 변경해줘야 할 부분 
            w.WriteStartElement("nak", "GetDataSetXml", null);

            // 공통부분 Strart command
            w.WriteStartElement("nak", "cmd", null);
            {
                w.WriteElementString("bbs", "CommandName", null, myCmd.CommandName);
                w.WriteElementString("bbs", "ConnectionName", null, myCmd.ConnectionName);
                w.WriteElementString("bbs", "CommandType", null, Convert.ToInt32(myCmd.CommandType).ToString());
                w.WriteElementString("bbs", "CommandText", null, myCmd.CommandText);

                // parameter
                w.WriteStartElement("bbs", "Parameters", null);
                foreach (var para in myCmd.Parameters)
                {
                    w.WriteStartElement("bbs", "MyPara", null);
                    w.WriteElementString("bbs", "ParameterName", null, para.ParameterName);
                    w.WriteElementString("bbs", "DbDataType", null, para.DbDataType.ToString());
                    w.WriteElementString("bbs", "Direction", null, Convert.ToInt32(para.Direction).ToString());
                    w.WriteEndElement(); // Parameters
                }
                w.WriteEndElement(); // Parameters

                // parameter value : ArrayOfArray
                w.WriteStartElement("bbs", "ParaValues", null);
                foreach (var paraValueSet in myCmd.ParaValues)
                {
                    w.WriteStartElement("bbs", "ArrayOfMyParaValue", null);
                    foreach (var paraValue in paraValueSet)
                    {
                        w.WriteStartElement("bbs", "MyParaValue", null);
                        w.WriteElementString("bbs", "ParameterName", null, paraValue.ParameterName);
                        w.WriteElementString("bbs", "ParaValue", null, paraValue.ParaValue);
                        w.WriteEndElement();  // MyParaValue

                        Console.WriteLine("Key:{0} Value: {1}", paraValue.ParameterName, paraValue.ParaValue);
                    }
                        
                    w.WriteEndElement(); // ArrayOfMyParaValue
                }
                w.WriteEndElement(); // ParaValues

            }

            w.WriteEndElement(); // End Of Command


            w.WriteEndElement(); // End Of 사용자 GetDataSetXml
            w.WriteEndElement(); // End Of soapenv:Body
            w.WriteEndElement(); // End Of First Start
            w.Close();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sb.ToString());
            return xmlDoc;

        }
        public XmlDocument NonExecQueryHttpReq_MyCmd(List<MyCommand> lstMyCmd)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;  // 필수

            XmlWriter w = XmlWriter.Create(sb, settings);


            w.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            w.WriteAttributeString("xmlns", "nak", null, "http://nakdong.wcf.service");
            w.WriteAttributeString("xmlns", "bbs", null, "http://schemas.datacontract.org/2004/07/BBS");

            w.WriteStartElement("soap", "Header", null);
            w.WriteEndElement(); // End Of soapenv:Header

            w.WriteStartElement("soap", "Body", null);

            // 변경해줘야 할 부분 
            w.WriteStartElement("nak", "ExecNonQuery", null);

            // 공통부분 Strart command
            w.WriteStartElement("nak", "cmd", null);

            foreach (var myCmd in lstMyCmd)
            {
                w.WriteStartElement("bbs", "MyCommand", null);
                {
                    w.WriteElementString("bbs", "CommandName", null, myCmd.CommandName);
                    w.WriteElementString("bbs", "ConnectionName", null, myCmd.ConnectionName);
                    w.WriteElementString("bbs", "CommandType", null, Convert.ToInt32(myCmd.CommandType).ToString());
                    w.WriteElementString("bbs", "CommandText", null, myCmd.CommandText);

                    // parameter
                    w.WriteStartElement("bbs", "Parameters", null);
                    foreach (var para in myCmd.Parameters)
                    {
                        w.WriteStartElement("bbs", "MyPara", null);

                        w.WriteElementString("bbs", "ParameterName", null, para.ParameterName);
                        w.WriteElementString("bbs", "DbDataType", null, para.DbDataType.ToString());
                        w.WriteElementString("bbs", "Direction", null, Convert.ToInt32(para.Direction).ToString());
                        w.WriteElementString("bbs", "HeaderCommandName", null, para.HeaderCommandName);
                        w.WriteElementString("bbs", "HeaderParameter", null, para.HeaderParameter);

                        w.WriteEndElement(); // Parameters
                    }
                    w.WriteEndElement(); // Parameters

                    // parameter value : ArrayOfArray
                    w.WriteStartElement("bbs", "ParaValues", null);
                    foreach (var paraValueSet in myCmd.ParaValues)
                    {
                        w.WriteStartElement("bbs", "ArrayOfMyParaValue", null);
                        foreach (var paraValue in paraValueSet)
                        {
                            w.WriteStartElement("bbs", "MyParaValue", null);
                            w.WriteElementString("bbs", "ParameterName", null, paraValue.ParameterName);
                            w.WriteElementString("bbs", "ParaValue", null, paraValue.ParaValue);
                            w.WriteEndElement();  // MyParaValue

                            Console.WriteLine("Key:{0} Value: {1}", paraValue.ParameterName, paraValue.ParaValue);
                        }

                        w.WriteEndElement(); // ArrayOfMyParaValue
                    }
                   
                    w.WriteEndElement(); // ParaValues
                }

                w.WriteEndElement(); // End Of bbs:MyCommand
            }
            w.WriteEndElement(); // End Of nak:cmd



            w.WriteEndElement(); // End Of 사용자 ExecNonQuery
            w.WriteEndElement(); // End Of soapenv:Body
            w.WriteEndElement(); // End Of First Start
            w.Close();



            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sb.ToString());
            return xmlDoc;

        }

    }

}