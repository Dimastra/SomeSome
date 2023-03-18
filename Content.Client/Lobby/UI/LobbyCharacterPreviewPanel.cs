using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Humanoid;
using Content.Client.Inventory;
using Content.Client.Preferences;
using Content.Client.UserInterface.Controls;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI
{
	// Token: 0x0200025B RID: 603
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LobbyCharacterPreviewPanel : Control
	{
		// Token: 0x06000F6C RID: 3948 RVA: 0x0005C764 File Offset: 0x0005A964
		public LobbyCharacterPreviewPanel()
		{
			IoCManager.InjectDependencies<LobbyCharacterPreviewPanel>(this);
			NanoHeading nanoHeading = new NanoHeading
			{
				Text = Loc.GetString("lobby-character-preview-panel-header")
			};
			this.CharacterSetupButton = new Button
			{
				Text = Loc.GetString("lobby-character-preview-panel-character-setup-button"),
				HorizontalAlignment = 1
			};
			this._summaryLabel = new Label();
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1
			};
			this._unloaded = new Label
			{
				Text = Loc.GetString("lobby-character-preview-panel-unloaded-preferences-label")
			};
			this._loaded = new BoxContainer
			{
				Orientation = 1,
				Visible = false
			};
			this._viewBox = new BoxContainer
			{
				Orientation = 0
			};
			VSpacer vspacer = new VSpacer();
			this._loaded.AddChild(this._summaryLabel);
			this._loaded.AddChild(this._viewBox);
			this._loaded.AddChild(vspacer);
			this._loaded.AddChild(this.CharacterSetupButton);
			boxContainer.AddChild(nanoHeading);
			boxContainer.AddChild(this._loaded);
			boxContainer.AddChild(this._unloaded);
			base.AddChild(boxContainer);
			this.UpdateUI();
			this._preferencesManager.OnServerDataLoaded += this.UpdateUI;
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000F6D RID: 3949 RVA: 0x0005C89C File Offset: 0x0005AA9C
		public Button CharacterSetupButton { get; }

		// Token: 0x06000F6E RID: 3950 RVA: 0x0005C8A4 File Offset: 0x0005AAA4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this._preferencesManager.OnServerDataLoaded -= this.UpdateUI;
			if (!disposing)
			{
				return;
			}
			if (this._previewDummy != null)
			{
				this._entityManager.DeleteEntity(this._previewDummy.Value);
			}
			this._previewDummy = null;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0005C902 File Offset: 0x0005AB02
		private SpriteView MakeSpriteView(EntityUid entity, Direction direction)
		{
			return new SpriteView
			{
				Sprite = this._entityManager.GetComponent<SpriteComponent>(entity),
				OverrideDirection = new Direction?(direction),
				Scale = new ValueTuple<float, float>(2f, 2f)
			};
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0005C944 File Offset: 0x0005AB44
		public void UpdateUI()
		{
			if (!this._preferencesManager.ServerDataLoaded)
			{
				this._loaded.Visible = false;
				this._unloaded.Visible = true;
				return;
			}
			this._loaded.Visible = true;
			this._unloaded.Visible = false;
			PlayerPreferences preferences = this._preferencesManager.Preferences;
			HumanoidCharacterProfile humanoidCharacterProfile = ((preferences != null) ? preferences.SelectedCharacter : null) as HumanoidCharacterProfile;
			if (humanoidCharacterProfile == null)
			{
				this._summaryLabel.Text = string.Empty;
				return;
			}
			this._previewDummy = new EntityUid?(this._entityManager.SpawnEntity(this._prototypeManager.Index<SpeciesPrototype>(humanoidCharacterProfile.Species).DollPrototype, MapCoordinates.Nullspace));
			SpriteView spriteView = this.MakeSpriteView(this._previewDummy.Value, 0);
			SpriteView spriteView2 = this.MakeSpriteView(this._previewDummy.Value, 4);
			SpriteView spriteView3 = this.MakeSpriteView(this._previewDummy.Value, 6);
			SpriteView spriteView4 = this.MakeSpriteView(this._previewDummy.Value, 2);
			this._viewBox.DisposeAllChildren();
			this._viewBox.AddChild(spriteView);
			this._viewBox.AddChild(spriteView2);
			this._viewBox.AddChild(spriteView3);
			this._viewBox.AddChild(spriteView4);
			this._summaryLabel.Text = humanoidCharacterProfile.Summary;
			EntitySystem.Get<HumanoidAppearanceSystem>().LoadProfile(this._previewDummy.Value, humanoidCharacterProfile, null);
			LobbyCharacterPreviewPanel.GiveDummyJobClothes(this._previewDummy.Value, humanoidCharacterProfile);
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x0005CAB4 File Offset: 0x0005ACB4
		public static void GiveDummyJobClothes(EntityUid dummy, HumanoidCharacterProfile profile)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ClientInventorySystem clientInventorySystem = EntitySystem.Get<ClientInventorySystem>();
			string key = profile.JobPriorities.FirstOrDefault((KeyValuePair<string, JobPriority> p) => p.Value == JobPriority.High).Key;
			JobPrototype jobPrototype = prototypeManager.Index<JobPrototype>(key ?? "Passenger");
			SlotDefinition[] array;
			if (jobPrototype.StartingGear != null && clientInventorySystem.TryGetSlots(dummy, out array, null))
			{
				StartingGearPrototype startingGearPrototype = prototypeManager.Index<StartingGearPrototype>(jobPrototype.StartingGear);
				foreach (SlotDefinition slotDefinition in array)
				{
					string gear = startingGearPrototype.GetGear(slotDefinition.Name, profile);
					EntityUid? entityUid;
					if (clientInventorySystem.TryUnequip(dummy, slotDefinition.Name, out entityUid, true, true, false, null, null))
					{
						entityManager.DeleteEntity(entityUid.Value);
					}
					if (gear != string.Empty)
					{
						EntityUid itemUid = entityManager.SpawnEntity(gear, MapCoordinates.Nullspace);
						clientInventorySystem.TryEquip(dummy, itemUid, slotDefinition.Name, true, true, false, null, null);
					}
				}
			}
		}

		// Token: 0x040007A5 RID: 1957
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040007A6 RID: 1958
		[Dependency]
		private readonly IClientPreferencesManager _preferencesManager;

		// Token: 0x040007A7 RID: 1959
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040007A8 RID: 1960
		private EntityUid? _previewDummy;

		// Token: 0x040007A9 RID: 1961
		private readonly Label _summaryLabel;

		// Token: 0x040007AA RID: 1962
		private readonly BoxContainer _loaded;

		// Token: 0x040007AB RID: 1963
		private readonly BoxContainer _viewBox;

		// Token: 0x040007AC RID: 1964
		private readonly Label _unloaded;
	}
}
