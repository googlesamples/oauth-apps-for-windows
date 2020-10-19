// Copyright 2020 Wiktor Lawski <wiktor.lawski@gmail.com>
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

#pragma once

typedef void(__cdecl *outputCallback)(const wchar_t *);

namespace OAuthConsoleAppCpp
{
	class CSharpLibWrapper;

	class _declspec(dllexport) CLRLib
	{
	public:
		CLRLib();
		~CLRLib();

		void printPrompt();
		void performAuth(const char * clientID, const char * clientSecret);

		static void setOutputCallback(outputCallback callback);

	private:
		CSharpLibWrapper* wrapper;
	};
}
