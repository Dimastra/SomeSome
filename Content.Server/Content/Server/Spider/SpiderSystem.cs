using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Spider;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Spider
{
	// Token: 0x020001AC RID: 428
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpiderSystem : SharedSpiderSystem
	{
		// Token: 0x06000871 RID: 2161 RVA: 0x0002B2BD File Offset: 0x000294BD
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpiderComponent, SpiderWebActionEvent>(new ComponentEventHandler<SpiderComponent, SpiderWebActionEvent>(this.OnSpawnNet), null, null);
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0002B2DC File Offset: 0x000294DC
		private void OnSpawnNet(EntityUid uid, SpiderComponent component, SpiderWebActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TransformComponent transform = base.Transform(uid);
			if (transform.GridUid == null)
			{
				this._popup.PopupEntity(Loc.GetString("spider-web-action-nogrid"), args.Performer, args.Performer, PopupType.Small);
				return;
			}
			EntityCoordinates coords = transform.Coordinates;
			bool result = false;
			if (!this.IsTileBlockedByWeb(coords))
			{
				base.Spawn(component.WebPrototype, coords);
				result = true;
			}
			for (int i = 0; i < 4; i++)
			{
				DirectionFlag direction = (sbyte)(1 << i);
				coords = transform.Coordinates.Offset(DirectionExtensions.ToVec(DirectionExtensions.AsDir(direction)));
				if (!this.IsTileBlockedByWeb(coords))
				{
					base.Spawn(component.WebPrototype, coords);
					result = true;
				}
			}
			if (result)
			{
				this._popup.PopupEntity(Loc.GetString("spider-web-action-success"), args.Performer, args.Performer, PopupType.Small);
				args.Handled = true;
				return;
			}
			this._popup.PopupEntity(Loc.GetString("spider-web-action-fail"), args.Performer, args.Performer, PopupType.Small);
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0002B3F0 File Offset: 0x000295F0
		private bool IsTileBlockedByWeb(EntityCoordinates coords)
		{
			foreach (EntityUid entity in coords.GetEntitiesInTile(4, null))
			{
				if (base.HasComp<SpiderWebObjectComponent>(entity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400052B RID: 1323
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
