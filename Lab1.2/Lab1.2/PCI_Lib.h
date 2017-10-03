#pragma once
#define FILE_PATH "pci.ids.txt"
#include <iostream>
#include <fstream>
#include <cstdio>
#include <string>
#include <vector>

using namespace std;

struct PCI_DEVTABLE
{
public:
	string _deviceID;
	string _vendorID;
	string _name;
	string _description;
};

class DevicesLib
{
public:
	DevicesLib();
	~DevicesLib();

	void GetVendorAndDeviceNames(vector<PCI_DEVTABLE>*);

private:
	fstream file;

	void FindEqualVendor(string, vector<PCI_DEVTABLE>*);
	void FindEqualDevice(string, vector<PCI_DEVTABLE>*);
};