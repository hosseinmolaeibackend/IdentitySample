namespace IdentitySample.Models.ViewModel
{
	public class AddOrRemoveClaimViewModel
	{
		public AddOrRemoveClaimViewModel()
		{
			UserClaims = new List<ClaimsViewModel>();
		}
		public AddOrRemoveClaimViewModel(string userId, IList<ClaimsViewModel> userClaims)
		{
			UserId = userId;
			UserClaims = userClaims;
		}
		public string UserId { get; set; }
		public IList<ClaimsViewModel> UserClaims { get; set; }
	}

	public class ClaimsViewModel
	{
		public ClaimsViewModel()
		{
			
		}
		public ClaimsViewModel(string claimType)
		{
			ClaimType = claimType;
		}
		public string ClaimType { get; set; }
		public bool IsSelected { get; set; }
	}
}
