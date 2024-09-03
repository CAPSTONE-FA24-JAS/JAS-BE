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
            var emailFrom = "danhdcss160060@fpt.edu.vn";
            var password = "iiws nica yiuu irnk";
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

            // message.Body = new TextPart("plain")
            // {
            //     Text = $"Please click the link below to confirm your email:\n{confirmationLink}"
            // };
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
    }

}
