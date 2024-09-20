#include "pch.h"
#include "shared/Shared.h"
#include <grpcpp/grpcpp.h>
#include "services/HimuServices.h"

//
// Globals
//

himu::Globals gGlobals;

int main()
{
	gGlobals.Initialize();
	himu::BuildServerAndStart(gGlobals.Configuration);
	gGlobals.Cleanup();
	return 0;
}
