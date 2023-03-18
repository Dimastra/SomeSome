using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Storage.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x02000164 RID: 356
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpawnItemsOnUseSystem : EntitySystem
	{
		// Token: 0x060006E5 RID: 1765 RVA: 0x00022223 File Offset: 0x00020423
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpawnItemsOnUseComponent, UseInHandEvent>(new ComponentEventHandler<SpawnItemsOnUseComponent, UseInHandEvent>(this.OnUseInHand), null, null);
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x00022240 File Offset: 0x00020440
		private void OnUseInHand(EntityUid uid, SpawnItemsOnUseComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityCoordinates coords = base.Transform(args.User).Coordinates;
			List<string> spawns = EntitySpawnCollection.GetSpawns(component.Items, this._random);
			EntityUid? entityToPlaceInHands = null;
			foreach (string proto in spawns)
			{
				entityToPlaceInHands = new EntityUid?(base.Spawn(proto, coords));
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.EntitySpawn;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(21, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "ToPrettyString(args.User)");
				logStringHandler.AppendLiteral(" used ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "ToPrettyString(component.Owner)");
				logStringHandler.AppendLiteral(" which spawned ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entityToPlaceInHands.Value), "ToPrettyString(entityToPlaceInHands.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			if (component.Sound != null)
			{
				SoundSystem.Play(component.Sound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, null);
			}
			component.Uses--;
			if (component.Uses == 0)
			{
				args.Handled = true;
				this.EntityManager.DeleteEntity(uid);
			}
			if (entityToPlaceInHands != null)
			{
				this._handsSystem.PickupOrDrop(new EntityUid?(args.User), entityToPlaceInHands.Value, true, false, null, null);
			}
		}

		// Token: 0x040003F5 RID: 1013
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040003F6 RID: 1014
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040003F7 RID: 1015
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;
	}
}
