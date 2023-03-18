using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.CombatMode;
using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Shared.Input;
using Content.Shared.Verbs;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Verbs.UI
{
	// Token: 0x02000063 RID: 99
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VerbMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x060001CA RID: 458 RVA: 0x0000CC60 File Offset: 0x0000AE60
		public void OnStateEntered(GameplayState state)
		{
			ContextMenuUIController context = this._context;
			context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Combine(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown));
			ContextMenuUIController context2 = this._context;
			context2.OnContextClosed = (Action)Delegate.Combine(context2.OnContextClosed, new Action(this.Close));
			VerbSystem verbSystem = this._verbSystem;
			verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Combine(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(this.HandleVerbsResponse));
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000CCE4 File Offset: 0x0000AEE4
		public void OnStateExited(GameplayState state)
		{
			ContextMenuUIController context = this._context;
			context.OnContextKeyEvent = (Action<ContextMenuElement, GUIBoundKeyEventArgs>)Delegate.Remove(context.OnContextKeyEvent, new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown));
			ContextMenuUIController context2 = this._context;
			context2.OnContextClosed = (Action)Delegate.Remove(context2.OnContextClosed, new Action(this.Close));
			if (this._verbSystem != null)
			{
				VerbSystem verbSystem = this._verbSystem;
				verbSystem.OnVerbsResponse = (Action<VerbsResponseEvent>)Delegate.Remove(verbSystem.OnVerbsResponse, new Action<VerbsResponseEvent>(this.HandleVerbsResponse));
			}
			this.Close();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000CD74 File Offset: 0x0000AF74
		[NullableContext(2)]
		public void OpenVerbMenu(EntityUid target, bool force = false, ContextMenuPopup popup = null)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.Valid && !this._combatMode.IsInCombatMode(new EntityUid?(valueOrDefault), null))
				{
					this.Close();
					ContextMenuPopup contextMenuPopup = popup ?? this._context.RootMenu;
					contextMenuPopup.MenuBody.DisposeAllChildren();
					this.CurrentTarget = target;
					this.CurrentVerbs = this._verbSystem.GetVerbs(target, valueOrDefault, Verb.VerbTypes, force);
					this.OpenMenu = contextMenuPopup;
					this.FillVerbPopup(contextMenuPopup);
					if (!target.IsClientSide())
					{
						this._context.AddElement(contextMenuPopup, new ContextMenuElement(Loc.GetString("verb-system-waiting-on-server-text")));
					}
					if (popup != null)
					{
						return;
					}
					contextMenuPopup.SetPositionLast();
					UIBox2 value = UIBox2.FromDimensions(this._userInterfaceManager.MousePositionScaled.Position, new ValueTuple<float, float>(1f, 1f));
					contextMenuPopup.Open(new UIBox2?(value), null);
					return;
				}
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000CE90 File Offset: 0x0000B090
		private void FillVerbPopup(ContextMenuPopup popup)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Verb verb in this.CurrentVerbs)
			{
				if (verb.Category == null)
				{
					VerbMenuElement element = new VerbMenuElement(verb);
					this._context.AddElement(popup, element);
				}
				else if (hashSet.Add(verb.Category.Text))
				{
					this.AddVerbCategory(verb.Category, popup);
				}
			}
			popup.InvalidateMeasure();
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000CF28 File Offset: 0x0000B128
		public void AddVerbCategory(VerbCategory category, ContextMenuPopup popup)
		{
			List<Verb> list = new List<Verb>();
			bool flag = false;
			foreach (Verb verb in this.CurrentVerbs)
			{
				VerbCategory category2 = verb.Category;
				if (((category2 != null) ? category2.Text : null) == category.Text)
				{
					list.Add(verb);
					flag = (flag || verb.Icon != null || verb.IconEntity != null);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			VerbMenuElement verbMenuElement = new VerbMenuElement(category, list[0].TextStyleClass);
			this._context.AddElement(popup, verbMenuElement);
			verbMenuElement.SubMenu = new ContextMenuPopup(this._context, verbMenuElement);
			foreach (Verb verb2 in list)
			{
				VerbMenuElement element = new VerbMenuElement(verb2)
				{
					IconVisible = flag,
					TextVisible = !category.IconsOnly
				};
				this._context.AddElement(verbMenuElement.SubMenu, element);
			}
			verbMenuElement.SubMenu.MenuBody.Columns = category.Columns;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000D078 File Offset: 0x0000B278
		public void AddServerVerbs([Nullable(new byte[]
		{
			2,
			1
		})] List<Verb> verbs, ContextMenuPopup popup)
		{
			popup.MenuBody.DisposeAllChildren();
			if (verbs == null)
			{
				this._context.AddElement(popup, new ContextMenuElement(Loc.GetString("verb-system-null-server-response")));
				return;
			}
			this.CurrentVerbs.UnionWith(verbs);
			this.FillVerbPopup(popup);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000D0B8 File Offset: 0x0000B2B8
		public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
		{
			if (args.Function != EngineKeyFunctions.Use && args.Function != ContentKeyFunctions.ActivateItemInWorld)
			{
				return;
			}
			VerbMenuElement verbMenuElement = element as VerbMenuElement;
			if (verbMenuElement == null)
			{
				ConfirmationMenuElement confirmationMenuElement = element as ConfirmationMenuElement;
				if (confirmationMenuElement == null)
				{
					return;
				}
				args.Handle();
				this.ExecuteVerb(confirmationMenuElement.Verb);
				return;
			}
			else
			{
				args.Handle();
				Verb verb = verbMenuElement.Verb;
				if (verb == null)
				{
					if (verbMenuElement.SubMenu == null || verbMenuElement.SubMenu.ChildCount == 0)
					{
						return;
					}
					if (verbMenuElement.SubMenu.MenuBody.ChildCount == 1)
					{
						VerbMenuElement verbMenuElement2 = verbMenuElement.SubMenu.MenuBody.Children.First<Control>() as VerbMenuElement;
						if (verbMenuElement2 != null)
						{
							verb = verbMenuElement2.Verb;
							if (verb == null)
							{
								return;
							}
							goto IL_BC;
						}
					}
					this._context.OpenSubMenu(verbMenuElement);
					return;
				}
				IL_BC:
				if (verb.ConfirmationPopup)
				{
					if (verbMenuElement.SubMenu == null)
					{
						ConfirmationMenuElement element2 = new ConfirmationMenuElement(verb, "Confirm");
						verbMenuElement.SubMenu = new ContextMenuPopup(this._context, verbMenuElement);
						this._context.AddElement(verbMenuElement.SubMenu, element2);
					}
					this._context.OpenSubMenu(verbMenuElement);
					return;
				}
				this.ExecuteVerb(verb);
				return;
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000D1D7 File Offset: 0x0000B3D7
		private void Close()
		{
			if (this.OpenMenu == null)
			{
				return;
			}
			this.OpenMenu.Close();
			this.OpenMenu = null;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000D1F4 File Offset: 0x0000B3F4
		private void HandleVerbsResponse(VerbsResponseEvent msg)
		{
			if (this.OpenMenu == null || !this.OpenMenu.Visible || this.CurrentTarget != msg.Entity)
			{
				return;
			}
			this.AddServerVerbs(msg.Verbs, this.OpenMenu);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000D234 File Offset: 0x0000B434
		private void ExecuteVerb(Verb verb)
		{
			this._verbSystem.ExecuteVerb(this.CurrentTarget, verb);
			if (verb.CloseMenu ?? verb.CloseMenuDefault)
			{
				this._context.Close();
			}
		}

		// Token: 0x04000133 RID: 307
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000134 RID: 308
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x04000135 RID: 309
		[Dependency]
		private readonly ContextMenuUIController _context;

		// Token: 0x04000136 RID: 310
		[UISystemDependency]
		private readonly CombatModeSystem _combatMode;

		// Token: 0x04000137 RID: 311
		[UISystemDependency]
		private readonly VerbSystem _verbSystem;

		// Token: 0x04000138 RID: 312
		public EntityUid CurrentTarget;

		// Token: 0x04000139 RID: 313
		public SortedSet<Verb> CurrentVerbs = new SortedSet<Verb>();

		// Token: 0x0400013A RID: 314
		[Nullable(2)]
		public ContextMenuPopup OpenMenu;
	}
}
