using System;
using System.Runtime.CompilerServices;
using Content.Client.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Maps;
using Robust.Client.Placement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Client.Mapping
{
	// Token: 0x0200024B RID: 587
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MappingSystem : EntitySystem
	{
		// Token: 0x06000ED3 RID: 3795 RVA: 0x000595C8 File Offset: 0x000577C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FillActionSlotEvent>(new EntityEventHandler<FillActionSlotEvent>(this.OnFillActionSlot), null, null);
			base.SubscribeLocalEvent<StartPlacementActionEvent>(new EntityEventHandler<StartPlacementActionEvent>(this.OnStartPlacementAction), null, null);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x000595F8 File Offset: 0x000577F8
		public void LoadMappingActions()
		{
			this._actionsSystem.LoadActionAssignments(this.DefaultMappingActions, false);
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0005960C File Offset: 0x0005780C
		private void OnFillActionSlot(FillActionSlotEvent ev)
		{
			if (!this._placementMan.IsActive)
			{
				return;
			}
			if (ev.Action != null)
			{
				return;
			}
			StartPlacementActionEvent startPlacementActionEvent = new StartPlacementActionEvent();
			ITileDefinition tileDefinition = null;
			if (this._placementMan.CurrentPermission != null)
			{
				startPlacementActionEvent.EntityType = this._placementMan.CurrentPermission.EntityType;
				startPlacementActionEvent.PlacementOption = this._placementMan.CurrentPermission.PlacementOption;
				if (this._placementMan.CurrentPermission.IsTile)
				{
					tileDefinition = this._tileMan[(int)this._placementMan.CurrentPermission.TileType];
					startPlacementActionEvent.TileId = tileDefinition.ID;
				}
			}
			else
			{
				if (!this._placementMan.Eraser)
				{
					return;
				}
				startPlacementActionEvent.Eraser = true;
			}
			if (tileDefinition != null)
			{
				ContentTileDefinition contentTileDefinition = tileDefinition as ContentTileDefinition;
				if (contentTileDefinition == null)
				{
					return;
				}
				SpriteSpecifier icon = contentTileDefinition.IsSpace ? this._spaceIcon : new SpriteSpecifier.Texture(contentTileDefinition.Sprite);
				ev.Action = new InstantAction
				{
					CheckCanInteract = false,
					Event = startPlacementActionEvent,
					DisplayName = Loc.GetString(tileDefinition.Name),
					Icon = icon
				};
				return;
			}
			else
			{
				if (startPlacementActionEvent.Eraser)
				{
					ev.Action = new InstantAction
					{
						CheckCanInteract = false,
						Event = startPlacementActionEvent,
						DisplayName = "action-name-mapping-erase",
						Icon = this._deleteIcon
					};
					return;
				}
				if (string.IsNullOrWhiteSpace(startPlacementActionEvent.EntityType))
				{
					return;
				}
				ev.Action = new InstantAction
				{
					CheckCanInteract = false,
					Event = startPlacementActionEvent,
					DisplayName = startPlacementActionEvent.EntityType,
					Icon = new SpriteSpecifier.EntityPrototype(startPlacementActionEvent.EntityType)
				};
				return;
			}
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x000597A0 File Offset: 0x000579A0
		private void OnStartPlacementAction(StartPlacementActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this._placementMan.BeginPlacing(new PlacementInformation
			{
				EntityType = args.EntityType,
				IsTile = (args.TileId != null),
				TileType = ((args.TileId != null) ? this._tileMan[args.TileId].TileId : 0),
				PlacementOption = args.PlacementOption
			}, null);
			if (this._placementMan.Eraser != args.Eraser)
			{
				this._placementMan.ToggleEraser();
			}
		}

		// Token: 0x0400075B RID: 1883
		[Dependency]
		private readonly IPlacementManager _placementMan;

		// Token: 0x0400075C RID: 1884
		[Dependency]
		private readonly ITileDefinitionManager _tileMan;

		// Token: 0x0400075D RID: 1885
		[Dependency]
		private readonly ActionsSystem _actionsSystem;

		// Token: 0x0400075E RID: 1886
		private readonly SpriteSpecifier _spaceIcon = new SpriteSpecifier.Texture(new ResourcePath("Tiles/cropped_parallax.png", "/"));

		// Token: 0x0400075F RID: 1887
		private readonly SpriteSpecifier _deleteIcon = new SpriteSpecifier.Texture(new ResourcePath("Interface/VerbIcons/delete.svg.192dpi.png", "/"));

		// Token: 0x04000760 RID: 1888
		public string DefaultMappingActions = "/mapping_actions.yml";
	}
}
