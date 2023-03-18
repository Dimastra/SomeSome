using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Shared.Revenant.Components;
using Content.Shared.Revenant.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Revenant.EntitySystems
{
	// Token: 0x02000230 RID: 560
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CorporealSystem : SharedCorporealSystem
	{
		// Token: 0x06000B24 RID: 2852 RVA: 0x0003A2FC File Offset: 0x000384FC
		public override void OnStartup(EntityUid uid, CorporealComponent component, ComponentStartup args)
		{
			base.OnStartup(uid, component, args);
			VisibilityComponent visibility;
			if (base.TryComp<VisibilityComponent>(uid, ref visibility))
			{
				this._visibilitySystem.RemoveLayer(visibility, 2, false);
				this._visibilitySystem.AddLayer(visibility, 1, false);
				this._visibilitySystem.RefreshVisibility(visibility);
			}
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0003A348 File Offset: 0x00038548
		public override void OnShutdown(EntityUid uid, CorporealComponent component, ComponentShutdown args)
		{
			base.OnShutdown(uid, component, args);
			VisibilityComponent visibility;
			if (base.TryComp<VisibilityComponent>(uid, ref visibility) && this._ticker.RunLevel != GameRunLevel.PostRound)
			{
				this._visibilitySystem.AddLayer(visibility, 2, false);
				this._visibilitySystem.RemoveLayer(visibility, 1, false);
				this._visibilitySystem.RefreshVisibility(visibility);
			}
		}

		// Token: 0x040006CF RID: 1743
		[Dependency]
		private readonly VisibilitySystem _visibilitySystem;

		// Token: 0x040006D0 RID: 1744
		[Dependency]
		private readonly GameTicker _ticker;
	}
}
