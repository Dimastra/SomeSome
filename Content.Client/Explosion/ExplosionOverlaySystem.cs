using System;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Explosion
{
	// Token: 0x02000324 RID: 804
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExplosionOverlaySystem : EntitySystem
	{
		// Token: 0x0600143C RID: 5180 RVA: 0x00076C6C File Offset: 0x00074E6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentInit>(new ComponentEventHandler<ExplosionVisualsComponent, ComponentInit>(this.OnExplosionInit), null, null);
			base.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentRemove>(new ComponentEventHandler<ExplosionVisualsComponent, ComponentRemove>(this.OnCompRemove), null, null);
			base.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<ExplosionVisualsComponent, ComponentHandleState>(this.OnExplosionHandleState), null, null);
			this._overlayMan.AddOverlay(new ExplosionOverlay());
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x00076CCC File Offset: 0x00074ECC
		private void OnExplosionHandleState(EntityUid uid, ExplosionVisualsComponent component, ref ComponentHandleState args)
		{
			ExplosionVisualsState explosionVisualsState = args.Current as ExplosionVisualsState;
			if (explosionVisualsState == null)
			{
				return;
			}
			component.Epicenter = explosionVisualsState.Epicenter;
			component.SpaceTiles = explosionVisualsState.SpaceTiles;
			component.Tiles = explosionVisualsState.Tiles;
			component.Intensity = explosionVisualsState.Intensity;
			component.ExplosionType = explosionVisualsState.ExplosionType;
			component.SpaceMatrix = explosionVisualsState.SpaceMatrix;
			component.SpaceTileSize = explosionVisualsState.SpaceTileSize;
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00076D3D File Offset: 0x00074F3D
		private void OnCompRemove(EntityUid uid, ExplosionVisualsComponent component, ComponentRemove args)
		{
			base.QueueDel(component.LightEntity);
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x00076D4C File Offset: 0x00074F4C
		private void OnExplosionInit(EntityUid uid, ExplosionVisualsComponent component, ComponentInit args)
		{
			ExplosionPrototype explosionPrototype;
			if (!this._protoMan.TryIndex<ExplosionPrototype>(component.ExplosionType, ref explosionPrototype))
			{
				return;
			}
			EntityUid entityUid = base.Spawn("ExplosionLight", component.Epicenter);
			PointLightComponent pointLightComponent = base.EnsureComp<PointLightComponent>(entityUid);
			pointLightComponent.Energy = (pointLightComponent.Radius = (float)component.Intensity.Count);
			pointLightComponent.Color = explosionPrototype.LightColor;
			component.LightEntity = entityUid;
			component.FireColor = explosionPrototype.FireColor;
			component.IntensityPerState = explosionPrototype.IntensityPerState;
			foreach (RSI.State state in this._resCache.GetResource<RSIResource>(explosionPrototype.TexturePath, true).RSI)
			{
				component.FireFrames.Add(state.GetFrames(0));
				if (component.FireFrames.Count == explosionPrototype.FireStates)
				{
					break;
				}
			}
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x00076E40 File Offset: 0x00075040
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayMan.RemoveOverlay<ExplosionOverlay>();
		}

		// Token: 0x04000A2C RID: 2604
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000A2D RID: 2605
		[Dependency]
		private readonly IResourceCache _resCache;

		// Token: 0x04000A2E RID: 2606
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x04000A2F RID: 2607
		public float ExplosionPersistence = 0.3f;
	}
}
