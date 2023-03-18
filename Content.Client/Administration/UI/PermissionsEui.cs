using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Client.Administration.UI
{
	// Token: 0x0200048B RID: 1163
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PermissionsEui : BaseEui
	{
		// Token: 0x06001CA3 RID: 7331 RVA: 0x000A61A0 File Offset: 0x000A43A0
		public PermissionsEui()
		{
			IoCManager.InjectDependencies<PermissionsEui>(this);
			this._menu = new PermissionsEui.Menu(this);
			this._menu.AddAdminButton.OnPressed += this.AddAdminPressed;
			this._menu.AddAdminRankButton.OnPressed += this.AddAdminRankPressed;
			this._menu.OnClose += this.CloseEverything;
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x000A622B File Offset: 0x000A442B
		public override void Closed()
		{
			base.Closed();
			this.CloseEverything();
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x000A623C File Offset: 0x000A443C
		private void CloseEverything()
		{
			DefaultWindow[] array = this._subWindows.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Close();
			}
			this._menu.Close();
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x000A6278 File Offset: 0x000A4478
		private void AddAdminPressed(BaseButton.ButtonEventArgs obj)
		{
			this.OpenEditWindow(null);
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x000A6294 File Offset: 0x000A4494
		private void AddAdminRankPressed(BaseButton.ButtonEventArgs obj)
		{
			this.OpenRankEditWindow(null);
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x000A62B0 File Offset: 0x000A44B0
		private void OnEditPressed(PermissionsEuiState.AdminData admin)
		{
			this.OpenEditWindow(new PermissionsEuiState.AdminData?(admin));
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x000A62C0 File Offset: 0x000A44C0
		private void OpenEditWindow(PermissionsEuiState.AdminData? data)
		{
			PermissionsEui.EditAdminWindow window = new PermissionsEui.EditAdminWindow(this, data);
			window.SaveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.SaveAdminPressed(window);
			};
			window.OpenCentered();
			window.OnClose += delegate()
			{
				this._subWindows.Remove(window);
			};
			if (data != null)
			{
				window.RemoveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.RemoveButtonPressed(window);
				};
			}
			this._subWindows.Add(window);
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x000A635C File Offset: 0x000A455C
		[NullableContext(0)]
		private void OpenRankEditWindow(KeyValuePair<int, PermissionsEuiState.AdminRankData>? rank)
		{
			PermissionsEui.EditAdminRankWindow window = new PermissionsEui.EditAdminRankWindow(this, rank);
			window.SaveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.SaveAdminRankPressed(window);
			};
			window.OpenCentered();
			window.OnClose += delegate()
			{
				this._subWindows.Remove(window);
			};
			if (rank != null)
			{
				window.RemoveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.RemoveRankButtonPressed(window);
				};
			}
			this._subWindows.Add(window);
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x000A63F7 File Offset: 0x000A45F7
		private void RemoveButtonPressed(PermissionsEui.EditAdminWindow window)
		{
			base.SendMessage(new PermissionsEuiMsg.RemoveAdmin
			{
				UserId = window.SourceData.Value.UserId
			});
			window.Close();
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000A6420 File Offset: 0x000A4620
		private void RemoveRankButtonPressed(PermissionsEui.EditAdminRankWindow window)
		{
			base.SendMessage(new PermissionsEuiMsg.RemoveAdminRank
			{
				Id = window.SourceId.Value
			});
			window.Close();
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x000A6444 File Offset: 0x000A4644
		private void SaveAdminPressed(PermissionsEui.EditAdminWindow popup)
		{
			AdminFlags posFlags;
			AdminFlags negFlags;
			popup.CollectSetFlags(out posFlags, out negFlags);
			int? num = new int?(popup.RankButton.SelectedId);
			int? num2 = num;
			int num3 = -1;
			if (num2.GetValueOrDefault() == num3 & num2 != null)
			{
				num = null;
			}
			string title = string.IsNullOrWhiteSpace(popup.TitleEdit.Text) ? null : popup.TitleEdit.Text;
			PermissionsEuiState.AdminData? sourceData = popup.SourceData;
			if (sourceData != null)
			{
				PermissionsEuiState.AdminData valueOrDefault = sourceData.GetValueOrDefault();
				base.SendMessage(new PermissionsEuiMsg.UpdateAdmin
				{
					UserId = valueOrDefault.UserId,
					Title = title,
					PosFlags = posFlags,
					NegFlags = negFlags,
					RankId = num
				});
			}
			else
			{
				base.SendMessage(new PermissionsEuiMsg.AddAdmin
				{
					UserNameOrId = popup.NameEdit.Text,
					Title = title,
					PosFlags = posFlags,
					NegFlags = negFlags,
					RankId = num
				});
			}
			popup.Close();
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x000A6540 File Offset: 0x000A4740
		private void SaveAdminRankPressed(PermissionsEui.EditAdminRankWindow popup)
		{
			AdminFlags flags = popup.CollectSetFlags();
			string text = popup.NameEdit.Text;
			int? sourceId = popup.SourceId;
			if (sourceId != null)
			{
				int valueOrDefault = sourceId.GetValueOrDefault();
				base.SendMessage(new PermissionsEuiMsg.UpdateAdminRank
				{
					Id = valueOrDefault,
					Flags = flags,
					Name = text
				});
			}
			else
			{
				base.SendMessage(new PermissionsEuiMsg.AddAdminRank
				{
					Flags = flags,
					Name = text
				});
			}
			popup.Close();
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x000A65B9 File Offset: 0x000A47B9
		public override void Opened()
		{
			this._menu.OpenCentered();
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x000A65C8 File Offset: 0x000A47C8
		public override void HandleState(EuiStateBase state)
		{
			PermissionsEuiState permissionsEuiState = (PermissionsEuiState)state;
			if (permissionsEuiState.IsLoading)
			{
				return;
			}
			this._ranks = permissionsEuiState.AdminRanks;
			this._menu.AdminsList.RemoveAllChildren();
			using (IEnumerator<PermissionsEuiState.AdminData> enumerator = (from d in permissionsEuiState.Admins
			orderby d.UserName
			select d).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PermissionsEuiState.AdminData admin = enumerator.Current;
					GridContainer adminsList = this._menu.AdminsList;
					string text;
					if ((text = admin.UserName) == null)
					{
						NetUserId userId = admin.UserId;
						text = userId.ToString();
					}
					string text2 = text;
					adminsList.AddChild(new Label
					{
						Text = text2
					});
					Label label = new Label
					{
						Text = (admin.Title ?? Loc.GetString("permissions-eui-edit-admin-title-control-text").ToLowerInvariant())
					};
					if (admin.Title == null)
					{
						label.StyleClasses.Add("Italic");
					}
					adminsList.AddChild(label);
					AdminFlags adminFlags = admin.PosFlags;
					int? rankId = admin.RankId;
					bool flag;
					string text3;
					if (rankId != null)
					{
						int valueOrDefault = rankId.GetValueOrDefault();
						flag = false;
						PermissionsEuiState.AdminRankData adminRankData = permissionsEuiState.AdminRanks[valueOrDefault];
						text3 = adminRankData.Name;
						adminFlags |= adminRankData.Flags;
					}
					else
					{
						flag = true;
						text3 = Loc.GetString("permissions-eui-edit-no-rank-text").ToLowerInvariant();
					}
					Label label2 = new Label
					{
						Text = text3
					};
					if (flag)
					{
						label2.StyleClasses.Add("Italic");
					}
					adminsList.AddChild(label2);
					string text4 = AdminFlagsHelper.PosNegFlagsText(admin.PosFlags, admin.NegFlags);
					adminsList.AddChild(new Label
					{
						Text = text4,
						HorizontalExpand = true,
						HorizontalAlignment = 2
					});
					Button button = new Button
					{
						Text = Loc.GetString("permissions-eui-edit-title-button")
					};
					button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
					{
						this.OnEditPressed(admin);
					};
					adminsList.AddChild(button);
					if ((adminFlags.HasFlag((AdminFlags)2147483648U) && !this._adminManager.HasFlag((AdminFlags)2147483648U)) || !this._adminManager.HasFlag(AdminFlags.Permissions))
					{
						button.Disabled = true;
						button.ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-admin-tooltip");
					}
				}
			}
			this._menu.AdminRanksList.RemoveAllChildren();
			using (Dictionary<int, PermissionsEuiState.AdminRankData>.Enumerator enumerator2 = permissionsEuiState.AdminRanks.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, PermissionsEuiState.AdminRankData> kv = enumerator2.Current;
					PermissionsEuiState.AdminRankData value = kv.Value;
					string text5 = string.Join<string>(' ', from f in AdminFlagsHelper.FlagsToNames(value.Flags)
					select "+" + f);
					this._menu.AdminRanksList.AddChild(new Label
					{
						Text = value.Name
					});
					this._menu.AdminRanksList.AddChild(new Label
					{
						Text = text5,
						HorizontalExpand = true,
						HorizontalAlignment = 2
					});
					Button button2 = new Button
					{
						Text = Loc.GetString("permissions-eui-edit-admin-rank-button")
					};
					button2.OnPressed += delegate(BaseButton.ButtonEventArgs _)
					{
						this.OnEditRankPressed(kv);
					};
					this._menu.AdminRanksList.AddChild(button2);
					if ((value.Flags != (AdminFlags)2147483648U && !this._adminManager.HasFlag(AdminFlags.Permissions)) || (value.Flags == (AdminFlags)2147483648U && !this._adminManager.HasFlag((AdminFlags)2147483648U)))
					{
						button2.Disabled = true;
						button2.ToolTip = Loc.GetString("permissions-eui-do-not-have-required-flags-to-edit-rank-tooltip");
					}
				}
			}
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x000A6A0C File Offset: 0x000A4C0C
		[NullableContext(0)]
		private void OnEditRankPressed(KeyValuePair<int, PermissionsEuiState.AdminRankData> rank)
		{
			this.OpenRankEditWindow(new KeyValuePair<int, PermissionsEuiState.AdminRankData>?(rank));
		}

		// Token: 0x04000E50 RID: 3664
		private const int NoRank = -1;

		// Token: 0x04000E51 RID: 3665
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x04000E52 RID: 3666
		private readonly PermissionsEui.Menu _menu;

		// Token: 0x04000E53 RID: 3667
		private readonly List<DefaultWindow> _subWindows = new List<DefaultWindow>();

		// Token: 0x04000E54 RID: 3668
		private Dictionary<int, PermissionsEuiState.AdminRankData> _ranks = new Dictionary<int, PermissionsEuiState.AdminRankData>();

		// Token: 0x0200048C RID: 1164
		[Nullable(0)]
		private sealed class Menu : DefaultWindow
		{
			// Token: 0x06001CB2 RID: 7346 RVA: 0x000A6A1C File Offset: 0x000A4C1C
			public Menu(PermissionsEui ui)
			{
				this._ui = ui;
				base.Title = Loc.GetString("permissions-eui-menu-title");
				TabContainer tabContainer = new TabContainer();
				this.AddAdminButton = new Button
				{
					Text = Loc.GetString("permissions-eui-menu-add-admin-button"),
					HorizontalAlignment = 3
				};
				this.AddAdminRankButton = new Button
				{
					Text = Loc.GetString("permissions-eui-menu-add-admin-rank-button"),
					HorizontalAlignment = 3
				};
				this.AdminsList = new GridContainer
				{
					Columns = 5,
					VerticalExpand = true
				};
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 1;
				Control.OrderedChildCollection children = boxContainer.Children;
				ScrollContainer scrollContainer = new ScrollContainer();
				scrollContainer.VerticalExpand = true;
				scrollContainer.Children.Add(this.AdminsList);
				children.Add(scrollContainer);
				boxContainer.Children.Add(this.AddAdminButton);
				BoxContainer boxContainer2 = boxContainer;
				TabContainer.SetTabTitle(boxContainer2, Loc.GetString("permissions-eui-menu-admins-tab-title"));
				this.AdminRanksList = new GridContainer
				{
					Columns = 3,
					VerticalExpand = true
				};
				BoxContainer boxContainer3 = new BoxContainer();
				boxContainer3.Orientation = 1;
				Control.OrderedChildCollection children2 = boxContainer3.Children;
				ScrollContainer scrollContainer2 = new ScrollContainer();
				scrollContainer2.VerticalExpand = true;
				scrollContainer2.Children.Add(this.AdminRanksList);
				children2.Add(scrollContainer2);
				boxContainer3.Children.Add(this.AddAdminRankButton);
				BoxContainer boxContainer4 = boxContainer3;
				TabContainer.SetTabTitle(boxContainer4, Loc.GetString("permissions-eui-menu-admin-ranks-tab-title"));
				tabContainer.AddChild(boxContainer2);
				tabContainer.AddChild(boxContainer4);
				base.Contents.AddChild(tabContainer);
			}

			// Token: 0x170005F4 RID: 1524
			// (get) Token: 0x06001CB3 RID: 7347 RVA: 0x000A6B88 File Offset: 0x000A4D88
			protected override Vector2 ContentsMinimumSize
			{
				get
				{
					return new ValueTuple<float, float>(600f, 400f);
				}
			}

			// Token: 0x04000E55 RID: 3669
			private readonly PermissionsEui _ui;

			// Token: 0x04000E56 RID: 3670
			public readonly GridContainer AdminsList;

			// Token: 0x04000E57 RID: 3671
			public readonly GridContainer AdminRanksList;

			// Token: 0x04000E58 RID: 3672
			public readonly Button AddAdminButton;

			// Token: 0x04000E59 RID: 3673
			public readonly Button AddAdminRankButton;
		}

		// Token: 0x0200048D RID: 1165
		[Nullable(0)]
		private sealed class EditAdminWindow : DefaultWindow
		{
			// Token: 0x06001CB4 RID: 7348 RVA: 0x000A6BA0 File Offset: 0x000A4DA0
			public EditAdminWindow(PermissionsEui ui, PermissionsEuiState.AdminData? data)
			{
				base.MinSize = new ValueTuple<float, float>(600f, 400f);
				this.SourceData = data;
				Control control;
				if (data != null)
				{
					PermissionsEuiState.AdminData valueOrDefault = data.GetValueOrDefault();
					string text = valueOrDefault.UserName ?? valueOrDefault.UserId.ToString();
					base.Title = Loc.GetString("permissions-eui-edit-admin-window-edit-admin-label", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("admin", text)
					});
					control = new Label
					{
						Text = text
					};
				}
				else
				{
					base.Title = Loc.GetString("permissions-eui-menu-add-admin-button");
					LineEdit lineEdit = new LineEdit();
					lineEdit.PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-name-edit-placeholder");
					LineEdit lineEdit2 = lineEdit;
					this.NameEdit = lineEdit;
					control = lineEdit2;
				}
				this.TitleEdit = new LineEdit
				{
					PlaceHolder = Loc.GetString("permissions-eui-edit-admin-window-title-edit-placeholder")
				};
				this.RankButton = new OptionButton();
				this.SaveButton = new Button
				{
					Text = Loc.GetString("permissions-eui-edit-admin-window-save-button"),
					HorizontalAlignment = 3
				};
				this.RankButton.AddItem(Loc.GetString("permissions-eui-edit-admin-window-no-rank-button"), new int?(-1));
				foreach (KeyValuePair<int, PermissionsEuiState.AdminRankData> keyValuePair in ui._ranks)
				{
					int num;
					PermissionsEuiState.AdminRankData adminRankData;
					keyValuePair.Deconstruct(out num, out adminRankData);
					int value = num;
					PermissionsEuiState.AdminRankData adminRankData2 = adminRankData;
					if (ui._adminManager.HasFlag((AdminFlags)2147483648U) || !adminRankData2.Flags.HasFlag((AdminFlags)2147483648U))
					{
						this.RankButton.AddItem(adminRankData2.Name, new int?(value));
					}
				}
				this.RankButton.SelectId(((data != null) ? data.GetValueOrDefault().RankId : null) ?? -1);
				this.RankButton.OnItemSelected += this.RankSelected;
				GridContainer gridContainer = new GridContainer
				{
					Columns = 4,
					HSeparationOverride = new int?(0),
					VSeparationOverride = new int?(0)
				};
				foreach (AdminFlags adminFlags in AdminFlagsHelper.AllFlags)
				{
					bool disabled;
					if (adminFlags != (AdminFlags)2147483648U)
					{
						disabled = !ui._adminManager.HasFlag(AdminFlags.Permissions);
					}
					else
					{
						disabled = !ui._adminManager.HasFlag((AdminFlags)2147483648U);
					}
					string text2 = adminFlags.ToString().ToUpper();
					ButtonGroup group = new ButtonGroup();
					Button button = new Button
					{
						Text = "I",
						StyleClasses = 
						{
							"OpenRight"
						},
						Disabled = disabled,
						Group = group
					};
					Button button2 = new Button
					{
						Text = "-",
						StyleClasses = 
						{
							"OpenBoth"
						},
						Disabled = disabled,
						Group = group
					};
					Button button3 = new Button
					{
						Text = "+",
						StyleClasses = 
						{
							"OpenLeft"
						},
						Disabled = disabled,
						Group = group
					};
					if (data != null)
					{
						PermissionsEuiState.AdminData valueOrDefault2 = data.GetValueOrDefault();
						if ((valueOrDefault2.NegFlags & adminFlags) != AdminFlags.None)
						{
							button2.Pressed = true;
						}
						else if ((valueOrDefault2.PosFlags & adminFlags) != AdminFlags.None)
						{
							button3.Pressed = true;
						}
						else
						{
							button.Pressed = true;
						}
					}
					else
					{
						button.Pressed = true;
					}
					gridContainer.AddChild(new Label
					{
						Text = text2
					});
					gridContainer.AddChild(button);
					gridContainer.AddChild(button2);
					gridContainer.AddChild(button3);
					this.FlagButtons.Add(adminFlags, new ValueTuple<Button, Button, Button>(button, button2, button3));
				}
				BoxContainer boxContainer = new BoxContainer
				{
					Orientation = 0
				};
				if (data != null)
				{
					this.RemoveButton = new Button
					{
						Text = Loc.GetString("permissions-eui-edit-admin-window-remove-flag-button")
					};
					boxContainer.AddChild(this.RemoveButton);
				}
				boxContainer.AddChild(this.SaveButton);
				Control contents = base.Contents;
				BoxContainer boxContainer2 = new BoxContainer();
				boxContainer2.Orientation = 1;
				Control.OrderedChildCollection children = boxContainer2.Children;
				BoxContainer boxContainer3 = new BoxContainer();
				boxContainer3.Orientation = 0;
				boxContainer3.SeparationOverride = new int?(2);
				Control.OrderedChildCollection children2 = boxContainer3.Children;
				BoxContainer boxContainer4 = new BoxContainer();
				boxContainer4.Orientation = 1;
				boxContainer4.HorizontalExpand = true;
				boxContainer4.Children.Add(control);
				boxContainer4.Children.Add(this.TitleEdit);
				boxContainer4.Children.Add(this.RankButton);
				children2.Add(boxContainer4);
				boxContainer3.Children.Add(gridContainer);
				boxContainer3.VerticalExpand = true;
				children.Add(boxContainer3);
				boxContainer2.Children.Add(boxContainer);
				contents.AddChild(boxContainer2);
			}

			// Token: 0x06001CB5 RID: 7349 RVA: 0x000A70B0 File Offset: 0x000A52B0
			private void RankSelected(OptionButton.ItemSelectedEventArgs obj)
			{
				this.RankButton.SelectId(obj.Id);
			}

			// Token: 0x06001CB6 RID: 7350 RVA: 0x000A70C4 File Offset: 0x000A52C4
			public void CollectSetFlags(out AdminFlags pos, out AdminFlags neg)
			{
				pos = AdminFlags.None;
				neg = AdminFlags.None;
				foreach (KeyValuePair<AdminFlags, ValueTuple<Button, Button, Button>> keyValuePair in this.FlagButtons)
				{
					AdminFlags adminFlags;
					ValueTuple<Button, Button, Button> valueTuple;
					keyValuePair.Deconstruct(out adminFlags, out valueTuple);
					ValueTuple<Button, Button, Button> valueTuple2 = valueTuple;
					AdminFlags adminFlags2 = adminFlags;
					Button item = valueTuple2.Item2;
					Button item2 = valueTuple2.Item3;
					if (item.Pressed)
					{
						neg |= adminFlags2;
					}
					else if (item2.Pressed)
					{
						pos |= adminFlags2;
					}
				}
			}

			// Token: 0x04000E5A RID: 3674
			public readonly PermissionsEuiState.AdminData? SourceData;

			// Token: 0x04000E5B RID: 3675
			[Nullable(2)]
			public readonly LineEdit NameEdit;

			// Token: 0x04000E5C RID: 3676
			public readonly LineEdit TitleEdit;

			// Token: 0x04000E5D RID: 3677
			public readonly OptionButton RankButton;

			// Token: 0x04000E5E RID: 3678
			public readonly Button SaveButton;

			// Token: 0x04000E5F RID: 3679
			[Nullable(2)]
			public readonly Button RemoveButton;

			// Token: 0x04000E60 RID: 3680
			[TupleElementNames(new string[]
			{
				"inherit",
				"sub",
				"plus"
			})]
			[Nullable(new byte[]
			{
				1,
				0,
				1,
				1,
				1
			})]
			public readonly Dictionary<AdminFlags, ValueTuple<Button, Button, Button>> FlagButtons = new Dictionary<AdminFlags, ValueTuple<Button, Button, Button>>();
		}

		// Token: 0x0200048E RID: 1166
		[Nullable(0)]
		private sealed class EditAdminRankWindow : DefaultWindow
		{
			// Token: 0x06001CB7 RID: 7351 RVA: 0x000A7154 File Offset: 0x000A5354
			[NullableContext(0)]
			public EditAdminRankWindow([Nullable(1)] PermissionsEui ui, KeyValuePair<int, PermissionsEuiState.AdminRankData>? data)
			{
				base.Title = Loc.GetString("permissions-eui-edit-admin-rank-window-title");
				base.MinSize = new ValueTuple<float, float>(600f, 400f);
				this.SourceId = ((data != null) ? new int?(data.GetValueOrDefault().Key) : null);
				this.NameEdit = new LineEdit
				{
					PlaceHolder = Loc.GetString("permissions-eui-edit-admin-rank-window-name-edit-placeholder")
				};
				if (data != null)
				{
					this.NameEdit.Text = data.Value.Value.Name;
				}
				this.SaveButton = new Button
				{
					Text = Loc.GetString("permissions-eui-menu-save-admin-rank-button"),
					HorizontalAlignment = 3,
					HorizontalExpand = true
				};
				BoxContainer boxContainer = new BoxContainer
				{
					Orientation = 1
				};
				foreach (AdminFlags adminFlags in AdminFlagsHelper.AllFlags)
				{
					bool disabled;
					if (adminFlags != (AdminFlags)2147483648U)
					{
						disabled = !ui._adminManager.HasFlag(AdminFlags.Permissions);
					}
					else
					{
						disabled = !ui._adminManager.HasFlag((AdminFlags)2147483648U);
					}
					string text = adminFlags.ToString().ToUpper();
					CheckBox checkBox = new CheckBox
					{
						Disabled = disabled,
						Text = text
					};
					if (data != null && (data.Value.Value.Flags & adminFlags) != AdminFlags.None)
					{
						checkBox.Pressed = true;
					}
					this.FlagCheckBoxes.Add(adminFlags, checkBox);
					boxContainer.AddChild(checkBox);
				}
				BoxContainer boxContainer2 = new BoxContainer
				{
					Orientation = 0
				};
				if (data != null)
				{
					this.RemoveButton = new Button
					{
						Text = Loc.GetString("permissions-eui-menu-remove-admin-rank-button")
					};
					boxContainer2.AddChild(this.RemoveButton);
				}
				boxContainer2.AddChild(this.SaveButton);
				Control contents = base.Contents;
				BoxContainer boxContainer3 = new BoxContainer();
				boxContainer3.Orientation = 1;
				boxContainer3.Children.Add(this.NameEdit);
				boxContainer3.Children.Add(boxContainer);
				boxContainer3.Children.Add(boxContainer2);
				contents.AddChild(boxContainer3);
			}

			// Token: 0x06001CB8 RID: 7352 RVA: 0x000A73B0 File Offset: 0x000A55B0
			public AdminFlags CollectSetFlags()
			{
				AdminFlags adminFlags = AdminFlags.None;
				foreach (KeyValuePair<AdminFlags, CheckBox> keyValuePair in this.FlagCheckBoxes)
				{
					AdminFlags adminFlags2;
					CheckBox checkBox;
					keyValuePair.Deconstruct(out adminFlags2, out checkBox);
					AdminFlags adminFlags3 = adminFlags2;
					if (checkBox.Pressed)
					{
						adminFlags |= adminFlags3;
					}
				}
				return adminFlags;
			}

			// Token: 0x04000E61 RID: 3681
			public readonly int? SourceId;

			// Token: 0x04000E62 RID: 3682
			public readonly LineEdit NameEdit;

			// Token: 0x04000E63 RID: 3683
			public readonly Button SaveButton;

			// Token: 0x04000E64 RID: 3684
			[Nullable(2)]
			public readonly Button RemoveButton;

			// Token: 0x04000E65 RID: 3685
			public readonly Dictionary<AdminFlags, CheckBox> FlagCheckBoxes = new Dictionary<AdminFlags, CheckBox>();
		}
	}
}
