using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Construction
{
	// Token: 0x0200038E RID: 910
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConstructionPlacementHijack : PlacementHijack
	{
		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001655 RID: 5717 RVA: 0x000836B5 File Offset: 0x000818B5
		public override bool CanRotate { get; }

		// Token: 0x06001656 RID: 5718 RVA: 0x000836BD File Offset: 0x000818BD
		public ConstructionPlacementHijack(ConstructionSystem constructionSystem, [Nullable(2)] ConstructionPrototype prototype)
		{
			this._constructionSystem = constructionSystem;
			this._prototype = prototype;
			this.CanRotate = (prototype == null || prototype.CanRotate);
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x000836E8 File Offset: 0x000818E8
		public override bool HijackPlacementRequest(EntityCoordinates coordinates)
		{
			if (this._prototype != null)
			{
				Direction direction = base.Manager.Direction;
				this._constructionSystem.SpawnGhost(this._prototype, coordinates, direction);
			}
			return true;
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x00083720 File Offset: 0x00081920
		public override bool HijackDeletion(EntityUid entity)
		{
			ConstructionGhostComponent constructionGhostComponent;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<ConstructionGhostComponent>(entity, ref constructionGhostComponent))
			{
				this._constructionSystem.ClearGhost(constructionGhostComponent.GhostId);
			}
			return true;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x00083750 File Offset: 0x00081950
		public override void StartHijack(PlacementManager manager)
		{
			base.StartHijack(manager);
			ConstructionPrototype prototype = this._prototype;
			IDirectionalTextureProvider directionalTextureProvider = (prototype != null) ? SpriteSpecifierExt.DirFrame0(prototype.Icon) : null;
			if (directionalTextureProvider == null)
			{
				manager.CurrentTextures = null;
				return;
			}
			manager.CurrentTextures = new List<IDirectionalTextureProvider>
			{
				directionalTextureProvider
			};
		}

		// Token: 0x04000BC8 RID: 3016
		private readonly ConstructionSystem _constructionSystem;

		// Token: 0x04000BC9 RID: 3017
		[Nullable(2)]
		private readonly ConstructionPrototype _prototype;
	}
}
