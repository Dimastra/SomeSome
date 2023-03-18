using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Objectives
{
	// Token: 0x020002AA RID: 682
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ConditionInfo
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x00019D2D File Offset: 0x00017F2D
		public string Title { get; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00019D35 File Offset: 0x00017F35
		public string Description { get; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00019D3D File Offset: 0x00017F3D
		public SpriteSpecifier SpriteSpecifier { get; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x00019D45 File Offset: 0x00017F45
		public float Progress { get; }

		// Token: 0x060007A7 RID: 1959 RVA: 0x00019D4D File Offset: 0x00017F4D
		public ConditionInfo(string title, string description, SpriteSpecifier spriteSpecifier, float progress)
		{
			this.Title = title;
			this.Description = description;
			this.SpriteSpecifier = spriteSpecifier;
			this.Progress = progress;
		}
	}
}
