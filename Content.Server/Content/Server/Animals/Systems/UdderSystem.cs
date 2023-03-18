using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Animals.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Animals.Systems
{
	// Token: 0x020007D0 RID: 2000
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class UdderSystem : EntitySystem
	{
		// Token: 0x06002B7B RID: 11131 RVA: 0x000E4124 File Offset: 0x000E2324
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<UdderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<UdderComponent, GetVerbsEvent<AlternativeVerb>>(this.AddMilkVerb), null, null);
			base.SubscribeLocalEvent<UdderComponent, DoAfterEvent>(new ComponentEventHandler<UdderComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06002B7C RID: 11132 RVA: 0x000E4154 File Offset: 0x000E2354
		public override void Update(float frameTime)
		{
			foreach (UdderComponent udder in this.EntityManager.EntityQuery<UdderComponent>(false))
			{
				udder.AccumulatedFrameTime += frameTime;
				while (udder.AccumulatedFrameTime > udder.UpdateRate)
				{
					udder.AccumulatedFrameTime -= udder.UpdateRate;
					HungerComponent hunger;
					if (this.EntityManager.TryGetComponent<HungerComponent>(udder.Owner, ref hunger))
					{
						float targetThreshold;
						hunger.HungerThresholds.TryGetValue(HungerThreshold.Peckish, out targetThreshold);
						if (hunger.CurrentHunger < targetThreshold)
						{
							continue;
						}
					}
					Solution solution;
					if (this._solutionContainerSystem.TryGetSolution(udder.Owner, udder.TargetSolutionName, out solution, null))
					{
						FixedPoint2 accepted;
						this._solutionContainerSystem.TryAddReagent(udder.Owner, solution, udder.ReagentId, udder.QuantityPerUpdate, out accepted, null);
					}
				}
			}
		}

		// Token: 0x06002B7D RID: 11133 RVA: 0x000E4254 File Offset: 0x000E2454
		[NullableContext(2)]
		private void AttemptMilk(EntityUid uid, EntityUid userUid, EntityUid containerUid, UdderComponent udder = null)
		{
			if (!base.Resolve<UdderComponent>(uid, ref udder, true))
			{
				return;
			}
			if (udder.BeingMilked)
			{
				this._popupSystem.PopupEntity(Loc.GetString("udder-system-already-milking"), uid, userUid, PopupType.Small);
				return;
			}
			udder.BeingMilked = true;
			float delay = 5f;
			EntityUid? target = new EntityUid?(uid);
			EntityUid? used = new EntityUid?(containerUid);
			DoAfterEventArgs doargs = new DoAfterEventArgs(userUid, delay, default(CancellationToken), target, used)
			{
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true,
				MovementThreshold = 1f
			};
			this._doAfterSystem.DoAfter(doargs);
		}

		// Token: 0x06002B7E RID: 11134 RVA: 0x000E42F4 File Offset: 0x000E24F4
		private void OnDoAfter(EntityUid uid, UdderComponent component, DoAfterEvent args)
		{
			if (args.Cancelled)
			{
				component.BeingMilked = false;
				return;
			}
			if (args.Handled || args.Args.Used == null)
			{
				return;
			}
			component.BeingMilked = false;
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, component.TargetSolutionName, out solution, null))
			{
				return;
			}
			Solution targetSolution;
			if (!this._solutionContainerSystem.TryGetRefillableSolution(args.Args.Used.Value, out targetSolution, null, null))
			{
				return;
			}
			FixedPoint2 quantity = solution.Volume;
			if (quantity == 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("udder-system-dry"), uid, args.Args.User, PopupType.Small);
				return;
			}
			if (quantity > targetSolution.AvailableVolume)
			{
				quantity = targetSolution.AvailableVolume;
			}
			Solution split = this._solutionContainerSystem.SplitSolution(uid, solution, quantity);
			this._solutionContainerSystem.TryAddSolution(args.Args.Used.Value, targetSolution, split);
			this._popupSystem.PopupEntity(Loc.GetString("udder-system-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", quantity),
				new ValueTuple<string, object>("target", Identity.Entity(args.Args.Used.Value, this.EntityManager))
			}), uid, args.Args.User, PopupType.Medium);
			args.Handled = true;
		}

		// Token: 0x06002B7F RID: 11135 RVA: 0x000E445C File Offset: 0x000E265C
		private void AddMilkVerb(EntityUid uid, UdderComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (args.Using == null || !args.CanInteract || !this.EntityManager.HasComponent<RefillableSolutionComponent>(args.Using.Value))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.AttemptMilk(uid, args.User, args.Using.Value, component);
				},
				Text = Loc.GetString("udder-system-verb-milk"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x04001AEA RID: 6890
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04001AEB RID: 6891
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001AEC RID: 6892
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
