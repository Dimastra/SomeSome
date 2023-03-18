using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Hands.Components
{
	// Token: 0x0200043F RID: 1087
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedHandsComponent : Component
	{
		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000D2C RID: 3372 RVA: 0x0002BB94 File Offset: 0x00029D94
		[ViewVariables]
		public EntityUid? ActiveHandEntity
		{
			get
			{
				Hand activeHand = this.ActiveHand;
				if (activeHand == null)
				{
					return null;
				}
				return activeHand.HeldEntity;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000D2D RID: 3373 RVA: 0x0002BBBA File Offset: 0x00029DBA
		public int Count
		{
			get
			{
				return this.Hands.Count;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000D2E RID: 3374 RVA: 0x0002BBC7 File Offset: 0x00029DC7
		// (set) Token: 0x06000D2F RID: 3375 RVA: 0x0002BBCF File Offset: 0x00029DCF
		[DataField("throwForceMultiplier", false, 1, false, false, null)]
		[ViewVariables]
		public float ThrowForceMultiplier { get; set; } = 10f;

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000D30 RID: 3376 RVA: 0x0002BBD8 File Offset: 0x00029DD8
		// (set) Token: 0x06000D31 RID: 3377 RVA: 0x0002BBE0 File Offset: 0x00029DE0
		[DataField("throwRange", false, 1, false, false, null)]
		[ViewVariables]
		public float ThrowRange { get; set; } = 8f;

		// Token: 0x04000CB6 RID: 3254
		[Nullable(2)]
		[ViewVariables]
		public Hand ActiveHand;

		// Token: 0x04000CB7 RID: 3255
		[ViewVariables]
		public Dictionary<string, Hand> Hands = new Dictionary<string, Hand>();

		// Token: 0x04000CB8 RID: 3256
		public List<string> SortedHands = new List<string>();
	}
}
