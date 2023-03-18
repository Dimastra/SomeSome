using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Disease.Components;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Paper;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Station.Systems;
using Content.Shared.Disease;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Disease
{
	// Token: 0x02000561 RID: 1377
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiseaseDiagnosisSystem : EntitySystem
	{
		// Token: 0x06001D22 RID: 7458 RVA: 0x0009A998 File Offset: 0x00098B98
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DiseaseSwabComponent, AfterInteractEvent>(new ComponentEventHandler<DiseaseSwabComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<DiseaseSwabComponent, ExaminedEvent>(new ComponentEventHandler<DiseaseSwabComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<DiseaseDiagnoserComponent, AfterInteractUsingEvent>(new ComponentEventHandler<DiseaseDiagnoserComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
			base.SubscribeLocalEvent<DiseaseVaccineCreatorComponent, AfterInteractUsingEvent>(new ComponentEventHandler<DiseaseVaccineCreatorComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsingVaccine), null, null);
			base.SubscribeLocalEvent<DiseaseMachineComponent, PowerChangedEvent>(new ComponentEventRefHandler<DiseaseMachineComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<DiseaseDiagnoserComponent, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent>(new ComponentEventHandler<DiseaseDiagnoserComponent, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent>(this.OnDiagnoserFinished), null, null);
			base.SubscribeLocalEvent<DiseaseVaccineCreatorComponent, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent>(new ComponentEventHandler<DiseaseVaccineCreatorComponent, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent>(this.OnVaccinatorFinished), null, null);
			base.SubscribeLocalEvent<DiseaseSwabComponent, DoAfterEvent>(new ComponentEventHandler<DiseaseSwabComponent, DoAfterEvent>(this.OnSwabDoAfter), null, null);
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x0009AA4C File Offset: 0x00098C4C
		public override void Update(float frameTime)
		{
			foreach (EntityUid uid in this.AddQueue)
			{
				base.EnsureComp<DiseaseMachineRunningComponent>(uid);
			}
			this.AddQueue.Clear();
			foreach (EntityUid uid2 in this.RemoveQueue)
			{
				base.RemComp<DiseaseMachineRunningComponent>(uid2);
			}
			this.RemoveQueue.Clear();
			foreach (ValueTuple<DiseaseMachineRunningComponent, DiseaseMachineComponent> valueTuple in base.EntityQuery<DiseaseMachineRunningComponent, DiseaseMachineComponent>(false))
			{
				DiseaseMachineComponent diseaseMachine = valueTuple.Item2;
				diseaseMachine.Accumulator += frameTime;
				while (diseaseMachine.Accumulator >= diseaseMachine.Delay)
				{
					diseaseMachine.Accumulator -= diseaseMachine.Delay;
					DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent ev = new DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent(diseaseMachine);
					base.RaiseLocalEvent<DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent>(diseaseMachine.Owner, ev, false);
					this.RemoveQueue.Enqueue(diseaseMachine.Owner);
				}
			}
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x0009AB98 File Offset: 0x00098D98
		private void OnAfterInteract(EntityUid uid, DiseaseSwabComponent swab, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach || !base.HasComp<DiseaseCarrierComponent>(args.Target))
			{
				return;
			}
			if (swab.Used)
			{
				this._popupSystem.PopupEntity(Loc.GetString("swab-already-used"), args.User, args.User, PopupType.Small);
				return;
			}
			EntityUid? maskUid;
			IngestionBlockerComponent blocker;
			if (this._inventorySystem.TryGetSlotEntity(args.Target.Value, "mask", out maskUid, null, null) && this.EntityManager.TryGetComponent<IngestionBlockerComponent>(maskUid, ref blocker) && blocker.Enabled)
			{
				this._popupSystem.PopupEntity(Loc.GetString("swab-mask-blocked", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(args.Target.Value, this.EntityManager)),
					new ValueTuple<string, object>("mask", maskUid)
				}), args.User, args.User, PopupType.Small);
				return;
			}
			bool isTarget = args.User != args.Target;
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float swabDelay = swab.SwabDelay;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, swabDelay, default(CancellationToken), target, used)
			{
				RaiseOnTarget = isTarget,
				RaiseOnUser = !isTarget,
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x0009AD38 File Offset: 0x00098F38
		private void OnAfterInteractUsing(EntityUid uid, DiseaseDiagnoserComponent component, AfterInteractUsingEvent args)
		{
			DiseaseMachineComponent machine = base.Comp<DiseaseMachineComponent>(uid);
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (base.HasComp<DiseaseMachineRunningComponent>(uid) || !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (!base.HasComp<HandsComponent>(args.User) || base.HasComp<ToolComponent>(args.Used))
			{
				return;
			}
			DiseaseSwabComponent swab;
			if (!base.TryComp<DiseaseSwabComponent>(args.Used, ref swab))
			{
				this._popupSystem.PopupEntity(Loc.GetString("diagnoser-cant-use-swab", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("machine", uid),
					new ValueTuple<string, object>("swab", args.Used)
				}), uid, args.User, PopupType.Small);
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString("machine-insert-item", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("machine", uid),
				new ValueTuple<string, object>("item", args.Used),
				new ValueTuple<string, object>("user", args.User)
			}), uid, args.User, PopupType.Small);
			machine.Disease = swab.Disease;
			this.EntityManager.DeleteEntity(args.Used);
			this.AddQueue.Enqueue(uid);
			this.UpdateAppearance(uid, true, true);
			SoundSystem.Play("/Audio/Machines/diagnoser_printing.ogg", Filter.Pvs(uid, 2f, null, null, null), uid, null);
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x0009AEC4 File Offset: 0x000990C4
		private void OnAfterInteractUsingVaccine(EntityUid uid, DiseaseVaccineCreatorComponent component, AfterInteractUsingEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (base.HasComp<DiseaseMachineRunningComponent>(uid) || !this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (!base.HasComp<HandsComponent>(args.User) || base.HasComp<ToolComponent>(args.Used))
			{
				return;
			}
			DiseaseSwabComponent swab;
			if (!base.TryComp<DiseaseSwabComponent>(args.Used, ref swab) || swab.Disease == null || !swab.Disease.Infectious)
			{
				this._popupSystem.PopupEntity(Loc.GetString("diagnoser-cant-use-swab", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("machine", uid),
					new ValueTuple<string, object>("swab", args.Used)
				}), uid, args.User, PopupType.Small);
				return;
			}
			this._popupSystem.PopupEntity(Loc.GetString("machine-insert-item", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("machine", uid),
				new ValueTuple<string, object>("item", args.Used),
				new ValueTuple<string, object>("user", args.User)
			}), uid, args.User, PopupType.Small);
			base.Comp<DiseaseMachineComponent>(uid).Disease = swab.Disease;
			this.EntityManager.DeleteEntity(args.Used);
			this.AddQueue.Enqueue(uid);
			this.UpdateAppearance(uid, true, true);
			SoundSystem.Play("/Audio/Machines/vaccinator_running.ogg", Filter.Pvs(uid, 2f, null, null, null), uid, null);
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x0009B062 File Offset: 0x00099262
		private void OnExamined(EntityUid uid, DiseaseSwabComponent swab, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				if (swab.Used)
				{
					args.PushMarkup(Loc.GetString("swab-used"));
					return;
				}
				args.PushMarkup(Loc.GetString("swab-unused"));
			}
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x0009B098 File Offset: 0x00099298
		private FormattedMessage AssembleDiseaseReport(DiseasePrototype disease)
		{
			FormattedMessage report = new FormattedMessage();
			string diseaseName = Loc.GetString(disease.Name);
			report.AddMarkup(Loc.GetString("diagnoser-disease-report-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("disease", diseaseName)
			}));
			report.PushNewline();
			if (disease.Infectious)
			{
				report.AddMarkup(Loc.GetString("diagnoser-disease-report-infectious"));
				report.PushNewline();
			}
			else
			{
				report.AddMarkup(Loc.GetString("diagnoser-disease-report-not-infectious"));
				report.PushNewline();
			}
			string cureResistLine = string.Empty;
			string str = cureResistLine;
			float cureResist = disease.CureResist;
			string @string;
			if (cureResist >= 0f)
			{
				if (cureResist > 0.05f)
				{
					if (cureResist > 0.14f)
					{
						@string = Loc.GetString("diagnoser-disease-report-cureresist-high");
					}
					else
					{
						@string = Loc.GetString("diagnoser-disease-report-cureresist-medium");
					}
				}
				else
				{
					@string = Loc.GetString("diagnoser-disease-report-cureresist-low");
				}
			}
			else
			{
				@string = Loc.GetString("diagnoser-disease-report-cureresist-none");
			}
			cureResistLine = str + @string;
			report.AddMarkup(cureResistLine);
			report.PushNewline();
			if (disease.Cures.Count == 0)
			{
				report.AddMarkup(Loc.GetString("diagnoser-no-cures"));
			}
			else
			{
				report.PushNewline();
				report.AddMarkup(Loc.GetString("diagnoser-cure-has"));
				report.PushNewline();
				foreach (DiseaseCure cure in disease.Cures)
				{
					report.AddMarkup(cure.CureText());
					report.PushNewline();
				}
			}
			return report;
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x0009B220 File Offset: 0x00099420
		public bool ServerHasDisease(DiseaseServerComponent server, DiseasePrototype disease)
		{
			bool has = false;
			using (List<DiseasePrototype>.Enumerator enumerator = server.Diseases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ID == disease.ID)
					{
						has = true;
					}
				}
			}
			return has;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x0009B284 File Offset: 0x00099484
		private void UpdateAppearance(EntityUid uid, bool isOn, bool isRunning)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, DiseaseMachineVisuals.IsOn, isOn, appearance);
			this._appearance.SetData(uid, DiseaseMachineVisuals.IsRunning, isRunning, appearance);
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x0009B2CF File Offset: 0x000994CF
		private void OnPowerChanged(EntityUid uid, DiseaseMachineComponent component, ref PowerChangedEvent args)
		{
			this.UpdateAppearance(uid, args.Powered, false);
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x0009B2E0 File Offset: 0x000994E0
		private void OnSwabDoAfter(EntityUid uid, DiseaseSwabComponent component, DoAfterEvent args)
		{
			DiseaseCarrierComponent carrier;
			DiseaseSwabComponent swab;
			if (args.Handled || args.Cancelled || !base.TryComp<DiseaseCarrierComponent>(args.Args.Target, ref carrier) || !base.TryComp<DiseaseSwabComponent>(args.Args.Used, ref swab))
			{
				return;
			}
			swab.Used = true;
			this._popupSystem.PopupEntity(Loc.GetString("swab-swabbed", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", Identity.Entity(args.Args.Target.Value, this.EntityManager))
			}), args.Args.Target.Value, args.Args.User, PopupType.Small);
			if (swab.Disease != null || carrier.Diseases.Count == 0)
			{
				return;
			}
			swab.Disease = RandomExtensions.Pick<DiseasePrototype>(this._random, carrier.Diseases);
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x0009B3C4 File Offset: 0x000995C4
		private void OnDiagnoserFinished(EntityUid uid, DiseaseDiagnoserComponent component, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent args)
		{
			bool isPowered = this.IsPowered(uid, this.EntityManager, null);
			this.UpdateAppearance(uid, isPowered, false);
			EntityUid printed = base.Spawn(args.Machine.MachineOutput, base.Transform(uid).Coordinates);
			PaperComponent paper;
			if (!base.TryComp<PaperComponent>(printed, ref paper))
			{
				return;
			}
			FormattedMessage contents = new FormattedMessage();
			string reportTitle;
			if (args.Machine.Disease != null)
			{
				string diseaseName = Loc.GetString(args.Machine.Disease.Name);
				reportTitle = Loc.GetString("diagnoser-disease-report", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("disease", diseaseName)
				});
				contents = this.AssembleDiseaseReport(args.Machine.Disease);
				bool known = false;
				foreach (DiseaseServerComponent server in base.EntityQuery<DiseaseServerComponent>(true))
				{
					if (!(this._stationSystem.GetOwningStation(server.Owner, null) != this._stationSystem.GetOwningStation(uid, null)))
					{
						if (this.ServerHasDisease(server, args.Machine.Disease))
						{
							known = true;
						}
						else
						{
							server.Diseases.Add(args.Machine.Disease);
						}
					}
				}
				if (!known)
				{
					base.Spawn("ResearchDisk5000", base.Transform(uid).Coordinates);
				}
			}
			else
			{
				reportTitle = Loc.GetString("diagnoser-disease-report-none");
				contents.AddMarkup(Loc.GetString("diagnoser-disease-report-none-contents"));
			}
			base.MetaData(printed).EntityName = reportTitle;
			this._paperSystem.SetContent(printed, contents.ToMarkup(), paper);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0009B5A8 File Offset: 0x000997A8
		private void OnVaccinatorFinished(EntityUid uid, DiseaseVaccineCreatorComponent component, DiseaseDiagnosisSystem.DiseaseMachineFinishedEvent args)
		{
			this.UpdateAppearance(uid, this.IsPowered(uid, this.EntityManager, null), false);
			EntityUid vaxx = base.Spawn(args.Machine.MachineOutput, base.Transform(uid).Coordinates);
			DiseaseVaccineComponent vaxxComp;
			if (!base.TryComp<DiseaseVaccineComponent>(vaxx, ref vaxxComp))
			{
				return;
			}
			vaxxComp.Disease = args.Machine.Disease;
		}

		// Token: 0x04001298 RID: 4760
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001299 RID: 4761
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400129A RID: 4762
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400129B RID: 4763
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x0400129C RID: 4764
		[Dependency]
		private readonly PaperSystem _paperSystem;

		// Token: 0x0400129D RID: 4765
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x0400129E RID: 4766
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400129F RID: 4767
		private Queue<EntityUid> AddQueue = new Queue<EntityUid>();

		// Token: 0x040012A0 RID: 4768
		private Queue<EntityUid> RemoveQueue = new Queue<EntityUid>();

		// Token: 0x02000A20 RID: 2592
		[Nullable(0)]
		private sealed class DiseaseMachineFinishedEvent : EntityEventArgs
		{
			// Token: 0x17000834 RID: 2100
			// (get) Token: 0x0600344F RID: 13391 RVA: 0x0010AA84 File Offset: 0x00108C84
			public DiseaseMachineComponent Machine { get; }

			// Token: 0x06003450 RID: 13392 RVA: 0x0010AA8C File Offset: 0x00108C8C
			public DiseaseMachineFinishedEvent(DiseaseMachineComponent machine)
			{
				this.Machine = machine;
			}
		}
	}
}
