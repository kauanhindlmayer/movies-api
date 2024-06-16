namespace Movies.Api.Auth;

public static class AuthConstants
{
    public const string AdminUserPolicyName = "Admin";
    public const string AdminUserClaimName = "Admin";
    
    public const string TrustedUserPolicyName = "Trusted";
    public const string TrustedUserClaimName = "Trusted";
    
    public const string ApiKeyHeaderName = "x-Api-Key";
}