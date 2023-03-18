using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink
{
	// Token: 0x020000B2 RID: 178
	[NullableContext(1)]
	public interface IAHelpUIHandler : IDisposable
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060004B5 RID: 1205
		bool IsAdmin { get; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060004B6 RID: 1206
		bool IsOpen { get; }

		// Token: 0x060004B7 RID: 1207
		void Receive(SharedBwoinkSystem.BwoinkTextMessage message);

		// Token: 0x060004B8 RID: 1208
		void Close();

		// Token: 0x060004B9 RID: 1209
		void Open(NetUserId netUserId);

		// Token: 0x060004BA RID: 1210
		void ToggleWindow();

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060004BB RID: 1211
		// (remove) Token: 0x060004BC RID: 1212
		event Action OnClose;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060004BD RID: 1213
		// (remove) Token: 0x060004BE RID: 1214
		event Action OnOpen;

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060004BF RID: 1215
		// (set) Token: 0x060004C0 RID: 1216
		[Nullable(new byte[]
		{
			2,
			1
		})]
		Action<NetUserId, string> SendMessageAction { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }
	}
}
