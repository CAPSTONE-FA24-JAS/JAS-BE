﻿using OtpNet;
using System;
using System.Text;

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
    }
}