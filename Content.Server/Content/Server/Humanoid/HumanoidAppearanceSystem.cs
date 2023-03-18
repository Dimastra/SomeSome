using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.White.TTS;
using Content.Shared.Administration;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.IdentityManagement;
using Content.Shared.Preferences;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Humanoid
{
	// Token: 0x02000459 RID: 1113
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HumanoidAppearanceSystem : SharedHumanoidAppearanceSystem
	{
		// Token: 0x06001673 RID: 5747 RVA: 0x00076744 File Offset: 0x00074944
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentInit>(new ComponentEventHandler<HumanoidAppearanceComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, HumanoidMarkingModifierMarkingSetMessage>(new ComponentEventHandler<HumanoidAppearanceComponent, HumanoidMarkingModifierMarkingSetMessage>(this.OnMarkingsSet), null, null);
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, HumanoidMarkingModifierBaseLayersSetMessage>(new ComponentEventHandler<HumanoidAppearanceComponent, HumanoidMarkingModifierBaseLayersSetMessage>(this.OnBaseLayersSet), null, null);
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<HumanoidAppearanceComponent, GetVerbsEvent<Verb>>(this.OnVerbsRequest), null, null);
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, ExaminedEvent>(new ComponentEventHandler<HumanoidAppearanceComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x000767BC File Offset: 0x000749BC
		private void OnInit(EntityUid uid, HumanoidAppearanceComponent humanoid, ComponentInit args)
		{
			if (string.IsNullOrEmpty(humanoid.Species))
			{
				return;
			}
			HumanoidProfilePrototype startingSet;
			if (string.IsNullOrEmpty(humanoid.Initial) || !this._prototypeManager.TryIndex<HumanoidProfilePrototype>(humanoid.Initial, ref startingSet))
			{
				this.LoadProfile(uid, HumanoidCharacterProfile.DefaultWithSpecies(humanoid.Species), humanoid);
				return;
			}
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> keyValuePair in startingSet.CustomBaseLayers)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo customBaseLayerInfo;
				keyValuePair.Deconstruct(out humanoidVisualLayers, out customBaseLayerInfo);
				HumanoidVisualLayers layer = humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo info = customBaseLayerInfo;
				humanoid.CustomBaseLayers.Add(layer, info);
			}
			this.LoadProfile(uid, startingSet.Profile, humanoid);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0007687C File Offset: 0x00074A7C
		private void OnExamined(EntityUid uid, HumanoidAppearanceComponent component, ExaminedEvent args)
		{
			EntityUid identity = Identity.Entity(component.Owner, this.EntityManager);
			string species = this.GetSpeciesRepresentation(component.Species).ToLower();
			string age = this.GetAgeRepresentation(component.Species, component.Age);
			args.PushText(Loc.GetString("humanoid-appearance-component-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", identity),
				new ValueTuple<string, object>("age", age),
				new ValueTuple<string, object>("species", species)
			}));
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x00076914 File Offset: 0x00074B14
		public void LoadProfile(EntityUid uid, HumanoidCharacterProfile profile, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			base.SetSpecies(uid, profile.Species, false, humanoid);
			base.SetSex(uid, profile.Sex, false, humanoid);
			humanoid.EyeColor = profile.Appearance.EyeColor;
			this.SetSkinColor(uid, profile.Appearance.SkinColor, false, null);
			humanoid.MarkingSet.Clear();
			this.AddMarking(uid, profile.Appearance.HairStyleId, new Color?(profile.Appearance.HairColor), false, false, null);
			this.AddMarking(uid, profile.Appearance.FacialHairStyleId, new Color?(profile.Appearance.FacialHairColor), false, false, null);
			foreach (Marking marking in profile.Appearance.Markings)
			{
				this.AddMarking(uid, marking.MarkingId, marking.MarkingColors, false, false, null);
			}
			this.EnsureDefaultMarkings(uid, humanoid);
			this.SetTTSVoice(uid, profile.Voice, humanoid);
			humanoid.Gender = profile.Gender;
			GrammarComponent grammar;
			if (base.TryComp<GrammarComponent>(uid, ref grammar))
			{
				grammar.Gender = new Gender?(profile.Gender);
			}
			humanoid.Age = profile.Age;
			base.Dirty(humanoid, null);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x00076A74 File Offset: 0x00074C74
		[NullableContext(2)]
		public void CloneAppearance(EntityUid source, EntityUid target, HumanoidAppearanceComponent sourceHumanoid = null, HumanoidAppearanceComponent targetHumanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(source, ref sourceHumanoid, true) || !base.Resolve<HumanoidAppearanceComponent>(target, ref targetHumanoid, true))
			{
				return;
			}
			targetHumanoid.Species = sourceHumanoid.Species;
			targetHumanoid.SkinColor = sourceHumanoid.SkinColor;
			base.SetSex(target, sourceHumanoid.Sex, false, targetHumanoid);
			targetHumanoid.CustomBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>(sourceHumanoid.CustomBaseLayers);
			targetHumanoid.MarkingSet = new MarkingSet(sourceHumanoid.MarkingSet);
			this.SetTTSVoice(target, sourceHumanoid.Voice, targetHumanoid);
			targetHumanoid.Gender = sourceHumanoid.Gender;
			GrammarComponent grammar;
			if (base.TryComp<GrammarComponent>(target, ref grammar))
			{
				grammar.Gender = new Gender?(sourceHumanoid.Gender);
			}
			base.Dirty(targetHumanoid, null);
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00076B2C File Offset: 0x00074D2C
		public void AddMarking(EntityUid uid, string marking, Color? color = null, bool sync = true, bool forced = false, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			MarkingPrototype prototype;
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !this._markingManager.Markings.TryGetValue(marking, out prototype))
			{
				return;
			}
			Marking markingObject = prototype.AsMarking();
			markingObject.Forced = forced;
			if (color != null)
			{
				for (int i = 0; i < prototype.Sprites.Count; i++)
				{
					markingObject.SetColor(i, color.Value);
				}
			}
			humanoid.MarkingSet.AddBack(prototype.MarkingCategory, markingObject);
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00076BB8 File Offset: 0x00074DB8
		public void AddMarking(EntityUid uid, string marking, IReadOnlyList<Color> colors, bool sync = true, bool forced = false, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			MarkingPrototype prototype;
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !this._markingManager.Markings.TryGetValue(marking, out prototype))
			{
				return;
			}
			Marking markingObject = new Marking(marking, colors);
			markingObject.Forced = forced;
			humanoid.MarkingSet.AddBack(prototype.MarkingCategory, markingObject);
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00076C18 File Offset: 0x00074E18
		public void RemoveMarking(EntityUid uid, string marking, bool sync = true, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			MarkingPrototype prototype;
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !this._markingManager.Markings.TryGetValue(marking, out prototype))
			{
				return;
			}
			humanoid.MarkingSet.Remove(prototype.MarkingCategory, marking);
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00076C68 File Offset: 0x00074E68
		[NullableContext(2)]
		public void RemoveMarking(EntityUid uid, MarkingCategories category, int index, HumanoidAppearanceComponent humanoid = null)
		{
			IReadOnlyList<Marking> markings;
			if (index < 0 || !base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !humanoid.MarkingSet.TryGetCategory(category, out markings) || index >= markings.Count)
			{
				return;
			}
			humanoid.MarkingSet.Remove(category, index);
			base.Dirty(humanoid, null);
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x00076CB8 File Offset: 0x00074EB8
		public void SetMarkingId(EntityUid uid, MarkingCategories category, int index, string markingId, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			MarkingPrototype markingPrototype;
			IReadOnlyList<Marking> markings;
			if (index < 0 || !this._markingManager.MarkingsByCategory(category).TryGetValue(markingId, out markingPrototype) || !base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !humanoid.MarkingSet.TryGetCategory(category, out markings) || index >= markings.Count)
			{
				return;
			}
			Marking marking = markingPrototype.AsMarking();
			int i = 0;
			while (i < marking.MarkingColors.Count && i < markings[index].MarkingColors.Count)
			{
				marking.SetColor(i, markings[index].MarkingColors[i]);
				i++;
			}
			humanoid.MarkingSet.Replace(category, index, marking);
			base.Dirty(humanoid, null);
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00076D6C File Offset: 0x00074F6C
		public void SetMarkingColor(EntityUid uid, MarkingCategories category, int index, List<Color> colors, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			IReadOnlyList<Marking> markings;
			if (index < 0 || !base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !humanoid.MarkingSet.TryGetCategory(category, out markings) || index >= markings.Count)
			{
				return;
			}
			int i = 0;
			while (i < markings[index].MarkingColors.Count && i < colors.Count)
			{
				markings[index].SetColor(i, colors[i]);
				i++;
			}
			base.Dirty(humanoid, null);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00076DE8 File Offset: 0x00074FE8
		public string GetSpeciesRepresentation(string speciesId)
		{
			SpeciesPrototype species;
			if (this._prototypeManager.TryIndex<SpeciesPrototype>(speciesId, ref species))
			{
				return Loc.GetString(species.Name);
			}
			return Loc.GetString("humanoid-appearance-component-unknown-species");
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x00076E1C File Offset: 0x0007501C
		public string GetAgeRepresentation(string species, int age)
		{
			SpeciesPrototype speciesPrototype;
			this._prototypeManager.TryIndex<SpeciesPrototype>(species, ref speciesPrototype);
			if (speciesPrototype == null)
			{
				Logger.Error("Tried to get age representation of species that couldn't be indexed: " + species);
				return Loc.GetString("identity-age-young");
			}
			if (age < speciesPrototype.YoungAge)
			{
				return Loc.GetString("identity-age-young");
			}
			if (age < speciesPrototype.OldAge)
			{
				return Loc.GetString("identity-age-middle-aged");
			}
			return Loc.GetString("identity-age-old");
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00076E88 File Offset: 0x00075088
		[NullableContext(2)]
		private void EnsureDefaultMarkings(EntityUid uid, HumanoidAppearanceComponent humanoid)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			humanoid.MarkingSet.EnsureDefault(new Color?(humanoid.SkinColor), this._markingManager);
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00076EB4 File Offset: 0x000750B4
		private void OnVerbsRequest(EntityUid uid, HumanoidAppearanceComponent component, GetVerbsEvent<Verb> args)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (!this._adminManager.HasAdminFlag(actor.PlayerSession, AdminFlags.Fun))
			{
				return;
			}
			args.Verbs.Add(new Verb
			{
				Text = "Modify markings",
				Category = VerbCategory.Tricks,
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Mobs/Customization/reptilian_parts.rsi", "/"), "tail_smooth"),
				Act = delegate()
				{
					this._uiSystem.TryOpen(uid, HumanoidMarkingModifierKey.Key, actor.PlayerSession, null);
					this._uiSystem.TrySetUiState(uid, HumanoidMarkingModifierKey.Key, new HumanoidMarkingModifierState(component.MarkingSet, component.Species, component.SkinColor, component.CustomBaseLayers), null, null, true);
				}
			});
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00076F64 File Offset: 0x00075164
		private void OnBaseLayersSet(EntityUid uid, HumanoidAppearanceComponent component, HumanoidMarkingModifierBaseLayersSetMessage message)
		{
			IPlayerSession player = message.Session as IPlayerSession;
			if (player == null || !this._adminManager.HasAdminFlag(player, AdminFlags.Fun))
			{
				return;
			}
			if (message.Info == null)
			{
				component.CustomBaseLayers.Remove(message.Layer);
			}
			else
			{
				component.CustomBaseLayers[message.Layer] = message.Info.Value;
			}
			base.Dirty(component, null);
			if (message.ResendState)
			{
				this._uiSystem.TrySetUiState(uid, HumanoidMarkingModifierKey.Key, new HumanoidMarkingModifierState(component.MarkingSet, component.Species, component.SkinColor, component.CustomBaseLayers), null, null, true);
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00077018 File Offset: 0x00075218
		private void OnMarkingsSet(EntityUid uid, HumanoidAppearanceComponent component, HumanoidMarkingModifierMarkingSetMessage message)
		{
			IPlayerSession player = message.Session as IPlayerSession;
			if (player == null || !this._adminManager.HasAdminFlag(player, AdminFlags.Fun))
			{
				return;
			}
			component.MarkingSet = message.MarkingSet;
			base.Dirty(component, null);
			if (message.ResendState)
			{
				this._uiSystem.TrySetUiState(uid, HumanoidMarkingModifierKey.Key, new HumanoidMarkingModifierState(component.MarkingSet, component.Species, component.SkinColor, component.CustomBaseLayers), null, null, true);
			}
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00077094 File Offset: 0x00075294
		public void SetTTSVoice(EntityUid uid, string voiceId, HumanoidAppearanceComponent humanoid)
		{
			TTSComponent comp;
			if (!base.TryComp<TTSComponent>(uid, ref comp))
			{
				return;
			}
			humanoid.Voice = voiceId;
			comp.VoicePrototypeId = voiceId;
		}

		// Token: 0x04000E0C RID: 3596
		[Dependency]
		private readonly MarkingManager _markingManager;

		// Token: 0x04000E0D RID: 3597
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000E0E RID: 3598
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04000E0F RID: 3599
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
