using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.UI;
using Content.Server.EUI;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000861 RID: 2145
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class SetOutfitCommand : IConsoleCommand
	{
		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06002EE7 RID: 12007 RVA: 0x000F3394 File Offset: 0x000F1594
		public string Command
		{
			get
			{
				return "setoutfit";
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06002EE8 RID: 12008 RVA: 0x000F339B File Offset: 0x000F159B
		public string Description
		{
			get
			{
				return Loc.GetString("set-outfit-command-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("requiredComponent", "InventoryComponent")
				});
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06002EE9 RID: 12009 RVA: 0x000F33C3 File Offset: 0x000F15C3
		public string Help
		{
			get
			{
				return Loc.GetString("set-outfit-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002EEA RID: 12010 RVA: 0x000F33EC File Offset: 0x000F15EC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			int entityUid;
			if (!int.TryParse(args[0], out entityUid))
			{
				shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			EntityUid target;
			target..ctor(entityUid);
			if (!target.IsValid() || !this._entities.EntityExists(target))
			{
				shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
				return;
			}
			if (!this._entities.HasComponent<InventoryComponent>(target))
			{
				shell.WriteLine(Loc.GetString("shell-target-entity-does-not-have-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("missing", "inventory")
				}));
				return;
			}
			if (args.Length != 1)
			{
				if (!SetOutfitCommand.SetOutfit(target, args[1], this._entities, null))
				{
					shell.WriteLine(Loc.GetString("set-outfit-command-invalid-outfit-id-error"));
				}
				return;
			}
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError(Loc.GetString("set-outfit-command-is-not-player-error"));
				return;
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			SetOutfitEui ui = new SetOutfitEui(target);
			euiManager.OpenEui(ui, player);
		}

		// Token: 0x06002EEB RID: 12011 RVA: 0x000F34F4 File Offset: 0x000F16F4
		public static bool SetOutfit(EntityUid target, string gear, IEntityManager entityManager, [Nullable(2)] Action<EntityUid, EntityUid> onEquipped = null)
		{
			InventoryComponent inventoryComponent;
			if (!entityManager.TryGetComponent<InventoryComponent>(target, ref inventoryComponent))
			{
				return false;
			}
			StartingGearPrototype startingGear;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<StartingGearPrototype>(gear, ref startingGear))
			{
				return false;
			}
			HumanoidCharacterProfile profile = null;
			ActorComponent actorComponent;
			if (entityManager.TryGetComponent<ActorComponent>(target, ref actorComponent))
			{
				NetUserId userId = actorComponent.PlayerSession.UserId;
				profile = (IoCManager.Resolve<IServerPreferencesManager>().GetPreferences(userId).SelectedCharacter as HumanoidCharacterProfile);
			}
			InventorySystem invSystem = entityManager.System<InventorySystem>();
			SlotDefinition[] slotDefinitions;
			if (invSystem.TryGetSlots(target, out slotDefinitions, inventoryComponent))
			{
				foreach (SlotDefinition slot in slotDefinitions)
				{
					invSystem.TryUnequip(target, slot.Name, true, true, false, inventoryComponent, null);
					string gearStr = startingGear.GetGear(slot.Name, profile);
					if (!(gearStr == string.Empty))
					{
						EntityUid equipmentEntity = entityManager.SpawnEntity(gearStr, entityManager.GetComponent<TransformComponent>(target).Coordinates);
						PDAComponent pdaComponent;
						if (slot.Name == "id" && entityManager.TryGetComponent<PDAComponent>(equipmentEntity, ref pdaComponent) && pdaComponent.ContainedID != null)
						{
							pdaComponent.ContainedID.FullName = entityManager.GetComponent<MetaDataComponent>(target).EntityName;
						}
						invSystem.TryEquip(target, equipmentEntity, slot.Name, true, false, false, inventoryComponent, null);
						if (onEquipped != null)
						{
							onEquipped(target, equipmentEntity);
						}
					}
				}
			}
			HandsComponent handsComponent;
			if (entityManager.TryGetComponent<HandsComponent>(target, ref handsComponent))
			{
				HandsSystem handsSystem = entityManager.System<HandsSystem>();
				EntityCoordinates coords = entityManager.GetComponent<TransformComponent>(target).Coordinates;
				foreach (KeyValuePair<string, string> keyValuePair in startingGear.Inhand)
				{
					string text;
					string text2;
					keyValuePair.Deconstruct(out text, out text2);
					string hand = text;
					string prototype = text2;
					EntityUid inhandEntity = entityManager.SpawnEntity(prototype, coords);
					handsSystem.TryPickup(target, inhandEntity, hand, false, false, handsComponent, null);
				}
			}
			return true;
		}

		// Token: 0x04001C60 RID: 7264
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04001C61 RID: 7265
		[Dependency]
		private readonly IPrototypeManager _prototypes;
	}
}
