using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Data;
using System.ServiceModel;

namespace BBS
{

    public class MyCommand
    {
        public MyCommand(string commandName, string connectionName, int commandType, string commandText)
        {
            CommandName = commandName;
            ConnectionName = connectionName;
            CommandType = commandType;
            CommandText = commandText;
        }
    
        public string CommandName { get; set; }
        public string ConnectionName { get; set; }
        public int CommandType { get; set; }
        public string CommandText { get; set; }
        public MyPara[] Parameters { get; set; }
        public MyParaValue[][] ParaValues { get; set; }
    }
    [DataContract]
    public class MyPara
    {
        public MyPara(string parameterName, int dbDataType, int direction, string headerCommandName = "", string headerParameter="")
        {
            ParameterName = parameterName;
            DbDataType = dbDataType;
            Direction = direction;
            HeaderCommandName = headerCommandName;
            HeaderParameter = headerParameter;
        }

        public string ParameterName { get; set; }
        public int DbDataType { get; set; }
        public int Direction { get; set; }
        public string HeaderCommandName { get; set; }
        public string HeaderParameter { get; set; }
    }

    public class MyParaValue
    {
        public MyParaValue(string parameterName, string paraValue)
        {
            ParameterName = parameterName;
            ParaValue = paraValue;
        }

        public string ParameterName { get; set; }
        public string ParaValue { get; set; }
    }
      
    public class SvcReturn
    {
        public string ReturnCD { get; set; }  // "FAIL", "OK"
        public string ReturnMsg { get; set; }
        public string ReturnStr { get; set; } 
    }
    /// <summary>
    /// ExecNonQuery시 Out Put값을 저장
    /// </summary>
    public class DBOutPut
    {
        public int Rowseq { get; set; }
        public string CommandName { get; set; }
        public string ParameterName { get; set; }
        public string OutValue { get; set; }

    }

}
