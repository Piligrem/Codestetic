//Contributor:  Nicholas Mayne


namespace Codestetic.Web.Services.Authentication.External
{
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}