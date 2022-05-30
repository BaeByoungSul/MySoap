using BBS;
using MySoapDB.ViewModels;
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

namespace MySoapDB.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test_Item : ContentPage
    {
        public Test_Item()
        {
            InitializeComponent();

            this.BindingContext = this;

            btnQuery.Clicked += BtnQuery_Clicked1;
            btnSave.Clicked += BtnSave_Clicked;

            btnMyCmdQuery.Clicked += BtnMyCmdQuery_Clicked;
            btnMyCmdSave.Clicked += BtnMyCmdSave_Clicked;
        }

        private async void BtnMyCmdSave_Clicked(object sender, EventArgs e)
        {
            List<MyCommand> reqCmds = new List<MyCommand>();
            //                ReqCommand cmd1 = T1_MST();
            MyCommand reqHDR = ITEM_MST_Command();
            MyCommand reqDTL = ITEM_DTL_Command();
            reqCmds.AddRange(new MyCommand[] { reqHDR, reqDTL });

            try
            {
                
                cursorBusy.IsRunning = true;
                cursorBusy.IsEnabled = true;
                cursorBusy.IsVisible = true;                
                
                await Task.Delay(10);

                TestItemService itemService = new TestItemService(DBAction.ExecNonQuery);
                ExecReturn execReturn = itemService.AddTestItem_MyCmd(reqCmds);

                if (execReturn.ReturnCD.Equals("OK"))
                {
                   await this.DisplayAlert("Ok", "정상적으로 처리되었습니다.", "Confirm");
                }
                Console.WriteLine(execReturn.ReturnOutPut);
                //lstv1.ItemsSource = testItemMsts;

            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error", ex.ToString(), "Confirm");
                return;

            }finally
            {
                cursorBusy.IsRunning = false;
                cursorBusy.IsEnabled = false;
                cursorBusy.IsVisible = false;
            }
        }
        private MyCommand ITEM_MST_Command()
        {
            
            MyCommand _cmd = new MyCommand ( "MST", "AZURE_DB", 
                (int)CommandType.StoredProcedure, "NakDongDB..USP_TEST_MST_INS" );

            _cmd.Parameters = new MyPara[2];
            _cmd.Parameters[0] = new MyPara( "@TEST_MST_NM", (int)SqlDbType.NVarChar, (int)ParameterDirection.Input);
            _cmd.Parameters[1] = new MyPara( "@TEST_ID", (int)SqlDbType.BigInt, (int)ParameterDirection.Output );

            // Parameter Value 가변배열 초기화
            MyParaValue[][] myParaValues = new MyParaValue[1][]; // 한세트 정의
            myParaValues[0] = new MyParaValue[2];

            myParaValues[0][0] = new MyParaValue ( "@TEST_MST_NM", "배병술" );
            myParaValues[0][1] = new MyParaValue ( "@TEST_ID",   ""  );

            _cmd.ParaValues = myParaValues;

            return _cmd;
        }
        private MyCommand ITEM_DTL_Command()
        {
            MyCommand _cmd = new MyCommand("DTL", "AZURE_DB",
                            (int)CommandType.StoredProcedure, "NakDongDB..USP_TEST_DTL_INS");


            _cmd.Parameters = new MyPara[3];

            _cmd.Parameters[0] = new MyPara("@TEST_ID", (int)SqlDbType.BigInt, (int)ParameterDirection.Input, 
                "MST", "@TEST_ID");
            _cmd.Parameters[1] = new MyPara("@TEST_DTL_NM", (int)SqlDbType.NVarChar,(int) ParameterDirection.Input);
            _cmd.Parameters[2] = new MyPara("@AMOUNT", (int) SqlDbType.Decimal, (int) ParameterDirection.Input);



            // Parameter Value 가변배열 초기화
            MyParaValue[][] myParaValues = new MyParaValue[3][]; // 3세트 정의

            myParaValues[0] = new MyParaValue[3]; // 첫번째 셋트
            myParaValues[0][0] = new MyParaValue("@TEST_ID", null);
            myParaValues[0][1] = new MyParaValue("@TEST_DTL_NM", "놀부1");
            myParaValues[0][2] = new MyParaValue("@AMOUNT", "123.5");

            myParaValues[1] = new MyParaValue[3]; // 두번째 셋트
            myParaValues[1][0] = new MyParaValue("@TEST_ID", null);
            myParaValues[1][1] = new MyParaValue("@TEST_DTL_NM", "놀부2");
            myParaValues[1][2] = new MyParaValue("@AMOUNT", "222.5");

            myParaValues[2] = new MyParaValue[3]; // 세번째 셋트
            myParaValues[2][0] = new MyParaValue("@TEST_ID", null);
            myParaValues[2][1] = new MyParaValue("@TEST_DTL_NM", "흥부");
            myParaValues[2][2] = new MyParaValue("@AMOUNT", "333.5");

            _cmd.ParaValues = myParaValues;

            return _cmd;
        }


        private async void BtnMyCmdQuery_Clicked(object sender, EventArgs e)
        {
            MyCommand _cmd = new MyCommand("MST", "AZURE_DB",
                            (int)CommandType.StoredProcedure, "NakDongDB..USP_TEST_MST_SEL");



            _cmd.Parameters = new MyPara[2];
            _cmd.Parameters[0] = new MyPara("@TEST_MST_NM", (int)SqlDbType.NVarChar, (int)ParameterDirection.Input);
            _cmd.Parameters[1] = new MyPara("@TEST_PARA", (int)SqlDbType.NVarChar, (int)ParameterDirection.Input);
            
            //가변배열 초기화
            MyParaValue[][] myParaValues = new MyParaValue[1][];
            myParaValues[0] = new MyParaValue[2];

            myParaValues[0][0] = new MyParaValue ( "@TEST_MST_NM",  "%" );
            myParaValues[0][1] = new MyParaValue ( "@TEST_PARA",  "%" );

            _cmd.ParaValues = myParaValues;

            try
            {
                cursorBusy.IsRunning = true;
                cursorBusy.IsEnabled = true;
                cursorBusy.IsVisible = true;

                await Task.Delay(10);

                TestItemService itemService = new TestItemService(DBAction.GetDataSet);
                List<TestItemMst> testItemMsts = itemService.GetTestItemMst_MyCmd(_cmd);

                lstv1.ItemsSource = testItemMsts;

                
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Error", ex.ToString(), "Confirm");
                return;

            }
            finally
            {
                cursorBusy.IsRunning = false;
                cursorBusy.IsEnabled = false;
                cursorBusy.IsVisible = false;

                //IsTaskRunning = false;

            }
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
                ExecReturn execReturn = itemService.AddTestItem(reqCmds);

                if (execReturn.ReturnCD.Equals("OK"))
                {
                    this.DisplayAlert("Ok", "정상적으로 처리되었습니다.", "Confirm");
                }
                Console.WriteLine(execReturn.ReturnOutPut);
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
            ReqCommand _cmd = new ReqCommand("MST", "AZURE_DB", CommandType.StoredProcedure, "NakDongDB..USP_TEST_MST_INS");
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
            ReqCommand _cmd = new ReqCommand("DTL", "AZURE_DB", CommandType.StoredProcedure, "NakDongDB..USP_TEST_DTL_INS");
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
                ConnectionName = "AZURE_DB",
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
        
    }


}