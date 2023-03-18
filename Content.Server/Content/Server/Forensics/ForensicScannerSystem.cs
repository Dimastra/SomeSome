using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Paper;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.DoAfter;
using Content.Shared.Forensics;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Timing;

namespace Content.Server.Forensics
{
	// Token: 0x020004E7 RID: 1255
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ForensicScannerSystem : EntitySystem
	{
		// Token: 0x060019CA RID: 6602 RVA: 0x000870A4 File Offset: 0x000852A4
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("forensics.scanner");
			base.SubscribeLocalEvent<ForensicScannerComponent, AfterInteractEvent>(new ComponentEventHandler<ForensicScannerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, AfterInteractUsingEvent>(new ComponentEventHandler<ForensicScannerComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, BeforeActivatableUIOpenEvent>(new ComponentEventHandler<ForensicScannerComponent, BeforeActivatableUIOpenEvent>(this.OnBeforeActivatableUIOpen), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<ForensicScannerComponent, GetVerbsEvent<UtilityVerb>>(this.OnUtilityVerb), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, ForensicScannerPrintMessage>(new ComponentEventHandler<ForensicScannerComponent, ForensicScannerPrintMessage>(this.OnPrint), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, ForensicScannerClearMessage>(new ComponentEventHandler<ForensicScannerComponent, ForensicScannerClearMessage>(this.OnClear), null, null);
			base.SubscribeLocalEvent<ForensicScannerComponent, DoAfterEvent>(new ComponentEventHandler<ForensicScannerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x00087154 File Offset: 0x00085354
		private void UpdateUserInterface(EntityUid uid, ForensicScannerComponent component)
		{
			ForensicScannerBoundUserInterfaceState state = new ForensicScannerBoundUserInterfaceState(component.Fingerprints, component.Fibers, component.LastScannedName, component.PrintCooldown, component.PrintReadyAt);
			if (!this._uiSystem.TrySetUiState(uid, ForensicScannerUiKey.Key, state, null, null, true))
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" was unable to set UI state.");
				sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x000871D4 File Offset: 0x000853D4
		private void OnDoAfter(EntityUid uid, ForensicScannerComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			ForensicScannerComponent scanner;
			if (!this.EntityManager.TryGetComponent<ForensicScannerComponent>(uid, ref scanner))
			{
				return;
			}
			if (args.Args.Target != null)
			{
				ForensicsComponent forensics;
				if (!base.TryComp<ForensicsComponent>(args.Args.Target, ref forensics))
				{
					scanner.Fingerprints = new List<string>();
					scanner.Fibers = new List<string>();
				}
				else
				{
					scanner.Fingerprints = forensics.Fingerprints.ToList<string>();
					scanner.Fibers = forensics.Fibers.ToList<string>();
				}
				scanner.LastScannedName = base.MetaData(args.Args.Target.Value).EntityName;
			}
			this.OpenUserInterface(args.Args.User, scanner);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00087298 File Offset: 0x00085498
		private void StartScan(EntityUid uid, ForensicScannerComponent component, EntityUid user, EntityUid target)
		{
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			float scanDelay = component.ScanDelay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, scanDelay, default(CancellationToken), target2, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x000872F8 File Offset: 0x000854F8
		private void OnUtilityVerb(EntityUid uid, ForensicScannerComponent component, GetVerbsEvent<UtilityVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess || component.CancelToken != null)
			{
				return;
			}
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate()
				{
					this.StartScan(uid, component, args.User, args.Target);
				},
				IconEntity = new EntityUid?(uid),
				Text = Loc.GetString("forensic-scanner-verb-text"),
				Message = Loc.GetString("forensic-scanner-verb-message")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x000873AC File Offset: 0x000855AC
		private void OnAfterInteract(EntityUid uid, ForensicScannerComponent component, AfterInteractEvent args)
		{
			if (component.CancelToken != null || args.Target == null || !args.CanReach)
			{
				return;
			}
			this.StartScan(uid, component, args.User, args.Target.Value);
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x000873F8 File Offset: 0x000855F8
		private void OnAfterInteractUsing(EntityUid uid, ForensicScannerComponent component, AfterInteractUsingEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			ForensicPadComponent pad;
			if (!base.TryComp<ForensicPadComponent>(args.Used, ref pad))
			{
				return;
			}
			using (List<string>.Enumerator enumerator = component.Fibers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == pad.Sample)
					{
						this._audioSystem.PlayPvs(component.SoundMatch, uid, null);
						this._popupSystem.PopupEntity(Loc.GetString("forensic-scanner-match-fiber"), uid, args.User, PopupType.Small);
						return;
					}
				}
			}
			using (List<string>.Enumerator enumerator = component.Fingerprints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == pad.Sample)
					{
						this._audioSystem.PlayPvs(component.SoundMatch, uid, null);
						this._popupSystem.PopupEntity(Loc.GetString("forensic-scanner-match-fingerprint"), uid, args.User, PopupType.Small);
						return;
					}
				}
			}
			this._audioSystem.PlayPvs(component.SoundNoMatch, uid, null);
			this._popupSystem.PopupEntity(Loc.GetString("forensic-scanner-match-none"), uid, args.User, PopupType.Small);
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x0008756C File Offset: 0x0008576C
		private void OnBeforeActivatableUIOpen(EntityUid uid, ForensicScannerComponent component, BeforeActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x00087578 File Offset: 0x00085778
		private void OpenUserInterface(EntityUid user, ForensicScannerComponent component)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			this.UpdateUserInterface(component.Owner, component);
			this._uiSystem.TryOpen(component.Owner, ForensicScannerUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x000875C0 File Offset: 0x000857C0
		private void OnPrint(EntityUid uid, ForensicScannerComponent component, ForensicScannerPrintMessage args)
		{
			if (args.Session.AttachedEntity == null)
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" got OnPrint without Session.AttachedEntity");
				sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			EntityUid user = args.Session.AttachedEntity.Value;
			if (this._gameTiming.CurTime < component.PrintReadyAt)
			{
				this._popupSystem.PopupEntity(Loc.GetString("forensic-scanner-printer-not-ready"), uid, user, PopupType.Small);
				return;
			}
			EntityUid printed = this.EntityManager.SpawnEntity("Paper", base.Transform(uid).Coordinates);
			this._handsSystem.PickupOrDrop(args.Session.AttachedEntity, printed, false, false, null, null);
			PaperComponent paper;
			if (!base.TryComp<PaperComponent>(printed, ref paper))
			{
				this._sawmill.Error("Printed paper did not have PaperComponent.");
				return;
			}
			base.MetaData(printed).EntityName = Loc.GetString("forensic-scanner-report-title", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("entity", component.LastScannedName)
			});
			StringBuilder text = new StringBuilder();
			text.AppendLine(Loc.GetString("forensic-scanner-interface-fingerprints"));
			foreach (string fingerprint in component.Fingerprints)
			{
				text.AppendLine(fingerprint);
			}
			text.AppendLine();
			text.AppendLine(Loc.GetString("forensic-scanner-interface-fibers"));
			foreach (string fiber in component.Fibers)
			{
				text.AppendLine(fiber);
			}
			this._paperSystem.SetContent(printed, text.ToString(), null);
			this._audioSystem.PlayPvs(component.SoundPrint, uid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.25f)).WithVolume(3f).WithRolloffFactor(2.8f).WithMaxDistance(4.5f)));
			component.PrintReadyAt = this._gameTiming.CurTime + component.PrintCooldown;
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x0008782C File Offset: 0x00085A2C
		private void OnClear(EntityUid uid, ForensicScannerComponent component, ForensicScannerClearMessage args)
		{
			if (args.Session.AttachedEntity == null)
			{
				return;
			}
			component.Fingerprints = new List<string>();
			component.Fibers = new List<string>();
			component.LastScannedName = string.Empty;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x0400103D RID: 4157
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400103E RID: 4158
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x0400103F RID: 4159
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04001040 RID: 4160
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001041 RID: 4161
		[Dependency]
		private readonly PaperSystem _paperSystem;

		// Token: 0x04001042 RID: 4162
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04001043 RID: 4163
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04001044 RID: 4164
		private ISawmill _sawmill;
	}
}
