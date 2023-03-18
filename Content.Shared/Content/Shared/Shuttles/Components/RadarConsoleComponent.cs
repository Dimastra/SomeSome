using System;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Shuttles.Components
{
	// Token: 0x020001CC RID: 460
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedRadarConsoleSystem)
	})]
	public sealed class RadarConsoleComponent : Component
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x00013A2C File Offset: 0x00011C2C
		// (set) Token: 0x06000540 RID: 1344 RVA: 0x00013A34 File Offset: 0x00011C34
		[ViewVariables]
		public float RangeVV
		{
			get
			{
				return this.MaxRange;
			}
			set
			{
				IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedRadarConsoleSystem>().SetRange(this, value);
			}
		}

		// Token: 0x04000535 RID: 1333
		[DataField("maxRange", false, 1, false, false, null)]
		public float MaxRange = 256f;
	}
}
