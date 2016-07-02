OAuth for Apps: Samples for Windows
============

This repository contains samples for doing OAuth 2.0 to Google for Windows apps,
including universal apps, traditional desktop apps, and CLI tools.

Introduction
------------

When doing an OAuth 2.0 Authorization flow to Google in a native application, it
is important to follow 
[best practices](https://tools.ietf.org/html/draft-ietf-oauth-native-apps), 
which require using the browser (and not an embedded browser).

These samples show how to complete an OAuth 2.0 Authorization request in a
traditional app, where a loopback redirect is used to received the code, and in
a universal app where a URI scheme is used for the same.

Samples
-------

[OAuthUniversalApp](OAuthUniversalApp/README.md) - Universal Windows Platform 
(UWP) sample app

[OAuthDesktopApp](OAuthDesktopApp/README.md) - Traditional desktop  
application sample (using WPF).

[OAuthConsoleApp](OAuthConsoleApp/README.md) - Command Line Interface (CLI)
console application sample.

All samples achieve the same end result of authenticating the user in the
system browser, but with environment-specific optimizations.

Google Documentation
--------------------

The protocols referenced in this sample are documented here:

- [OAuth 2.0](https://developers.google.com/identity/protocols/OAuth2)
- [Using OAuth 2.0 for Mobile and Desktop Applications](https://developers.google.com/identity/protocols/OAuth2InstalledApp)

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

- [Internet-Draft: OAuth 2.0 for Native Apps BCP](https://tools.ietf.org/html/draft-ietf-oauth-native-apps)
- [RFC6749: OAuth 2.0](https://tools.ietf.org/html/rfc6749)
- [RFC6750: OAuth 2.0 Bearer Token Usage](https://tools.ietf.org/html/rfc6750)
- [RFC6819: OAuth 2.0 Threat Model and Security Considerations](https://tools.ietf.org/html/rfc6819)
- [RFC7636: OAuth 2.0 PKCE](https://tools.ietf.org/html/rfc7636)

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
