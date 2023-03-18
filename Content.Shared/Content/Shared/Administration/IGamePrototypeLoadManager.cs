using System;
using System.Runtime.CompilerServices;

namespace Content.Shared.Administration
{
	// Token: 0x02000738 RID: 1848
	[NullableContext(1)]
	public interface IGamePrototypeLoadManager
	{
		// Token: 0x0600165E RID: 5726
		void Initialize();

		// Token: 0x0600165F RID: 5727
		void SendGamePrototype(string prototype);
	}
}
