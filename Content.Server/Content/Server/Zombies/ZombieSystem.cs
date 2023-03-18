using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.Cloning;
using Content.Server.Disease;
using Content.Server.Disease.Components;
using Content.Server.Drone.Components;
using Content.Server.Emoting.Systems;
using Content.Server.Humanoid;
using Content.Server.Inventory;
using Content.Server.Speech.EntitySystems;
using Content.Shared.Bed.Sleep;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.Disease.Events;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Zombies;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Zombies
{
	// Token: 0x02000019 RID: 25
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ZombieSystem : SharedZombieSystem
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003040 File Offset: 0x00001240
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ZombieComponent, ComponentStartup>(new ComponentEventHandler<ZombieComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<ZombieComponent, EmoteEvent>(new ComponentEventRefHandler<ZombieComponent, EmoteEvent>(this.OnEmote), new Type[]
			{
				typeof(VocalSystem),
				typeof(BodyEmotesSystem)
			}, null);
			base.SubscribeLocalEvent<ZombieComponent, MeleeHitEvent>(new ComponentEventHandler<ZombieComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
			base.SubscribeLocalEvent<ZombieComponent, MobStateChangedEvent>(new ComponentEventHandler<ZombieComponent, MobStateChangedEvent>(this.OnMobState), null, null);
			base.SubscribeLocalEvent<ZombieComponent, CloningEvent>(new ComponentEventRefHandler<ZombieComponent, CloningEvent>(this.OnZombieCloning), null, null);
			base.SubscribeLocalEvent<ActiveZombieComponent, DamageChangedEvent>(new ComponentEventHandler<ActiveZombieComponent, DamageChangedEvent>(this.OnDamage), null, null);
			base.SubscribeLocalEvent<ActiveZombieComponent, AttemptSneezeCoughEvent>(new ComponentEventRefHandler<ActiveZombieComponent, AttemptSneezeCoughEvent>(this.OnSneeze), null, null);
			base.SubscribeLocalEvent<ActiveZombieComponent, TryingToSleepEvent>(new ComponentEventRefHandler<ActiveZombieComponent, TryingToSleepEvent>(this.OnSleepAttempt), null, null);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003112 File Offset: 0x00001312
		private void OnSleepAttempt(EntityUid uid, ActiveZombieComponent component, ref TryingToSleepEvent args)
		{
			args.Cancelled = true;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000311B File Offset: 0x0000131B
		private void OnStartup(EntityUid uid, ZombieComponent component, ComponentStartup args)
		{
			if (component.EmoteSoundsId == null)
			{
				return;
			}
			this._protoManager.TryIndex<EmoteSoundsPrototype>(component.EmoteSoundsId, ref component.EmoteSounds);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000313E File Offset: 0x0000133E
		private void OnEmote(EntityUid uid, ZombieComponent component, ref EmoteEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this._chat.TryPlayEmoteSound(uid, component.EmoteSounds, args.Emote);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003167 File Offset: 0x00001367
		private void OnMobState(EntityUid uid, ZombieComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState == MobState.Alive)
			{
				base.EnsureComp<ActiveZombieComponent>(uid);
				return;
			}
			base.RemComp<ActiveZombieComponent>(uid);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003184 File Offset: 0x00001384
		private void OnDamage(EntityUid uid, ActiveZombieComponent component, DamageChangedEvent args)
		{
			if (args.DamageIncreased)
			{
				this.DoGroan(uid, component);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003196 File Offset: 0x00001396
		private void OnSneeze(EntityUid uid, ActiveZombieComponent component, ref AttemptSneezeCoughEvent args)
		{
			args.Cancelled = true;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000031A0 File Offset: 0x000013A0
		private float GetZombieInfectionChance(EntityUid uid, ZombieComponent component)
		{
			float baseChance = component.MaxZombieInfectionChance;
			InventoryComponent inventoryComponent;
			if (!base.TryComp<InventoryComponent>(uid, ref inventoryComponent))
			{
				return baseChance;
			}
			InventorySystem.ContainerSlotEnumerator enumerator = new InventorySystem.ContainerSlotEnumerator(uid, inventoryComponent.TemplateId, this._protoManager, this._inv, SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK | SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING | SlotFlags.NECK | SlotFlags.GLOVES | SlotFlags.FEET);
			float items = 0f;
			float total = 0f;
			ContainerSlot con;
			while (enumerator.MoveNext(out con))
			{
				total += 1f;
				if (con.ContainedEntity != null)
				{
					items += 1f;
				}
			}
			float maxZombieInfectionChance = component.MaxZombieInfectionChance;
			float min = component.MinZombieInfectionChance;
			return (maxZombieInfectionChance - min) * ((total - items) / total) + min;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000323C File Offset: 0x0000143C
		private void OnMeleeHit(EntityUid uid, ZombieComponent component, MeleeHitEvent args)
		{
			ZombieComponent zombieComp;
			if (!this.EntityManager.TryGetComponent<ZombieComponent>(args.User, ref zombieComp))
			{
				return;
			}
			if (!args.HitEntities.Any<EntityUid>())
			{
				return;
			}
			foreach (EntityUid entity in args.HitEntities)
			{
				MobStateComponent mobState;
				if (!(args.User == entity) && base.TryComp<MobStateComponent>(entity, ref mobState) && !base.HasComp<DroneComponent>(entity))
				{
					if (base.HasComp<DiseaseCarrierComponent>(entity) && RandomExtensions.Prob(this._robustRandom, this.GetZombieInfectionChance(entity, component)))
					{
						this._disease.TryAddDisease(entity, "ActiveZombieVirus", null);
					}
					if (base.HasComp<ZombieComponent>(entity))
					{
						args.BonusDamage = -args.BaseDamage * zombieComp.OtherZombieDamageCoefficient;
					}
					if ((mobState.CurrentState == MobState.Dead || mobState.CurrentState == MobState.Critical) && !base.HasComp<ZombieComponent>(entity))
					{
						this._zombify.ZombifyEntity(entity);
						args.BonusDamage = -args.BaseDamage;
					}
					else if (mobState.CurrentState == MobState.Alive)
					{
						Solution healingSolution = new Solution();
						healingSolution.AddReagent("Bicaridine", 1.0, true);
						this._bloodstream.TryAddToChemicals(args.User, healingSolution, null);
					}
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000033B4 File Offset: 0x000015B4
		private void DoGroan(EntityUid uid, ActiveZombieComponent component)
		{
			if (component.LastDamageGroanCooldown > 0f)
			{
				return;
			}
			if (RandomExtensions.Prob(this._robustRandom, 0.5f))
			{
				this._chat.TrySendInGameICMessage(uid, "[automated zombie groan]", InGameICChatType.Speak, false, false, null, null, null, true, false);
			}
			else
			{
				this._chat.TryEmoteWithoutChat(uid, component.GroanEmoteId);
			}
			component.LastDamageGroanCooldown = component.GroanCooldown;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000341C File Offset: 0x0000161C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ActiveZombieComponent zombiecomp in base.EntityQuery<ActiveZombieComponent>(false))
			{
				zombiecomp.Accumulator += frameTime;
				zombiecomp.LastDamageGroanCooldown -= frameTime;
				if (zombiecomp.Accumulator >= zombiecomp.RandomGroanAttempt)
				{
					zombiecomp.Accumulator -= zombiecomp.RandomGroanAttempt;
					if (RandomExtensions.Prob(this._robustRandom, zombiecomp.GroanChance))
					{
						this.DoGroan(zombiecomp.Owner, zombiecomp);
					}
				}
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000034C8 File Offset: 0x000016C8
		[NullableContext(2)]
		public bool UnZombify(EntityUid source, EntityUid target, ZombieComponent zombiecomp)
		{
			if (!base.Resolve<ZombieComponent>(source, ref zombiecomp, true))
			{
				return false;
			}
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> keyValuePair in zombiecomp.BeforeZombifiedCustomBaseLayers)
			{
				HumanoidVisualLayers humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo customBaseLayerInfo;
				keyValuePair.Deconstruct(out humanoidVisualLayers, out customBaseLayerInfo);
				HumanoidVisualLayers layer = humanoidVisualLayers;
				HumanoidAppearanceState.CustomBaseLayerInfo info = customBaseLayerInfo;
				this._humanoidSystem.SetBaseLayerColor(target, layer, info.Color, true, null);
				this._humanoidSystem.SetBaseLayerId(target, layer, info.ID, true, null);
			}
			this._humanoidSystem.SetSkinColor(target, zombiecomp.BeforeZombifiedSkinColor, true, null);
			base.MetaData(target).EntityName = zombiecomp.BeforeZombifiedEntityName;
			return true;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003588 File Offset: 0x00001788
		private void OnZombieCloning(EntityUid uid, ZombieComponent zombiecomp, ref CloningEvent args)
		{
			if (this.UnZombify(args.Source, args.Target, zombiecomp))
			{
				args.NameHandled = true;
			}
		}

		// Token: 0x0400002F RID: 47
		[Dependency]
		private readonly DiseaseSystem _disease;

		// Token: 0x04000030 RID: 48
		[Dependency]
		private readonly BloodstreamSystem _bloodstream;

		// Token: 0x04000031 RID: 49
		[Dependency]
		private readonly ZombifyOnDeathSystem _zombify;

		// Token: 0x04000032 RID: 50
		[Dependency]
		private readonly ServerInventorySystem _inv;

		// Token: 0x04000033 RID: 51
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x04000034 RID: 52
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000035 RID: 53
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000036 RID: 54
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoidSystem;
	}
}
