using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000798 RID: 1944
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AutomaticAtmosSystem : EntitySystem
	{
		// Token: 0x06002A04 RID: 10756 RVA: 0x000DCE5C File Offset: 0x000DB05C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged), null, null);
		}

		// Token: 0x06002A05 RID: 10757 RVA: 0x000DCE78 File Offset: 0x000DB078
		private void OnTileChanged(ref TileChangedEvent ev)
		{
			if (((!ev.OldTile.IsSpace(this._tileDefinitionManager) || ev.NewTile.IsSpace(this._tileDefinitionManager)) && (ev.OldTile.IsSpace(this._tileDefinitionManager) || !ev.NewTile.IsSpace(this._tileDefinitionManager))) || this._atmosphereSystem.HasAtmosphere(ev.Entity))
			{
				return;
			}
			PhysicsComponent physics;
			if (!base.TryComp<PhysicsComponent>(ev.Entity, ref physics))
			{
				return;
			}
			if (physics.Mass / 0.5f >= 7f)
			{
				base.AddComp<GridAtmosphereComponent>(ev.Entity);
				string text = "atmos";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Giving grid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(ev.Entity);
				defaultInterpolatedStringHandler.AppendLiteral(" GridAtmosphereComponent.");
				Logger.InfoS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x040019F4 RID: 6644
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x040019F5 RID: 6645
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;
	}
}
