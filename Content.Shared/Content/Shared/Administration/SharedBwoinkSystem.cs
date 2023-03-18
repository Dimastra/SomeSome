using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000745 RID: 1861
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedBwoinkSystem : EntitySystem
	{
		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x00049AF1 File Offset: 0x00047CF1
		public static NetUserId SystemUserId { get; } = new NetUserId(Guid.Empty);

		// Token: 0x06001694 RID: 5780 RVA: 0x00049AF8 File Offset: 0x00047CF8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<SharedBwoinkSystem.BwoinkTextMessage>(new EntitySessionEventHandler<SharedBwoinkSystem.BwoinkTextMessage>(this.OnBwoinkTextMessage), null, null);
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x00049B15 File Offset: 0x00047D15
		protected virtual void OnBwoinkTextMessage(SharedBwoinkSystem.BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
		{
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x00049B17 File Offset: 0x00047D17
		protected void LogBwoink(SharedBwoinkSystem.BwoinkTextMessage message)
		{
		}

		// Token: 0x0200089A RID: 2202
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class BwoinkTextMessage : EntityEventArgs
		{
			// Token: 0x1700054A RID: 1354
			// (get) Token: 0x06001A06 RID: 6662 RVA: 0x00051C1F File Offset: 0x0004FE1F
			public DateTime SentAt { get; }

			// Token: 0x1700054B RID: 1355
			// (get) Token: 0x06001A07 RID: 6663 RVA: 0x00051C27 File Offset: 0x0004FE27
			public NetUserId UserId { get; }

			// Token: 0x1700054C RID: 1356
			// (get) Token: 0x06001A08 RID: 6664 RVA: 0x00051C2F File Offset: 0x0004FE2F
			public NetUserId TrueSender { get; }

			// Token: 0x1700054D RID: 1357
			// (get) Token: 0x06001A09 RID: 6665 RVA: 0x00051C37 File Offset: 0x0004FE37
			public string Text { get; }

			// Token: 0x06001A0A RID: 6666 RVA: 0x00051C40 File Offset: 0x0004FE40
			public BwoinkTextMessage(NetUserId userId, NetUserId trueSender, string text, DateTime? sentAt = null)
			{
				this.SentAt = (sentAt ?? DateTime.Now);
				this.UserId = userId;
				this.TrueSender = trueSender;
				this.Text = text;
			}
		}
	}
}
