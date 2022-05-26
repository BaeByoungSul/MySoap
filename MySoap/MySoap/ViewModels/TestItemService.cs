using BBS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MySoapDB.ViewModels
{
    /// <summary>
    /// 검사항목 조회 및 저장
    /// </summary>
    public  class TestItemService
    {
        // http web request
        private MyHttpDB MyHttpWebReq { get; set; }

        // http web request 생성
        public TestItemService(DBAction dBAction )
        {
            MyHttpWebReq = new MyHttpDB (dBAction);
        }
    
        public List<TestItemMst>  GetTestItemMst( ReqCommand reqcommand)
        {
            try
            {
                XmlDocument reqXmlDoc = MyHttpWebReq.GetDataDB_HttpReq(reqcommand);
                SvcReturn resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

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

        public ExecReturn AddTestItem(List<ReqCommand> lstReqCmd)
        {
            try
            {
                //XmlDocument reqXmlDoc = NonExecQueryHttpReq(lstReqCmd);
                XmlDocument reqXmlDoc = MyHttpWebReq.NonExecQueryHttpReq(lstReqCmd);

                SvcReturn resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

                var doc = XDocument.Parse(resRtn.ReturnStr);
                List<DBOutPut> lstReturn = new List<DBOutPut>();

                lstReturn = (from r in doc.Root.Elements("output")
                             select new DBOutPut()
                             {
                                 Rowseq = Convert.ToInt32(r.Element("rowseq").Value),
                                 CommandName = r.Element("CommandName").Value,
                                 ParameterName = r.Element("ParameterName").Value,
                                 OutValue = r.Element("OutValue").Value,

                             }).ToList();
                ExecReturn execReturn = new ExecReturn()
                {
                    ReturnCD = resRtn.ReturnCD,
                    ReturnMsg= resRtn.ReturnMsg,    
                    ReturnOutPut= lstReturn
                };
                return execReturn;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<TestItemMst> GetTestItemMst_MyCmd(MyCommand mycmd)
        {
            try
            {
                XmlDocument reqXmlDoc = MyHttpWebReq.GetDataDB_HttpReq_MyCmd(mycmd);
                SvcReturn resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

                var doc = XDocument.Parse(resRtn.ReturnStr);
                List<TestItemMst> lstReturn = new List<TestItemMst>();

                lstReturn = (from r in doc.Root.Elements("Table")
                             select new TestItemMst()
                             {
                                 TEST_ID = Convert.ToInt32(r.Element("TEST_ID").Value),
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

        public ExecReturn AddTestItem_MyCmd(List<MyCommand> lstMyCmd)
        {
            try
            {
                //XmlDocument reqXmlDoc = NonExecQueryHttpReq(lstReqCmd);
                XmlDocument reqXmlDoc = MyHttpWebReq.NonExecQueryHttpReq_MyCmd(lstMyCmd);

                SvcReturn resRtn = MyHttpWebReq.GetResponse(reqXmlDoc);

                var doc = XDocument.Parse(resRtn.ReturnStr);
                List<DBOutPut> lstReturn = new List<DBOutPut>();

                lstReturn = (from r in doc.Root.Elements("output")
                             select new DBOutPut()
                             {
                                 Rowseq = Convert.ToInt32(r.Element("rowseq").Value),
                                 CommandName = r.Element("CommandName").Value,
                                 ParameterName = r.Element("ParameterName").Value,
                                 OutValue = r.Element("OutValue").Value,

                             }).ToList();
                ExecReturn execReturn = new ExecReturn()
                {
                    ReturnCD = resRtn.ReturnCD,
                    ReturnMsg = resRtn.ReturnMsg,
                    ReturnOutPut = lstReturn
                };
                return execReturn;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    /// <summary>
    /// 검사항목 Class
    /// </summary>
    public class TestItemMst
    {
        public int TEST_ID { get; set; }
        public string TEST_MST_NM { get; set; }
        public DateTime CREATION_DATE { get; set; }

    }

    /// <summary>
    /// db command execnonquery시 output parameter값을 저장
    /// </summary>
    public class ExecReturn
    {

        public string ReturnCD { get; set; }  // "FAIL", "OK"

        public string ReturnMsg { get; set; }

        public List<DBOutPut> ReturnOutPut { get; set; }
    }
}
