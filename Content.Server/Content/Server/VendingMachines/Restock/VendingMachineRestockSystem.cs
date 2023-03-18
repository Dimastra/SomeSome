using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Cargo.Systems;
using Content.Server.DoAfter;
using Content.Server.Wires;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.VendingMachines;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.VendingMachines.Restock
{
	// Token: 0x020000D6 RID: 214
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VendingMachineRestockSystem : EntitySystem
	{
		// Token: 0x060003D8 RID: 984 RVA: 0x00014550 File Offset: 0x00012750
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VendingMachineRestockComponent, AfterInteractEvent>(new ComponentEventHandler<VendingMachineRestockComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<VendingMachineRestockComponent, PriceCalculationEvent>(new ComponentEventRefHandler<VendingMachineRestockComponent, PriceCalculationEvent>(this.OnPriceCalculation), null, null);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00014580 File Offset: 0x00012780
		public bool TryAccessMachine(EntityUid uid, VendingMachineRestockComponent restock, VendingMachineComponent machineComponent, EntityUid user, EntityUid target)
		{
			WiresComponent wires;
			if (!base.TryComp<WiresComponent>(target, ref wires) || !wires.IsPanelOpen)
			{
				this._popupSystem.PopupCursor(Loc.GetString("vending-machine-restock-needs-panel-open", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("this", uid),
					new ValueTuple<string, object>("user", user),
					new ValueTuple<string, object>("target", target)
				}), user, PopupType.Small);
				return false;
			}
			return true;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0001460C File Offset: 0x0001280C
		public bool TryMatchPackageToMachine(EntityUid uid, VendingMachineRestockComponent component, VendingMachineComponent machineComponent, EntityUid user, EntityUid target)
		{
			if (!component.CanRestock.Contains(machineComponent.PackPrototypeId))
			{
				this._popupSystem.PopupCursor(Loc.GetString("vending-machine-restock-invalid-inventory", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("this", uid),
					new ValueTuple<string, object>("user", user),
					new ValueTuple<string, object>("target", target)
				}), user, PopupType.Small);
				return false;
			}
			return true;
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00014694 File Offset: 0x00012894
		private void OnAfterInteract(EntityUid uid, VendingMachineRestockComponent component, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach)
			{
				return;
			}
			VendingMachineComponent machineComponent;
			if (!base.TryComp<VendingMachineComponent>(args.Target, ref machineComponent))
			{
				return;
			}
			if (!this.TryMatchPackageToMachine(uid, component, machineComponent, args.User, args.Target.Value))
			{
				return;
			}
			if (!this.TryAccessMachine(uid, component, machineComponent, args.User, args.Target.Value))
			{
				return;
			}
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float delay = (float)component.RestockDelay.TotalSeconds;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, delay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				BreakOnDamage = true,
				NeedHand = true
			});
			this._popupSystem.PopupEntity(Loc.GetString("vending-machine-restock-start", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("this", uid),
				new ValueTuple<string, object>("user", args.User),
				new ValueTuple<string, object>("target", args.Target)
			}), args.User, PopupType.Medium);
			this._audioSystem.PlayPvs(component.SoundRestockStart, component.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f).WithVariation(new float?(0.2f))));
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001481C File Offset: 0x00012A1C
		private void OnPriceCalculation(EntityUid uid, VendingMachineRestockComponent component, ref PriceCalculationEvent args)
		{
			List<double> priceSets = new List<double>();
			foreach (string vendingInventory in component.CanRestock)
			{
				double total = 0.0;
				VendingMachineInventoryPrototype inventoryPrototype;
				if (this._prototypeManager.TryIndex<VendingMachineInventoryPrototype>(vendingInventory, ref inventoryPrototype))
				{
					foreach (KeyValuePair<string, uint> keyValuePair in inventoryPrototype.StartingInventory)
					{
						string text;
						uint num;
						keyValuePair.Deconstruct(out text, out num);
						string item = text;
						uint amount = num;
						EntityPrototype entity;
						if (this._prototypeManager.TryIndex<EntityPrototype>(item, ref entity))
						{
							total += this._pricingSystem.GetEstimatedPrice(entity, null) * amount;
						}
					}
				}
				priceSets.Add(total);
			}
			args.Price += priceSets.Max();
		}

		// Token: 0x04000263 RID: 611
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000264 RID: 612
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000265 RID: 613
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000266 RID: 614
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x04000267 RID: 615
		[Dependency]
		private readonly PricingSystem _pricingSystem;
	}
}
