using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Audio
{
	// Token: 0x02000428 RID: 1064
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AmbientSoundSystem : SharedAmbientSoundSystem
	{
		// Token: 0x060019F1 RID: 6641 RVA: 0x0009482C File Offset: 0x00092A2C
		protected override void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
		{
			this._treeSys.QueueTreeUpdate(uid, ambience, null);
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060019F2 RID: 6642 RVA: 0x0009483C File Offset: 0x00092A3C
		private int MaxSingleSound
		{
			get
			{
				return (int)((float)this._maxAmbientCount / 2.6666667f);
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x060019F3 RID: 6643 RVA: 0x0009484C File Offset: 0x00092A4C
		// (set) Token: 0x060019F4 RID: 6644 RVA: 0x00094854 File Offset: 0x00092A54
		public bool OverlayEnabled
		{
			get
			{
				return this._overlayEnabled;
			}
			set
			{
				if (this._overlayEnabled == value)
				{
					return;
				}
				this._overlayEnabled = value;
				IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
				if (this._overlayEnabled)
				{
					this._overlay = new AmbientSoundOverlay(this.EntityManager, this, this.EntityManager.System<EntityLookupSystem>());
					overlayManager.AddOverlay(this._overlay);
					return;
				}
				overlayManager.RemoveOverlay(this._overlay);
				this._overlay = null;
			}
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x000948BF File Offset: 0x00092ABF
		public bool IsActive(AmbientSoundComponent component)
		{
			return this._playingSounds.ContainsKey(component);
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x000948D0 File Offset: 0x00092AD0
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.UpdatesAfter.Add(typeof(AmbientSoundTreeSystem));
			this._cfg.OnValueChanged<float>(CCVars.AmbientCooldown, new Action<float>(this.SetCooldown), true);
			this._cfg.OnValueChanged<int>(CCVars.MaxAmbientSources, new Action<int>(this.SetAmbientCount), true);
			this._cfg.OnValueChanged<float>(CCVars.AmbientRange, new Action<float>(this.SetAmbientRange), true);
			this._cfg.OnValueChanged<float>(CCVars.AmbienceVolume, new Action<float>(this.SetAmbienceVolume), true);
			base.SubscribeLocalEvent<AmbientSoundComponent, ComponentShutdown>(new ComponentEventHandler<AmbientSoundComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x00094988 File Offset: 0x00092B88
		private void OnShutdown(EntityUid uid, AmbientSoundComponent component, ComponentShutdown args)
		{
			ValueTuple<IPlayingAudioStream, string> valueTuple;
			if (!this._playingSounds.Remove(component, out valueTuple))
			{
				return;
			}
			IPlayingAudioStream item = valueTuple.Item1;
			if (item != null)
			{
				item.Stop();
			}
			Dictionary<string, int> playingCount = this._playingCount;
			string item2 = valueTuple.Item2;
			playingCount[item2]--;
			if (this._playingCount[valueTuple.Item2] == 0)
			{
				this._playingCount.Remove(valueTuple.Item2);
			}
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x000949FA File Offset: 0x00092BFA
		private void SetAmbienceVolume(float value)
		{
			this._ambienceVolume = value;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x00094A03 File Offset: 0x00092C03
		private void SetCooldown(float value)
		{
			this._cooldown = value;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x00094A0C File Offset: 0x00092C0C
		private void SetAmbientCount(int value)
		{
			this._maxAmbientCount = value;
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x00094A15 File Offset: 0x00092C15
		private void SetAmbientRange(float value)
		{
			this._maxAmbientRange = value;
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x00094A20 File Offset: 0x00092C20
		public override void Shutdown()
		{
			base.Shutdown();
			this.ClearSounds();
			this._cfg.UnsubValueChanged<float>(CCVars.AmbientCooldown, new Action<float>(this.SetCooldown));
			this._cfg.UnsubValueChanged<int>(CCVars.MaxAmbientSources, new Action<int>(this.SetAmbientCount));
			this._cfg.UnsubValueChanged<float>(CCVars.AmbientRange, new Action<float>(this.SetAmbientRange));
			this._cfg.UnsubValueChanged<float>(CCVars.AmbienceVolume, new Action<float>(this.SetAmbienceVolume));
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x00094AAC File Offset: 0x00092CAC
		private int PlayingCount(string countSound)
		{
			int num = 0;
			foreach (KeyValuePair<AmbientSoundComponent, ValueTuple<IPlayingAudioStream, string>> keyValuePair in this._playingSounds)
			{
				AmbientSoundComponent ambientSoundComponent;
				ValueTuple<IPlayingAudioStream, string> valueTuple;
				keyValuePair.Deconstruct(out ambientSoundComponent, out valueTuple);
				if (valueTuple.Item2.Equals(countSound))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x00094B1C File Offset: 0x00092D1C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			if (this._cooldown <= 0f)
			{
				return;
			}
			if (this._gameTiming.CurTime < this._targetTime)
			{
				return;
			}
			this._targetTime = this._gameTiming.CurTime + TimeSpan.FromSeconds((double)this._cooldown);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			TransformComponent playerXform;
			if (!this.EntityManager.TryGetComponent<TransformComponent>(entityUid, ref playerXform))
			{
				this.ClearSounds();
				return;
			}
			this.ProcessNearbyAmbience(playerXform);
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x00094BC8 File Offset: 0x00092DC8
		private void ClearSounds()
		{
			foreach (ValueTuple<IPlayingAudioStream, string> valueTuple in this._playingSounds.Values)
			{
				IPlayingAudioStream item = valueTuple.Item1;
				if (item != null)
				{
					item.Stop();
				}
			}
			this._playingSounds.Clear();
			this._playingCount.Clear();
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x00094C40 File Offset: 0x00092E40
		private static bool Callback(ref AmbientSoundSystem.QueryState state, [Nullable(new byte[]
		{
			0,
			1
		})] in ComponentTreeEntry<AmbientSoundComponent> value)
		{
			ComponentTreeEntry<AmbientSoundComponent> componentTreeEntry = value;
			AmbientSoundComponent ambientSoundComponent;
			TransformComponent transformComponent;
			componentTreeEntry.Deconstruct(ref ambientSoundComponent, ref transformComponent);
			AmbientSoundComponent ambientSoundComponent2 = ambientSoundComponent;
			TransformComponent transformComponent2 = transformComponent;
			float length = ((transformComponent2.ParentUid == state.Player.ParentUid) ? (transformComponent2.LocalPosition - state.Player.LocalPosition) : (transformComponent2.WorldPosition - state.MapPos)).Length;
			if (length >= ambientSoundComponent2.Range)
			{
				return true;
			}
			SoundPathSpecifier soundPathSpecifier = ambientSoundComponent2.Sound as SoundPathSpecifier;
			string text;
			if (soundPathSpecifier != null)
			{
				ResourcePath path = soundPathSpecifier.Path;
				text = (((path != null) ? path.ToString() : null) ?? string.Empty);
			}
			else
			{
				text = (((SoundCollectionSpecifier)ambientSoundComponent2.Sound).Collection ?? string.Empty);
			}
			float item = length * (ambientSoundComponent2.Volume + 32f);
			Extensions.GetOrNew<string, List<ValueTuple<float, AmbientSoundComponent>>>(state.SourceDict, text).Add(new ValueTuple<float, AmbientSoundComponent>(item, ambientSoundComponent2));
			return true;
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x00094D34 File Offset: 0x00092F34
		private void ProcessNearbyAmbience(TransformComponent playerXform)
		{
			EntityQuery<TransformComponent> entityQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<MetaDataComponent> entityQuery2 = base.GetEntityQuery<MetaDataComponent>();
			MapCoordinates mapPosition = playerXform.MapPosition;
			foreach (KeyValuePair<AmbientSoundComponent, ValueTuple<IPlayingAudioStream, string>> keyValuePair in this._playingSounds)
			{
				AmbientSoundComponent ambientSoundComponent;
				ValueTuple<IPlayingAudioStream, string> valueTuple;
				keyValuePair.Deconstruct(out ambientSoundComponent, out valueTuple);
				AmbientSoundComponent ambientSoundComponent2 = ambientSoundComponent;
				ValueTuple<IPlayingAudioStream, string> valueTuple2 = valueTuple;
				EntityUid owner = ambientSoundComponent2.Owner;
				TransformComponent transformComponent;
				if (!ambientSoundComponent2.Enabled || !entityQuery.TryGetComponent(owner, ref transformComponent) || !(transformComponent.MapID == playerXform.MapID) || entityQuery2.GetComponent(owner).EntityPaused || ((transformComponent.ParentUid == playerXform.ParentUid) ? (transformComponent.LocalPosition - playerXform.LocalPosition) : (transformComponent.WorldPosition - mapPosition.Position)).LengthSquared >= ambientSoundComponent2.Range * ambientSoundComponent2.Range)
				{
					IPlayingAudioStream item = valueTuple2.Item1;
					if (item != null)
					{
						item.Stop();
					}
					this._playingSounds.Remove(ambientSoundComponent2);
					Dictionary<string, int> playingCount = this._playingCount;
					string item2 = valueTuple2.Item2;
					playingCount[item2]--;
					if (this._playingCount[valueTuple2.Item2] == 0)
					{
						this._playingCount.Remove(valueTuple2.Item2);
					}
				}
			}
			if (this._playingSounds.Count >= this._maxAmbientCount)
			{
				return;
			}
			Vector2 position = mapPosition.Position;
			AmbientSoundSystem.QueryState queryState = new AmbientSoundSystem.QueryState(position, playerXform, entityQuery);
			Box2 box;
			box..ctor(position - this._maxAmbientRange, position + this._maxAmbientRange);
			ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent> treeSys = this._treeSys;
			DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>.QueryCallbackDelegate<AmbientSoundSystem.QueryState> queryCallbackDelegate;
			if ((queryCallbackDelegate = AmbientSoundSystem.<>O.<0>__Callback) == null)
			{
				queryCallbackDelegate = (AmbientSoundSystem.<>O.<0>__Callback = new DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>.QueryCallbackDelegate<AmbientSoundSystem.QueryState>(AmbientSoundSystem.Callback));
			}
			treeSys.QueryAabb<AmbientSoundSystem.QueryState>(ref queryState, queryCallbackDelegate, mapPosition.MapId, box, true);
			foreach (KeyValuePair<string, List<ValueTuple<float, AmbientSoundComponent>>> keyValuePair2 in queryState.SourceDict)
			{
				string item2;
				List<ValueTuple<float, AmbientSoundComponent>> list;
				keyValuePair2.Deconstruct(out item2, out list);
				string text = item2;
				List<ValueTuple<float, AmbientSoundComponent>> list2 = list;
				if (this._playingSounds.Count >= this._maxAmbientCount)
				{
					break;
				}
				int num;
				if (!this._playingCount.TryGetValue(text, out num) || num < this.MaxSingleSound)
				{
					list2.Sort(([TupleElementNames(new string[]
					{
						"Importance",
						null
					})] [Nullable(new byte[]
					{
						0,
						1
					})] ValueTuple<float, AmbientSoundComponent> a, [TupleElementNames(new string[]
					{
						"Importance",
						null
					})] [Nullable(new byte[]
					{
						0,
						1
					})] ValueTuple<float, AmbientSoundComponent> b) => b.Item1.CompareTo(a.Item1));
					foreach (ValueTuple<float, AmbientSoundComponent> valueTuple3 in list2)
					{
						AmbientSoundComponent item3 = valueTuple3.Item2;
						EntityUid owner2 = item3.Owner;
						if (!this._playingSounds.ContainsKey(item3) && !entityQuery2.GetComponent(owner2).EntityPaused)
						{
							AudioParams value = AmbientSoundSystem._params.AddVolume(item3.Volume + this._ambienceVolume).WithPlayOffset(this._random.NextFloat(0f, 100f)).WithMaxDistance(item3.Range);
							IPlayingAudioStream playingAudioStream = this._audio.PlayPvs(item3.Sound, owner2, new AudioParams?(value));
							if (playingAudioStream != null)
							{
								this._playingSounds[item3] = new ValueTuple<IPlayingAudioStream, string>(playingAudioStream, text);
								num++;
								if (this._playingSounds.Count >= this._maxAmbientCount)
								{
									break;
								}
							}
						}
					}
					if (num != 0)
					{
						this._playingCount[text] = num;
					}
				}
			}
		}

		// Token: 0x04000D23 RID: 3363
		[Dependency]
		private readonly AmbientSoundTreeSystem _treeSys;

		// Token: 0x04000D24 RID: 3364
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D25 RID: 3365
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000D26 RID: 3366
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000D27 RID: 3367
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000D28 RID: 3368
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000D29 RID: 3369
		[Nullable(2)]
		private AmbientSoundOverlay _overlay;

		// Token: 0x04000D2A RID: 3370
		private int _maxAmbientCount;

		// Token: 0x04000D2B RID: 3371
		private bool _overlayEnabled;

		// Token: 0x04000D2C RID: 3372
		private float _maxAmbientRange;

		// Token: 0x04000D2D RID: 3373
		private float _cooldown;

		// Token: 0x04000D2E RID: 3374
		private TimeSpan _targetTime = TimeSpan.Zero;

		// Token: 0x04000D2F RID: 3375
		private float _ambienceVolume;

		// Token: 0x04000D30 RID: 3376
		private static AudioParams _params = AudioParams.Default.WithVariation(new float?(0.01f)).WithLoop(true).WithAttenuation(8);

		// Token: 0x04000D31 RID: 3377
		[TupleElementNames(new string[]
		{
			"Stream",
			"Sound"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			2,
			1
		})]
		private readonly Dictionary<AmbientSoundComponent, ValueTuple<IPlayingAudioStream, string>> _playingSounds = new Dictionary<AmbientSoundComponent, ValueTuple<IPlayingAudioStream, string>>();

		// Token: 0x04000D32 RID: 3378
		private readonly Dictionary<string, int> _playingCount = new Dictionary<string, int>();

		// Token: 0x02000429 RID: 1065
		[Nullable(0)]
		private readonly struct QueryState
		{
			// Token: 0x06001A04 RID: 6660 RVA: 0x00095188 File Offset: 0x00093388
			public QueryState(Vector2 mapPos, TransformComponent player, [Nullable(new byte[]
			{
				0,
				1
			})] EntityQuery<TransformComponent> query)
			{
				this.SourceDict = new Dictionary<string, List<ValueTuple<float, AmbientSoundComponent>>>();
				this.MapPos = mapPos;
				this.Player = player;
				this.Query = query;
			}

			// Token: 0x04000D33 RID: 3379
			[TupleElementNames(new string[]
			{
				"Importance",
				null
			})]
			[Nullable(new byte[]
			{
				1,
				1,
				1,
				0,
				1
			})]
			public readonly Dictionary<string, List<ValueTuple<float, AmbientSoundComponent>>> SourceDict;

			// Token: 0x04000D34 RID: 3380
			public readonly Vector2 MapPos;

			// Token: 0x04000D35 RID: 3381
			public readonly TransformComponent Player;

			// Token: 0x04000D36 RID: 3382
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public readonly EntityQuery<TransformComponent> Query;
		}

		// Token: 0x0200042A RID: 1066
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000D37 RID: 3383
			[Nullable(new byte[]
			{
				0,
				0,
				1
			})]
			public static DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>.QueryCallbackDelegate<AmbientSoundSystem.QueryState> <0>__Callback;
		}
	}
}
