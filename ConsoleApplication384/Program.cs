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

            var serverCert = new X509Certificate2(File.ReadAllBytes(@"c:\server_cert.cer"));
            var clientCert = new X509Certificate2(File.ReadAllBytes(@"c:\client_cert.pfx"), "password");

            var address = new EndpointAddress(new Uri("http://localhost/ServicioSeguro/ServicioCalculadora.svc"),
                                                       EndpointIdentity.CreateX509CertificateIdentity(serverCert));
            var myCustomBinding = CreateCustomBinding(address);
            var client = new SimpleServiceSoapClient(myCustomBinding, address);

            client.ClientCredentials.ClientCertificate.Certificate = clientCert;
            var res = client.EchoString("hello");
                        
            client.Close();
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
