using OtpNet;

namespace Application.Utils
{
    public static class OtpService
    {
        public static string GenerateOtp(string secretKey)
        {
            var guidBytes = Guid.Parse(secretKey).ToByteArray();
            var Base = Base32Encoding.ToString(guidBytes);
            var secretKeyBytes = Base32Encoding.ToBytes(Base);
            var totp = new Totp(secretKeyBytes, step: 60);
            var otp = totp.ComputeTotp();
            return otp;
        }

        public static object ValidateOtp(string secretKey, string otp)
        {
            var guidBytes = Guid.Parse(secretKey).ToByteArray();
            var Base = Base32Encoding.ToString(guidBytes);
            var secretKeyBytes = Base32Encoding.ToBytes(Base);
            var totp = new Totp(secretKeyBytes, step: 60);
            bool isValid = totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (isValid)
            {
                var response = new { msg = "Verify successful", status = true };
                return response;
            }
            else
            {
                var response = new { msg = "OTP not valid, maybe timeStepMatched", status = false, timeStepMatched = timeStepMatched };
                return response;
            }
        }

        public static string GenerateOtpForAuthorized(string secretKey, int jewelryId, int sellerId)
        {
            string combine = secretKey + jewelryId.ToString() + sellerId.ToString();
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combine));
                var Base = Base32Encoding.ToString(hashBytes); // Chuyển hash thành Base32
                var secretKeyBytes = Base32Encoding.ToBytes(Base); // Chuyển Base32 thành mảng byte

                var totp = new Totp(secretKeyBytes, step: 200);
                var otp = totp.ComputeTotp();
                return otp;
            }
        }


        public static object ValidateOtpForAuthorized(string secretKey, int jewelryid, int staffId, string otp)
        {
            string combine = secretKey + jewelryid.ToString() + staffId.ToString();
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combine));
                var Base = Base32Encoding.ToString(hashBytes); // Chuyển hash thành Base32
                var secretKeyBytes = Base32Encoding.ToBytes(Base); // Chuyển Base32 thành mảng byte

                var totp = new Totp(secretKeyBytes, step: 200);
                bool isValid = totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (isValid)
                {
                    var response = new { msg = "Verify successful", status = true };
                    return response;
                }
                else
                {
                    var response = new { msg = "OTP not valid", status = false, timeStepMatched = timeStepMatched };
                    return response;
                }
            }
        }

    }
}
