using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Electrocution;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Pulling.Components;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Electrocution
{
	// Token: 0x02000535 RID: 1333
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ElectrocutionSystem : SharedElectrocutionSystem
	{
		// Token: 0x06001BD0 RID: 7120 RVA: 0x00093F04 File Offset: 0x00092104
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ElectrifiedComponent, StartCollideEvent>(new ComponentEventRefHandler<ElectrifiedComponent, StartCollideEvent>(this.OnElectrifiedStartCollide), null, null);
			base.SubscribeLocalEvent<ElectrifiedComponent, AttackedEvent>(new ComponentEventHandler<ElectrifiedComponent, AttackedEvent>(this.OnElectrifiedAttacked), null, null);
			base.SubscribeLocalEvent<ElectrifiedComponent, InteractHandEvent>(new ComponentEventHandler<ElectrifiedComponent, InteractHandEvent>(this.OnElectrifiedHandInteract), null, null);
			base.SubscribeLocalEvent<ElectrifiedComponent, InteractUsingEvent>(new ComponentEventHandler<ElectrifiedComponent, InteractUsingEvent>(this.OnElectrifiedInteractUsing), null, null);
			base.SubscribeLocalEvent<RandomInsulationComponent, MapInitEvent>(new ComponentEventHandler<RandomInsulationComponent, MapInitEvent>(this.OnRandomInsulationMapInit), null, null);
			base.UpdatesAfter.Add(typeof(PowerNetSystem));
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x00093F90 File Offset: 0x00092190
		public override void Update(float frameTime)
		{
			RemQueue<ElectrocutionComponent> finishedElectrocutionsQueue = default(RemQueue<ElectrocutionComponent>);
			foreach (ValueTuple<ElectrocutionComponent, PowerConsumerComponent> valueTuple in this.EntityManager.EntityQuery<ElectrocutionComponent, PowerConsumerComponent>(false))
			{
				ElectrocutionComponent electrocution = valueTuple.Item1;
				PowerConsumerComponent consumer = valueTuple.Item2;
				float ftAdjusted = Math.Min(frameTime, electrocution.TimeLeft);
				electrocution.TimeLeft -= ftAdjusted;
				electrocution.AccumulatedDamage += consumer.ReceivedPower * 0.0015f * ftAdjusted;
				if (MathHelper.CloseTo(electrocution.TimeLeft, 0f, 1E-07f))
				{
					finishedElectrocutionsQueue.Add(electrocution);
				}
			}
			foreach (ElectrocutionComponent finished in finishedElectrocutionsQueue)
			{
				EntityUid uid = finished.Owner;
				if (this.EntityManager.EntityExists(finished.Electrocuting))
				{
					DamageSpecifier damage = new DamageSpecifier(this._prototypeManager.Index<DamageTypePrototype>("Shock"), (int)finished.AccumulatedDamage);
					DamageSpecifier actual = this._damageableSystem.TryChangeDamage(new EntityUid?(finished.Electrocuting), damage, false, true, null, new EntityUid?(finished.Source));
					if (actual != null)
					{
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Electrocution;
						LogStringHandler logStringHandler = new LogStringHandler(45, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(finished.Electrocuting), "entity", "ToPrettyString(finished.Electrocuting)");
						logStringHandler.AppendLiteral(" received ");
						logStringHandler.AppendFormatted<FixedPoint2>(actual.Total, "damage", "actual.Total");
						logStringHandler.AppendLiteral(" powered electrocution damage from ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(finished.Source), "source", "ToPrettyString(finished.Source)");
						adminLogger.Add(type, ref logStringHandler);
					}
				}
				this.EntityManager.DeleteEntity(uid);
			}
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x000941A8 File Offset: 0x000923A8
		private void OnElectrifiedStartCollide(EntityUid uid, ElectrifiedComponent electrified, ref StartCollideEvent args)
		{
			if (!electrified.OnBump)
			{
				return;
			}
			this.TryDoElectrifiedAct(uid, args.OtherFixture.Body.Owner, 1f, electrified, null, null);
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x000941D3 File Offset: 0x000923D3
		private void OnElectrifiedAttacked(EntityUid uid, ElectrifiedComponent electrified, AttackedEvent args)
		{
			if (!electrified.OnAttacked)
			{
				return;
			}
			this.TryDoElectrifiedAct(uid, args.User, 1f, electrified, null, null);
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x000941F4 File Offset: 0x000923F4
		private void OnElectrifiedHandInteract(EntityUid uid, ElectrifiedComponent electrified, InteractHandEvent args)
		{
			if (!electrified.OnHandInteract)
			{
				return;
			}
			this.TryDoElectrifiedAct(uid, args.User, 1f, electrified, null, null);
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x00094218 File Offset: 0x00092418
		private void OnElectrifiedInteractUsing(EntityUid uid, ElectrifiedComponent electrified, InteractUsingEvent args)
		{
			if (!electrified.OnInteractUsing)
			{
				return;
			}
			InsulatedComponent insulation;
			float siemens = base.TryComp<InsulatedComponent>(args.Used, ref insulation) ? insulation.SiemensCoefficient : 1f;
			this.TryDoElectrifiedAct(uid, args.User, siemens, electrified, null, null);
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00094260 File Offset: 0x00092460
		[NullableContext(2)]
		public bool TryDoElectrifiedAct(EntityUid uid, EntityUid targetUid, float siemens = 1f, ElectrifiedComponent electrified = null, NodeContainerComponent nodeContainer = null, TransformComponent transform = null)
		{
			ElectrocutionSystem.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.nodeContainer = nodeContainer;
			if (!base.Resolve<ElectrifiedComponent, TransformComponent>(uid, ref electrified, ref transform, false))
			{
				return false;
			}
			if (!electrified.Enabled)
			{
				return false;
			}
			if (electrified.NoWindowInTile)
			{
				foreach (EntityUid entity in transform.Coordinates.GetEntitiesInTile(5, this._entityLookup))
				{
					if (this._tagSystem.HasTag(entity, "Window"))
					{
						return false;
					}
				}
			}
			siemens *= electrified.SiemensCoefficient;
			if (!this.DoCommonElectrocutionAttempt(targetUid, new EntityUid?(uid), ref siemens, false) || siemens <= 0f)
			{
				return false;
			}
			List<ValueTuple<EntityUid, int>> targets = new List<ValueTuple<EntityUid, int>>();
			this.GetChainedElectrocutionTargets(targetUid, targets);
			if (!electrified.RequirePower || electrified.UsesApcPower)
			{
				if (electrified.UsesApcPower && !this.IsPowered(uid, this.EntityManager, null))
				{
					return false;
				}
				bool lastRet = true;
				for (int i = targets.Count - 1; i >= 0; i--)
				{
					ValueTuple<EntityUid, int> valueTuple = targets[i];
					EntityUid entity2 = valueTuple.Item1;
					int depth = valueTuple.Item2;
					lastRet = this.TryDoElectrocution(entity2, new EntityUid?(uid), (int)((float)electrified.ShockDamage * MathF.Pow(0.75f, (float)depth)), TimeSpan.FromSeconds((double)(electrified.ShockTime * MathF.Pow(0.8f, (float)depth))), true, electrified.SiemensCoefficient, null, false);
				}
				return lastRet;
			}
			else
			{
				if (!base.Resolve<NodeContainerComponent>(uid, ref CS$<>8__locals1.nodeContainer, false))
				{
					return false;
				}
				Node node2;
				if ((node2 = ElectrocutionSystem.<TryDoElectrifiedAct>g__TryNode|28_0(electrified.HighVoltageNode, ref CS$<>8__locals1)) == null)
				{
					node2 = (ElectrocutionSystem.<TryDoElectrifiedAct>g__TryNode|28_0(electrified.MediumVoltageNode, ref CS$<>8__locals1) ?? ElectrocutionSystem.<TryDoElectrifiedAct>g__TryNode|28_0(electrified.LowVoltageNode, ref CS$<>8__locals1));
				}
				Node node = node2;
				if (node == null)
				{
					return false;
				}
				NodeGroupID nodeGroupID = node.NodeGroupID;
				ValueTuple<float, float> valueTuple2;
				if (nodeGroupID != NodeGroupID.HVPower)
				{
					if (nodeGroupID != NodeGroupID.MVPower)
					{
						valueTuple2 = new ValueTuple<float, float>(1f, 1f);
					}
					else
					{
						valueTuple2 = new ValueTuple<float, float>(electrified.MediumVoltageDamageMultiplier, electrified.MediumVoltageTimeMultiplier);
					}
				}
				else
				{
					valueTuple2 = new ValueTuple<float, float>(electrified.HighVoltageDamageMultiplier, electrified.HighVoltageTimeMultiplier);
				}
				ValueTuple<float, float> valueTuple3 = valueTuple2;
				float damageMult = valueTuple3.Item1;
				float timeMult = valueTuple3.Item2;
				bool lastRet2 = true;
				for (int j = targets.Count - 1; j >= 0; j--)
				{
					ValueTuple<EntityUid, int> valueTuple4 = targets[j];
					EntityUid entity3 = valueTuple4.Item1;
					int depth2 = valueTuple4.Item2;
					lastRet2 = this.TryDoElectrocutionPowered(entity3, uid, node, (int)((float)electrified.ShockDamage * MathF.Pow(0.75f, (float)depth2) * damageMult), TimeSpan.FromSeconds((double)(electrified.ShockTime * MathF.Pow(0.8f, (float)depth2) * timeMult)), true, electrified.SiemensCoefficient, null, null);
				}
				return lastRet2;
			}
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00094518 File Offset: 0x00092718
		[NullableContext(2)]
		public bool TryDoElectrocution(EntityUid uid, EntityUid? sourceUid, int shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f, StatusEffectsComponent statusEffects = null, bool ignoreInsulation = false)
		{
			if (!this.DoCommonElectrocutionAttempt(uid, sourceUid, ref siemensCoefficient, ignoreInsulation) || !this.DoCommonElectrocution(uid, sourceUid, new int?(shockDamage), time, refresh, siemensCoefficient, statusEffects))
			{
				return false;
			}
			base.RaiseLocalEvent<ElectrocutedEvent>(uid, new ElectrocutedEvent(uid, sourceUid, siemensCoefficient), true);
			return true;
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00094554 File Offset: 0x00092754
		[NullableContext(2)]
		private bool TryDoElectrocutionPowered(EntityUid uid, EntityUid sourceUid, [Nullable(1)] Node node, int shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f, StatusEffectsComponent statusEffects = null, TransformComponent sourceTransform = null)
		{
			if (!this.DoCommonElectrocutionAttempt(uid, new EntityUid?(sourceUid), ref siemensCoefficient, false))
			{
				return false;
			}
			if (siemensCoefficient <= 0.5f)
			{
				return this.DoCommonElectrocution(uid, new EntityUid?(sourceUid), new int?(shockDamage), time, refresh, siemensCoefficient, statusEffects);
			}
			if (!this.DoCommonElectrocution(uid, new EntityUid?(sourceUid), null, time, refresh, siemensCoefficient, statusEffects))
			{
				return false;
			}
			if (!base.Resolve<TransformComponent>(sourceUid, ref sourceTransform, true))
			{
				return true;
			}
			EntityManager entityManager = this.EntityManager;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("VirtualElectrocutionLoad");
			defaultInterpolatedStringHandler.AppendFormatted<NodeGroupID>(node.NodeGroupID);
			EntityUid electrocutionEntity = entityManager.SpawnEntity(defaultInterpolatedStringHandler.ToStringAndClear(), sourceTransform.Coordinates);
			ElectrocutionNode electrocutionNode = this.EntityManager.GetComponent<NodeContainerComponent>(electrocutionEntity).GetNode<ElectrocutionNode>("electrocution");
			ElectrocutionComponent component = this.EntityManager.GetComponent<ElectrocutionComponent>(electrocutionEntity);
			electrocutionNode.CableEntity = sourceUid;
			electrocutionNode.NodeName = node.Name;
			this._nodeGroupSystem.QueueReflood(electrocutionNode);
			component.TimeLeft = 1f;
			component.Electrocuting = uid;
			component.Source = sourceUid;
			base.RaiseLocalEvent<ElectrocutedEvent>(uid, new ElectrocutedEvent(uid, new EntityUid?(sourceUid), siemensCoefficient), true);
			return true;
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x0009467C File Offset: 0x0009287C
		private bool DoCommonElectrocutionAttempt(EntityUid uid, EntityUid? sourceUid, ref float siemensCoefficient, bool ignoreInsulation = false)
		{
			ElectrocutionAttemptEvent attemptEvent = new ElectrocutionAttemptEvent(uid, sourceUid, siemensCoefficient, ignoreInsulation ? SlotFlags.NONE : (~SlotFlags.POCKET));
			base.RaiseLocalEvent<ElectrocutionAttemptEvent>(uid, attemptEvent, true);
			if (attemptEvent.Cancelled)
			{
				return false;
			}
			siemensCoefficient = attemptEvent.SiemensCoefficient;
			return true;
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x000946BC File Offset: 0x000928BC
		[NullableContext(2)]
		private bool DoCommonElectrocution(EntityUid uid, EntityUid? sourceUid, int? shockDamage, TimeSpan time, bool refresh, float siemensCoefficient = 1f, StatusEffectsComponent statusEffects = null)
		{
			if (siemensCoefficient <= 0f)
			{
				return false;
			}
			if (shockDamage != null)
			{
				int? num = shockDamage;
				shockDamage = new int?((int)(((num != null) ? new float?((float)num.GetValueOrDefault()) : null) * siemensCoefficient).Value);
				if (shockDamage.Value <= 0)
				{
					return false;
				}
			}
			if (!base.Resolve<StatusEffectsComponent>(uid, ref statusEffects, false) || !this._statusEffectsSystem.CanApplyEffect(uid, "Electrocution", statusEffects))
			{
				return false;
			}
			if (!this._statusEffectsSystem.TryAddStatusEffect<ElectrocutedComponent>(uid, "Electrocution", time, refresh, statusEffects))
			{
				return false;
			}
			if (siemensCoefficient > 0.5f)
			{
				this._stunSystem.TryParalyze(uid, time * 1.0, refresh, statusEffects);
			}
			if (shockDamage != null)
			{
				int dmg = shockDamage.GetValueOrDefault();
				DamageSpecifier actual = this._damageableSystem.TryChangeDamage(new EntityUid?(uid), new DamageSpecifier(this._prototypeManager.Index<DamageTypePrototype>("Shock"), dmg), false, true, null, sourceUid);
				if (actual != null)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Electrocution;
					LogStringHandler logStringHandler = new LogStringHandler(39, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(statusEffects.Owner), "entity", "ToPrettyString(statusEffects.Owner)");
					logStringHandler.AppendLiteral(" received ");
					logStringHandler.AppendFormatted<FixedPoint2>(actual.Total, "damage", "actual.Total");
					logStringHandler.AppendLiteral(" powered electrocution damage");
					logStringHandler.AppendFormatted((sourceUid != null) ? (" from " + base.ToPrettyString(sourceUid.Value)) : "", 0, "source");
					adminLogger.Add(type, ref logStringHandler);
				}
			}
			this._stutteringSystem.DoStutter(uid, time * 1.5, refresh, statusEffects);
			this._jitteringSystem.DoJitter(uid, time * 0.75, refresh, 80f, 8f, true, statusEffects);
			this._popupSystem.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-popup-player"), uid, uid, PopupType.Small);
			Filter filter = Filter.PvsExcept(uid, 2f, this.EntityManager);
			if (sourceUid != null)
			{
				this._popupSystem.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-by-source-popup-others", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mob", uid),
					new ValueTuple<string, object>("source", sourceUid.Value)
				}), uid, filter, true, PopupType.Small);
				this.PlayElectrocutionSound(uid, sourceUid.Value, null);
			}
			else
			{
				this._popupSystem.PopupEntity(Loc.GetString("electrocuted-component-mob-shocked-popup-others", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mob", uid)
				}), uid, filter, true, PopupType.Small);
			}
			return true;
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x000949B8 File Offset: 0x00092BB8
		private void GetChainedElectrocutionTargets(EntityUid source, [TupleElementNames(new string[]
		{
			"entity",
			"depth"
		})] [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<EntityUid, int>> all)
		{
			HashSet<EntityUid> visited = new HashSet<EntityUid>();
			this.GetChainedElectrocutionTargetsRecurse(source, 1, visited, all);
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000949D8 File Offset: 0x00092BD8
		private void GetChainedElectrocutionTargetsRecurse(EntityUid entity, int depth, HashSet<EntityUid> visited, [TupleElementNames(new string[]
		{
			"entity",
			"depth"
		})] [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<EntityUid, int>> all)
		{
			all.Add(new ValueTuple<EntityUid, int>(entity, depth));
			visited.Add(entity);
			SharedPullableComponent pullable;
			if (this.EntityManager.TryGetComponent<SharedPullableComponent>(entity, ref pullable))
			{
				EntityUid? entityUid = pullable.Puller;
				if (entityUid != null)
				{
					EntityUid pullerId = entityUid.GetValueOrDefault();
					if (pullerId.Valid && !visited.Contains(pullerId))
					{
						this.GetChainedElectrocutionTargetsRecurse(pullerId, depth + 1, visited, all);
					}
				}
			}
			SharedPullerComponent puller;
			if (this.EntityManager.TryGetComponent<SharedPullerComponent>(entity, ref puller))
			{
				EntityUid? entityUid = puller.Pulling;
				if (entityUid != null)
				{
					EntityUid pullingId = entityUid.GetValueOrDefault();
					if (pullingId.Valid && !visited.Contains(pullingId))
					{
						this.GetChainedElectrocutionTargetsRecurse(pullingId, depth + 1, visited, all);
					}
				}
			}
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00094A8C File Offset: 0x00092C8C
		private void OnRandomInsulationMapInit(EntityUid uid, RandomInsulationComponent randomInsulation, MapInitEvent args)
		{
			InsulatedComponent insulated;
			if (!this.EntityManager.TryGetComponent<InsulatedComponent>(uid, ref insulated))
			{
				return;
			}
			if (randomInsulation.List.Length == 0)
			{
				return;
			}
			base.SetInsulatedSiemensCoefficient(uid, RandomExtensions.Pick<float>(this._random, randomInsulation.List), insulated);
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00094AD0 File Offset: 0x00092CD0
		[NullableContext(2)]
		private void PlayElectrocutionSound(EntityUid targetUid, EntityUid sourceUid, ElectrifiedComponent electrified = null)
		{
			if (!base.Resolve<ElectrifiedComponent>(sourceUid, ref electrified, true) || !electrified.PlaySoundOnShock)
			{
				return;
			}
			SoundSystem.Play(electrified.ShockNoises.GetSound(null, null), Filter.Pvs(targetUid, 2f, null, null, null), targetUid, new AudioParams?(AudioParams.Default.WithVolume(electrified.ShockVolume)));
		}

		// Token: 0x06001BE0 RID: 7136 RVA: 0x00094B34 File Offset: 0x00092D34
		[NullableContext(2)]
		[CompilerGenerated]
		internal static Node <TryDoElectrifiedAct>g__TryNode|28_0(string id, ref ElectrocutionSystem.<>c__DisplayClass28_0 A_1)
		{
			Node tryNode;
			if (id != null && A_1.nodeContainer.TryGetNode<Node>(id, out tryNode))
			{
				IBasePowerNet basePowerNet = tryNode.NodeGroup as IBasePowerNet;
				if (basePowerNet != null)
				{
					PowerState.Network networkNode = basePowerNet.NetworkNode;
					if (networkNode != null && networkNode.LastCombinedSupply > 0f)
					{
						return tryNode;
					}
				}
			}
			return null;
		}

		// Token: 0x040011D6 RID: 4566
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x040011D7 RID: 4567
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040011D8 RID: 4568
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040011D9 RID: 4569
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x040011DA RID: 4570
		[Dependency]
		private readonly SharedJitteringSystem _jitteringSystem;

		// Token: 0x040011DB RID: 4571
		[Dependency]
		private readonly SharedStunSystem _stunSystem;

		// Token: 0x040011DC RID: 4572
		[Dependency]
		private readonly SharedStutteringSystem _stutteringSystem;

		// Token: 0x040011DD RID: 4573
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040011DE RID: 4574
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040011DF RID: 4575
		[Dependency]
		private readonly NodeGroupSystem _nodeGroupSystem;

		// Token: 0x040011E0 RID: 4576
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040011E1 RID: 4577
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x040011E2 RID: 4578
		private const string StatusEffectKey = "Electrocution";

		// Token: 0x040011E3 RID: 4579
		private const string DamageType = "Shock";

		// Token: 0x040011E4 RID: 4580
		private const float ElectrifiedDamagePerWatt = 0.0015f;

		// Token: 0x040011E5 RID: 4581
		private const float RecursiveDamageMultiplier = 0.75f;

		// Token: 0x040011E6 RID: 4582
		private const float RecursiveTimeMultiplier = 0.8f;

		// Token: 0x040011E7 RID: 4583
		private const float ParalyzeTimeMultiplier = 1f;

		// Token: 0x040011E8 RID: 4584
		private const float StutteringTimeMultiplier = 1.5f;

		// Token: 0x040011E9 RID: 4585
		private const float JitterTimeMultiplier = 0.75f;

		// Token: 0x040011EA RID: 4586
		private const float JitterAmplitude = 80f;

		// Token: 0x040011EB RID: 4587
		private const float JitterFrequency = 8f;
	}
}
