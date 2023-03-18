using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Content.Client.Actions;
using Content.Client.Construction;
using Content.Client.DragDrop;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Outline;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Actions.Controls;
using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Actions.Windows;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Players;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Actions
{
	// Token: 0x020000BD RID: 189
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActionUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<ActionsSystem>, IOnSystemLoaded<ActionsSystem>, IOnSystemUnloaded<ActionsSystem>
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000526 RID: 1318 RVA: 0x0001C60B File Offset: 0x0001A80B
		[Nullable(2)]
		private ActionsBar ActionsBar
		{
			[NullableContext(2)]
			get
			{
				return this.UIManager.GetActiveUIWidgetOrNull<ActionsBar>();
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001C618 File Offset: 0x0001A818
		[Nullable(2)]
		private MenuButton ActionButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.ActionButton;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x0001C630 File Offset: 0x0001A830
		private ActionUIController.ActionPage CurrentPage
		{
			get
			{
				return this._pages[this._currentPageIndex];
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x0001C643 File Offset: 0x0001A843
		public bool IsDragging
		{
			get
			{
				return this._menuDragHelper.IsDragging;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x0001C650 File Offset: 0x0001A850
		// (set) Token: 0x0600052B RID: 1323 RVA: 0x0001C658 File Offset: 0x0001A858
		[Nullable(2)]
		public TargetedAction SelectingTargetFor { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x0600052C RID: 1324 RVA: 0x0001C664 File Offset: 0x0001A864
		public ActionUIController()
		{
			this._menuDragHelper = new DragDropHelper<ActionButton>(new OnBeginDrag(this.OnMenuBeginDrag), new OnContinueDrag(this.OnMenuContinueDrag), new OnEndDrag(this.OnMenuEndDrag));
			this._dragShadow = new TextureRect
			{
				MinSize = new ValueTuple<float, float>(64f, 64f),
				Stretch = 1,
				Visible = false,
				SetSize = new ValueTuple<float, float>(64f, 64f),
				MouseFilter = 2
			};
			int num = ContentKeyFunctions.GetLoadoutBoundKeys().Length;
			int size = ContentKeyFunctions.GetHotbarBoundKeys().Length;
			for (int i = 0; i < num; i++)
			{
				ActionUIController.ActionPage item = new ActionUIController.ActionPage(size);
				this._pages.Add(item);
			}
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001C734 File Offset: 0x0001A934
		public void OnStateEntered(GameplayState state)
		{
			this._window = this.UIManager.CreateWindow<ActionsWindow>();
			LayoutContainer.SetAnchorPreset(this._window, 5, false);
			this._window.OnOpen += this.OnWindowOpened;
			this._window.OnClose += this.OnWindowClosed;
			this._window.ClearButton.OnPressed += this.OnClearPressed;
			this._window.SearchBar.OnTextChanged += this.OnSearchChanged;
			this._window.FilterButton.OnItemSelected += this.OnFilterSelected;
			if (this._actionsSystem != null)
			{
				this._actionsSystem.ActionAdded += this.OnActionAdded;
				this._actionsSystem.ActionRemoved += this.OnActionRemoved;
				this._actionsSystem.ActionReplaced += this.OnActionReplaced;
				this._actionsSystem.ActionsUpdated += this.OnActionsUpdated;
			}
			this.UpdateFilterLabel();
			this.SearchAndDisplay(null);
			this._dragShadow.Orphan();
			this.UIManager.PopupRoot.AddChild(this._dragShadow);
			CommandBinds.BindingsBuilder bindingsBuilder = CommandBinds.Builder;
			BoundKeyFunction[] hotbarBoundKeys = ContentKeyFunctions.GetHotbarBoundKeys();
			for (int i = 0; i < hotbarBoundKeys.Length; i++)
			{
				int boundId = i;
				BoundKeyFunction boundKeyFunction = hotbarBoundKeys[i];
				bindingsBuilder = bindingsBuilder.Bind(boundKeyFunction, new PointerInputCmdHandler(delegate(in PointerInputCmdHandler.PointerInputCmdArgs args)
				{
					if (args.State != null)
					{
						return false;
					}
					this.TriggerAction(boundId);
					return true;
				}, false, false));
			}
			BoundKeyFunction[] loadoutBoundKeys = ContentKeyFunctions.GetLoadoutBoundKeys();
			for (int j = 0; j < loadoutBoundKeys.Length; j++)
			{
				int boundId = j;
				BoundKeyFunction boundKeyFunction2 = loadoutBoundKeys[j];
				bindingsBuilder = bindingsBuilder.Bind(boundKeyFunction2, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
				{
					this.ChangePage(boundId);
				}, null, true, true));
			}
			bindingsBuilder.Bind(ContentKeyFunctions.OpenActionsMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true)).BindBefore(EngineKeyFunctions.Use, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.TargetingOnUse), true, true), new Type[]
			{
				typeof(ConstructionSystem),
				typeof(DragDropSystem)
			}).BindBefore(EngineKeyFunctions.UIRightClick, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.TargetingCancel), true, true), Array.Empty<Type>()).Register<ActionUIController>();
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001C9A6 File Offset: 0x0001ABA6
		private bool TargetingCancel(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (!this._timing.IsFirstTimePredicted)
			{
				return false;
			}
			if (this.SelectingTargetFor == null)
			{
				return false;
			}
			this.StopTargeting();
			return true;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001C9C8 File Offset: 0x0001ABC8
		private bool TargetingOnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (this._timing.IsFirstTimePredicted && this._actionsSystem != null)
			{
				TargetedAction selectingTargetFor = this.SelectingTargetFor;
				if (selectingTargetFor != null)
				{
					LocalPlayer localPlayer = this._playerManager.LocalPlayer;
					EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
					if (entityUid == null)
					{
						return false;
					}
					EntityUid valueOrDefault = entityUid.GetValueOrDefault();
					ActionsComponent actionComp;
					if (!this._entities.TryGetComponent<ActionsComponent>(valueOrDefault, ref actionComp))
					{
						return false;
					}
					if (selectingTargetFor.Enabled)
					{
						if (selectingTargetFor.Charges != null)
						{
							int? charges = selectingTargetFor.Charges;
							int num = 0;
							if (charges.GetValueOrDefault() == num & charges != null)
							{
								goto IL_CD;
							}
						}
						if (selectingTargetFor.Cooldown == null || !(selectingTargetFor.Cooldown.Value.Item2 > this._timing.CurTime))
						{
							WorldTargetAction worldTargetAction = selectingTargetFor as WorldTargetAction;
							if (worldTargetAction != null)
							{
								return this.TryTargetWorld(args, worldTargetAction, valueOrDefault, actionComp) || !selectingTargetFor.InteractOnMiss;
							}
							EntityTargetAction entityTargetAction = selectingTargetFor as EntityTargetAction;
							if (entityTargetAction == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
								defaultInterpolatedStringHandler.AppendLiteral("Unknown targeting action: ");
								defaultInterpolatedStringHandler.AppendFormatted<Type>(selectingTargetFor.GetType());
								Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								return false;
							}
							return this.TryTargetEntity(args, entityTargetAction, valueOrDefault, actionComp) || !selectingTargetFor.InteractOnMiss;
						}
					}
					IL_CD:
					return !selectingTargetFor.InteractOnMiss;
				}
			}
			return false;
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001CB28 File Offset: 0x0001AD28
		private bool TryTargetWorld(in PointerInputCmdHandler.PointerInputCmdArgs args, WorldTargetAction action, EntityUid user, ActionsComponent actionComp)
		{
			if (this._actionsSystem == null)
			{
				return false;
			}
			EntityCoordinates coordinates = args.Coordinates;
			if (!this._actionsSystem.ValidateWorldTarget(user, coordinates, action))
			{
				if (action.DeselectOnMiss)
				{
					this.StopTargeting();
				}
				return false;
			}
			if (action.ClientExclusive)
			{
				if (action.Event != null)
				{
					action.Event.Target = coordinates;
					action.Event.Performer = user;
				}
				this._actionsSystem.PerformAction(user, actionComp, action, action.Event, this._timing.CurTime, true);
			}
			else
			{
				this._entities.RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(action, coordinates));
			}
			if (!action.Repeat)
			{
				this.StopTargeting();
			}
			return true;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001CBD4 File Offset: 0x0001ADD4
		private bool TryTargetEntity(in PointerInputCmdHandler.PointerInputCmdArgs args, EntityTargetAction action, EntityUid user, ActionsComponent actionComp)
		{
			if (this._actionsSystem == null)
			{
				return false;
			}
			if (!this._actionsSystem.ValidateEntityTarget(user, args.EntityUid, action))
			{
				if (action.DeselectOnMiss)
				{
					this.StopTargeting();
				}
				return false;
			}
			if (action.ClientExclusive)
			{
				if (action.Event != null)
				{
					action.Event.Target = args.EntityUid;
					action.Event.Performer = user;
				}
				this._actionsSystem.PerformAction(user, actionComp, action, action.Event, this._timing.CurTime, true);
			}
			else
			{
				this._entities.RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(action, args.EntityUid));
			}
			if (!action.Repeat)
			{
				this.StopTargeting();
			}
			return true;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001CC86 File Offset: 0x0001AE86
		public void UnloadButton()
		{
			if (this.ActionButton == null)
			{
				return;
			}
			this.ActionButton.OnPressed -= this.ActionButtonPressed;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001CCA8 File Offset: 0x0001AEA8
		public void LoadButton()
		{
			if (this.ActionButton == null)
			{
				return;
			}
			this.ActionButton.OnPressed += this.ActionButtonPressed;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001CCCA File Offset: 0x0001AECA
		private void OnWindowOpened()
		{
			if (this.ActionButton != null)
			{
				this.ActionButton.Pressed = true;
			}
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001CCE0 File Offset: 0x0001AEE0
		private void OnWindowClosed()
		{
			if (this.ActionButton != null)
			{
				this.ActionButton.Pressed = false;
			}
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001CCF8 File Offset: 0x0001AEF8
		public void OnStateExited(GameplayState state)
		{
			if (this._actionsSystem != null)
			{
				this._actionsSystem.ActionAdded -= this.OnActionAdded;
				this._actionsSystem.ActionRemoved -= this.OnActionRemoved;
				this._actionsSystem.ActionReplaced -= this.OnActionReplaced;
				this._actionsSystem.ActionsUpdated -= this.OnActionsUpdated;
			}
			if (this._window != null)
			{
				this._window.OnOpen -= this.OnWindowOpened;
				this._window.OnClose -= this.OnWindowClosed;
				this._window.ClearButton.OnPressed -= this.OnClearPressed;
				this._window.SearchBar.OnTextChanged -= this.OnSearchChanged;
				this._window.FilterButton.OnItemSelected -= this.OnFilterSelected;
				this._window.Dispose();
				this._window = null;
			}
			CommandBinds.Unregister<ActionUIController>();
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x0001CE10 File Offset: 0x0001B010
		private void TriggerAction(int index)
		{
			ActionType actionType = this.CurrentPage[index];
			if (actionType == null)
			{
				return;
			}
			TargetedAction targetedAction = actionType as TargetedAction;
			if (targetedAction != null)
			{
				this.ToggleTargeting(targetedAction);
				return;
			}
			ActionsSystem actionsSystem = this._actionsSystem;
			if (actionsSystem == null)
			{
				return;
			}
			actionsSystem.TriggerAction(actionType);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001CE54 File Offset: 0x0001B054
		private void ChangePage(int index)
		{
			int num = this._pages.Count - 1;
			if (index < 0)
			{
				index = num;
			}
			else if (index > num)
			{
				index = 0;
			}
			this._currentPageIndex = index;
			ActionUIController.ActionPage p = this._pages[this._currentPageIndex];
			ActionButtonContainer container = this._container;
			if (container != null)
			{
				container.SetActionData(p);
			}
			Label label = this.ActionsBar.PageButtons.Label;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._currentPageIndex + 1);
			label.Text = defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001CEE3 File Offset: 0x0001B0E3
		private void OnLeftArrowPressed(BaseButton.ButtonEventArgs args)
		{
			this.ChangePage(this._currentPageIndex - 1);
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001CEF3 File Offset: 0x0001B0F3
		private void OnRightArrowPressed(BaseButton.ButtonEventArgs args)
		{
			this.ChangePage(this._currentPageIndex + 1);
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001CF04 File Offset: 0x0001B104
		private void AppendAction(ActionType action)
		{
			if (this._container == null)
			{
				return;
			}
			foreach (ActionButton actionButton in this._container.GetButtons())
			{
				if (actionButton.Action == null)
				{
					this.SetAction(actionButton, action);
					return;
				}
			}
			foreach (ActionUIController.ActionPage actionPage in this._pages)
			{
				for (int i = 0; i < actionPage.Size; i++)
				{
					if (actionPage[i] == null)
					{
						actionPage[i] = action;
						return;
					}
				}
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001CFD0 File Offset: 0x0001B1D0
		private void OnActionAdded(ActionType action)
		{
			foreach (ActionUIController.ActionPage actionPage in this._pages)
			{
				for (int i = 0; i < actionPage.Size; i++)
				{
					if (actionPage[i] == action)
					{
						return;
					}
				}
			}
			this.AppendAction(action);
			this.SearchAndDisplay(null);
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001D048 File Offset: 0x0001B248
		private void OnActionRemoved(ActionType action)
		{
			if (this._container == null)
			{
				return;
			}
			foreach (ActionButton actionButton in this._container.GetButtons())
			{
				if (actionButton.Action == action)
				{
					this.SetAction(actionButton, null);
				}
			}
			foreach (ActionUIController.ActionPage actionPage in this._pages)
			{
				for (int i = 0; i < actionPage.Size; i++)
				{
					if (actionPage[i] == action)
					{
						actionPage[i] = null;
					}
				}
			}
			this.SearchAndDisplay(null);
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001D118 File Offset: 0x0001B318
		private void OnActionReplaced(ActionType existing, ActionType action)
		{
			if (this._container == null)
			{
				return;
			}
			foreach (ActionButton actionButton in this._container.GetButtons())
			{
				if (actionButton.Action == existing)
				{
					actionButton.UpdateData(action);
				}
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001D17C File Offset: 0x0001B37C
		private void OnActionsUpdated()
		{
			if (this._container == null)
			{
				return;
			}
			foreach (ActionButton actionButton in this._container.GetButtons())
			{
				actionButton.UpdateIcons();
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001D1D4 File Offset: 0x0001B3D4
		private void ActionButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.ToggleWindow();
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001D1DC File Offset: 0x0001B3DC
		private void ToggleWindow()
		{
			if (this._window == null)
			{
				return;
			}
			if (this._window.IsOpen)
			{
				this._window.Close();
				return;
			}
			this._window.Open();
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001D20C File Offset: 0x0001B40C
		private void UpdateFilterLabel()
		{
			if (this._window == null)
			{
				return;
			}
			if (this._window.FilterButton.SelectedKeys.Count == 0)
			{
				this._window.FilterLabel.Visible = false;
				return;
			}
			this._window.FilterLabel.Visible = true;
			this._window.FilterLabel.Text = Loc.GetString("ui-actionmenu-filter-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("selectedLabels", string.Join(", ", this._window.FilterButton.SelectedLabels))
			});
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001D2A8 File Offset: 0x0001B4A8
		private bool MatchesFilter(ActionType action, ActionsWindow.Filters filter)
		{
			bool result;
			switch (filter)
			{
			case ActionsWindow.Filters.Enabled:
				result = action.Enabled;
				break;
			case ActionsWindow.Filters.Item:
			{
				bool flag;
				if (action.Provider != null)
				{
					EntityUid? provider = action.Provider;
					ActionsSystem actionsSystem = this._actionsSystem;
					EntityUid? entityUid;
					if (actionsSystem == null)
					{
						entityUid = null;
					}
					else
					{
						ActionsComponent playerActions = actionsSystem.PlayerActions;
						entityUid = ((playerActions != null) ? new EntityUid?(playerActions.Owner) : null);
					}
					flag = (provider != entityUid);
				}
				else
				{
					flag = false;
				}
				result = flag;
				break;
			}
			case ActionsWindow.Filters.Innate:
			{
				bool flag2;
				if (action.Provider != null)
				{
					EntityUid? provider2 = action.Provider;
					ActionsSystem actionsSystem2 = this._actionsSystem;
					EntityUid? entityUid2;
					if (actionsSystem2 == null)
					{
						entityUid2 = null;
					}
					else
					{
						ActionsComponent playerActions2 = actionsSystem2.PlayerActions;
						entityUid2 = ((playerActions2 != null) ? new EntityUid?(playerActions2.Owner) : null);
					}
					flag2 = (provider2 == entityUid2);
				}
				else
				{
					flag2 = true;
				}
				result = flag2;
				break;
			}
			case ActionsWindow.Filters.Instant:
				result = (action is InstantAction);
				break;
			case ActionsWindow.Filters.Targeted:
				result = (action is TargetedAction);
				break;
			default:
				throw new ArgumentOutOfRangeException("filter", filter, null);
			}
			return result;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001D40E File Offset: 0x0001B60E
		private void ClearList()
		{
			ActionsWindow window = this._window;
			if (window != null && !window.Disposed)
			{
				this._window.ResultsGrid.RemoveAllChildren();
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001D438 File Offset: 0x0001B638
		private void PopulateActions(IEnumerable<ActionType> actions)
		{
			if (this._window == null)
			{
				return;
			}
			this.ClearList();
			foreach (ActionType action in actions)
			{
				ActionButton actionButton = new ActionButton
				{
					Locked = true
				};
				actionButton.UpdateData(action);
				actionButton.ActionPressed += this.OnWindowActionPressed;
				actionButton.ActionUnpressed += this.OnWindowActionUnPressed;
				actionButton.ActionFocusExited += this.OnWindowActionFocusExisted;
				this._window.ResultsGrid.AddChild(actionButton);
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001D4E4 File Offset: 0x0001B6E4
		[NullableContext(2)]
		private void SearchAndDisplay(ActionsComponent component = null)
		{
			if (this._window == null)
			{
				return;
			}
			string search = this._window.SearchBar.Text;
			IReadOnlyList<ActionsWindow.Filters> filters = this._window.FilterButton.SelectedKeys;
			object obj = component;
			if (component == null)
			{
				ActionsSystem actionsSystem = this._actionsSystem;
				obj = ((actionsSystem != null) ? actionsSystem.PlayerActions : null);
			}
			object obj2 = obj;
			IEnumerable<ActionType> enumerable = (obj2 != null) ? obj2.Actions : null;
			if (enumerable == null)
			{
				enumerable = Array.Empty<ActionType>();
			}
			if (filters.Count == 0 && string.IsNullOrWhiteSpace(search))
			{
				this.PopulateActions(enumerable);
				return;
			}
			Func<string, bool> <>9__2;
			enumerable = enumerable.Where(delegate(ActionType action)
			{
				if (filters.Count > 0 && filters.Any((ActionsWindow.Filters filter) => !this.MatchesFilter(action, filter)))
				{
					return false;
				}
				IEnumerable<string> keywords = action.Keywords;
				Func<string, bool> predicate;
				if ((predicate = <>9__2) == null)
				{
					predicate = (<>9__2 = ((string keyword) => search.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
				}
				if (keywords.Any(predicate))
				{
					return true;
				}
				if (action.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (action.Provider != null)
				{
					EntityUid? provider = action.Provider;
					ActionsSystem actionsSystem2 = this._actionsSystem;
					EntityUid? entityUid;
					if (actionsSystem2 == null)
					{
						entityUid = null;
					}
					else
					{
						ActionsComponent playerActions = actionsSystem2.PlayerActions;
						entityUid = ((playerActions != null) ? new EntityUid?(playerActions.Owner) : null);
					}
					if (!(provider == entityUid))
					{
						return this._entities.GetComponent<MetaDataComponent>(action.Provider.Value).EntityName.Contains(search, StringComparison.OrdinalIgnoreCase);
					}
				}
				return false;
			});
			this.PopulateActions(enumerable);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001D59C File Offset: 0x0001B79C
		private void SetAction(ActionButton button, [Nullable(2)] ActionType type)
		{
			int index;
			if (type == null)
			{
				button.ClearData();
				ActionButtonContainer container = this._container;
				if (container != null && container.TryGetButtonIndex(button, out index))
				{
					this.CurrentPage[index] = type;
				}
				return;
			}
			if (button.TryReplaceWith(type) && this._container != null && this._container.TryGetButtonIndex(button, out index))
			{
				this.CurrentPage[index] = type;
			}
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001D608 File Offset: 0x0001B808
		private void DragAction()
		{
			ActionButton actionButton = this.UIManager.CurrentlyHovered as ActionButton;
			if (actionButton != null)
			{
				if (this._menuDragHelper.IsDragging)
				{
					ActionButton dragged = this._menuDragHelper.Dragged;
					ActionType actionType = (dragged != null) ? dragged.Action : null;
					if (actionType != null)
					{
						this.SetAction(actionButton, actionType);
						goto IL_50;
					}
				}
				this._menuDragHelper.EndDrag();
				return;
			}
			IL_50:
			ActionButton dragged2 = this._menuDragHelper.Dragged;
			if (dragged2 != null && dragged2.Parent is ActionButtonContainer)
			{
				this.SetAction(dragged2, null);
			}
			this._menuDragHelper.EndDrag();
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001D694 File Offset: 0x0001B894
		private void OnClearPressed(BaseButton.ButtonEventArgs args)
		{
			if (this._window == null)
			{
				return;
			}
			this._window.SearchBar.Clear();
			this._window.FilterButton.DeselectAll();
			this.UpdateFilterLabel();
			this.SearchAndDisplay(null);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001D6CC File Offset: 0x0001B8CC
		private void OnSearchChanged(LineEdit.LineEditEventArgs args)
		{
			this.SearchAndDisplay(null);
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001D6D5 File Offset: 0x0001B8D5
		private void OnFilterSelected(MultiselectOptionButton<ActionsWindow.Filters>.ItemPressedEventArgs args)
		{
			this.UpdateFilterLabel();
			this.SearchAndDisplay(null);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001D6E4 File Offset: 0x0001B8E4
		private void OnWindowActionPressed(GUIBoundKeyEventArgs args, ActionButton action)
		{
			if (args.Function != EngineKeyFunctions.UIClick && args.Function != EngineKeyFunctions.Use)
			{
				return;
			}
			this._menuDragHelper.MouseDown(action);
			args.Handle();
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001D71D File Offset: 0x0001B91D
		private void OnWindowActionUnPressed(GUIBoundKeyEventArgs args, ActionButton dragged)
		{
			if (args.Function != EngineKeyFunctions.UIClick && args.Function != EngineKeyFunctions.Use)
			{
				return;
			}
			this.DragAction();
			args.Handle();
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001D750 File Offset: 0x0001B950
		private void OnWindowActionFocusExisted(ActionButton button)
		{
			this._menuDragHelper.EndDrag();
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001D760 File Offset: 0x0001B960
		private void OnActionPressed(GUIBoundKeyEventArgs args, ActionButton button)
		{
			if (args.Function == EngineKeyFunctions.UIClick)
			{
				this._menuDragHelper.MouseDown(button);
				args.Handle();
				return;
			}
			if (args.Function == EngineKeyFunctions.UIRightClick)
			{
				this.SetAction(button, null);
				args.Handle();
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0001D7B4 File Offset: 0x0001B9B4
		private void OnActionUnpressed(GUIBoundKeyEventArgs args, ActionButton button)
		{
			if (args.Function != EngineKeyFunctions.UIClick)
			{
				return;
			}
			if (this.UIManager.CurrentlyHovered == button)
			{
				this._menuDragHelper.EndDrag();
				TargetedAction targetedAction = button.Action as TargetedAction;
				if (targetedAction != null)
				{
					this.ToggleTargeting(targetedAction);
					return;
				}
				ActionsSystem actionsSystem = this._actionsSystem;
				if (actionsSystem != null)
				{
					actionsSystem.TriggerAction(button.Action);
				}
			}
			else
			{
				this.DragAction();
			}
			args.Handle();
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001D82C File Offset: 0x0001BA2C
		private bool OnMenuBeginDrag()
		{
			ActionButton dragged = this._menuDragHelper.Dragged;
			ActionType actionType = (dragged != null) ? dragged.Action : null;
			if (actionType != null)
			{
				if (actionType.EntityIcon != null)
				{
					TextureRect dragShadow = this._dragShadow;
					IRsiStateLike icon = this._entities.GetComponent<SpriteComponent>(actionType.EntityIcon.Value).Icon;
					dragShadow.Texture = ((icon != null) ? icon.GetFrame(0, 0) : null);
				}
				else if (actionType.Icon != null)
				{
					this._dragShadow.Texture = SpriteSpecifierExt.Frame0(actionType.Icon);
				}
				else
				{
					this._dragShadow.Texture = null;
				}
			}
			LayoutContainer.SetPosition(this._dragShadow, this.UIManager.MousePositionScaled.Position - new ValueTuple<float, float>(32f, 32f));
			return true;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001D900 File Offset: 0x0001BB00
		private bool OnMenuContinueDrag(float frameTime)
		{
			LayoutContainer.SetPosition(this._dragShadow, this.UIManager.MousePositionScaled.Position - new ValueTuple<float, float>(32f, 32f));
			this._dragShadow.Visible = true;
			return true;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001D94E File Offset: 0x0001BB4E
		private void OnMenuEndDrag()
		{
			this._dragShadow.Texture = null;
			this._dragShadow.Visible = false;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001D968 File Offset: 0x0001BB68
		public void ReloadActionContainer()
		{
			this.UnloadGui();
			this.LoadGui();
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x0001D978 File Offset: 0x0001BB78
		public void UnloadGui()
		{
			ActionsSystem actionsSystem = this._actionsSystem;
			if (actionsSystem != null)
			{
				actionsSystem.UnlinkAllActions();
			}
			if (this.ActionsBar == null)
			{
				return;
			}
			this.ActionsBar.PageButtons.LeftArrow.OnPressed -= this.OnLeftArrowPressed;
			this.ActionsBar.PageButtons.RightArrow.OnPressed -= this.OnRightArrowPressed;
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0001D9E4 File Offset: 0x0001BBE4
		public void LoadGui()
		{
			if (this.ActionsBar == null)
			{
				return;
			}
			this.ActionsBar.PageButtons.LeftArrow.OnPressed += this.OnLeftArrowPressed;
			this.ActionsBar.PageButtons.RightArrow.OnPressed += this.OnRightArrowPressed;
			this.RegisterActionContainer(this.ActionsBar.ActionsContainer);
			ActionsSystem actionsSystem = this._actionsSystem;
			if (actionsSystem == null)
			{
				return;
			}
			actionsSystem.LinkAllActions(null);
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0001DA60 File Offset: 0x0001BC60
		public void RegisterActionContainer(ActionButtonContainer container)
		{
			if (this._container != null)
			{
				this._container.ActionPressed -= this.OnActionPressed;
				this._container.ActionUnpressed -= this.OnActionPressed;
			}
			this._container = container;
			this._container.ActionPressed += this.OnActionPressed;
			this._container.ActionUnpressed += this.OnActionUnpressed;
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0001DAD8 File Offset: 0x0001BCD8
		public void ClearActions()
		{
			ActionButtonContainer container = this._container;
			if (container == null)
			{
				return;
			}
			container.ClearActionData();
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0001DAEC File Offset: 0x0001BCEC
		private void AssignSlots(List<ActionsSystem.SlotAssignment> assignments)
		{
			Span<ActionsSystem.SlotAssignment> span = CollectionsMarshal.AsSpan<ActionsSystem.SlotAssignment>(assignments);
			for (int i = 0; i < span.Length; i++)
			{
				ref ActionsSystem.SlotAssignment ptr = ref span[i];
				this._pages[(int)ptr.Hotbar][(int)ptr.Slot] = ptr.Action;
			}
			ActionButtonContainer container = this._container;
			if (container == null)
			{
				return;
			}
			container.SetActionData(this._pages[this._currentPageIndex]);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0001DB63 File Offset: 0x0001BD63
		public void RemoveActionContainer()
		{
			this._container = null;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001DB6C File Offset: 0x0001BD6C
		public void OnSystemLoaded(ActionsSystem system)
		{
			system.LinkActions += this.OnComponentLinked;
			system.UnlinkActions += this.OnComponentUnlinked;
			system.ClearAssignments += this.ClearActions;
			system.AssignSlot += this.AssignSlots;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001DBC4 File Offset: 0x0001BDC4
		public void OnSystemUnloaded(ActionsSystem system)
		{
			system.LinkActions -= this.OnComponentLinked;
			system.UnlinkActions -= this.OnComponentUnlinked;
			system.ClearAssignments -= this.ClearActions;
			system.AssignSlot -= this.AssignSlots;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001DC19 File Offset: 0x0001BE19
		public override void FrameUpdate(FrameEventArgs args)
		{
			this._menuDragHelper.Update(args.DeltaSeconds);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x0001DC2D File Offset: 0x0001BE2D
		private void OnComponentLinked(ActionsComponent component)
		{
			this.LoadDefaultActions(component);
			ActionButtonContainer container = this._container;
			if (container != null)
			{
				container.SetActionData(this._pages[0]);
			}
			this.SearchAndDisplay(component);
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001DC5F File Offset: 0x0001BE5F
		private void OnComponentUnlinked()
		{
			ActionButtonContainer container = this._container;
			if (container != null)
			{
				container.ClearActionData();
			}
			this.SearchAndDisplay(null);
			this.StopTargeting();
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001DC80 File Offset: 0x0001BE80
		private void LoadDefaultActions(ActionsComponent component)
		{
			List<ActionType> list = (from actionType in component.Actions
			where actionType.AutoPopulate
			select actionType).ToList<ActionType>();
			int num = 0;
			int count = this._pages.Count;
			int i = count;
			int num2 = 0;
			while (i > 0)
			{
				ActionUIController.ActionPage actionPage = this._pages[num2];
				int size = actionPage.Size;
				for (int j = 0; j < size; j++)
				{
					if (j + num < list.Count)
					{
						actionPage[j] = list[j + num];
					}
					else
					{
						actionPage[j] = null;
					}
				}
				num += size;
				num2++;
				if (num2 == count)
				{
					num2 = 0;
				}
				i--;
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001DD41 File Offset: 0x0001BF41
		public void ToggleTargeting(TargetedAction action)
		{
			if (this.SelectingTargetFor == action)
			{
				this.StopTargeting();
				return;
			}
			this.StartTargeting(action);
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0001DD5C File Offset: 0x0001BF5C
		private void StartTargeting(TargetedAction action)
		{
			this.StopTargeting();
			this.SelectingTargetFor = action;
			ShowHandItemOverlay showHandItemOverlay;
			if (action.TargetingIndicator && this._overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
			{
				if (action.ItemIconStyle == ItemActionIconStyle.BigItem && action.Provider != null)
				{
					showHandItemOverlay.EntityOverride = action.Provider;
				}
				else if (action.Toggled && action.IconOn != null)
				{
					showHandItemOverlay.IconOverride = SpriteSpecifierExt.Frame0(action.IconOn);
				}
				else if (action.Icon != null)
				{
					showHandItemOverlay.IconOverride = SpriteSpecifierExt.Frame0(action.Icon);
				}
			}
			EntityTargetAction entityAction = action as EntityTargetAction;
			if (entityAction == null)
			{
				return;
			}
			Func<EntityUid, bool> predicate = null;
			if (!entityAction.CanTargetSelf)
			{
				predicate = ((EntityUid e) => e != entityAction.AttachedEntity);
			}
			float range = entityAction.CheckCanAccess ? action.Range : -1f;
			InteractionOutlineSystem interactionOutline = this._interactionOutline;
			if (interactionOutline != null)
			{
				interactionOutline.SetEnabled(false);
			}
			TargetOutlineSystem targetOutline = this._targetOutline;
			if (targetOutline == null)
			{
				return;
			}
			targetOutline.Enable(range, entityAction.CheckCanAccess, predicate, entityAction.Whitelist, null);
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0001DE7C File Offset: 0x0001C07C
		public void StopTargeting()
		{
			if (this.SelectingTargetFor == null)
			{
				return;
			}
			this.SelectingTargetFor = null;
			TargetOutlineSystem targetOutline = this._targetOutline;
			if (targetOutline != null)
			{
				targetOutline.Disable();
			}
			InteractionOutlineSystem interactionOutline = this._interactionOutline;
			if (interactionOutline != null)
			{
				interactionOutline.SetEnabled(true);
			}
			ShowHandItemOverlay showHandItemOverlay;
			if (!this._overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
			{
				return;
			}
			showHandItemOverlay.IconOverride = null;
			showHandItemOverlay.EntityOverride = null;
		}

		// Token: 0x04000263 RID: 611
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04000264 RID: 612
		[Dependency]
		private readonly IOverlayManager _overlays;

		// Token: 0x04000265 RID: 613
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000266 RID: 614
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000267 RID: 615
		[Nullable(2)]
		[UISystemDependency]
		private readonly ActionsSystem _actionsSystem;

		// Token: 0x04000268 RID: 616
		[Nullable(2)]
		[UISystemDependency]
		private readonly InteractionOutlineSystem _interactionOutline;

		// Token: 0x04000269 RID: 617
		[Nullable(2)]
		[UISystemDependency]
		private readonly TargetOutlineSystem _targetOutline;

		// Token: 0x0400026A RID: 618
		private const int DefaultPageIndex = 0;

		// Token: 0x0400026B RID: 619
		[Nullable(2)]
		private ActionButtonContainer _container;

		// Token: 0x0400026C RID: 620
		private readonly List<ActionUIController.ActionPage> _pages = new List<ActionUIController.ActionPage>();

		// Token: 0x0400026D RID: 621
		private int _currentPageIndex;

		// Token: 0x0400026E RID: 622
		private readonly DragDropHelper<ActionButton> _menuDragHelper;

		// Token: 0x0400026F RID: 623
		private readonly TextureRect _dragShadow;

		// Token: 0x04000270 RID: 624
		[Nullable(2)]
		private ActionsWindow _window;

		// Token: 0x020000BE RID: 190
		[NullableContext(2)]
		[Nullable(0)]
		private sealed class ActionPage
		{
			// Token: 0x06000565 RID: 1381 RVA: 0x0001DEDF File Offset: 0x0001C0DF
			public ActionPage(int size)
			{
				this._data = new ActionType[size];
			}

			// Token: 0x170000D0 RID: 208
			public ActionType this[int index]
			{
				get
				{
					return this._data[index];
				}
				set
				{
					this._data[index] = value;
				}
			}

			// Token: 0x06000568 RID: 1384 RVA: 0x0001DF08 File Offset: 0x0001C108
			[NullableContext(1)]
			[return: Nullable(new byte[]
			{
				1,
				2
			})]
			public static implicit operator ActionType[](ActionUIController.ActionPage p)
			{
				return p._data.ToArray<ActionType>();
			}

			// Token: 0x06000569 RID: 1385 RVA: 0x0001DF15 File Offset: 0x0001C115
			public void Clear()
			{
				Array.Fill<ActionType>(this._data, null);
			}

			// Token: 0x170000D1 RID: 209
			// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001DF23 File Offset: 0x0001C123
			public int Size
			{
				get
				{
					return this._data.Length;
				}
			}

			// Token: 0x04000272 RID: 626
			[Nullable(new byte[]
			{
				1,
				2
			})]
			private readonly ActionType[] _data;
		}
	}
}
