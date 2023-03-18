using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.Disease.Components;
using Content.Server.DoAfter;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Clothing.Components;
using Content.Shared.Disease;
using Content.Shared.Disease.Components;
using Content.Shared.Disease.Events;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Disease
{
	// Token: 0x02000562 RID: 1378
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiseaseSystem : EntitySystem
	{
		// Token: 0x06001D30 RID: 7472 RVA: 0x0009B624 File Offset: 0x00099824
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DiseaseCarrierComponent, ComponentInit>(new ComponentEventHandler<DiseaseCarrierComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<DiseaseCarrierComponent, CureDiseaseAttemptEvent>(new ComponentEventHandler<DiseaseCarrierComponent, CureDiseaseAttemptEvent>(this.OnTryCureDisease), null, null);
			base.SubscribeLocalEvent<DiseaseCarrierComponent, RejuvenateEvent>(new ComponentEventHandler<DiseaseCarrierComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
			base.SubscribeLocalEvent<DiseasedComponent, ContactInteractionEvent>(new ComponentEventHandler<DiseasedComponent, ContactInteractionEvent>(this.OnContactInteraction), null, null);
			base.SubscribeLocalEvent<DiseasedComponent, EntitySpokeEvent>(new ComponentEventHandler<DiseasedComponent, EntitySpokeEvent>(this.OnEntitySpeak), null, null);
			base.SubscribeLocalEvent<DiseaseProtectionComponent, GotEquippedEvent>(new ComponentEventHandler<DiseaseProtectionComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<DiseaseProtectionComponent, GotUnequippedEvent>(new ComponentEventHandler<DiseaseProtectionComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<DiseaseVaccineComponent, AfterInteractEvent>(new ComponentEventHandler<DiseaseVaccineComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<DiseaseVaccineComponent, ExaminedEvent>(new ComponentEventHandler<DiseaseVaccineComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<DiseaseCarrierComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<DiseaseCarrierComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
			base.SubscribeLocalEvent<DiseaseVaccineComponent, DoAfterEvent>(new ComponentEventHandler<DiseaseVaccineComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0009B714 File Offset: 0x00099914
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EntityUid entity in this.AddQueue)
			{
				base.EnsureComp<DiseasedComponent>(entity);
			}
			this.AddQueue.Clear();
			foreach (ValueTuple<DiseaseCarrierComponent, DiseasePrototype> tuple in this.CureQueue)
			{
				if (tuple.Item1.Diseases.Count == 1)
				{
					base.RemComp<DiseasedComponent>(tuple.Item1.Owner);
				}
				tuple.Item1.PastDiseases.Add(tuple.Item2);
				tuple.Item1.Diseases.Remove(tuple.Item2);
			}
			this.CureQueue.Clear();
			foreach (ValueTuple<DiseasedComponent, DiseaseCarrierComponent, MobStateComponent> valueTuple in base.EntityQuery<DiseasedComponent, DiseaseCarrierComponent, MobStateComponent>(false))
			{
				DiseaseCarrierComponent carrierComp = valueTuple.Item2;
				MobStateComponent mobState = valueTuple.Item3;
				if (this._mobStateSystem.IsDead(mobState.Owner, mobState))
				{
					if (RandomExtensions.Prob(this._random, 0.005f * frameTime))
					{
						this.CureDisease(carrierComp, RandomExtensions.Pick<DiseasePrototype>(this._random, carrierComp.Diseases));
					}
				}
				else
				{
					for (int i = 0; i < carrierComp.Diseases.Count; i++)
					{
						DiseasePrototype disease = carrierComp.Diseases[i];
						disease.Accumulator += frameTime;
						disease.TotalAccumulator += frameTime;
						if (disease.Accumulator >= disease.TickTime)
						{
							HashSet<string> carrierDiseases = carrierComp.CarrierDiseases;
							bool doEffects = carrierDiseases == null || !carrierDiseases.Contains(disease.ID);
							DiseaseEffectArgs args = new DiseaseEffectArgs(carrierComp.Owner, disease, this.EntityManager);
							disease.Accumulator -= disease.TickTime;
							int stage = 0;
							float lastThreshold = 0f;
							for (int j = 0; j < disease.Stages.Count; j++)
							{
								if (disease.TotalAccumulator >= disease.Stages[j] && disease.Stages[j] > lastThreshold)
								{
									lastThreshold = disease.Stages[j];
									stage = j;
								}
							}
							foreach (DiseaseCure cure in disease.Cures)
							{
								if (cure.Stages.AsSpan<int>().Contains(stage) && cure.Cure(args))
								{
									this.CureDisease(carrierComp, disease);
								}
							}
							if (doEffects)
							{
								foreach (DiseaseEffect effect in disease.Effects)
								{
									if (effect.Stages.AsSpan<int>().Contains(stage) && RandomExtensions.Prob(this._random, effect.Probability))
									{
										effect.Effect(args);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001D32 RID: 7474 RVA: 0x0009BAD4 File Offset: 0x00099CD4
		private void OnInit(EntityUid uid, DiseaseCarrierComponent component, ComponentInit args)
		{
			if (component.NaturalImmunities == null || component.NaturalImmunities.Count == 0)
			{
				return;
			}
			foreach (string immunity in component.NaturalImmunities)
			{
				DiseasePrototype disease;
				if (this._prototypeManager.TryIndex<DiseasePrototype>(immunity, ref disease))
				{
					component.PastDiseases.Add(disease);
				}
				else
				{
					Logger.Error("Failed to index disease prototype + " + immunity + " for " + uid.ToString());
				}
			}
		}

		// Token: 0x06001D33 RID: 7475 RVA: 0x0009BB78 File Offset: 0x00099D78
		private void OnTryCureDisease(EntityUid uid, DiseaseCarrierComponent component, CureDiseaseAttemptEvent args)
		{
			foreach (DiseasePrototype disease in component.Diseases)
			{
				float cureProb = args.CureChance / (float)component.Diseases.Count - disease.CureResist;
				if (cureProb < 0f)
				{
					break;
				}
				if (cureProb > 1f)
				{
					this.CureDisease(component, disease);
					break;
				}
				if (RandomExtensions.Prob(this._random, cureProb))
				{
					this.CureDisease(component, disease);
					break;
				}
			}
		}

		// Token: 0x06001D34 RID: 7476 RVA: 0x0009BC14 File Offset: 0x00099E14
		private void OnRejuvenate(EntityUid uid, DiseaseCarrierComponent component, RejuvenateEvent args)
		{
			this.CureAllDiseases(uid, component);
		}

		// Token: 0x06001D35 RID: 7477 RVA: 0x0009BC20 File Offset: 0x00099E20
		private void OnEquipped(EntityUid uid, DiseaseProtectionComponent component, GotEquippedEvent args)
		{
			ClothingComponent clothing;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			DiseaseCarrierComponent carrier;
			if (base.TryComp<DiseaseCarrierComponent>(args.Equipee, ref carrier))
			{
				carrier.DiseaseResist += component.Protection;
			}
			component.IsActive = true;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x0009BC84 File Offset: 0x00099E84
		private void OnUnequipped(EntityUid uid, DiseaseProtectionComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			DiseaseCarrierComponent carrier;
			if (base.TryComp<DiseaseCarrierComponent>(args.Equipee, ref carrier))
			{
				carrier.DiseaseResist -= component.Protection;
			}
			component.IsActive = false;
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x0009BCC4 File Offset: 0x00099EC4
		private void CureDisease(DiseaseCarrierComponent carrier, DiseasePrototype disease)
		{
			ValueTuple<DiseaseCarrierComponent, DiseasePrototype> CureTuple = new ValueTuple<DiseaseCarrierComponent, DiseasePrototype>(carrier, disease);
			this.CureQueue.Enqueue(CureTuple);
			this._popupSystem.PopupEntity(Loc.GetString("disease-cured"), carrier.Owner, carrier.Owner, PopupType.Small);
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x0009BD08 File Offset: 0x00099F08
		[NullableContext(2)]
		public void CureAllDiseases(EntityUid uid, DiseaseCarrierComponent carrier = null)
		{
			if (!base.Resolve<DiseaseCarrierComponent>(uid, ref carrier, true))
			{
				return;
			}
			foreach (DiseasePrototype disease in carrier.Diseases)
			{
				this.CureDisease(carrier, disease);
			}
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x0009BD6C File Offset: 0x00099F6C
		private void OnContactInteraction(EntityUid uid, DiseasedComponent component, ContactInteractionEvent args)
		{
			this.InteractWithDiseased(uid, args.Other, null);
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x0009BD7C File Offset: 0x00099F7C
		private void OnEntitySpeak(EntityUid uid, DiseasedComponent component, EntitySpokeEvent args)
		{
			DiseaseCarrierComponent carrier;
			if (base.TryComp<DiseaseCarrierComponent>(uid, ref carrier))
			{
				this.SneezeCough(uid, RandomExtensions.Pick<DiseasePrototype>(this._random, carrier.Diseases), string.Empty, null, true, null);
			}
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x0009BDB8 File Offset: 0x00099FB8
		private void OnAfterInteract(EntityUid uid, DiseaseVaccineComponent vaxx, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach)
			{
				return;
			}
			if (vaxx.Used)
			{
				this._popupSystem.PopupEntity(Loc.GetString("vaxx-already-used"), args.User, args.User, PopupType.Small);
				return;
			}
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float injectDelay = vaxx.InjectDelay;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(uid);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, injectDelay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x0009BE5C File Offset: 0x0009A05C
		private void OnExamined(EntityUid uid, DiseaseVaccineComponent vaxx, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				if (vaxx.Used)
				{
					args.PushMarkup(Loc.GetString("vaxx-used"));
					return;
				}
				args.PushMarkup(Loc.GetString("vaxx-unused"));
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x0009BE90 File Offset: 0x0009A090
		private void OnApplyMetabolicMultiplier(EntityUid uid, DiseaseCarrierComponent component, ApplyMetabolicMultiplierEvent args)
		{
			if (args.Apply)
			{
				using (List<DiseasePrototype>.Enumerator enumerator = component.Diseases.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						enumerator.Current.TickTime *= args.Multiplier;
						return;
					}
				}
			}
			foreach (DiseasePrototype disease in component.Diseases)
			{
				disease.TickTime /= args.Multiplier;
				if (disease.Accumulator >= disease.TickTime)
				{
					disease.Accumulator = disease.TickTime;
				}
			}
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x0009BF60 File Offset: 0x0009A160
		[NullableContext(2)]
		private void InteractWithDiseased(EntityUid diseased, EntityUid target, DiseaseCarrierComponent diseasedCarrier = null)
		{
			DiseaseCarrierComponent carrier;
			if (!base.Resolve<DiseaseCarrierComponent>(diseased, ref diseasedCarrier, false) || diseasedCarrier.Diseases.Count == 0 || !base.TryComp<DiseaseCarrierComponent>(target, ref carrier))
			{
				return;
			}
			DiseasePrototype disease = RandomExtensions.Pick<DiseasePrototype>(this._random, diseasedCarrier.Diseases);
			this.TryInfect(carrier, disease, 0.4f, false);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x0009BFB4 File Offset: 0x0009A1B4
		public void TryAddDisease(EntityUid host, DiseasePrototype addedDisease, [Nullable(2)] DiseaseCarrierComponent target = null)
		{
			if (!base.Resolve<DiseaseCarrierComponent>(host, ref target, false))
			{
				return;
			}
			using (List<DiseasePrototype>.Enumerator enumerator = target.AllDiseases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ID == ((addedDisease != null) ? addedDisease.ID : null))
					{
						return;
					}
				}
			}
			DiseasePrototype freshDisease = this._serializationManager.CreateCopy<DiseasePrototype>(addedDisease, null, false, true);
			if (freshDisease == null)
			{
				return;
			}
			target.Diseases.Add(freshDisease);
			this.AddQueue.Enqueue(target.Owner);
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0009C058 File Offset: 0x0009A258
		[NullableContext(2)]
		public void TryAddDisease(EntityUid host, string addedDisease, DiseaseCarrierComponent target = null)
		{
			DiseasePrototype added;
			if (addedDisease == null || !this._prototypeManager.TryIndex<DiseasePrototype>(addedDisease, ref added))
			{
				return;
			}
			this.TryAddDisease(host, added, target);
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0009C084 File Offset: 0x0009A284
		public void TryInfect(DiseaseCarrierComponent carrier, [Nullable(2)] DiseasePrototype disease, float chance = 0.7f, bool forced = false)
		{
			if (disease == null || (!forced && !disease.Infectious))
			{
				return;
			}
			float infectionChance = chance - carrier.DiseaseResist;
			if (infectionChance <= 0f)
			{
				return;
			}
			if (RandomExtensions.Prob(this._random, infectionChance))
			{
				this.TryAddDisease(carrier.Owner, disease, carrier);
			}
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0009C0D0 File Offset: 0x0009A2D0
		public void TryInfect(DiseaseCarrierComponent carrier, [Nullable(2)] string disease, float chance = 0.7f, bool forced = false)
		{
			DiseasePrototype d;
			if (disease == null || !this._prototypeManager.TryIndex<DiseasePrototype>(disease, ref d))
			{
				return;
			}
			this.TryInfect(carrier, d, chance, forced);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x0009C0FC File Offset: 0x0009A2FC
		[NullableContext(2)]
		public bool SneezeCough(EntityUid uid, DiseasePrototype disease, [Nullable(1)] string snoughMessage, SoundSpecifier snoughSound, bool airTransmit = true, TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return false;
			}
			if (this._mobStateSystem.IsDead(uid, null))
			{
				return false;
			}
			AttemptSneezeCoughEvent attemptSneezeCoughEvent = new AttemptSneezeCoughEvent(uid, snoughMessage, snoughSound, false);
			base.RaiseLocalEvent<AttemptSneezeCoughEvent>(uid, ref attemptSneezeCoughEvent, false);
			if (attemptSneezeCoughEvent.Cancelled)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(snoughMessage))
			{
				this._popupSystem.PopupEntity(Loc.GetString(snoughMessage, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("person", Identity.Entity(uid, this.EntityManager))
				}), uid, PopupType.Small);
			}
			if (snoughSound != null)
			{
				this._audioSystem.PlayPvs(snoughSound, uid, null);
			}
			if (disease == null || !disease.Infectious || !airTransmit)
			{
				return true;
			}
			EntityUid? maskUid;
			IngestionBlockerComponent blocker;
			if (this._inventorySystem.TryGetSlotEntity(uid, "mask", out maskUid, null, null) && this.EntityManager.TryGetComponent<IngestionBlockerComponent>(maskUid, ref blocker) && blocker.Enabled)
			{
				return true;
			}
			EntityQuery<DiseaseCarrierComponent> carrierQuery = base.GetEntityQuery<DiseaseCarrierComponent>();
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(xform.MapPosition, 2f, 46))
			{
				DiseaseCarrierComponent carrier;
				if (carrierQuery.TryGetComponent(entity, ref carrier) && this._interactionSystem.InRangeUnobstructed(uid, entity, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
				{
					this.TryInfect(carrier, disease, 0.3f, false);
				}
			}
			return true;
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x0009C280 File Offset: 0x0009A480
		public void Vaccinate(DiseaseCarrierComponent carrier, DiseasePrototype disease)
		{
			using (List<DiseasePrototype>.Enumerator enumerator = carrier.Diseases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ID == disease.ID)
					{
						return;
					}
				}
			}
			carrier.PastDiseases.Add(disease);
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x0009C2EC File Offset: 0x0009A4EC
		private void OnDoAfter(EntityUid uid, DiseaseVaccineComponent component, DoAfterEvent args)
		{
			DiseaseCarrierComponent carrier;
			if (args.Handled || args.Cancelled || !base.TryComp<DiseaseCarrierComponent>(args.Args.Target, ref carrier) || component.Disease == null)
			{
				return;
			}
			this.Vaccinate(carrier, component.Disease);
			this.EntityManager.DeleteEntity(uid);
			args.Handled = true;
		}

		// Token: 0x040012A1 RID: 4769
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x040012A2 RID: 4770
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040012A3 RID: 4771
		[Dependency]
		private readonly ISerializationManager _serializationManager;

		// Token: 0x040012A4 RID: 4772
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040012A5 RID: 4773
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040012A6 RID: 4774
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040012A7 RID: 4775
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040012A8 RID: 4776
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040012A9 RID: 4777
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040012AA RID: 4778
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040012AB RID: 4779
		private Queue<EntityUid> AddQueue = new Queue<EntityUid>();

		// Token: 0x040012AC RID: 4780
		[TupleElementNames(new string[]
		{
			"carrier",
			"disease"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private Queue<ValueTuple<DiseaseCarrierComponent, DiseasePrototype>> CureQueue = new Queue<ValueTuple<DiseaseCarrierComponent, DiseasePrototype>>();
	}
}
