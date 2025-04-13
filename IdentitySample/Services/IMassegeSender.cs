namespace IdentitySample.Services
{
	public interface IMessageSender
	{
		public Task SendEmailAsync(string email, string subject, string message, bool isHtml = false);
	}
}
