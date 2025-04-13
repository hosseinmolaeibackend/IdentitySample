using IdentitySample.Models.ViewModel;

namespace IdentitySample.Utitlities
{
	public interface IUtilities
	{
		public IList<ActionAndControllerName> ActionAndControllerNameList();
		public IList<string> GetAllAreaNames();
		public string DataBaseRoleValidationGuid();
	}
}
