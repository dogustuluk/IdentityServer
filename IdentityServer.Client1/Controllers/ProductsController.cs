using IdentityModel.Client;
using IdentityServer.Client1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServer.Client1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IConfiguration _configuration; //Http içerisindeki datayı okuyabilmek için IConfiguration'a ihtiyaç vardır.

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();   
            HttpClient httpClient= new HttpClient();

            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (discovery.IsError)
            {
                //hatayı yakalamak için --> discovery.Error;
                //loglama yap.
            }

            ClientCredentialsTokenRequest clientCredentialsTokenRequest = new ClientCredentialsTokenRequest();

            clientCredentialsTokenRequest.ClientId = _configuration["Client:ClientId"];
            clientCredentialsTokenRequest.ClientSecret = _configuration["Client:ClientSecret"];
            clientCredentialsTokenRequest.Address = discovery.TokenEndpoint; //hangi endpoint'ten token alınacağını veriyoruz.

            var token = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest); //elimizde bir token oluyor. artık istek yapabiliriz.

            if (token.IsError)
            {
                //loglama yap, gerekli mesaj ver. Business ihtiyaçlarını yaz.
            }

            //istek yapmadan önce istek yapacağımız, istek yapacağımız request'in header'ında authorization key-value çiftini göndermemiz gerekir. yardımcı metot olan SetBearerToken ile yaparız.
            httpClient.SetBearerToken(token.AccessToken);//token'dan acces token'ı alıyoruz.
            //artık istek yapılabilir.
            var response = await httpClient.GetAsync("https://localhost:5016/api/product/getproducts");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                products = JsonConvert.DeserializeObject<List<Product>>(content);//deserialize et.
            }
            else
            {
                //loglama yap.hatayı göster vs.
            }

            return View(products);
        }
    }
}
