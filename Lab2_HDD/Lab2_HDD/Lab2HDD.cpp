// Lab2HDD.cpp: определяет точку входа для консольного приложения.
//
#define _WIN32_DCOM
#include "stdafx.h"
#include <string>
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
	string conInterface;
	LONG *pVal;
	long count;
};

bool GetInformationAboutDiscs(vector<HDD_DEVICE> *);
bool InitializeWMI(IWbemLocator **, IWbemServices **, IEnumWbemClassObject **, IWbemClassObject **);
bool CloseWMI(IWbemServices **, IEnumWbemClassObject **, IWbemClassObject **);
string& BstrToStdString(const BSTR bstr, string& dst, int cp = CP_ACP);
// conversion with temp.
string BstrToStdString(BSTR bstr, int cp = CP_ACP);
unsigned long long GetDiskSize(const char *);
void GetDiskFreeMem(HDD_DEVICE **);
long GetSafeArray(LONG **, SAFEARRAY **);
void OutputCapabilities(LONG **);

int main()
{
	vector<HDD_DEVICE> devices;

	if (!GetInformationAboutDiscs(&devices))
	{
		cout << "Error in app." << endl;
		return 0;
	}

	for (int i = 0; i < devices.size(); ++i)
	{
		cout << "\nModel: " << devices[i].model.c_str() << endl
			<< "Firmware version: " << devices[i].firmwareRevision.c_str() << endl
			<< "Serial number: " << devices[i].serialNumber.c_str() << endl
			<< "Memory(free/is used/total): " << devices[i].freeSpace << "/" << devices[i].totalSpace - devices[i].freeSpace << "/" << devices[i].totalSpace << endl 
			<< "Interface type: " << devices[i].conInterface.c_str() << endl << endl;
	}

	return 0;
}

void OutputCapabilities(LONG **pVal, long count)
{
	for (int j = 0; j < count; ++j)
	{
		LONG lVal = (*pVal)[j];
		switch (lVal)
		{

		}
		cout << ", ";
	}
}

bool InitializeWMI(IWbemLocator **pLoc, IWbemServices **pSvc, IEnumWbemClassObject **pEnum, IWbemClassObject **pclsObj)
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
		IID_IWbemLocator, (LPVOID *)&(*pLoc));
	if (FAILED(hRes))
	{
		CoUninitialize();
		return false;
	}

	hRes = (*pLoc)->ConnectServer(
		_bstr_t(L"ROOT\\CIMV2"), // Object path of WMI namespace
		NULL,                    // User name. NULL = current user
		NULL,                    // User password. NULL = current
		0,                       // Locale. NULL indicates current
		NULL,                    // Security flags.
		0,                       // Authority (for example, Kerberos)
		0,                       // Context object 
		&(*pSvc)                    // pointer to IWbemServices proxy
	);
	if (FAILED(hRes))
	{
		CoUninitialize();
		return false;
	}

	hRes = CoSetProxyBlanket(
		*pSvc,                        // Indicates the proxy to set
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
		(*pSvc)->Release();
		(*pLoc)->Release();
		CoUninitialize();
		return false;
	}
	return true;
}

bool CloseWMI(IWbemLocator **pLoc, IWbemServices **pSvc, IEnumWbemClassObject **pEnum, IWbemClassObject **pclsObj)
{
	(*pLoc)->Release();
	(*pSvc)->Release();
	(*pEnum)->Release();
	(*pclsObj)->Release();
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
	if (!InitializeWMI(&pLoc, &pSvc, &pEnumerator, &pclsObj))
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
		hRes = pEnumerator->Next(WBEM_INFINITE, 1,
			&pclsObj, &uReturn);
		if (0 == uReturn)
		{
			break;
		}

		HDD_DEVICE *dev = new HDD_DEVICE();
		VARIANT vtProp;

		hRes = pclsObj->Get(L"FirmwareRevision", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->firmwareRevision = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"Model", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->model = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"SerialNumber", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->serialNumber = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"DeviceID", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->deviceID = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"InterfaceType", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->conInterface = BstrToStdString(vtProp.bstrVal);
		VariantClear(&vtProp);

		hRes = pclsObj->Get(L"Capabilities", 0, &vtProp, 0, 0);
		if (FALSE(hRes)) cout << "\nInvalid getting of value!" << endl;
		dev->pVal = NULL;
		dev->count = 0;
		dev->count = GetSafeArray(&(dev->pVal), &vtProp.parray);
		VariantClear(&vtProp);

		dev->totalSpace = GetDiskSize(dev->deviceID.c_str());
		GetDiskFreeMem(&dev);

		devices->push_back(*dev);
		delete dev;
	}
	CloseWMI(&pLoc, &pSvc, &pEnumerator, &pclsObj);
	return true;
}

long GetSafeArray(LONG **pVal, SAFEARRAY **capabilities)
{
	HRESULT hr = SafeArrayAccessData(*capabilities, (void**)pVal);
	if (FAILED(hr)) return 0;
	long lowerBound, upperBound;
	SafeArrayGetLBound(*capabilities, 1, &lowerBound);
	SafeArrayGetUBound(*capabilities, 1, &upperBound);
	return upperBound - lowerBound + 1;
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

void GetDiskFreeMem(HDD_DEVICE **dev)
{
	DWORD dr = GetLogicalDrives();
	for (int i = 0; i < 26; i++)
	{
		if ((dr >> i) & 1) {
			ULONGLONG FreeBytesAvailable, TotalNumberOfBytes, TotalNumberOfFreeBytes;
			string directory = " :\\\0";
			directory[0] = (char)(65 + i);
			bool rez = GetDiskFreeSpaceEx((LPCWSTR)directory.c_str(), (PULARGE_INTEGER)&FreeBytesAvailable, (PULARGE_INTEGER)&TotalNumberOfBytes, (PULARGE_INTEGER)&TotalNumberOfFreeBytes);
			if (rez == false) break;
			HANDLE h = CreateFile((LPCWSTR)directory.c_str(), 0, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
			if (h == INVALID_HANDLE_VALUE) break;
			LPCTSTR szPhysical;
			STORAGE_DEVICE_NUMBER sd = { 0 };
			DWORD dwRet;
			if (DeviceIoControl(h, IOCTL_STORAGE_GET_DEVICE_NUMBER, NULL, 0, &sd, sizeof(STORAGE_DEVICE_NUMBER), &dwRet, NULL))
			{
				if (string("\\\\.\\PHYSICALDRIVE" + sd.DeviceNumber) == (*dev)->deviceID)
				{
					(*dev)->freeSpace += TotalNumberOfFreeBytes;
				}
			}
			CloseHandle(h);
		}
	}
}