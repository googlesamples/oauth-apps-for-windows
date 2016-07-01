OAuth for Apps: Samples for Windows
============

This repository contains samples for doing OAuth 2.0 to Google for Windows apps,
including traditional and universal apps.

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

If you are building a Universal Windows Platform (UWP) app, view the
[OAuthUniversalApp](OAuthUniversalApp/README.md) sample.


If you are building a traditional desktop Windows application, view the
[OAuthDesktopApp](OAuthDesktopApp/README.md) sample.

Both samples achieve the same end result, but in a slightly different way.

Pre-requisites
--------------

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
