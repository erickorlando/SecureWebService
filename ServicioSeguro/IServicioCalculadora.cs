using System.ServiceModel;

namespace ServicioSeguro
{
    [ServiceContract]
    public interface IServicioCalculadora
    {

        [OperationContract]
        string SayHello(string value);

    }
    
}
