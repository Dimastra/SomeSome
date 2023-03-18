using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Storage.Components;
using Content.Shared.Audio;
using Content.Shared.Storage.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Storage.EntitySystems
{
	// Token: 0x0200015E RID: 350
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CursedEntityStorageSystem : EntitySystem
	{
		// Token: 0x060006B3 RID: 1715 RVA: 0x00020C48 File Offset: 0x0001EE48
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CursedEntityStorageComponent, StorageAfterCloseEvent>(new ComponentEventRefHandler<CursedEntityStorageComponent, StorageAfterCloseEvent>(this.OnClose), null, null);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00020C64 File Offset: 0x0001EE64
		private void OnClose(EntityUid uid, CursedEntityStorageComponent component, ref StorageAfterCloseEvent args)
		{
			EntityStorageComponent storage;
			if (!base.TryComp<EntityStorageComponent>(uid, ref storage))
			{
				return;
			}
			if (storage.Open || storage.Contents.ContainedEntities.Count <= 0)
			{
				return;
			}
			List<EntityStorageComponent> lockerQuery = base.EntityQuery<EntityStorageComponent>(false).ToList<EntityStorageComponent>();
			lockerQuery.Remove(storage);
			if (lockerQuery.Count == 0)
			{
				return;
			}
			EntityUid lockerEnt = RandomExtensions.Pick<EntityStorageComponent>(this._random, lockerQuery).Owner;
			foreach (EntityUid entity in storage.Contents.ContainedEntities.ToArray<EntityUid>())
			{
				storage.Contents.Remove(entity, null, null, null, true, false, null, null);
				this._entityStorage.AddToContents(entity, lockerEnt, null);
			}
			SoundSystem.Play(component.CursedSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.125f, this._random)));
		}

		// Token: 0x040003E0 RID: 992
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040003E1 RID: 993
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;
	}
}
