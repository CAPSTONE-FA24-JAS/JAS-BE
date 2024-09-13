using Google.Authenticator;
using OtpNet;
namespace WebAPI.Service
{
    public class OtpService
    {
        private readonly byte[] _secretKey;

        public OtpService(string secretKey)
        {
            _secretKey = OtpNet.Base32Encoding.ToBytes(secretKey);
        }

        public string GenerateOtp()
        {
            var totp = new Totp(_secretKey, step: 30);
            var otp = totp.ComputeTotp();
            return otp; 
        }

        public bool ValidateOtp(string otp)
        {
            var tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(_secretKey, otp);
        }
    }

}
