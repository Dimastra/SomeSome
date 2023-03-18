using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.White.TTS;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;

namespace Content.Client.White.TTS
{
	// Token: 0x0200001D RID: 29
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TTSSystem : EntitySystem
	{
		// Token: 0x0600005E RID: 94 RVA: 0x0000462C File Offset: 0x0000282C
		public override void Initialize()
		{
			this._sawmill = Logger.GetSawmill("tts");
			this._cfg.OnValueChanged<float>(CCVars.TtsVolume, new Action<float>(this.OnTtsVolumeChanged), true);
			base.SubscribeNetworkEvent<PlayTTSEvent>(new EntityEventHandler<PlayTTSEvent>(this.OnPlayTTS), null, null);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000467A File Offset: 0x0000287A
		public override void Shutdown()
		{
			base.Shutdown();
			this._cfg.UnsubValueChanged<float>(CCVars.TtsVolume, new Action<float>(this.OnTtsVolumeChanged));
			this.EndStreams();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000046A4 File Offset: 0x000028A4
		public override void FrameUpdate(float frameTime)
		{
			HashSet<TTSSystem.AudioStream> hashSet = new HashSet<TTSSystem.AudioStream>();
			Vector2 position = this._eye.CurrentEye.Position.Position;
			foreach (TTSSystem.AudioStream audioStream in this._currentStreams)
			{
				MetaDataComponent metaDataComponent;
				TransformComponent transformComponent;
				if (!audioStream.Source.IsPlaying || !this._entity.TryGetComponent<MetaDataComponent>(audioStream.Uid, ref metaDataComponent) || base.Deleted(audioStream.Uid, metaDataComponent) || !this._entity.TryGetComponent<TransformComponent>(audioStream.Uid, ref transformComponent))
				{
					audioStream.Source.Dispose();
					hashSet.Add(audioStream);
				}
				else
				{
					MapCoordinates mapPosition = transformComponent.MapPosition;
					if (mapPosition.MapId != MapId.Nullspace && !audioStream.Source.SetPosition(mapPosition.Position))
					{
						this._sawmill.Warning("Can't set position for audio stream, stop stream.");
						audioStream.Source.StopPlaying();
					}
					if (mapPosition.MapId == this._eye.CurrentMap)
					{
						int num = 2;
						Vector2 vector = position - mapPosition.Position;
						float occlusion = 0f;
						if (vector.Length > 0f)
						{
							occlusion = this._broadPhase.IntersectRayPenetration(mapPosition.MapId, new CollisionRay(mapPosition.Position, vector.Normalized, num), vector.Length, new EntityUid?(audioStream.Uid));
						}
						audioStream.Source.SetOcclusion(occlusion);
					}
				}
			}
			foreach (TTSSystem.AudioStream audioStream2 in hashSet)
			{
				this._currentStreams.Remove(audioStream2);
				this.ProcessEntityQueue(audioStream2.Uid);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000048B0 File Offset: 0x00002AB0
		private void OnTtsVolumeChanged(float volume)
		{
			this._volume = volume;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000048BC File Offset: 0x00002ABC
		private void OnPlayTTS(PlayTTSEvent ev)
		{
			IClydeAudioSource source;
			if (!this.TryCreateAudioSource(ev.Data, out source))
			{
				return;
			}
			TTSSystem.AudioStream stream = new TTSSystem.AudioStream(ev.Uid, source);
			this.AddEntityStreamToQueue(stream);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000048EE File Offset: 0x00002AEE
		public void PlayCustomText(string text)
		{
			base.RaiseNetworkEvent(new RequestTTSEvent(text));
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000048FC File Offset: 0x00002AFC
		private bool TryCreateAudioSource(byte[] data, [Nullable(2)] [NotNullWhen(true)] out IClydeAudioSource source)
		{
			MemoryStream memoryStream = new MemoryStream(data)
			{
				Position = 0L
			};
			Robust.Client.Audio.AudioStream audioStream = this._clyde.LoadAudioWav(memoryStream, null);
			source = this._clyde.CreateAudioSource(audioStream);
			IClydeAudioSource clydeAudioSource = source;
			if (clydeAudioSource != null)
			{
				clydeAudioSource.SetVolume(this._volume);
			}
			return source != null;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000494C File Offset: 0x00002B4C
		private void AddEntityStreamToQueue(TTSSystem.AudioStream stream)
		{
			Queue<TTSSystem.AudioStream> queue;
			if (this._entityQueues.TryGetValue(stream.Uid, out queue))
			{
				queue.Enqueue(stream);
				return;
			}
			this._entityQueues.Add(stream.Uid, new Queue<TTSSystem.AudioStream>(new TTSSystem.AudioStream[]
			{
				stream
			}));
			if (!this.IsEntityCurrentlyPlayStream(stream.Uid))
			{
				this.ProcessEntityQueue(stream.Uid);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000049B0 File Offset: 0x00002BB0
		private bool IsEntityCurrentlyPlayStream(EntityUid uid)
		{
			return this._currentStreams.Any((TTSSystem.AudioStream s) => s.Uid == uid);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000049E4 File Offset: 0x00002BE4
		private void ProcessEntityQueue(EntityUid uid)
		{
			TTSSystem.AudioStream stream;
			if (this.TryTakeEntityStreamFromQueue(uid, out stream))
			{
				this.PlayEntity(stream);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004A04 File Offset: 0x00002C04
		[NullableContext(2)]
		private bool TryTakeEntityStreamFromQueue(EntityUid uid, [NotNullWhen(true)] out TTSSystem.AudioStream stream)
		{
			Queue<TTSSystem.AudioStream> queue;
			if (this._entityQueues.TryGetValue(uid, out queue))
			{
				stream = queue.Dequeue();
				if (queue.Count == 0)
				{
					this._entityQueues.Remove(uid);
				}
				return true;
			}
			stream = null;
			return false;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004A44 File Offset: 0x00002C44
		private void PlayEntity(TTSSystem.AudioStream stream)
		{
			TransformComponent transformComponent;
			if (!this._entity.TryGetComponent<TransformComponent>(stream.Uid, ref transformComponent) || !stream.Source.SetPosition(transformComponent.WorldPosition))
			{
				return;
			}
			stream.Source.StartPlaying();
			this._currentStreams.Add(stream);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004A94 File Offset: 0x00002C94
		public void StopAllStreams()
		{
			foreach (TTSSystem.AudioStream audioStream in this._currentStreams)
			{
				audioStream.Source.StopPlaying();
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004AEC File Offset: 0x00002CEC
		private void EndStreams()
		{
			foreach (TTSSystem.AudioStream audioStream in this._currentStreams)
			{
				audioStream.Source.StopPlaying();
				audioStream.Source.Dispose();
			}
			this._currentStreams.Clear();
			this._entityQueues.Clear();
		}

		// Token: 0x04000036 RID: 54
		[Dependency]
		private readonly IClydeAudio _clyde;

		// Token: 0x04000037 RID: 55
		[Dependency]
		private readonly IEntityManager _entity;

		// Token: 0x04000038 RID: 56
		[Dependency]
		private readonly IEyeManager _eye;

		// Token: 0x04000039 RID: 57
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400003A RID: 58
		[Dependency]
		private readonly SharedPhysicsSystem _broadPhase;

		// Token: 0x0400003B RID: 59
		private ISawmill _sawmill;

		// Token: 0x0400003C RID: 60
		private float _volume;

		// Token: 0x0400003D RID: 61
		private readonly HashSet<TTSSystem.AudioStream> _currentStreams = new HashSet<TTSSystem.AudioStream>();

		// Token: 0x0400003E RID: 62
		private readonly Dictionary<EntityUid, Queue<TTSSystem.AudioStream>> _entityQueues = new Dictionary<EntityUid, Queue<TTSSystem.AudioStream>>();

		// Token: 0x0200001E RID: 30
		[Nullable(0)]
		private sealed class AudioStream
		{
			// Token: 0x1700000D RID: 13
			// (get) Token: 0x0600006D RID: 109 RVA: 0x00004B82 File Offset: 0x00002D82
			public EntityUid Uid { get; }

			// Token: 0x1700000E RID: 14
			// (get) Token: 0x0600006E RID: 110 RVA: 0x00004B8A File Offset: 0x00002D8A
			public IClydeAudioSource Source { get; }

			// Token: 0x0600006F RID: 111 RVA: 0x00004B92 File Offset: 0x00002D92
			public AudioStream(EntityUid uid, IClydeAudioSource source)
			{
				this.Uid = uid;
				this.Source = source;
			}
		}
	}
}
