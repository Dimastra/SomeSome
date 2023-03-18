using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C1 RID: 1729
	[NullableContext(1)]
	[Nullable(0)]
	public class SetSpeakerColorEvent
	{
		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600241E RID: 9246 RVA: 0x000BC97D File Offset: 0x000BAB7D
		// (set) Token: 0x0600241F RID: 9247 RVA: 0x000BC985 File Offset: 0x000BAB85
		public EntityUid Sender { get; set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06002420 RID: 9248 RVA: 0x000BC98E File Offset: 0x000BAB8E
		// (set) Token: 0x06002421 RID: 9249 RVA: 0x000BC996 File Offset: 0x000BAB96
		public string Name { get; set; }

		// Token: 0x06002422 RID: 9250 RVA: 0x000BC99F File Offset: 0x000BAB9F
		public SetSpeakerColorEvent(EntityUid sender, string name)
		{
			this.Sender = sender;
			this.Name = name;
		}
	}
}
