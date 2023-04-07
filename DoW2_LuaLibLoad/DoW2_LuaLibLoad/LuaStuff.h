#include "LuaConfig.h"

/* Lua internals */
struct CClosure
{
  void *next;
  unsigned char tt;
  unsigned char marked;
  unsigned char isC;
  unsigned char nupvalues;
  void *gclist;
  void *env;
  lua_CFunction f;
};

/* Functions to be called by Lua */
int __stdcall luaLoadfileStandard(lua_State *L)
{
  const char *filename = luaL_optstring(L, 1, NULL);
  if(luaL_loadfile(L, filename) == 0)
  {
    return 1;
  }

  lua_pushnil(L);
  lua_insert(L, -2);
  return 2;
}

int __stdcall luaLoadlib(lua_State *L)
{
  const char *path = luaL_checkstring(L,1);
  const char *init = luaL_checkstring(L,2);
  wchar_t tmpLib[MAX_PATH];
  mbstowcs_s(NULL, tmpLib, MAX_PATH, path, MAX_PATH);
  HINSTANCE lib = LoadLibraryW(tmpLib);
  if(lib != NULL)
  {
    lua_CFunction f = reinterpret_cast<lua_CFunction>(GetProcAddress(lib,init));
    if(f != NULL)
    {
      lua_pushlightuserdata(L, lib);
      lua_pushcclosure(L, f, 1);
      return 1;
    }
  }
  lua_pushnil(L);
  int error = GetLastError();
  wchar_t buffer[128];
  char cbuffer[128];
  if(FormatMessage(FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM, 0, error, 0, buffer, sizeof(buffer), 0))
  {
	wcstombs_s(NULL, cbuffer, 128, buffer, 256);
    lua_pushstring(L,cbuffer);
  }
  else
    lua_pushfstring(L,"system error %d\n",error);
  lua_pushstring(L, (lib != NULL) ? "init" : "open");
  if(lib != NULL)
    FreeLibrary(lib);
  return 3;
}

int __stdcall luaLoadfile(lua_State *L) {
	if(lua_type(L, 2) == LUA_TNONE || lua_type(L, 2) == LUA_TNIL) {
		return luaLoadfileStandard(L);
	} else {
		return luaLoadlib(L);
	}
}