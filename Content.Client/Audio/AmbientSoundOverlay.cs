using System;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Audio
{
	// Token: 0x02000427 RID: 1063
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AmbientSoundOverlay : Overlay
	{
		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060019EE RID: 6638 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x000946DB File Offset: 0x000928DB
		public AmbientSoundOverlay(IEntityManager entManager, AmbientSoundSystem ambient, EntityLookupSystem lookup)
		{
			this._entManager = entManager;
			this._ambient = ambient;
			this._lookup = lookup;
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x000946F8 File Offset: 0x000928F8
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleWorld worldHandle = args.WorldHandle;
			EntityQuery<AmbientSoundComponent> entityQuery = this._entManager.GetEntityQuery<AmbientSoundComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
			foreach (EntityUid entityUid in this._lookup.GetEntitiesIntersecting(args.MapId, args.WorldBounds, 46))
			{
				AmbientSoundComponent ambientSoundComponent;
				TransformComponent transformComponent;
				if (entityQuery.TryGetComponent(entityUid, ref ambientSoundComponent) && entityQuery2.TryGetComponent(entityUid, ref transformComponent))
				{
					if (ambientSoundComponent.Enabled)
					{
						if (this._ambient.IsActive(ambientSoundComponent))
						{
							worldHandle.DrawCircle(transformComponent.WorldPosition, 0.25f, Color.LightGreen.WithAlpha(0.5f), true);
						}
						else
						{
							worldHandle.DrawCircle(transformComponent.WorldPosition, 0.25f, Color.Orange.WithAlpha(0.25f), true);
						}
					}
					else
					{
						worldHandle.DrawCircle(transformComponent.WorldPosition, 0.25f, Color.Red.WithAlpha(0.25f), true);
					}
				}
			}
		}

		// Token: 0x04000D20 RID: 3360
		private IEntityManager _entManager;

		// Token: 0x04000D21 RID: 3361
		private AmbientSoundSystem _ambient;

		// Token: 0x04000D22 RID: 3362
		private EntityLookupSystem _lookup;
	}
}
