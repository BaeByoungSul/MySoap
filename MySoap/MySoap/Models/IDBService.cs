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
    [ServiceContract( Namespace ="http://nakdong.wcf.service"  )]
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
        [DataMember(Order= 0,IsRequired =true)]
        public string CommandName { get; set; }
        [DataMember(Order = 1,IsRequired =true)]
        public string ConnectionName { get; set; }
        [DataMember(Order = 2,IsRequired =true)]
        public int CommandType { get; set; }
        [DataMember(Order = 3,IsRequired=true)]
        public string CommandText { get; set; }
        [DataMember(Order = 4 )]
        public MyPara[] Parameters { get; set; }
        [DataMember(Order = 5)]
        public MyParaValue[][] ParaValues { get; set; }
    }
    [DataContract]
    public class MyPara
    {
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



}
