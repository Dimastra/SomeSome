using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Radiation.Overlays;
using Content.Shared.Radiation.Events;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.Radiation.Systems
{
	// Token: 0x0200017A RID: 378
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationSystem : EntitySystem
	{
		// Token: 0x060009D2 RID: 2514 RVA: 0x00039161 File Offset: 0x00037361
		public override void Initialize()
		{
			base.SubscribeNetworkEvent<OnRadiationOverlayToggledEvent>(new EntityEventHandler<OnRadiationOverlayToggledEvent>(this.OnOverlayToggled), null, null);
			base.SubscribeNetworkEvent<OnRadiationOverlayUpdateEvent>(new EntityEventHandler<OnRadiationOverlayUpdateEvent>(this.OnOverlayUpdate), null, null);
			base.SubscribeNetworkEvent<OnRadiationOverlayResistanceUpdateEvent>(new EntityEventHandler<OnRadiationOverlayResistanceUpdateEvent>(this.OnResistanceUpdate), null, null);
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0003919F File Offset: 0x0003739F
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayMan.RemoveOverlay<RadiationDebugOverlay>();
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x000391B3 File Offset: 0x000373B3
		private void OnOverlayToggled(OnRadiationOverlayToggledEvent ev)
		{
			if (ev.IsEnabled)
			{
				this._overlayMan.AddOverlay(new RadiationDebugOverlay());
				return;
			}
			this._overlayMan.RemoveOverlay<RadiationDebugOverlay>();
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x000391DC File Offset: 0x000373DC
		private void OnOverlayUpdate(OnRadiationOverlayUpdateEvent ev)
		{
			RadiationDebugOverlay radiationDebugOverlay;
			if (!this._overlayMan.TryGetOverlay<RadiationDebugOverlay>(ref radiationDebugOverlay))
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 4);
			defaultInterpolatedStringHandler.AppendLiteral("Radiation update: ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(ev.ElapsedTimeMs);
			defaultInterpolatedStringHandler.AppendLiteral("ms with. Receivers: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(ev.ReceiversCount);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendLiteral("Sources: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(ev.SourcesCount);
			defaultInterpolatedStringHandler.AppendLiteral(", Rays: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(ev.Rays.Count);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			this.Rays = ev.Rays;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00039290 File Offset: 0x00037490
		private void OnResistanceUpdate(OnRadiationOverlayResistanceUpdateEvent ev)
		{
			RadiationDebugOverlay radiationDebugOverlay;
			if (!this._overlayMan.TryGetOverlay<RadiationDebugOverlay>(ref radiationDebugOverlay))
			{
				return;
			}
			this.ResistanceGrids = ev.Grids;
		}

		// Token: 0x040004E0 RID: 1248
		[Dependency]
		private readonly IOverlayManager _overlayMan;

		// Token: 0x040004E1 RID: 1249
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public List<RadiationRay> Rays;

		// Token: 0x040004E2 RID: 1250
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Dictionary<EntityUid, Dictionary<Vector2i, float>> ResistanceGrids;
	}
}
