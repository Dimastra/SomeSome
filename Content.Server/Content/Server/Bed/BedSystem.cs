using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Bed.Components;
using Content.Server.Bed.Sleep;
using Content.Server.Body.Systems;
using Content.Server.Construction;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Bed;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Bed
{
	// Token: 0x02000725 RID: 1829
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BedSystem : EntitySystem
	{
		// Token: 0x06002663 RID: 9827 RVA: 0x000CB06C File Offset: 0x000C926C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HealOnBuckleComponent, BuckleChangeEvent>(new ComponentEventHandler<HealOnBuckleComponent, BuckleChangeEvent>(this.ManageUpdateList), null, null);
			base.SubscribeLocalEvent<StasisBedComponent, BuckleChangeEvent>(new ComponentEventHandler<StasisBedComponent, BuckleChangeEvent>(this.OnBuckleChange), null, null);
			base.SubscribeLocalEvent<StasisBedComponent, PowerChangedEvent>(new ComponentEventRefHandler<StasisBedComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<StasisBedComponent, GotEmaggedEvent>(new ComponentEventRefHandler<StasisBedComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<StasisBedComponent, RefreshPartsEvent>(new ComponentEventHandler<StasisBedComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<StasisBedComponent, UpgradeExamineEvent>(new ComponentEventHandler<StasisBedComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x06002664 RID: 9828 RVA: 0x000CB0F8 File Offset: 0x000C92F8
		private void ManageUpdateList(EntityUid uid, HealOnBuckleComponent component, BuckleChangeEvent args)
		{
			InstantActionPrototype sleepAction;
			this._prototypeManager.TryIndex<InstantActionPrototype>("Sleep", ref sleepAction);
			if (args.Buckling)
			{
				base.AddComp<HealOnBuckleHealingComponent>(uid);
				component.NextHealTime = this._timing.CurTime + TimeSpan.FromSeconds((double)component.HealTime);
				if (sleepAction != null)
				{
					this._actionsSystem.AddAction(args.BuckledEntity, new InstantAction(sleepAction), null, null, true);
				}
				return;
			}
			if (sleepAction != null)
			{
				this._actionsSystem.RemoveAction(args.BuckledEntity, sleepAction, null);
			}
			this._sleepingSystem.TryWaking(args.BuckledEntity, null, false, null);
			base.RemComp<HealOnBuckleHealingComponent>(uid);
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x000CB1AC File Offset: 0x000C93AC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<HealOnBuckleHealingComponent, HealOnBuckleComponent, StrapComponent> valueTuple in base.EntityQuery<HealOnBuckleHealingComponent, HealOnBuckleComponent, StrapComponent>(false))
			{
				HealOnBuckleComponent bedComponent = valueTuple.Item2;
				StrapComponent strapComponent = valueTuple.Item3;
				if (!(this._timing.CurTime < bedComponent.NextHealTime))
				{
					bedComponent.NextHealTime += TimeSpan.FromSeconds((double)bedComponent.HealTime);
					if (strapComponent.BuckledEntities.Count != 0)
					{
						foreach (EntityUid healedEntity in strapComponent.BuckledEntities)
						{
							if (!this._mobStateSystem.IsDead(healedEntity, null))
							{
								DamageSpecifier damage = bedComponent.Damage;
								if (base.HasComp<SleepingComponent>(healedEntity))
								{
									damage *= bedComponent.SleepMultiplier;
								}
								this._damageableSystem.TryChangeDamage(new EntityUid?(healedEntity), damage, true, true, null, new EntityUid?(bedComponent.Owner));
							}
						}
					}
				}
			}
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000CB2E4 File Offset: 0x000C94E4
		private void UpdateAppearance(EntityUid uid, bool isOn)
		{
			this._appearance.SetData(uid, StasisBedVisuals.IsOn, isOn, null);
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x000CB300 File Offset: 0x000C9500
		private void OnBuckleChange(EntityUid uid, StasisBedComponent component, BuckleChangeEvent args)
		{
			if (!base.HasComp<BodyComponent>(args.BuckledEntity))
			{
				return;
			}
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			ApplyMetabolicMultiplierEvent metabolicEvent = new ApplyMetabolicMultiplierEvent
			{
				Uid = args.BuckledEntity,
				Multiplier = component.Multiplier,
				Apply = args.Buckling
			};
			base.RaiseLocalEvent<ApplyMetabolicMultiplierEvent>(args.BuckledEntity, metabolicEvent, false);
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x000CB365 File Offset: 0x000C9565
		private void OnPowerChanged(EntityUid uid, StasisBedComponent component, ref PowerChangedEvent args)
		{
			this.UpdateAppearance(uid, args.Powered);
			this.UpdateMetabolisms(uid, component, args.Powered);
		}

		// Token: 0x06002669 RID: 9833 RVA: 0x000CB382 File Offset: 0x000C9582
		private void OnEmagged(EntityUid uid, StasisBedComponent component, ref GotEmaggedEvent args)
		{
			args.Repeatable = true;
			this.UpdateMetabolisms(uid, component, false);
			component.Multiplier = 1f / component.Multiplier;
			this.UpdateMetabolisms(uid, component, true);
			args.Handled = true;
		}

		// Token: 0x0600266A RID: 9834 RVA: 0x000CB3B8 File Offset: 0x000C95B8
		private void UpdateMetabolisms(EntityUid uid, StasisBedComponent component, bool shouldApply)
		{
			StrapComponent strap;
			if (!base.TryComp<StrapComponent>(uid, ref strap) || strap.BuckledEntities.Count == 0)
			{
				return;
			}
			foreach (EntityUid buckledEntity in strap.BuckledEntities)
			{
				ApplyMetabolicMultiplierEvent metabolicEvent = new ApplyMetabolicMultiplierEvent
				{
					Uid = buckledEntity,
					Multiplier = component.Multiplier,
					Apply = shouldApply
				};
				base.RaiseLocalEvent<ApplyMetabolicMultiplierEvent>(buckledEntity, metabolicEvent, false);
			}
		}

		// Token: 0x0600266B RID: 9835 RVA: 0x000CB448 File Offset: 0x000C9648
		private void OnRefreshParts(EntityUid uid, StasisBedComponent component, RefreshPartsEvent args)
		{
			float metabolismRating = args.PartRatings[component.MachinePartMetabolismModifier];
			component.Multiplier = component.BaseMultiplier * metabolismRating;
			if (base.HasComp<EmaggedComponent>(uid))
			{
				component.Multiplier = 1f / component.Multiplier;
			}
		}

		// Token: 0x0600266C RID: 9836 RVA: 0x000CB490 File Offset: 0x000C9690
		private void OnUpgradeExamine(EntityUid uid, StasisBedComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("stasis-bed-component-upgrade-stasis", component.Multiplier / component.BaseMultiplier);
		}

		// Token: 0x040017E2 RID: 6114
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040017E3 RID: 6115
		[Dependency]
		private readonly ActionsSystem _actionsSystem;

		// Token: 0x040017E4 RID: 6116
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040017E5 RID: 6117
		[Dependency]
		private readonly SleepingSystem _sleepingSystem;

		// Token: 0x040017E6 RID: 6118
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040017E7 RID: 6119
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040017E8 RID: 6120
		[Dependency]
		private readonly IGameTiming _timing;
	}
}
