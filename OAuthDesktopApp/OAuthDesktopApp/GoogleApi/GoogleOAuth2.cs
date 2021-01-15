using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OAuthDesktopApp.Utilities;

namespace OAuthDesktopApp.GoogleApi
{
	public class GoogleOAuth2 : GoogleApiBase
	{
		// client configuration
		private const string ClientID = "581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";
		private const string ClientSecret = "3f6NggMbPtrmIBpgx-MK2xXK";
		private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
		private const string TokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
		private const string ResponseString = "<html><head><title>Done</title><script>function loaded() { window.close(); }</script></head>" +
																					"<body onload='loaded()'>Please return to the app.<h4>If this window does not close automatically, please close manually.</h4></body></html>";
		private const string CodeChallengeMethod = "S256";

		public string AccessToken { get; private set; }

		public string RefreshToken { get; private set; }

		public GoogleOAuth2(Action<string> outputCallback) : base(outputCallback)
		{
		}

		public async Task PerformAuthenticationAsync(Action activateCallback)
		{
			// Clear any previous tokens
			this.AccessToken = null;
			this.RefreshToken = null;

			// Generates state and PKCE values.
			string state = OAuthHelpers.RandomDataBase64url(32);
			string codeVerifier = OAuthHelpers.RandomDataBase64url(32);
			string codeChallenge = OAuthHelpers.Base64urlencodeNoPadding(OAuthHelpers.Sha256(codeVerifier));

			// Creates a redirect URI using an available port on the loopback address.
			string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, OAuthHelpers.GetRandomUnusedPort());
			Output("redirect URI: " + redirectURI);

			// Creates an HttpListener to listen for requests on that redirect URI.
			var http = new HttpListener();
			http.Prefixes.Add(redirectURI);
			Output("Listening..");
			http.Start();

			// Creates the OAuth 2.0 authorization request.
			string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
					AuthorizationEndpoint,
					System.Uri.EscapeDataString(redirectURI),
					ClientID,
					state,
					codeChallenge,
					CodeChallengeMethod);

			// Opens request in the browser.
			System.Diagnostics.Process.Start(authorizationRequest);

			// Waits for the OAuth authorization response.
			var context = await http.GetContextAsync();

			// Brings this app back to the foreground.
			activateCallback?.Invoke();

			// Sends an HTTP response to the browser.
			var response = context.Response;

			var buffer = Encoding.UTF8.GetBytes(ResponseString);
			response.ContentLength64 = buffer.Length;
			var responseOutput = response.OutputStream;
			Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
			{
				responseOutput.Close();
				http.Stop();
				Console.WriteLine("HTTP server stopped.");
			});

			// Checks for errors.
			if (context.Request.QueryString.Get("error") != null)
			{
				throw new OAuthException(string.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
			}

			if (context.Request.QueryString.Get("code") == null
					|| context.Request.QueryString.Get("state") == null)
			{
				throw new OAuthException("Malformed authorization response. " + context.Request.QueryString);
			}

			// extracts the code
			var code = context.Request.QueryString.Get("code");
			var incoming_state = context.Request.QueryString.Get("state");

			// Compares the receieved state to the expected value, to ensure that
			// this app made the request which resulted in authorization.
			if (incoming_state != state)
			{
				throw new OAuthException(string.Format("Received request with invalid state ({0})", incoming_state));
			}

			Output("Authorization code: " + code);

			// Starts the code exchange at the Token Endpoint.
			await PerformCodeExchange(code, codeVerifier, redirectURI);
		}

		private async Task PerformCodeExchange(string code, string code_verifier, string redirectURI)
		{
			Output("Exchanging code for tokens...");

			// builds the request
			string tokenRequestURI = TokenEndpoint;
			string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
					code,
					System.Uri.EscapeDataString(redirectURI),
					ClientID,
					code_verifier,
					ClientSecret
					);

			// sends the request
			HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
			tokenRequest.Method = "POST";
			tokenRequest.ContentType = "application/x-www-form-urlencoded";
			tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
			tokenRequest.ContentLength = _byteVersion.Length;
			Stream stream = tokenRequest.GetRequestStream();
			await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
			stream.Close();

			try
			{
				// gets the response
				WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
				using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
				{
					// reads response body
					string responseText = await reader.ReadToEndAsync();
					Output(responseText);

					// converts to dictionary
					Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

					this.AccessToken = tokenEndpointDecoded["access_token"];
					this.RefreshToken = tokenEndpointDecoded["refresh_token"];
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					if (ex.Response is HttpWebResponse response)
					{
						Output("HTTP: " + response.StatusCode);
						using (StreamReader reader = new StreamReader(response.GetResponseStream()))
						{
							// reads response body
							string responseText = await reader.ReadToEndAsync();
							Output(responseText);
						}
					}
				}
			}
		}
	}
}
