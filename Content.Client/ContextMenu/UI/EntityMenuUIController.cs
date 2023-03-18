using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.CombatMode;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Verbs;
using Content.Client.Verbs.UI;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.ContextMenu.UI
{
	// Token: 0x02000386 RID: 902
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x0600161F RID: 5663 RVA: 0x00082757 File Offset: 0x00080957
		// (set) Token: 0x06001620 RID: 5664 RVA: 0x0008275F File Offset: 0x0008095F
		private int GroupingContextMenuType { get; set; }

		// Token: 0x06001621 RID: 5665 RVA: 0x00082768 File Offset: 0x00080968
		public void OnGroupingChanged(int obj)
		{
			this._context.Close();
			this.GroupingContextMenuType = obj;
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x0008277C File Offset: 0x0008097C
		private List<List<EntityUid>> GroupEntities(IEnumerable<EntityUid> entities, int depth = 0)
		{
			if (this.GroupingContextMenuType == 0)
			{
				return (from grp in entities.GroupBy(delegate(EntityUid e)
				{
					string str = Identity.Name(e, this._entityManager, null);
					EntityPrototype entityPrototype = this._entityManager.GetComponent<MetaDataComponent>(e).EntityPrototype;
					return str + (((entityPrototype != null) ? entityPrototype.ID : null) ?? string.Empty);
				}).ToList<IGrouping<string, EntityUid>>()
				select grp.ToList<EntityUid>()).ToList<List<EntityUid>>();
			}
			return (from grp in entities.GroupBy((EntityUid e) => e, new EntityMenuUIController.PrototypeAndStatesContextMenuComparer(depth, this._entityManager)).ToList<IGrouping<EntityUid, EntityUid>>()
			select grp.ToList<EntityUid>()).ToList<List<EntityUid>>();
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00082834 File Offset: 0x00080A34
		public void OnStateEntered(GameplayState state)
		{
			this._updating = true;
			this._cfg.OnValueChanged<int>(CCVars.EntityMenuGroupingType, new Action<int>(this.OnGroupingChanged), true);
			ContextMenuUIController context = this._context;
			context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Combine(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown));
			CommandBinds.Builder.Bind(EngineKeyFunctions.UseSecondary, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.HandleOpenEntityMenu), true, true)).Register<EntityMenuUIController>();
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000828B4 File Offset: 0x00080AB4
		public void OnStateExited(GameplayState state)
		{
			this._updating = false;
			this.Elements.Clear();
			this._cfg.UnsubValueChanged<int>(CCVars.EntityMenuGroupingType, new Action<int>(this.OnGroupingChanged));
			ContextMenuUIController context = this._context;
			context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Remove(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown));
			CommandBinds.Unregister<EntityMenuUIController>();
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0008291C File Offset: 0x00080B1C
		public void OpenRootMenu(List<EntityUid> entities)
		{
			if (this._context.RootMenu.Visible)
			{
				this._context.Close();
			}
			List<List<EntityUid>> list = this.GroupEntities(entities, 0).ToList<List<EntityUid>>();
			list.Sort(delegate(List<EntityUid> x, List<EntityUid> y)
			{
				EntityPrototype entityPrototype = this._entityManager.GetComponent<MetaDataComponent>(x.First<EntityUid>()).EntityPrototype;
				string strA = (entityPrototype != null) ? entityPrototype.Name : null;
				EntityPrototype entityPrototype2 = this._entityManager.GetComponent<MetaDataComponent>(y.First<EntityUid>()).EntityPrototype;
				return string.CompareOrdinal(strA, (entityPrototype2 != null) ? entityPrototype2.Name : null);
			});
			this.Elements.Clear();
			this.AddToUI(list);
			UIBox2 value = UIBox2.FromDimensions(this._userInterfaceManager.MousePositionScaled.Position, new ValueTuple<float, float>(1f, 1f));
			this._context.RootMenu.Open(new UIBox2?(value), null);
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x000829C4 File Offset: 0x00080BC4
		public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
		{
			EntityMenuElement entityMenuElement = element as EntityMenuElement;
			if (entityMenuElement == null)
			{
				return;
			}
			EntityUid? entityUid = entityMenuElement.Entity;
			EntityUid? entityUid2 = entityUid;
			if (entityUid2 == null)
			{
				entityUid = this.GetFirstEntityOrNull(element.SubMenu);
			}
			if (this._entityManager.Deleted(entityUid))
			{
				return;
			}
			if (args.Function == ContentKeyFunctions.ExamineEntity)
			{
				this._systemManager.GetEntitySystem<ExamineSystem>().DoExamine(entityUid.Value, true);
				args.Handle();
				return;
			}
			if (args.Function == EngineKeyFunctions.Use || args.Function == ContentKeyFunctions.ActivateItemInWorld || args.Function == ContentKeyFunctions.AltActivateItemInWorld || args.Function == ContentKeyFunctions.Point || args.Function == ContentKeyFunctions.TryPullObject || args.Function == ContentKeyFunctions.MovePulledObject)
			{
				InputSystem entitySystem = this._systemManager.GetEntitySystem<InputSystem>();
				BoundKeyFunction function = args.Function;
				KeyFunctionId keyFunctionId = this._inputManager.NetworkBindMap.KeyFunctionID(function);
				FullInputCmdMessage fullInputCmdMessage = new FullInputCmdMessage(this._gameTiming.CurTick, this._gameTiming.TickFraction, keyFunctionId, 1, this._entityManager.GetComponent<TransformComponent>(entityUid.Value).Coordinates, args.PointerLocation, entityUid.Value);
				LocalPlayer localPlayer = this._playerManager.LocalPlayer;
				ICommonSession commonSession = (localPlayer != null) ? localPlayer.Session : null;
				if (commonSession != null)
				{
					entitySystem.HandleInputCommand(commonSession, function, fullInputCmdMessage, false);
				}
				this._context.Close();
				args.Handle();
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x00082B54 File Offset: 0x00080D54
		private bool HandleOpenEntityMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (args.State != 1)
			{
				return false;
			}
			if (!(this._stateManager.CurrentState is GameplayStateBase))
			{
				return false;
			}
			SharedCombatModeSystem combatMode = this._combatMode;
			ICommonSession session = args.Session;
			if (combatMode.IsInCombatMode((session != null) ? session.AttachedEntity : null, null))
			{
				return false;
			}
			MapCoordinates targetPos = args.Coordinates.ToMap(this._entityManager);
			List<EntityUid> entities;
			if (this._verbSystem.TryGetEntityMenuEntities(targetPos, out entities))
			{
				this.OpenRootMenu(entities);
			}
			return true;
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00082BD4 File Offset: 0x00080DD4
		public override void FrameUpdate(FrameEventArgs args)
		{
			if (!this._updating || this._context.RootMenu == null)
			{
				return;
			}
			if (!this._context.RootMenu.Visible)
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid player = entityUid.GetValueOrDefault();
				if (player.IsValid())
				{
					bool flag = !this._eyeManager.CurrentEye.DrawFov || (this._verbSystem.Visibility & MenuVisibility.NoFov) == MenuVisibility.NoFov;
					ExaminerComponent examinerComp;
					this._entityManager.TryGetComponent<ExaminerComponent>(player, ref examinerComp);
					EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
					using (List<EntityUid>.Enumerator enumerator = this.Elements.Keys.ToList<EntityUid>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EntityUid entity = enumerator.Current;
							TransformComponent transformComponent;
							if (!entityQuery.TryGetComponent(entity, ref transformComponent))
							{
								this.RemoveEntity(entity);
							}
							else if (!flag)
							{
								MapCoordinates target;
								target..ctor(this._xform.GetWorldPosition(transformComponent, entityQuery), transformComponent.MapID);
								if (!this._examineSystem.CanExamine(player, target, (EntityUid e) => e == player || e == entity, new EntityUid?(entity), examinerComp))
								{
									this.RemoveEntity(entity);
								}
							}
						}
					}
					return;
				}
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x00082D84 File Offset: 0x00080F84
		private void AddToUI(List<List<EntityUid>> entityGroups)
		{
			if (entityGroups.Count == 1)
			{
				this.AddGroupToMenu(entityGroups[0], this._context.RootMenu);
				return;
			}
			foreach (List<EntityUid> list in entityGroups)
			{
				if (list.Count > 1)
				{
					this.AddGroupToUI(list);
				}
				else
				{
					this.AddEntityToMenu(list[0], this._context.RootMenu);
				}
			}
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x00082E18 File Offset: 0x00081018
		private void AddGroupToUI(List<EntityUid> group)
		{
			EntityMenuElement entityMenuElement = new EntityMenuElement(null);
			ContextMenuPopup menu = new ContextMenuPopup(this._context, entityMenuElement);
			this.AddGroupToMenu(group, menu);
			this.UpdateElement(entityMenuElement);
			this._context.AddElement(this._context.RootMenu, entityMenuElement);
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x00082E68 File Offset: 0x00081068
		private void AddGroupToMenu(List<EntityUid> group, ContextMenuPopup menu)
		{
			foreach (EntityUid entity in group)
			{
				this.AddEntityToMenu(entity, menu);
			}
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x00082EB8 File Offset: 0x000810B8
		private void AddEntityToMenu(EntityUid entity, ContextMenuPopup menu)
		{
			EntityMenuElement element = new EntityMenuElement(new EntityUid?(entity));
			element.SubMenu = new ContextMenuPopup(this._context, element);
			element.SubMenu.OnPopupOpen += delegate()
			{
				this._verb.OpenVerbMenu(entity, false, element.SubMenu);
			};
			element.SubMenu.OnPopupHide += element.SubMenu.MenuBody.DisposeAllChildren;
			this._context.AddElement(menu, element);
			this.Elements.TryAdd(entity, element);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x00082F7C File Offset: 0x0008117C
		private void RemoveEntity(EntityUid entity)
		{
			EntityMenuElement entityMenuElement;
			if (!this.Elements.TryGetValue(entity, out entityMenuElement))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Attempted to remove unknown entity from the entity menu: ");
				defaultInterpolatedStringHandler.AppendFormatted(this._entityManager.GetComponent<MetaDataComponent>(entity).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			ContextMenuPopup parentMenu = entityMenuElement.ParentMenu;
			ContextMenuElement contextMenuElement = (parentMenu != null) ? parentMenu.ParentElement : null;
			entityMenuElement.Dispose();
			this.Elements.Remove(entity);
			EntityMenuElement entityMenuElement2 = contextMenuElement as EntityMenuElement;
			if (entityMenuElement2 != null)
			{
				this.UpdateElement(entityMenuElement2);
			}
			if (this._context.RootMenu.MenuBody.ChildCount == 0)
			{
				this._context.Close();
			}
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0008304C File Offset: 0x0008124C
		private void UpdateElement(EntityMenuElement element)
		{
			if (element.SubMenu == null)
			{
				return;
			}
			EntityUid? firstEntityOrNull = this.GetFirstEntityOrNull(element.SubMenu);
			if (firstEntityOrNull == null)
			{
				element.Dispose();
				return;
			}
			element.UpdateEntity(firstEntityOrNull);
			element.Count = 0;
			foreach (Control control in element.SubMenu.MenuBody.Children)
			{
				EntityMenuElement entityMenuElement = control as EntityMenuElement;
				if (entityMenuElement != null)
				{
					element.Count += entityMenuElement.Count;
				}
			}
			element.CountLabel.Text = element.Count.ToString();
			if (element.Count == 1)
			{
				element.Entity = firstEntityOrNull;
				element.SubMenu.Dispose();
				element.SubMenu = null;
				element.CountLabel.Visible = false;
				this.Elements[firstEntityOrNull.Value] = element;
			}
			ContextMenuPopup parentMenu = element.ParentMenu;
			EntityMenuElement entityMenuElement2 = ((parentMenu != null) ? parentMenu.ParentElement : null) as EntityMenuElement;
			if (entityMenuElement2 != null)
			{
				this.UpdateElement(entityMenuElement2);
			}
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0008316C File Offset: 0x0008136C
		[NullableContext(2)]
		private EntityUid? GetFirstEntityOrNull(ContextMenuPopup menu)
		{
			if (menu == null)
			{
				return null;
			}
			foreach (Control control in menu.MenuBody.Children)
			{
				EntityMenuElement entityMenuElement = control as EntityMenuElement;
				if (entityMenuElement != null)
				{
					if (entityMenuElement.Entity != null)
					{
						if (!this._entityManager.Deleted(entityMenuElement.Entity))
						{
							return entityMenuElement.Entity;
						}
					}
					else
					{
						EntityUid? firstEntityOrNull = this.GetFirstEntityOrNull(entityMenuElement.SubMenu);
						if (firstEntityOrNull != null)
						{
							return firstEntityOrNull;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x04000B9C RID: 2972
		public const int GroupingTypesCount = 2;

		// Token: 0x04000B9E RID: 2974
		[Dependency]
		private readonly IEntitySystemManager _systemManager;

		// Token: 0x04000B9F RID: 2975
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000BA0 RID: 2976
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000BA1 RID: 2977
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000BA2 RID: 2978
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000BA3 RID: 2979
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000BA4 RID: 2980
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000BA5 RID: 2981
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000BA6 RID: 2982
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000BA7 RID: 2983
		[Dependency]
		private readonly ContextMenuUIController _context;

		// Token: 0x04000BA8 RID: 2984
		[Dependency]
		private readonly VerbMenuUIController _verb;

		// Token: 0x04000BA9 RID: 2985
		[UISystemDependency]
		private readonly VerbSystem _verbSystem;

		// Token: 0x04000BAA RID: 2986
		[UISystemDependency]
		private readonly ExamineSystem _examineSystem;

		// Token: 0x04000BAB RID: 2987
		[UISystemDependency]
		private readonly TransformSystem _xform;

		// Token: 0x04000BAC RID: 2988
		[UISystemDependency]
		private readonly CombatModeSystem _combatMode;

		// Token: 0x04000BAD RID: 2989
		private bool _updating;

		// Token: 0x04000BAE RID: 2990
		public Dictionary<EntityUid, EntityMenuElement> Elements = new Dictionary<EntityUid, EntityMenuElement>();

		// Token: 0x02000387 RID: 903
		[Nullable(0)]
		private sealed class PrototypeAndStatesContextMenuComparer : IEqualityComparer<EntityUid>
		{
			// Token: 0x17000474 RID: 1140
			// (get) Token: 0x06001633 RID: 5683 RVA: 0x000832DE File Offset: 0x000814DE
			private static int Count
			{
				get
				{
					return EntityMenuUIController.PrototypeAndStatesContextMenuComparer.EqualsList.Count - 1;
				}
			}

			// Token: 0x06001634 RID: 5684 RVA: 0x000832EC File Offset: 0x000814EC
			[NullableContext(2)]
			public PrototypeAndStatesContextMenuComparer(int step = 0, IEntityManager entMan = null)
			{
				IoCManager.Resolve<IEntityManager>(ref entMan);
				this._depth = ((step > EntityMenuUIController.PrototypeAndStatesContextMenuComparer.Count) ? EntityMenuUIController.PrototypeAndStatesContextMenuComparer.Count : step);
				this._entMan = entMan;
			}

			// Token: 0x06001635 RID: 5685 RVA: 0x00083318 File Offset: 0x00081518
			public bool Equals(EntityUid x, EntityUid y)
			{
				if (x == default(EntityUid))
				{
					return y == default(EntityUid);
				}
				return y != default(EntityUid) && EntityMenuUIController.PrototypeAndStatesContextMenuComparer.EqualsList[this._depth](x, y, this._entMan);
			}

			// Token: 0x06001636 RID: 5686 RVA: 0x00083376 File Offset: 0x00081576
			public int GetHashCode(EntityUid e)
			{
				return EntityMenuUIController.PrototypeAndStatesContextMenuComparer.GetHashCodeList[this._depth](e, this._entMan);
			}

			// Token: 0x04000BAF RID: 2991
			private static readonly List<Func<EntityUid, EntityUid, IEntityManager, bool>> EqualsList = new List<Func<EntityUid, EntityUid, IEntityManager, bool>>
			{
				(EntityUid a, EntityUid b, IEntityManager entMan) => entMan.GetComponent<MetaDataComponent>(a).EntityPrototype.ID == entMan.GetComponent<MetaDataComponent>(b).EntityPrototype.ID,
				delegate(EntityUid a, EntityUid b, IEntityManager entMan)
				{
					SpriteComponent spriteComponent;
					entMan.TryGetComponent<SpriteComponent>(a, ref spriteComponent);
					SpriteComponent spriteComponent2;
					entMan.TryGetComponent<SpriteComponent>(b, ref spriteComponent2);
					if (spriteComponent == null || spriteComponent2 == null)
					{
						return spriteComponent == spriteComponent2;
					}
					IEnumerable<string> source = from e in spriteComponent.AllLayers
					where e.Visible
					select e into s
					select s.RsiState.Name;
					IEnumerable<string> source2 = from e in spriteComponent2.AllLayers
					where e.Visible
					select e into s
					select s.RsiState.Name;
					return (from t in source
					orderby t
					select t).SequenceEqual(from t in source2
					orderby t
					select t);
				}
			};

			// Token: 0x04000BB0 RID: 2992
			private static readonly List<Func<EntityUid, IEntityManager, int>> GetHashCodeList = new List<Func<EntityUid, IEntityManager, int>>
			{
				(EntityUid e, IEntityManager entMan) => EqualityComparer<string>.Default.GetHashCode(entMan.GetComponent<MetaDataComponent>(e).EntityPrototype.ID),
				delegate(EntityUid e, IEntityManager entMan)
				{
					int num = 0;
					foreach (string obj2 in from obj in entMan.GetComponent<SpriteComponent>(e).AllLayers
					where obj.Visible
					select obj into s
					select s.RsiState.Name)
					{
						num ^= EqualityComparer<string>.Default.GetHashCode(obj2);
					}
					return num;
				}
			};

			// Token: 0x04000BB1 RID: 2993
			private readonly int _depth;

			// Token: 0x04000BB2 RID: 2994
			private readonly IEntityManager _entMan;
		}
	}
}
