#include "stdafx.h"
#include "PCI_Lib.h"

using namespace std;

DevicesLib::DevicesLib()
{
}

DevicesLib::~DevicesLib()
{
}

void DevicesLib::GetVendorAndDeviceNames(vector<PCI_DEVTABLE> *devices)
{
	char *temp = (char *)malloc(140 * sizeof(char));
	file.open(FILE_PATH, ios_base::in);
	if (!file.is_open())
	{
		cout << "\nНевозможно открыть файл " << FILE_PATH << "!" << endl;
		return;
	}
	int i = 0;
	while (!file.eof())
	{ 
		if (file.eof()) break;
		file.getline(temp, 140);
		
		if (temp[0] == '#' || temp[0] == '\n') continue;
		if (temp[0] != '\t') 
		{
			FindEqualVendor(string(temp), devices);
		}
		else if (temp[0] == '\t' && temp[1] != '\t') {
			FindEqualDevice(string(temp), devices);
		}
		if (i == 29477) break;
		i++;
	}
	file.close();
	return;
}

void DevicesLib::FindEqualVendor(string str, vector<PCI_DEVTABLE> *devices)
{
	if (str.empty()) return;
	bool flag = false;
	for (int i = 0; i < 4; ++i)
	{

		if (str[i] >= 'a' && str[i] <= 'z')
		{
			str[i] -= 32;
		}
	}

	for (int k = 0; k < devices->size(); ++k)
	{

		if (str.substr(0, 4) == (*devices)[k]._vendorID.substr(2, 4)) {
			(*devices)[k]._description = str.substr(4);
		}
	}

	return;
}

void DevicesLib::FindEqualDevice(string str, vector<PCI_DEVTABLE> *devices)
{
	if (str.empty()) return;
	bool flag = false;

	for (int i = 1; i < 5; ++i)
	{
		
		if (str[i] >= 'a' && str[i] <= 'z')
		{
			str[i] -= 32;
		}
	}

	for (int k = 0; k < devices->size(); k++)
	{

		if (str.substr(1, 4) == (*devices)[k]._deviceID.substr(2, 4))
		{
			(*devices)[k]._name = str.substr(5);
		}
	}
	return;
}
