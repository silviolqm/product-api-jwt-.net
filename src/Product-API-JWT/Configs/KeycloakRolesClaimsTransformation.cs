using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var newIdentity = principal.Identity as ClaimsIdentity;
        if (newIdentity == null)
        {
            return Task.FromResult(principal);
        }

        var realmAccessClaim = principal.FindFirst("realm_access");
        if (realmAccessClaim?.Value == null)
        {
            return Task.FromResult(principal);
        }

        try
        {
            var realmAccess = JsonDocument.Parse(realmAccessClaim.Value);
            var roles = realmAccess.RootElement.GetProperty("roles").EnumerateArray();

            foreach (var role in roles)
            {
                var roleValue = role.GetString();
                if (!string.IsNullOrEmpty(roleValue))
                {
                    newIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to extract roles from JWT:" + e.Message);
        }

        return Task.FromResult(new ClaimsPrincipal(newIdentity));
    }
}