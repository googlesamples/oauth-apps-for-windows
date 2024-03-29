OAuth for Apps: Sample Universal Application for Windows
============

This sample shows how to do an OAuth 2.0 Authorization flow from a
Universal Windows Platform (UWP) app.  It is one of a
[series of OAuth samples](../README.md) for Windows.

Introduction
------------

When doing an OAuth 2.0 Authorization flow in a native application, it is 
important to follow best practices, which require using the browser (and not 
an embedded browser).

This sample demonstrates how you can open the user's browser with your OAuth 2.0
authorization request (where they might already be logged in!), have them
complete the consent, receive the Authorization Code using a URI scheme
registered by your app, and exchanging that code for authorization tokens.

Google Documentation
--------------------

The protocols referenced in this sample are documented here:

- [OAuth 2.0](https://developers.google.com/identity/protocols/OAuth2)
- [Using OAuth 2.0 for Mobile and Desktop Applications](https://developers.google.com/identity/protocols/OAuth2InstalledApp)

Getting Started
---------------

1. Open the solution file: `OAuthUniversalApp.sln`
2. Run the app your Local Device, or the device of your choosing.
3. When the app starts, tap "Sign in with Google" and go through the flow.


Using your own credentials
--------------------------

The Sample comes backed with some demo client credentials, which are fine for
testing, but make sure you use your own credentials before releasing any app,
or sharing it with friends.

1. Visit the [Credentials page of the Developers Console](https://console.developers.google.com/apis/credentials?project=_)
2. Create a new OAuth 2.0 client, select `iOS` (yes, it's a little strange to
   select iOS, but the way the OAuth client works with UWP is similar to iOS, 
   so this is currently the correct client type to create).
3. As your bundle ID, enter your domain name in reverse DNS notation. E.g.
   if your domain was "example.com", use "com.example" as your bundle ID.
   Note that your bundle ID MUST contain a period character `.`, and MUST be
   less than 39 characters long
4. Copy the created client-id and replace the clientID value in this sample
5. Edit the manifest by right-clicking and selecting "View Code" (due to a
   limitation of Visual Studio it wasn't possible to declare a URI scheme
   containing a period in the UI).
6. Find the "Protocol" scheme, and replace it with the bundle id you registered
   in step 3. (e.g. "com.example")

Support
-------

If you have a question related to these samples, or Google OAuth in general,
please ask on Stack Overflow with the `google-oauth` tag
 https://stackoverflow.com/questions/tagged/google-oauth

If you've found an error in this sample, please file an issue:
https://github.com/googlesamples/oauth-apps-for-windows/issues

Patches are encouraged, and may be submitted by forking this project and
submitting a pull request through GitHub.

Advanced Reading
----------------

The protocols and best practices used and implemented in these samples are
defined by RFCs. These expert-level documents detail how the protocols work,
and explain the reasoning behind many decisions, such as why we send a
`code_challenge` on the Authorization Request for a native app.

- [RFC 8252: OAuth 2.0 for Native Apps](https://tools.ietf.org/html/rfc8252)
- [RFC 6749: OAuth 2.0](https://tools.ietf.org/html/rfc6749)
- [RFC 6750: OAuth 2.0 Bearer Token Usage](https://tools.ietf.org/html/rfc6750)
- [RFC 6819: OAuth 2.0 Threat Model and Security Considerations](https://tools.ietf.org/html/rfc6819)
- [RFC 7636: OAuth 2.0 PKCE](https://tools.ietf.org/html/rfc7636)

License
-------

Copyright 2016 Google Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
