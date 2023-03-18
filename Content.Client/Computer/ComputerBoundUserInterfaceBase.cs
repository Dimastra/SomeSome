using System;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Computer
{
	// Token: 0x0200039A RID: 922
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class ComputerBoundUserInterfaceBase : BoundUserInterface
	{
		// Token: 0x060016F4 RID: 5876 RVA: 0x000021BC File Offset: 0x000003BC
		public ComputerBoundUserInterfaceBase(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00085B90 File Offset: 0x00083D90
		public void SendMessage(BoundUserInterfaceMessage msg)
		{
			base.SendMessage(msg);
		}
	}
}
