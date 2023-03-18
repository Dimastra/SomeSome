using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069A RID: 1690
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionContainerSystem : EntitySystem
	{
		// Token: 0x06002328 RID: 9000 RVA: 0x000B7A24 File Offset: 0x000B5C24
		public void Refill(EntityUid targetUid, Solution targetSolution, Solution addedSolution, [Nullable(2)] RefillableSolutionComponent refillableSolution = null)
		{
			if (!base.Resolve<RefillableSolutionComponent>(targetUid, ref refillableSolution, false))
			{
				return;
			}
			this.TryAddSolution(targetUid, targetSolution, addedSolution);
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x000B7A3D File Offset: 0x000B5C3D
		public void Inject(EntityUid targetUid, Solution targetSolution, Solution addedSolution, [Nullable(2)] InjectableSolutionComponent injectableSolution = null)
		{
			if (!base.Resolve<InjectableSolutionComponent>(targetUid, ref injectableSolution, false))
			{
				return;
			}
			this.TryAddSolution(targetUid, targetSolution, addedSolution);
		}

		// Token: 0x0600232A RID: 9002 RVA: 0x000B7A56 File Offset: 0x000B5C56
		public Solution Draw(EntityUid targetUid, Solution solution, FixedPoint2 amount, [Nullable(2)] DrawableSolutionComponent drawableSolution = null)
		{
			if (!base.Resolve<DrawableSolutionComponent>(targetUid, ref drawableSolution, false))
			{
				return new Solution();
			}
			return this.SplitSolution(targetUid, solution, amount);
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x000B7A73 File Offset: 0x000B5C73
		public Solution Drain(EntityUid targetUid, Solution targetSolution, FixedPoint2 amount, [Nullable(2)] DrainableSolutionComponent drainableSolution = null)
		{
			if (!base.Resolve<DrainableSolutionComponent>(targetUid, ref drainableSolution, false))
			{
				return new Solution();
			}
			return this.SplitSolution(targetUid, targetSolution, amount);
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x000B7A90 File Offset: 0x000B5C90
		[NullableContext(2)]
		public bool TryGetInjectableSolution(EntityUid targetUid, [NotNullWhen(true)] out Solution solution, InjectableSolutionComponent injectable = null, SolutionContainerManagerComponent manager = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent, InjectableSolutionComponent>(targetUid, ref manager, ref injectable, false) || !manager.Solutions.TryGetValue(injectable.Solution, out solution))
			{
				solution = null;
				return false;
			}
			return true;
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x000B7ABC File Offset: 0x000B5CBC
		[NullableContext(2)]
		public bool TryGetRefillableSolution(EntityUid targetUid, [NotNullWhen(true)] out Solution solution, SolutionContainerManagerComponent solutionManager = null, RefillableSolutionComponent refillable = null)
		{
			Solution refillableSolution;
			if (!base.Resolve<SolutionContainerManagerComponent, RefillableSolutionComponent>(targetUid, ref solutionManager, ref refillable, false) || !solutionManager.Solutions.TryGetValue(refillable.Solution, out refillableSolution))
			{
				solution = null;
				return false;
			}
			solution = refillableSolution;
			return true;
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x000B7AF6 File Offset: 0x000B5CF6
		[NullableContext(2)]
		public bool TryGetDrainableSolution(EntityUid uid, [NotNullWhen(true)] out Solution solution, DrainableSolutionComponent drainable = null, SolutionContainerManagerComponent manager = null)
		{
			if (!base.Resolve<DrainableSolutionComponent, SolutionContainerManagerComponent>(uid, ref drainable, ref manager, false) || !manager.Solutions.TryGetValue(drainable.Solution, out solution))
			{
				solution = null;
				return false;
			}
			return true;
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x000B7B21 File Offset: 0x000B5D21
		[NullableContext(2)]
		public bool TryGetDrawableSolution(EntityUid uid, [NotNullWhen(true)] out Solution solution, DrawableSolutionComponent drawable = null, SolutionContainerManagerComponent manager = null)
		{
			if (!base.Resolve<DrawableSolutionComponent, SolutionContainerManagerComponent>(uid, ref drawable, ref manager, false) || !manager.Solutions.TryGetValue(drawable.Solution, out solution))
			{
				solution = null;
				return false;
			}
			return true;
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x000B7B4C File Offset: 0x000B5D4C
		public FixedPoint2 DrainAvailable(EntityUid uid)
		{
			Solution solution;
			if (this.TryGetDrainableSolution(uid, out solution, null, null))
			{
				return solution.Volume;
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x000B7B74 File Offset: 0x000B5D74
		public float PercentFull(EntityUid uid)
		{
			Solution solution;
			if (!this.TryGetDrainableSolution(uid, out solution, null, null) || solution.MaxVolume.Equals(FixedPoint2.Zero))
			{
				return 0f;
			}
			return solution.FillFraction * 100f;
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x000B7BB5 File Offset: 0x000B5DB5
		[NullableContext(2)]
		public bool TryGetFitsInDispenser(EntityUid owner, [NotNullWhen(true)] out Solution solution, FitsInDispenserComponent dispenserFits = null, SolutionContainerManagerComponent solutionManager = null)
		{
			if (!base.Resolve<FitsInDispenserComponent, SolutionContainerManagerComponent>(owner, ref dispenserFits, ref solutionManager, false) || !solutionManager.Solutions.TryGetValue(dispenserFits.Solution, out solution))
			{
				solution = null;
				return false;
			}
			return true;
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x000B7BE0 File Offset: 0x000B5DE0
		public static string ToPrettyString(Solution solution)
		{
			StringBuilder sb = new StringBuilder();
			if (solution.Name == null)
			{
				sb.Append("[");
			}
			else
			{
				StringBuilder stringBuilder = sb;
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder);
				appendInterpolatedStringHandler.AppendFormatted(solution.Name);
				appendInterpolatedStringHandler.AppendLiteral(":[");
				stringBuilder2.Append(ref appendInterpolatedStringHandler);
			}
			bool first = true;
			foreach (Solution.ReagentQuantity reagentQuantity in solution.Contents)
			{
				string text;
				FixedPoint2 fixedPoint;
				reagentQuantity.Deconstruct(out text, out fixedPoint);
				string id = text;
				FixedPoint2 quantity = fixedPoint;
				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append(", ");
				}
				sb.AppendFormat("{0}: {1}u", id, quantity);
			}
			sb.Append(']');
			return sb.ToString();
		}

		// Token: 0x06002334 RID: 9012 RVA: 0x000B7CC4 File Offset: 0x000B5EC4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentInit>(new ComponentEventHandler<SolutionContainerManagerComponent, ComponentInit>(this.InitSolution), null, null);
			base.SubscribeLocalEvent<ExaminableSolutionComponent, ExaminedEvent>(new ComponentEventHandler<ExaminableSolutionComponent, ExaminedEvent>(this.OnExamineSolution), null, null);
		}

		// Token: 0x06002335 RID: 9013 RVA: 0x000B7CF4 File Offset: 0x000B5EF4
		private void InitSolution(EntityUid uid, SolutionContainerManagerComponent component, ComponentInit args)
		{
			foreach (KeyValuePair<string, Solution> keyValuePair in component.Solutions)
			{
				string text;
				Solution solution;
				keyValuePair.Deconstruct(out text, out solution);
				string name = text;
				Solution solutionHolder = solution;
				solutionHolder.Name = name;
				solutionHolder.ValidateSolution();
				this.UpdateAppearance(uid, solutionHolder, null);
			}
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x000B7D68 File Offset: 0x000B5F68
		private void OnExamineSolution(EntityUid uid, ExaminableSolutionComponent examinableComponent, ExaminedEvent args)
		{
			SolutionContainerManagerComponent solutionsManager = null;
			Solution solutionHolder;
			if (!base.Resolve<SolutionContainerManagerComponent>(args.Examined, ref solutionsManager, true) || !solutionsManager.Solutions.TryGetValue(examinableComponent.Solution, out solutionHolder))
			{
				return;
			}
			string primaryReagent = solutionHolder.GetPrimaryReagentId();
			if (string.IsNullOrEmpty(primaryReagent))
			{
				args.PushText(Loc.GetString("shared-solution-container-component-on-examine-empty-container"));
				return;
			}
			ReagentPrototype proto;
			if (!this._prototypeManager.TryIndex<ReagentPrototype>(primaryReagent, ref proto))
			{
				Logger.Error("Solution could not find the prototype associated with " + primaryReagent + ".");
				return;
			}
			string colorHex = solutionHolder.GetColor(this._prototypeManager).ToHexNoAlpha();
			string messageString = "shared-solution-container-component-on-examine-main-text";
			args.PushMarkup(Loc.GetString(messageString, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", colorHex),
				new ValueTuple<string, object>("wordedAmount", Loc.GetString((solutionHolder.Contents.Count == 1) ? "shared-solution-container-component-on-examine-worded-amount-one-reagent" : "shared-solution-container-component-on-examine-worded-amount-multiple-reagents")),
				new ValueTuple<string, object>("desc", proto.LocalizedPhysicalDescription)
			}));
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x000B7E70 File Offset: 0x000B6070
		public void UpdateAppearance(EntityUid uid, Solution solution, [Nullable(2)] AppearanceComponent appearanceComponent = null)
		{
			if (!this.EntityManager.EntityExists(uid) || !base.Resolve<AppearanceComponent>(uid, ref appearanceComponent, false))
			{
				return;
			}
			this._appearance.SetData(uid, SolutionContainerVisuals.FillFraction, solution.FillFraction, appearanceComponent);
			this._appearance.SetData(uid, SolutionContainerVisuals.Color, solution.GetColor(this._prototypeManager), appearanceComponent);
			string reagent = solution.GetPrimaryReagentId();
			if (reagent != null)
			{
				this._appearance.SetData(uid, SolutionContainerVisuals.BaseOverride, reagent, appearanceComponent);
				return;
			}
			this._appearance.SetData(uid, SolutionContainerVisuals.BaseOverride, string.Empty, appearanceComponent);
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x000B7F11 File Offset: 0x000B6111
		public Solution SplitSolution(EntityUid targetUid, Solution solutionHolder, FixedPoint2 quantity)
		{
			Solution result = solutionHolder.SplitSolution(quantity);
			this.UpdateChemicals(targetUid, solutionHolder, false, null);
			return result;
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x000B7F24 File Offset: 0x000B6124
		public void UpdateChemicals(EntityUid uid, Solution solutionHolder, bool needsReactionsProcessing = false, [Nullable(2)] ReactionMixerComponent mixerComponent = null)
		{
			if (needsReactionsProcessing && solutionHolder.CanReact)
			{
				this._chemistrySystem.FullyReactSolution(solutionHolder, uid, solutionHolder.MaxVolume, mixerComponent);
			}
			this.UpdateAppearance(uid, solutionHolder, null);
			base.RaiseLocalEvent<SolutionChangedEvent>(uid, new SolutionChangedEvent(solutionHolder), false);
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x000B7F5D File Offset: 0x000B615D
		public void RemoveAllSolution(EntityUid uid, Solution solutionHolder)
		{
			if (solutionHolder.Volume == 0)
			{
				return;
			}
			solutionHolder.RemoveAllSolution();
			this.UpdateChemicals(uid, solutionHolder, false, null);
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x000B7F80 File Offset: 0x000B6180
		[NullableContext(2)]
		public void RemoveAllSolution(EntityUid uid, SolutionContainerManagerComponent solutionContainerManager = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionContainerManager, true))
			{
				return;
			}
			foreach (Solution solution in solutionContainerManager.Solutions.Values)
			{
				this.RemoveAllSolution(uid, solution);
			}
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x000B7FE8 File Offset: 0x000B61E8
		public void SetCapacity(EntityUid targetUid, Solution targetSolution, FixedPoint2 capacity)
		{
			if (targetSolution.MaxVolume == capacity)
			{
				return;
			}
			targetSolution.MaxVolume = capacity;
			if (capacity < targetSolution.Volume)
			{
				targetSolution.RemoveSolution(targetSolution.Volume - capacity);
			}
			this.UpdateChemicals(targetUid, targetSolution, false, null);
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x000B8038 File Offset: 0x000B6238
		public bool TryAddReagent(EntityUid targetUid, Solution targetSolution, string reagentId, FixedPoint2 quantity, out FixedPoint2 acceptedQuantity, float? temperature = null)
		{
			acceptedQuantity = ((targetSolution.AvailableVolume > quantity) ? quantity : targetSolution.AvailableVolume);
			if (acceptedQuantity <= 0)
			{
				return quantity == 0;
			}
			if (temperature == null)
			{
				targetSolution.AddReagent(reagentId, acceptedQuantity, true);
			}
			else
			{
				targetSolution.AddReagent(this._prototypeManager.Index<ReagentPrototype>(reagentId), acceptedQuantity, temperature.Value, this._prototypeManager);
			}
			this.UpdateChemicals(targetUid, targetSolution, true, null);
			return acceptedQuantity == quantity;
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x000B80D5 File Offset: 0x000B62D5
		public bool TryRemoveReagent(EntityUid targetUid, [Nullable(2)] Solution container, string reagentId, FixedPoint2 quantity)
		{
			if (container == null || !container.ContainsReagent(reagentId))
			{
				return false;
			}
			container.RemoveReagent(reagentId, quantity);
			this.UpdateChemicals(targetUid, container, false, null);
			return true;
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x000B80FA File Offset: 0x000B62FA
		public bool TryAddSolution(EntityUid targetUid, [Nullable(2)] Solution targetSolution, Solution addedSolution)
		{
			if (targetSolution == null || !targetSolution.CanAddSolution(addedSolution) || addedSolution.Volume == 0)
			{
				return false;
			}
			targetSolution.AddSolution(addedSolution, this._prototypeManager);
			this.UpdateChemicals(targetUid, targetSolution, true, null);
			return true;
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x000B8130 File Offset: 0x000B6330
		public bool TryTransferSolution(EntityUid sourceUid, EntityUid targetUid, Solution source, Solution target, FixedPoint2 quantity)
		{
			if (quantity < 0)
			{
				return this.TryTransferSolution(targetUid, sourceUid, target, source, -quantity);
			}
			quantity = FixedPoint2.Min(new FixedPoint2[]
			{
				quantity,
				target.AvailableVolume,
				source.Volume
			});
			if (quantity == 0)
			{
				return false;
			}
			target.AddSolution(source.SplitSolution(quantity), this._prototypeManager);
			this.UpdateChemicals(sourceUid, source, false, null);
			this.UpdateChemicals(targetUid, target, true, null);
			return true;
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x000B81C4 File Offset: 0x000B63C4
		public bool TryTransferSolution(EntityUid sourceUid, EntityUid targetUid, string source, string target, FixedPoint2 quantity)
		{
			Solution sourceSoln;
			Solution targetSoln;
			return this.TryGetSolution(sourceUid, source, out sourceSoln, null) && this.TryGetSolution(targetUid, target, out targetSoln, null) && this.TryTransferSolution(sourceUid, targetUid, sourceSoln, targetSoln, quantity);
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000B81FC File Offset: 0x000B63FC
		public bool TryMixAndOverflow(EntityUid targetUid, Solution targetSolution, Solution addedSolution, FixedPoint2 overflowThreshold, [Nullable(2)] [NotNullWhen(true)] out Solution overflowingSolution)
		{
			if (addedSolution.Volume == 0 || overflowThreshold > targetSolution.MaxVolume)
			{
				overflowingSolution = null;
				return false;
			}
			targetSolution.AddSolution(addedSolution, this._prototypeManager);
			this.UpdateChemicals(targetUid, targetSolution, true, null);
			overflowingSolution = targetSolution.SplitSolution(FixedPoint2.Max(FixedPoint2.Zero, targetSolution.Volume - overflowThreshold));
			return true;
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000B8264 File Offset: 0x000B6464
		[NullableContext(2)]
		public bool TryGetSolution(EntityUid uid, [Nullable(1)] string name, [NotNullWhen(true)] out Solution solution, SolutionContainerManagerComponent solutionsMgr = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionsMgr, false))
			{
				solution = null;
				return false;
			}
			return solutionsMgr.Solutions.TryGetValue(name, out solution);
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x000B8288 File Offset: 0x000B6488
		public Solution EnsureSolution(EntityUid uid, string name, out bool existed, [Nullable(2)] SolutionContainerManagerComponent solutionsMgr = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionsMgr, false))
			{
				solutionsMgr = this.EntityManager.EnsureComponent<SolutionContainerManagerComponent>(uid);
			}
			Solution existing;
			if (!solutionsMgr.Solutions.TryGetValue(name, out existing))
			{
				Solution newSolution = new Solution
				{
					Name = name
				};
				solutionsMgr.Solutions.Add(name, newSolution);
				existed = false;
				return newSolution;
			}
			existed = true;
			return existing;
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x000B82E4 File Offset: 0x000B64E4
		public Solution EnsureSolution(EntityUid uid, string name, [Nullable(2)] SolutionContainerManagerComponent solutionsMgr = null)
		{
			bool flag;
			return this.EnsureSolution(uid, name, out flag, solutionsMgr);
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x000B82FC File Offset: 0x000B64FC
		public Solution EnsureSolution(EntityUid uid, string name, FixedPoint2 minVol, out bool existed, [Nullable(2)] SolutionContainerManagerComponent solutionsMgr = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionsMgr, false))
			{
				solutionsMgr = this.EntityManager.EnsureComponent<SolutionContainerManagerComponent>(uid);
			}
			Solution existing;
			if (!solutionsMgr.Solutions.TryGetValue(name, out existing))
			{
				Solution newSolution = new Solution
				{
					Name = name
				};
				solutionsMgr.Solutions.Add(name, newSolution);
				existed = false;
				newSolution.MaxVolume = minVol;
				return newSolution;
			}
			existed = true;
			existing.MaxVolume = FixedPoint2.Max(existing.MaxVolume, minVol);
			return existing;
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x000B8374 File Offset: 0x000B6574
		public Solution EnsureSolution(EntityUid uid, string name, IEnumerable<Solution.ReagentQuantity> reagents, bool setMaxVol = true, [Nullable(2)] SolutionContainerManagerComponent solutionsMgr = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionsMgr, false))
			{
				solutionsMgr = this.EntityManager.EnsureComponent<SolutionContainerManagerComponent>(uid);
			}
			Solution existing;
			if (!solutionsMgr.Solutions.TryGetValue(name, out existing))
			{
				Solution newSolution = new Solution(reagents, setMaxVol);
				solutionsMgr.Solutions.Add(name, newSolution);
				return newSolution;
			}
			existing.SetContents(reagents, setMaxVol);
			return existing;
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x000B83D0 File Offset: 0x000B65D0
		public Solution RemoveEachReagent(EntityUid uid, Solution solution, FixedPoint2 quantity)
		{
			if (quantity <= 0)
			{
				return new Solution();
			}
			Solution removedSolution = new Solution();
			for (int i = solution.Contents.Count - 1; i >= 0; i--)
			{
				string text;
				FixedPoint2 fixedPoint;
				solution.Contents[i].Deconstruct(out text, out fixedPoint);
				string reagentId = text;
				FixedPoint2 removedQuantity = solution.RemoveReagent(reagentId, quantity);
				if (removedQuantity > 0)
				{
					removedSolution.AddReagent(reagentId, removedQuantity, true);
				}
			}
			this.UpdateChemicals(uid, solution, false, null);
			return removedSolution;
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000B844C File Offset: 0x000B664C
		public FixedPoint2 GetReagentQuantity(EntityUid owner, string reagentId)
		{
			FixedPoint2 reagentQuantity = FixedPoint2.New(0);
			SolutionContainerManagerComponent managerComponent;
			if (this.EntityManager.EntityExists(owner) && this.EntityManager.TryGetComponent<SolutionContainerManagerComponent>(owner, ref managerComponent))
			{
				foreach (Solution solution in managerComponent.Solutions.Values)
				{
					reagentQuantity += solution.GetReagentQuantity(reagentId);
				}
			}
			return reagentQuantity;
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x000B84D4 File Offset: 0x000B66D4
		[NullableContext(2)]
		public bool TryGetMixableSolution(EntityUid uid, [NotNullWhen(true)] out Solution solution, SolutionContainerManagerComponent solutionsMgr = null)
		{
			if (!base.Resolve<SolutionContainerManagerComponent>(uid, ref solutionsMgr, false))
			{
				solution = null;
				return false;
			}
			GetMixableSolutionAttemptEvent getMixableSolutionAttempt = new GetMixableSolutionAttemptEvent(uid, null);
			base.RaiseLocalEvent<GetMixableSolutionAttemptEvent>(uid, ref getMixableSolutionAttempt, false);
			if (getMixableSolutionAttempt.MixedSolution != null)
			{
				solution = getMixableSolutionAttempt.MixedSolution;
				return true;
			}
			KeyValuePair<string, Solution>? tryGetSolution = Extensions.FirstOrNull<KeyValuePair<string, Solution>>(solutionsMgr.Solutions, (KeyValuePair<string, Solution> x) => x.Value.CanMix);
			if (tryGetSolution != null)
			{
				solution = tryGetSolution.Value.Value;
				return true;
			}
			solution = null;
			return false;
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x000B8564 File Offset: 0x000B6764
		public void SetTemperature(EntityUid owner, Solution solution, float temperature)
		{
			if (temperature == solution.Temperature)
			{
				return;
			}
			solution.Temperature = temperature;
			this.UpdateChemicals(owner, solution, true, null);
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x000B8584 File Offset: 0x000B6784
		public void SetThermalEnergy(EntityUid owner, Solution solution, float thermalEnergy)
		{
			float heatCap = solution.GetHeatCapacity(this._prototypeManager);
			solution.Temperature = ((heatCap == 0f) ? 0f : (thermalEnergy / heatCap));
			this.UpdateChemicals(owner, solution, true, null);
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x000B85C0 File Offset: 0x000B67C0
		public void AddThermalEnergy(EntityUid owner, Solution solution, float thermalEnergy)
		{
			if (thermalEnergy == 0f)
			{
				return;
			}
			float heatCap = solution.GetHeatCapacity(this._prototypeManager);
			solution.Temperature += ((heatCap == 0f) ? 0f : (thermalEnergy / heatCap));
			this.UpdateChemicals(owner, solution, true, null);
		}

		// Token: 0x040015B9 RID: 5561
		[Dependency]
		private readonly SharedChemicalReactionSystem _chemistrySystem;

		// Token: 0x040015BA RID: 5562
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040015BB RID: 5563
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
