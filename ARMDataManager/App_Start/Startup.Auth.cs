using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ARMDataManager.Providers;
using ARMDataManager.Models;

namespace ARMDataManager
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // Kimlik doğrulamayı yapılandırma hakkında daha fazla bilgi için lütfen https://go.microsoft.com/fwlink/?LinkId=301864 adresini ziyaret edin
        public void ConfigureAuth(IAppBuilder app)
        {
            // db bağlamını ve kullanıcı yöneticisini istek başına tek örnek kullanacak şekilde yapılandırın
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Oturum açan kullanıcıya ilişkin bilgileri depolamak üzere bir tanımlama bilgisi kullanmak için uygulamayı etkinleştirin
            // ve üçüncü taraf bir oturum açma sağlayıcısı üzerinden oturum açan kullanıcı bilgilerini tanımlama bilgileri olarak saklamak için  veritabanı bağlamını ve kullanıcı yöneticisini yapılandırın
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uygulamayı OAuth tabanlı akış için yapılandırın
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // Üretimde olan mod kümesi AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Kullanıcıların kimliğini doğrulamak üzere uygulamanın taşıyıcı belirteçleri kullanmasını sağlayın
            app.UseOAuthBearerTokens(OAuthOptions);

            // Üçüncü taraf oturum açma sağlayıcılarıyla oturum açmayı etkinleştirmek için aşağıdaki satırların açıklamasını kaldırın
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
