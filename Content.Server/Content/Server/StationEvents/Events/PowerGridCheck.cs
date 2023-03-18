using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Station.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018C RID: 396
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerGridCheck : StationEventSystem
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00026D71 File Offset: 0x00024F71
		public override string Prototype
		{
			get
			{
				return "PowerGridCheck";
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x00026D78 File Offset: 0x00024F78
		private float UpdateRate
		{
			get
			{
				return 1f / (float)this._numberPerSecond;
			}
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00026D87 File Offset: 0x00024F87
		public override void Added()
		{
			base.Added();
			this._endAfter = (float)this.RobustRandom.Next(60, 120);
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x00026DA8 File Offset: 0x00024FA8
		public override void Started()
		{
			if (this.StationSystem.Stations.Count == 0)
			{
				return;
			}
			EntityUid chosenStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations.ToList<EntityUid>());
			foreach (ValueTuple<ApcComponent, TransformComponent> valueTuple in base.EntityQuery<ApcComponent, TransformComponent>(true))
			{
				ApcComponent apc = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				if (apc.MainBreakerEnabled)
				{
					StationMemberComponent stationMemberComponent = base.CompOrNull<StationMemberComponent>(transform.GridUid);
					if (stationMemberComponent != null && stationMemberComponent.Station == chosenStation)
					{
						this._powered.Add(apc.Owner);
					}
				}
			}
			this.RobustRandom.Shuffle<EntityUid>(this._powered);
			this._numberPerSecond = Math.Max(1, (int)((float)this._powered.Count / 30f));
			base.Started();
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00026E9C File Offset: 0x0002509C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!base.RuleStarted)
			{
				return;
			}
			if (base.Elapsed > this._endAfter)
			{
				base.ForceEndSelf();
				return;
			}
			int updates = 0;
			this._frameTimeAccumulator += frameTime;
			if (this._frameTimeAccumulator > this.UpdateRate)
			{
				updates = (int)(this._frameTimeAccumulator / this.UpdateRate);
				this._frameTimeAccumulator -= this.UpdateRate * (float)updates;
			}
			int i = 0;
			while (i < updates && this._powered.Count != 0)
			{
				EntityUid selected = Extensions.Pop<EntityUid>(this._powered);
				if (!this.EntityManager.Deleted(selected))
				{
					ApcComponent apcComponent;
					if (this.EntityManager.TryGetComponent<ApcComponent>(selected, ref apcComponent) && apcComponent.MainBreakerEnabled)
					{
						this._apcSystem.ApcToggleBreaker(selected, apcComponent, null);
					}
					this._unpowered.Add(selected);
				}
				i++;
			}
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00026F78 File Offset: 0x00025178
		public override void Ended()
		{
			foreach (EntityUid entity in this._unpowered)
			{
				ApcComponent apcComponent;
				if (!this.EntityManager.Deleted(entity) && this.EntityManager.TryGetComponent<ApcComponent>(entity, ref apcComponent) && !apcComponent.MainBreakerEnabled)
				{
					this._apcSystem.ApcToggleBreaker(entity, apcComponent, null);
				}
			}
			CancellationTokenSource announceCancelToken = this._announceCancelToken;
			if (announceCancelToken != null)
			{
				announceCancelToken.Cancel();
			}
			this._announceCancelToken = new CancellationTokenSource();
			Timer.Spawn(3000, delegate()
			{
				this._audioSystem.PlayGlobal("/Audio/Announcements/power_on.ogg", Filter.Broadcast(), true, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
			}, this._announceCancelToken.Token);
			this._unpowered.Clear();
			base.Ended();
		}

		// Token: 0x040004CA RID: 1226
		[Dependency]
		private readonly ApcSystem _apcSystem;

		// Token: 0x040004CB RID: 1227
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x040004CC RID: 1228
		[Nullable(2)]
		private CancellationTokenSource _announceCancelToken;

		// Token: 0x040004CD RID: 1229
		private readonly List<EntityUid> _powered = new List<EntityUid>();

		// Token: 0x040004CE RID: 1230
		private readonly List<EntityUid> _unpowered = new List<EntityUid>();

		// Token: 0x040004CF RID: 1231
		private const float SecondsUntilOff = 30f;

		// Token: 0x040004D0 RID: 1232
		private int _numberPerSecond;

		// Token: 0x040004D1 RID: 1233
		private float _frameTimeAccumulator;

		// Token: 0x040004D2 RID: 1234
		private float _endAfter;
	}
}
