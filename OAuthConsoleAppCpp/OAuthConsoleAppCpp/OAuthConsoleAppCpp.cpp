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

#include <cstdio>

#include "CLRLib.h"

using OAuthConsoleAppCpp::CLRLib;

void output(const char * output)
{
	printf("%s\n", output);
}

int main()
{
	const auto clientID = "581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";
	const auto clientSecret = "3f6NggMbPtrmIBpgx-MK2xXK";

	CLRLib::setOutputCallback(output);

	CLRLib clrLib;

	clrLib.printPrompt();
	clrLib.performAuth(clientID, clientSecret);

	getchar();

    return 0;
}