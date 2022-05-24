using BBS;
using MySoap.Models;
using MySoap.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MySoap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test_Item : ContentPage
    {
        public Test_Item()
        {
            InitializeComponent();
            //btnQuery.Clicked += BtnQuery_Clicked;
            btnQuery.Clicked += BtnQuery_Clicked1;
            btnSave.Clicked += BtnSave_Clicked;

        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            List<ReqCommand> reqCmds = new List<ReqCommand>();
            //                ReqCommand cmd1 = T1_MST();
            ReqCommand reqHDR = T1_MST();
            ReqCommand reqDTL = T1_MST2_DTL();
            reqCmds.AddRange(new ReqCommand[] { reqHDR, reqDTL });

            try
            {
                TestItemService itemService = new TestItemService(DBAction.ExecNonQuery);
                SvcResult svcResult = itemService.AddTestItem(reqCmds);

                Console.WriteLine(svcResult.ReturnStr);
                //lstv1.ItemsSource = testItemMsts;

            }
            catch (Exception ex)
            {
                this.DisplayAlert("Error", ex.ToString(), "Confirm");
                return;

            }

        }
        private ReqCommand T1_MST()
        {
            ReqCommand _cmd = new ReqCommand("MST", "AZURE_PC", CommandType.StoredProcedure, "NakDongDB..USP_TEST_MST");
            _cmd.Parameters.Add(new ReqPara("@TEST_MST_NM", SqlDbType.NVarChar, ParameterDirection.Input));
            _cmd.Parameters.Add(new ReqPara("@TEST_ID", SqlDbType.BigInt, ParameterDirection.Output));

            Dictionary<string, object> pairValue = new Dictionary<string, object>(){
                        {"@TEST_MST_NM", "홍길동1"},
                        {"@TEST_ID", ""}
                };
            _cmd.ParameterValues.Add(pairValue);

            return _cmd;
        }
        private ReqCommand T1_MST2_DTL()
        {
            ReqCommand _cmd = new ReqCommand("DTL", "AZURE_PC", CommandType.StoredProcedure, "NakDongDB..USP_TEST_DTL");
            _cmd.Parameters.Add(new ReqPara("@TEST_ID", SqlDbType.BigInt, ParameterDirection.Input, "MST", "@TEST_ID"));
            _cmd.Parameters.Add(new ReqPara("@TEST_DTL_NM", SqlDbType.NVarChar, ParameterDirection.Input));
            _cmd.Parameters.Add(new ReqPara("@AMOUNT", SqlDbType.Decimal, ParameterDirection.Input));

            Dictionary<string, object> pairValue = new Dictionary<string, object>();

            pairValue = new Dictionary<string, object>() {
                        {"@TEST_ID", null },
                        {"@TEST_DTL_NM", "흥부2"},
                        {"@AMOUNT", 123.2 }

                };
            _cmd.ParameterValues.Add(pairValue);

            pairValue = new Dictionary<string, object>() {
                        {"@TEST_ID", null},
                        {"@TEST_DTL_NM", "놀부"},
                        {"@AMOUNT", 123.3 }
                };
            _cmd.ParameterValues.Add(pairValue);
            pairValue = new Dictionary<string, object>() {
                        {"@TEST_ID", null},
                        {"@TEST_DTL_NM", "안녕하세요"},
                        {"@AMOUNT", 123.4 }
                };
            _cmd.ParameterValues.Add(pairValue);

            return _cmd;
        }

        private void BtnQuery_Clicked1(object sender, EventArgs e)
        {
            ReqCommand _cmd = new ReqCommand()
            {
                CommandName = "MST",
                ConnectionName = "AZURE_PC",
                CommandType = CommandType.StoredProcedure,
                CommandText = "NakDongDB..[USP_TEST_MST_SEL]",
                Parameters = new List<ReqPara>(),
                ParameterValues = new List<Dictionary<string, object>>()
            };

            _cmd.Parameters.Add(new ReqPara("@TEST_MST_NM", SqlDbType.VarChar, ParameterDirection.Input));
            _cmd.Parameters.Add(new ReqPara("@TEST_PARA", SqlDbType.VarChar, ParameterDirection.Input));

            Dictionary<string, object> pairValue = new Dictionary<string, object>()
                {
                    {"@TEST_MST_NM","%"},
                    {"@TEST_PARA","%"}

                };
            _cmd.ParameterValues.Add(pairValue);

            try
            {
                TestItemService itemService = new TestItemService(DBAction.GetDataSet);
                List<TestItemMst> testItemMsts=  itemService.GetTestItemMst(_cmd);

                lstv1.ItemsSource = testItemMsts;

            }
            catch (Exception ex)
            {
                this.DisplayAlert("Error", ex.ToString(), "Confirm");
                return;
                
            }
        }

        //private void BtnQuery_Clicked(object sender, EventArgs e)
        //{
        //    ReqCommand _cmd = new ReqCommand()
        //    {
        //        CommandName = "MST",
        //        ConnectionName = "AZURE_PC",
        //        CommandType = CommandType.StoredProcedure,
        //        CommandText = "NakDongDB..[USP_TEST_MST_SEL]",
        //        Parameters = new List<ReqPara>(),
        //        ParameterValues = new List<Dictionary<string, object>>()
        //    };
        //    //ReqCommand _cmd = new ReqCommand()
        //    //{
        //    //    CommandName = "MST",
        //    //    ConnectionName = "BSBAE",
        //    //    CommandType = CommandType.StoredProcedure,
        //    //    CommandText = "TESTDB..[USP_TEST_MST_SEL]",
        //    //    Parameters = new List<ReqPara>(),
        //    //    ParameterValues = new List<Dictionary<string, object>>()
        //    //};
        //    _cmd.Parameters.Add(new ReqPara("@TEST_MST_NM", SqlDbType.VarChar, ParameterDirection.Input));
        //    _cmd.Parameters.Add(new ReqPara("@TEST_PARA", SqlDbType.VarChar, ParameterDirection.Input));

        //    Dictionary<string, object> pairValue = new Dictionary<string, object>()
        //        {
        //            {"@TEST_MST_NM","%"},
        //            {"@TEST_PARA","%"}

        //        };
        //    _cmd.ParameterValues.Add(pairValue);
        //    try
        //    {
        //        // 요청 파라미터 
        //        XmlDocument reqXmlDoc = GetDataDB_HttpReq(_cmd);

        //        MyHttpDB myHttp = new MyHttpDB(DBAction.GetDataSet);
        //        SvcResult resRtn = myHttp.GetResponse(reqXmlDoc);

        //        var doc = XDocument.Parse(resRtn.ReturnStr);

        //        List<TestSel> lstSel = new List<TestSel>();

        //        lstSel = (from r in doc.Root.Elements("Table")
        //                  select new TestSel()
        //                  {
        //                      TEST_ID = r.Element("TEST_ID").Value,
        //                      TEST_MST_NM = r.Element("TEST_MST_NM").Value,
        //                      CREATION_DATE = Convert.ToDateTime(r.Element("CREATION_DATE").Value)

        //                  }).ToList();
        //        //TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local);

        //        lstv1.ItemsSource = lstSel;


        //        //foreach (var item in doc.Root.Elements("Table"))
        //        //{
        //        //    Debug.WriteLine(item.Element("TEST_ID").Value);
        //        //    Debug.WriteLine(item.Element("TEST_MST_NM").Value);
        //        //    Debug.WriteLine(item.Element("CREATION_DATE").Value);
        //        //}


        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.DisplayAlert("Error", ex.ToString(), "Confirm");
        //        return;
        //        //throw;
        //    }
        //}

        //private XmlDocument GetDataDB_HttpReq(ReqCommand reqCmd)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.OmitXmlDeclaration = true;  // 필수

        //    XmlWriter w = XmlWriter.Create(sb, settings);


        //    w.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        //    w.WriteAttributeString("xmlns", "nak", null, "http://nakdong.wcf.service");
        //    w.WriteAttributeString("xmlns", "bbs", null, "http://schemas.datacontract.org/2004/07/BBS");

        //    w.WriteStartElement("soap", "Header", null);
        //    w.WriteEndElement(); // End Of soapenv:Header

        //    w.WriteStartElement("soap", "Body", null);

        //    // 변경해줘야 할 부분 
        //    w.WriteStartElement("nak", "GetDataSetXml", null);

        //    // 공통부분 Strart command
        //    w.WriteStartElement("nak", "cmd", null);


        //    {
        //        w.WriteElementString("bbs", "CommandName", null, reqCmd.CommandName);
        //        w.WriteElementString("bbs", "ConnectionName", null, reqCmd.ConnectionName);
        //        w.WriteElementString("bbs", "CommandType", null, Convert.ToInt32(reqCmd.CommandType).ToString());
        //        w.WriteElementString("bbs", "CommandText", null, reqCmd.CommandText);

        //        // parameter
        //        w.WriteStartElement("bbs", "Parameters", null);
        //        foreach (var para in reqCmd.Parameters)
        //        {
        //            w.WriteStartElement("bbs", "MyPara", null);
        //            w.WriteElementString("bbs", "ParameterName", null, para.ParameterName);
        //            w.WriteElementString("bbs", "DbDataType", null, para.DbDataType.ToString());
        //            w.WriteElementString("bbs", "Direction", null, Convert.ToInt32(para.Direction).ToString());
        //            w.WriteEndElement(); // Parameters
        //        }
        //        w.WriteEndElement(); // Parameters

        //        // parameter value : ArrayOfArray
        //        w.WriteStartElement("bbs", "ParaValues", null);
        //        foreach (Dictionary<string, object> paraDic in reqCmd.ParameterValues)
        //        {
        //            w.WriteStartElement("bbs", "ArrayOfMyParaValue", null);

        //            foreach (KeyValuePair<string, object> paraPair in paraDic)
        //            {
        //                w.WriteStartElement("bbs", "MyParaValue", null);
        //                w.WriteElementString("bbs", "ParameterName", null, paraPair.Key);
        //                w.WriteElementString("bbs", "ParaValue", null, paraPair.Value.ToString());
        //                w.WriteEndElement();  // MyParaValue

        //                Console.WriteLine("Key:{0} Value: {1}", paraPair.Key, paraPair.Value);
        //            }
        //            w.WriteEndElement(); // ArrayOfMyParaValue
        //        }
        //        w.WriteEndElement(); // ParaValues

        //    }

        //    w.WriteEndElement(); // End Of Command


        //    w.WriteEndElement(); // End Of 사용자 GetDataSetXml
        //    w.WriteEndElement(); // End Of soapenv:Body
        //    w.WriteEndElement(); // End Of First Start
        //    w.Close();



        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(sb.ToString());
        //    return xmlDoc;

        //}

    }
    //[System.Xml.Serialization.XmlRoot("Table")]
    //public class TestSel
    //{
    //    public string TEST_ID { get; set; }
    //    public string TEST_MST_NM { get; set; }
    //    public DateTime CREATION_DATE { get; set; }
    //}

}