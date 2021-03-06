﻿using System;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ConferenceWebAPI.Providers;
using ConferenceWebAPI.DAL;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Google;

namespace ConferenceWebAPI
{
	public partial class Startup
	{
		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		public static string PublicClientId { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth( IAppBuilder app )
		{
			// Configure the db context and user manager to use a single instance per request
			app.CreatePerOwinContext( ConferenceContext.Create );
			app.CreatePerOwinContext<ApplicationUserManager>( ApplicationUserManager.Create );

			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseCookieAuthentication( new CookieAuthenticationOptions() );
			app.UseExternalSignInCookie( DefaultAuthenticationTypes.ExternalCookie );

			// Configure the application for OAuth based flow
			PublicClientId = "self";
			OAuthOptions = new OAuthAuthorizationServerOptions
			{
				TokenEndpointPath = new PathString( "/Token" ),
				Provider = new ApplicationOAuthProvider( PublicClientId ),
				AuthorizeEndpointPath = new PathString( "/api/Account/ExternalLogin" ),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays( 14 ),
				// In production mode set AllowInsecureHttp = false
				AllowInsecureHttp = true
			};

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens( OAuthOptions );

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			//app.UseTwitterAuthentication(
			//   consumerKey: ConfigurationManager.AppSettings[ "D0vSEK2n6GJB89udErOlLVo4o"],
			//   consumerSecret: ConfigurationManager.AppSettings[" D9P7tLyDTVluRunSe0IYmYfvuvzSF8iybqrU06NroUE18WfHHT"]);

			//app.UseFacebookAuthentication(
			//    appId: "",
			//    appSecret: "");

			app.UseGoogleAuthentication( new GoogleOAuth2AuthenticationOptions()
			{
				ClientId = ConfigurationManager.AppSettings[ "GoogleClientId" ],
				ClientSecret = ConfigurationManager.AppSettings[ "GoogleClientSecret" ],
				CallbackPath = new PathString( "/google-login" )
			} );
		}
	}
}
