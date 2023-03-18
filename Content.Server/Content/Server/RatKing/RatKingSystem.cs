using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.RatKing
{
	// Token: 0x02000251 RID: 593
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RatKingSystem : EntitySystem
	{
		// Token: 0x06000BD0 RID: 3024 RVA: 0x0003E6B8 File Offset: 0x0003C8B8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RatKingComponent, ComponentStartup>(new ComponentEventHandler<RatKingComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<RatKingComponent, RatKingRaiseArmyActionEvent>(new ComponentEventHandler<RatKingComponent, RatKingRaiseArmyActionEvent>(this.OnRaiseArmy), null, null);
			base.SubscribeLocalEvent<RatKingComponent, RatKingDomainActionEvent>(new ComponentEventHandler<RatKingComponent, RatKingDomainActionEvent>(this.OnDomain), null, null);
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0003E708 File Offset: 0x0003C908
		private void OnStartup(EntityUid uid, RatKingComponent component, ComponentStartup args)
		{
			this._action.AddAction(uid, component.ActionRaiseArmy, null, null, true);
			this._action.AddAction(uid, component.ActionDomain, null, null, true);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0003E750 File Offset: 0x0003C950
		private void OnRaiseArmy(EntityUid uid, RatKingComponent component, RatKingRaiseArmyActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			HungerComponent hunger;
			if (!base.TryComp<HungerComponent>(uid, ref hunger))
			{
				return;
			}
			if (hunger.CurrentHunger < component.HungerPerArmyUse)
			{
				this._popup.PopupEntity(Loc.GetString("rat-king-too-hungry"), uid, uid, PopupType.Small);
				return;
			}
			args.Handled = true;
			hunger.CurrentHunger -= component.HungerPerArmyUse;
			base.Spawn(component.ArmyMobSpawnId, base.Transform(uid).Coordinates);
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0003E7CC File Offset: 0x0003C9CC
		private void OnDomain(EntityUid uid, RatKingComponent component, RatKingDomainActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			HungerComponent hunger;
			if (!base.TryComp<HungerComponent>(uid, ref hunger))
			{
				return;
			}
			if (hunger.CurrentHunger < component.HungerPerDomainUse)
			{
				this._popup.PopupEntity(Loc.GetString("rat-king-too-hungry"), uid, uid, PopupType.Small);
				return;
			}
			args.Handled = true;
			hunger.CurrentHunger -= component.HungerPerDomainUse;
			this._popup.PopupEntity(Loc.GetString("rat-king-domain-popup"), uid, PopupType.Small);
			TransformComponent transform = base.Transform(uid);
			Vector2i indices = this._xform.GetGridOrMapTilePosition(uid, transform);
			GasMixture tileMixture = this._atmos.GetTileMixture(transform.GridUid, transform.MapUid, indices, true);
			if (tileMixture == null)
			{
				return;
			}
			tileMixture.AdjustMoles(Gas.Miasma, component.MolesMiasmaPerDomain);
		}

		// Token: 0x0400075E RID: 1886
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x0400075F RID: 1887
		[Dependency]
		private readonly ActionsSystem _action;

		// Token: 0x04000760 RID: 1888
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x04000761 RID: 1889
		[Dependency]
		private readonly TransformSystem _xform;
	}
}
