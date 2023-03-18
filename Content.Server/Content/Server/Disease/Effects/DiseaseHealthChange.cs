using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Effects
{
	// Token: 0x0200056A RID: 1386
	public sealed class DiseaseHealthChange : DiseaseEffect
	{
		// Token: 0x06001D51 RID: 7505 RVA: 0x0009C5C0 File Offset: 0x0009A7C0
		public override void Effect(DiseaseEffectArgs args)
		{
			EntitySystem.Get<DamageableSystem>().TryChangeDamage(new EntityUid?(args.DiseasedEntity), this.Damage, true, false, null, null);
		}

		// Token: 0x040012C1 RID: 4801
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
