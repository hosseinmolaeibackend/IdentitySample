using System.Net;
using System.Net.Mail;

namespace IdentitySample.Services
{
	public class MessageSender : IMessageSender
	{
		public async Task SendEmailAsync(string toemail, string subject, string message, bool isHtml = false)
		{
			var cerdentials = new NetworkCredential("hosein.molaei3@gmail.com", "triynkeklqytlgfe");

			using var _client = new SmtpClient()
			{
				EnableSsl = true,
				Credentials = cerdentials,
				Port = 587,
				Host = "smtp.gmail.com",
				UseDefaultCredentials = false,
			};
			using var email = new MailMessage()
			{
				From = new MailAddress("hosein.molaei3@gmail.com"),
				Subject = subject,
				Body = message,
				IsBodyHtml = isHtml
			};

			email.To.Add(toemail);

			await _client.SendMailAsync(email);
		}
	}
}
