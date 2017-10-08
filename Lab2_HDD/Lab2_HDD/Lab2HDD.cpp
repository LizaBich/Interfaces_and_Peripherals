// Lab2HDD.cpp: определяет точку входа для консольного приложения.
//
#define _WIN32_DCOM
#include "stdafx.h"
#include <string>
#include <comdef.h>
#include <WbemIdl.h>
#include <Ntddscsi.h>

#pragma comment(lib, "wbemuuid.lib")

#define MAX_COUNT_OF_LOGICAL_DISKS 26
#define CODE_OF_A 97
//#define TO_GIGABYTE 1073741824
#define TO_GIGABYTE 1000000000

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
	bool pio;
};

bool GetInformationAboutDiscs(vector<HDD_DEVICE> *);
bool InitializeWMI(IWbemLocator **, IWbemServices **, IEnumWbemClassObject **, IWbemClassObject **);
bool CloseWMI(IWbemServices **, IEnumWbemClassObject **, IWbemClassObject **);
string& BstrToStdString(const BSTR bstr, string& dst, int cp = CP_ACP);
string BstrToStdString(BSTR bstr, int cp = CP_ACP);
unsigned long long GetDiskSize(const char *);
void GetDiskFreeMem(HDD_DEVICE **);
void PioOrDma(HDD_DEVICE **);
void PrintInfo(vector<HDD_DEVICE>);


int main()
{
	vector<HDD_DEVICE> devices;

	PrintInfo(devices);

	return 0;
}


void PrintInfo(vector<HDD_DEVICE> devices)
{
	if (!GetInformationAboutDiscs(&devices))
	{
		cout << "Error in app." << endl;
		return;
	}

	for (int i = 0; i < devices.size(); ++i)
	{
		cout << "\nModel: " << devices[i].model.c_str() << endl
			<< "Firmware version: " << devices[i].firmwareRevision.c_str() << endl
			<< "Serial number: " << devices[i].serialNumber.c_str() << endl
			<< "Memory(free/is used/total): " << devices[i].freeSpace / TO_GIGABYTE << " / " << devices[i].totalSpace / TO_GIGABYTE - devices[i].freeSpace / TO_GIGABYTE << " / " << devices[i].totalSpace / TO_GIGABYTE << endl
			<< "Interface type: " << devices[i].conInterface.c_str() << endl;
		if (devices[i].pio) {
			cout << "Transfer mode: PIO" << endl;
		}
		else {
			cout << "Transfer mode: DMA" << endl;
		}
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

		dev->totalSpace = GetDiskSize(dev->deviceID.c_str());
		GetDiskFreeMem(&dev);

		PioOrDma(&dev);

		devices->push_back(*dev);
		delete dev;
	}
	CloseWMI(&pLoc, &pSvc, &pEnumerator, &pclsObj);
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

void GetDiskFreeMem(HDD_DEVICE **dev)
{
	DWORD dr = GetLogicalDrives();
	const string BASE_DRIVE_NAME = "\\\\.\\PHYSICALDRIVE";
	(*dev)->freeSpace = 0;
	for (int i = 0; i < MAX_COUNT_OF_LOGICAL_DISKS; i++)
	{
		if ((dr >> i) & 1) {
			ULONGLONG FreeBytesAvailable, TotalNumberOfBytes, TotalNumberOfFreeBytes;
			wstring directory = L" :\\\0";
			directory[0] = (char)(CODE_OF_A + i);
			bool rez = GetDiskFreeSpaceEx(directory.c_str(), (PULARGE_INTEGER)&FreeBytesAvailable, (PULARGE_INTEGER)&TotalNumberOfBytes, (PULARGE_INTEGER)&TotalNumberOfFreeBytes);
			if (rez == false) continue;
			directory = L"\\\\.\\ :";
			directory[4] = (char)(CODE_OF_A + i);
			HANDLE h = CreateFile(directory.c_str(), 0, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
			if (h == INVALID_HANDLE_VALUE) continue;
			LPCTSTR szPhysical;
			STORAGE_DEVICE_NUMBER sd = { 0 };
			DWORD dwRet;
			if (DeviceIoControl(h, IOCTL_STORAGE_GET_DEVICE_NUMBER, NULL, 0, &sd, sizeof(STORAGE_DEVICE_NUMBER), &dwRet, NULL))
			{
				string name = BASE_DRIVE_NAME + to_string(sd.DeviceNumber) + "\0";
				if (strcmp(name.c_str(), (*dev)->deviceID.c_str()) == 0)
				{
					(*dev)->freeSpace += TotalNumberOfFreeBytes;
				}
			}
			CloseHandle(h);
		}
	}
}

void PioOrDma(HDD_DEVICE **dev)
{
	HANDLE device = CreateFileA((LPCSTR)(*dev)->deviceID.c_str(), 0, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	if (device == INVALID_HANDLE_VALUE)
		return;

	STORAGE_PROPERTY_QUERY query = {};
	query.PropertyId = StorageAdapterProperty;
	query.QueryType = PropertyStandardQuery;

	STORAGE_ADAPTER_DESCRIPTOR descriptor = {};

	DWORD read;
	if (!DeviceIoControl(device, IOCTL_STORAGE_QUERY_PROPERTY,
		&query,
		sizeof(query),
		&descriptor,
		sizeof(descriptor),
		&read,
		NULL
	))
	{
		cout << "Error: " << GetLastError() << endl;
		return;
	}
	else
	{
		(*dev)->pio = descriptor.AdapterUsesPio;
	}

	CloseHandle(device);
	return;
}