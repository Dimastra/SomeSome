using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Events;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Abilities.Boxer
{
	// Token: 0x02000888 RID: 2184
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BoxingSystem : EntitySystem
	{
		// Token: 0x06002F91 RID: 12177 RVA: 0x000F6560 File Offset: 0x000F4760
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BoxerComponent, ComponentInit>(new ComponentEventHandler<BoxerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<BoxerComponent, MeleeHitEvent>(new ComponentEventHandler<BoxerComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
			base.SubscribeLocalEvent<BoxingGlovesComponent, StaminaMeleeHitEvent>(new ComponentEventHandler<BoxingGlovesComponent, StaminaMeleeHitEvent>(this.OnStamHit), null, null);
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x000F65B0 File Offset: 0x000F47B0
		private void OnInit(EntityUid uid, BoxerComponent component, ComponentInit args)
		{
			MeleeWeaponComponent meleeComp;
			if (base.TryComp<MeleeWeaponComponent>(uid, ref meleeComp))
			{
				meleeComp.Range *= component.RangeBonus;
			}
		}

		// Token: 0x06002F93 RID: 12179 RVA: 0x000F65DB File Offset: 0x000F47DB
		private void OnMeleeHit(EntityUid uid, BoxerComponent component, MeleeHitEvent args)
		{
			args.ModifiersList.Add(component.UnarmedModifiers);
		}

		// Token: 0x06002F94 RID: 12180 RVA: 0x000F65F0 File Offset: 0x000F47F0
		private void OnStamHit(EntityUid uid, BoxingGlovesComponent component, StaminaMeleeHitEvent args)
		{
			IContainer equipee;
			if (!this._containerSystem.TryGetContainingContainer(uid, ref equipee, null, null))
			{
				return;
			}
			BoxerComponent boxer;
			if (base.TryComp<BoxerComponent>(equipee.Owner, ref boxer))
			{
				args.Multiplier *= boxer.BoxingGlovesModifier;
			}
		}

		// Token: 0x04001CA3 RID: 7331
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
