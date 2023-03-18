using System;
using Content.Server.Atmos.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A1 RID: 1953
	[RegisterComponent]
	public sealed class AtmosPlaqueComponent : Component
	{
		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06002A66 RID: 10854 RVA: 0x000DFB12 File Offset: 0x000DDD12
		// (set) Token: 0x06002A67 RID: 10855 RVA: 0x000DFB1A File Offset: 0x000DDD1A
		[ViewVariables]
		public PlaqueType TypeVV
		{
			get
			{
				return this.Type;
			}
			set
			{
				this.Type = value;
				EntitySystem.Get<AtmosPlaqueSystem>().UpdateSign(base.Owner, this);
			}
		}

		// Token: 0x04001A32 RID: 6706
		[DataField("plaqueType", false, 1, false, false, null)]
		public PlaqueType Type;
	}
}
