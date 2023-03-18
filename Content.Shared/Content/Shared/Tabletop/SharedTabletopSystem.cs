using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Tabletop.Components;
using Content.Shared.Tabletop.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop
{
	// Token: 0x020000ED RID: 237
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedTabletopSystem : EntitySystem
	{
		// Token: 0x060002AC RID: 684 RVA: 0x0000C894 File Offset: 0x0000AA94
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TabletopDraggableComponent, ComponentGetState>(new ComponentEventRefHandler<TabletopDraggableComponent, ComponentGetState>(this.GetDraggableState), null, null);
			base.SubscribeAllEvent<TabletopDraggingPlayerChangedEvent>(new EntitySessionEventHandler<TabletopDraggingPlayerChangedEvent>(this.OnDraggingPlayerChanged), null, null);
			base.SubscribeAllEvent<TabletopMoveEvent>(new EntitySessionEventHandler<TabletopMoveEvent>(this.OnTabletopMove), null, null);
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		protected virtual void OnTabletopMove(TabletopMoveEvent msg, EntitySessionEventArgs args)
		{
			ICommonSession playerSession = args.SenderSession;
			if (playerSession != null)
			{
				EntityUid? attachedEntity = playerSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEntity = attachedEntity.GetValueOrDefault();
					TabletopDraggableComponent tabletopDraggableComponent;
					if (!this.CanSeeTable(playerEntity, new EntityUid?(msg.TableUid)) || !this.CanDrag(playerEntity, msg.MovedEntityUid, out tabletopDraggableComponent))
					{
						return;
					}
					TransformComponent transform = this.EntityManager.GetComponent<TransformComponent>(msg.MovedEntityUid);
					this._transforms.SetParent(msg.MovedEntityUid, transform, this._mapMan.GetMapEntityId(transform.MapID), null);
					this._transforms.SetLocalPositionNoLerp(transform, msg.Coordinates.Position);
					return;
				}
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000C97A File Offset: 0x0000AB7A
		private void GetDraggableState(EntityUid uid, TabletopDraggableComponent component, ref ComponentGetState args)
		{
			args.State = new SharedTabletopSystem.TabletopDraggableComponentState(component.DraggingPlayer);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000C990 File Offset: 0x0000AB90
		private void OnDraggingPlayerChanged(TabletopDraggingPlayerChangedEvent msg, EntitySessionEventArgs args)
		{
			EntityUid dragged = msg.DraggedEntityUid;
			TabletopDraggableComponent draggableComponent;
			if (!base.TryComp<TabletopDraggableComponent>(dragged, ref draggableComponent))
			{
				return;
			}
			draggableComponent.DraggingPlayer = (msg.IsDragging ? new NetUserId?(args.SenderSession.UserId) : null);
			base.Dirty(draggableComponent, null);
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(dragged, ref appearance))
			{
				return;
			}
			if (draggableComponent.DraggingPlayer != null)
			{
				this._appearance.SetData(dragged, TabletopItemVisuals.Scale, new Vector2(1.25f, 1.25f), appearance);
				this._appearance.SetData(dragged, TabletopItemVisuals.DrawDepth, 4, appearance);
				return;
			}
			this._appearance.SetData(dragged, TabletopItemVisuals.Scale, Vector2.One, appearance);
			this._appearance.SetData(dragged, TabletopItemVisuals.DrawDepth, 3, appearance);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000CA74 File Offset: 0x0000AC74
		protected bool CanSeeTable(EntityUid playerEntity, EntityUid? table)
		{
			MetaDataComponent meta;
			return base.TryComp<MetaDataComponent>(table, ref meta) && meta.EntityLifeStage < 4 && (meta.Flags & 2) != 2 && this._interactionSystem.InRangeUnobstructed(playerEntity, table.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this.ActionBlockerSystem.CanInteract(playerEntity, table);
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000CAD4 File Offset: 0x0000ACD4
		[NullableContext(2)]
		protected bool CanDrag(EntityUid playerEntity, EntityUid target, [NotNullWhen(true)] out TabletopDraggableComponent draggable)
		{
			SharedHandsComponent hands;
			return base.TryComp<TabletopDraggableComponent>(target, ref draggable) && base.TryComp<SharedHandsComponent>(playerEntity, ref hands) && hands.Hands.Count > 0;
		}

		// Token: 0x040002F6 RID: 758
		[Dependency]
		protected readonly ActionBlockerSystem ActionBlockerSystem;

		// Token: 0x040002F7 RID: 759
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040002F8 RID: 760
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040002F9 RID: 761
		[Dependency]
		private readonly SharedTransformSystem _transforms;

		// Token: 0x040002FA RID: 762
		[Dependency]
		private readonly IMapManager _mapMan;

		// Token: 0x02000796 RID: 1942
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class TabletopDraggableComponentState : ComponentState
		{
			// Token: 0x060017CB RID: 6091 RVA: 0x0004D1A8 File Offset: 0x0004B3A8
			public TabletopDraggableComponentState(NetUserId? draggingPlayer)
			{
				this.DraggingPlayer = draggingPlayer;
			}

			// Token: 0x040017A7 RID: 6055
			public NetUserId? DraggingPlayer;
		}
	}
}
