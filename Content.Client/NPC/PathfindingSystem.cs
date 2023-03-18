using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.NPC
{
	// Token: 0x02000210 RID: 528
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PathfindingSystem : SharedPathfindingSystem
	{
		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x00053330 File Offset: 0x00051530
		// (set) Token: 0x06000DCF RID: 3535 RVA: 0x00053338 File Offset: 0x00051538
		public PathfindingDebugMode Modes
		{
			get
			{
				return this._modes;
			}
			set
			{
				IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
				if (value == PathfindingDebugMode.None)
				{
					this.Breadcrumbs.Clear();
					this.Polys.Clear();
					overlayManager.RemoveOverlay<PathfindingOverlay>();
				}
				else if (!overlayManager.HasOverlay<PathfindingOverlay>())
				{
					overlayManager.AddOverlay(new PathfindingOverlay(this.EntityManager, this._eyeManager, this._inputManager, this._mapManager, this._cache, this));
				}
				if ((value & PathfindingDebugMode.Steering) != PathfindingDebugMode.None)
				{
					this._steering.DebugEnabled = true;
				}
				else
				{
					this._steering.DebugEnabled = false;
				}
				this._modes = value;
				base.RaiseNetworkEvent(new RequestPathfindingDebugMessage
				{
					Mode = this._modes
				});
			}
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x000533E4 File Offset: 0x000515E4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<PathBreadcrumbsMessage>(new EntityEventHandler<PathBreadcrumbsMessage>(this.OnBreadcrumbs), null, null);
			base.SubscribeNetworkEvent<PathBreadcrumbsRefreshMessage>(new EntityEventHandler<PathBreadcrumbsRefreshMessage>(this.OnBreadcrumbsRefresh), null, null);
			base.SubscribeNetworkEvent<PathPolysMessage>(new EntityEventHandler<PathPolysMessage>(this.OnPolys), null, null);
			base.SubscribeNetworkEvent<PathPolysRefreshMessage>(new EntityEventHandler<PathPolysRefreshMessage>(this.OnPolysRefresh), null, null);
			base.SubscribeNetworkEvent<PathRouteMessage>(new EntityEventHandler<PathRouteMessage>(this.OnRoute), null, null);
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x0005345C File Offset: 0x0005165C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			for (int i = 0; i < this.Routes.Count; i++)
			{
				ValueTuple<TimeSpan, PathRouteMessage> valueTuple = this.Routes[i];
				if (this._timing.RealTime < valueTuple.Item1)
				{
					break;
				}
				this.Routes.RemoveAt(i);
			}
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x000534C5 File Offset: 0x000516C5
		private void OnRoute(PathRouteMessage ev)
		{
			this.Routes.Add(new ValueTuple<TimeSpan, PathRouteMessage>(this._timing.RealTime + TimeSpan.FromSeconds(0.5), ev));
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x000534F6 File Offset: 0x000516F6
		private void OnPolys(PathPolysMessage ev)
		{
			this.Polys = ev.Polys;
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00053504 File Offset: 0x00051704
		private void OnPolysRefresh(PathPolysRefreshMessage ev)
		{
			Extensions.GetOrNew<EntityUid, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>(this.Polys, ev.GridUid)[ev.Origin] = ev.Polys;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x00053528 File Offset: 0x00051728
		public override void Shutdown()
		{
			base.Shutdown();
			this._modes = PathfindingDebugMode.None;
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00053537 File Offset: 0x00051737
		private void OnBreadcrumbs(PathBreadcrumbsMessage ev)
		{
			this.Breadcrumbs = ev.Breadcrumbs;
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00053548 File Offset: 0x00051748
		private void OnBreadcrumbsRefresh(PathBreadcrumbsRefreshMessage ev)
		{
			Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
			if (!this.Breadcrumbs.TryGetValue(ev.GridUid, out dictionary))
			{
				return;
			}
			dictionary[ev.Origin] = ev.Data;
		}

		// Token: 0x040006C8 RID: 1736
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x040006C9 RID: 1737
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040006CA RID: 1738
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x040006CB RID: 1739
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040006CC RID: 1740
		[Dependency]
		private readonly IResourceCache _cache;

		// Token: 0x040006CD RID: 1741
		[Dependency]
		private readonly NPCSteeringSystem _steering;

		// Token: 0x040006CE RID: 1742
		private PathfindingDebugMode _modes;

		// Token: 0x040006CF RID: 1743
		public Dictionary<EntityUid, Dictionary<Vector2i, List<PathfindingBreadcrumb>>> Breadcrumbs = new Dictionary<EntityUid, Dictionary<Vector2i, List<PathfindingBreadcrumb>>>();

		// Token: 0x040006D0 RID: 1744
		public Dictionary<EntityUid, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>> Polys = new Dictionary<EntityUid, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>();

		// Token: 0x040006D1 RID: 1745
		[TupleElementNames(new string[]
		{
			"Time",
			"Message"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public readonly List<ValueTuple<TimeSpan, PathRouteMessage>> Routes = new List<ValueTuple<TimeSpan, PathRouteMessage>>();
	}
}
