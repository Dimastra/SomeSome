using System;
using Content.Shared.CombatMode.Pacification;
using Robust.Shared.GameObjects;

namespace Content.Server.Traits.Assorted
{
	// Token: 0x02000106 RID: 262
	public sealed class PacifistSystem : EntitySystem
	{
		// Token: 0x060004BD RID: 1213 RVA: 0x00016A54 File Offset: 0x00014C54
		public override void Update(float frameTime)
		{
			foreach (PacifistComponent comp in base.EntityQuery<PacifistComponent>(false))
			{
				base.EnsureComp<PacifiedComponent>(comp.Owner);
			}
		}
	}
}
