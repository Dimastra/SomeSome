using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Light.Components;
using Content.Server.Light.EntitySystems;
using Content.Server.Radio;
using Content.Server.Radio.Components;
using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018F RID: 399
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolarFlare : StationEventSystem
	{
		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x000274A1 File Offset: 0x000256A1
		public override string Prototype
		{
			get
			{
				return "SolarFlare";
			}
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000274A8 File Offset: 0x000256A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActiveRadioComponent, RadioReceiveAttemptEvent>(new ComponentEventHandler<ActiveRadioComponent, RadioReceiveAttemptEvent>(this.OnRadioSendAttempt), null, null);
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x000274C4 File Offset: 0x000256C4
		public override void Added()
		{
			base.Added();
			SolarFlareEventRuleConfiguration ev = this.Configuration as SolarFlareEventRuleConfiguration;
			if (ev == null)
			{
				return;
			}
			this._event = ev;
			this._event.EndAfter = (float)this.RobustRandom.Next(ev.MinEndAfter, ev.MaxEndAfter);
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00027511 File Offset: 0x00025711
		public override void Started()
		{
			base.Started();
			this.MessLights();
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00027520 File Offset: 0x00025720
		private void MessLights()
		{
			foreach (PoweredLightComponent comp in base.EntityQuery<PoweredLightComponent>(false))
			{
				if (RandomExtensions.Prob(this.RobustRandom, this._event.LightBreakChance))
				{
					EntityUid uid = comp.Owner;
					this._poweredLight.TryDestroyBulb(uid, comp);
				}
			}
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00027594 File Offset: 0x00025794
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!base.RuleStarted)
			{
				return;
			}
			if (base.Elapsed > this._event.EndAfter)
			{
				base.ForceEndSelf();
				return;
			}
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x000275C0 File Offset: 0x000257C0
		private void OnRadioSendAttempt(EntityUid uid, ActiveRadioComponent component, RadioReceiveAttemptEvent args)
		{
			if (base.RuleStarted && this._event.AffectedChannels.Contains(args.Channel.ID) && (!this._event.OnlyJamHeadsets || base.HasComp<HeadsetComponent>(uid) || base.HasComp<HeadsetComponent>(args.RadioSource)))
			{
				args.Cancel();
			}
		}

		// Token: 0x040004D4 RID: 1236
		[Dependency]
		private readonly PoweredLightSystem _poweredLight;

		// Token: 0x040004D5 RID: 1237
		private SolarFlareEventRuleConfiguration _event;
	}
}
