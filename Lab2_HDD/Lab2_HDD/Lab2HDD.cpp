// Lab2HDD.cpp: определяет точку входа для консольного приложения.
//
#define _WIN32_DCOM
#include "stdafx.h"
#include <comdef.h>
#include <WbemIdl.h>

#pragma comment(lib, "wbemuuid.lib")

using namespace std;

struct HDD_DEVICE
{
	string model;
	string serialNumber;
	string firmwareRevision;
	string deviceID;
	unsigned long long totalSpace;
	unsigned long long freeSpace;
};

bool GetInformationAboutDiscs(vector<HDD_DEVICE> *);
bool InitializeWMI(IWbemLocator *, IWbemServices *, IEnumWbemClassObject *, IWbemClassObject *);
bool CloseWMI(IWbemServices *, IEnumWbemClassObject *, IWbemClassObject *);
string& BstrToStdString(const BSTR bstr, string& dst, int cp = CP_ACP);
// conversion with temp.
string BstrToStdString(BSTR bstr, int cp = CP_ACP);
unsigned long long GetDiskSize(const char *);
unsigned long long GetDiskFreeMem(const char *);

int main()
{
	setlocale(LC_ALL, "Russian");

	vector<HDD_DEVICE> devices;

	if (!GetInformationAboutDiscs(&devices))
	{
		cout << "Error in app." << endl;
		return 0;
	}

	for (int i = 0; i < devices.size(); ++i)
	{
		cout << "\nМодель: " << devices[i].model.c_str() << endl
			<< "Версия прошивки: " << devices[i].firmwareRevision.c_str() << endl
			<< "Серийный номер: " << devices[i].serialNumber.c_str() << endl
			<< "Память(свободно/занято/всего): " << devices[i].freeSpace << "/" << devices[i].totalSpace - devices[i].freeSpace << "/" << devices[i].totalSpace << endl << endl;
	}


    return 0;
}

bool InitializeWMI(IWbemLocator *pLoc, IWbemServices *pSvc, IEnumWbemClassObject *pEnum, IWbemClassObject *pclsObj)
{
	HRESULT hRes = CoInitializeEx(0, COINIT_MULTITHREADED);
	if (FAILED(hRes))
	{
		return false;
	}

	hRes = CoInitializeSecurity(NULL,
		-1,                          // COM authentication
		NULL,                        // Authentication services
		NULL,                        // Reserved
		RPC_C_AUTHN_LEVEL_DEFAULT,   // Default authentication 
		RPC_C_IMP_LEVEL_IMPERSONATE, // Default Impersonation  
		NULL,                        // Authentication info
		EOAC_NONE,                   // Additional capabilities 
		NULL);
	if (FAILED(hRes))
	{
		CoUninitialize();
		return false;
	}

	hRes = CoCreateInstance(
		CLSID_WbemLocator,
		0,
		CLSCTX_INPROC_SERVER,
		IID_IWbemLocator, (LPVOID *)&pLoc);
	if (FAILED(hRes))
	{
		CoUninitialize();
		return false;
	}

	hRes = pLoc->ConnectServer(
		_bstr_t(L"ROOT\\CIMV2"), // Object path of WMI namespace
		NULL,                    // User name. NULL = current user
		NULL,                    // User password. NULL = current
		0,                       // Locale. NULL indicates current
		NULL,                    // Security flags.
		0,                       // Authority (for example, Kerberos)
		0,                       // Context object 
		&pSvc                    // pointer to IWbemServices proxy
	);
	if (FAILED(hRes))
	{
		CoUninitialize();
		return false;
	}

	hRes = CoSetProxyBlanket(
		pSvc,                        // Indicates the proxy to set
		RPC_C_AUTHN_WINNT,           // RPC_C_AUTHN_xxx
		RPC_C_AUTHZ_NONE,            // RPC_C_AUTHZ_xxx
		NULL,                        // Server principal name 
		RPC_C_AUTHN_LEVEL_CALL,      // RPC_C_AUTHN_LEVEL_xxx 
		RPC_C_IMP_LEVEL_IMPERSONATE, // RPC_C_IMP_LEVEL_xxx
		NULL,                        // client identity
		EOAC_NONE                    // proxy capabilities 
	);
	if (FAILED(hRes))
	{
		pSvc->Release();
		pLoc->Release();
		CoUninitialize();
		return false;
	}
	return true;
}

bool CloseWMI(IWbemLocator *pLoc, IWbemServices *pSvc, IEnumWbemClassObject *pEnum, IWbemClassObject *pclsObj)
{
	pLoc->Release();
	pSvc->Release();
	pEnum->Release();
	pclsObj->Release();
	CoUninitialize();
	return true;
}

bool GetInformationAboutDiscs(vector<HDD_DEVICE> *devices)
{
	IWbemLocator *pLoc = NULL;
	IWbemServices *pSvc = NULL;
	IEnumWbemClassObject *pEnumerator = NULL;
	IWbemClassObject *pclsObj = NULL;
	ULONG uReturn = 0;
	if (!InitializeWMI(pLoc, pSvc, pEnumerator, pclsObj))
	{
		return false;
	}

	string str = "SELECT * FROM Win32_DiskDrive";

	HRESULT hRes = pSvc->ExecQuery(
		bstr_t("WQL"),
		bstr_t(str.c_str()),
		WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
		NULL,
		&pEnumerator);
	if (FAILED(hRes))
	{
		pSvc->Release();
		pLoc->Release();
		CoUninitialize();
		return false;
	}

	while (pEnumerator)
	{
		HDD_DEVICE dev;

		hRes = pEnumerator->Next(WBEM_INFINITE, 1,
			&pclsObj, &uReturn);
		if (0 == uReturn)
		{
			break;
		}

		VARIANT vtProp;

		hRes = pclsObj->Get(L"FirmwareRevision", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev.firmwareRevision = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"Model", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev.model = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"SerialNumber", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev.serialNumber = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"DeviceID", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev.deviceID = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);
		dev.deviceID.substr(4).insert(0,"\\\\.\\");

		dev.totalSpace = GetDiskSize(dev.deviceID.c_str());
		dev.freeSpace = GetDiskFreeMem(dev.deviceID.c_str());

		devices->push_back(dev);
	}
	CloseWMI(pLoc, pSvc, pEnumerator, pclsObj);
	return true;
}

string& BstrToStdString(const BSTR bstr, string& dst, int cp)
{
	if (!bstr)
	{
		// define NULL functionality. I just clear the target.
		dst.clear();
		return dst;
	}

	// request content length in single-chars through a terminating
	//  nullchar in the BSTR. note: BSTR's support imbedded nullchars,
	//  so this will only convert through the first nullchar.
	int res = WideCharToMultiByte(cp, 0, bstr, -1, NULL, 0, NULL, NULL);
	if (res > 0)
	{
		dst.resize(res);
		WideCharToMultiByte(cp, 0, bstr, -1, &dst[0], res, NULL, NULL);
	}
	else
	{    // no content. clear target
		dst.clear();
	}
	return dst;
}

string BstrToStdString(BSTR bstr, int cp)
{
	string str;
	BstrToStdString(bstr, str, cp);
	return str;
}

unsigned long long GetDiskSize(const char *drive)
{
	DISK_GEOMETRY pdn = { 0 };
	HANDLE hDevice;
	BOOL bResult;
	DWORD junk;

	hDevice = CreateFileA((LPCSTR)drive, 0, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);

	if (hDevice == INVALID_HANDLE_VALUE)
		return 0;

	bResult = DeviceIoControl(hDevice, IOCTL_DISK_GET_DRIVE_GEOMETRY, NULL, 0, &pdn, sizeof(pdn), &junk, (LPOVERLAPPED)NULL);
	if (!bResult) return 0;
	CloseHandle(hDevice);

	//ULONGLONG DiskSize = pdn.Cylinders.QuadPart * (ULONG)pdn.TracksPerCylinder * (ULONG)pdn.SectorsPerTrack * (ULONG)pdn.BytesPerSector;

	return pdn.Cylinders.QuadPart * (ULONG)pdn.TracksPerCylinder * (ULONG)pdn.SectorsPerTrack * (ULONG)pdn.BytesPerSector;
}

unsigned long long GetDiskFreeMem(const char *name)
{
	DWORD FreeBytesAvailable, TotalNumberOfBytes, TotalNumberOfFreeBytes;
	bool rez = GetDiskFreeSpaceEx((LPCWSTR)name, (PULARGE_INTEGER)&FreeBytesAvailable, (PULARGE_INTEGER)&TotalNumberOfBytes, (PULARGE_INTEGER)&TotalNumberOfFreeBytes);
	if (rez == false) return 0;
	else {
		return TotalNumberOfFreeBytes;
	}
}