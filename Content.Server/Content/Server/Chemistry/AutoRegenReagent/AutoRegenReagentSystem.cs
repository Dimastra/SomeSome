using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.AutoRegenReagent
{
	// Token: 0x0200064C RID: 1612
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AutoRegenReagentSystem : EntitySystem
	{
		// Token: 0x0600222F RID: 8751 RVA: 0x000B2E6C File Offset: 0x000B106C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AutoRegenReagentComponent, ComponentInit>(new ComponentEventHandler<AutoRegenReagentComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<AutoRegenReagentComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<AutoRegenReagentComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSwitchVerb), null, null);
			base.SubscribeLocalEvent<AutoRegenReagentComponent, ExaminedEvent>(new ComponentEventHandler<AutoRegenReagentComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x000B2EBC File Offset: 0x000B10BC
		private void OnInit(EntityUid uid, AutoRegenReagentComponent component, ComponentInit args)
		{
			if (component.SolutionName == null)
			{
				return;
			}
			Solution solution;
			if (this._solutionSystem.TryGetSolution(uid, component.SolutionName, out solution, null))
			{
				component.Solution = solution;
			}
			component.CurrentReagent = component.Reagents[component.CurrentIndex];
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x000B2F08 File Offset: 0x000B1108
		private void AddSwitchVerb(EntityUid uid, AutoRegenReagentComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			if (component.Reagents.Count <= 1)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.SwitchReagent(component, args.User);
				},
				Text = Loc.GetString("autoreagent-switch"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x000B2FA0 File Offset: 0x000B11A0
		private string SwitchReagent(AutoRegenReagentComponent component, EntityUid user)
		{
			if (component.CurrentIndex + 1 == component.Reagents.Count)
			{
				component.CurrentIndex = 0;
			}
			else
			{
				component.CurrentIndex++;
			}
			if (component.Solution != null)
			{
				component.Solution.ScaleSolution(0);
			}
			component.CurrentReagent = component.Reagents[component.CurrentIndex];
			this._popups.PopupEntity(Loc.GetString("autoregen-switched", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("reagent", component.CurrentReagent)
			}), user, user, PopupType.Small);
			return component.CurrentReagent;
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x000B303F File Offset: 0x000B123F
		private void OnExamined(EntityUid uid, AutoRegenReagentComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("reagent-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("reagent", component.CurrentReagent)
			}));
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x000B3070 File Offset: 0x000B1270
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (AutoRegenReagentComponent autoComp in base.EntityQuery<AutoRegenReagentComponent>(false))
			{
				if (autoComp.Solution == null)
				{
					break;
				}
				autoComp.Accumulator += frameTime;
				if (autoComp.Accumulator >= 1f)
				{
					autoComp.Accumulator -= 1f;
					FixedPoint2 accepted;
					this._solutionSystem.TryAddReagent(autoComp.Owner, autoComp.Solution, autoComp.CurrentReagent, autoComp.unitsPerSecond, out accepted, null);
				}
			}
		}

		// Token: 0x04001522 RID: 5410
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x04001523 RID: 5411
		[Dependency]
		private readonly PopupSystem _popups;
	}
}
