using System;
using System.Runtime.CompilerServices;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Players;

namespace Content.Server.Popups
{
	// Token: 0x020002C2 RID: 706
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PopupSystem : SharedPopupSystem
	{
		// Token: 0x06000E3E RID: 3646 RVA: 0x000480F3 File Offset: 0x000462F3
		public override void PopupCursor(string message, PopupType type = PopupType.Small)
		{
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x000480F5 File Offset: 0x000462F5
		public override void PopupCursor(string message, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			base.RaiseNetworkEvent(new PopupCursorEvent(message, type), recipient);
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x00048108 File Offset: 0x00046308
		public override void PopupCursor(string message, EntityUid recipient, PopupType type = PopupType.Small)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(recipient, ref actor))
			{
				base.RaiseNetworkEvent(new PopupCursorEvent(message, type), actor.PlayerSession);
			}
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x00048133 File Offset: 0x00046333
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, Filter filter, bool replayRecord, PopupType type = PopupType.Small)
		{
			base.RaiseNetworkEvent(new PopupCoordinatesEvent(message, type, coordinates), filter, replayRecord);
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x00048148 File Offset: 0x00046348
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, PopupType type = PopupType.Small)
		{
			MapCoordinates mapPos = coordinates.ToMap(this.EntityManager);
			Filter filter = Filter.Empty().AddPlayersByPvs(mapPos, 2f, this.EntityManager, this._player, this._cfg);
			base.RaiseNetworkEvent(new PopupCoordinatesEvent(message, type, coordinates), filter, true);
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00048196 File Offset: 0x00046396
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			base.RaiseNetworkEvent(new PopupCoordinatesEvent(message, type, coordinates), recipient);
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x000481A8 File Offset: 0x000463A8
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, EntityUid recipient, PopupType type = PopupType.Small)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(recipient, ref actor))
			{
				base.RaiseNetworkEvent(new PopupCoordinatesEvent(message, type, coordinates), actor.PlayerSession);
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x000481D8 File Offset: 0x000463D8
		public override void PopupEntity(string message, EntityUid uid, PopupType type = PopupType.Small)
		{
			Filter filter = Filter.Empty().AddPlayersByPvs(uid, 2f, this.EntityManager, this._player, this._cfg);
			base.RaiseNetworkEvent(new PopupEntityEvent(message, type, uid), filter, true);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00048218 File Offset: 0x00046418
		public override void PopupEntity(string message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(recipient, ref actor))
			{
				base.RaiseNetworkEvent(new PopupEntityEvent(message, type, uid), actor.PlayerSession);
			}
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00048245 File Offset: 0x00046445
		public override void PopupEntity(string message, EntityUid uid, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			base.RaiseNetworkEvent(new PopupEntityEvent(message, type, uid), recipient);
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00048257 File Offset: 0x00046457
		public override void PopupEntity(string message, EntityUid uid, Filter filter, bool recordReplay, PopupType type = PopupType.Small)
		{
			base.RaiseNetworkEvent(new PopupEntityEvent(message, type, uid), filter, recordReplay);
		}

		// Token: 0x04000855 RID: 2133
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000856 RID: 2134
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
