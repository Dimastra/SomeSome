using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Client.Weather
{
	// Token: 0x0200002A RID: 42
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WeatherSystem : SharedWeatherSystem
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x0000621C File Offset: 0x0000441C
		public override void Initialize()
		{
			base.Initialize();
			this._overlayManager.AddOverlay(new WeatherOverlay(this._transform, this.EntityManager.System<SpriteSystem>(), this));
			base.SubscribeLocalEvent<WeatherComponent, ComponentHandleState>(new ComponentEventRefHandler<WeatherComponent, ComponentHandleState>(this.OnWeatherHandleState), null, null);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000625B File Offset: 0x0000445B
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayManager.RemoveOverlay<WeatherOverlay>();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006270 File Offset: 0x00004470
		protected override void Run(EntityUid uid, WeatherComponent component, WeatherPrototype weather, WeatherState state, float frameTime)
		{
			base.Run(uid, component, weather, state, frameTime);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			EntityUid? mapUid = base.Transform(uid).MapUid;
			TransformComponent transformComponent = base.Transform(entityUid.Value);
			if (mapUid == null || transformComponent.MapUid != mapUid)
			{
				this._lastOcclusion = 0f;
				this._lastAlpha = 0f;
				IPlayingAudioStream stream = component.Stream;
				if (stream != null)
				{
					stream.Stop();
				}
				component.Stream = null;
				return;
			}
			if (!this.Timing.IsFirstTimePredicted || weather.Sound == null)
			{
				return;
			}
			if (component.Stream == null)
			{
				component.Stream = this._audio.PlayGlobal(weather.Sound, Filter.Local(), true, null);
			}
			float num = MathF.Pow(10f, weather.Sound.Params.Volume / 10f);
			AudioSystem.PlayingStream playingStream = (AudioSystem.PlayingStream)component.Stream;
			float num2 = this.GetPercent(component, mapUid.Value, weather);
			num2 = MathF.Pow(num2, 2f) * num;
			float num3 = 0f;
			MapGridComponent mapGridComponent;
			if (base.TryComp<MapGridComponent>(transformComponent.GridUid, ref mapGridComponent))
			{
				TileRef tileRef = mapGridComponent.GetTileRef(transformComponent.Coordinates);
				Queue<TileRef> queue = new Queue<TileRef>();
				queue.Enqueue(tileRef);
				EntityCoordinates? entityCoordinates = null;
				EntityQuery<PhysicsComponent> entityQuery = base.GetEntityQuery<PhysicsComponent>();
				HashSet<Vector2i> hashSet = new HashSet<Vector2i>();
				TileRef tileRef2;
				while (queue.TryDequeue(out tileRef2))
				{
					if (hashSet.Add(tileRef2.GridIndices))
					{
						if (base.CanWeatherAffect(mapGridComponent, tileRef2, entityQuery))
						{
							entityCoordinates = new EntityCoordinates?(new EntityCoordinates(transformComponent.GridUid.Value, tileRef2.GridIndices + (float)mapGridComponent.TileSize / 2f));
							break;
						}
						for (int i = -1; i <= 1; i++)
						{
							for (int j = -1; j <= 1; j++)
							{
								if ((Math.Abs(i) != 1 || Math.Abs(j) != 1) && (i != 0 || j != 0) && (new Vector2((float)i, (float)j) + tileRef2.GridIndices - tileRef.GridIndices).Length <= 3f)
								{
									queue.Enqueue(mapGridComponent.GetTileRef(new Vector2i(i, j) + tileRef2.GridIndices));
								}
							}
						}
					}
				}
				if (entityCoordinates == null)
				{
					num2 = 0f;
				}
				else
				{
					Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent);
					Vector2 vector = entityCoordinates.Value.ToMap(this.EntityManager).Position - worldPosition;
					if (vector.LengthSquared > 1f)
					{
						num3 = this._physics.IntersectRayPenetration(transformComponent.MapID, new CollisionRay(worldPosition, vector.Normalized, this._audio.OcclusionCollisionMask), vector.Length, playingStream.TrackingEntity);
					}
				}
			}
			if (MathHelper.CloseTo(this._lastOcclusion, num3, 0.01f))
			{
				this._lastOcclusion = num3;
			}
			else
			{
				this._lastOcclusion += (num3 - this._lastOcclusion) * 4f * frameTime;
			}
			if (MathHelper.CloseTo(this._lastAlpha, num2, 0.01f))
			{
				this._lastAlpha = num2;
			}
			else
			{
				this._lastAlpha += (num2 - this._lastAlpha) * 4f * frameTime;
			}
			playingStream.Source.SetVolumeDirect(this._lastAlpha);
			playingStream.Source.SetOcclusion(this._lastOcclusion);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006674 File Offset: 0x00004874
		public float GetPercent(WeatherComponent component, EntityUid mapUid, WeatherPrototype weatherProto)
		{
			TimeSpan pauseTime = this._metadata.GetPauseTime(mapUid, null);
			TimeSpan timeSpan = this.Timing.CurTime - (component.StartTime + pauseTime);
			TimeSpan t = component.Duration - timeSpan;
			float result;
			if (timeSpan < weatherProto.StartupTime)
			{
				result = (float)(timeSpan / weatherProto.StartupTime);
			}
			else if (t < weatherProto.ShutdownTime)
			{
				result = (float)(t / weatherProto.ShutdownTime);
			}
			else
			{
				result = 1f;
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000066FC File Offset: 0x000048FC
		protected override bool SetState(EntityUid uid, WeatherComponent component, WeatherState state, WeatherPrototype prototype)
		{
			if (!base.SetState(uid, component, state, prototype))
			{
				return false;
			}
			if (!this.Timing.IsFirstTimePredicted)
			{
				return true;
			}
			IPlayingAudioStream stream = component.Stream;
			if (stream != null)
			{
				stream.Stop();
			}
			component.Stream = null;
			component.Stream = this._audio.PlayGlobal(prototype.Sound, Filter.Local(), true, null);
			return true;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006767 File Offset: 0x00004967
		protected override void EndWeather(WeatherComponent component)
		{
			this._lastOcclusion = 0f;
			this._lastAlpha = 0f;
			base.EndWeather(component);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006788 File Offset: 0x00004988
		private void OnWeatherHandleState(EntityUid uid, WeatherComponent component, ref ComponentHandleState args)
		{
			SharedWeatherSystem.WeatherComponentState weatherComponentState = args.Current as SharedWeatherSystem.WeatherComponentState;
			if (weatherComponentState == null)
			{
				return;
			}
			if (component.Weather != weatherComponentState.Weather || !component.EndTime.Equals(weatherComponentState.EndTime) || !component.StartTime.Equals(weatherComponentState.StartTime))
			{
				this.EndWeather(component);
				if (weatherComponentState.Weather != null)
				{
					base.StartWeather(component, this.ProtoMan.Index<WeatherPrototype>(weatherComponentState.Weather));
				}
			}
			component.EndTime = weatherComponentState.EndTime;
			component.StartTime = weatherComponentState.StartTime;
		}

		// Token: 0x0400006E RID: 110
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x0400006F RID: 111
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000070 RID: 112
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04000071 RID: 113
		[Dependency]
		private readonly MetaDataSystem _metadata;

		// Token: 0x04000072 RID: 114
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000073 RID: 115
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000074 RID: 116
		private float _lastAlpha;

		// Token: 0x04000075 RID: 117
		private float _lastOcclusion;

		// Token: 0x04000076 RID: 118
		private const float OcclusionLerpRate = 4f;

		// Token: 0x04000077 RID: 119
		private const float AlphaLerpRate = 4f;
	}
}
