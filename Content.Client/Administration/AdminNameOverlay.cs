using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Systems;
using Content.Shared.Administration;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Administration
{
	// Token: 0x02000484 RID: 1156
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AdminNameOverlay : Overlay
	{
		// Token: 0x06001C7B RID: 7291 RVA: 0x000A51E4 File Offset: 0x000A33E4
		public AdminNameOverlay(AdminSystem system, IEntityManager entityManager, IEyeManager eyeManager, IResourceCache resourceCache, EntityLookupSystem entityLookup)
		{
			this._system = system;
			this._entityManager = entityManager;
			this._eyeManager = eyeManager;
			this._entityLookup = entityLookup;
			base.ZIndex = new int?(200);
			this._font = new VectorFont(resourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06001C7C RID: 7292 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x000A5240 File Offset: 0x000A3440
		protected override void Draw(in OverlayDrawArgs args)
		{
			Box2 worldAABB = args.WorldAABB;
			foreach (PlayerInfo playerInfo in this._system.PlayerList)
			{
				if (this._entityManager.EntityExists(playerInfo.EntityUid))
				{
					EntityUid value = playerInfo.EntityUid.Value;
					if (!(this._entityManager.GetComponent<TransformComponent>(value).MapID != this._eyeManager.CurrentMap))
					{
						Box2 worldAABB2 = this._entityLookup.GetWorldAABB(value, null);
						if (worldAABB2.Intersects(ref worldAABB))
						{
							Vector2 vector;
							vector..ctor(0f, 11f);
							IEyeManager eyeManager = this._eyeManager;
							Vector2 center = worldAABB2.Center;
							Angle angle = new Angle(-this._eyeManager.CurrentEye.Rotation);
							Vector2 vector2 = worldAABB2.TopRight - worldAABB2.Center;
							Vector2 vector3 = eyeManager.WorldToScreen(center + angle.RotateVec(ref vector2)) + new Vector2(1f, 7f);
							if (playerInfo.Antag)
							{
								args.ScreenHandle.DrawString(this._font, vector3 + vector * 2f, "ANTAG", Color.OrangeRed);
							}
							args.ScreenHandle.DrawString(this._font, vector3 + vector, playerInfo.Username, playerInfo.Connected ? Color.Yellow : Color.White);
							args.ScreenHandle.DrawString(this._font, vector3, playerInfo.CharacterName, playerInfo.Connected ? Color.Aquamarine : Color.White);
						}
					}
				}
			}
		}

		// Token: 0x04000E40 RID: 3648
		private readonly AdminSystem _system;

		// Token: 0x04000E41 RID: 3649
		private readonly IEntityManager _entityManager;

		// Token: 0x04000E42 RID: 3650
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000E43 RID: 3651
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04000E44 RID: 3652
		private readonly Font _font;
	}
}
