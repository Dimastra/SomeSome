using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Payload.Components;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Payload.EntitySystems
{
	// Token: 0x020002E2 RID: 738
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PayloadSystem : EntitySystem
	{
		// Token: 0x06000F16 RID: 3862 RVA: 0x0004D3E4 File Offset: 0x0004B5E4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PayloadCaseComponent, TriggerEvent>(new ComponentEventHandler<PayloadCaseComponent, TriggerEvent>(this.OnCaseTriggered), null, null);
			base.SubscribeLocalEvent<PayloadTriggerComponent, TriggerEvent>(new ComponentEventHandler<PayloadTriggerComponent, TriggerEvent>(this.OnTriggerTriggered), null, null);
			base.SubscribeLocalEvent<PayloadCaseComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PayloadCaseComponent, EntInsertedIntoContainerMessage>(this.OnEntityInserted), null, null);
			base.SubscribeLocalEvent<PayloadCaseComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PayloadCaseComponent, EntRemovedFromContainerMessage>(this.OnEntityRemoved), null, null);
			base.SubscribeLocalEvent<PayloadCaseComponent, ExaminedEvent>(new ComponentEventHandler<PayloadCaseComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<ChemicalPayloadComponent, TriggerEvent>(new ComponentEventHandler<ChemicalPayloadComponent, TriggerEvent>(this.HandleChemicalPayloadTrigger), null, null);
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0004D46F File Offset: 0x0004B66F
		public IEnumerable<EntityUid> GetAllPayloads(EntityUid uid, [Nullable(2)] ContainerManagerComponent contMan = null)
		{
			if (!base.Resolve<ContainerManagerComponent>(uid, ref contMan, false))
			{
				yield break;
			}
			foreach (IContainer container in contMan.Containers.Values)
			{
				foreach (EntityUid entity in container.ContainedEntities)
				{
					if (this._tagSystem.HasTag(entity, "Payload"))
					{
						yield return entity;
					}
				}
				IEnumerator<EntityUid> enumerator2 = null;
			}
			Dictionary<string, IContainer>.ValueCollection.Enumerator enumerator = default(Dictionary<string, IContainer>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0004D490 File Offset: 0x0004B690
		private void OnCaseTriggered(EntityUid uid, PayloadCaseComponent component, TriggerEvent args)
		{
			ContainerManagerComponent contMan;
			if (!base.TryComp<ContainerManagerComponent>(uid, ref contMan))
			{
				return;
			}
			foreach (EntityUid ent in this.GetAllPayloads(uid, contMan))
			{
				base.RaiseLocalEvent<TriggerEvent>(ent, args, false);
			}
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0004D4F0 File Offset: 0x0004B6F0
		private void OnTriggerTriggered(EntityUid uid, PayloadTriggerComponent component, TriggerEvent args)
		{
			if (!component.Active)
			{
				return;
			}
			EntityUid parent = base.Transform(uid).ParentUid;
			if (!parent.Valid)
			{
				return;
			}
			base.RaiseLocalEvent<TriggerEvent>(parent, args, false);
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0004D528 File Offset: 0x0004B728
		private void OnEntityInserted(EntityUid uid, PayloadCaseComponent _, EntInsertedIntoContainerMessage args)
		{
			PayloadTriggerComponent trigger;
			if (!base.TryComp<PayloadTriggerComponent>(args.Entity, ref trigger))
			{
				return;
			}
			trigger.Active = true;
			if (trigger.Components == null)
			{
				return;
			}
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in trigger.Components)
			{
				string text;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
				keyValuePair.Deconstruct(out text, out componentRegistryEntry);
				string name = text;
				EntityPrototype.ComponentRegistryEntry data = componentRegistryEntry;
				ComponentRegistration registration;
				if (this._componentFactory.TryGetRegistration(name, ref registration, false) && !base.HasComp(uid, registration.Type))
				{
					Component component = this._componentFactory.GetComponent(registration.Type) as Component;
					if (component != null)
					{
						component.Owner = uid;
						object temp = component;
						this._serializationManager.CopyTo(data.Component, ref temp, null, false, false);
						this.EntityManager.AddComponent<Component>(uid, (Component)temp, false);
						trigger.GrantedComponents.Add(registration.Type);
					}
				}
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0004D634 File Offset: 0x0004B834
		private void OnEntityRemoved(EntityUid uid, PayloadCaseComponent component, EntRemovedFromContainerMessage args)
		{
			PayloadTriggerComponent trigger;
			if (!base.TryComp<PayloadTriggerComponent>(args.Entity, ref trigger))
			{
				return;
			}
			trigger.Active = false;
			foreach (Type type in trigger.GrantedComponents)
			{
				this.EntityManager.RemoveComponent(uid, type);
			}
			trigger.GrantedComponents.Clear();
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0004D6B4 File Offset: 0x0004B8B4
		private void OnExamined(EntityUid uid, PayloadCaseComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				args.PushMarkup(Loc.GetString("payload-case-not-close-enough", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ent", uid)
				}));
				return;
			}
			if (this.GetAllPayloads(uid, null).Any<EntityUid>())
			{
				args.PushMarkup(Loc.GetString("payload-case-has-payload", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ent", uid)
				}));
				return;
			}
			args.PushMarkup(Loc.GetString("payload-case-does-not-have-payload", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("ent", uid)
			}));
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0004D764 File Offset: 0x0004B964
		private void HandleChemicalPayloadTrigger(EntityUid uid, ChemicalPayloadComponent component, TriggerEvent args)
		{
			EntityUid? item = component.BeakerSlotA.Item;
			if (item != null)
			{
				EntityUid beakerA = item.GetValueOrDefault();
				item = component.BeakerSlotB.Item;
				if (item != null)
				{
					EntityUid beakerB = item.GetValueOrDefault();
					FitsInDispenserComponent compA;
					FitsInDispenserComponent compB;
					Solution solutionA;
					Solution solutionB;
					if (base.TryComp<FitsInDispenserComponent>(beakerA, ref compA) && base.TryComp<FitsInDispenserComponent>(beakerB, ref compB) && this._solutionSystem.TryGetSolution(beakerA, compA.Solution, out solutionA, null) && this._solutionSystem.TryGetSolution(beakerB, compB.Solution, out solutionB, null) && !(solutionA.Volume == 0) && !(solutionB.Volume == 0))
					{
						string solStringA = SolutionContainerSystem.ToPrettyString(solutionA);
						string solStringB = SolutionContainerSystem.ToPrettyString(solutionB);
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.ChemicalReaction;
						LogStringHandler logStringHandler = new LogStringHandler(60, 4);
						logStringHandler.AppendLiteral("Chemical bomb payload ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "payload", "ToPrettyString(uid)");
						logStringHandler.AppendLiteral(" at ");
						logStringHandler.AppendFormatted<MapCoordinates>(base.Transform(uid).MapPosition, "location", "Transform(uid).MapPosition");
						logStringHandler.AppendLiteral(" is combining two solutions: ");
						logStringHandler.AppendFormatted(solStringA, 0, "solutionA");
						logStringHandler.AppendLiteral(" and ");
						logStringHandler.AppendFormatted(solStringB, 0, "solutionB");
						adminLogger.Add(type, ref logStringHandler);
						solutionA.MaxVolume += solutionB.MaxVolume;
						this._solutionSystem.TryAddSolution(beakerA, solutionA, solutionB);
						this._solutionSystem.RemoveAllSolution(beakerB, solutionB);
						Solution tmpSol = this._solutionSystem.SplitSolution(beakerA, solutionA, solutionA.Volume * solutionB.MaxVolume / solutionA.MaxVolume);
						this._solutionSystem.TryAddSolution(beakerB, solutionB, tmpSol);
						solutionA.MaxVolume -= solutionB.MaxVolume;
						this._solutionSystem.UpdateChemicals(beakerA, solutionA, false, null);
						args.Handled = true;
						return;
					}
				}
			}
		}

		// Token: 0x040008DB RID: 2267
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x040008DC RID: 2268
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x040008DD RID: 2269
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040008DE RID: 2270
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x040008DF RID: 2271
		[Dependency]
		private readonly ISerializationManager _serializationManager;
	}
}
