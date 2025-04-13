using Microsoft.AspNetCore.Authorization;

namespace IdentitySample.Security.Default
{
	public class ClaimRequirement : IAuthorizationRequirement
	{
		public ClaimRequirement(string claimtype,string claimvalue)
		{
			ClaimType = claimtype;
			ClaimValue = claimvalue;
		}
		public string ClaimType { get; }
		public string ClaimValue { get; }

	}
}
