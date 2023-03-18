using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A4 RID: 1700
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ChemMasterSystem)
	})]
	public sealed class ChemMasterComponent : Component
	{
		// Token: 0x040015D5 RID: 5589
		[DataField("pillType", false, 1, false, false, null)]
		[ViewVariables]
		public uint PillType;

		// Token: 0x040015D6 RID: 5590
		[DataField("mode", false, 1, false, false, null)]
		[ViewVariables]
		public ChemMasterMode Mode;

		// Token: 0x040015D7 RID: 5591
		[DataField("pillDosageLimit", false, 1, true, false, null)]
		[ViewVariables]
		public uint PillDosageLimit;

		// Token: 0x040015D8 RID: 5592
		[Nullable(1)]
		[DataField("clickSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);
	}
}
