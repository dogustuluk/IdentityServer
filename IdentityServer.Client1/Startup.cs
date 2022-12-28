using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Client1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*desc
             * AddAuthentication blo�u ile kurmu� oldu�umuz AuthServer ile haberle�ebilir bir hale geldik. authorization code d�nmesi ve ard�ndan token endpoint'e istek yapmas� i�lemlerini framework ger�ekle�tirecek. burada sadece client ile ilgili olan ayarlamalard�r. 
             * hen�z buradaki ayarlar� authorization server bilmiyor. onu bilgilendirmemiz i�in kodlar yazmam�z gerekiyor.
             * Bu ayarlar� yaparak merkezi bir �yelik sistemi olu�turmu� olduk.
             */
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = "Cookies";
                opts.DefaultChallengeScheme = "oidc"; //cookie'nin kimle haberle�ece�ini veriyoruz. openIdConnect'ten gelen cookie ile haberle�ecek yani identityServer'dan gelenle.
                //schema kullanmam�z�n nedeni --> uygulamam�zda birden fazla cookie mekanizmas�l olabilir. �rne�in bir web sitemiz var, bir tanesi normal kullan�c�lar bir di�eri de bayii kullan�c�lar� olabilir. bunlar� birbirinden ay�rmak i�in kullan�r�z.
            }).AddCookie("Cookies").AddOpenIdConnect("oidc", opts =>
            {
                opts.SignInScheme = "Cookies";
                opts.Authority = "https://localhost:5001"; //token'� da��tan yeri yazar�z. yani yetkili yer.
                opts.ClientId = "Client1-Mvc";
                opts.ClientSecret = "secret";
                opts.ResponseType = "code id_token";
                /*GetClaimsFromUserInfoEndpoint
                 * E�er kullan�c� hakk�nda ek bilgileri de almak istiyorsak true'ya set etmeliyiz.
                 * Default de�erlerde ek bilgilerin cookie'de tutulmas�n� identity server sa�lamaz ��nk� cookie'yi gereksiz bir s�r� bilgi ile doldurmak istemez. Ama iste�e ba�l� olarak a�abiliriz.
                 */
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.SaveTokens = true;
                /*Scope
                 * Bu iste�i AuthServer'a yapt���m�z zaman, AuthServer i�erisinde bulunan Config dosyas�ndan AllowedScopes i�erisinde ilgili izin olup olmad���n� kontrol edecek ve buna g�re bir cevap d�necektir. 
                 * E�er olmayan bir izni yazarsak hata verecektir.
                 * �zin istemek i�in �nce AuthServer'�n Config dosyas�nda AllowedScopes'ta tan�ml�yoruz, daha sonra ilgili Client'�n startup dosyas�nda da AddOpenIdConnect'te hangi izni tan�mlad�ysak buraya da ge�meliyiz a�a��daki gibi.
                 */
                opts.Scope.Add("api1.read");
                opts.Scope.Add("offline_access");//refresh token i�in gereklidir
                
                //custom identity resource eklediysek
                opts.Scope.Add("CountryAndCity");
                //buradak country isminde bir �eye e�itle deriz ilkinde. �kinci parametrede ise Config'te eklemi� oldu�umuz identity resource i�erisindeki key ad�n� veriyoruz. Bu sayede mapleme i�lemini yapm�� oluruz.
                opts.ClaimActions.MapUniqueJsonKey("country", "country");
                opts.ClaimActions.MapUniqueJsonKey("city", "city");
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
