using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace OAuthDesktopApp.GoogleApi
{
	public class ApiCalls : GoogleApiBase
	{
		private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

		public ApiCalls(Action<string> outputCallback) : base(outputCallback)
		{
		}

		public async Task<string> UserinfoCallAsync(GoogleOAuth2 oAuth2)
		{
			Output("Making API Call to Userinfo...");

			// builds the  request
			string userinfoRequestURI = UserInfoEndpoint;

			// sends the request
			HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
			userinfoRequest.Method = "GET";
			userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", oAuth2.AccessToken));
			userinfoRequest.ContentType = "application/x-www-form-urlencoded";
			userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

			// gets the response
			WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
			using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
			{
				// reads response body
				string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
				Output(userinfoResponseText);

				return userinfoResponseText;
			}
		}

		// TODO: add refresh token code

		// TODO: make calling code generic
	}
}
