using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Hands.Components
{
	// Token: 0x02000440 RID: 1088
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class Hand
	{
		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000D33 RID: 3379 RVA: 0x0002BC1D File Offset: 0x00029E1D
		[ViewVariables]
		public string Name { get; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0002BC25 File Offset: 0x00029E25
		[ViewVariables]
		public HandLocation Location { get; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000D35 RID: 3381 RVA: 0x0002BC30 File Offset: 0x00029E30
		[ViewVariables]
		public EntityUid? HeldEntity
		{
			get
			{
				ContainerSlot container = this.Container;
				if (container == null)
				{
					return null;
				}
				return container.ContainedEntity;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000D36 RID: 3382 RVA: 0x0002BC58 File Offset: 0x00029E58
		public bool IsEmpty
		{
			get
			{
				return this.HeldEntity == null;
			}
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0002BC76 File Offset: 0x00029E76
		public Hand(string name, HandLocation location, [Nullable(2)] ContainerSlot container = null)
		{
			this.Name = name;
			this.Location = location;
			this.Container = container;
		}

		// Token: 0x04000CBD RID: 3261
		[Nullable(2)]
		[ViewVariables]
		[NonSerialized]
		public ContainerSlot Container;
	}
}
