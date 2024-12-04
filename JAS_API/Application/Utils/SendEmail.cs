using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Application.Utils
{
    public static class SendEmail
    {
        public static async Task<bool> SendConfirmationEmail(
            string toEmail,
            string confirmationLink
        )
        {
            var userName = "JAS";
            var emailFrom = "jassystem57@gmail.com";
            var password = "xqoj jhnk jgjs qvev";
            string emailTemplate =
                @"
<html>
    <head>
        <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #ffffff;
                color: #333333;
                margin: 0;
                padding: 0;
                text-align: center;
            }
            .container {
                max-width: 600px;
                margin: 50px auto;
                padding: 20px;
                border: 1px solid #e0e0e0;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                background-color: #ffffff;
            }
            .header {
                padding: 20px;
                border-bottom: 1px solid #f0f0f0;
            }
            .header img {
                max-width: 100px;
                margin-bottom: 10px;
            }
            .header h1 {
                font-size: 24px;
                color: #d4af37; /* Vàng ánh kim */
            }
            .content {
                padding: 20px;
            }
            .content p {
                font-size: 16px;
                line-height: 1.6;
                margin: 20px 0;
            }
            .button {
                display: inline-block;
                padding: 10px 20px;
                font-size: 18px;
                color: #ffffff;
                background-color: #d4af37; /* Vàng ánh kim */
                border-radius: 5px;
                text-decoration: none;
                margin-top: 20px;
            }
            .button:hover {
                background-color: #b58d30; /* Màu vàng ánh kim đậm hơn khi hover */
            }
            .footer {
                margin-top: 30px;
                font-size: 12px;
                color: #999999;
            }
            .gold-divider {
                width: 50%;
                height: 2px;
                background-color: #d4af37;
                margin: 20px auto;
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <img src='https://clipartcraft.com/images/diamond-clipart-gold-8.png' alt='Diamond Icon'>
                <h1>Confirm Your Email</h1>
            </div>
            <div class='content'>
                <p>Thank you for registering with our diamond system. Please confirm your email address by clicking the button below.</p>
                <a href='"
                + confirmationLink
                + @"' class='button'>Confirm Email</a>
                <div class='gold-divider'></div>
                <p>If you did not create an account, please ignore this email.</p>
            </div>
            <div class='footer'>
                <p>&copy; 2024 Diamond System. All rights reserved.</p>
            </div>
        </div>
    </body>
</html>";



            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Confirmation Email";
            message.Body = new TextPart("html")
            {
                Text = emailTemplate
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Thiết lập xác thực với tài khoản Gmail
                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public static async Task<bool> SendEmailOTP(string toEmail, string OTP)
        {
            var userName = "JAS";
            var emailFrom = "jassystem57@gmail.com";
            var password = "xqoj jhnk jgjs qvev";
            string emailTemplate = @"<!DOCTYPE html>
            <html lang='vi'>

            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <meta http-equiv='X-UA-Compatible' content='ie=edge'>
                <title>Xác nhận OTP</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f9f9f9;
                    }

                    .container {
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #fff;
                        border-radius: 10px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                    }

                    .header {
                        text-align: center;
                        padding: 20px;
                        background-color: #ffcc00;
                        border-top-left-radius: 10px;
                        border-top-right-radius: 10px;
                    }

                    .header img {
                        max-width: 150px;
                    }

                    .content {
                        padding: 30px;
                        text-align: center;
                        color: #333;
                    }

                    .content h1 {
                        color: #ffcc00;
                    }

                    .content p {
                        font-size: 18px;
                        line-height: 1.6;
                        color: #555;
                    }

                    .otp-code {
                        display: inline-block;
                        font-size: 24px;
                        font-weight: bold;
                        margin: 20px 0;
                        padding: 10px 20px;
                        border-radius: 5px;
                        background-color: #ffcc00;
                        color: #fff;
                    }

                    .footer {
                        text-align: center;
                        padding: 20px;
                        font-size: 14px;
                        color: #999;
                    }
                </style>
            </head>

            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://res.cloudinary.com/dmbfbs6ok/image/upload/v1725771144/pblsuayckjd3ry9qg6wz.jpg' alt='Company Logo'>
                    </div>
                    <div class='content'>
                        <h1>Xác nhận OTP của bạn</h1>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi. Dưới đây là mã OTP của bạn:</p>
                        <div class='otp-code'>" + OTP + @"</div>
                        <p>Mã OTP này sẽ hết hạn sau 1 phút. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                    </div>
                    <div class='footer'>
                        <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
                    </div>
                </div>
            </body>

            </html>
            ";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(userName, emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Verify OTP";
            message.Body = new TextPart("html")
            {
                Text = emailTemplate
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                client.Authenticate(emailFrom, password);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }

}
