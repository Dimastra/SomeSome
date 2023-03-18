using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Radiation.Events;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Damage
{
	// Token: 0x02000534 RID: 1332
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageableSystem : EntitySystem
	{
		// Token: 0x0600102F RID: 4143 RVA: 0x00034678 File Offset: 0x00032878
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DamageableComponent, ComponentInit>(new ComponentEventHandler<DamageableComponent, ComponentInit>(this.DamageableInit), null, null);
			base.SubscribeLocalEvent<DamageableComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageableComponent, ComponentHandleState>(this.DamageableHandleState), null, null);
			base.SubscribeLocalEvent<DamageableComponent, ComponentGetState>(new ComponentEventRefHandler<DamageableComponent, ComponentGetState>(this.DamageableGetState), null, null);
			base.SubscribeLocalEvent<DamageableComponent, OnIrradiatedEvent>(new ComponentEventHandler<DamageableComponent, OnIrradiatedEvent>(this.OnIrradiated), null, null);
			base.SubscribeLocalEvent<DamageableComponent, RejuvenateEvent>(new ComponentEventHandler<DamageableComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x000346EC File Offset: 0x000328EC
		public FormattedMessage GetDamageExamine(DamageSpecifier damageSpecifier, [Nullable(2)] string type = null)
		{
			FormattedMessage msg = new FormattedMessage();
			if (string.IsNullOrEmpty(type))
			{
				msg.AddMarkup(Loc.GetString("damage-examine"));
			}
			else
			{
				msg.AddMarkup(Loc.GetString("damage-examine-type", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("type", type)
				}));
			}
			foreach (KeyValuePair<string, FixedPoint2> damage in damageSpecifier.DamageDict)
			{
				if (damage.Value != FixedPoint2.Zero)
				{
					msg.PushNewline();
					msg.AddMarkup(Loc.GetString("damage-value", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("type", damage.Key),
						new ValueTuple<string, object>("amount", damage.Value)
					}));
				}
			}
			return msg;
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x000347E8 File Offset: 0x000329E8
		private void DamageableInit(EntityUid uid, DamageableComponent component, ComponentInit _)
		{
			DamageContainerPrototype damageContainerPrototype;
			if (component.DamageContainerID != null && this._prototypeManager.TryIndex<DamageContainerPrototype>(component.DamageContainerID, ref damageContainerPrototype))
			{
				foreach (string type in damageContainerPrototype.SupportedTypes)
				{
					component.Damage.DamageDict.TryAdd(type, FixedPoint2.Zero);
				}
				using (List<string>.Enumerator enumerator = damageContainerPrototype.SupportedGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupID = enumerator.Current;
						foreach (string type2 in this._prototypeManager.Index<DamageGroupPrototype>(groupID).DamageTypes)
						{
							component.Damage.DamageDict.TryAdd(type2, FixedPoint2.Zero);
						}
					}
					goto IL_13D;
				}
			}
			foreach (DamageTypePrototype type3 in this._prototypeManager.EnumeratePrototypes<DamageTypePrototype>())
			{
				component.Damage.DamageDict.TryAdd(type3.ID, FixedPoint2.Zero);
			}
			IL_13D:
			component.DamagePerGroup = component.Damage.GetDamagePerGroup(this._prototypeManager);
			component.TotalDamage = component.Damage.Total;
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00034990 File Offset: 0x00032B90
		public void SetDamage(DamageableComponent damageable, DamageSpecifier damage)
		{
			damageable.Damage = damage;
			this.DamageChanged(damageable, null, true, null);
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x000349B8 File Offset: 0x00032BB8
		public void DamageChanged(DamageableComponent component, [Nullable(2)] DamageSpecifier damageDelta = null, bool interruptsDoAfters = true, EntityUid? origin = null)
		{
			component.DamagePerGroup = component.Damage.GetDamagePerGroup(this._prototypeManager);
			component.TotalDamage = component.Damage.Total;
			base.Dirty(component, null);
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearance) && damageDelta != null)
			{
				DamageVisualizerGroupData data = new DamageVisualizerGroupData(damageDelta.GetDamagePerGroup(this._prototypeManager).Keys.ToList<string>());
				this._appearance.SetData(component.Owner, DamageVisualizerKeys.DamageUpdateGroups, data, appearance);
			}
			base.RaiseLocalEvent<DamageChangedEvent>(component.Owner, new DamageChangedEvent(component, damageDelta, interruptsDoAfters, origin), false);
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00034A5C File Offset: 0x00032C5C
		[NullableContext(2)]
		public DamageSpecifier TryChangeDamage(EntityUid? uid, [Nullable(1)] DamageSpecifier damage, bool ignoreResistances = false, bool interruptsDoAfters = true, DamageableComponent damageable = null, EntityUid? origin = null)
		{
			if (uid == null || !base.Resolve<DamageableComponent>(uid.Value, ref damageable, false))
			{
				return null;
			}
			if (damage == null)
			{
				Logger.Error("Null DamageSpecifier. Probably because a required yaml field was not given.");
				return null;
			}
			if (damage.Empty)
			{
				return damage;
			}
			if (!ignoreResistances)
			{
				DamageModifierSetPrototype modifierSet;
				if (damageable.DamageModifierSetId != null && this._prototypeManager.TryIndex<DamageModifierSetPrototype>(damageable.DamageModifierSetId, ref modifierSet))
				{
					damage = DamageSpecifier.ApplyModifierSet(damage, modifierSet);
				}
				DamageModifyEvent ev = new DamageModifyEvent(damage);
				base.RaiseLocalEvent<DamageModifyEvent>(uid.Value, ev, false);
				damage = ev.Damage;
				if (damage.Empty)
				{
					return damage;
				}
			}
			DamageSpecifier oldDamage = new DamageSpecifier(damageable.Damage);
			damageable.Damage.ExclusiveAdd(damage);
			damageable.Damage.ClampMin(FixedPoint2.Zero);
			DamageSpecifier delta = damageable.Damage - oldDamage;
			delta.TrimZeros();
			if (!delta.Empty)
			{
				this.DamageChanged(damageable, delta, interruptsDoAfters, origin);
			}
			return delta;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00034B48 File Offset: 0x00032D48
		public void SetAllDamage(DamageableComponent component, FixedPoint2 newValue)
		{
			if (newValue < 0)
			{
				return;
			}
			foreach (string type in component.Damage.DamageDict.Keys)
			{
				component.Damage.DamageDict[type] = newValue;
			}
			this.DamageChanged(component, new DamageSpecifier(), true, null);
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00034BD0 File Offset: 0x00032DD0
		public void SetDamageModifierSetId(EntityUid uid, string damageModifierSetId, [Nullable(2)] DamageableComponent comp = null)
		{
			if (!base.Resolve<DamageableComponent>(uid, ref comp, true))
			{
				return;
			}
			comp.DamageModifierSetId = damageModifierSetId;
			base.Dirty(comp, null);
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00034BF0 File Offset: 0x00032DF0
		private void DamageableGetState(EntityUid uid, DamageableComponent component, ref ComponentGetState args)
		{
			if (this._netMan.IsServer)
			{
				args.State = new DamageableComponentState(component.Damage.DamageDict, component.DamageModifierSetId);
				return;
			}
			args.State = new DamageableComponentState(Extensions.ShallowClone<string, FixedPoint2>(component.Damage.DamageDict), component.DamageModifierSetId);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x00034C48 File Offset: 0x00032E48
		private void OnIrradiated(EntityUid uid, DamageableComponent component, OnIrradiatedEvent args)
		{
			FixedPoint2 damageValue = FixedPoint2.New(args.TotalRads);
			DamageSpecifier damage = new DamageSpecifier();
			foreach (string typeId in component.RadiationDamageTypeIDs)
			{
				damage.DamageDict.Add(typeId, damageValue);
			}
			this.TryChangeDamage(new EntityUid?(uid), damage, false, true, null, null);
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00034CD0 File Offset: 0x00032ED0
		private void OnRejuvenate(EntityUid uid, DamageableComponent component, RejuvenateEvent args)
		{
			this.SetAllDamage(component, 0);
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00034CE0 File Offset: 0x00032EE0
		private void DamageableHandleState(EntityUid uid, DamageableComponent component, ref ComponentHandleState args)
		{
			DamageableComponentState state = args.Current as DamageableComponentState;
			if (state == null)
			{
				return;
			}
			component.DamageModifierSetId = state.ModifierSetId;
			DamageSpecifier newDamage = new DamageSpecifier
			{
				DamageDict = new Dictionary<string, FixedPoint2>(state.DamageDict)
			};
			DamageSpecifier delta = component.Damage - newDamage;
			delta.TrimZeros();
			if (!delta.Empty)
			{
				component.Damage = newDamage;
				this.DamageChanged(component, delta, true, null);
			}
		}

		// Token: 0x04000F45 RID: 3909
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F46 RID: 3910
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000F47 RID: 3911
		[Dependency]
		private readonly INetManager _netMan;
	}
}
