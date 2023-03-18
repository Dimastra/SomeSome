using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Disease;
using Content.Server.DoAfter;
using Content.Server.Medical.Components;
using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.MedicalScanner;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Medical
{
	// Token: 0x020003B0 RID: 944
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HealthAnalyzerSystem : EntitySystem
	{
		// Token: 0x06001367 RID: 4967 RVA: 0x000642C8 File Offset: 0x000624C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HealthAnalyzerComponent, ActivateInWorldEvent>(new ComponentEventHandler<HealthAnalyzerComponent, ActivateInWorldEvent>(this.HandleActivateInWorld), null, null);
			base.SubscribeLocalEvent<HealthAnalyzerComponent, AfterInteractEvent>(new ComponentEventHandler<HealthAnalyzerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<HealthAnalyzerComponent, DoAfterEvent>(new ComponentEventHandler<HealthAnalyzerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x00064317 File Offset: 0x00062517
		private void HandleActivateInWorld(EntityUid uid, HealthAnalyzerComponent healthAnalyzer, ActivateInWorldEvent args)
		{
			this.OpenUserInterface(args.User, healthAnalyzer);
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x00064328 File Offset: 0x00062528
		private void OnAfterInteract(EntityUid uid, HealthAnalyzerComponent healthAnalyzer, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach || !base.HasComp<MobStateComponent>(args.Target))
			{
				return;
			}
			this._audio.PlayPvs(healthAnalyzer.ScanningBeginSound, uid, null);
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float scanDelay = healthAnalyzer.ScanDelay;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, scanDelay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x000643CC File Offset: 0x000625CC
		private void OnDoAfter(EntityUid uid, HealthAnalyzerComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			this._audio.PlayPvs(component.ScanningEndSound, args.Args.User, null);
			this.UpdateScannedUser(uid, args.Args.User, new EntityUid?(args.Args.Target.Value), component);
			if (string.IsNullOrEmpty(component.Disease))
			{
				args.Handled = true;
				return;
			}
			this._disease.TryAddDisease(args.Args.Target.Value, component.Disease, null);
			if (args.Args.User == args.Args.Target)
			{
				this._popupSystem.PopupEntity(Loc.GetString("disease-scanner-gave-self", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("disease", component.Disease)
				}), args.Args.User, args.Args.User, PopupType.Small);
			}
			else
			{
				this._popupSystem.PopupEntity(Loc.GetString("disease-scanner-gave-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(args.Args.Target.Value, this.EntityManager)),
					new ValueTuple<string, object>("disease", component.Disease)
				}), args.Args.User, args.Args.User, PopupType.Small);
			}
			args.Handled = true;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x00064580 File Offset: 0x00062780
		private void OpenUserInterface(EntityUid user, HealthAnalyzerComponent healthAnalyzer)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor) || healthAnalyzer.UserInterface == null)
			{
				return;
			}
			this._uiSystem.OpenUi(healthAnalyzer.UserInterface, actor.PlayerSession);
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x000645BC File Offset: 0x000627BC
		[NullableContext(2)]
		public void UpdateScannedUser(EntityUid uid, EntityUid user, EntityUid? target, HealthAnalyzerComponent healthAnalyzer)
		{
			if (!base.Resolve<HealthAnalyzerComponent>(uid, ref healthAnalyzer, true))
			{
				return;
			}
			if (target == null || healthAnalyzer.UserInterface == null)
			{
				return;
			}
			if (!base.HasComp<DamageableComponent>(target))
			{
				return;
			}
			this.OpenUserInterface(user, healthAnalyzer);
			this._uiSystem.SendUiMessage(healthAnalyzer.UserInterface, new SharedHealthAnalyzerComponent.HealthAnalyzerScannedUserMessage(target));
		}

		// Token: 0x04000BD0 RID: 3024
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000BD1 RID: 3025
		[Dependency]
		private readonly DiseaseSystem _disease;

		// Token: 0x04000BD2 RID: 3026
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000BD3 RID: 3027
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000BD4 RID: 3028
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
