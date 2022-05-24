using BBS;
using MySoap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MySoap.ViewModels
{
    public  class TestItemService
    {
        private MyHttpDB MyHttpWebReq { get; set; }
        public TestItemService(DBAction dBAction )
        {
            MyHttpWebReq = new MyHttpDB (dBAction);
        }
    
        public List<TestItemMst>  GetTestItemMst( ReqCommand reqcommand)
        {
            try
            {
                XmlDocument reqXmlDoc = GetDataDB_HttpReq(reqcommand);
                SvcResult resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

                // MyHttpDB myHttp = new MyHttpDB(DBAction.GetDataSet);
                //SvcResult resRtn = myHttp.GetResponse(reqXmlDoc);

                var doc = XDocument.Parse(resRtn.ReturnStr);
                List<TestItemMst> lstReturn = new List<TestItemMst>();

                lstReturn = (from r in doc.Root.Elements("Table")
                          select new TestItemMst()
                          {
                              TEST_ID = Convert.ToInt32( r.Element("TEST_ID").Value),
                              TEST_MST_NM = r.Element("TEST_MST_NM").Value,
                              CREATION_DATE = Convert.ToDateTime(r.Element("CREATION_DATE").Value)

                          }).ToList();
                return lstReturn;

            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }

        public SvcResult AddTestItem(List<ReqCommand> lstReqCmd)
        {
            try
            {
                XmlDocument reqXmlDoc = NonExecQueryHttpReq(lstReqCmd);
                SvcResult resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

                return resRtn;
                
                //var doc = XDocument.Parse(resRtn.ReturnStr);
                //List<SvcReturn> lstReturn = new List<SvcResult>();

                //lstReturn = (from r in doc.Root.Elements("Table")
                //             select new TestItemMst()
                //             {
                //                 TEST_ID = Convert.ToInt32(r.Element("TEST_ID").Value),
                //                 TEST_MST_NM = r.Element("TEST_MST_NM").Value,
                //                 CREATION_DATE = Convert.ToDateTime(r.Element("CREATION_DATE").Value)

                //             }).ToList();
                //return lstReturn;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private  XmlDocument GetDataDB_HttpReq(ReqCommand reqCmd)
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

        private XmlDocument NonExecQueryHttpReq(List<ReqCommand> lstReqCmd)
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

    }
}
