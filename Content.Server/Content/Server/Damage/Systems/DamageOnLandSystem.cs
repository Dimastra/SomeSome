using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Components;
using Content.Shared.Damage;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C1 RID: 1473
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageOnLandSystem : EntitySystem
	{
		// Token: 0x06001F77 RID: 8055 RVA: 0x000A532A File Offset: 0x000A352A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageOnLandComponent, LandEvent>(new ComponentEventRefHandler<DamageOnLandComponent, LandEvent>(this.DamageOnLand), null, null);
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x000A5348 File Offset: 0x000A3548
		private void DamageOnLand(EntityUid uid, DamageOnLandComponent component, ref LandEvent args)
		{
			this._damageableSystem.TryChangeDamage(new EntityUid?(uid), component.Damage, component.IgnoreResistances, true, null, null);
		}

		// Token: 0x0400138D RID: 5005
		[Dependency]
		private readonly DamageableSystem _damageableSystem;
	}
}
