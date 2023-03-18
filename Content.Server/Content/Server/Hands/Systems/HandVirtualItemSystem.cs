using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Hands.Systems
{
	// Token: 0x02000479 RID: 1145
	public sealed class HandVirtualItemSystem : SharedHandVirtualItemSystem
	{
		// Token: 0x060016E7 RID: 5863 RVA: 0x00078C74 File Offset: 0x00076E74
		public bool TrySpawnVirtualItemInHand(EntityUid blockingEnt, EntityUid user)
		{
			EntityUid? entityUid;
			return this.TrySpawnVirtualItemInHand(blockingEnt, user, out entityUid);
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x00078C8C File Offset: 0x00076E8C
		public bool TrySpawnVirtualItemInHand(EntityUid blockingEnt, EntityUid user, [NotNullWhen(true)] out EntityUid? virtualItem)
		{
			Hand hand;
			if (!this._handsSystem.TryGetEmptyHand(user, out hand, null))
			{
				virtualItem = null;
				return false;
			}
			EntityCoordinates pos = this.EntityManager.GetComponent<TransformComponent>(user).Coordinates;
			virtualItem = new EntityUid?(this.EntityManager.SpawnEntity("HandVirtualItem", pos));
			this.EntityManager.GetComponent<HandVirtualItemComponent>(virtualItem.Value).BlockingEntity = blockingEnt;
			this._handsSystem.DoPickup(user, hand, virtualItem.Value, null);
			return true;
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x00078D10 File Offset: 0x00076F10
		public void DeleteInHandsMatching(EntityUid user, EntityUid matching)
		{
			foreach (Hand hand in this._handsSystem.EnumerateHands(user, null))
			{
				HandVirtualItemComponent virt;
				if (base.TryComp<HandVirtualItemComponent>(hand.HeldEntity, ref virt) && virt.BlockingEntity == matching)
				{
					base.Delete(virt, user);
				}
			}
		}

		// Token: 0x04000E61 RID: 3681
		[Nullable(1)]
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;
	}
}
