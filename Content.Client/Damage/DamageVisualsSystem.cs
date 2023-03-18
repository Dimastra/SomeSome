using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Damage
{
	// Token: 0x02000366 RID: 870
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DamageVisualsSystem : VisualizerSystem<DamageVisualsComponent>
	{
		// Token: 0x06001574 RID: 5492 RVA: 0x0007E5A2 File Offset: 0x0007C7A2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageVisualsComponent, ComponentInit>(new ComponentEventHandler<DamageVisualsComponent, ComponentInit>(this.InitializeEntity), null, null);
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x0007E5BE File Offset: 0x0007C7BE
		private void InitializeEntity(EntityUid entity, DamageVisualsComponent comp, ComponentInit args)
		{
			this.VerifyVisualizerSetup(entity, comp);
			if (!comp.Valid)
			{
				base.RemCompDeferred<DamageVisualsComponent>(entity);
				return;
			}
			this.InitializeVisualizer(entity, comp);
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x0007E5E4 File Offset: 0x0007C7E4
		private void VerifyVisualizerSetup(EntityUid entity, DamageVisualsComponent damageVisComp)
		{
			if (damageVisComp.Thresholds.Count < 1)
			{
				string text = "DamageVisuals";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ThresholdsLookup were invalid for entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(". ThresholdsLookup: ");
				defaultInterpolatedStringHandler.AppendFormatted<List<FixedPoint2>>(damageVisComp.Thresholds);
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.Divisor == 0f)
			{
				string text2 = "DamageVisuals";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Divisor for ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(" is set to zero.");
				Logger.ErrorS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.Overlay)
			{
				if (damageVisComp.DamageOverlayGroups == null && damageVisComp.DamageOverlay == null)
				{
					string text3 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Enabled overlay without defined damage overlay sprites on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text3, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay == null)
				{
					string text4 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Enabled all damage tracking without a damage overlay sprite on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text4, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay != null)
				{
					string text5 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Disabled all damage tracking with a damage overlay sprite on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.WarningS(text5, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
				{
					string text6 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Enabled all damage tracking with damage overlay groups on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.WarningS(text6, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
			}
			else if (!damageVisComp.Overlay)
			{
				if (damageVisComp.TargetLayers == null)
				{
					string text7 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Disabled overlay without target layers on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text7, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				if (damageVisComp.DamageOverlayGroups != null || damageVisComp.DamageOverlay != null)
				{
					string text8 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Disabled overlay with defined damage overlay sprites on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text8, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				if (damageVisComp.DamageGroup == null)
				{
					string text9 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Disabled overlay without defined damage group on ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text9, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
			}
			if (damageVisComp.DamageOverlayGroups != null && damageVisComp.DamageGroup != null)
			{
				string text10 = "DamageVisuals";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Damage overlay sprites and damage group are both defined on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.WarningS(text10, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (damageVisComp.DamageOverlay != null && damageVisComp.DamageGroup != null)
			{
				string text11 = "DamageVisuals";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Damage overlay sprites and damage group are both defined on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.WarningS(text11, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x0007E988 File Offset: 0x0007CB88
		private void InitializeVisualizer(EntityUid entity, DamageVisualsComponent damageVisComp)
		{
			SpriteComponent spriteComponent;
			DamageableComponent damageableComponent;
			if (!base.TryComp<SpriteComponent>(entity, ref spriteComponent) || !base.TryComp<DamageableComponent>(entity, ref damageableComponent) || !base.HasComp<AppearanceComponent>(entity))
			{
				return;
			}
			damageVisComp.Thresholds.Add(FixedPoint2.Zero);
			damageVisComp.Thresholds.Sort();
			if (damageVisComp.Thresholds[0] != 0)
			{
				string text = "DamageVisuals";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ThresholdsLookup were invalid for entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(". ThresholdsLookup: ");
				defaultInterpolatedStringHandler.AppendFormatted<List<FixedPoint2>>(damageVisComp.Thresholds);
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				damageVisComp.Valid = false;
				return;
			}
			DamageContainerPrototype damageContainerPrototype;
			if (damageableComponent.DamageContainerID != null && this._prototypeManager.TryIndex<DamageContainerPrototype>(damageableComponent.DamageContainerID, ref damageContainerPrototype))
			{
				if (damageVisComp.DamageOverlayGroups != null)
				{
					using (Dictionary<string, DamageVisualizerSprite>.KeyCollection.Enumerator enumerator = damageVisComp.DamageOverlayGroups.Keys.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text2 = enumerator.Current;
							if (!damageContainerPrototype.SupportedGroups.Contains(text2))
							{
								string text3 = "DamageVisuals";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
								defaultInterpolatedStringHandler.AppendLiteral("Damage key ");
								defaultInterpolatedStringHandler.AppendFormatted(text2);
								defaultInterpolatedStringHandler.AppendLiteral(" was invalid for entity ");
								defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
								defaultInterpolatedStringHandler.AppendLiteral(".");
								Logger.ErrorS(text3, defaultInterpolatedStringHandler.ToStringAndClear());
								damageVisComp.Valid = false;
								return;
							}
							damageVisComp.LastThresholdPerGroup.Add(text2, FixedPoint2.Zero);
						}
						goto IL_359;
					}
				}
				if (!damageVisComp.Overlay && damageVisComp.DamageGroup != null)
				{
					if (!damageContainerPrototype.SupportedGroups.Contains(damageVisComp.DamageGroup))
					{
						string text4 = "DamageVisuals";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Damage keys were invalid for entity ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						Logger.ErrorS(text4, defaultInterpolatedStringHandler.ToStringAndClear());
						damageVisComp.Valid = false;
						return;
					}
					damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
				}
			}
			else
			{
				List<string> list = this._prototypeManager.EnumeratePrototypes<DamageGroupPrototype>().Select((DamageGroupPrototype p, int _) => p.ID).ToList<string>();
				if (damageVisComp.DamageOverlayGroups != null)
				{
					using (Dictionary<string, DamageVisualizerSprite>.KeyCollection.Enumerator enumerator = damageVisComp.DamageOverlayGroups.Keys.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text5 = enumerator.Current;
							if (!list.Contains(text5))
							{
								string text6 = "DamageVisuals";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
								defaultInterpolatedStringHandler.AppendLiteral("Damage keys were invalid for entity ");
								defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
								defaultInterpolatedStringHandler.AppendLiteral(".");
								Logger.ErrorS(text6, defaultInterpolatedStringHandler.ToStringAndClear());
								damageVisComp.Valid = false;
								return;
							}
							damageVisComp.LastThresholdPerGroup.Add(text5, FixedPoint2.Zero);
						}
						goto IL_359;
					}
				}
				if (damageVisComp.DamageGroup != null)
				{
					if (!list.Contains(damageVisComp.DamageGroup))
					{
						string text7 = "DamageVisuals";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Damage keys were invalid for entity ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						Logger.ErrorS(text7, defaultInterpolatedStringHandler.ToStringAndClear());
						damageVisComp.Valid = false;
						return;
					}
					damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
				}
			}
			IL_359:
			List<Enum> targetLayers = damageVisComp.TargetLayers;
			if (targetLayers != null && targetLayers.Count > 0)
			{
				foreach (Enum @enum in damageVisComp.TargetLayers)
				{
					int num;
					if (!spriteComponent.LayerMapTryGet(@enum, ref num, false))
					{
						string text8 = "DamageVisuals";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Layer at key ");
						defaultInterpolatedStringHandler.AppendFormatted<Enum>(@enum);
						defaultInterpolatedStringHandler.AppendLiteral(" was invalid for entity ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						Logger.WarningS(text8, defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						damageVisComp.TargetLayerMapKeys.Add(@enum);
					}
				}
				if (damageVisComp.TargetLayerMapKeys.Count == 0)
				{
					string text9 = "DamageVisuals";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Target layers were invalid for entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.ErrorS(text9, defaultInterpolatedStringHandler.ToStringAndClear());
					damageVisComp.Valid = false;
					return;
				}
				using (List<Enum>.Enumerator enumerator2 = damageVisComp.TargetLayerMapKeys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Enum enum2 = enumerator2.Current;
						int num2 = spriteComponent.AllLayers.Count<ISpriteLayer>();
						int num3 = spriteComponent.LayerMapGet(enum2);
						if (num3 + 1 != num2)
						{
							num3++;
						}
						damageVisComp.LayerMapKeyStates.Add(enum2, enum2.ToString());
						if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
						{
							foreach (KeyValuePair<string, DamageVisualizerSprite> keyValuePair in damageVisComp.DamageOverlayGroups)
							{
								string text10;
								DamageVisualizerSprite damageVisualizerSprite;
								keyValuePair.Deconstruct(out text10, out damageVisualizerSprite);
								string value = text10;
								DamageVisualizerSprite damageVisualizerSprite2 = damageVisualizerSprite;
								SpriteComponent spriteComponent2 = spriteComponent;
								DamageVisualizerSprite sprite = damageVisualizerSprite2;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
								defaultInterpolatedStringHandler.AppendFormatted<Enum>(enum2);
								defaultInterpolatedStringHandler.AppendLiteral("_");
								defaultInterpolatedStringHandler.AppendFormatted(value);
								defaultInterpolatedStringHandler.AppendLiteral("_");
								defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(damageVisComp.Thresholds[1]);
								string state = defaultInterpolatedStringHandler.ToStringAndClear();
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
								defaultInterpolatedStringHandler.AppendFormatted<Enum>(enum2);
								defaultInterpolatedStringHandler.AppendFormatted(value);
								this.AddDamageLayerToSprite(spriteComponent2, sprite, state, defaultInterpolatedStringHandler.ToStringAndClear(), new int?(num3));
							}
							damageVisComp.DisabledLayers.Add(enum2, false);
						}
						else if (damageVisComp.DamageOverlay != null)
						{
							SpriteComponent spriteComponent3 = spriteComponent;
							DamageVisualizerSprite damageOverlay = damageVisComp.DamageOverlay;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
							defaultInterpolatedStringHandler.AppendFormatted<Enum>(enum2);
							defaultInterpolatedStringHandler.AppendLiteral("_");
							defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(damageVisComp.Thresholds[1]);
							string state2 = defaultInterpolatedStringHandler.ToStringAndClear();
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
							defaultInterpolatedStringHandler.AppendFormatted<Enum>(enum2);
							defaultInterpolatedStringHandler.AppendLiteral("trackDamage");
							this.AddDamageLayerToSprite(spriteComponent3, damageOverlay, state2, defaultInterpolatedStringHandler.ToStringAndClear(), new int?(num3));
							damageVisComp.DisabledLayers.Add(enum2, false);
						}
					}
					return;
				}
			}
			if (damageVisComp.DamageOverlayGroups != null)
			{
				using (Dictionary<string, DamageVisualizerSprite>.Enumerator enumerator3 = damageVisComp.DamageOverlayGroups.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						KeyValuePair<string, DamageVisualizerSprite> keyValuePair = enumerator3.Current;
						string text10;
						DamageVisualizerSprite damageVisualizerSprite;
						keyValuePair.Deconstruct(out text10, out damageVisualizerSprite);
						string text11 = text10;
						DamageVisualizerSprite damageVisualizerSprite3 = damageVisualizerSprite;
						SpriteComponent spriteComponent4 = spriteComponent;
						DamageVisualizerSprite sprite2 = damageVisualizerSprite3;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
						defaultInterpolatedStringHandler.AppendLiteral("DamageOverlay_");
						defaultInterpolatedStringHandler.AppendFormatted(text11);
						defaultInterpolatedStringHandler.AppendLiteral("_");
						defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(damageVisComp.Thresholds[1]);
						this.AddDamageLayerToSprite(spriteComponent4, sprite2, defaultInterpolatedStringHandler.ToStringAndClear(), "DamageOverlay" + text11, null);
						damageVisComp.TopMostLayerKey = "DamageOverlay" + text11;
					}
					return;
				}
			}
			if (damageVisComp.DamageOverlay != null)
			{
				SpriteComponent spriteComponent5 = spriteComponent;
				DamageVisualizerSprite damageOverlay2 = damageVisComp.DamageOverlay;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("DamageOverlay_");
				defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(damageVisComp.Thresholds[1]);
				this.AddDamageLayerToSprite(spriteComponent5, damageOverlay2, defaultInterpolatedStringHandler.ToStringAndClear(), "DamageOverlay", null);
				damageVisComp.TopMostLayerKey = "DamageOverlay";
			}
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x0007F19C File Offset: 0x0007D39C
		private void AddDamageLayerToSprite(SpriteComponent spriteComponent, DamageVisualizerSprite sprite, string state, string mapKey, int? index = null)
		{
			int num = spriteComponent.AddLayer(new SpriteSpecifier.Rsi(new ResourcePath(sprite.Sprite, "/"), state), index);
			spriteComponent.LayerMapSet(mapKey, num);
			if (sprite.Color != null)
			{
				spriteComponent.LayerSetColor(num, Color.FromHex(sprite.Color, null));
			}
			spriteComponent.LayerSetVisible(num, false);
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x0007F204 File Offset: 0x0007D404
		protected override void OnAppearanceChange(EntityUid uid, DamageVisualsComponent damageVisComp, ref AppearanceChangeEvent args)
		{
			if (!damageVisComp.Valid)
			{
				return;
			}
			bool disabled;
			if (this.AppearanceSystem.TryGetData<bool>(uid, DamageVisualizerKeys.Disabled, ref disabled, args.Component))
			{
				damageVisComp.Disabled = disabled;
			}
			if (damageVisComp.Disabled)
			{
				return;
			}
			this.HandleDamage(args.Component, damageVisComp);
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x0007F254 File Offset: 0x0007D454
		private void HandleDamage(AppearanceComponent component, DamageVisualsComponent damageVisComp)
		{
			SpriteComponent spriteComponent;
			DamageableComponent damageComponent;
			if (!base.TryComp<SpriteComponent>(component.Owner, ref spriteComponent) || !base.TryComp<DamageableComponent>(component.Owner, ref damageComponent))
			{
				return;
			}
			if (damageVisComp.TargetLayers != null && damageVisComp.DamageOverlayGroups != null)
			{
				this.UpdateDisabledLayers(spriteComponent, component, damageVisComp);
			}
			if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null && damageVisComp.TargetLayers == null)
			{
				this.CheckOverlayOrdering(spriteComponent, damageVisComp);
			}
			bool flag;
			if (this.AppearanceSystem.TryGetData<bool>(component.Owner, DamageVisualizerKeys.ForceUpdate, ref flag, component) && flag)
			{
				this.ForceUpdateLayers(damageComponent, spriteComponent, damageVisComp);
				return;
			}
			if (damageVisComp.TrackAllDamage)
			{
				this.UpdateDamageVisuals(damageComponent, spriteComponent, damageVisComp);
				return;
			}
			DamageVisualizerGroupData damageVisualizerGroupData;
			if (this.AppearanceSystem.TryGetData<DamageVisualizerGroupData>(component.Owner, DamageVisualizerKeys.DamageUpdateGroups, ref damageVisualizerGroupData, component))
			{
				this.UpdateDamageVisuals(damageVisualizerGroupData.GroupList, damageComponent, spriteComponent, damageVisComp);
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0007F320 File Offset: 0x0007D520
		private void UpdateDisabledLayers(SpriteComponent spriteComponent, AppearanceComponent component, DamageVisualsComponent damageVisComp)
		{
			foreach (Enum @enum in damageVisComp.TargetLayerMapKeys)
			{
				bool? flag = null;
				bool value;
				if (this.AppearanceSystem.TryGetData<bool>(component.Owner, @enum, ref value, component))
				{
					flag = new bool?(value);
				}
				if (flag != null && damageVisComp.DisabledLayers[@enum] != flag.Value)
				{
					damageVisComp.DisabledLayers[@enum] = flag.Value;
					if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
					{
						using (Dictionary<string, DamageVisualizerSprite>.KeyCollection.Enumerator enumerator2 = damageVisComp.DamageOverlayGroups.Keys.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string value2 = enumerator2.Current;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
								defaultInterpolatedStringHandler.AppendFormatted<Enum>(@enum);
								defaultInterpolatedStringHandler.AppendFormatted(value2);
								spriteComponent.LayerSetVisible(defaultInterpolatedStringHandler.ToStringAndClear(), damageVisComp.DisabledLayers[@enum]);
							}
							continue;
						}
					}
					if (damageVisComp.TrackAllDamage)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
						defaultInterpolatedStringHandler.AppendFormatted<Enum>(@enum);
						defaultInterpolatedStringHandler.AppendLiteral("trackDamage");
						spriteComponent.LayerSetVisible(defaultInterpolatedStringHandler.ToStringAndClear(), damageVisComp.DisabledLayers[@enum]);
					}
				}
			}
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x0007F4AC File Offset: 0x0007D6AC
		private void CheckOverlayOrdering(SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp)
		{
			if (spriteComponent[damageVisComp.TopMostLayerKey] != spriteComponent[spriteComponent.AllLayers.Count<ISpriteLayer>() - 1])
			{
				if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
				{
					using (Dictionary<string, DamageVisualizerSprite>.Enumerator enumerator = damageVisComp.DamageOverlayGroups.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, DamageVisualizerSprite> keyValuePair = enumerator.Current;
							string text;
							DamageVisualizerSprite damageVisualizerSprite;
							keyValuePair.Deconstruct(out text, out damageVisualizerSprite);
							string text2 = text;
							DamageVisualizerSprite sprite = damageVisualizerSprite;
							FixedPoint2 threshold = damageVisComp.LastThresholdPerGroup[text2];
							this.ReorderOverlaySprite(spriteComponent, damageVisComp, sprite, "DamageOverlay" + text2, "DamageOverlay_" + text2, threshold);
						}
						return;
					}
				}
				if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay != null)
				{
					this.ReorderOverlaySprite(spriteComponent, damageVisComp, damageVisComp.DamageOverlay, "DamageOverlay", "DamageOverlay", damageVisComp.LastDamageThreshold);
				}
			}
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x0007F59C File Offset: 0x0007D79C
		private void ReorderOverlaySprite(SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp, DamageVisualizerSprite sprite, string key, string statePrefix, FixedPoint2 threshold)
		{
			int num;
			spriteComponent.LayerMapTryGet(key, ref num, false);
			bool visible = spriteComponent[num].Visible;
			spriteComponent.RemoveLayer(num);
			if (threshold == FixedPoint2.Zero)
			{
				threshold = damageVisComp.Thresholds[1];
			}
			ResourcePath resourcePath = new ResourcePath(sprite.Sprite, "/");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(statePrefix);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(threshold);
			num = spriteComponent.AddLayer(new SpriteSpecifier.Rsi(resourcePath, defaultInterpolatedStringHandler.ToStringAndClear()), new int?(num));
			spriteComponent.LayerMapSet(key, num);
			spriteComponent.LayerSetVisible(num, visible);
			damageVisComp.TopMostLayerKey = key;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x0007F650 File Offset: 0x0007D850
		private void UpdateDamageVisuals(DamageableComponent damageComponent, SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp)
		{
			FixedPoint2 fixedPoint;
			if (!this.CheckThresholdBoundary(damageComponent.TotalDamage, damageVisComp.LastDamageThreshold, damageVisComp, out fixedPoint))
			{
				return;
			}
			damageVisComp.LastDamageThreshold = fixedPoint;
			if (damageVisComp.TargetLayers != null)
			{
				using (List<Enum>.Enumerator enumerator = damageVisComp.TargetLayerMapKeys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Enum layerMapKey = enumerator.Current;
						this.UpdateTargetLayer(spriteComponent, damageVisComp, layerMapKey, fixedPoint);
					}
					return;
				}
			}
			this.UpdateOverlay(spriteComponent, fixedPoint);
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0007F6D8 File Offset: 0x0007D8D8
		private void UpdateDamageVisuals(List<string> delta, DamageableComponent damageComponent, SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp)
		{
			foreach (string text in delta)
			{
				DamageGroupPrototype group;
				FixedPoint2 damageTotal;
				FixedPoint2 lastThreshold;
				FixedPoint2 fixedPoint;
				if ((damageVisComp.Overlay || !(text != damageVisComp.DamageGroup)) && this._prototypeManager.TryIndex<DamageGroupPrototype>(text, ref group) && damageComponent.Damage.TryGetDamageInGroup(group, out damageTotal) && damageVisComp.LastThresholdPerGroup.TryGetValue(text, out lastThreshold) && this.CheckThresholdBoundary(damageTotal, lastThreshold, damageVisComp, out fixedPoint))
				{
					damageVisComp.LastThresholdPerGroup[text] = fixedPoint;
					if (damageVisComp.TargetLayers != null)
					{
						using (List<Enum>.Enumerator enumerator2 = damageVisComp.TargetLayerMapKeys.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Enum layerMapKey = enumerator2.Current;
								this.UpdateTargetLayer(spriteComponent, damageVisComp, layerMapKey, text, fixedPoint);
							}
							continue;
						}
					}
					this.UpdateOverlay(spriteComponent, damageVisComp, text, fixedPoint);
				}
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x0007F7F8 File Offset: 0x0007D9F8
		private bool CheckThresholdBoundary(FixedPoint2 damageTotal, FixedPoint2 lastThreshold, DamageVisualsComponent damageVisComp, out FixedPoint2 threshold)
		{
			threshold = FixedPoint2.Zero;
			damageTotal /= damageVisComp.Divisor;
			int num = damageVisComp.Thresholds.BinarySearch(damageTotal);
			if (num < 0)
			{
				num = ~num;
				threshold = damageVisComp.Thresholds[num - 1];
			}
			else
			{
				threshold = damageVisComp.Thresholds[num];
			}
			return !(threshold == lastThreshold);
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x0007F870 File Offset: 0x0007DA70
		private void ForceUpdateLayers(DamageableComponent damageComponent, SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp)
		{
			if (damageVisComp.DamageOverlayGroups != null)
			{
				this.UpdateDamageVisuals(damageVisComp.DamageOverlayGroups.Keys.ToList<string>(), damageComponent, spriteComponent, damageVisComp);
				return;
			}
			if (damageVisComp.DamageGroup != null)
			{
				this.UpdateDamageVisuals(new List<string>
				{
					damageVisComp.DamageGroup
				}, damageComponent, spriteComponent, damageVisComp);
				return;
			}
			if (damageVisComp.DamageOverlay != null)
			{
				this.UpdateDamageVisuals(damageComponent, spriteComponent, damageVisComp);
			}
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x0007F8D4 File Offset: 0x0007DAD4
		private void UpdateTargetLayer(SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp, object layerMapKey, FixedPoint2 threshold)
		{
			if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
			{
				if (!damageVisComp.DisabledLayers[layerMapKey])
				{
					string text = damageVisComp.LayerMapKeyStates[layerMapKey];
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler.AppendFormatted<object>(layerMapKey);
					defaultInterpolatedStringHandler.AppendLiteral("trackDamage");
					int spriteLayer;
					spriteComponent.LayerMapTryGet(defaultInterpolatedStringHandler.ToStringAndClear(), ref spriteLayer, false);
					this.UpdateDamageLayerState(spriteComponent, spriteLayer, text ?? "", threshold);
					return;
				}
			}
			else if (!damageVisComp.Overlay)
			{
				string text2 = damageVisComp.LayerMapKeyStates[layerMapKey];
				int spriteLayer2;
				spriteComponent.LayerMapTryGet(layerMapKey, ref spriteLayer2, false);
				this.UpdateDamageLayerState(spriteComponent, spriteLayer2, text2 ?? "", threshold);
			}
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x0007F988 File Offset: 0x0007DB88
		private void UpdateTargetLayer(SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp, object layerMapKey, string damageGroup, FixedPoint2 threshold)
		{
			if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
			{
				if (damageVisComp.DamageOverlayGroups.ContainsKey(damageGroup) && !damageVisComp.DisabledLayers[layerMapKey])
				{
					string str = damageVisComp.LayerMapKeyStates[layerMapKey];
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
					defaultInterpolatedStringHandler.AppendFormatted<object>(layerMapKey);
					defaultInterpolatedStringHandler.AppendFormatted(damageGroup);
					int spriteLayer;
					spriteComponent.LayerMapTryGet(defaultInterpolatedStringHandler.ToStringAndClear(), ref spriteLayer, false);
					this.UpdateDamageLayerState(spriteComponent, spriteLayer, str + "_" + damageGroup, threshold);
					return;
				}
			}
			else if (!damageVisComp.Overlay)
			{
				string str2 = damageVisComp.LayerMapKeyStates[layerMapKey];
				int spriteLayer2;
				spriteComponent.LayerMapTryGet(layerMapKey, ref spriteLayer2, false);
				this.UpdateDamageLayerState(spriteComponent, spriteLayer2, str2 + "_" + damageGroup, threshold);
			}
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0007FA50 File Offset: 0x0007DC50
		private void UpdateOverlay(SpriteComponent spriteComponent, FixedPoint2 threshold)
		{
			int spriteLayer;
			spriteComponent.LayerMapTryGet("DamageOverlay", ref spriteLayer, false);
			this.UpdateDamageLayerState(spriteComponent, spriteLayer, "DamageOverlay", threshold);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x0007FA7C File Offset: 0x0007DC7C
		private void UpdateOverlay(SpriteComponent spriteComponent, DamageVisualsComponent damageVisComp, string damageGroup, FixedPoint2 threshold)
		{
			if (damageVisComp.DamageOverlayGroups != null && damageVisComp.DamageOverlayGroups.ContainsKey(damageGroup))
			{
				int spriteLayer;
				spriteComponent.LayerMapTryGet("DamageOverlay" + damageGroup, ref spriteLayer, false);
				this.UpdateDamageLayerState(spriteComponent, spriteLayer, "DamageOverlay_" + damageGroup, threshold);
			}
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x0007FACC File Offset: 0x0007DCCC
		private void UpdateDamageLayerState(SpriteComponent spriteComponent, int spriteLayer, string statePrefix, FixedPoint2 threshold)
		{
			if (threshold == 0)
			{
				spriteComponent.LayerSetVisible(spriteLayer, false);
				return;
			}
			if (!spriteComponent[spriteLayer].Visible)
			{
				spriteComponent.LayerSetVisible(spriteLayer, true);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(statePrefix);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(threshold);
			spriteComponent.LayerSetState(spriteLayer, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04000B42 RID: 2882
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000B43 RID: 2883
		private const string SawmillName = "DamageVisuals";
	}
}
