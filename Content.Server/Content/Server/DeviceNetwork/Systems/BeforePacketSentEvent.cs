using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000584 RID: 1412
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeforePacketSentEvent : CancellableEntityEventArgs
	{
		// Token: 0x06001DA0 RID: 7584 RVA: 0x0009DC4F File Offset: 0x0009BE4F
		public BeforePacketSentEvent(EntityUid sender, TransformComponent xform, Vector2 senderPosition)
		{
			this.Sender = sender;
			this.SenderTransform = xform;
			this.SenderPosition = senderPosition;
		}

		// Token: 0x040012F9 RID: 4857
		public readonly EntityUid Sender;

		// Token: 0x040012FA RID: 4858
		public readonly TransformComponent SenderTransform;

		// Token: 0x040012FB RID: 4859
		public readonly Vector2 SenderPosition;
	}
}
