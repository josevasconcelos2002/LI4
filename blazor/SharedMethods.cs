using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;


public static class SharedMethods
{
    public static async Task<string> GetNifFromToken(ILocalStorageService localStorage)
    {
        var token = await localStorage.GetItemAsync<string>("jwt_token");
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        var nifClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type.Equals("Nif", StringComparison.OrdinalIgnoreCase));

        return nifClaim?.Value;
    }
}
