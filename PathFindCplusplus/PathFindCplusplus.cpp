// PathFindCplusplus.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include <windows.h>

struct Vector2
{
	float x, y;
};
unsigned char* g_data = nullptr;
int g_data_length = 0;
int data_width = 0;
int data_height = 0;

bool isWalkable(const Vector2& v)
{
	int x = (int)v.x;
	int y = (int)v.y;
	int id = y * data_width + x;
	if (id < 0 || id >= g_data_length)
	{
		return false;
	}
	return g_data[id] != 0;
}

extern "C"
{

	__declspec(dllexport) bool __cdecl InitCPP(int w,int h,void* pathdata, int length)
	{
		if (pathdata == nullptr)
		{
			return false;
		}
		if (length <= 0)
		{
			return false;
		}
		data_width = w;
		data_height = h;
		if (g_data != nullptr)
		{
			delete g_data;
			g_data = nullptr;
		}
		g_data = new unsigned char[length];
		memcpy(g_data, pathdata, length);
		g_data_length = length;

		return true;
	}
	__declspec(dllexport) bool __cdecl ReleaseCPP()
	{
		if (g_data != nullptr)
		{
			delete g_data;
			g_data = nullptr;
		}
		g_data_length = 0;
		data_width = 0;
		data_height = 0;
		return true;
	}
	__declspec(dllexport) bool __cdecl FindCPP(Vector2& start, Vector2& end, Vector2* outPath,int* pathCount)
	{
		OutputDebugString(L"FindCPP Start");
		*pathCount = 0;
		if (g_data_length == 0)
		{
			return false;
		}
		if (g_data == nullptr)
		{
			return false;
		}
		return false;
	}
}