using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Humanoid
{
	// Token: 0x020002CF RID: 719
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HumanoidAppearanceSystem : SharedHumanoidAppearanceSystem
	{
		// Token: 0x060011F6 RID: 4598 RVA: 0x0006AA7D File Offset: 0x00068C7D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentHandleState>(new ComponentEventRefHandler<HumanoidAppearanceComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x0006AA9C File Offset: 0x00068C9C
		private void OnHandleState(EntityUid uid, HumanoidAppearanceComponent component, ref ComponentHandleState args)
		{
			HumanoidAppearanceState humanoidAppearanceState = args.Current as HumanoidAppearanceState;
			if (humanoidAppearanceState == null)
			{
				return;
			}
			this.ApplyState(uid, component, base.Comp<SpriteComponent>(uid), humanoidAppearanceState);
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x0006AACC File Offset: 0x00068CCC
		private void ApplyState(EntityUid uid, HumanoidAppearanceComponent component, SpriteComponent sprite, HumanoidAppearanceState state)
		{
			component.Sex = state.Sex;
			component.Species = state.Species;
			component.Age = state.Age;
			component.SkinColor = state.SkinColor;
			component.EyeColor = state.EyeColor;
			component.HiddenLayers = new HashSet<HumanoidVisualLayers>(state.HiddenLayers);
			component.PermanentlyHidden = new HashSet<HumanoidVisualLayers>(state.PermanentlyHidden);
			component.CustomBaseLayers = Extensions.ShallowClone<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>(state.CustomBaseLayers);
			this.UpdateLayers(component, sprite);
			this.ApplyMarkingSet(uid, state.Markings, component, sprite);
			sprite[sprite.LayerMapReserveBlank(HumanoidVisualLayers.Eyes)].Color = state.EyeColor;
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x0006AB86 File Offset: 0x00068D86
		private static bool IsHidden(HumanoidAppearanceComponent humanoid, HumanoidVisualLayers layer)
		{
			return humanoid.HiddenLayers.Contains(layer) || humanoid.PermanentlyHidden.Contains(layer);
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x0006ABA4 File Offset: 0x00068DA4
		private void UpdateLayers(HumanoidAppearanceComponent component, SpriteComponent sprite)
		{
			HashSet<HumanoidVisualLayers> hashSet = new HashSet<HumanoidVisualLayers>(component.BaseLayers.Keys);
			component.BaseLayers.Clear();
			SpeciesPrototype speciesPrototype = this._prototypeManager.Index<SpeciesPrototype>(component.Species);
			foreach (KeyValuePair<HumanoidVisualLayers, string> keyValuePair in this._prototypeManager.Index<HumanoidSpeciesBaseSpritesPrototype>(speciesPrototype.SpriteSet).Sprites)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				string text;
				keyValuePair.Deconstruct(out humanoidVisualLayers, out text);
				HumanoidVisualLayers humanoidVisualLayers2 = humanoidVisualLayers;
				string protoId = text;
				hashSet.Remove(humanoidVisualLayers2);
				if (!component.CustomBaseLayers.ContainsKey(humanoidVisualLayers2))
				{
					this.SetLayerData(component, sprite, humanoidVisualLayers2, protoId, true, null);
				}
			}
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> keyValuePair2 in component.CustomBaseLayers)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo customBaseLayerInfo;
				keyValuePair2.Deconstruct(out humanoidVisualLayers, out customBaseLayerInfo);
				HumanoidVisualLayers humanoidVisualLayers3 = humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo customBaseLayerInfo2 = customBaseLayerInfo;
				hashSet.Remove(humanoidVisualLayers3);
				this.SetLayerData(component, sprite, humanoidVisualLayers3, customBaseLayerInfo2.ID, false, customBaseLayerInfo2.Color);
			}
			foreach (HumanoidVisualLayers humanoidVisualLayers4 in hashSet)
			{
				int num;
				if (sprite.LayerMapTryGet(humanoidVisualLayers4, ref num, false))
				{
					sprite[num].Visible = false;
				}
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0006AD38 File Offset: 0x00068F38
		private void SetLayerData(HumanoidAppearanceComponent component, SpriteComponent sprite, HumanoidVisualLayers key, [Nullable(2)] string protoId, bool sexMorph = false, Color? color = null)
		{
			int num = sprite.LayerMapReserveBlank(key);
			ISpriteLayer spriteLayer = sprite[num];
			spriteLayer.Visible = !HumanoidAppearanceSystem.IsHidden(component, key);
			if (color != null)
			{
				spriteLayer.Color = color.Value;
			}
			if (protoId == null)
			{
				return;
			}
			if (sexMorph)
			{
				protoId = HumanoidVisualLayersExtension.GetSexMorph(key, component.Sex, protoId);
			}
			HumanoidSpeciesSpriteLayer humanoidSpeciesSpriteLayer = this._prototypeManager.Index<HumanoidSpeciesSpriteLayer>(protoId);
			component.BaseLayers[key] = humanoidSpeciesSpriteLayer;
			if (humanoidSpeciesSpriteLayer.MatchSkin)
			{
				spriteLayer.Color = component.SkinColor.WithAlpha(humanoidSpeciesSpriteLayer.LayerAlpha);
			}
			if (humanoidSpeciesSpriteLayer.BaseSprite != null)
			{
				sprite.LayerSetSprite(num, humanoidSpeciesSpriteLayer.BaseSprite);
			}
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x0006ADEC File Offset: 0x00068FEC
		public void LoadProfile(EntityUid uid, HumanoidCharacterProfile profile, [Nullable(2)] HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
			{
				return;
			}
			Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> customBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>();
			SpeciesPrototype speciesPrototype = this._prototypeManager.Index<SpeciesPrototype>(profile.Species);
			MarkingSet markingSet = new MarkingSet(profile.Appearance.Markings, speciesPrototype.MarkingPoints, this._markingManager, this._prototypeManager);
			markingSet.EnsureDefault(new Color?(profile.Appearance.SkinColor), this._markingManager);
			markingSet.RemoveCategory(MarkingCategories.Hair);
			markingSet.RemoveCategory(MarkingCategories.FacialHair);
			Marking marking = new Marking(profile.Appearance.HairStyleId, new Color[]
			{
				profile.Appearance.HairColor
			});
			markingSet.AddBack(MarkingCategories.Hair, marking);
			Marking marking2 = new Marking(profile.Appearance.FacialHairStyleId, new Color[]
			{
				profile.Appearance.FacialHairColor
			});
			markingSet.AddBack(MarkingCategories.FacialHair, marking2);
			markingSet.FilterSpecies(profile.Species, this._markingManager, this._prototypeManager);
			HumanoidAppearanceState state = new HumanoidAppearanceState(markingSet, new HashSet<HumanoidVisualLayers>(), new HashSet<HumanoidVisualLayers>(), customBaseLayers, profile.Sex, profile.Gender, profile.Age, profile.Species, profile.Appearance.SkinColor, profile.Appearance.EyeColor);
			this.ApplyState(uid, humanoid, base.Comp<SpriteComponent>(uid), state);
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x0006AF3C File Offset: 0x0006913C
		private void ApplyMarkingSet(EntityUid uid, MarkingSet newMarkings, HumanoidAppearanceComponent humanoid, SpriteComponent sprite)
		{
			if (humanoid.MarkingSet.Markings.Count == 0 && newMarkings.Markings.Count == 0)
			{
				return;
			}
			this.ClearAllMarkings(uid, humanoid, sprite);
			humanoid.MarkingSet = new MarkingSet(newMarkings);
			foreach (List<Marking> list in humanoid.MarkingSet.Markings.Values)
			{
				foreach (Marking marking in list)
				{
					MarkingPrototype markingPrototype;
					if (this._markingManager.TryGetMarking(marking, out markingPrototype))
					{
						this.ApplyMarking(uid, markingPrototype, marking.MarkingColors, marking.Visible, humanoid, sprite);
					}
				}
			}
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x0006B024 File Offset: 0x00069224
		private void ClearAllMarkings(EntityUid uid, HumanoidAppearanceComponent humanoid, SpriteComponent sprite)
		{
			foreach (List<Marking> list in humanoid.MarkingSet.Markings.Values)
			{
				foreach (Marking marking in list)
				{
					this.RemoveMarking(uid, marking, sprite);
				}
			}
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x0006B0B8 File Offset: 0x000692B8
		private void ClearMarkings(EntityUid uid, List<Marking> markings, HumanoidAppearanceComponent humanoid, SpriteComponent spriteComp)
		{
			foreach (Marking marking in markings)
			{
				this.RemoveMarking(uid, marking, spriteComp);
			}
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0006B10C File Offset: 0x0006930C
		private void RemoveMarking(EntityUid uid, Marking marking, SpriteComponent spriteComp)
		{
			MarkingPrototype markingPrototype;
			if (!this._markingManager.TryGetMarking(marking, out markingPrototype))
			{
				return;
			}
			foreach (SpriteSpecifier spriteSpecifier in markingPrototype.Sprites)
			{
				SpriteSpecifier.Rsi rsi = spriteSpecifier as SpriteSpecifier.Rsi;
				if (rsi != null)
				{
					string text = marking.MarkingId + "-" + rsi.RsiState;
					int num;
					if (spriteComp.LayerMapTryGet(text, ref num, false))
					{
						spriteComp.LayerMapRemove(text);
						spriteComp.RemoveLayer(num);
					}
				}
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0006B1A4 File Offset: 0x000693A4
		private void ApplyMarking(EntityUid uid, MarkingPrototype markingPrototype, [Nullable(2)] IReadOnlyList<Color> colors, bool visible, HumanoidAppearanceComponent humanoid, SpriteComponent sprite)
		{
			int num;
			if (!sprite.LayerMapTryGet(markingPrototype.BodyPart, ref num, false))
			{
				return;
			}
			visible &= !HumanoidAppearanceSystem.IsHidden(humanoid, markingPrototype.BodyPart);
			HumanoidSpeciesSpriteLayer humanoidSpeciesSpriteLayer;
			visible &= (humanoid.BaseLayers.TryGetValue(markingPrototype.BodyPart, out humanoidSpeciesSpriteLayer) && humanoidSpeciesSpriteLayer.AllowsMarkings);
			for (int i = 0; i < markingPrototype.Sprites.Count; i++)
			{
				SpriteSpecifier.Rsi rsi = markingPrototype.Sprites[i] as SpriteSpecifier.Rsi;
				if (rsi != null)
				{
					string text = markingPrototype.ID + "-" + rsi.RsiState;
					int num2;
					if (!sprite.LayerMapTryGet(text, ref num2, false))
					{
						int num3 = sprite.AddLayer(markingPrototype.Sprites[i], new int?(num + i + 1));
						sprite.LayerMapSet(text, num3);
						sprite.LayerSetSprite(text, rsi);
					}
					sprite.LayerSetVisible(text, visible);
					if (visible && humanoidSpeciesSpriteLayer != null)
					{
						if (markingPrototype.FollowSkinColor || colors == null || humanoidSpeciesSpriteLayer.MarkingsMatchSkin)
						{
							Color skinColor = humanoid.SkinColor;
							skinColor.A = humanoidSpeciesSpriteLayer.LayerAlpha;
							sprite.LayerSetColor(text, skinColor);
						}
						else
						{
							sprite.LayerSetColor(text, colors[i]);
						}
					}
				}
			}
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x0006B2E8 File Offset: 0x000694E8
		[NullableContext(2)]
		public override void SetSkinColor(EntityUid uid, Color skinColor, bool sync = true, HumanoidAppearanceComponent humanoid = null)
		{
			if (!base.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || humanoid.SkinColor == skinColor)
			{
				return;
			}
			humanoid.SkinColor = skinColor;
			if (sync)
			{
				base.Dirty(humanoid, null);
			}
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> keyValuePair in humanoid.BaseLayers)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				HumanoidSpeciesSpriteLayer humanoidSpeciesSpriteLayer;
				keyValuePair.Deconstruct(out humanoidVisualLayers, out humanoidSpeciesSpriteLayer);
				HumanoidVisualLayers humanoidVisualLayers2 = humanoidVisualLayers;
				HumanoidSpeciesSpriteLayer humanoidSpeciesSpriteLayer2 = humanoidSpeciesSpriteLayer;
				if (humanoidSpeciesSpriteLayer2.MatchSkin)
				{
					int num = spriteComponent.LayerMapReserveBlank(humanoidVisualLayers2);
					spriteComponent[num].Color = skinColor.WithAlpha(humanoidSpeciesSpriteLayer2.LayerAlpha);
				}
			}
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0006B3B4 File Offset: 0x000695B4
		protected override void SetLayerVisibility(EntityUid uid, HumanoidAppearanceComponent humanoid, HumanoidVisualLayers layer, bool visible, bool permanent, ref bool dirty)
		{
			base.SetLayerVisibility(uid, humanoid, layer, visible, permanent, ref dirty);
			SpriteComponent spriteComponent = base.Comp<SpriteComponent>(uid);
			int num;
			if (!spriteComponent.LayerMapTryGet(layer, ref num, false))
			{
				if (!visible)
				{
					return;
				}
				num = spriteComponent.LayerMapReserveBlank(layer);
			}
			ISpriteLayer spriteLayer = spriteComponent[num];
			if (spriteLayer.Visible == visible)
			{
				return;
			}
			spriteLayer.Visible = visible;
			foreach (List<Marking> list in humanoid.MarkingSet.Markings.Values)
			{
				foreach (Marking marking in list)
				{
					MarkingPrototype markingPrototype;
					if (this._markingManager.TryGetMarking(marking, out markingPrototype) && markingPrototype.BodyPart == layer)
					{
						this.ApplyMarking(uid, markingPrototype, marking.MarkingColors, marking.Visible, humanoid, spriteComponent);
					}
				}
			}
		}

		// Token: 0x040008E8 RID: 2280
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040008E9 RID: 2281
		[Dependency]
		private readonly MarkingManager _markingManager;
	}
}
