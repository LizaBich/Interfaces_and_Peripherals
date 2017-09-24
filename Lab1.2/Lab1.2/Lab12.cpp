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
#include "PCI_Lib.h"

using namespace std;

vector<string> getInfo(HDEVINFO, SP_DEVINFO_DATA);
vector<_DEVICE_DESCRIPTION> getDescriptions(vector<string>);

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
	vector<_DEVICE_DESCRIPTION> devices = getDescriptions(getInfo(hDev, deviceInfo));

	if (devices.size() == 0) cout << "Not found." << endl;
	else cout << devices.size() << endl;

	for (int i = 0; i < devices.size(); i++)
	{
		cout << i; 
		cout << ") DeviceID: " << devices[i]._deviceID.c_str() << "; VendorID: " << devices[i]._vendorID.c_str()
			<< "\n  Description: " << devices[i]._name.c_str() << ", " << devices[i]._description.c_str() << "\n" << endl;
	}

	return 0;
}

vector<string> getInfo(HDEVINFO _hDev, SP_DEVINFO_DATA _deviceInfo)
{
	vector<string> rezult;

	for (DWORD i = 0; SetupDiEnumDeviceInfo(_hDev, i, &_deviceInfo); i++)
	{
		DWORD data;
		LPTSTR buffer = NULL;
		DWORD bufferSize = 0;

		while (!SetupDiGetDeviceRegistryProperty(_hDev, &_deviceInfo, SPDRP_HARDWAREID, &data, (PBYTE)buffer, bufferSize, &bufferSize))
		{
			if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
			{
				if (buffer) LocalFree(buffer);
				buffer = (LPTSTR)LocalAlloc(LPTR, bufferSize * 2);
			}
			else break;
		}

		rezult.push_back(string(buffer));

		if (buffer) LocalFree(buffer);
	}

	return rezult;
}

vector<_DEVICE_DESCRIPTION> getDescriptions(vector<string> _devices)
{
	vector<_DEVICE_DESCRIPTION> rezult;

	while (_devices.size() != 0)
	{
		string vendor = "0x" + _devices.back().substr(8, 4);
		string device = "0x" + _devices.back().substr(17, 4);
		_devices.pop_back();
		for (int i = 0; i < LENGTH; i++)
		{
			if (_devicesLib[i]._deviceID == device && _devicesLib[i]._vendorID == vendor)
			{
				rezult.push_back(_devicesLib[i]);
				break;
			}
		}
	}

	return rezult;
}