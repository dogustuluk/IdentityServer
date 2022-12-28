using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;

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
                new ApiResource("resource_api1") //basic auth için kullanıcı adıdır.
                {
                    Scopes={ "api1.read", "api1.write", "api1.update" },

                    ApiSecrets = new[]{new Secret("secretapi1".Sha256()) }//basic auth için şifre oluşturmuş olduk.
                },
                new ApiResource("resource_api2")
                {
                    Scopes ={ "api2.read", "api2.write", "api2.update" },
                     ApiSecrets = new[]{new Secret("secretapi2".Sha256()) }
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
        /// Kullanıcılardan hangi bilgilerin alınacağını ve token içerisinde hangi dataların tutulacağını içeren metottur.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //ön tanımlı olarak gelen resource'ları tanımlayalım.
            //bunlar kendi içlerinde claimler tutar. claimler: Kullanıcı hakkında tutmuş olduğumuz ekstra datalardır, token'ın payload'ında kullanıcı hakkında ekstra bilgiler tutabiliriz. OpenId, subId'ye karşılık gelir. Profile içerisinde de bir sürü claimler vardır. bunların içerisinde tutulan claimleri görmek için ezber yapmayız, dokümandan bakarız.
            //bunlardan ilki ve üyelik sisteminin olmazsa olmazıdır. neden gerekli; bir token döndüğü zaman içerisinde bu token'ın mutlaka bir kullanıcının id'si olmak zorundadır. Bu id subjectId olarak geçer. kullanıcı işin içine girince resource'lardan birisinin OpenId olması gerekir çünkü bu token'ın kim için üretildiğini bilelim. Yani hangi kullanıcı için olduğunu bilmek için.
            return new List<IdentityResource>()
            {
            new IdentityResources.OpenId(), //subId
            //profil bilgileri isteğe bağlıdır. burada kullanıcının ekstra bilgileri yer alır; yaşadığı şehir, il ilçe, soyadı, ilk ismi, göbek adı gibi.
            new IdentityResources.Profile(),
            new IdentityResource(){Name ="CountryAndCity", DisplayName="Country And City", Description="Kullanıcının ülke ve şehir bilgisi", UserClaims = new[]{"country","city"} }
            };
        }
        /// <summary>
        /// Test amaçlı oluşturulan kullanıcıları içeren metottur.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>()
        {
            new TestUser()
            {
                SubjectId = "1",
                Username = "DogusTuluk",
                Password = "password",
                Claims = new List<Claim>()
                {
                    new Claim("given_name","Doğuş"),
                    new Claim("family_name","Tuluk"),
                    new Claim("country","Turkey"),
                    new Claim("city","Izmir")
                }
            },
            new TestUser()
            {
                SubjectId = "2",
                Username = "Dogus2Tuluk2",
                Password = "password",
                Claims = new List<Claim>()
                {
                    new Claim("given_name","Doğuş2"),
                    new Claim("family_name","Tuluk2"),
                    new Claim("country","Turkey"),
                    new Claim("city","Istanbul")
                }
            }
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
                    AllowedScopes = { "api1.read" } //hangi api'de hangi izinlerin kullanılacağını vermiş oluyoruz.
                },
                new Client()
                {
                    ClientId = "Client2",
                    ClientName = "Client 2 app uygulaması",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read","api1.update" ,"api2.write", "api2.update" }
                },
                new Client()
                {
                    ClientId = "Client1-Mvc",
                    RequirePkce = false, //server-side olduğu için false
                    ClientName = "Client 1 app mvc uygulaması",
                    ClientSecrets = new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid, //response type olarak sadece code ise GrantTypes.Code yaparız.
                    /*RedirectUris
                     * Burada client1'in hangi portta çalıştığını bulup dönüş uri'sine veriyoruz ve ardından "/signin-oidc" yazarız. Eğer biz Client1 tarafında startup'a OpenIdConnect servisini eklediğimiz zaman bizim Client1 adlı sitemizde/uygulamamızda aşağıda tanımladığımız şekilde bir url oluşur. Bu url token alma işlemini gerçekleştiren url'dir. Olay şöyle gerçekleşir; eğer biz Client1 olarak Autorize endpoint'ine bir istek yapıp bize geriye code(authorization code) ve id_token geliyor. İşte buradan gelen değerlerin geri dönüş url'i buradaki adres olacaktır. buradaki url de token alma işlemini gerçekleştirmiş olacak ve arkasından bu url'den de benim herhangi bir sayfama yönlendirme işlemi gerçekleşecek.
                     * Kısaca authorize endpoint'ine bir istek yaptığımız zaman nereye döneceğimizi belirten bir url'dir.
                     * Client1 uygulamamda dependency paketi olarak Microsoft.AspNetCore.Authentication.OpenIdConnect paketini yüklediğim için otomatik olarak burada vermiş olduğum "signin-oidc" isminde bir url oluşuyor. Authorization server bu url'e dönüş yapıyor ve bu url üzerinden de bizim sitemize dönüş gerçekleşiyor. Burada token alma işlemi ve cookie işlemi gerçekleşiyor. Bunların hepsi otomatik olarak gerçekleşir. Eğer burada startup dosyasında OpenIdConnect paketini kullanmasaydık buradaki url oluşmayacaktı.
                     * Burada AuthServer'daki Config dosyamıza; herhangi bir kullanıcı bilgilerini doğru girdikten sonra döneceği adresi veriyoruz.
                     */
                    RedirectUris = new List<string>{ "https://localhost:5006/signin-oidc" },
                    /*signout
                     * signout işleminde iki durum olacak. ilki kendi uygulamamızdan yani buraya göre Client1'den hem de identity server'dan çıkış yapmalıyız.
                     * identity server'dan çıkış yapmak için bir yönlendirme yaptığımızda, identity server'dan tekrardan buraya yönlendirme yapmamız için redirect uri'ye ihtiyacımız vardır. Yani identity serverda logout olduğumuzda hangi üri'ye döneceğimizi belirtmemiz gerekmektedir. Bu uri'de ezberden değil, otomatik olarak OpenIdConnect protokolünden verilir. 
                     */
                    PostLogoutRedirectUris = new List<string>{ "https://localhost:5006/signout-callback-oidc" },
                    /*AllowedScopes
                     * Burada üyelikle ilgili bir client olduğundan dolayı identity server'ın sabitlerinden kullanıcıyı tanımlayam id(OpenId) ile opsiyonel olarak istediğimizi Profile bilgisini alıyoruz. Eğer istersek hangi izinlere sahip olduğunu da verebiliriz; örn. api1.read.
                     * offlineAccess ile eğer refresh token aldıysak kullanıcı siteye dahi girmese ben arka tarafta kullanıcı adına bir access token elde edebilirim. Özetle bir refresh token dağıtmak istiyorsak OfflineAccess değerini true'ya set etmek gerekir.
                     */
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "api1.read", IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity" },
                    /*access token lifetime
                     * access token'ın default olarak tanımlanan süresi 1 saat(3600 saniye).
                     * refresh token olarak iki tip ömür verme durumuna sahiptir; absolute ve sliding.
                     * absolute verirsek kesin bir tarih içerir. default olarak 30 gün olarak ayarlanır.
                     * sliding ise default olarak 15 gün olarak ayarlanır. Eğer bu 15 günün herhangi bir noktasında tekrardan istenirse, istenilen tarihin üzerine tekrardan 15 gün eklenir(ya da default tarihi kullanmazsak ayarladığımız süre boyunca ekleme yapılır.)
                     */
                    AccessTokenLifetime = (int)(DateTime.Now.AddHours(2) - DateTime.Now).TotalSeconds,
                    /*refresh token
                     * true'ya set edersek refresh token kullanılacağını belirtiriz.
                     * refresh token alabilmek için allowedScopes'a mutlaka --> IdentityServerConstants.StandardScopes.OfflineAccess <-- eklememiz gerekir.
                     * bunu ekledikten sonra ilgili client'ın startup'ına gidip --> opts.Scope.Add("offline_access") <-- yazmamız gereklidir.
                     */
                    AllowOfflineAccess = true,
                    /*RefreshTokenUsage
                     * refresh token'ın kaç kez kullanılacağını belirtiriz.
                     */
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,

                    /*Consent
                     * Eğer remember my decision özelliğini aktif yaparsak refresh token almamamız gerekmektedir. Eğer refresh token alan bir yapıya sahipsek yapılan seçimleri hatırla özelliğini aktif etsek dahi her giriş yaptığımızda tekrardan izinleri soracaktır. Bunun önüne geçmek için custom kodlama yapabiliriz ama doğru olanı refresh token alındığında eğer yeni bir izin eklenirse izinler sayfasına kullanıcıyı tekrardan yönlendirmek olacaktır.
                     */
                    RequireConsent = true

                }
            };
        }
    }
}
