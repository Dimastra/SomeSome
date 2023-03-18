using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Server.IgnitionSource
{
	// Token: 0x02000456 RID: 1110
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IgnitionSourceSystem : EntitySystem
	{
		// Token: 0x06001661 RID: 5729 RVA: 0x0007629C File Offset: 0x0007449C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IgnitionSourceComponent, IsHotEvent>(new ComponentEventHandler<IgnitionSourceComponent, IsHotEvent>(this.OnIsHot), null, null);
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x000762B8 File Offset: 0x000744B8
		private void OnIsHot(EntityUid uid, IgnitionSourceComponent component, IsHotEvent args)
		{
			Logger.Debug(args.IsHot.ToString());
			this.SetIgnited(uid, component, args.IsHot);
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x000762E6 File Offset: 0x000744E6
		private void SetIgnited(EntityUid uid, IgnitionSourceComponent component, bool newState)
		{
			component.Ignited = newState;
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x000762F0 File Offset: 0x000744F0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<IgnitionSourceComponent, TransformComponent> valueTuple in base.EntityQuery<IgnitionSourceComponent, TransformComponent>(false))
			{
				IgnitionSourceComponent component = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				EntityUid source = component.Owner;
				if (component.Ignited)
				{
					EntityUid? gridUid2 = transform.GridUid;
					if (gridUid2 != null)
					{
						EntityUid gridUid = gridUid2.GetValueOrDefault();
						Vector2i position = this._transformSystem.GetGridOrMapTilePosition(source, transform);
						this._atmosphereSystem.HotspotExpose(gridUid, position, (float)component.Temperature, 50f, true);
					}
				}
			}
		}

		// Token: 0x04000E05 RID: 3589
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000E06 RID: 3590
		[Dependency]
		private readonly TransformSystem _transformSystem;
	}
}
