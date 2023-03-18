using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.CardboardBox;
using Content.Shared.CardboardBox.Components;
using Content.Shared.Examine;
using Content.Shared.Movement.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.CardboardBox
{
	// Token: 0x0200040F RID: 1039
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CardboardBoxSystem : SharedCardboardBoxSystem
	{
		// Token: 0x060019A0 RID: 6560 RVA: 0x000931C1 File Offset: 0x000913C1
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<PlayBoxEffectMessage>(new EntityEventHandler<PlayBoxEffectMessage>(this.OnBoxEffect), null, null);
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x000931E0 File Offset: 0x000913E0
		private void OnBoxEffect(PlayBoxEffectMessage msg)
		{
			CardboardBoxComponent cardboardBoxComponent;
			if (!base.TryComp<CardboardBoxComponent>(msg.Source, ref cardboardBoxComponent))
			{
				return;
			}
			EntityQuery<TransformComponent> entityQuery = base.GetEntityQuery<TransformComponent>();
			TransformComponent transformComponent;
			if (!entityQuery.TryGetComponent(msg.Source, ref transformComponent))
			{
				return;
			}
			MapCoordinates mapPosition = transformComponent.MapPosition;
			HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
			foreach (MobMoverComponent mobMoverComponent in this._entityLookup.GetComponentsInRange<MobMoverComponent>(transformComponent.Coordinates, cardboardBoxComponent.Distance))
			{
				if (!(mobMoverComponent.Owner == msg.Mover))
				{
					hashSet.Add(mobMoverComponent.Owner);
				}
			}
			foreach (EntityUid entityUid in hashSet)
			{
				TransformComponent transformComponent2;
				if (entityQuery.TryGetComponent(entityUid, ref transformComponent2) && ExamineSystemShared.InRangeUnOccluded(mapPosition, transformComponent2.MapPosition, cardboardBoxComponent.Distance, null, true, null))
				{
					EntityUid entityUid2 = base.Spawn(cardboardBoxComponent.Effect, transformComponent2.MapPosition);
					TransformComponent transformComponent3;
					SpriteComponent spriteComponent;
					if (entityQuery.TryGetComponent(entityUid2, ref transformComponent3) && base.TryComp<SpriteComponent>(entityUid2, ref spriteComponent))
					{
						spriteComponent.Offset = new Vector2(0f, 1f);
						transformComponent3.AttachParent(entityUid);
					}
				}
			}
		}

		// Token: 0x04000CFF RID: 3327
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;
	}
}
