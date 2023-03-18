using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mind.Components
{
	// Token: 0x020003A4 RID: 932
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(MindSystem)
	})]
	public sealed class MindComponent : Component
	{
		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001320 RID: 4896 RVA: 0x00062ED3 File Offset: 0x000610D3
		// (set) Token: 0x06001321 RID: 4897 RVA: 0x00062EDB File Offset: 0x000610DB
		[ViewVariables]
		[Access]
		public Mind Mind { get; set; }

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001322 RID: 4898 RVA: 0x00062EE4 File Offset: 0x000610E4
		[ViewVariables]
		public bool HasMind
		{
			get
			{
				return this.Mind != null;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001323 RID: 4899 RVA: 0x00062EEF File Offset: 0x000610EF
		// (set) Token: 0x06001324 RID: 4900 RVA: 0x00062EF7 File Offset: 0x000610F7
		[ViewVariables]
		[DataField("showExamineInfo", false, 1, false, false, null)]
		public bool ShowExamineInfo { get; set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001325 RID: 4901 RVA: 0x00062F00 File Offset: 0x00061100
		// (set) Token: 0x06001326 RID: 4902 RVA: 0x00062F08 File Offset: 0x00061108
		[ViewVariables]
		[DataField("ghostOnShutdown", false, 1, false, false, null)]
		public bool GhostOnShutdown { get; set; } = true;

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x00062F11 File Offset: 0x00061111
		// (set) Token: 0x06001328 RID: 4904 RVA: 0x00062F19 File Offset: 0x00061119
		[ViewVariables]
		[DataField("preventGhosting", false, 1, false, false, null)]
		public bool PreventGhosting { get; set; }
	}
}
