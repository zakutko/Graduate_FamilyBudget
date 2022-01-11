using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Eir.Services
{
    public static class SSL
    {
        public static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain defaultChain, SslPolicyErrors defaultErrors)
        {
            string CApem = File.ReadAllText("CA.pem");

            X509Certificate2 CApemCert = new(Encoding.UTF8.GetBytes(CApem));

            X509Chain caCertChain = new()
            {
                ChainPolicy = new X509ChainPolicy
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                    RevocationFlag = X509RevocationFlag.EntireChain,
                }
            };

            caCertChain.ChainPolicy.ExtraStore.Add(CApemCert);

            X509Certificate2 serverCert = new(certificate);

            caCertChain.Build(serverCert);

            if (caCertChain.ChainStatus.Length == 0)
            {
                // No errors
                return true;
            }

            foreach (X509ChainStatus status in caCertChain.ChainStatus)
            {
                // Check if we got any errors other than UntrustedRoot (which we will always get if we don't install the CA cert to the system store)
                if (status.Status != X509ChainStatusFlags.UntrustedRoot)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
