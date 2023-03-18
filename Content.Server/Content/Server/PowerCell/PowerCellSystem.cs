using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Explosion.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Kitchen.Components;
using Content.Server.Power.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.PowerCell
{
	// Token: 0x02000273 RID: 627
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerCellSystem : SharedPowerCellSystem
	{
		// Token: 0x06000C84 RID: 3204 RVA: 0x000416F4 File Offset: 0x0003F8F4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PowerCellComponent, ChargeChangedEvent>(new ComponentEventHandler<PowerCellComponent, ChargeChangedEvent>(this.OnChargeChanged), null, null);
			base.SubscribeLocalEvent<PowerCellComponent, SolutionChangedEvent>(new ComponentEventHandler<PowerCellComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
			base.SubscribeLocalEvent<PowerCellComponent, RejuvenateEvent>(new ComponentEventHandler<PowerCellComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
			base.SubscribeLocalEvent<PowerCellComponent, ExaminedEvent>(new ComponentEventHandler<PowerCellComponent, ExaminedEvent>(this.OnCellExamined), null, null);
			base.SubscribeLocalEvent<PowerCellSlotComponent, BeingMicrowavedEvent>(new ComponentEventHandler<PowerCellSlotComponent, BeingMicrowavedEvent>(this.OnSlotMicrowaved), null, null);
			base.SubscribeLocalEvent<BatteryComponent, BeingMicrowavedEvent>(new ComponentEventHandler<BatteryComponent, BeingMicrowavedEvent>(this.OnMicrowaved), null, null);
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0004177F File Offset: 0x0003F97F
		private void OnRejuvenate(EntityUid uid, PowerCellComponent component, RejuvenateEvent args)
		{
			component.IsRigged = false;
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x00041788 File Offset: 0x0003F988
		private void OnSlotMicrowaved(EntityUid uid, PowerCellSlotComponent component, BeingMicrowavedEvent args)
		{
			ItemSlot slot;
			if (this._itemSlotsSystem.TryGetSlot(uid, component.CellSlotId, out slot, null))
			{
				if (slot.Item == null)
				{
					return;
				}
				base.RaiseLocalEvent<BeingMicrowavedEvent>(slot.Item.Value, args, false);
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x000417D3 File Offset: 0x0003F9D3
		private void OnMicrowaved(EntityUid uid, BatteryComponent component, BeingMicrowavedEvent args)
		{
			if (component.CurrentCharge == 0f)
			{
				return;
			}
			args.Handled = true;
			this.Explode(uid, component, args.User);
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x000417F8 File Offset: 0x0003F9F8
		private void OnChargeChanged(EntityUid uid, PowerCellComponent component, ChargeChangedEvent args)
		{
			if (component.IsRigged)
			{
				this.Explode(uid, null, null);
				return;
			}
			BatteryComponent battery;
			if (!base.TryComp<BatteryComponent>(uid, ref battery))
			{
				return;
			}
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			byte level = (byte)ContentHelpers.RoundToNearestLevels((double)(battery.CurrentCharge / battery.MaxCharge), 1.0, 2);
			this._sharedAppearanceSystem.SetData(uid, PowerCellVisuals.ChargeLevel, level, appearance);
			IContainer container;
			PowerCellSlotComponent slot;
			ItemSlot itemSlot;
			if (this._containerSystem.TryGetContainingContainer(uid, ref container, null, null) && base.TryComp<PowerCellSlotComponent>(container.Owner, ref slot) && this._itemSlotsSystem.TryGetSlot(container.Owner, slot.CellSlotId, out itemSlot, null))
			{
				EntityUid? item = itemSlot.Item;
				if (item != null && (item == null || item.GetValueOrDefault() == uid))
				{
					base.RaiseLocalEvent<PowerCellChangedEvent>(container.Owner, new PowerCellChangedEvent(false), false);
				}
			}
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x000418F8 File Offset: 0x0003FAF8
		[NullableContext(2)]
		private void Explode(EntityUid uid, BatteryComponent battery = null, EntityUid? cause = null)
		{
			if (!base.Resolve<BatteryComponent>(uid, ref battery, true))
			{
				return;
			}
			float radius = MathF.Min(5f, MathF.Sqrt(battery.CurrentCharge) / 9f);
			ExplosionSystem explosionSystem = this._explosionSystem;
			ExplosiveComponent explosive = null;
			bool delete = true;
			float? radius2 = new float?(radius);
			explosionSystem.TriggerExplosive(uid, explosive, delete, null, radius2, cause);
			base.QueueDel(uid);
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x00041958 File Offset: 0x0003FB58
		[NullableContext(2)]
		public bool TryGetBatteryFromSlot(EntityUid uid, [NotNullWhen(true)] out BatteryComponent battery, PowerCellSlotComponent component = null)
		{
			if (!base.Resolve<PowerCellSlotComponent>(uid, ref component, false))
			{
				battery = null;
				return false;
			}
			ItemSlot slot;
			if (this._itemSlotsSystem.TryGetSlot(uid, component.CellSlotId, out slot, null))
			{
				return base.TryComp<BatteryComponent>(slot.Item, ref battery);
			}
			battery = null;
			return false;
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x000419A0 File Offset: 0x0003FBA0
		private void OnSolutionChange(EntityUid uid, PowerCellComponent component, SolutionChangedEvent args)
		{
			Solution solution;
			FixedPoint2 plasma;
			component.IsRigged = (this._solutionsSystem.TryGetSolution(uid, "powerCell", out solution, null) && solution.TryGetReagent("Plasma", out plasma) && plasma >= 5);
			if (component.IsRigged)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Explosion;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(52, 1);
				logStringHandler.AppendLiteral("Power cell ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" has been rigged up to explode when used.");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x00041A30 File Offset: 0x0003FC30
		private void OnCellExamined(EntityUid uid, PowerCellComponent component, ExaminedEvent args)
		{
			BatteryComponent battery;
			if (!base.TryComp<BatteryComponent>(uid, ref battery))
			{
				return;
			}
			float charge = battery.CurrentCharge / battery.MaxCharge * 100f;
			string text = "power-cell-component-examine-details";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "currentCharge";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(charge, "F0");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			args.PushMarkup(Loc.GetString(text, array));
		}

		// Token: 0x040007B6 RID: 1974
		[Dependency]
		private readonly SolutionContainerSystem _solutionsSystem;

		// Token: 0x040007B7 RID: 1975
		[Dependency]
		private readonly ExplosionSystem _explosionSystem;

		// Token: 0x040007B8 RID: 1976
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040007B9 RID: 1977
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x040007BA RID: 1978
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040007BB RID: 1979
		[Dependency]
		private readonly SharedAppearanceSystem _sharedAppearanceSystem;
	}
}
