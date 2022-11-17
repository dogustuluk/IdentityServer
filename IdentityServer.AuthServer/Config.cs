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
        /// <summary>
        /// IdentityServer'ın client'ları bildiği metottur. Burada uygulamanın sahip olduğu api'leri hangi client'ların kullanacağı tanıtılır.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>(){
                new Client()
                {
                    ClientId = "Client1", //kullanıcının username'i gibi düşünebiliriz.
                    ClientName = "Client 1 app uygulaması",//bir kullanıcı api ile ilgili data almak istediğinde işimize yarar.
                    ClientSecrets = new[]{new Secret("secret".Sha256())}, //sha ile hash'lememiz gerekir, normal tutulmaz. hash'lersek geriye dönemeyiz. hashlandikten sonra hashlenmiş olan değerle karşılaştırırz. 
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read", "api2.write", "api2.update" } //hangi api'de hangi izinlerin kullanılacağını vermiş oluyoruz.
                },
                new Client()
                {
                    ClientId = "Client2",
                    ClientName = "Client 2 app uygulaması",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read", "api2.write", "api2.update" }
                }
            };
        } 
    }
}
