using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Resources;
using Content.Shared.Access.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Access
{
	// Token: 0x020004F5 RID: 1269
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AccessOverlay : Overlay
	{
		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600204C RID: 8268 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600204D RID: 8269 RVA: 0x000BB5A7 File Offset: 0x000B97A7
		public AccessOverlay(IEntityManager entManager, IResourceCache cache, EntityLookupSystem lookup)
		{
			this._entityManager = entManager;
			this._lookup = lookup;
			this._font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
		}

		// Token: 0x0600204E RID: 8270 RVA: 0x000BB5D0 File Offset: 0x000B97D0
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (args.ViewportControl == null)
			{
				return;
			}
			EntityQuery<AccessReaderComponent> entityQuery = this._entityManager.GetEntityQuery<AccessReaderComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entityManager.GetEntityQuery<TransformComponent>();
			foreach (EntityUid entityUid in this._lookup.GetEntitiesIntersecting(args.MapId, args.WorldAABB, 5))
			{
				AccessReaderComponent accessReaderComponent;
				TransformComponent transformComponent;
				if (entityQuery.TryGetComponent(entityUid, ref accessReaderComponent) && entityQuery2.TryGetComponent(entityUid, ref transformComponent))
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num = 0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this._entityManager.ToPrettyString(entityUid));
					string value = defaultInterpolatedStringHandler.ToStringAndClear();
					stringBuilder.Append(value);
					foreach (HashSet<string> hashSet in accessReaderComponent.AccessLists)
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Tag ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						value = defaultInterpolatedStringHandler.ToStringAndClear();
						stringBuilder.AppendLine(value);
						foreach (string str in hashSet)
						{
							value = "- " + str;
							stringBuilder.AppendLine(value);
						}
						num++;
					}
					string text;
					if (stringBuilder.Length >= 2)
					{
						text = stringBuilder.ToString();
						string text2 = text;
						text = text2.Substring(0, text2.Length - 2);
					}
					else
					{
						text = "";
					}
					Vector2 vector = args.ViewportControl.WorldToScreen(transformComponent.WorldPosition);
					args.ScreenHandle.DrawString(this._font, vector, text, Color.Gold);
				}
			}
		}

		// Token: 0x04000F6A RID: 3946
		private readonly IEntityManager _entityManager;

		// Token: 0x04000F6B RID: 3947
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000F6C RID: 3948
		private readonly Font _font;
	}
}
