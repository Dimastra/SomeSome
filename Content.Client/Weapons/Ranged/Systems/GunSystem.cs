using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.IoC;
using Content.Client.Items;
using Content.Client.Resources;
using Content.Client.Weapons.Ranged.Components;
using Content.Shared.Camera;
using Content.Shared.Popups;
using Content.Shared.Rounding;
using Content.Shared.Spawners.Components;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Weapons.Ranged.Systems
{
	// Token: 0x0200002E RID: 46
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GunSystem : SharedGunSystem
	{
		// Token: 0x060000BC RID: 188 RVA: 0x00006C4B File Offset: 0x00004E4B
		private void OnAmmoCounterCollect(EntityUid uid, AmmoCounterComponent component, ItemStatusCollectMessage args)
		{
			this.RefreshControl(uid, component);
			if (component.Control != null)
			{
				args.Controls.Add(component.Control);
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00006C70 File Offset: 0x00004E70
		[NullableContext(2)]
		private void RefreshControl(EntityUid uid, AmmoCounterComponent component = null)
		{
			if (!base.Resolve<AmmoCounterComponent>(uid, ref component, false))
			{
				return;
			}
			Control control = component.Control;
			if (control != null)
			{
				control.Dispose();
			}
			component.Control = null;
			GunSystem.AmmoCounterControlEvent ammoCounterControlEvent = new GunSystem.AmmoCounterControlEvent();
			base.RaiseLocalEvent<GunSystem.AmmoCounterControlEvent>(uid, ammoCounterControlEvent, false);
			GunSystem.AmmoCounterControlEvent ammoCounterControlEvent2 = ammoCounterControlEvent;
			if (ammoCounterControlEvent2.Control == null)
			{
				ammoCounterControlEvent2.Control = new GunSystem.DefaultStatusControl();
			}
			component.Control = ammoCounterControlEvent.Control;
			this.UpdateAmmoCount(uid, component);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006CDC File Offset: 0x00004EDC
		private void UpdateAmmoCount(EntityUid uid, AmmoCounterComponent component)
		{
			if (component.Control == null)
			{
				return;
			}
			GunSystem.UpdateAmmoCounterEvent updateAmmoCounterEvent = new GunSystem.UpdateAmmoCounterEvent
			{
				Control = component.Control
			};
			base.RaiseLocalEvent<GunSystem.UpdateAmmoCounterEvent>(uid, updateAmmoCounterEvent, false);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006D10 File Offset: 0x00004F10
		protected override void UpdateAmmoCount(EntityUid uid)
		{
			AmmoCounterComponent component;
			if (!this.Timing.IsFirstTimePredicted || !base.TryComp<AmmoCounterComponent>(uid, ref component))
			{
				return;
			}
			this.UpdateAmmoCount(uid, component);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006D3E File Offset: 0x00004F3E
		protected override void InitializeBallistic()
		{
			base.InitializeBallistic();
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnBallisticAmmoCount), null, null);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006D5C File Offset: 0x00004F5C
		private void OnBallisticAmmoCount(EntityUid uid, BallisticAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			GunSystem.DefaultStatusControl defaultStatusControl = args.Control as GunSystem.DefaultStatusControl;
			if (defaultStatusControl != null)
			{
				defaultStatusControl.Update(base.GetBallisticShots(component), component.Capacity);
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006D8C File Offset: 0x00004F8C
		protected override void Cycle(BallisticAmmoProviderComponent component, MapCoordinates coordinates)
		{
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			EntityUid? entityUid = null;
			if (component.Entities.Count > 0)
			{
				List<EntityUid> entities = component.Entities;
				EntityUid entityUid2 = entities[entities.Count - 1];
				component.Entities.RemoveAt(component.Entities.Count - 1);
				component.Container.Remove(entityUid2, null, null, null, true, false, null, null);
				base.EnsureComp<AmmoComponent>(entityUid2);
			}
			else if (component.UnspawnedCount > 0)
			{
				component.UnspawnedCount--;
				entityUid = new EntityUid?(base.Spawn(component.FillProto, coordinates));
				base.EnsureComp<AmmoComponent>(entityUid.Value);
			}
			if (entityUid != null && entityUid.Value.IsClientSide())
			{
				base.Del(entityUid.Value);
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006E78 File Offset: 0x00005078
		protected override void InitializeBasicEntity()
		{
			base.InitializeBasicEntity();
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnBasicEntityAmmoCount), null, null);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006E94 File Offset: 0x00005094
		private void OnBasicEntityAmmoCount(EntityUid uid, BasicEntityAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			GunSystem.DefaultStatusControl defaultStatusControl = args.Control as GunSystem.DefaultStatusControl;
			if (defaultStatusControl != null && component.Count != null && component.Capacity != null)
			{
				defaultStatusControl.Update(component.Count.Value, component.Capacity.Value);
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006EE8 File Offset: 0x000050E8
		protected override void InitializeBattery()
		{
			base.InitializeBattery();
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(this.OnControl), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnAmmoCountUpdate), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(this.OnControl), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnAmmoCountUpdate), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(this.OnControl), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnAmmoCountUpdate), null, null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006F74 File Offset: 0x00005174
		private void OnAmmoCountUpdate(EntityUid uid, BatteryAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			GunSystem.BoxesStatusControl boxesStatusControl = args.Control as GunSystem.BoxesStatusControl;
			if (boxesStatusControl == null)
			{
				return;
			}
			boxesStatusControl.Update(component.Shots, component.Capacity);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006FA3 File Offset: 0x000051A3
		private void OnControl(EntityUid uid, BatteryAmmoProviderComponent component, GunSystem.AmmoCounterControlEvent args)
		{
			args.Control = new GunSystem.BoxesStatusControl();
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006FB0 File Offset: 0x000051B0
		protected override void InitializeChamberMagazine()
		{
			base.InitializeChamberMagazine();
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(this.OnChamberMagazineCounter), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnChamberMagazineAmmoUpdate), null, null);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006FE0 File Offset: 0x000051E0
		protected override void OnMagazineSlotChange(EntityUid uid, MagazineAmmoProviderComponent component, ContainerModifiedMessage args)
		{
			base.OnMagazineSlotChange(uid, component, args);
			if (!("gun_chamber" != args.Container.ID))
			{
				EntRemovedFromContainerMessage entRemovedFromContainerMessage = args as EntRemovedFromContainerMessage;
				if (entRemovedFromContainerMessage != null)
				{
					if (entRemovedFromContainerMessage.Entity.IsClientSide())
					{
						base.QueueDel(args.Entity);
					}
					return;
				}
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007034 File Offset: 0x00005234
		private void OnChamberMagazineCounter(EntityUid uid, ChamberMagazineAmmoProviderComponent component, GunSystem.AmmoCounterControlEvent args)
		{
			args.Control = new GunSystem.ChamberMagazineStatusControl();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00007044 File Offset: 0x00005244
		private void OnChamberMagazineAmmoUpdate(EntityUid uid, ChamberMagazineAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			GunSystem.ChamberMagazineStatusControl chamberMagazineStatusControl = args.Control as GunSystem.ChamberMagazineStatusControl;
			if (chamberMagazineStatusControl == null)
			{
				return;
			}
			EntityUid? chamberEntity = base.GetChamberEntity(uid);
			EntityUid? magazineEntity = base.GetMagazineEntity(uid);
			GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
			if (magazineEntity != null)
			{
				base.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref getAmmoCountEvent, false);
			}
			chamberMagazineStatusControl.Update(chamberEntity != null, magazineEntity != null, getAmmoCountEvent.Count, getAmmoCountEvent.Capacity);
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000CC RID: 204 RVA: 0x000070B2 File Offset: 0x000052B2
		// (set) Token: 0x060000CD RID: 205 RVA: 0x000070BC File Offset: 0x000052BC
		public bool SpreadOverlay
		{
			get
			{
				return this._spreadOverlay;
			}
			set
			{
				if (this._spreadOverlay == value)
				{
					return;
				}
				this._spreadOverlay = value;
				IOverlayManager overlayManager = IoCManager.Resolve<IOverlayManager>();
				if (this._spreadOverlay)
				{
					overlayManager.AddOverlay(new GunSpreadOverlay(this.EntityManager, this._eyeManager, this.Timing, this._inputManager, this._player, this));
					return;
				}
				overlayManager.RemoveOverlay<GunSpreadOverlay>();
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000711C File Offset: 0x0000531C
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.SubscribeLocalEvent<AmmoCounterComponent, ItemStatusCollectMessage>(new ComponentEventHandler<AmmoCounterComponent, ItemStatusCollectMessage>(this.OnAmmoCounterCollect), null, null);
			base.SubscribeAllEvent<MuzzleFlashEvent>(new EntityEventHandler<MuzzleFlashEvent>(this.OnMuzzleFlash), null, null);
			base.SubscribeNetworkEvent<SharedGunSystem.HitscanEvent>(new EntityEventHandler<SharedGunSystem.HitscanEvent>(this.OnHitscan), null, null);
			this.InitializeMagazineVisuals();
			this.InitializeSpentAmmo();
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007180 File Offset: 0x00005380
		private void OnMuzzleFlash(MuzzleFlashEvent args)
		{
			this.CreateEffect(args.Uid, args, null);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000071A4 File Offset: 0x000053A4
		private void OnHitscan(SharedGunSystem.HitscanEvent ev)
		{
			foreach (ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float> valueTuple in ev.Sprites)
			{
				SpriteSpecifier.Rsi rsi = valueTuple.Item3 as SpriteSpecifier.Rsi;
				if (rsi != null && !base.Deleted(valueTuple.Item1.EntityId, null))
				{
					EntityUid entityUid = base.Spawn("HitscanEffect", valueTuple.Item1);
					SpriteComponent spriteComponent = base.Comp<SpriteComponent>(entityUid);
					base.Transform(entityUid).LocalRotation = valueTuple.Item2;
					spriteComponent[EffectLayers.Unshaded].AutoAnimated = false;
					spriteComponent.LayerSetSprite(EffectLayers.Unshaded, rsi);
					spriteComponent.LayerSetState(EffectLayers.Unshaded, rsi.RsiState);
					spriteComponent.Scale = new Vector2(valueTuple.Item4, 1f);
					spriteComponent[EffectLayers.Unshaded].Visible = true;
					Animation animation = new Animation
					{
						Length = TimeSpan.FromSeconds(0.47999998927116394),
						AnimationTracks = 
						{
							new AnimationTrackSpriteFlick
							{
								LayerKey = EffectLayers.Unshaded,
								KeyFrames = 
								{
									new AnimationTrackSpriteFlick.KeyFrame(rsi.RsiState, 0f)
								}
							}
						}
					};
					this._animPlayer.Play(entityUid, null, animation, "hitscan-effect");
				}
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007320 File Offset: 0x00005520
		public override void Update(float frameTime)
		{
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			EntityUid value = entityUid.Value;
			GunComponent gun = base.GetGun(value);
			if (gun == null)
			{
				return;
			}
			if (this._inputSystem.CmdStates.GetState(EngineKeyFunctions.Use) != 1)
			{
				if (gun.ShotCounter != 0)
				{
					this.EntityManager.RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent
					{
						Gun = gun.Owner
					});
				}
				return;
			}
			if (gun.NextFire > this.Timing.CurTime)
			{
				return;
			}
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
			if (mapCoordinates.MapId == MapId.Nullspace)
			{
				if (gun.ShotCounter != 0)
				{
					this.EntityManager.RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent
					{
						Gun = gun.Owner
					});
				}
				return;
			}
			EntityCoordinates coordinates = EntityCoordinates.FromMap(value, mapCoordinates, this.EntityManager);
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Sending shoot request tick ");
			defaultInterpolatedStringHandler.AppendFormatted<GameTick>(this.Timing.CurTick);
			defaultInterpolatedStringHandler.AppendLiteral(" / ");
			defaultInterpolatedStringHandler.AppendFormatted<TimeSpan>(this.Timing.CurTime);
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			this.EntityManager.RaisePredictiveEvent<RequestShootEvent>(new RequestShootEvent
			{
				Coordinates = coordinates,
				Gun = gun.Owner
			});
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000074AC File Offset: 0x000056AC
		public override void Shoot(GunComponent gun, List<IShootable> ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid? user = null)
		{
			Vector2 recoil = fromCoordinates.ToMapPos(this.EntityManager) - toCoordinates.ToMapPos(this.EntityManager);
			foreach (IShootable shootable in ammo)
			{
				CartridgeAmmoComponent cartridgeAmmoComponent = shootable as CartridgeAmmoComponent;
				if (cartridgeAmmoComponent == null)
				{
					AmmoComponent ammoComponent = shootable as AmmoComponent;
					if (ammoComponent == null)
					{
						if (shootable is HitscanPrototype)
						{
							this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
							this.Recoil(user, recoil);
						}
					}
					else
					{
						base.MuzzleFlash(gun.Owner, ammoComponent, user);
						this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
						this.Recoil(user, recoil);
						if (ammoComponent.Owner.IsClientSide())
						{
							base.Del(ammoComponent.Owner);
						}
						else
						{
							base.RemComp<AmmoComponent>(ammoComponent.Owner);
						}
					}
				}
				else
				{
					if (!cartridgeAmmoComponent.Spent)
					{
						base.SetCartridgeSpent(cartridgeAmmoComponent, true);
						base.MuzzleFlash(gun.Owner, cartridgeAmmoComponent, user);
						this.Audio.PlayPredicted(gun.SoundGunshot, gun.Owner, user, null);
						this.Recoil(user, recoil);
					}
					else
					{
						this.Audio.PlayPredicted(gun.SoundEmpty, gun.Owner, user, null);
					}
					if (cartridgeAmmoComponent.Owner.IsClientSide())
					{
						base.Del(cartridgeAmmoComponent.Owner);
					}
				}
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007680 File Offset: 0x00005880
		private void Recoil(EntityUid? user, Vector2 recoil)
		{
			if (!this.Timing.IsFirstTimePredicted || user == null || recoil == Vector2.Zero)
			{
				return;
			}
			this._recoil.KickCamera(user.Value, recoil.Normalized * 0.5f, null);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000076D5 File Offset: 0x000058D5
		protected override void Popup(string message, EntityUid? uid, EntityUid? user)
		{
			if (uid == null || user == null || !this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			this.PopupSystem.PopupEntity(message, uid.Value, user.Value, PopupType.Small);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007714 File Offset: 0x00005914
		protected override void CreateEffect(EntityUid uid, MuzzleFlashEvent message, EntityUid? user = null)
		{
			if (!this.Timing.IsFirstTimePredicted)
			{
				return;
			}
			EntityCoordinates coordinates;
			if (message.MatchRotation)
			{
				coordinates..ctor(uid, Vector2.Zero);
			}
			else
			{
				TransformComponent transformComponent;
				if (!base.TryComp<TransformComponent>(uid, ref transformComponent))
				{
					return;
				}
				coordinates = transformComponent.Coordinates;
			}
			EntityUid entityUid = base.Spawn(message.Prototype, coordinates);
			TransformComponent transformComponent2 = base.Transform(entityUid);
			transformComponent2.LocalRotation -= 1.5707964f;
			transformComponent2.LocalPosition += new Vector2(0f, -0.5f);
			float num = 0.4f;
			TimedDespawnComponent timedDespawnComponent;
			if (base.TryComp<TimedDespawnComponent>(uid, ref timedDespawnComponent))
			{
				num = timedDespawnComponent.Lifetime;
			}
			Animation animation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)num),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Color",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(Color.White.WithAlpha(1f), 0f),
							new AnimationTrackProperty.KeyFrame(Color.White.WithAlpha(0f), num)
						}
					}
				}
			};
			this._animPlayer.Play(entityUid, animation, "muzzle-flash");
			PointLightComponent pointLightComponent = base.EnsureComp<PointLightComponent>(uid);
			pointLightComponent.NetSyncEnabled = false;
			pointLightComponent.Enabled = true;
			pointLightComponent.Color = Color.FromHex("#cc8e2b", null);
			pointLightComponent.Radius = 2f;
			pointLightComponent.Energy = 5f;
			Animation animation2 = new Animation
			{
				Length = TimeSpan.FromSeconds((double)num),
				AnimationTracks = 
				{
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(PointLightComponent),
						Property = "Energy",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(5f, 0f),
							new AnimationTrackProperty.KeyFrame(0f, num)
						}
					},
					new AnimationTrackComponentProperty
					{
						ComponentType = typeof(PointLightComponent),
						Property = "Enabled",
						InterpolationMode = 0,
						KeyFrames = 
						{
							new AnimationTrackProperty.KeyFrame(true, 0f),
							new AnimationTrackProperty.KeyFrame(false, num)
						}
					}
				}
			};
			AnimationPlayerComponent animationPlayerComponent = base.EnsureComp<AnimationPlayerComponent>(uid);
			this._animPlayer.Stop(uid, animationPlayerComponent, "muzzle-flash-light");
			this._animPlayer.Play(uid, animationPlayerComponent, animation2, "muzzle-flash-light");
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000079D0 File Offset: 0x00005BD0
		protected override void InitializeMagazine()
		{
			base.InitializeMagazine();
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnMagazineAmmoUpdate), null, null);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000079EC File Offset: 0x00005BEC
		private void OnMagazineAmmoUpdate(EntityUid uid, MagazineAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			EntityUid? magazineEntity = base.GetMagazineEntity(uid);
			if (magazineEntity == null)
			{
				GunSystem.DefaultStatusControl defaultStatusControl = args.Control as GunSystem.DefaultStatusControl;
				if (defaultStatusControl != null)
				{
					defaultStatusControl.Update(0, 0);
				}
				return;
			}
			base.RaiseLocalEvent<GunSystem.UpdateAmmoCounterEvent>(magazineEntity.Value, args, false);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007A31 File Offset: 0x00005C31
		private void InitializeMagazineVisuals()
		{
			base.SubscribeLocalEvent<MagazineVisualsComponent, ComponentInit>(new ComponentEventHandler<MagazineVisualsComponent, ComponentInit>(this.OnMagazineVisualsInit), null, null);
			base.SubscribeLocalEvent<MagazineVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MagazineVisualsComponent, AppearanceChangeEvent>(this.OnMagazineVisualsChange), null, null);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007A5C File Offset: 0x00005C5C
		private void OnMagazineVisualsInit(EntityUid uid, MagazineVisualsComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int num;
			if (spriteComponent.LayerMapTryGet(GunVisualLayers.Mag, ref num, false))
			{
				SpriteComponent spriteComponent2 = spriteComponent;
				object obj = GunVisualLayers.Mag;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(component.MagSteps - 1);
				spriteComponent2.LayerSetState(obj, defaultInterpolatedStringHandler.ToStringAndClear());
				spriteComponent.LayerSetVisible(GunVisualLayers.Mag, false);
			}
			if (spriteComponent.LayerMapTryGet(GunVisualLayers.MagUnshaded, ref num, false))
			{
				SpriteComponent spriteComponent3 = spriteComponent;
				object obj2 = GunVisualLayers.MagUnshaded;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
				defaultInterpolatedStringHandler.AppendLiteral("-unshaded-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(component.MagSteps - 1);
				spriteComponent3.LayerSetState(obj2, defaultInterpolatedStringHandler.ToStringAndClear());
				spriteComponent.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
			}
			if (spriteComponent.LayerMapTryGet(GunVisualLayers.TwoModeFirst, ref num, false))
			{
				SpriteComponent spriteComponent4 = spriteComponent;
				object obj3 = GunVisualLayers.TwoModeFirst;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
				defaultInterpolatedStringHandler.AppendLiteral("-twomode1-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(component.MagSteps - 1);
				spriteComponent4.LayerSetState(obj3, defaultInterpolatedStringHandler.ToStringAndClear());
				spriteComponent.LayerSetVisible(GunVisualLayers.TwoModeFirst, false);
			}
			if (spriteComponent.LayerMapTryGet(GunVisualLayers.TwoModeSecond, ref num, false))
			{
				SpriteComponent spriteComponent5 = spriteComponent;
				object obj4 = GunVisualLayers.TwoModeSecond;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
				defaultInterpolatedStringHandler.AppendLiteral("-twomode2-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(component.MagSteps - 1);
				spriteComponent5.LayerSetState(obj4, defaultInterpolatedStringHandler.ToStringAndClear());
				spriteComponent.LayerSetVisible(GunVisualLayers.TwoModeSecond, false);
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007C14 File Offset: 0x00005E14
		private void OnMagazineVisualsChange(EntityUid uid, MagazineVisualsComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent sprite = args.Sprite;
			if (sprite == null)
			{
				return;
			}
			object obj;
			if (!args.AppearanceData.TryGetValue(AmmoVisuals.MagLoaded, out obj) || (obj is bool && (bool)obj))
			{
				object obj2;
				if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoMax, out obj2))
				{
					obj2 = component.MagSteps;
				}
				object obj3;
				if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoCount, out obj3))
				{
					obj3 = component.MagSteps;
				}
				int num = ContentHelpers.RoundToLevels((double)((int)obj3), (double)((int)obj2), component.MagSteps);
				int num2;
				if (num == 0 && !component.ZeroVisible)
				{
					if (sprite.LayerMapTryGet(GunVisualLayers.Mag, ref num2, false))
					{
						sprite.LayerSetVisible(GunVisualLayers.Mag, false);
					}
					if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, ref num2, false))
					{
						sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
					}
					if (sprite.LayerMapTryGet(GunVisualLayers.TwoModeFirst, ref num2, false))
					{
						sprite.LayerSetVisible(GunVisualLayers.TwoModeFirst, false);
					}
					if (sprite.LayerMapTryGet(GunVisualLayers.TwoModeSecond, ref num2, false))
					{
						sprite.LayerSetVisible(GunVisualLayers.TwoModeSecond, false);
					}
					return;
				}
				if (sprite.LayerMapTryGet(GunVisualLayers.Mag, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.Mag, true);
					SpriteComponent spriteComponent = sprite;
					object obj4 = GunVisualLayers.Mag;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
					defaultInterpolatedStringHandler.AppendLiteral("-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					spriteComponent.LayerSetState(obj4, defaultInterpolatedStringHandler.ToStringAndClear());
				}
				if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, true);
					SpriteComponent spriteComponent2 = sprite;
					object obj5 = GunVisualLayers.MagUnshaded;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
					defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
					defaultInterpolatedStringHandler.AppendLiteral("-unshaded-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					spriteComponent2.LayerSetState(obj5, defaultInterpolatedStringHandler.ToStringAndClear());
				}
				object obj6;
				if (!args.AppearanceData.TryGetValue(AmmoVisuals.InStun, out obj6) || (obj6 is bool && (bool)obj6))
				{
					if (sprite.LayerMapTryGet(GunVisualLayers.TwoModeFirst, ref num2, false))
					{
						sprite.LayerSetVisible(GunVisualLayers.TwoModeSecond, false);
						sprite.LayerSetVisible(GunVisualLayers.TwoModeFirst, true);
						SpriteComponent spriteComponent3 = sprite;
						object obj7 = GunVisualLayers.TwoModeFirst;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
						defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
						defaultInterpolatedStringHandler.AppendLiteral("-twomode1-");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						spriteComponent3.LayerSetState(obj7, defaultInterpolatedStringHandler.ToStringAndClear());
						return;
					}
				}
				else if (obj6 is bool && !(bool)obj6 && sprite.LayerMapTryGet(GunVisualLayers.TwoModeSecond, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.TwoModeFirst, false);
					sprite.LayerSetVisible(GunVisualLayers.TwoModeSecond, true);
					SpriteComponent spriteComponent4 = sprite;
					object obj8 = GunVisualLayers.TwoModeSecond;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
					defaultInterpolatedStringHandler.AppendFormatted(component.MagState);
					defaultInterpolatedStringHandler.AppendLiteral("-twomode2-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					spriteComponent4.LayerSetState(obj8, defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
			}
			else
			{
				int num2;
				if (sprite.LayerMapTryGet(GunVisualLayers.Mag, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.Mag, false);
				}
				if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
				}
				if (sprite.LayerMapTryGet(GunVisualLayers.TwoModeFirst, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.TwoModeFirst, false);
				}
				if (sprite.LayerMapTryGet(GunVisualLayers.TwoModeSecond, ref num2, false))
				{
					sprite.LayerSetVisible(GunVisualLayers.TwoModeSecond, false);
				}
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007F9C File Offset: 0x0000619C
		protected override void InitializeRevolver()
		{
			base.InitializeRevolver();
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(this.OnRevolverCounter), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(this.OnRevolverAmmoUpdate), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnRevolverEntRemove), null, null);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00007FEC File Offset: 0x000061EC
		private void OnRevolverEntRemove(EntityUid uid, RevolverAmmoProviderComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID != "revolver-ammo")
			{
				return;
			}
			if (!args.Entity.IsClientSide())
			{
				return;
			}
			base.QueueDel(args.Entity);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00008030 File Offset: 0x00006230
		private void OnRevolverAmmoUpdate(EntityUid uid, RevolverAmmoProviderComponent component, GunSystem.UpdateAmmoCounterEvent args)
		{
			GunSystem.RevolverStatusControl revolverStatusControl = args.Control as GunSystem.RevolverStatusControl;
			if (revolverStatusControl == null)
			{
				return;
			}
			revolverStatusControl.Update(component.CurrentIndex, component.Chambers);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000805F File Offset: 0x0000625F
		private void OnRevolverCounter(EntityUid uid, RevolverAmmoProviderComponent component, GunSystem.AmmoCounterControlEvent args)
		{
			args.Control = new GunSystem.RevolverStatusControl();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000806C File Offset: 0x0000626C
		private void InitializeSpentAmmo()
		{
			base.SubscribeLocalEvent<SpentAmmoVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SpentAmmoVisualsComponent, AppearanceChangeEvent>(this.OnSpentAmmoAppearance), null, null);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008084 File Offset: 0x00006284
		private void OnSpentAmmoAppearance(EntityUid uid, SpentAmmoVisualsComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent sprite = args.Sprite;
			if (sprite == null)
			{
				return;
			}
			object obj;
			if (!args.AppearanceData.TryGetValue(AmmoVisuals.Spent, out obj))
			{
				return;
			}
			string text;
			if ((bool)obj)
			{
				text = (component.Suffix ? (component.State + "-spent") : "spent");
			}
			else
			{
				text = component.State;
			}
			sprite.LayerSetState(AmmoVisualLayers.Base, text);
		}

		// Token: 0x04000081 RID: 129
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000082 RID: 130
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000083 RID: 131
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000084 RID: 132
		[Dependency]
		private readonly AnimationPlayerSystem _animPlayer;

		// Token: 0x04000085 RID: 133
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x04000086 RID: 134
		[Dependency]
		private readonly SharedCameraRecoilSystem _recoil;

		// Token: 0x04000087 RID: 135
		private bool _spreadOverlay;

		// Token: 0x0200002F RID: 47
		[NullableContext(0)]
		public sealed class AmmoCounterControlEvent : EntityEventArgs
		{
			// Token: 0x04000088 RID: 136
			[Nullable(2)]
			public Control Control;
		}

		// Token: 0x02000030 RID: 48
		[NullableContext(0)]
		public sealed class UpdateAmmoCounterEvent : HandledEntityEventArgs
		{
			// Token: 0x04000089 RID: 137
			[Nullable(1)]
			public Control Control;
		}

		// Token: 0x02000031 RID: 49
		[Nullable(0)]
		private sealed class DefaultStatusControl : Control
		{
			// Token: 0x060000E4 RID: 228 RVA: 0x00008110 File Offset: 0x00006310
			public DefaultStatusControl()
			{
				base.MinHeight = 15f;
				base.HorizontalExpand = true;
				base.VerticalAlignment = 2;
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 1;
				boxContainer.HorizontalExpand = true;
				boxContainer.VerticalAlignment = 2;
				boxContainer.SeparationOverride = new int?(0);
				Control.OrderedChildCollection children = boxContainer.Children;
				BoxContainer boxContainer2 = new BoxContainer();
				boxContainer2.Orientation = 0;
				boxContainer2.SeparationOverride = new int?(0);
				BoxContainer boxContainer3 = boxContainer2;
				this._bulletsListTop = boxContainer2;
				children.Add(boxContainer3);
				Control.OrderedChildCollection children2 = boxContainer.Children;
				BoxContainer boxContainer4 = new BoxContainer();
				boxContainer4.Orientation = 0;
				boxContainer4.HorizontalExpand = true;
				Control.OrderedChildCollection children3 = boxContainer4.Children;
				Control control = new Control();
				control.HorizontalExpand = true;
				Control.OrderedChildCollection children4 = control.Children;
				BoxContainer boxContainer5 = new BoxContainer();
				boxContainer5.Orientation = 0;
				boxContainer5.VerticalAlignment = 2;
				boxContainer5.SeparationOverride = new int?(0);
				boxContainer3 = boxContainer5;
				this._bulletsListBottom = boxContainer5;
				children4.Add(boxContainer3);
				children3.Add(control);
				children2.Add(boxContainer4);
				base.AddChild(boxContainer);
			}

			// Token: 0x060000E5 RID: 229 RVA: 0x000081FC File Offset: 0x000063FC
			public void Update(int count, int capacity)
			{
				this._bulletsListTop.RemoveAllChildren();
				this._bulletsListBottom.RemoveAllChildren();
				string path;
				if (capacity <= 20)
				{
					path = "/Textures/Interface/ItemStatus/Bullets/normal.png";
				}
				else if (capacity <= 30)
				{
					path = "/Textures/Interface/ItemStatus/Bullets/small.png";
				}
				else
				{
					path = "/Textures/Interface/ItemStatus/Bullets/tiny.png";
				}
				Texture texture = StaticIoC.ResC.GetTexture(path);
				if (capacity > 60)
				{
					GunSystem.DefaultStatusControl.FillBulletRow(this._bulletsListBottom, Math.Min(60, count), 60, texture);
					GunSystem.DefaultStatusControl.FillBulletRow(this._bulletsListTop, Math.Max(0, count - 60), capacity - 60, texture);
					return;
				}
				GunSystem.DefaultStatusControl.FillBulletRow(this._bulletsListBottom, count, capacity, texture);
			}

			// Token: 0x060000E6 RID: 230 RVA: 0x00008290 File Offset: 0x00006490
			private static void FillBulletRow(Control container, int count, int capacity, Texture texture)
			{
				Color color = Color.FromHex("#b68f0e", null);
				Color color2 = Color.FromHex("#d7df60", null);
				Color color3 = Color.FromHex("#000000", null);
				Color color4 = Color.FromHex("#222222", null);
				bool flag = false;
				for (int i = count; i < capacity; i++)
				{
					container.AddChild(new TextureRect
					{
						Texture = texture,
						ModulateSelfOverride = new Color?(flag ? color3 : color4)
					});
					flag = !flag;
				}
				for (int j = 0; j < count; j++)
				{
					container.AddChild(new TextureRect
					{
						Texture = texture,
						ModulateSelfOverride = new Color?(flag ? color : color2)
					});
					flag = !flag;
				}
			}

			// Token: 0x0400008A RID: 138
			private readonly BoxContainer _bulletsListTop;

			// Token: 0x0400008B RID: 139
			private readonly BoxContainer _bulletsListBottom;
		}

		// Token: 0x02000032 RID: 50
		[Nullable(0)]
		public sealed class BoxesStatusControl : Control
		{
			// Token: 0x060000E7 RID: 231 RVA: 0x00008380 File Offset: 0x00006580
			public BoxesStatusControl()
			{
				base.MinHeight = 15f;
				base.HorizontalExpand = true;
				base.VerticalAlignment = 2;
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 0;
				boxContainer.HorizontalExpand = true;
				Control.OrderedChildCollection children = boxContainer.Children;
				Control control = new Control();
				control.HorizontalExpand = true;
				Control.OrderedChildCollection children2 = control.Children;
				BoxContainer boxContainer2 = new BoxContainer();
				boxContainer2.Orientation = 0;
				boxContainer2.VerticalAlignment = 2;
				boxContainer2.SeparationOverride = new int?(4);
				BoxContainer boxContainer3 = boxContainer2;
				this._bulletsList = boxContainer2;
				children2.Add(boxContainer3);
				children.Add(control);
				boxContainer.Children.Add(new Control
				{
					MinSize = new ValueTuple<float, float>(5f, 0f)
				});
				Control.OrderedChildCollection children3 = boxContainer.Children;
				Label label = new Label();
				label.StyleClasses.Add("ItemStatus");
				label.HorizontalAlignment = 3;
				Label label2 = label;
				this._ammoCount = label;
				children3.Add(label2);
				base.AddChild(boxContainer);
			}

			// Token: 0x060000E8 RID: 232 RVA: 0x0000846C File Offset: 0x0000666C
			public void Update(int count, int max)
			{
				this._bulletsList.RemoveAllChildren();
				this._ammoCount.Visible = true;
				Label ammoCount = this._ammoCount;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("x");
				defaultInterpolatedStringHandler.AppendFormatted<int>(count, "00");
				ammoCount.Text = defaultInterpolatedStringHandler.ToStringAndClear();
				float step = 1f;
				if (max > 8)
				{
					step = (float)max / 8f;
				}
				GunSystem.BoxesStatusControl.FillBulletRow(this._bulletsList, count, max, step);
			}

			// Token: 0x060000E9 RID: 233 RVA: 0x000084E8 File Offset: 0x000066E8
			private static void FillBulletRow(Control container, int count, int capacity, float step = 1f)
			{
				Color backgroundColor = Color.FromHex("#000000", null);
				Color backgroundColor2 = Color.FromHex("#E00000", null);
				int num = 0;
				for (float num2 = (float)count; num2 < (float)capacity; num2 += step)
				{
					if ((float)capacity - num2 >= step)
					{
						container.AddChild(new PanelContainer
						{
							PanelOverride = new StyleBoxFlat
							{
								BackgroundColor = backgroundColor
							},
							MinSize = new ValueTuple<float, float>(10f, 15f)
						});
						num++;
					}
				}
				for (int i = 0; i < 8 - num; i++)
				{
					container.AddChild(new PanelContainer
					{
						PanelOverride = new StyleBoxFlat
						{
							BackgroundColor = backgroundColor2
						},
						MinSize = new ValueTuple<float, float>(10f, 15f)
					});
				}
			}

			// Token: 0x0400008C RID: 140
			private readonly BoxContainer _bulletsList;

			// Token: 0x0400008D RID: 141
			private readonly Label _ammoCount;
		}

		// Token: 0x02000033 RID: 51
		[Nullable(0)]
		private sealed class ChamberMagazineStatusControl : Control
		{
			// Token: 0x060000EA RID: 234 RVA: 0x000085C8 File Offset: 0x000067C8
			public ChamberMagazineStatusControl()
			{
				base.MinHeight = 15f;
				base.HorizontalExpand = true;
				base.VerticalAlignment = 2;
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 0;
				boxContainer.HorizontalExpand = true;
				Control.OrderedChildCollection children = boxContainer.Children;
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/chambered_rotated.png");
				textureRect.VerticalAlignment = 2;
				textureRect.HorizontalAlignment = 3;
				TextureRect textureRect2 = textureRect;
				this._chamberedBullet = textureRect;
				children.Add(textureRect2);
				boxContainer.Children.Add(new Control
				{
					MinSize = new ValueTuple<float, float>(5f, 0f)
				});
				Control.OrderedChildCollection children2 = boxContainer.Children;
				Control control = new Control();
				control.HorizontalExpand = true;
				Control.OrderedChildCollection children3 = control.Children;
				BoxContainer boxContainer2 = new BoxContainer();
				boxContainer2.Orientation = 0;
				boxContainer2.VerticalAlignment = 2;
				boxContainer2.SeparationOverride = new int?(0);
				BoxContainer boxContainer3 = boxContainer2;
				this._bulletsList = boxContainer2;
				children3.Add(boxContainer3);
				Control.OrderedChildCollection children4 = control.Children;
				Label label = new Label();
				label.Text = "No Magazine!";
				label.StyleClasses.Add("ItemStatus");
				Label label2 = label;
				this._noMagazineLabel = label;
				children4.Add(label2);
				children2.Add(control);
				boxContainer.Children.Add(new Control
				{
					MinSize = new ValueTuple<float, float>(5f, 0f)
				});
				Control.OrderedChildCollection children5 = boxContainer.Children;
				Label label3 = new Label();
				label3.StyleClasses.Add("ItemStatus");
				label3.HorizontalAlignment = 3;
				label2 = label3;
				this._ammoCount = label3;
				children5.Add(label2);
				base.AddChild(boxContainer);
			}

			// Token: 0x060000EB RID: 235 RVA: 0x0000874C File Offset: 0x0000694C
			public void Update(bool chambered, bool magazine, int count, int capacity)
			{
				this._chamberedBullet.ModulateSelfOverride = new Color?(chambered ? Color.FromHex("#d7df60", null) : Color.Black);
				this._bulletsList.RemoveAllChildren();
				if (!magazine)
				{
					this._noMagazineLabel.Visible = true;
					this._ammoCount.Visible = false;
					return;
				}
				this._noMagazineLabel.Visible = false;
				this._ammoCount.Visible = true;
				string path = "/Textures/Interface/ItemStatus/Bullets/normal.png";
				Texture texture = StaticIoC.ResC.GetTexture(path);
				Label ammoCount = this._ammoCount;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendLiteral("x");
				defaultInterpolatedStringHandler.AppendFormatted<int>(count, "00");
				ammoCount.Text = defaultInterpolatedStringHandler.ToStringAndClear();
				capacity = Math.Min(capacity, 20);
				GunSystem.ChamberMagazineStatusControl.FillBulletRow(this._bulletsList, count, capacity, texture);
			}

			// Token: 0x060000EC RID: 236 RVA: 0x0000882C File Offset: 0x00006A2C
			private static void FillBulletRow(Control container, int count, int capacity, Texture texture)
			{
				Color color = Color.FromHex("#b68f0e", null);
				Color color2 = Color.FromHex("#d7df60", null);
				Color color3 = Color.FromHex("#000000", null);
				Color color4 = Color.FromHex("#222222", null);
				bool flag = false;
				for (int i = count; i < capacity; i++)
				{
					container.AddChild(new TextureRect
					{
						Texture = texture,
						ModulateSelfOverride = new Color?(flag ? color3 : color4),
						Stretch = 4
					});
					flag = !flag;
				}
				count = Math.Min(count, capacity);
				for (int j = 0; j < count; j++)
				{
					container.AddChild(new TextureRect
					{
						Texture = texture,
						ModulateSelfOverride = new Color?(flag ? color : color2),
						Stretch = 4
					});
					flag = !flag;
				}
			}

			// Token: 0x060000ED RID: 237 RVA: 0x00008933 File Offset: 0x00006B33
			public void PlayAlarmAnimation(Animation animation)
			{
				this._noMagazineLabel.PlayAnimation(animation, "alarm");
			}

			// Token: 0x0400008E RID: 142
			private readonly BoxContainer _bulletsList;

			// Token: 0x0400008F RID: 143
			private readonly TextureRect _chamberedBullet;

			// Token: 0x04000090 RID: 144
			private readonly Label _noMagazineLabel;

			// Token: 0x04000091 RID: 145
			private readonly Label _ammoCount;
		}

		// Token: 0x02000034 RID: 52
		[Nullable(0)]
		private sealed class RevolverStatusControl : Control
		{
			// Token: 0x060000EE RID: 238 RVA: 0x00008948 File Offset: 0x00006B48
			public RevolverStatusControl()
			{
				base.MinHeight = 15f;
				base.HorizontalExpand = true;
				base.VerticalAlignment = 2;
				BoxContainer boxContainer = new BoxContainer();
				boxContainer.Orientation = 0;
				boxContainer.HorizontalExpand = true;
				boxContainer.VerticalAlignment = 2;
				boxContainer.SeparationOverride = new int?(0);
				BoxContainer boxContainer2 = boxContainer;
				this._bulletsList = boxContainer;
				base.AddChild(boxContainer2);
			}

			// Token: 0x060000EF RID: 239 RVA: 0x000089AC File Offset: 0x00006BAC
			public void Update(int currentIndex, bool?[] bullets)
			{
				this._bulletsList.RemoveAllChildren();
				int num = bullets.Length;
				string path;
				if (num <= 20)
				{
					path = "/Textures/Interface/ItemStatus/Bullets/normal.png";
				}
				else if (num <= 30)
				{
					path = "/Textures/Interface/ItemStatus/Bullets/small.png";
				}
				else
				{
					path = "/Textures/Interface/ItemStatus/Bullets/tiny.png";
				}
				Texture texture = StaticIoC.ResC.GetTexture(path);
				Texture texture2 = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/empty.png");
				this.FillBulletRow(currentIndex, bullets, this._bulletsList, texture, texture2);
			}

			// Token: 0x060000F0 RID: 240 RVA: 0x00008A14 File Offset: 0x00006C14
			private void FillBulletRow(int currentIndex, bool?[] bullets, Control container, Texture texture, Texture emptyTexture)
			{
				int num = bullets.Length;
				Color color = Color.FromHex("#b68f0e", null);
				Color color2 = Color.FromHex("#d7df60", null);
				Color color3 = Color.FromHex("#b50e25", null);
				Color color4 = Color.FromHex("#d3745f", null);
				Color color5 = Color.FromHex("#000000", null);
				Color color6 = Color.FromHex("#222222", null);
				bool flag = false;
				float num2 = 1.3f;
				for (int i = 0; i < num; i++)
				{
					bool? flag2 = bullets[i];
					Control control = new Control
					{
						MinSize = texture.Size * num2
					};
					if (i == currentIndex)
					{
						control.AddChild(new TextureRect
						{
							Texture = texture,
							TextureScale = new ValueTuple<float, float>(num2, num2),
							ModulateSelfOverride = new Color?(Color.LimeGreen)
						});
					}
					Texture texture2 = texture;
					Color value;
					if (flag2 != null)
					{
						if (flag2.Value)
						{
							value = (flag ? color : color2);
						}
						else
						{
							value = (flag ? color3 : color4);
							texture2 = emptyTexture;
						}
					}
					else
					{
						value = (flag ? color5 : color6);
					}
					control.AddChild(new TextureRect
					{
						Stretch = 4,
						Texture = texture2,
						ModulateSelfOverride = new Color?(value)
					});
					flag = !flag;
					container.AddChild(control);
				}
			}

			// Token: 0x04000092 RID: 146
			private readonly BoxContainer _bulletsList;
		}
	}
}
