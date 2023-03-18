using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.Flash.Components
{
	// Token: 0x020004FC RID: 1276
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FlashSystem)
	})]
	public sealed class FlashComponent : Component
	{
		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06001A3F RID: 6719 RVA: 0x0008AA62 File Offset: 0x00088C62
		// (set) Token: 0x06001A40 RID: 6720 RVA: 0x0008AA6A File Offset: 0x00088C6A
		[DataField("duration", false, 1, false, false, null)]
		[ViewVariables]
		public int FlashDuration { get; set; } = 5000;

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x0008AA73 File Offset: 0x00088C73
		// (set) Token: 0x06001A42 RID: 6722 RVA: 0x0008AA7B File Offset: 0x00088C7B
		[DataField("range", false, 1, false, false, null)]
		[ViewVariables]
		public float Range { get; set; } = 7f;

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x0008AA84 File Offset: 0x00088C84
		// (set) Token: 0x06001A44 RID: 6724 RVA: 0x0008AA8C File Offset: 0x00088C8C
		[ViewVariables]
		[DataField("aoeFlashDuration", false, 1, false, false, null)]
		public int AoeFlashDuration { get; set; } = 2000;

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06001A45 RID: 6725 RVA: 0x0008AA95 File Offset: 0x00088C95
		// (set) Token: 0x06001A46 RID: 6726 RVA: 0x0008AA9D File Offset: 0x00088C9D
		[DataField("slowTo", false, 1, false, false, null)]
		[ViewVariables]
		public float SlowTo { get; set; } = 0.5f;

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06001A47 RID: 6727 RVA: 0x0008AAA6 File Offset: 0x00088CA6
		// (set) Token: 0x06001A48 RID: 6728 RVA: 0x0008AAAE File Offset: 0x00088CAE
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound { get; set; } = new SoundPathSpecifier("/Audio/Weapons/flash.ogg", null);

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x0008AAB7 File Offset: 0x00088CB7
		public bool HasUses
		{
			get
			{
				return this.Uses > 0;
			}
		}

		// Token: 0x040010A5 RID: 4261
		[DataField("uses", false, 1, false, false, null)]
		[ViewVariables]
		public int Uses = 5;

		// Token: 0x040010AA RID: 4266
		[DataField("autoRecharge", false, 1, false, false, null)]
		[ViewVariables]
		public bool AutoRecharge;

		// Token: 0x040010AB RID: 4267
		[DataField("rechargeDuration", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan RechargeDuration = TimeSpan.FromSeconds(120.0);

		// Token: 0x040010AC RID: 4268
		[DataField("nextChargeTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextChargeTime = TimeSpan.FromSeconds(120.0);

		// Token: 0x040010AD RID: 4269
		[DataField("maxCharges", false, 1, false, false, null)]
		[ViewVariables]
		public int MaxCharges = 5;

		// Token: 0x040010AE RID: 4270
		public bool Flashing;
	}
}
