using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Fluids.Components;
using Content.Server.Popups;
using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004EE RID: 1262
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MoppingSystem : SharedMoppingSystem
	{
		// Token: 0x060019F3 RID: 6643 RVA: 0x00088490 File Offset: 0x00086690
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AbsorbentComponent, ComponentInit>(new ComponentEventHandler<AbsorbentComponent, ComponentInit>(this.OnAbsorbentInit), null, null);
			base.SubscribeLocalEvent<AbsorbentComponent, AfterInteractEvent>(new ComponentEventHandler<AbsorbentComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<AbsorbentComponent, DoAfterEvent<MoppingSystem.AbsorbantData>>(new ComponentEventHandler<AbsorbentComponent, DoAfterEvent<MoppingSystem.AbsorbantData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x000884DF File Offset: 0x000866DF
		private void OnAbsorbentInit(EntityUid uid, AbsorbentComponent component, ComponentInit args)
		{
			this.UpdateAbsorbent(uid, component);
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x000884E9 File Offset: 0x000866E9
		private void OnAbsorbentSolutionChange(EntityUid uid, AbsorbentComponent component, SolutionChangedEvent args)
		{
			this.UpdateAbsorbent(uid, component);
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x000884F4 File Offset: 0x000866F4
		private void UpdateAbsorbent(EntityUid uid, AbsorbentComponent component)
		{
			Solution solution;
			if (!this._solutionSystem.TryGetSolution(uid, "absorbed", out solution, null))
			{
				return;
			}
			float oldProgress = component.Progress;
			component.Progress = (float)(solution.Volume / solution.MaxVolume);
			if (component.Progress.Equals(oldProgress))
			{
				return;
			}
			base.Dirty(component, null);
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x00088554 File Offset: 0x00086754
		private void OnAfterInteract(EntityUid uid, AbsorbentComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach || args.Handled)
			{
				return;
			}
			Solution absorberSoln;
			if (!this._solutionSystem.TryGetSolution(args.Used, "absorbed", out absorberSoln, null))
			{
				return;
			}
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				if (target.Valid)
				{
					args.Handled = (this.TryPuddleInteract(args.User, uid, target, component, absorberSoln) || this.TryEmptyAbsorber(args.User, uid, target, component, absorberSoln) || this.TryFillAbsorber(args.User, uid, target, component, absorberSoln));
					return;
				}
			}
			args.Handled = this.TryCreatePuddle(args.User, args.ClickLocation, component, absorberSoln);
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x00088608 File Offset: 0x00086808
		private bool TryCreatePuddle(EntityUid user, EntityCoordinates clickLocation, AbsorbentComponent absorbent, Solution absorberSoln)
		{
			if (absorberSoln.Volume <= 0)
			{
				return false;
			}
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(clickLocation.GetGridUid(this.EntityManager), ref mapGrid))
			{
				return false;
			}
			FixedPoint2 releaseAmount = FixedPoint2.Min(absorbent.ResidueAmount, absorberSoln.Volume);
			Solution releasedSolution = this._solutionSystem.SplitSolution(absorbent.Owner, absorberSoln, releaseAmount);
			this._spillableSystem.SpillAt(mapGrid.GetTileRef(clickLocation), releasedSolution, "PuddleSmear", true, true, false, true);
			this._popups.PopupEntity(Loc.GetString("mopping-system-release-to-floor"), user, user, PopupType.Small);
			return true;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x000886A4 File Offset: 0x000868A4
		private bool TryFillAbsorber(EntityUid user, EntityUid used, EntityUid target, AbsorbentComponent component, Solution absorberSoln)
		{
			DrainableSolutionComponent drainable;
			if (absorberSoln.AvailableVolume <= 0 || !base.TryComp<DrainableSolutionComponent>(target, ref drainable))
			{
				return false;
			}
			Solution drainableSolution;
			if (!this._solutionSystem.TryGetDrainableSolution(target, out drainableSolution, null, null))
			{
				return false;
			}
			if (drainableSolution.Volume <= 0)
			{
				string msg = Loc.GetString("mopping-system-target-container-empty", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", target)
				});
				this._popups.PopupEntity(msg, user, user, PopupType.Small);
				return true;
			}
			FixedPoint2 quantity = FixedPoint2.Max(component.PickupAmount, absorberSoln.AvailableVolume / 2f);
			quantity = FixedPoint2.Min(quantity, drainableSolution.Volume);
			this.DoMopInteraction(user, used, target, component, drainable.Solution, quantity, 1f, "mopping-system-drainable-success", component.TransferSound);
			return true;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x00088778 File Offset: 0x00086978
		private bool TryEmptyAbsorber(EntityUid user, EntityUid used, EntityUid target, AbsorbentComponent component, Solution absorberSoln)
		{
			RefillableSolutionComponent refillable;
			if (absorberSoln.Volume <= 0 || !base.TryComp<RefillableSolutionComponent>(target, ref refillable))
			{
				return false;
			}
			Solution targetSolution;
			if (!this._solutionSystem.TryGetRefillableSolution(target, out targetSolution, null, null))
			{
				return false;
			}
			string msg;
			if (targetSolution.AvailableVolume <= 0)
			{
				msg = Loc.GetString("mopping-system-target-container-full", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", target)
				});
				this._popups.PopupEntity(msg, user, user, PopupType.Small);
				return true;
			}
			if (targetSolution.MaxVolume <= FixedPoint2.New(20))
			{
				msg = Loc.GetString("mopping-system-target-container-too-small", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", target)
				});
				this._popups.PopupEntity(msg, user, user, PopupType.Small);
				return true;
			}
			FixedPoint2 quantity = absorberSoln.Volume;
			float delay;
			if (this._tagSystem.HasTag(used, "Mop") && !this._tagSystem.HasTag(target, "Wringer"))
			{
				delay = 5f;
				FixedPoint2 frac = quantity / absorberSoln.MaxVolume;
				if (frac > 0.25)
				{
					quantity *= 0.6;
				}
				if (frac > 0.5)
				{
					msg = "mopping-system-hand-squeeze-still-wet";
				}
				else if (frac > 0.5)
				{
					msg = "mopping-system-hand-squeeze-little-wet";
				}
				else
				{
					msg = "mopping-system-hand-squeeze-dry";
				}
			}
			else
			{
				msg = "mopping-system-refillable-success";
				delay = 1f;
			}
			quantity = -FixedPoint2.Min(targetSolution.AvailableVolume, quantity);
			this.DoMopInteraction(user, used, target, component, refillable.Solution, quantity, delay, msg, component.TransferSound);
			return true;
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x00088940 File Offset: 0x00086B40
		private bool TryPuddleInteract(EntityUid user, EntityUid used, EntityUid target, AbsorbentComponent absorber, Solution absorberSoln)
		{
			PuddleComponent puddle;
			if (!base.TryComp<PuddleComponent>(target, ref puddle))
			{
				return false;
			}
			Solution puddleSolution;
			if (!this._solutionSystem.TryGetSolution(target, puddle.SolutionName, out puddleSolution, null) || puddleSolution.Volume <= 0)
			{
				return false;
			}
			FixedPoint2 lowerLimit = FixedPoint2.Zero;
			EvaporationComponent evaporation;
			if (base.TryComp<EvaporationComponent>(target, ref evaporation) && evaporation.EvaporationToggle && evaporation.LowerLimit == 0)
			{
				lowerLimit = absorber.LowerLimit;
			}
			if (puddleSolution.Volume <= lowerLimit)
			{
				FixedPoint2 quantity = FixedPoint2.Min(absorber.ResidueAmount, absorberSoln.Volume);
				if (quantity <= 0)
				{
					return false;
				}
				this._solutionSystem.TryTransferSolution(used, target, absorberSoln, puddleSolution, quantity);
				this._audio.PlayPvs(absorber.TransferSound, used, null);
				this._popups.PopupEntity(Loc.GetString("mopping-system-puddle-diluted"), user, PopupType.Small);
				return true;
			}
			else
			{
				if (absorberSoln.AvailableVolume < 0)
				{
					this._popups.PopupEntity(Loc.GetString("mopping-system-tool-full", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("used", used)
					}), user, user, PopupType.Small);
					return true;
				}
				FixedPoint2 quantity = FixedPoint2.Min(new FixedPoint2[]
				{
					absorber.PickupAmount,
					puddleSolution.Volume - lowerLimit,
					absorberSoln.AvailableVolume
				});
				if (quantity <= 0)
				{
					return false;
				}
				float delay = absorber.PickupAmount.Float() / absorber.Speed;
				this.DoMopInteraction(user, used, target, absorber, puddle.SolutionName, quantity, delay, "mopping-system-puddle-success", absorber.PickupSound);
				return true;
			}
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x00088AEC File Offset: 0x00086CEC
		private void DoMopInteraction(EntityUid user, EntityUid used, EntityUid target, AbsorbentComponent component, string targetSolution, FixedPoint2 transferAmount, float delay, string msg, SoundSpecifier sfx)
		{
			if (component.MaxInteractingEntities < component.InteractingEntities.Count + 1)
			{
				return;
			}
			if (!component.InteractingEntities.Add(target))
			{
				return;
			}
			MoppingSystem.AbsorbantData aborbantData = new MoppingSystem.AbsorbantData(targetSolution, msg, sfx, transferAmount);
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used2 = new EntityUid?(used);
			DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used2)
			{
				BreakOnUserMove = true,
				BreakOnStun = true,
				BreakOnDamage = true,
				MovementThreshold = 0.2f
			};
			this._doAfterSystem.DoAfter<MoppingSystem.AbsorbantData>(doAfterArgs, aborbantData);
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x00088B84 File Offset: 0x00086D84
		private void OnDoAfter(EntityUid uid, AbsorbentComponent component, DoAfterEvent<MoppingSystem.AbsorbantData> args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			this._audio.PlayPvs(args.AdditionalData.Sound, uid, null);
			this._popups.PopupEntity(Loc.GetString(args.AdditionalData.Message, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", args.Args.Target.Value),
				new ValueTuple<string, object>("used", uid)
			}), uid, PopupType.Small);
			this._solutionSystem.TryTransferSolution(args.Args.Target.Value, uid, args.AdditionalData.TargetSolution, "absorbed", args.AdditionalData.TransferAmount);
			component.InteractingEntities.Remove(args.Args.Target.Value);
			args.Handled = true;
		}

		// Token: 0x04001051 RID: 4177
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04001052 RID: 4178
		[Dependency]
		private readonly SpillableSystem _spillableSystem;

		// Token: 0x04001053 RID: 4179
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04001054 RID: 4180
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001055 RID: 4181
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x04001056 RID: 4182
		[Dependency]
		private readonly PopupSystem _popups;

		// Token: 0x04001057 RID: 4183
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04001058 RID: 4184
		private const string PuddlePrototypeId = "PuddleSmear";

		// Token: 0x020009FB RID: 2555
		[NullableContext(0)]
		private struct AbsorbantData : IEquatable<MoppingSystem.AbsorbantData>
		{
			// Token: 0x06003400 RID: 13312 RVA: 0x0010971A File Offset: 0x0010791A
			[NullableContext(1)]
			public AbsorbantData(string TargetSolution, string Message, SoundSpecifier Sound, FixedPoint2 TransferAmount)
			{
				this.TargetSolution = TargetSolution;
				this.Message = Message;
				this.Sound = Sound;
				this.TransferAmount = TransferAmount;
			}

			// Token: 0x06003401 RID: 13313 RVA: 0x0010973C File Offset: 0x0010793C
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AbsorbantData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003402 RID: 13314 RVA: 0x00109788 File Offset: 0x00107988
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("TargetSolution = ");
				builder.Append(this.TargetSolution);
				builder.Append(", Message = ");
				builder.Append(this.Message);
				builder.Append(", Sound = ");
				builder.Append(this.Sound);
				builder.Append(", TransferAmount = ");
				builder.Append(this.TransferAmount.ToString());
				return true;
			}

			// Token: 0x06003403 RID: 13315 RVA: 0x00109805 File Offset: 0x00107A05
			[CompilerGenerated]
			public static bool operator !=(MoppingSystem.AbsorbantData left, MoppingSystem.AbsorbantData right)
			{
				return !(left == right);
			}

			// Token: 0x06003404 RID: 13316 RVA: 0x00109811 File Offset: 0x00107A11
			[CompilerGenerated]
			public static bool operator ==(MoppingSystem.AbsorbantData left, MoppingSystem.AbsorbantData right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003405 RID: 13317 RVA: 0x0010981C File Offset: 0x00107A1C
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<string>.Default.GetHashCode(this.TargetSolution) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Message)) * -1521134295 + EqualityComparer<SoundSpecifier>.Default.GetHashCode(this.Sound)) * -1521134295 + EqualityComparer<FixedPoint2>.Default.GetHashCode(this.TransferAmount);
			}

			// Token: 0x06003406 RID: 13318 RVA: 0x0010987E File Offset: 0x00107A7E
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is MoppingSystem.AbsorbantData && this.Equals((MoppingSystem.AbsorbantData)obj);
			}

			// Token: 0x06003407 RID: 13319 RVA: 0x00109898 File Offset: 0x00107A98
			[CompilerGenerated]
			public readonly bool Equals(MoppingSystem.AbsorbantData other)
			{
				return EqualityComparer<string>.Default.Equals(this.TargetSolution, other.TargetSolution) && EqualityComparer<string>.Default.Equals(this.Message, other.Message) && EqualityComparer<SoundSpecifier>.Default.Equals(this.Sound, other.Sound) && EqualityComparer<FixedPoint2>.Default.Equals(this.TransferAmount, other.TransferAmount);
			}

			// Token: 0x06003408 RID: 13320 RVA: 0x00109905 File Offset: 0x00107B05
			[NullableContext(1)]
			[CompilerGenerated]
			public readonly void Deconstruct(out string TargetSolution, out string Message, out SoundSpecifier Sound, out FixedPoint2 TransferAmount)
			{
				TargetSolution = this.TargetSolution;
				Message = this.Message;
				Sound = this.Sound;
				TransferAmount = this.TransferAmount;
			}

			// Token: 0x040022E0 RID: 8928
			[Nullable(1)]
			public readonly string TargetSolution;

			// Token: 0x040022E1 RID: 8929
			[Nullable(1)]
			public readonly string Message;

			// Token: 0x040022E2 RID: 8930
			[Nullable(1)]
			public readonly SoundSpecifier Sound;

			// Token: 0x040022E3 RID: 8931
			public readonly FixedPoint2 TransferAmount;
		}
	}
}
