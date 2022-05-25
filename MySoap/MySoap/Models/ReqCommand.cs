using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBS
{
    //public class ReqReturn
    //{
    //    public string ReturnCD { get; set; }
    //    public string ReturnMsg { get; set; }
    //    public DataSet Ds { get; set; }
    //}

    /// <summary>
    /// Service parameter MyCommand 가 불편해서 추가
    /// </summary>
    public class ReqCommand
    {
       // private List<ReqPara> parameters;
        public ReqCommand()
        {
        }

        public ReqCommand(string commandName, string connectionName, CommandType commandType, string commandText)
        {
            CommandName = commandName;
            ConnectionName = connectionName;
            CommandType = commandType;
            CommandText = commandText;
            Parameters = new List<ReqPara>();
            ParameterValues = new List<Dictionary<string, object>>();
        }

        public string CommandName { get; set; }
        public string ConnectionName { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public List<ReqPara> Parameters { get; set; }
        public List<Dictionary<string, object>> ParameterValues { get; set; }


        /// <summary>
        /// 변형된 request parameter를 서비스 parameter 
        /// </summary>
        /// <param name="reqCommand"></param>
        /// <returns></returns>
        public MyCommand GetMyCommand()
        {
            MyCommand mycmd = new MyCommand(  this.CommandName, this.ConnectionName,
                (int)this.CommandType, this.CommandText
            );

            if (this.Parameters == null) return mycmd;

            // Parameter 처리
            List<MyPara> myParas = new List<MyPara>();
            foreach (ReqPara req in this.Parameters )
            {
                MyPara myPara = new MyPara(
                    req.ParameterName,
                    req.DbDataType,
                    (int)req.Direction,
                    req.HeaderCommandName,
                    req.HeaderParameter
                );
                myParas.Add(myPara);
            }
            mycmd.Parameters = myParas.ToArray();

            // Parameter값 처리 처리
            if (this.ParameterValues == null) return mycmd;

            // List Dictionary Loop > 하나의 Dictionary별로 실행이 됨 
            // Dictionary to MyParaValue Array > List Add > List To MyParaValue[][] Array
            List<MyParaValue[]> listOflist_ParaValues = new List<MyParaValue[]>();
            foreach (Dictionary <string,object> paraDic in this.ParameterValues )
            {
                List<MyParaValue> listParaValues = new List<MyParaValue>();
                foreach (KeyValuePair<string,object> paraPair in paraDic)
                {
                    MyParaValue myPara = new MyParaValue(
                        paraPair.Key, 
                        paraPair.Value == null ? string.Empty : paraPair.Value.ToString()
                    );
                    listParaValues.Add(myPara);
                    Console.WriteLine("Key:{0} Value: {1}", paraPair.Key, paraPair.Value);
                }
                listOflist_ParaValues.Add(listParaValues.ToArray());

            }

            mycmd.ParaValues = listOflist_ParaValues.ToArray();
            return mycmd;
        }
    }
    public class ReqPara
    {
        public ReqPara()
        {
        }

        public ReqPara( string parameterName, Enum dbDataType, ParameterDirection direction, 
                        string headerCommandName = null, string headerParameter = null)
        {
            ParameterName = parameterName;
            DbDataType = Convert.ToInt32( dbDataType);
            Direction = direction;
            HeaderCommandName = headerCommandName;
            HeaderParameter = headerParameter;
        }
        public string ParameterName { get; set; }
        public int DbDataType { get; set; }
        public ParameterDirection Direction { get; set; }
        public string HeaderCommandName { get; set; }
        public string HeaderParameter { get; set; }
    }
    public class ReqParaValue
    {
        public string ParameterName { get; set; }
        public string ParaValue { get; set; }

    }

}
