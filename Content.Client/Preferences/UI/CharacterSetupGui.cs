﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Humanoid;
using Content.Client.Info;
using Content.Client.Lobby.UI;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Preferences.UI
{
	// Token: 0x02000183 RID: 387
	[GenerateTypedNameReferences]
	public sealed class CharacterSetupGui : Control
	{
		// Token: 0x06000A14 RID: 2580 RVA: 0x0003A3B4 File Offset: 0x000385B4
		[NullableContext(1)]
		public CharacterSetupGui(IEntityManager entityManager, IResourceCache resourceCache, IClientPreferencesManager preferencesManager, IPrototypeManager prototypeManager, IConfigurationManager configurationManager)
		{
			CharacterSetupGui <>4__this = this;
			CharacterSetupGui.!XamlIlPopulateTrampoline(this);
			this._entityManager = entityManager;
			this._prototypeManager = prototypeManager;
			this._preferencesManager = preferencesManager;
			this._configurationManager = configurationManager;
			Texture texture = resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = new Color(37, 37, 42, byte.MaxValue)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			this.BackgroundPanel.PanelOverride = styleBoxTexture;
			this._createNewCharacterButton = new Button
			{
				Text = Loc.GetString("character-setup-gui-create-new-character-button")
			};
			this._createNewCharacterButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				preferencesManager.CreateCharacter(HumanoidCharacterProfile.Random(null));
				<>4__this.UpdateUI();
				args.Event.Handle();
			};
			this._humanoidProfileEditor = new HumanoidProfileEditor(preferencesManager, prototypeManager, entityManager, configurationManager);
			this._humanoidProfileEditor.OnProfileChanged += new Action<HumanoidCharacterProfile, int>(this.ProfileChanged);
			this.CharEditor.AddChild(this._humanoidProfileEditor);
			this.UpdateUI();
			this.RulesButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				new RulesAndInfoWindow().Open();
			};
			preferencesManager.OnServerDataLoaded += this.UpdateUI;
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0003A503 File Offset: 0x00038703
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			this._preferencesManager.OnServerDataLoaded -= this.UpdateUI;
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x0003A527 File Offset: 0x00038727
		public void Save()
		{
			this._humanoidProfileEditor.Save();
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x0003A534 File Offset: 0x00038734
		[NullableContext(1)]
		private void ProfileChanged(ICharacterProfile profile, int profileSlot)
		{
			this._humanoidProfileEditor.UpdateControls();
			this.UpdateUI();
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0003A548 File Offset: 0x00038748
		private void UpdateUI()
		{
			int num = 0;
			ButtonGroup group = new ButtonGroup();
			this.Characters.RemoveAllChildren();
			if (!this._preferencesManager.ServerDataLoaded)
			{
				return;
			}
			this._createNewCharacterButton.ToolTip = Loc.GetString("character-setup-gui-create-new-character-button-tooltip", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("maxCharacters", this._preferencesManager.Settings.MaxCharacterSlots)
			});
			bool flag = false;
			foreach (KeyValuePair<int, ICharacterProfile> keyValuePair in this._preferencesManager.Preferences.Characters)
			{
				int num2;
				ICharacterProfile character2;
				keyValuePair.Deconstruct(out num2, out character2);
				int characterIndexCopy2 = num2;
				ICharacterProfile character = character2;
				if (character != null)
				{
					flag = (num >= this._preferencesManager.Settings.MaxCharacterSlots);
					if (flag)
					{
						break;
					}
					num++;
					CharacterSetupGui.CharacterPickerButton characterPickerButton = new CharacterSetupGui.CharacterPickerButton(this._entityManager, this._preferencesManager, this._prototypeManager, group, character);
					this.Characters.AddChild(characterPickerButton);
					int characterIndexCopy = characterIndexCopy2;
					characterPickerButton.OnPressed += delegate(BaseButton.ButtonEventArgs args)
					{
						this._humanoidProfileEditor.Profile = (HumanoidCharacterProfile)character;
						this._humanoidProfileEditor.CharacterSlot = characterIndexCopy;
						this._humanoidProfileEditor.UpdateControls();
						this._preferencesManager.SelectCharacter(character);
						this.UpdateUI();
						args.Event.Handle();
					};
				}
			}
			this._createNewCharacterButton.Disabled = flag;
			this.Characters.AddChild(this._createNewCharacterButton);
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x0003A6BC File Offset: 0x000388BC
		private PanelContainer BackgroundPanel
		{
			get
			{
				return base.FindControl<PanelContainer>("BackgroundPanel");
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000A1A RID: 2586 RVA: 0x0003A6C9 File Offset: 0x000388C9
		private Button RulesButton
		{
			get
			{
				return base.FindControl<Button>("RulesButton");
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x0003A6D6 File Offset: 0x000388D6
		public Button SaveButton
		{
			get
			{
				return base.FindControl<Button>("SaveButton");
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000A1C RID: 2588 RVA: 0x0003A6E3 File Offset: 0x000388E3
		public Button CloseButton
		{
			get
			{
				return base.FindControl<Button>("CloseButton");
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000A1D RID: 2589 RVA: 0x0003A6F0 File Offset: 0x000388F0
		private BoxContainer Characters
		{
			get
			{
				return base.FindControl<BoxContainer>("Characters");
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000A1E RID: 2590 RVA: 0x0003A6FD File Offset: 0x000388FD
		private BoxContainer CharEditor
		{
			get
			{
				return base.FindControl<BoxContainer>("CharEditor");
			}
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x0003A70C File Offset: 0x0003890C
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Preferences.UI.CharacterSetupGui.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.VerticalExpand = true;
			Control control = new Control();
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.Name = "BackgroundPanel";
			Control control2 = panelContainer;
			context.RobustNameScope.Register("BackgroundPanel", control2);
			control2 = panelContainer;
			control.XamlChildren.Add(control2);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(0);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.MinSize = new Vector2(0f, 40f);
			Label label = new Label();
			label.Text = (string)new LocExtension("character-setup-gui-character-setup-label").ProvideValue();
			label.Margin = new Thickness(8f, 0f, 0f, 0f);
			label.VAlign = 1;
			string item = "LabelHeadingBigger";
			label.StyleClasses.Add(item);
			control2 = label;
			boxContainer2.XamlChildren.Add(control2);
			Button button = new Button();
			button.Name = "RulesButton";
			control2 = button;
			context.RobustNameScope.Register("RulesButton", control2);
			button.HorizontalExpand = true;
			button.Text = (string)new LocExtension("character-setup-gui-character-setup-rules-button").ProvideValue();
			item = "ButtonBig";
			button.StyleClasses.Add(item);
			button.HorizontalAlignment = 3;
			control2 = button;
			boxContainer2.XamlChildren.Add(control2);
			Button button2 = new Button();
			button2.Name = "SaveButton";
			control2 = button2;
			context.RobustNameScope.Register("SaveButton", control2);
			button2.Access = new AccessLevel?(0);
			button2.Text = (string)new LocExtension("character-setup-gui-character-setup-save-button").ProvideValue();
			item = "ButtonBig";
			button2.StyleClasses.Add(item);
			control2 = button2;
			boxContainer2.XamlChildren.Add(control2);
			Button button3 = new Button();
			button3.Name = "CloseButton";
			control2 = button3;
			context.RobustNameScope.Register("CloseButton", control2);
			button3.Access = new AccessLevel?(0);
			button3.Text = (string)new LocExtension("character-setup-gui-character-setup-close-button").ProvideValue();
			item = "ButtonBig";
			button3.StyleClasses.Add(item);
			control2 = button3;
			boxContainer2.XamlChildren.Add(control2);
			control2 = boxContainer2;
			boxContainer.XamlChildren.Add(control2);
			control2 = new PanelContainer
			{
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = StyleNano.NanoGold,
					ContentMarginTopOverride = new float?(2f)
				}
			};
			boxContainer.XamlChildren.Add(control2);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			boxContainer3.VerticalExpand = true;
			boxContainer3.SeparationOverride = new int?(0);
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.MinSize = new Vector2(325f, 0f);
			scrollContainer.Margin = new Thickness(5f, 5f, 0f, 0f);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Name = "Characters";
			control2 = boxContainer4;
			context.RobustNameScope.Register("Characters", control2);
			boxContainer4.Orientation = 1;
			control2 = boxContainer4;
			scrollContainer.XamlChildren.Add(control2);
			control2 = scrollContainer;
			boxContainer3.XamlChildren.Add(control2);
			control2 = new PanelContainer
			{
				MinSize = new Vector2(2f, 0f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = StyleNano.NanoGold,
					ContentMarginTopOverride = new float?(2f)
				}
			};
			boxContainer3.XamlChildren.Add(control2);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Name = "CharEditor";
			control2 = boxContainer5;
			context.RobustNameScope.Register("CharEditor", control2);
			control2 = boxContainer5;
			boxContainer3.XamlChildren.Add(control2);
			control2 = boxContainer3;
			boxContainer.XamlChildren.Add(control2);
			control2 = boxContainer;
			control.XamlChildren.Add(control2);
			control2 = control;
			A_1.XamlChildren.Add(control2);
			if ((control2 = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control2.NameScope);
				control2.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0003ABFA File Offset: 0x00038DFA
		private static void !XamlIlPopulateTrampoline(CharacterSetupGui A_0)
		{
			CharacterSetupGui.Populate:Content.Client.Preferences.UI.CharacterSetupGui.xaml(null, A_0);
		}

		// Token: 0x040004F8 RID: 1272
		[Nullable(1)]
		private readonly IClientPreferencesManager _preferencesManager;

		// Token: 0x040004F9 RID: 1273
		[Nullable(1)]
		private readonly IEntityManager _entityManager;

		// Token: 0x040004FA RID: 1274
		[Nullable(1)]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040004FB RID: 1275
		[Nullable(1)]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040004FC RID: 1276
		[Nullable(1)]
		private readonly Button _createNewCharacterButton;

		// Token: 0x040004FD RID: 1277
		[Nullable(1)]
		private readonly HumanoidProfileEditor _humanoidProfileEditor;

		// Token: 0x02000184 RID: 388
		private sealed class CharacterPickerButton : ContainerButton
		{
			// Token: 0x06000A21 RID: 2593 RVA: 0x0003AC04 File Offset: 0x00038E04
			[NullableContext(1)]
			public CharacterPickerButton(IEntityManager entityManager, IClientPreferencesManager preferencesManager, IPrototypeManager prototypeManager, ButtonGroup group, ICharacterProfile profile)
			{
				CharacterSetupGui.CharacterPickerButton <>4__this = this;
				base.AddStyleClass("button");
				base.ToggleMode = true;
				base.Group = group;
				HumanoidCharacterProfile humanoidCharacterProfile = profile as HumanoidCharacterProfile;
				if (humanoidCharacterProfile != null)
				{
					string dollPrototype = prototypeManager.Index<SpeciesPrototype>(humanoidCharacterProfile.Species).DollPrototype;
					this._previewDummy = entityManager.SpawnEntity(dollPrototype, MapCoordinates.Nullspace);
				}
				else
				{
					this._previewDummy = entityManager.SpawnEntity(prototypeManager.Index<SpeciesPrototype>("Human").DollPrototype, MapCoordinates.Nullspace);
				}
				EntitySystem.Get<HumanoidAppearanceSystem>().LoadProfile(this._previewDummy, (HumanoidCharacterProfile)profile, null);
				if (humanoidCharacterProfile != null)
				{
					LobbyCharacterPreviewPanel.GiveDummyJobClothes(this._previewDummy, humanoidCharacterProfile);
				}
				ICharacterProfile profile2 = profile;
				PlayerPreferences preferences = preferencesManager.Preferences;
				bool flag = profile2 == ((preferences != null) ? preferences.SelectedCharacter : null);
				if (flag)
				{
					base.Pressed = true;
				}
				SpriteView spriteView = new SpriteView
				{
					Sprite = entityManager.GetComponent<SpriteComponent>(this._previewDummy),
					Scale = new ValueTuple<float, float>(2f, 2f),
					OverrideDirection = new Direction?(0)
				};
				string text = profile.Name;
				string text2;
				if (humanoidCharacterProfile == null)
				{
					text2 = null;
				}
				else
				{
					text2 = humanoidCharacterProfile.JobPriorities.SingleOrDefault((KeyValuePair<string, JobPriority> p) => p.Value == JobPriority.High).Key;
				}
				string text3 = text2;
				if (text3 != null)
				{
					string localizedName = IoCManager.Resolve<IPrototypeManager>().Index<JobPrototype>(text3).LocalizedName;
					text = text + "\n" + localizedName;
				}
				Label label = new Label
				{
					Text = text,
					ClipText = true,
					HorizontalExpand = true
				};
				Button deleteButton = new Button
				{
					Text = Loc.GetString("character-setup-gui-character-picker-button-delete-button"),
					Visible = !flag
				};
				Button confirmDeleteButton = new Button
				{
					Text = Loc.GetString("character-setup-gui-character-picker-button-confirm-delete-button"),
					Visible = false
				};
				confirmDeleteButton.ModulateSelfOverride = new Color?(StyleNano.ButtonColorCautionDefault);
				confirmDeleteButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					Control parent = <>4__this.Parent;
					if (parent != null)
					{
						parent.RemoveChild(<>4__this);
					}
					Control parent2 = <>4__this.Parent;
					if (parent2 != null)
					{
						parent2.RemoveChild(confirmDeleteButton);
					}
					preferencesManager.DeleteCharacter(profile);
				};
				deleteButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					deleteButton.Visible = false;
					confirmDeleteButton.Visible = true;
				};
				deleteButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					deleteButton.Visible = false;
					confirmDeleteButton.Visible = true;
				};
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 0;
				boxContainer.HorizontalExpand = true;
				boxContainer.SeparationOverride = new int?(0);
				boxContainer.Children.Add(spriteView);
				boxContainer.Children.Add(label);
				boxContainer.Children.Add(deleteButton);
				boxContainer.Children.Add(confirmDeleteButton);
				BoxContainer boxContainer2 = boxContainer;
				base.AddChild(boxContainer2);
			}

			// Token: 0x06000A22 RID: 2594 RVA: 0x0003AEC9 File Offset: 0x000390C9
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (!disposing)
				{
					return;
				}
				IoCManager.Resolve<IEntityManager>().DeleteEntity(this._previewDummy);
				this._previewDummy = default(EntityUid);
			}

			// Token: 0x040004FE RID: 1278
			private EntityUid _previewDummy;
		}
	}
}
