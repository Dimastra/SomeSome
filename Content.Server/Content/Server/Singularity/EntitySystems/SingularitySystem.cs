using System;
using System.Runtime.CompilerServices;
using Content.Server.Physics.Components;
using Content.Server.Singularity.Components;
using Content.Server.Singularity.Events;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Content.Shared.Singularity.Events;
using Robust.Server.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001EE RID: 494
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SingularitySystem : SharedSingularitySystem
	{
		// Token: 0x06000992 RID: 2450 RVA: 0x00030980 File Offset: 0x0002EB80
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SingularityDistortionComponent, ComponentStartup>(new ComponentEventHandler<SingularityDistortionComponent, ComponentStartup>(this.OnDistortionStartup), null, null);
			base.SubscribeLocalEvent<SingularityComponent, ComponentShutdown>(new ComponentEventHandler<SingularityComponent, ComponentShutdown>(this.OnSingularityShutdown), null, null);
			base.SubscribeLocalEvent<SingularityComponent, EventHorizonConsumedEntityEvent>(new ComponentEventHandler<SingularityComponent, EventHorizonConsumedEntityEvent>(this.OnConsumed), null, null);
			base.SubscribeLocalEvent<SinguloFoodComponent, EventHorizonConsumedEntityEvent>(new ComponentEventHandler<SinguloFoodComponent, EventHorizonConsumedEntityEvent>(this.OnConsumed), null, null);
			base.SubscribeLocalEvent<SingularityComponent, EntityConsumedByEventHorizonEvent>(new ComponentEventHandler<SingularityComponent, EntityConsumedByEventHorizonEvent>(this.OnConsumedEntity), null, null);
			base.SubscribeLocalEvent<SingularityComponent, TilesConsumedByEventHorizonEvent>(new ComponentEventHandler<SingularityComponent, TilesConsumedByEventHorizonEvent>(this.OnConsumedTiles), null, null);
			base.SubscribeLocalEvent<SingularityComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<SingularityComponent, SingularityLevelChangedEvent>(this.UpdateEnergyDrain), null, null);
			base.SubscribeLocalEvent<SingularityComponent, ComponentGetState>(new ComponentEventRefHandler<SingularityComponent, ComponentGetState>(this.HandleSingularityState), null, null);
			base.SubscribeLocalEvent<RandomWalkComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<RandomWalkComponent, SingularityLevelChangedEvent>(this.UpdateRandomWalk), null, null);
			base.SubscribeLocalEvent<GravityWellComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<GravityWellComponent, SingularityLevelChangedEvent>(this.UpdateGravityWell), null, null);
			ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
			typeHandler.AddPath<float>("Energy", (EntityUid _, SingularityComponent comp) => comp.Energy, new ComponentPropertySetter<SingularityComponent, float>(this.SetEnergy));
			typeHandler.AddPath<TimeSpan>("TargetUpdatePeriod", (EntityUid _, SingularityComponent comp) => comp.TargetUpdatePeriod, new ComponentPropertySetter<SingularityComponent, TimeSpan>(this.SetUpdatePeriod));
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x00030AD3 File Offset: 0x0002ECD3
		public override void Shutdown()
		{
			ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
			typeHandler.RemovePath("Energy");
			typeHandler.RemovePath("TargetUpdatePeriod");
			base.Shutdown();
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x00030B00 File Offset: 0x0002ED00
		public override void Update(float frameTime)
		{
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			foreach (SingularityComponent singularity in this.EntityManager.EntityQuery<SingularityComponent>(false))
			{
				TimeSpan curTime = this._timing.CurTime;
				if (singularity.NextUpdateTime <= curTime)
				{
					this.Update(singularity.Owner, curTime - singularity.LastUpdateTime, singularity);
				}
			}
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00030B90 File Offset: 0x0002ED90
		[NullableContext(2)]
		public void Update(EntityUid uid, SingularityComponent singularity = null)
		{
			if (base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				this.Update(uid, this._timing.CurTime - singularity.LastUpdateTime, singularity);
			}
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00030BBC File Offset: 0x0002EDBC
		[NullableContext(2)]
		public void Update(EntityUid uid, TimeSpan frameTime, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			singularity.LastUpdateTime = this._timing.CurTime;
			singularity.NextUpdateTime = singularity.LastUpdateTime + singularity.TargetUpdatePeriod;
			this.AdjustEnergy(uid, -singularity.EnergyDrain * (float)frameTime.TotalSeconds, float.MinValue, float.MaxValue, true, true, singularity);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00030C24 File Offset: 0x0002EE24
		[NullableContext(2)]
		public void SetEnergy(EntityUid uid, float value, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			if (singularity.Energy == value)
			{
				return;
			}
			singularity.Energy = value;
			byte value2;
			if (value < 1500f)
			{
				if (value < 1000f)
				{
					if (value < 600f)
					{
						if (value < 300f)
						{
							if (value < 200f)
							{
								if (value <= 0f)
								{
									value2 = 0;
								}
								else
								{
									value2 = 1;
								}
							}
							else
							{
								value2 = 2;
							}
						}
						else
						{
							value2 = 3;
						}
					}
					else
					{
						value2 = 4;
					}
				}
				else
				{
					value2 = 5;
				}
			}
			else
			{
				value2 = 6;
			}
			base.SetLevel(uid, value2, singularity);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00030CA8 File Offset: 0x0002EEA8
		[NullableContext(2)]
		public void AdjustEnergy(EntityUid uid, float delta, float min = -3.4028235E+38f, float max = 3.4028235E+38f, bool snapMin = true, bool snapMax = true, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			float newValue = singularity.Energy + delta;
			if ((!snapMin && newValue < min) || (!snapMax && newValue > max))
			{
				return;
			}
			this.SetEnergy(uid, MathHelper.Clamp(newValue, min, max), singularity);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00030CF0 File Offset: 0x0002EEF0
		[NullableContext(2)]
		public void SetUpdatePeriod(EntityUid uid, TimeSpan value, SingularityComponent singularity = null)
		{
			if (!base.Resolve<SingularityComponent>(uid, ref singularity, true))
			{
				return;
			}
			if (MathHelper.CloseTo(singularity.TargetUpdatePeriod.TotalSeconds, value.TotalSeconds, 1E-07))
			{
				return;
			}
			singularity.TargetUpdatePeriod = value;
			singularity.NextUpdateTime = singularity.LastUpdateTime + singularity.TargetUpdatePeriod;
			TimeSpan curTime = this._timing.CurTime;
			if (singularity.NextUpdateTime <= curTime)
			{
				this.Update(uid, curTime - singularity.LastUpdateTime, singularity);
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00030D7C File Offset: 0x0002EF7C
		protected override void OnSingularityStartup(EntityUid uid, SingularityComponent comp, ComponentStartup args)
		{
			comp.LastUpdateTime = this._timing.CurTime;
			comp.NextUpdateTime = comp.LastUpdateTime + comp.TargetUpdatePeriod;
			MetaDataComponent metaData = null;
			if (base.Resolve<MetaDataComponent>(uid, ref metaData, true) && metaData.EntityLifeStage <= 1)
			{
				this._audio.Play(comp.FormationSound, Filter.Pvs(comp.Owner, 2f, null, null, null), comp.Owner, true, null);
			}
			comp.AmbientSoundStream = this._audio.Play(comp.AmbientSound, Filter.Pvs(comp.Owner, 2f, null, null, null), comp.Owner, true, null);
			base.UpdateSingularityLevel(uid, comp);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00030E3F File Offset: 0x0002F03F
		public void OnDistortionStartup(EntityUid uid, SingularityDistortionComponent comp, ComponentStartup args)
		{
			this._pvs.AddGlobalOverride(uid);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00030E50 File Offset: 0x0002F050
		public void OnSingularityShutdown(EntityUid uid, SingularityComponent comp, ComponentShutdown args)
		{
			IPlayingAudioStream ambientSoundStream = comp.AmbientSoundStream;
			if (ambientSoundStream != null)
			{
				ambientSoundStream.Stop();
			}
			MetaDataComponent metaData = null;
			if (base.Resolve<MetaDataComponent>(uid, ref metaData, true) && metaData.EntityLifeStage >= 4)
			{
				this._audio.Play(comp.DissipationSound, Filter.Pvs(comp.Owner, 2f, null, null, null), comp.Owner, true, null);
			}
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00030EBA File Offset: 0x0002F0BA
		private void HandleSingularityState(EntityUid uid, SingularityComponent comp, ref ComponentGetState args)
		{
			args.State = new SharedSingularitySystem.SingularityComponentState(comp);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00030EC8 File Offset: 0x0002F0C8
		public void OnConsumedEntity(EntityUid uid, SingularityComponent comp, EntityConsumedByEventHorizonEvent args)
		{
			this.AdjustEnergy(uid, 1f, float.MinValue, float.MaxValue, true, true, comp);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00030EE3 File Offset: 0x0002F0E3
		public void OnConsumedTiles(EntityUid uid, SingularityComponent comp, TilesConsumedByEventHorizonEvent args)
		{
			this.AdjustEnergy(uid, (float)args.Tiles.Count * 1f, float.MinValue, float.MaxValue, true, true, comp);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00030F0C File Offset: 0x0002F10C
		private void OnConsumed(EntityUid uid, SingularityComponent comp, EventHorizonConsumedEntityEvent args)
		{
			SingularityComponent singulo;
			if (this.EntityManager.TryGetComponent<SingularityComponent>(args.EventHorizon.Owner, ref singulo))
			{
				this.AdjustEnergy(singulo.Owner, comp.Energy, float.MinValue, float.MaxValue, true, true, singulo);
				this.SetEnergy(uid, 0f, comp);
			}
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00030F60 File Offset: 0x0002F160
		public void OnConsumed(EntityUid uid, SinguloFoodComponent comp, EventHorizonConsumedEntityEvent args)
		{
			SingularityComponent singulo;
			if (this.EntityManager.TryGetComponent<SingularityComponent>(args.EventHorizon.Owner, ref singulo))
			{
				this.AdjustEnergy(args.EventHorizon.Owner, comp.Energy, float.MinValue, float.MaxValue, true, true, singulo);
			}
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00030FAC File Offset: 0x0002F1AC
		public void UpdateEnergyDrain(EntityUid uid, SingularityComponent comp, SingularityLevelChangedEvent args)
		{
			int num;
			switch (args.NewValue)
			{
			case 1:
				num = 1;
				break;
			case 2:
				num = 2;
				break;
			case 3:
				num = 5;
				break;
			case 4:
				num = 10;
				break;
			case 5:
				num = 15;
				break;
			case 6:
				num = 20;
				break;
			default:
				num = 0;
				break;
			}
			comp.EnergyDrain = (float)num;
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0003100C File Offset: 0x0002F20C
		private void UpdateRandomWalk(EntityUid uid, RandomWalkComponent comp, SingularityLevelChangedEvent args)
		{
			float scale = MathF.Max((float)args.NewValue, 4f);
			comp.MinSpeed = 7.5f / scale;
			comp.MaxSpeed = 10f / scale;
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00031048 File Offset: 0x0002F248
		private void UpdateGravityWell(EntityUid uid, GravityWellComponent comp, SingularityLevelChangedEvent args)
		{
			SingularityComponent singulos = args.Singularity;
			comp.MaxRange = base.GravPulseRange(singulos);
			ValueTuple<float, float> valueTuple = base.GravPulseAcceleration(singulos);
			comp.BaseRadialAcceleration = valueTuple.Item1;
			comp.BaseTangentialAcceleration = valueTuple.Item2;
		}

		// Token: 0x040005BC RID: 1468
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005BD RID: 1469
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040005BE RID: 1470
		[Dependency]
		private readonly PVSOverrideSystem _pvs;

		// Token: 0x040005BF RID: 1471
		public const float BaseTileEnergy = 1f;

		// Token: 0x040005C0 RID: 1472
		public const float BaseEntityEnergy = 1f;
	}
}
