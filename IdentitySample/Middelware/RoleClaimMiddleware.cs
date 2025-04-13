using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
public class RoleClaimMiddleware
{
	private readonly RequestDelegate _next;
	private readonly string _requiredRole;
	private readonly string _requiredClaimType;
	private readonly string _requiredClaimValue;

	public RoleClaimMiddleware(
		RequestDelegate next,
		string requiredRole,
		string requiredClaimType,
		string requiredClaimValue)
	{
		_next = next;
		_requiredRole = requiredRole;
		_requiredClaimType = requiredClaimType;
		_requiredClaimValue = requiredClaimValue;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		bool hasRole = context.User.IsInRole(_requiredRole);
		bool hasClaim = context.User.HasClaim(_requiredClaimType, _requiredClaimValue);

		if (!hasRole || !hasClaim)
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			context.Response.Redirect("/error/403");
			return;
		}

		await _next(context);
	}
}



public static class RoleClaimMiddlewareExtensions
{
	public static IApplicationBuilder UseRoleClaimMiddlewareForPath(
		this IApplicationBuilder builder,
		string path,
		string requiredRole,
		string requiredClaimType,
		string requiredClaimValue)
	{
		return builder.UseWhen(
			context => context.Request.Path.StartsWithSegments(path),
			appBranch => appBranch.UseMiddleware<RoleClaimMiddleware>(
				requiredRole,
				requiredClaimType,
				requiredClaimValue
			)
		);
	}
}