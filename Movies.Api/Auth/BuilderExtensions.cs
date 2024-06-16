using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Movies.Api.Auth;

public static class BuilderExtensions
{
    public static void AddJwtAuthentication(this WebApplicationBuilder builder, IConfiguration config)
    {
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true
            };
        });

        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy(AuthConstants.AdminUserPolicyName,
                p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));
            x.AddPolicy(AuthConstants.TrustedUserPolicyName, p => p.RequireAssertion(c =>
                c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                c.User.HasClaim(m => m is { Type: AuthConstants.TrustedUserClaimName, Value: "true" })));
        });
    }

    public static void AddApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1.0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
        }).AddMvc().AddApiExplorer();
    }

    public static void AddOutputCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddOutputCache(x =>
        {
            x.AddBasePolicy(c =>
            {
                c.Cache().Expire(TimeSpan.FromMinutes(1))
                    .SetVaryByQuery(["title", "yearOfRelease", "sortBy", "page", "pageSize"]).Tag("movies");
            });
        });
    }
}