using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace ServicioSeguro
{
    public class CertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (new X509Chain().Build(certificate))
            {
                if (certificate.Thumbprint.Equals("5ABB6C7F369E09D555EFC7ABCF25AABDDA18A35A"))
                {
                    return;
                }
            }

            throw new SecurityTokenValidationException();
        }
    }
}