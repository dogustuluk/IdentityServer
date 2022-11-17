using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.AuthServer
{
    public static class Config
    {
        /// <summary>
        /// AuthServer'ın hangi api'lerden sorumlu olduğunu belirten metot. Ek olarak api'lerin hangi scope'lara sahip olduğu da belirtilmelidir.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("resource_api1")
                {
                    Scopes={ "api1.read", "api1.write", "api1.update" }
                },
                new ApiResource("resource_api2")
                {
                    Scopes ={ "api2.read", "api2.write", "api2.update" }
                }
            };
        }
        /// <summary>
        /// api'lerin hangi izinlere sahip olacaklarını belirten metot.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new ApiScope("api1.read","API1 için okuma izni"),
                new ApiScope("api1.write","API1 için yazma izni"),
                new ApiScope("api1.update","API1 için güncelleme izni"),
                new ApiScope("api2.read","API2 için okuma izni"),
                new ApiScope("api2.write","API2 için yazma izni"),
                new ApiScope("api2.update","API2 için güncelleme izni"),
            };
        }
    }
}
