using System.ServiceModel;

using Para.Server.Contract.Argument;

namespace Para.Server.Contract
{
    [ServiceContract]
    public interface IParaService
    {
        [OperationContract]
        Response.Response GetValue(GetValueArgument argument);
        [OperationContract]
        Response.Response ConvertValue(ConvertValueArgument argument);
        [OperationContract]
        void SaveValue();
        [OperationContract]
        void StartTimer();
    }
}