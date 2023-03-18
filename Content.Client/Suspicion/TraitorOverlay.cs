using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Client.Suspicion
{
	// Token: 0x02000103 RID: 259
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitorOverlay : Overlay
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0002611C File Offset: 0x0002431C
		public TraitorOverlay(IEntityManager entityManager, IPlayerManager playerManager, IResourceCache resourceCache, EntityLookupSystem lookup)
		{
			this._playerManager = playerManager;
			this._entityManager = entityManager;
			this._lookup = lookup;
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00026170 File Offset: 0x00024370
		protected override void Draw(in OverlayDrawArgs args)
		{
			TraitorOverlay.<>c__DisplayClass8_0 CS$<>8__locals1 = new TraitorOverlay.<>c__DisplayClass8_0();
			Box2 worldAABB = args.WorldAABB;
			TraitorOverlay.<>c__DisplayClass8_0 CS$<>8__locals2 = CS$<>8__locals1;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			CS$<>8__locals2.ent = ((localPlayer != null) ? localPlayer.ControlledEntity : null);
			SuspicionRoleComponent suspicionRoleComponent;
			if (!this._entityManager.TryGetComponent<SuspicionRoleComponent>(CS$<>8__locals1.ent, ref suspicionRoleComponent))
			{
				return;
			}
			foreach (ValueTuple<string, EntityUid> valueTuple in suspicionRoleComponent.Allies)
			{
				EntityUid ally = valueTuple.Item2;
				PhysicsComponent physicsComponent;
				if (this._entityManager.EntityExists(ally) && this._entityManager.TryGetComponent<PhysicsComponent>(ally, ref physicsComponent))
				{
					TransformComponent component = this._entityManager.GetComponent<TransformComponent>(ally);
					MapCoordinates mapPosition = this._entityManager.GetComponent<TransformComponent>(CS$<>8__locals1.ent.Value).MapPosition;
					MapCoordinates mapPosition2 = component.MapPosition;
					if (ExamineSystemShared.InRangeUnOccluded(mapPosition, mapPosition2, 15f, (EntityUid entity) => entity == CS$<>8__locals1.ent || entity == ally, true, null) && !(component.MapID != args.Viewport.Eye.Position.MapId) && !ContainerHelpers.IsInContainer(physicsComponent.Owner, null))
					{
						component.GetWorldPositionRotation();
						Box2 worldAABB2 = this._lookup.GetWorldAABB(ally, component);
						if (worldAABB2.Intersects(ref worldAABB) && !worldAABB2.IsEmpty())
						{
							Vector2 vector = args.ViewportControl.WorldToScreen(worldAABB2.TopLeft + new ValueTuple<float, float>(0f, 0.5f));
							args.ScreenHandle.DrawString(this._font, vector, this._traitorText, Color.OrangeRed);
						}
					}
				}
			}
		}

		// Token: 0x0400035C RID: 860
		private readonly IEntityManager _entityManager;

		// Token: 0x0400035D RID: 861
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400035E RID: 862
		private readonly EntityLookupSystem _lookup;

		// Token: 0x0400035F RID: 863
		private readonly Font _font;

		// Token: 0x04000360 RID: 864
		private readonly string _traitorText = Loc.GetString("traitor-overlay-traitor-text");
	}
}
