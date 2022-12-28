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
             * AddAuthentication bloðu ile kurmuþ olduðumuz AuthServer ile haberleþebilir bir hale geldik. authorization code dönmesi ve ardýndan token endpoint'e istek yapmasý iþlemlerini framework gerçekleþtirecek. burada sadece client ile ilgili olan ayarlamalardýr. 
             * henüz buradaki ayarlarý authorization server bilmiyor. onu bilgilendirmemiz için kodlar yazmamýz gerekiyor.
             * Bu ayarlarý yaparak merkezi bir üyelik sistemi oluþturmuþ olduk.
             */
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = "Cookies";
                opts.DefaultChallengeScheme = "oidc"; //cookie'nin kimle haberleþeceðini veriyoruz. openIdConnect'ten gelen cookie ile haberleþecek yani identityServer'dan gelenle.
                //schema kullanmamýzýn nedeni --> uygulamamýzda birden fazla cookie mekanizmasýl olabilir. örneðin bir web sitemiz var, bir tanesi normal kullanýcýlar bir diðeri de bayii kullanýcýlarý olabilir. bunlarý birbirinden ayýrmak için kullanýrýz.
            }).AddCookie("Cookies").AddOpenIdConnect("oidc", opts =>
            {
                opts.SignInScheme = "Cookies";
                opts.Authority = "https://localhost:5001"; //token'ý daðýtan yeri yazarýz. yani yetkili yer.
                opts.ClientId = "Client1-Mvc";
                opts.ClientSecret = "secret";
                opts.ResponseType = "code id_token";
                /*GetClaimsFromUserInfoEndpoint
                 * Eðer kullanýcý hakkýnda ek bilgileri de almak istiyorsak true'ya set etmeliyiz.
                 * Default deðerlerde ek bilgilerin cookie'de tutulmasýný identity server saðlamaz çünkü cookie'yi gereksiz bir sürü bilgi ile doldurmak istemez. Ama isteðe baðlý olarak açabiliriz.
                 */
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.SaveTokens = true;
                /*Scope
                 * Bu isteði AuthServer'a yaptýðýmýz zaman, AuthServer içerisinde bulunan Config dosyasýndan AllowedScopes içerisinde ilgili izin olup olmadýðýný kontrol edecek ve buna göre bir cevap dönecektir. 
                 * Eðer olmayan bir izni yazarsak hata verecektir.
                 * Ýzin istemek için önce AuthServer'ýn Config dosyasýnda AllowedScopes'ta tanýmlýyoruz, daha sonra ilgili Client'ýn startup dosyasýnda da AddOpenIdConnect'te hangi izni tanýmladýysak buraya da geçmeliyiz aþaðýdaki gibi.
                 */
                opts.Scope.Add("api1.read");
                opts.Scope.Add("offline_access");//refresh token için gereklidir
                
                //custom identity resource eklediysek
                opts.Scope.Add("CountryAndCity");
                //buradak country isminde bir þeye eþitle deriz ilkinde. Ýkinci parametrede ise Config'te eklemiþ olduðumuz identity resource içerisindeki key adýný veriyoruz. Bu sayede mapleme iþlemini yapmýþ oluruz.
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
