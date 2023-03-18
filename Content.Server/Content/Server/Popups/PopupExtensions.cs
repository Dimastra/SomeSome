using System;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Server.Popups
{
	// Token: 0x020002C0 RID: 704
	[NullableContext(1)]
	[Nullable(0)]
	public static class PopupExtensions
	{
		// Token: 0x06000E37 RID: 3639 RVA: 0x00047FF4 File Offset: 0x000461F4
		[Obsolete("Use PopupSystem.PopupEntity instead")]
		public static void PopupMessageOtherClients(this EntityUid source, string message)
		{
			foreach (ICommonSession viewer in Filter.Empty().AddPlayersByPvs(source, 2f, null, null, null).Recipients)
			{
				EntityUid? attachedEntity = viewer.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid viewerEntity = attachedEntity.GetValueOrDefault();
					if (viewerEntity.Valid && !(source == viewerEntity) && viewer.AttachedEntity != null)
					{
						source.PopupMessage(viewerEntity, message);
					}
				}
			}
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00048090 File Offset: 0x00046290
		[Obsolete("Use PopupSystem.PopupEntity instead")]
		public static void PopupMessageEveryone(this EntityUid source, string message, [Nullable(2)] IPlayerManager playerManager = null, int range = 15)
		{
			source.PopupMessage(message);
			source.PopupMessageOtherClients(message);
		}
	}
}
