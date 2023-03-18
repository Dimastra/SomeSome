using System;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Weather
{
	// Token: 0x0200003E RID: 62
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedWeatherSystem : EntitySystem
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00002E32 File Offset: 0x00001032
		public override void Initialize()
		{
			base.Initialize();
			this.Sawmill = Logger.GetSawmill("weather");
			base.SubscribeLocalEvent<WeatherComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<WeatherComponent, EntityUnpausedEvent>(this.OnWeatherUnpaused), null, null);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002E5E File Offset: 0x0000105E
		private void OnWeatherUnpaused(EntityUid uid, WeatherComponent component, ref EntityUnpausedEvent args)
		{
			component.EndTime += args.PausedTime;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00002E78 File Offset: 0x00001078
		public bool CanWeatherAffect(MapGridComponent grid, TileRef tileRef, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodyQuery)
		{
			if (tileRef.Tile.IsEmpty)
			{
				return true;
			}
			if (!((ContentTileDefinition)this._tileDefManager[(int)tileRef.Tile.TypeId]).Weather)
			{
				return false;
			}
			EntityUid? ent;
			while (grid.GetAnchoredEntitiesEnumerator(tileRef.GridIndices).MoveNext(ref ent))
			{
				PhysicsComponent body;
				if (bodyQuery.TryGetComponent(ent, ref body) && body.CanCollide)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00002EE8 File Offset: 0x000010E8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			TimeSpan curTime = this.Timing.CurTime;
			foreach (ValueTuple<WeatherComponent, MetaDataComponent> valueTuple in base.EntityQuery<WeatherComponent, MetaDataComponent>(false))
			{
				WeatherComponent comp = valueTuple.Item1;
				if (comp.Weather != null)
				{
					EntityUid uid = comp.Owner;
					TimeSpan endTime = comp.EndTime;
					WeatherPrototype weatherProto;
					if (endTime < curTime)
					{
						this.EndWeather(comp);
					}
					else if (!this.ProtoMan.TryIndex<WeatherPrototype>(comp.Weather, ref weatherProto))
					{
						this.Sawmill.Error("Unable to find weather prototype for " + comp.Weather + ", ending!");
						this.EndWeather(comp);
					}
					else
					{
						if (endTime - curTime < weatherProto.ShutdownTime)
						{
							this.SetState(uid, comp, WeatherState.Ending, weatherProto);
						}
						else
						{
							TimeSpan startTime = comp.StartTime;
							if (this.Timing.CurTime - startTime < weatherProto.StartupTime)
							{
								this.SetState(uid, comp, WeatherState.Starting, weatherProto);
							}
						}
						this.Run(uid, comp, weatherProto, comp.State, frameTime);
					}
				}
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003034 File Offset: 0x00001234
		[NullableContext(2)]
		public void SetWeather(MapId mapId, WeatherPrototype weather)
		{
			WeatherComponent weatherComp = base.EnsureComp<WeatherComponent>(this.MapManager.GetMapEntityId(mapId));
			this.EndWeather(weatherComp);
			if (weather != null)
			{
				this.StartWeather(weatherComp, weather);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003066 File Offset: 0x00001266
		protected virtual void Run(EntityUid uid, WeatherComponent component, WeatherPrototype weather, WeatherState state, float frameTime)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003068 File Offset: 0x00001268
		protected void StartWeather(WeatherComponent component, WeatherPrototype weather)
		{
			component.Weather = weather.ID;
			double duration = this._random.NextDouble(weather.DurationMinimum.TotalSeconds, weather.DurationMaximum.TotalSeconds);
			component.EndTime = this.Timing.CurTime + TimeSpan.FromSeconds(duration);
			component.StartTime = this.Timing.CurTime;
			base.Dirty(component, null);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000030D8 File Offset: 0x000012D8
		protected virtual void EndWeather(WeatherComponent component)
		{
			IPlayingAudioStream stream = component.Stream;
			if (stream != null)
			{
				stream.Stop();
			}
			component.Stream = null;
			component.Weather = null;
			component.StartTime = TimeSpan.Zero;
			component.EndTime = TimeSpan.Zero;
			component.State = WeatherState.Invalid;
			base.Dirty(component, null);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003129 File Offset: 0x00001329
		protected virtual bool SetState(EntityUid uid, WeatherComponent component, WeatherState state, WeatherPrototype prototype)
		{
			if (component.State.Equals(state))
			{
				return false;
			}
			component.State = state;
			return true;
		}

		// Token: 0x040000A1 RID: 161
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x040000A2 RID: 162
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x040000A3 RID: 163
		[Dependency]
		protected readonly IPrototypeManager ProtoMan;

		// Token: 0x040000A4 RID: 164
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000A5 RID: 165
		[Dependency]
		private readonly ITileDefinitionManager _tileDefManager;

		// Token: 0x040000A6 RID: 166
		protected ISawmill Sawmill;

		// Token: 0x02000780 RID: 1920
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class WeatherComponentState : ComponentState
		{
			// Token: 0x04001768 RID: 5992
			[Nullable(2)]
			public string Weather;

			// Token: 0x04001769 RID: 5993
			public TimeSpan StartTime;

			// Token: 0x0400176A RID: 5994
			public TimeSpan EndTime;
		}
	}
}
