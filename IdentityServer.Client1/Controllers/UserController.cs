using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Client1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            //kullanıcının id'sini almak istersek.
            //var userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

            return View();
        }
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync("Cookies"); //uygulamamızdan çıkış yapar.
            await HttpContext.SignOutAsync("oidc"); //identity server'dan çıkış yapar.
        }
        /// <summary>
        /// Bu method ile yeni bir access token alma işlemi gerçekleştirilmiş olur. Burada token süresi biterse kullanıcının tekrardan login ekranına dönmesini istemeyiz, arka tarafta kaydetmiş olduğumuz refresh token üzerinden yeni bir access token almış oluruz.Geriye bir değer dönmez, index sayfasına yönlendirme yapar.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetRefreshToken()
        {
            //istek yapmak için HttpClient alırız.
            HttpClient httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001"); //endpoint'i alırız.
            if (disco.IsError)
            {
                //loglama yap ve/veya hata sayfasına yönlendir.
            }
            /*refreshToken
             * burada bir refresh token alıyoruz.
             * OpenIdConnectParameterNames'ın sabitlerinden RefreshToken'ı alıyoruz. Bu Authentication'ın parametrelerinden refresh token'ı elde edecek. yani cookie içerisindeki refresh token'ı aldık.
             */
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest();
            refreshTokenRequest.ClientId = _configuration["Client1Mvc:ClientId"];
            refreshTokenRequest.ClientSecret = _configuration["Client1Mvc:ClientSecret"];
            refreshTokenRequest.RefreshToken = refreshToken;
            refreshTokenRequest.Address = disco.TokenEndpoint; //endpoint'i belirtiriz.
            //burada username ve password gibi alanları belirtmedik çünkü bu istek cookie'ler üzerinden gideceği için identity server bu property'leri biliyor olacak.


            /*RequestRefreshTokenAsync
             *Bir istek üzerinden refresh token almak için kullanırız.
             *Bu metot bizden RefreshTokenRequest ister.
             *Bu metot --> refresh_token grant tipinde bir istek gönderir.
             *Bu metot ile tüm token'lar gelecek(IdToken,AccessToken,RefreshToken).
             */
            var token = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest); 

            if (token.IsError)
            {
                //loglama yap ve/veya hata sayfasına yönlendir.
            }


            /*tokens
             *Bu işlemlerin devamında ise bizim yapmamız gereken IdToken, AccessToken, RefreshToken ve token'ın ömründe değişiklikler yapmamız gerekir.
             *AuthenticationToken listesi oluşturup bunun içerisinde yeni gelen token'ları oluşturuyoruz.
             *token ömründe int bir değer döner, bunu datetime'a çevirmemiz gerekir. Ayrıca bunu utc üzerinden değiştirmemiz gerekir. yani evrensel zamanı almamız gereklidir. Ek olarak Culture'a bağımlı olmasını istemeyiz yani ingiltereden giren birine farklı tarih formatı veya türkiyeden giren biri için farklı formatlar çıkmaması için InvariantCulture olarak işaretleme yapmalıyız.
             */
            var tokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = token.IdentityToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
                new AuthenticationToken { Name = OpenIdConnectParameterNames.ExpiresIn, Value = DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture) }
            };

            /*authenticationResult
             * HttpContext üzerinden Authentication verilerini alıyoruz.
             */
            var authenticationResult = await HttpContext.AuthenticateAsync();

            /*properties
             * authenticationResult üzerinden mevcut property'leri alıyoruz.
             */
            var properties = authenticationResult.Properties;

            /*StoreTokens
             * Alınan property'leri güncelleme işlemi yapılıyor.
             * StoreTokens bir IEnumerable tipinde AuthenticationToken istiyor. tokens ile bunu verebiliriz.
             * Artık token'lar set edilmiş oldu. tokens'ta vermiş olduğumuz değerler geldi.
             */
            properties.StoreTokens(tokens);

            /*desc
             * tekrardan giriş yapıyor olucaz.
             * schema -> Cookies, ClaimPrincipal --> authenticationResult nesnesi üzerinden otomatik olarak alırız(kimlik kartının içindeki ad,soyad,doğum tarihi gibi bilgilerin tutulduğu kart olarak düşünülebilir.), AuthenticationProperties olarak ise yukarıda tanımladığımız properties veriliyor.
             * signin olunduğunda ise cookie'nin içerisindeki authenticationResult'ın içerisinde gelen dataları güncellemiş olduk.
             */
            await HttpContext.SignInAsync("Cookies", authenticationResult.Principal, properties);

            return RedirectToAction("Index");
        }
    }
}