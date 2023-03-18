using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003DD RID: 989
	public sealed class UseInHandEvent : HandledEntityEventArgs
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00026402 File Offset: 0x00024602
		public EntityUid User { get; }

		// Token: 0x06000B9E RID: 2974 RVA: 0x0002640A File Offset: 0x0002460A
		public UseInHandEvent(EntityUid user)
		{
			this.User = user;
		}
	}
}
