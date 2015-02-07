using Para.Server.Contract.Argument;
using Para.Server.Contract.Response;

namespace Para.Server.Business.Manager
{
    public interface IParaManager
    {
        Response ConvertValue(ConvertValueArgument argument);
        Response GetValue(GetValueArgument argument);
        
        void SaveValue();
    }
}