using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Standing
{
	// Token: 0x02000163 RID: 355
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(StandingStateSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StandingStateComponent : Component
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00011292 File Offset: 0x0000F492
		[ViewVariables]
		[DataField("downSound", false, 1, false, false, null)]
		public SoundSpecifier DownSound { get; } = new SoundCollectionSpecifier("BodyFall", null);

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x0001129A File Offset: 0x0000F49A
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x000112A2 File Offset: 0x0000F4A2
		[DataField("standing", false, 1, false, false, null)]
		public bool Standing { get; set; } = true;

		// Token: 0x04000418 RID: 1048
		[DataField("changedFixtures", false, 1, false, false, null)]
		public List<string> ChangedFixtures = new List<string>();
	}
}
