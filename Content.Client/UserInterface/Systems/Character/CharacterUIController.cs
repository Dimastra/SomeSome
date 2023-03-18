using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.CharacterInfo;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Character.Controls;
using Content.Client.UserInterface.Systems.Character.Windows;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Objectives.Controls;
using Content.Shared.Input;
using Content.Shared.Objectives;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.Input.Binding;
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.Character
{
	// Token: 0x020000AE RID: 174
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CharacterUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<CharacterInfoSystem>, IOnSystemLoaded<CharacterInfoSystem>, IOnSystemUnloaded<CharacterInfoSystem>
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x00019CAC File Offset: 0x00017EAC
		[Nullable(2)]
		private MenuButton CharacterButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.CharacterButton;
			}
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00019CC4 File Offset: 0x00017EC4
		public void OnStateEntered(GameplayState state)
		{
			this._window = this.UIManager.CreateWindow<CharacterWindow>();
			LayoutContainer.SetAnchorPreset(this._window, 5, false);
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCharacterMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true)).Register<CharacterUIController>();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00019D18 File Offset: 0x00017F18
		public void OnStateExited(GameplayState state)
		{
			if (this._window != null)
			{
				this._window.Dispose();
				this._window = null;
			}
			CommandBinds.Unregister<CharacterUIController>();
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00019D39 File Offset: 0x00017F39
		public void OnSystemLoaded(CharacterInfoSystem system)
		{
			system.OnCharacterUpdate += this.CharacterUpdated;
			system.OnCharacterDetached += this.CharacterDetached;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00019D5F File Offset: 0x00017F5F
		public void OnSystemUnloaded(CharacterInfoSystem system)
		{
			system.OnCharacterUpdate -= this.CharacterUpdated;
			system.OnCharacterDetached -= this.CharacterDetached;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00019D85 File Offset: 0x00017F85
		public void UnloadButton()
		{
			if (this.CharacterButton == null)
			{
				return;
			}
			this.CharacterButton.OnPressed -= this.CharacterButtonPressed;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00019DA8 File Offset: 0x00017FA8
		public void LoadButton()
		{
			if (this.CharacterButton == null)
			{
				return;
			}
			this.CharacterButton.OnPressed += this.CharacterButtonPressed;
			if (this._window == null)
			{
				return;
			}
			this._window.OnClose += this.DeactivateButton;
			this._window.OnOpen += this.ActivateButton;
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00019E0C File Offset: 0x0001800C
		private void DeactivateButton()
		{
			this.CharacterButton.Pressed = false;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00019E1A File Offset: 0x0001801A
		private void ActivateButton()
		{
			this.CharacterButton.Pressed = true;
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00019E28 File Offset: 0x00018028
		private void CharacterUpdated(CharacterInfoSystem.CharacterData data)
		{
			if (this._window == null)
			{
				return;
			}
			CharacterInfoSystem.CharacterData characterData = data;
			string text;
			Dictionary<string, List<ConditionInfo>> dictionary;
			string text2;
			SpriteComponent spriteComponent;
			string text3;
			characterData.Deconstruct(out text, out dictionary, out text2, out spriteComponent, out text3);
			string text4 = text;
			Dictionary<string, List<ConditionInfo>> dictionary2 = dictionary;
			string text5 = text2;
			SpriteComponent sprite = spriteComponent;
			string text6 = text3;
			this._window.SubText.Text = text4;
			this._window.Objectives.RemoveAllChildren();
			foreach (KeyValuePair<string, List<ConditionInfo>> keyValuePair in dictionary2)
			{
				List<ConditionInfo> list;
				keyValuePair.Deconstruct(out text3, out list);
				string text7 = text3;
				List<ConditionInfo> list2 = list;
				CharacterObjectiveControl characterObjectiveControl = new CharacterObjectiveControl
				{
					Orientation = 1,
					Modulate = Color.Gray
				};
				characterObjectiveControl.AddChild(new Label
				{
					Text = text7,
					Modulate = Color.LightSkyBlue
				});
				foreach (ConditionInfo conditionInfo in list2)
				{
					characterObjectiveControl.AddChild(new ObjectiveConditionsControl
					{
						ProgressTexture = 
						{
							Texture = SpriteSpecifierExt.Frame0(conditionInfo.SpriteSpecifier),
							Progress = conditionInfo.Progress
						},
						Title = 
						{
							Text = conditionInfo.Title
						},
						Description = 
						{
							Text = conditionInfo.Description
						}
					});
				}
				characterObjectiveControl.AddChild(new ObjectiveBriefingControl
				{
					Label = 
					{
						Text = text5
					}
				});
				this._window.Objectives.AddChild(characterObjectiveControl);
			}
			this._window.SpriteView.Sprite = sprite;
			this._window.NameLabel.Text = text6;
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001A00C File Offset: 0x0001820C
		private void CharacterDetached()
		{
			this.CloseWindow();
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0001A014 File Offset: 0x00018214
		private void CharacterButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.ToggleWindow();
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001A01C File Offset: 0x0001821C
		private void CloseWindow()
		{
			CharacterWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0001A030 File Offset: 0x00018230
		private void ToggleWindow()
		{
			if (this._window == null)
			{
				return;
			}
			if (this.CharacterButton != null)
			{
				this.CharacterButton.Pressed = !this._window.IsOpen;
			}
			if (this._window.IsOpen)
			{
				this.CloseWindow();
				return;
			}
			this._characterInfo.RequestCharacterInfo();
			this._window.Open();
		}

		// Token: 0x04000229 RID: 553
		[UISystemDependency]
		private readonly CharacterInfoSystem _characterInfo;

		// Token: 0x0400022A RID: 554
		[Nullable(2)]
		private CharacterWindow _window;
	}
}
