using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Forensics
{
	// Token: 0x020004E6 RID: 1254
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ForensicPadSystem : EntitySystem
	{
		// Token: 0x060019C4 RID: 6596 RVA: 0x00086BC8 File Offset: 0x00084DC8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ForensicPadComponent, ExaminedEvent>(new ComponentEventHandler<ForensicPadComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<ForensicPadComponent, AfterInteractEvent>(new ComponentEventHandler<ForensicPadComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<ForensicPadComponent, DoAfterEvent<ForensicPadSystem.ForensicPadData>>(new ComponentEventHandler<ForensicPadComponent, DoAfterEvent<ForensicPadSystem.ForensicPadData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00086C18 File Offset: 0x00084E18
		private void OnExamined(EntityUid uid, ForensicPadComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (!component.Used)
			{
				args.PushMarkup(Loc.GetString("forensic-pad-unused"));
				return;
			}
			args.PushMarkup(Loc.GetString("forensic-pad-sample", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("sample", component.Sample)
			}));
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x00086C74 File Offset: 0x00084E74
		private void OnAfterInteract(EntityUid uid, ForensicPadComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach || args.Target == null)
			{
				return;
			}
			if (base.HasComp<ForensicScannerComponent>(args.Target))
			{
				return;
			}
			args.Handled = true;
			if (component.Used)
			{
				this._popupSystem.PopupEntity(Loc.GetString("forensic-pad-already-used"), args.Target.Value, args.User, PopupType.Small);
				return;
			}
			EntityUid? gloves;
			if (this._inventory.TryGetSlotEntity(args.Target.Value, "gloves", out gloves, null, null))
			{
				this._popupSystem.PopupEntity(Loc.GetString("forensic-pad-gloves", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(args.Target.Value, this.EntityManager))
				}), args.Target.Value, args.User, PopupType.Small);
				return;
			}
			FingerprintComponent fingerprint;
			if (base.TryComp<FingerprintComponent>(args.Target, ref fingerprint) && fingerprint.Fingerprint != null)
			{
				if (args.User != args.Target)
				{
					this._popupSystem.PopupEntity(Loc.GetString("forensic-pad-start-scan-user", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(args.Target.Value, this.EntityManager))
					}), args.Target.Value, args.User, PopupType.Small);
					this._popupSystem.PopupEntity(Loc.GetString("forensic-pad-start-scan-target", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager))
					}), args.Target.Value, args.Target.Value, PopupType.Small);
				}
				this.StartScan(uid, args.User, args.Target.Value, component, fingerprint.Fingerprint);
				return;
			}
			FiberComponent fiber;
			if (base.TryComp<FiberComponent>(args.Target, ref fiber))
			{
				this.StartScan(uid, args.User, args.Target.Value, component, string.IsNullOrEmpty(fiber.FiberColor) ? Loc.GetString("forensic-fibers", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("material", fiber.FiberMaterial)
				}) : Loc.GetString("forensic-fibers-colored", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("color", fiber.FiberColor),
					new ValueTuple<string, object>("material", fiber.FiberMaterial)
				}));
			}
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x00086F38 File Offset: 0x00085138
		private void StartScan(EntityUid used, EntityUid user, EntityUid target, ForensicPadComponent pad, string sample)
		{
			ForensicPadSystem.ForensicPadData padData = new ForensicPadSystem.ForensicPadData(sample);
			float scanDelay = pad.ScanDelay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used2 = new EntityUid?(used);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, scanDelay, default(CancellationToken), target2, used2)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			};
			this._doAfterSystem.DoAfter<ForensicPadSystem.ForensicPadData>(doAfterEventArgs, padData);
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00086FA0 File Offset: 0x000851A0
		private void OnDoAfter(EntityUid uid, ForensicPadComponent component, DoAfterEvent<ForensicPadSystem.ForensicPadData> args)
		{
			ForensicPadComponent padComponent;
			if (args.Handled || args.Cancelled || !this.EntityManager.TryGetComponent<ForensicPadComponent>(args.Args.Used, ref padComponent))
			{
				return;
			}
			if (args.Args.Target != null)
			{
				if (base.HasComp<FingerprintComponent>(args.Args.Target))
				{
					base.MetaData(uid).EntityName = Loc.GetString("forensic-pad-fingerprint-name", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entity", args.Args.Target)
					});
				}
				else
				{
					base.MetaData(uid).EntityName = Loc.GetString("forensic-pad-gloves-name", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entity", args.Args.Target)
					});
				}
			}
			padComponent.Sample = args.AdditionalData.Sample;
			padComponent.Used = true;
			args.Handled = true;
		}

		// Token: 0x0400103A RID: 4154
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x0400103B RID: 4155
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x0400103C RID: 4156
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x020009F8 RID: 2552
		[Nullable(0)]
		private sealed class ForensicPadData
		{
			// Token: 0x060033FB RID: 13307 RVA: 0x001096B2 File Offset: 0x001078B2
			public ForensicPadData(string sample)
			{
				this.Sample = sample;
			}

			// Token: 0x040022D8 RID: 8920
			public string Sample;
		}
	}
}
