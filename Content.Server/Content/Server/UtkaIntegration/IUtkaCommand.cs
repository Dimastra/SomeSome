using System;
using System.Runtime.CompilerServices;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000D8 RID: 216
	[NullableContext(1)]
	public interface IUtkaCommand
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003E9 RID: 1001
		string Name { get; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003EA RID: 1002
		Type RequestMessageType { get; }

		// Token: 0x060003EB RID: 1003
		void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage);
	}
}
