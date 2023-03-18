using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Hands.Systems;
using Content.Server.Popups;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;

namespace Content.Server.Holiday.Christmas
{
	// Token: 0x02000470 RID: 1136
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LimitedItemGiverSystem : EntitySystem
	{
		// Token: 0x060016C4 RID: 5828 RVA: 0x00077CA2 File Offset: 0x00075EA2
		public override void Initialize()
		{
			base.SubscribeLocalEvent<LimitedItemGiverComponent, InteractHandEvent>(new ComponentEventHandler<LimitedItemGiverComponent, InteractHandEvent>(this.OnInteractHand), null, null);
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00077CB8 File Offset: 0x00075EB8
		private void OnInteractHand(EntityUid uid, LimitedItemGiverComponent component, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (component.GrantedPlayers.Contains(actor.PlayerSession.UserId) || (component.RequiredHoliday != null && !this._holiday.IsCurrentlyHoliday(component.RequiredHoliday)))
			{
				this._popup.PopupEntity(Loc.GetString(component.DeniedPopup), uid, args.User, PopupType.Small);
				return;
			}
			List<string> spawns = EntitySpawnCollection.GetSpawns(component.SpawnEntries, null);
			EntityCoordinates coords = base.Transform(args.User).Coordinates;
			foreach (string item in spawns)
			{
				if (item != null)
				{
					EntityUid spawned = base.Spawn(item, coords);
					this._hands.PickupOrDrop(new EntityUid?(args.User), spawned, true, false, null, null);
				}
			}
			component.GrantedPlayers.Add(actor.PlayerSession.UserId);
			this._popup.PopupEntity(Loc.GetString(component.ReceivedPopup), uid, args.User, PopupType.Small);
		}

		// Token: 0x04000E42 RID: 3650
		[Dependency]
		private readonly HandsSystem _hands;

		// Token: 0x04000E43 RID: 3651
		[Dependency]
		private readonly HolidaySystem _holiday;

		// Token: 0x04000E44 RID: 3652
		[Dependency]
		private readonly PopupSystem _popup;
	}
}
