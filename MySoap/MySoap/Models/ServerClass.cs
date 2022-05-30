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
    [ServiceContract(Namespace = "http://nakdong.wcf.service")]
    public interface IDBService
    {
        [OperationContract]
        SvcReturn ExecNonQuery(MyCommand[] cmd);

        [OperationContract]
        SvcReturn GetDataSetXml(MyCommand cmd);
    }

    [DataContract]
    public class MyCommand
    {
        public MyCommand(string commandName, string connectionName, int commandType, string commandText)
        {
            CommandName = commandName;
            ConnectionName = connectionName;
            CommandType = commandType;
            CommandText = commandText;
        }

        [DataMember(Order = 0, IsRequired = true)]
        public string CommandName { get; set; }
        [DataMember(Order = 1, IsRequired = true)]
        public string ConnectionName { get; set; }
        [DataMember(Order = 2, IsRequired = true)]
        public int CommandType { get; set; }
        [DataMember(Order = 3, IsRequired = true)]
        public string CommandText { get; set; }
        [DataMember(Order = 4)]
        public MyPara[] Parameters { get; set; }
        [DataMember(Order = 5)]
        public MyParaValue[][] ParaValues { get; set; }
    }
    [DataContract]
    public class MyPara
    {
        public MyPara(string parameterName, int dbDataType, int direction, string headerCommandName = "", string headerParameter = "")
        {
            ParameterName = parameterName;
            DbDataType = dbDataType;
            Direction = direction;
            HeaderCommandName = headerCommandName;
            HeaderParameter = headerParameter;
        }

        [DataMember(Order = 0)]
        public string ParameterName { get; set; }
        [DataMember(Order = 1)]
        public int DbDataType { get; set; }
        [DataMember(Order = 2)]
        public int Direction { get; set; }
        [DataMember(Order = 3)]
        public string HeaderCommandName { get; set; }
        [DataMember(Order = 4)]
        public string HeaderParameter { get; set; }
    }

    [DataContract]
    public class MyParaValue
    {
        public MyParaValue(string parameterName, string paraValue)
        {
            ParameterName = parameterName;
            ParaValue = paraValue;
        }

        [DataMember(Order = 0)]
        public string ParameterName { get; set; }
        [DataMember(Order = 1)]
        public string ParaValue { get; set; }

    }
    /// <summary>
    /// ReturnString: GetDataSetXml ( Xml string ) ExecNonQuery( Xml string )
    ///               GetMyUtilityFiles( strins ) 
    /// </summary>

    [DataContract]
    public class SvcReturn
    {
        [DataMember(Order = 0)]
        public string ReturnCD { get; set; }  // "FAIL", "OK"
        [DataMember(Order = 1)]
        public string ReturnMsg { get; set; }
        [DataMember(Order = 2)]
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
