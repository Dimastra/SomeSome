using System;
using System.Runtime.CompilerServices;
using Content.Client.Parallax.Managers;
using Content.Shared.Parallax;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client.Parallax
{
	// Token: 0x020001DC RID: 476
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ParallaxSystem : SharedParallaxSystem
	{
		// Token: 0x06000C4F RID: 3151 RVA: 0x00048350 File Offset: 0x00046550
		public override void Initialize()
		{
			base.Initialize();
			this._overlay.AddOverlay(new ParallaxOverlay());
			base.SubscribeLocalEvent<ParallaxComponent, ComponentHandleState>(new ComponentEventRefHandler<ParallaxComponent, ComponentHandleState>(this.OnParallaxHandleState), null, null);
			this._protoManager.PrototypesReloaded += this.OnReload;
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x000483A0 File Offset: 0x000465A0
		private void OnReload(PrototypesReloadedEventArgs obj)
		{
			this._parallax.UnloadParallax("Default");
			this._parallax.LoadDefaultParallax();
			foreach (ParallaxComponent parallaxComponent in base.EntityQuery<ParallaxComponent>(true))
			{
				this._parallax.UnloadParallax(parallaxComponent.Parallax);
				this._parallax.LoadParallaxByName(parallaxComponent.Parallax);
			}
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x00048428 File Offset: 0x00046628
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlay.RemoveOverlay<ParallaxOverlay>();
			this._protoManager.PrototypesReloaded -= this.OnReload;
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x00048454 File Offset: 0x00046654
		private void OnParallaxHandleState(EntityUid uid, ParallaxComponent component, ref ComponentHandleState args)
		{
			SharedParallaxSystem.ParallaxComponentState parallaxComponentState = args.Current as SharedParallaxSystem.ParallaxComponentState;
			if (parallaxComponentState == null)
			{
				return;
			}
			component.Parallax = parallaxComponentState.Parallax;
			if (!this._parallax.IsLoaded(component.Parallax))
			{
				this._parallax.LoadParallaxByName(component.Parallax);
			}
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x000484A2 File Offset: 0x000466A2
		public ParallaxLayerPrepared[] GetParallaxLayers(MapId mapId)
		{
			return this._parallax.GetParallaxLayers(this.GetParallax(this._map.GetMapEntityId(mapId)));
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x000484C1 File Offset: 0x000466C1
		public string GetParallax(MapId mapId)
		{
			return this.GetParallax(this._map.GetMapEntityId(mapId));
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x000484D8 File Offset: 0x000466D8
		public string GetParallax(EntityUid mapUid)
		{
			ParallaxComponent parallaxComponent;
			if (!base.TryComp<ParallaxComponent>(mapUid, ref parallaxComponent))
			{
				return "Default";
			}
			return parallaxComponent.Parallax;
		}

		// Token: 0x04000616 RID: 1558
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x04000617 RID: 1559
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x04000618 RID: 1560
		[Dependency]
		private readonly IParallaxManager _parallax;

		// Token: 0x04000619 RID: 1561
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x0400061A RID: 1562
		private const string Fallback = "Default";

		// Token: 0x0400061B RID: 1563
		public const int ParallaxZIndex = 0;
	}
}
