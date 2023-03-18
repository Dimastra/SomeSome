using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Tabletop.Components;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Tabletop;
using Content.Shared.Tabletop.Events;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Server.Tabletop
{
	// Token: 0x02000132 RID: 306
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopSystem : SharedTabletopSystem
	{
		// Token: 0x06000584 RID: 1412 RVA: 0x0001B4A0 File Offset: 0x000196A0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<TabletopStopPlayingEvent>(new EntitySessionEventHandler<TabletopStopPlayingEvent>(this.OnStopPlaying), null, null);
			base.SubscribeLocalEvent<TabletopGameComponent, ActivateInWorldEvent>(new ComponentEventHandler<TabletopGameComponent, ActivateInWorldEvent>(this.OnTabletopActivate), null, null);
			base.SubscribeLocalEvent<TabletopGameComponent, ComponentShutdown>(new ComponentEventHandler<TabletopGameComponent, ComponentShutdown>(this.OnGameShutdown), null, null);
			base.SubscribeLocalEvent<TabletopGamerComponent, PlayerDetachedEvent>(new ComponentEventHandler<TabletopGamerComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<TabletopGamerComponent, ComponentShutdown>(new ComponentEventHandler<TabletopGamerComponent, ComponentShutdown>(this.OnGamerShutdown), null, null);
			base.SubscribeLocalEvent<TabletopGameComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<TabletopGameComponent, GetVerbsEvent<ActivationVerb>>(this.AddPlayGameVerb), null, null);
			this.InitializeMap();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0001B534 File Offset: 0x00019734
		protected override void OnTabletopMove(TabletopMoveEvent msg, EntitySessionEventArgs args)
		{
			IPlayerSession playerSession = args.SenderSession as IPlayerSession;
			if (playerSession == null)
			{
				return;
			}
			TabletopGameComponent tabletop;
			if (base.TryComp<TabletopGameComponent>(msg.TableUid, ref tabletop))
			{
				TabletopSession session = tabletop.Session;
				if (session != null)
				{
					if (!session.Players.ContainsKey(playerSession))
					{
						return;
					}
					base.OnTabletopMove(msg, args);
					return;
				}
			}
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0001B584 File Offset: 0x00019784
		private void AddPlayGameVerb(EntityUid uid, TabletopGameComponent component, GetVerbsEvent<ActivationVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			ActivationVerb verb = new ActivationVerb();
			verb.Text = Loc.GetString("tabletop-verb-play-game");
			verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/die.svg.192dpi.png", "/"));
			verb.Act = delegate()
			{
				this.OpenSessionFor(actor.PlayerSession, uid);
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0001B620 File Offset: 0x00019820
		private void OnTabletopActivate(EntityUid uid, TabletopGameComponent component, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			this.OpenSessionFor(actor.PlayerSession, uid);
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001B650 File Offset: 0x00019850
		private void OnGameShutdown(EntityUid uid, TabletopGameComponent component, ComponentShutdown args)
		{
			this.CleanupSession(uid);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001B659 File Offset: 0x00019859
		private void OnStopPlaying(TabletopStopPlayingEvent msg, EntitySessionEventArgs args)
		{
			this.CloseSessionFor((IPlayerSession)args.SenderSession, msg.TableUid, true);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0001B674 File Offset: 0x00019874
		private void OnPlayerDetached(EntityUid uid, TabletopGamerComponent component, PlayerDetachedEvent args)
		{
			if (component.Tabletop.IsValid())
			{
				this.CloseSessionFor(args.Player, component.Tabletop, true);
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0001B6A4 File Offset: 0x000198A4
		private void OnGamerShutdown(EntityUid uid, TabletopGamerComponent component, ComponentShutdown args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(uid, ref actor))
			{
				return;
			}
			if (component.Tabletop.IsValid())
			{
				this.CloseSessionFor(actor.PlayerSession, component.Tabletop, true);
			}
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0001B6E8 File Offset: 0x000198E8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (TabletopGamerComponent gamer in this.EntityManager.EntityQuery<TabletopGamerComponent>(false))
			{
				if (this.EntityManager.EntityExists(gamer.Tabletop))
				{
					ActorComponent actor;
					if (!this.EntityManager.TryGetComponent<ActorComponent>(gamer.Owner, ref actor))
					{
						this.EntityManager.RemoveComponent<TabletopGamerComponent>(gamer.Owner);
						break;
					}
					EntityUid gamerUid = gamer.Owner;
					if (actor.PlayerSession.Status != 3 || !base.CanSeeTable(gamerUid, new EntityUid?(gamer.Tabletop)))
					{
						this.CloseSessionFor(actor.PlayerSession, gamer.Tabletop, true);
					}
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0001B7B8 File Offset: 0x000199B8
		// (set) Token: 0x0600058E RID: 1422 RVA: 0x0001B7C0 File Offset: 0x000199C0
		public MapId TabletopMap { get; private set; } = MapId.Nullspace;

		// Token: 0x0600058F RID: 1423 RVA: 0x0001B7C9 File Offset: 0x000199C9
		private void InitializeMap()
		{
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x0001B7E0 File Offset: 0x000199E0
		private Vector2 GetNextTabletopPosition()
		{
			int tabletops = this._tabletops;
			this._tabletops = tabletops + 1;
			return this.UlamSpiral(tabletops) * 100;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x0001B810 File Offset: 0x00019A10
		private void EnsureTabletopMap()
		{
			if (this.TabletopMap != MapId.Nullspace && this._mapManager.MapExists(this.TabletopMap))
			{
				return;
			}
			this.TabletopMap = this._mapManager.CreateMap(null);
			this._tabletops = 0;
			MapComponent component = this.EntityManager.GetComponent<MapComponent>(this._mapManager.GetMapEntityId(this.TabletopMap));
			component.LightingEnabled = false;
			component.Dirty(null);
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0001B890 File Offset: 0x00019A90
		private Vector2i UlamSpiral(int n)
		{
			int i = (int)MathF.Ceiling(MathF.Sqrt((float)n) - 1f) / 2;
			int t = 2 * i + 1;
			int j = (int)MathF.Pow((float)t, 2f);
			t--;
			if (n >= j - t)
			{
				return new Vector2i(i - (j - n), -i);
			}
			j -= t;
			if (n >= j - t)
			{
				return new Vector2i(-i, -i + (j - n));
			}
			j -= t;
			if (n >= j - t)
			{
				return new Vector2i(-i + (j - n), i);
			}
			return new Vector2i(i, i - (j - n - t));
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001B91A File Offset: 0x00019B1A
		private void OnRoundRestart(RoundRestartCleanupEvent _)
		{
			if (this.TabletopMap == MapId.Nullspace || !this._mapManager.MapExists(this.TabletopMap))
			{
				return;
			}
			this._mapManager.DeleteMap(this.TabletopMap);
			this._tabletops = 0;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0001B95C File Offset: 0x00019B5C
		public TabletopSession EnsureSession(TabletopGameComponent tabletop)
		{
			if (tabletop.Session != null)
			{
				return tabletop.Session;
			}
			this.EnsureTabletopMap();
			TabletopSession session = new TabletopSession(this.TabletopMap, this.GetNextTabletopPosition());
			tabletop.Session = session;
			tabletop.Setup.SetupTabletop(session, this.EntityManager);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Created tabletop session number ");
			defaultInterpolatedStringHandler.AppendFormatted<TabletopGameComponent>(tabletop);
			defaultInterpolatedStringHandler.AppendLiteral(" at position ");
			defaultInterpolatedStringHandler.AppendFormatted<MapCoordinates>(session.Position);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return session;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0001B9FC File Offset: 0x00019BFC
		public void CleanupSession(EntityUid uid)
		{
			TabletopGameComponent tabletop;
			if (!this.EntityManager.TryGetComponent<TabletopGameComponent>(uid, ref tabletop))
			{
				return;
			}
			TabletopSession session = tabletop.Session;
			if (session == null)
			{
				return;
			}
			foreach (KeyValuePair<IPlayerSession, TabletopSessionPlayerData> keyValuePair in session.Players)
			{
				IPlayerSession playerSession;
				TabletopSessionPlayerData tabletopSessionPlayerData;
				keyValuePair.Deconstruct(out playerSession, out tabletopSessionPlayerData);
				IPlayerSession player = playerSession;
				this.CloseSessionFor(player, uid, true);
			}
			foreach (EntityUid euid in session.Entities)
			{
				this.EntityManager.QueueDeleteEntity(euid);
			}
			tabletop.Session = null;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x0001BAD0 File Offset: 0x00019CD0
		public void OpenSessionFor(IPlayerSession player, EntityUid uid)
		{
			TabletopGameComponent tabletop;
			if (this.EntityManager.TryGetComponent<TabletopGameComponent>(uid, ref tabletop))
			{
				EntityUid? attachedEntity2 = player.AttachedEntity;
				if (attachedEntity2 != null)
				{
					EntityUid attachedEntity = attachedEntity2.GetValueOrDefault();
					if (attachedEntity.Valid)
					{
						TabletopSession session = this.EnsureSession(tabletop);
						if (session.Players.ContainsKey(player))
						{
							return;
						}
						TabletopGamerComponent gamer;
						if (this.EntityManager.TryGetComponent<TabletopGamerComponent>(attachedEntity, ref gamer))
						{
							this.CloseSessionFor(player, gamer.Tabletop, false);
						}
						ComponentExt.EnsureComponent<TabletopGamerComponent>(attachedEntity).Tabletop = uid;
						EntityUid camera = this.CreateCamera(tabletop, player, default(Vector2));
						session.Players[player] = new TabletopSessionPlayerData
						{
							Camera = camera
						};
						base.RaiseNetworkEvent(new TabletopPlayEvent(uid, camera, Loc.GetString(tabletop.BoardName), tabletop.Size), player.ConnectedClient);
						return;
					}
				}
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0001BBA4 File Offset: 0x00019DA4
		public void CloseSessionFor(IPlayerSession player, EntityUid uid, bool removeGamerComponent = true)
		{
			TabletopGameComponent tabletop;
			if (this.EntityManager.TryGetComponent<TabletopGameComponent>(uid, ref tabletop))
			{
				TabletopSession session = tabletop.Session;
				if (session != null)
				{
					TabletopSessionPlayerData data;
					if (!session.Players.TryGetValue(player, out data))
					{
						return;
					}
					if (removeGamerComponent)
					{
						EntityUid? attachedEntity2 = player.AttachedEntity;
						if (attachedEntity2 != null)
						{
							EntityUid attachedEntity = attachedEntity2.GetValueOrDefault();
							TabletopGamerComponent gamer;
							if (this.EntityManager.TryGetComponent<TabletopGamerComponent>(attachedEntity, ref gamer))
							{
								gamer.Tabletop = EntityUid.Invalid;
								this.EntityManager.RemoveComponent<TabletopGamerComponent>(attachedEntity);
							}
						}
					}
					session.Players.Remove(player);
					session.Entities.Remove(data.Camera);
					this.EntityManager.QueueDeleteEntity(data.Camera);
					return;
				}
			}
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0001BC54 File Offset: 0x00019E54
		private EntityUid CreateCamera(TabletopGameComponent tabletop, IPlayerSession player, Vector2 offset = default(Vector2))
		{
			TabletopSession session = tabletop.Session;
			EntityUid camera = this.EntityManager.SpawnEntity(null, session.Position.Offset(offset));
			EyeComponent eyeComponent = ComponentExt.EnsureComponent<EyeComponent>(camera);
			eyeComponent.DrawFov = false;
			eyeComponent.Zoom = tabletop.CameraZoom;
			this._viewSubscriberSystem.AddViewSubscriber(camera, player);
			return camera;
		}

		// Token: 0x04000356 RID: 854
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000357 RID: 855
		[Dependency]
		private readonly ViewSubscriberSystem _viewSubscriberSystem;

		// Token: 0x04000358 RID: 856
		private const int TabletopSeparation = 100;

		// Token: 0x0400035A RID: 858
		private int _tabletops;
	}
}
