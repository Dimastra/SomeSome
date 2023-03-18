using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Hands.Components;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Sandbox;
using Robust.Server.Console;
using Robust.Server.Placement;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;
using Robust.Shared.ViewVariables;

namespace Content.Server.Sandbox
{
	// Token: 0x02000216 RID: 534
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SandboxSystem : SharedSandboxSystem
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0003771A File Offset: 0x0003591A
		// (set) Token: 0x06000A94 RID: 2708 RVA: 0x00037722 File Offset: 0x00035922
		[ViewVariables]
		public bool IsSandboxEnabled
		{
			get
			{
				return this._isSandboxEnabled;
			}
			set
			{
				this._isSandboxEnabled = value;
				this.UpdateSandboxStatusForAll();
			}
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00037734 File Offset: 0x00035934
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxRespawn>(new EntitySessionEventHandler<SharedSandboxSystem.MsgSandboxRespawn>(this.SandboxRespawnReceived), null, null);
			base.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxGiveAccess>(new EntitySessionEventHandler<SharedSandboxSystem.MsgSandboxGiveAccess>(this.SandboxGiveAccessReceived), null, null);
			base.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxGiveAghost>(new EntitySessionEventHandler<SharedSandboxSystem.MsgSandboxGiveAghost>(this.SandboxGiveAghostReceived), null, null);
			base.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxSuicide>(new EntitySessionEventHandler<SharedSandboxSystem.MsgSandboxSuicide>(this.SandboxSuicideReceived), null, null);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.GameTickerOnOnRunLevelChanged), null, null);
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			this._placementManager.AllowPlacementFunc = delegate(MsgPlacement placement)
			{
				EntityUid playerUid = this._playerManager.GetSessionByUserId(placement.MsgChannel.UserId).AttachedEntity.GetValueOrDefault();
				EntityCoordinates coordinates = placement.EntityCoordinates;
				LogStringHandler logStringHandler;
				switch (placement.PlaceType)
				{
				case 3:
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.EntitySpawn;
					LogImpact impact = LogImpact.High;
					logStringHandler = new LogStringHandler(27, 5);
					logStringHandler.AppendFormatted(placement.EntityTemplateName);
					logStringHandler.AppendLiteral(" was spawned by");
					logStringHandler.AppendLiteral(" ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(playerUid), "player", "ToPrettyString(playerUid)");
					logStringHandler.AppendLiteral(" at ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(coordinates.EntityId), "entity", "ToPrettyString(coordinates.EntityId)");
					logStringHandler.AppendLiteral(" X=");
					logStringHandler.AppendFormatted<float>(coordinates.X, "coordinates.X");
					logStringHandler.AppendLiteral(", Y=");
					logStringHandler.AppendFormatted<float>(coordinates.Y, "coordinates.Y");
					adminLogger.Add(type, impact, ref logStringHandler);
					break;
				}
				case 4:
				{
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.EntitySpawn;
					LogImpact impact2 = LogImpact.High;
					logStringHandler = new LogStringHandler(16, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(placement.EntityUid), "entity", "ToPrettyString(placement.EntityUid)");
					logStringHandler.AppendLiteral(" was deleted by ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(playerUid), "player", "ToPrettyString(playerUid)");
					adminLogger2.Add(type2, impact2, ref logStringHandler);
					break;
				}
				}
				ISharedAdminLogManager adminLogger3 = this._adminLogger;
				LogType type3 = LogType.EntitySpawn;
				logStringHandler = new LogStringHandler(8, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(placement.EntityUid), "entity", "ToPrettyString(placement.EntityUid)");
				logStringHandler.AppendLiteral(" spawned");
				adminLogger3.Add(type3, ref logStringHandler);
				if (this.IsSandboxEnabled)
				{
					return true;
				}
				INetChannel channel = placement.MsgChannel;
				IPlayerSession player = this._playerManager.GetSessionByChannel(channel);
				return this._conGroupController.CanAdminPlace(player);
			};
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x000377D9 File Offset: 0x000359D9
		public override void Shutdown()
		{
			base.Shutdown();
			this._placementManager.AllowPlacementFunc = null;
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00037804 File Offset: 0x00035A04
		private void GameTickerOnOnRunLevelChanged(GameRunLevelChangedEvent obj)
		{
			if (obj.New == GameRunLevel.PreRoundLobby)
			{
				this.IsSandboxEnabled = false;
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00037815 File Offset: 0x00035A15
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus != 2 || e.OldStatus != 1)
			{
				return;
			}
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxStatus
			{
				SandboxAllowed = this.IsSandboxEnabled
			}, e.Session.ConnectedClient);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0003784C File Offset: 0x00035A4C
		private void SandboxRespawnReceived(SharedSandboxSystem.MsgSandboxRespawn message, EntitySessionEventArgs args)
		{
			if (!this.IsSandboxEnabled)
			{
				return;
			}
			IPlayerSession player = this._playerManager.GetSessionByChannel(args.SenderSession.ConnectedClient);
			if (player.AttachedEntity == null)
			{
				return;
			}
			this._ticker.Respawn(player);
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00037898 File Offset: 0x00035A98
		private void SandboxGiveAccessReceived(SharedSandboxSystem.MsgSandboxGiveAccess message, EntitySessionEventArgs args)
		{
			SandboxSystem.<>c__DisplayClass19_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			if (!this.IsSandboxEnabled)
			{
				return;
			}
			EntityUid? attachedEntity = this._playerManager.GetSessionByChannel(args.SenderSession.ConnectedClient).AttachedEntity;
			if (attachedEntity != null)
			{
				CS$<>8__locals1.attached = attachedEntity.GetValueOrDefault();
				CS$<>8__locals1.allAccess = (from p in this.PrototypeManager.EnumeratePrototypes<AccessLevelPrototype>()
				select p.ID).ToArray<string>();
				EntityUid? slotEntity;
				HandsComponent hands;
				if (this._inventory.TryGetSlotEntity(CS$<>8__locals1.attached, "id", out slotEntity, null, null))
				{
					if (base.HasComp<AccessComponent>(slotEntity))
					{
						this.<SandboxGiveAccessReceived>g__UpgradeId|19_1(slotEntity.Value, ref CS$<>8__locals1);
						return;
					}
					PDAComponent pda;
					if (base.TryComp<PDAComponent>(slotEntity, ref pda))
					{
						if (pda.ContainedID != null)
						{
							this.<SandboxGiveAccessReceived>g__UpgradeId|19_1(pda.ContainedID.Owner, ref CS$<>8__locals1);
							return;
						}
						EntityUid newID = this.<SandboxGiveAccessReceived>g__CreateFreshId|19_2(ref CS$<>8__locals1);
						ItemSlotsComponent itemSlots;
						if (base.TryComp<ItemSlotsComponent>(pda.Owner, ref itemSlots))
						{
							this._slots.TryInsert(slotEntity.Value, pda.IdSlot, newID, null);
							return;
						}
					}
				}
				else if (base.TryComp<HandsComponent>(CS$<>8__locals1.attached, ref hands))
				{
					EntityUid card = this.<SandboxGiveAccessReceived>g__CreateFreshId|19_2(ref CS$<>8__locals1);
					if (!this._inventory.TryEquip(CS$<>8__locals1.attached, card, "id", true, true, false, null, null))
					{
						this._handsSystem.PickupOrDrop(new EntityUid?(CS$<>8__locals1.attached), card, true, false, hands, null);
					}
				}
				return;
			}
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00037A24 File Offset: 0x00035C24
		private void SandboxGiveAghostReceived(SharedSandboxSystem.MsgSandboxGiveAghost message, EntitySessionEventArgs args)
		{
			if (!this.IsSandboxEnabled)
			{
				return;
			}
			IPlayerSession player = this._playerManager.GetSessionByChannel(args.SenderSession.ConnectedClient);
			this._host.ExecuteCommand(player, this._conGroupController.CanCommand(player, "aghost") ? "aghost" : "ghost");
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00037A80 File Offset: 0x00035C80
		private void SandboxSuicideReceived(SharedSandboxSystem.MsgSandboxSuicide message, EntitySessionEventArgs args)
		{
			if (!this.IsSandboxEnabled)
			{
				return;
			}
			IPlayerSession player = this._playerManager.GetSessionByChannel(args.SenderSession.ConnectedClient);
			this._host.ExecuteCommand(player, "suicide");
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00037ABF File Offset: 0x00035CBF
		private void UpdateSandboxStatusForAll()
		{
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxStatus
			{
				SandboxAllowed = this.IsSandboxEnabled
			});
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00037CD0 File Offset: 0x00035ED0
		[CompilerGenerated]
		private void <SandboxGiveAccessReceived>g__UpgradeId|19_1(EntityUid id, ref SandboxSystem.<>c__DisplayClass19_0 A_2)
		{
			this._access.TrySetTags(id, A_2.allAccess, null);
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00037CE8 File Offset: 0x00035EE8
		[CompilerGenerated]
		private EntityUid <SandboxGiveAccessReceived>g__CreateFreshId|19_2(ref SandboxSystem.<>c__DisplayClass19_0 A_1)
		{
			EntityUid card = base.Spawn("CaptainIDCard", base.Transform(A_1.attached).Coordinates);
			this.<SandboxGiveAccessReceived>g__UpgradeId|19_1(card, ref A_1);
			base.Comp<IdCardComponent>(card).FullName = base.MetaData(A_1.attached).EntityName;
			return card;
		}

		// Token: 0x04000677 RID: 1655
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000678 RID: 1656
		[Dependency]
		private readonly IPlacementManager _placementManager;

		// Token: 0x04000679 RID: 1657
		[Dependency]
		private readonly IConGroupController _conGroupController;

		// Token: 0x0400067A RID: 1658
		[Dependency]
		private readonly IServerConsoleHost _host;

		// Token: 0x0400067B RID: 1659
		[Dependency]
		private readonly SharedAccessSystem _access;

		// Token: 0x0400067C RID: 1660
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x0400067D RID: 1661
		[Dependency]
		private readonly ItemSlotsSystem _slots;

		// Token: 0x0400067E RID: 1662
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x0400067F RID: 1663
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000680 RID: 1664
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000681 RID: 1665
		private bool _isSandboxEnabled;
	}
}
