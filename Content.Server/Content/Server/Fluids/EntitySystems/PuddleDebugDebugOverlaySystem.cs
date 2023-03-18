using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Fluids.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004EF RID: 1263
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PuddleDebugDebugOverlaySystem : SharedPuddleDebugOverlaySystem
	{
		// Token: 0x060019FF RID: 6655 RVA: 0x00088C98 File Offset: 0x00086E98
		public bool ToggleObserver(IPlayerSession observer)
		{
			TimeSpan value = this.NextTick.GetValueOrDefault();
			if (this.NextTick == null)
			{
				value = this._timing.CurTime + this.Cooldown;
				this.NextTick = new TimeSpan?(value);
			}
			if (this._playerObservers.Contains(observer))
			{
				this.RemoveObserver(observer);
				return false;
			}
			this._playerObservers.Add(observer);
			return true;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x00088D08 File Offset: 0x00086F08
		private void RemoveObserver(IPlayerSession observer)
		{
			if (!this._playerObservers.Remove(observer))
			{
				return;
			}
			PuddleOverlayDisableMessage message = new PuddleOverlayDisableMessage();
			base.RaiseNetworkEvent(message, observer.ConnectedClient);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x00088D38 File Offset: 0x00086F38
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this.NextTick == null || this._timing.CurTime < this.NextTick)
			{
				return;
			}
			foreach (IPlayerSession playerSession in this._playerObservers)
			{
				EntityUid? attachedEntity = playerSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid entity = attachedEntity.GetValueOrDefault();
					if (entity.Valid)
					{
						TransformComponent transform = this.EntityManager.GetComponent<TransformComponent>(entity);
						Box2 worldBounds = Box2.CenteredAround(transform.WorldPosition, new Vector2(16f, 16f));
						foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(transform.MapID, worldBounds, false))
						{
							List<PuddleDebugOverlayData> data = new List<PuddleDebugOverlayData>();
							EntityUid gridUid = grid.Owner;
							if (base.Exists(gridUid))
							{
								foreach (EntityUid uid in grid.GetAnchoredEntities(worldBounds))
								{
									PuddleComponent puddle = null;
									TransformComponent xform = null;
									if (base.Resolve<PuddleComponent, TransformComponent>(uid, ref puddle, ref xform, false))
									{
										Vector2i pos = xform.Coordinates.ToVector2i(this.EntityManager, this._mapManager);
										FixedPoint2 vol = this._puddle.CurrentVolume(uid, puddle);
										data.Add(new PuddleDebugOverlayData(pos, vol));
									}
								}
								base.RaiseNetworkEvent(new PuddleOverlayDebugMessage(gridUid, data.ToArray()));
							}
						}
					}
				}
			}
			this.NextTick = new TimeSpan?(this._timing.CurTime + this.Cooldown);
		}

		// Token: 0x04001059 RID: 4185
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400105A RID: 4186
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400105B RID: 4187
		[Dependency]
		private readonly PuddleSystem _puddle;

		// Token: 0x0400105C RID: 4188
		private readonly HashSet<IPlayerSession> _playerObservers = new HashSet<IPlayerSession>();
	}
}
