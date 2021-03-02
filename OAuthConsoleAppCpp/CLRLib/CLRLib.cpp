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

#using "CSharpLib.dll"

#include <msclr\auto_gcroot.h>

#include "CLRLib.h"

using System::String;
using System::Runtime::InteropServices::Marshal;

using namespace System;
using namespace System::Runtime::InteropServices;

namespace
{
	outputCallback currentCallback;

	void outputCallbackWrapper(String^ output)
	{
		auto outputPtr = Marshal::StringToHGlobalUni(output).ToPointer();

		currentCallback(static_cast<const wchar_t *>(outputPtr));

		Marshal::FreeHGlobal(static_cast<IntPtr>(outputPtr));
	}
}

namespace OAuthConsoleAppCpp
{
	class CSharpLibWrapper
	{
	public:
		msclr::auto_gcroot<CSharpLib^> csharpLib;
	};

	CLRLib::CLRLib()
	{
		wrapper = new CSharpLibWrapper();
		wrapper->csharpLib = gcnew CSharpLib();

		wrapper->csharpLib->output += gcnew CSharpLib::outputDelegate(outputCallbackWrapper);
	}

	CLRLib::~CLRLib()
	{
		wrapper->csharpLib->output -= gcnew CSharpLib::outputDelegate(outputCallbackWrapper);

		delete wrapper;
	}

	void CLRLib::printPrompt()
	{
		wrapper->csharpLib->printPrompt();
	}

	void CLRLib::performAuth(const char * clientID, const char * clientSecret)
	{
		wrapper->csharpLib->performAuth(gcnew String(clientID), gcnew String(clientSecret));
	}

	void CLRLib::setOutputCallback(outputCallback callback)
	{
		currentCallback = callback;
	}
}