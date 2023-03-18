using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Content.Shared.StepTrigger.Components;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004F0 RID: 1264
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PuddleSystem : EntitySystem
	{
		// Token: 0x06001A03 RID: 6659 RVA: 0x00088F8C File Offset: 0x0008718C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PuddleComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<PuddleComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<PuddleComponent, ExaminedEvent>(new ComponentEventHandler<PuddleComponent, ExaminedEvent>(this.HandlePuddleExamined), null, null);
			base.SubscribeLocalEvent<PuddleComponent, SolutionChangedEvent>(new ComponentEventHandler<PuddleComponent, SolutionChangedEvent>(this.OnSolutionUpdate), null, null);
			base.SubscribeLocalEvent<PuddleComponent, ComponentInit>(new ComponentEventHandler<PuddleComponent, ComponentInit>(this.OnPuddleInit), null, null);
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x00088FF0 File Offset: 0x000871F0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EntityUid ent in this._deletionQueue)
			{
				base.Del(ent);
			}
			this._deletionQueue.Clear();
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x00089058 File Offset: 0x00087258
		private void OnPuddleInit(EntityUid uid, PuddleComponent component, ComponentInit args)
		{
			bool flag;
			this._solutionContainerSystem.EnsureSolution(uid, component.SolutionName, FixedPoint2.New(PuddleSystem.PuddleVolume), out flag, null);
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x00089088 File Offset: 0x00087288
		private void OnSolutionUpdate(EntityUid uid, PuddleComponent component, SolutionChangedEvent args)
		{
			if (args.Solution.Name != component.SolutionName)
			{
				return;
			}
			if (args.Solution.Volume <= 0)
			{
				this._deletionQueue.Add(uid);
				return;
			}
			this._deletionQueue.Remove(uid);
			this.UpdateSlip(uid, component);
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x000890F0 File Offset: 0x000872F0
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, PuddleComponent puddleComponent = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<PuddleComponent, AppearanceComponent>(uid, ref puddleComponent, ref appearance, false) || this.EmptyHolder(uid, puddleComponent))
			{
				return;
			}
			Solution puddleSolution = this._solutionContainerSystem.EnsureSolution(uid, puddleComponent.SolutionName, null);
			float volumeScale = puddleSolution.Volume.Float() / puddleComponent.OverflowVolume.Float() * puddleComponent.OpacityModifier;
			EvaporationComponent evaporation;
			bool isEvaporating = base.TryComp<EvaporationComponent>(uid, ref evaporation) && evaporation.EvaporationToggle;
			Color color = puddleSolution.GetColor(this._protoMan);
			this._appearance.SetData(uid, PuddleVisuals.VolumeScale, volumeScale, appearance);
			this._appearance.SetData(uid, PuddleVisuals.CurrentVolume, puddleSolution.Volume, appearance);
			this._appearance.SetData(uid, PuddleVisuals.SolutionColor, color, appearance);
			this._appearance.SetData(uid, PuddleVisuals.IsEvaporatingVisual, isEvaporating, appearance);
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000891E0 File Offset: 0x000873E0
		private void UpdateSlip(EntityUid entityUid, PuddleComponent puddleComponent)
		{
			FixedPoint2 vol = this.CurrentVolume(puddleComponent.Owner, puddleComponent);
			StepTriggerComponent stepTrigger;
			if ((puddleComponent.SlipThreshold == FixedPoint2.New(-1) || vol < puddleComponent.SlipThreshold) && base.TryComp<StepTriggerComponent>(entityUid, ref stepTrigger))
			{
				this._stepTrigger.SetActive(entityUid, false, stepTrigger);
				return;
			}
			if (vol >= puddleComponent.SlipThreshold)
			{
				StepTriggerComponent comp = base.EnsureComp<StepTriggerComponent>(entityUid);
				this._stepTrigger.SetActive(entityUid, true, comp);
			}
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x0008925C File Offset: 0x0008745C
		private void HandlePuddleExamined(EntityUid uid, PuddleComponent component, ExaminedEvent args)
		{
			StepTriggerComponent slippery;
			if (base.TryComp<StepTriggerComponent>(uid, ref slippery) && slippery.Active)
			{
				args.PushText(Loc.GetString("puddle-component-examine-is-slipper-text"));
			}
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x0008928C File Offset: 0x0008748C
		private void OnAnchorChanged(EntityUid uid, PuddleComponent puddle, ref AnchorStateChangedEvent args)
		{
			if (!args.Anchored)
			{
				base.QueueDel(uid);
			}
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x000892A0 File Offset: 0x000874A0
		[NullableContext(2)]
		public bool EmptyHolder(EntityUid uid, PuddleComponent puddleComponent = null)
		{
			Solution solution;
			return !base.Resolve<PuddleComponent>(uid, ref puddleComponent, true) || !this._solutionContainerSystem.TryGetSolution(puddleComponent.Owner, puddleComponent.SolutionName, out solution, null) || solution.Contents.Count == 0;
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x000892E8 File Offset: 0x000874E8
		[NullableContext(2)]
		public FixedPoint2 CurrentVolume(EntityUid uid, PuddleComponent puddleComponent = null)
		{
			if (!base.Resolve<PuddleComponent>(uid, ref puddleComponent, true))
			{
				return FixedPoint2.Zero;
			}
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(puddleComponent.Owner, puddleComponent.SolutionName, out solution, null))
			{
				return FixedPoint2.Zero;
			}
			return solution.Volume;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x00089330 File Offset: 0x00087530
		public bool TryAddSolution(EntityUid puddleUid, Solution addedSolution, bool sound = true, bool checkForOverflow = true, [Nullable(2)] PuddleComponent puddleComponent = null)
		{
			if (!base.Resolve<PuddleComponent>(puddleUid, ref puddleComponent, true))
			{
				return false;
			}
			Solution solution;
			if (addedSolution.Volume == 0 || !this._solutionContainerSystem.TryGetSolution(puddleComponent.Owner, puddleComponent.SolutionName, out solution, null))
			{
				return false;
			}
			solution.AddSolution(addedSolution, this._protoMan);
			this._solutionContainerSystem.UpdateChemicals(puddleUid, solution, true, null);
			if (checkForOverflow && this.IsOverflowing(puddleUid, puddleComponent))
			{
				this._fluidSpreaderSystem.AddOverflowingPuddle(puddleComponent.Owner, puddleComponent, null);
			}
			if (!sound)
			{
				return true;
			}
			SoundSystem.Play(puddleComponent.SpillSound.GetSound(null, null), Filter.Pvs(puddleComponent.Owner, 2f, null, null, null), puddleComponent.Owner, null);
			return true;
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x000893F8 File Offset: 0x000875F8
		[NullableContext(2)]
		public void EqualizePuddles(EntityUid srcPuddle, [Nullable(1)] List<PuddleComponent> destinationPuddles, FixedPoint2 totalVolume, HashSet<EntityUid> stillOverflowing = null, PuddleComponent sourcePuddleComponent = null)
		{
			Solution srcSolution;
			if (!base.Resolve<PuddleComponent>(srcPuddle, ref sourcePuddleComponent, true) || !this._solutionContainerSystem.TryGetSolution(srcPuddle, sourcePuddleComponent.SolutionName, out srcSolution, null))
			{
				return;
			}
			FixedPoint2 dividedVolume = totalVolume / (float)(destinationPuddles.Count + 1);
			foreach (PuddleComponent destPuddle in destinationPuddles)
			{
				Solution destSolution;
				if (this._solutionContainerSystem.TryGetSolution(destPuddle.Owner, destPuddle.SolutionName, out destSolution, null))
				{
					FixedPoint2 takeAmount = FixedPoint2.Max(0, dividedVolume - destSolution.Volume);
					this.TryAddSolution(destPuddle.Owner, srcSolution.SplitSolution(takeAmount), false, false, destPuddle);
					if (stillOverflowing != null && this.IsOverflowing(destPuddle.Owner, destPuddle))
					{
						stillOverflowing.Add(destPuddle.Owner);
					}
				}
			}
			if (stillOverflowing != null && srcSolution.Volume > sourcePuddleComponent.OverflowVolume)
			{
				stillOverflowing.Add(srcPuddle);
			}
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x00089504 File Offset: 0x00087704
		public bool WouldOverflow(EntityUid uid, Solution solution, [Nullable(2)] PuddleComponent puddle = null)
		{
			return base.Resolve<PuddleComponent>(uid, ref puddle, true) && this.CurrentVolume(uid, puddle) + solution.Volume > puddle.OverflowVolume;
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x00089532 File Offset: 0x00087732
		[NullableContext(2)]
		private bool IsOverflowing(EntityUid uid, PuddleComponent puddle = null)
		{
			return base.Resolve<PuddleComponent>(uid, ref puddle, true) && this.CurrentVolume(uid, puddle) > puddle.OverflowVolume;
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x00089558 File Offset: 0x00087758
		public PuddleComponent SpawnPuddle(EntityUid srcUid, EntityCoordinates pos, [Nullable(2)] PuddleComponent srcPuddleComponent = null)
		{
			MetaDataComponent metadata = null;
			base.Resolve<PuddleComponent, MetaDataComponent>(srcUid, ref srcPuddleComponent, ref metadata, true);
			string text;
			if (metadata == null)
			{
				text = null;
			}
			else
			{
				EntityPrototype entityPrototype = metadata.EntityPrototype;
				text = ((entityPrototype != null) ? entityPrototype.ID : null);
			}
			string prototype = text ?? "PuddleSmear";
			EntityUid destUid = this.EntityManager.SpawnEntity(prototype, pos);
			return this.EntityManager.EnsureComponent<PuddleComponent>(destUid);
		}

		// Token: 0x0400105D RID: 4189
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x0400105E RID: 4190
		[Dependency]
		private readonly FluidSpreaderSystem _fluidSpreaderSystem;

		// Token: 0x0400105F RID: 4191
		[Dependency]
		private readonly StepTriggerSystem _stepTrigger;

		// Token: 0x04001060 RID: 4192
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001061 RID: 4193
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04001062 RID: 4194
		public static float PuddleVolume = 1000f;

		// Token: 0x04001063 RID: 4195
		private HashSet<EntityUid> _deletionQueue = new HashSet<EntityUid>();
	}
}
