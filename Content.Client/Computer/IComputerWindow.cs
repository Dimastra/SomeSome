using System;
using System.Runtime.CompilerServices;

namespace Content.Client.Computer
{
	// Token: 0x0200039B RID: 923
	[NullableContext(1)]
	public interface IComputerWindow<[Nullable(2)] TState>
	{
		// Token: 0x060016F6 RID: 5878 RVA: 0x0001B008 File Offset: 0x00019208
		void SetupComputerWindow(ComputerBoundUserInterfaceBase cb)
		{
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x0001B008 File Offset: 0x00019208
		void UpdateState(TState state)
		{
		}
	}
}
