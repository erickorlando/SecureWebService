using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using ConsoleApplication384.encoder;
using ConsoleApplication384.ServiceReference1;

namespace ConsoleApplication384
{
    class Program
    {
        static void Main()
        {

            try
            {
                var serverCert = new X509Certificate2(File.ReadAllBytes(@"N:\Clases\Certificates\server_cert.cer"));
                var clientCert = new X509Certificate2(File.ReadAllBytes(@"N:\Clases\Certificates\client_cert.pfx"),
                    ".Pr3sto2016");

                var address = new EndpointAddress(new Uri("http://localhost/ServicioSeguro/ServicioCalculadora.svc"),
                    EndpointIdentity.CreateX509CertificateIdentity(serverCert));
                var myCustomBinding = CreateCustomBinding(address);
                var client = new ServicioCalculadoraClient(myCustomBinding, address);

                if (client.ClientCredentials != null)
                    client.ClientCredentials.ClientCertificate.Certificate = clientCert;
                var res = client.SayHello(Environment.UserName);

                Console.WriteLine(res);

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static CustomBinding CreateCustomBinding(EndpointAddress address)
        {
            var res = new CustomBinding();

            var sec =
                (SymmetricSecurityBindingElement)SecurityBindingElement.CreateMutualCertificateBindingElement(
                    MessageSecurityVersion.
                        WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10);
            res.Elements.Add(sec);
            sec.SetKeyDerivation(false);
            sec.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
            sec.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic128;

            sec.EndpointSupportingTokenParameters.Signed.Add(sec.EndpointSupportingTokenParameters.Endorsing[0]);
            sec.EndpointSupportingTokenParameters.Endorsing.Clear();

            sec.EnableUnsecuredResponse = true;

            res.Elements.Add(new CustomTextMessageBindingElement());
            res.Elements.Add(new HttpTransportBindingElement());

            return res;
        }
    }
}
