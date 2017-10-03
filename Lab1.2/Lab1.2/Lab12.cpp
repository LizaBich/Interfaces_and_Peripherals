// Lab12.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"

#pragma comment (lib, "Setupapi.lib")
#include <Windows.h>
#include <stdio.h>
#include <SetupAPI.h>
#include <devguid.h>
#include <RegStr.h>
#include <vector>
#include <string.h>
#include <locale.h>
#include <conio.h>
#include "PCI_Lib.h"

using namespace std;

vector<string> getInfo(HDEVINFO, SP_DEVINFO_DATA);
vector<PCI_DEVTABLE> getDescriptions(vector<string>);

int main(int argc, char *argv[])
{
	setlocale(LC_ALL, "Russian");

	HDEVINFO hDev;
	SP_DEVINFO_DATA deviceInfo;

	hDev = SetupDiGetClassDevs(NULL, REGSTR_KEY_PCIENUM, 0, DIGCF_PRESENT | DIGCF_ALLCLASSES);
	if (hDev == INVALID_HANDLE_VALUE)
	{
		return 1;
	}

	deviceInfo.cbSize = sizeof(SP_DEVINFO_DATA);
	vector<PCI_DEVTABLE> devices = getDescriptions(getInfo(hDev, deviceInfo));

	for (int i = 0; i < devices.size(); i++)
	{
		cout << i + 1; 
		cout << ") DeviceID: " << devices[i]._deviceID.c_str() << "; VendorID: " << devices[i]._vendorID.c_str()
			<< "\n  Description: " << devices[i]._name.c_str() << ", " << devices[i]._description.c_str() << "\n" << endl;
	}

	_getch();

	return 0;
}

vector<string> getInfo(HDEVINFO _hDev, SP_DEVINFO_DATA _deviceInfo)
{
	vector<string> result;

	for (DWORD i = 0; SetupDiEnumDeviceInfo(_hDev, i, &_deviceInfo); i++)
	{
		LPTSTR buffer = NULL;
		DWORD bufferSize = 0;

		while (!SetupDiGetDeviceRegistryProperty(_hDev, &_deviceInfo, SPDRP_HARDWAREID, NULL, (PBYTE)buffer, bufferSize, &bufferSize))
		{
			if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
			{
				if (buffer) LocalFree(buffer);
				buffer = (LPTSTR)LocalAlloc(LPTR, bufferSize * 2);
			}
			else break;
		}

		result.push_back(string(buffer));

		if (buffer) LocalFree(buffer);
	}

	return result;
}

vector<PCI_DEVTABLE> getDescriptions(vector<string> _devices)
{
	vector<PCI_DEVTABLE> result;
	DevicesLib *lib = new DevicesLib();
	
	while (_devices.size() != 0)
	{
		string vendor = "0x" + _devices.back().substr(8, 4);
		string device = "0x" + _devices.back().substr(17, 4);
		_devices.pop_back();
		PCI_DEVTABLE temp;
		temp._vendorID = vendor;
		temp._deviceID = device;
		result.push_back(temp);
	}
	lib->GetVendorAndDeviceNames(&result);
	return result;
}