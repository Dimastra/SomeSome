using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;

namespace Content.Client.Chat.Managers
{
	// Token: 0x020003EA RID: 1002
	[NullableContext(1)]
	public interface IChatManager
	{
		// Token: 0x0600189D RID: 6301
		void Initialize();

		// Token: 0x0600189E RID: 6302
		void SendMessage(string text, ChatSelectChannel channel);
	}
}
