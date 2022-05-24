using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BBS
{
    public enum SapInterfaceID
    {
        PP0060,
        PP0100,
        PP0320,
        PP0370
    }
    public static class MyStatic
	{
		// 재고정보 조회
        public static string PP0370_Uri_QAS = @"http://infvpidb01.kolon.com:50000/XISOAPAdapter/MessageServlet?senderParty=&senderService=INF_ESP_QAS&receiverParty=&receiverService=&interface=SI_GRP_PP0370_SO&interfaceNamespace=http://grpeccpp.esp.com/infesp";
        // 생산실적
        public static string PP0320_Uri_QAS = @"http://infvpidb01.kolon.com:50000/XISOAPAdapter/MessageServlet?senderParty=&senderService=INF_ESP_QAS&receiverParty=&receiverService=&interface=SI_GRP_PP0320_SO&interfaceNamespace=http://grpeccpp.esp.com/infesp";
        // 생산투입 
        public static string PP0100_Uri_QAS = @"http://infvpidb01.kolon.com:50000/XISOAPAdapter/MessageServlet?senderParty=&senderService=INF_ESP_QAS&receiverParty=&receiverService=&interface=SI_GRP_PP0100_SO&interfaceNamespace=http://grpeccpp.esp.com/infesp";
        // 기간오픈정보
        public static string PP0060_Uri_QAS = @"http://infvpidb01.kolon.com:50000/XISOAPAdapter/MessageServlet?senderParty=&senderService=INF_ESP_QAS&receiverParty=&receiverService=&interface=SI_GRP_PP0060_SO&interfaceNamespace=http://grpeccpp.esp.com/infesp";

        //public static string DB_EndpointAddress = "http://20.227.136.125:9099/DBServiceHttp";
//        public static string DB_EndpointAddress = "http://172.20.105.36:9090/DBServiceHttp";


        //public static XmlDocument PP0370_REQ(SapReqBody_PP0370 sapReqBody)
        //{

        //    Dictionary<string, string> headerDic = GetHeaderDic(SapInterfaceID.PP0370);


        //    StringBuilder sb = new StringBuilder();
        //    StringWriter strw = new StringWriter(sb);

        //    using (XmlTextWriter w = new XmlTextWriter(strw))
        //    {
        //        w.Formatting = Formatting.Indented;

        //        w.WriteStartElement("soapenv", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        //        w.WriteAttributeString("xmlns", "inf", null, "http://grpeccpp.esp.com/infesp");

        //        w.WriteStartElement("soapenv", "Header", null);
        //        w.WriteEndElement(); // End Of soapenv:Header

        //        w.WriteStartElement("soapenv", "Body", null);
        //        w.WriteStartElement("inf", "MT_GRP_PP0370_Con", null);

        //        // 공통부분
        //        w.WriteStartElement("Header");
        //        {
        //            foreach (KeyValuePair<string, string> di in headerDic)
        //            {
        //                w.WriteElementString(di.Key, di.Value);
        //            }

        //        }
        //        w.WriteEndElement(); // End Of Header
        //        // 공통부분 끝


        //        w.WriteStartElement("Body");
        //        {
        //            List<string> lstWerks = sapReqBody.T_WERKS;

        //            foreach (var werks in lstWerks)
        //            {
        //                w.WriteStartElement("T_WERKS");
        //                w.WriteElementString("WERKS", werks);
        //                w.WriteEndElement();
        //            }

        //            List<string> lstMatnr = sapReqBody.T_MATNR;
        //            foreach (var matnr in lstMatnr)
        //            {
        //                w.WriteStartElement("T_MATNR");
        //                w.WriteElementString("MATNR", matnr);
        //                w.WriteEndElement();
        //            }
        //            List<string> lstLgort = sapReqBody.T_LGORT;
        //            foreach (var lgort in lstLgort)
        //            {
        //                w.WriteStartElement("T_LGORT");
        //                w.WriteElementString("LGORT", lgort);
        //                w.WriteEndElement();
        //            }
        //        }

        //        w.WriteEndElement(); // End Of Body


        //        w.WriteEndElement(); // End Of inf:MT_GRP_PP0370_Con
        //        w.WriteEndElement(); // End Of soapenv:Body
        //        w.WriteEndElement(); // End Of First Start
        //        w.Close();

        //    }

        //    //Console.WriteLine(strw.ToString());

        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(sb.ToString());
        //    return xmlDoc;
        //}
        //private static Dictionary<string, string> GetHeaderDic(SapInterfaceID interfaceID )
        //{
        //    Dictionary<string, string> headerDic = new Dictionary<string, string>();
        //    if (interfaceID == SapInterfaceID.PP0060)
        //    {
        //        headerDic.Add("zInterfaceId", "GRP_PP0060");
        //    }
        //    else if (interfaceID == SapInterfaceID.PP0100)
        //    {
        //        headerDic.Add("zInterfaceId", "GRP_PP0100");
        //    }
        //    else if (interfaceID == SapInterfaceID.PP0320)
        //    {
        //        headerDic.Add("zInterfaceId", "GRP_PP0320");
        //    }
        //    else if (interfaceID == SapInterfaceID.PP0370)
        //    {
        //        headerDic.Add("zInterfaceId", "GRP_PP0370");

        //    }
        //    headerDic.Add("zConSysId", "KII_CHA");
        //    headerDic.Add("zProSysId", "GRP_ECC_PP");
        //    headerDic.Add("zUserId", "bbs");
        //    headerDic.Add("zPiUser", "IF_KIICHA");
        //    headerDic.Add("zTimeId", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        //    headerDic.Add("zLang", "");

        //    return headerDic;
        //}


        public static XmlDocument GetDataDB_HttpReq(ReqCommand reqCmd)
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


    }

    public class XmlUtil
    {
        #region deserialization
        // deserialization
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }
        // deserialization
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region serialization
        // serialization
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //Serialized object
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }
        #endregion
    }


}
