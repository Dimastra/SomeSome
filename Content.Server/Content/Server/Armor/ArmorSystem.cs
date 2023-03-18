using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Systems;
using Content.Server.Examine;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Armor
{
	// Token: 0x020007BB RID: 1979
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArmorSystem : EntitySystem
	{
		// Token: 0x06002AE2 RID: 10978 RVA: 0x000E0B90 File Offset: 0x000DED90
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>(new ComponentEventHandler<ArmorComponent, InventoryRelayedEvent<DamageModifyEvent>>(this.OnDamageModify), null, null);
			base.SubscribeLocalEvent<ArmorComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<ArmorComponent, GetVerbsEvent<ExamineVerb>>(this.OnArmorVerbExamine), null, null);
			base.SubscribeLocalEvent<ArmorComponent, PriceCalculationEvent>(new ComponentEventRefHandler<ArmorComponent, PriceCalculationEvent>(this.GetArmorPrice), null, null);
		}

		// Token: 0x06002AE3 RID: 10979 RVA: 0x000E0BE0 File Offset: 0x000DEDE0
		private void GetArmorPrice(EntityUid uid, ArmorComponent component, ref PriceCalculationEvent args)
		{
			if (component.Modifiers == null)
			{
				return;
			}
			double price = 0.0;
			foreach (KeyValuePair<string, float> modifier in component.Modifiers.Coefficients)
			{
				DamageTypePrototype damageType;
				this._protoManager.TryIndex<DamageTypePrototype>(modifier.Key, ref damageType);
				if (damageType != null)
				{
					price += damageType.ArmorPriceCoefficient * 100.0 * (double)(1f - modifier.Value);
				}
				else
				{
					price += 200.0 * (double)(1f - modifier.Value);
				}
			}
			foreach (KeyValuePair<string, float> modifier2 in component.Modifiers.FlatReduction)
			{
				DamageTypePrototype damageType2;
				this._protoManager.TryIndex<DamageTypePrototype>(modifier2.Key, ref damageType2);
				if (damageType2 != null)
				{
					price += damageType2.ArmorPriceFlat * (double)modifier2.Value;
				}
				else
				{
					price += 10.0 * (double)modifier2.Value;
				}
			}
			args.Price += price;
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x000E0D2C File Offset: 0x000DEF2C
		private void OnDamageModify(EntityUid uid, ArmorComponent component, InventoryRelayedEvent<DamageModifyEvent> args)
		{
			args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, component.Modifiers);
		}

		// Token: 0x06002AE5 RID: 10981 RVA: 0x000E0D50 File Offset: 0x000DEF50
		private void OnArmorVerbExamine(EntityUid uid, ArmorComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			DamageModifierSet armorModifiers = component.Modifiers;
			if (armorModifiers == null)
			{
				return;
			}
			FormattedMessage examineMarkup = ArmorSystem.GetArmorExamine(armorModifiers);
			this._examine.AddDetailedExamineVerb(args, component, examineMarkup, Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/VerbIcons/dot.svg.192dpi.png", Loc.GetString("armor-examinable-verb-message"));
		}

		// Token: 0x06002AE6 RID: 10982 RVA: 0x000E0DA8 File Offset: 0x000DEFA8
		private static FormattedMessage GetArmorExamine(DamageModifierSet armorModifiers)
		{
			FormattedMessage msg = new FormattedMessage();
			msg.AddMarkup(Loc.GetString("armor-examine"));
			foreach (KeyValuePair<string, float> coefficientArmor in armorModifiers.Coefficients)
			{
				msg.PushNewline();
				msg.AddMarkup(Loc.GetString("armor-coefficient-value", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("type", coefficientArmor.Key),
					new ValueTuple<string, object>("value", MathF.Round((1f - coefficientArmor.Value) * 100f, 1))
				}));
			}
			foreach (KeyValuePair<string, float> flatArmor in armorModifiers.FlatReduction)
			{
				msg.PushNewline();
				msg.AddMarkup(Loc.GetString("armor-reduction-value", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("type", flatArmor.Key),
					new ValueTuple<string, object>("value", flatArmor.Value)
				}));
			}
			return msg;
		}

		// Token: 0x04001A87 RID: 6791
		private const double CoefDefaultPrice = 2.0;

		// Token: 0x04001A88 RID: 6792
		private const double FlatDefaultPrice = 10.0;

		// Token: 0x04001A89 RID: 6793
		[Dependency]
		private readonly ExamineSystem _examine;

		// Token: 0x04001A8A RID: 6794
		[Dependency]
		private readonly IPrototypeManager _protoManager;
	}
}
