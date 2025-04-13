using System.Security.Claims;

namespace IdentitySample.Services
{
	public static class ClaimStore
	{
		public static List<Claim> AllClaims = new List<Claim>()
		{
			new Claim(ClaimTypeStore.ViewEmployeeList,true.ToString()),
			new Claim(ClaimTypeStore.DeleteEmployee,true.ToString()),
			new Claim(ClaimTypeStore.UpdateEmployee,true.ToString()),
			new Claim(ClaimTypeStore.AddEmployee,true.ToString()),
			new Claim(ClaimTypeStore.ViewEmployeeDetail,true.ToString())
		};
	}

	public static class ClaimTypeStore
	{
		public const string ViewEmployeeList = "Employee list";
		public const string ViewEmployeeDetail = "Employee Details";
		public const string AddEmployee = "Employee Create";
		public const string UpdateEmployee = "Employee Edit";
		public const string DeleteEmployee = "Employee Delete";

	}
}
