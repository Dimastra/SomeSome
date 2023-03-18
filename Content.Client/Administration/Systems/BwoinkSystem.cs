using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Client.Administration.Systems
{
	// Token: 0x020004E0 RID: 1248
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BwoinkSystem : SharedBwoinkSystem
	{
		// Token: 0x140000C2 RID: 194
		// (add) Token: 0x06001FCE RID: 8142 RVA: 0x000B982C File Offset: 0x000B7A2C
		// (remove) Token: 0x06001FCF RID: 8143 RVA: 0x000B9864 File Offset: 0x000B7A64
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event EventHandler<SharedBwoinkSystem.BwoinkTextMessage> OnBwoinkTextMessageRecieved;

		// Token: 0x06001FD0 RID: 8144 RVA: 0x000B9899 File Offset: 0x000B7A99
		protected override void OnBwoinkTextMessage(SharedBwoinkSystem.BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
		{
			EventHandler<SharedBwoinkSystem.BwoinkTextMessage> onBwoinkTextMessageRecieved = this.OnBwoinkTextMessageRecieved;
			if (onBwoinkTextMessageRecieved == null)
			{
				return;
			}
			onBwoinkTextMessageRecieved(this, message);
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x000B98B0 File Offset: 0x000B7AB0
		public void Send(NetUserId channelId, string text)
		{
			base.RaiseNetworkEvent(new SharedBwoinkSystem.BwoinkTextMessage(channelId, channelId, text, null));
		}
	}
}
