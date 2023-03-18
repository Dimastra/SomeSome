using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Coordinates.Helpers;
using Content.Shared.Audio;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x0200068B RID: 1675
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class AreaReactionEffect : ReagentEffect, ISerializationHooks
	{
		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x060022C4 RID: 8900 RVA: 0x000B4F6C File Offset: 0x000B316C
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000B4F6F File Offset: 0x000B316F
		public override LogImpact LogImpact
		{
			get
			{
				return LogImpact.High;
			}
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x000B4F72 File Offset: 0x000B3172
		void ISerializationHooks.AfterDeserialization()
		{
			IoCManager.InjectDependencies<AreaReactionEffect>(this);
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x000B4F7C File Offset: 0x000B317C
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Source == null)
			{
				return;
			}
			Solution splitSolution = EntitySystem.Get<SolutionContainerSystem>().SplitSolution(args.SolutionEntity, args.Source, args.Source.Volume);
			int amount = (int)Math.Round((double)this._rangeConstant + (double)this._rangeMultiplier * Math.Sqrt((double)args.Quantity.Float()));
			amount = Math.Min(amount, this._maxRange);
			if (this._diluteReagents)
			{
				float solutionFraction = 1f / (this._reagentDilutionFactor * (float)amount + 1f);
				splitSolution.RemoveSolution(splitSolution.Volume * (1f - solutionFraction));
			}
			TransformComponent transform = args.EntityManager.GetComponent<TransformComponent>(args.SolutionEntity);
			MapGridComponent grid;
			if (!this._mapManager.TryFindGridAt(transform.MapPosition, ref grid))
			{
				return;
			}
			EntityCoordinates coords = grid.MapToGrid(transform.MapPosition);
			EntityUid ent = args.EntityManager.SpawnEntity(this._prototypeId, coords.SnapToGrid(null, null));
			SolutionAreaEffectComponent areaEffectComponent = this.GetAreaEffectComponent(ent);
			if (areaEffectComponent == null)
			{
				Logger.Error("Couldn't get AreaEffectComponent from " + this._prototypeId);
				IoCManager.Resolve<IEntityManager>().QueueDeleteEntity(ent);
				return;
			}
			areaEffectComponent.TryAddSolution(splitSolution);
			areaEffectComponent.Start(amount, this._duration, this._spreadDelay, this._removeDelay);
			SoundSystem.Play(this._sound.GetSound(null, null), Filter.Pvs(args.SolutionEntity, 2f, null, null, null), args.SolutionEntity, new AudioParams?(AudioHelpers.WithVariation(0.125f)));
		}

		// Token: 0x060022C8 RID: 8904
		[NullableContext(2)]
		protected abstract SolutionAreaEffectComponent GetAreaEffectComponent(EntityUid entity);

		// Token: 0x04001581 RID: 5505
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001582 RID: 5506
		[DataField("rangeConstant", false, 1, false, false, null)]
		private float _rangeConstant;

		// Token: 0x04001583 RID: 5507
		[DataField("rangeMultiplier", false, 1, false, false, null)]
		private float _rangeMultiplier = 1.1f;

		// Token: 0x04001584 RID: 5508
		[DataField("maxRange", false, 1, false, false, null)]
		private int _maxRange = 10;

		// Token: 0x04001585 RID: 5509
		[DataField("diluteReagents", false, 1, false, false, null)]
		private bool _diluteReagents;

		// Token: 0x04001586 RID: 5510
		[DataField("reagentDilutionFactor", false, 1, false, false, null)]
		private float _reagentDilutionFactor = 1f;

		// Token: 0x04001587 RID: 5511
		[DataField("duration", false, 1, false, false, null)]
		private float _duration = 10f;

		// Token: 0x04001588 RID: 5512
		[DataField("spreadDelay", false, 1, false, false, null)]
		private float _spreadDelay = 0.5f;

		// Token: 0x04001589 RID: 5513
		[DataField("removeDelay", false, 1, false, false, null)]
		private float _removeDelay = 0.5f;

		// Token: 0x0400158A RID: 5514
		[DataField("prototypeId", false, 1, true, false, null)]
		private string _prototypeId;

		// Token: 0x0400158B RID: 5515
		[DataField("sound", false, 1, true, false, null)]
		private SoundSpecifier _sound;
	}
}
