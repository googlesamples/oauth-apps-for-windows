// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace OAuthAppUniversalScheme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>s
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// OAuth 2.0 client configuration.
        /// </summary>
        const string clientID = "581786658708-r4jimt0msgjtp77b15lonfom92ko6aeg.apps.googleusercontent.com";
        const string redirectURI = "pw.oauth2:/oauth2redirect";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts an OAuth 2.0 Authorization Request.
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Stores the state and code_verifier values into local settings.
            // Member variables of this class may not be present when the app is resumed with the
            // authorization response, so LocalSettings can be used to persist any needed values.
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["state"] = state;
            localSettings.Values["code_verifier"] = code_verifier;

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI), 
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            output("Opening authorization request URI: " + authorizationRequest);

            // Opens the Authorization URI in the browser.
            var success = Windows.System.Launcher.LaunchUriAsync(new Uri(authorizationRequest));
        }

        /// <summary>
        /// Processes the OAuth 2.0 Authorization Response
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Uri)
            {
                // Gets URI from navigation parameters.
                Uri authorizationResponse = (Uri)e.Parameter;
                string queryString = authorizationResponse.Query;
                output("MainPage received authorizationResponse: " + authorizationResponse);

                // Parses URI params into a dictionary
                // ref: http://stackoverflow.com/a/11957114/72176
                Dictionary<string, string> queryStringParams =
                        queryString.Substring(1).Split('&')
                             .ToDictionary(c => c.Split('=')[0],
                                           c => Uri.UnescapeDataString(c.Split('=')[1]));

                if (queryStringParams.ContainsKey("error"))
                {
                    output(String.Format("OAuth authorization error: {0}.", queryStringParams["error"]));
                    return;
                }

                if (!queryStringParams.ContainsKey("code")
                    || !queryStringParams.ContainsKey("state"))
                {
                    output("Malformed authorization response. " + queryString);
                    return;
                }

                // Gets the Authorization code & state
                string code = queryStringParams["code"];
                string incoming_state = queryStringParams["state"];

                // Retrieves the expected 'state' value from local settings (saved when the request was made).
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                string expected_state = (String) localSettings.Values["state"];

                // Compares the receieved state to the expected value, to ensure that
                // this app made the request which resulted in authorization
                if (incoming_state != expected_state)
                {
                    output(String.Format("Received request with invalid state ({0})", incoming_state));
                    return;
                }

                // Resets expected state value to avoid a replay attack.
                localSettings.Values["state"] = null;

                // Authorization Code is now ready to use!
                output(Environment.NewLine + "Authorization code: " + code);

                string code_verifier = (String)localSettings.Values["code_verifier"];
                performCodeExchangeAsync(code, code_verifier);
            }
            else
            {
                Debug.WriteLine(e.Parameter);
            }
        }

        async void performCodeExchangeAsync(string code, string code_verifier)
        {
            // Builds the Token request
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier
                );
            StringContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Performs the authorization code exchange.
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            HttpClient client = new HttpClient(handler);

            output(Environment.NewLine + "Exchanging code for tokens...");
            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, content);
            string responseString = await response.Content.ReadAsStringAsync();
            output(responseString);

            if (!response.IsSuccessStatusCode)
            {
                output("Authorization code exchange failed.");
                return;
            }

            // Sets the Authentication header of our HTTP client using the acquired access token.
            JsonObject tokens = JsonObject.Parse(responseString);
            string accessToken = tokens.GetNamedString("access_token");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Makes a call to the Userinfo endpoint, and prints the results.
            output("Making API Call to Userinfo...");
            HttpResponseMessage userinfoResponse = client.GetAsync(userInfoEndpoint).Result;
            string userinfoResponseContent = await userinfoResponse.Content.ReadAsStringAsync();
            output(userinfoResponseContent);
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string output)
        {
            textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Debug.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        public static string randomDataBase64url(uint length)
        {
            IBuffer buffer = CryptographicBuffer.GenerateRandom(length);
            return base64urlencodeNoPadding(buffer);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static IBuffer sha256(string inputString)
        {
            HashAlgorithmProvider sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);
            return sha.HashData(buff);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string base64urlencodeNoPadding(IBuffer buffer)
        {
            string base64 = CryptographicBuffer.EncodeToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
    }
}
