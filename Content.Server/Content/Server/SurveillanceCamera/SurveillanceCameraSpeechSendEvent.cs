using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000142 RID: 322
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraSpeechSendEvent : EntityEventArgs
	{
		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x0001C7D3 File Offset: 0x0001A9D3
		public EntityUid Speaker { get; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x0001C7DB File Offset: 0x0001A9DB
		public string Message { get; }

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001C7E3 File Offset: 0x0001A9E3
		public SurveillanceCameraSpeechSendEvent(EntityUid speaker, string message)
		{
			this.Speaker = speaker;
			this.Message = message;
		}
	}
}
