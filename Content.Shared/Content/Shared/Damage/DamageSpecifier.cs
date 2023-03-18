using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage
{
	// Token: 0x02000530 RID: 1328
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class DamageSpecifier : IEquatable<DamageSpecifier>
	{
		// Token: 0x17000331 RID: 817
		// (get) Token: 0x0600100E RID: 4110 RVA: 0x00033AF3 File Offset: 0x00031CF3
		// (set) Token: 0x0600100F RID: 4111 RVA: 0x00033AFB File Offset: 0x00031CFB
		[JsonIgnore]
		[ViewVariables]
		[IncludeDataField(true, 1, false, typeof(DamageSpecifierDictionarySerializer))]
		public Dictionary<string, FixedPoint2> DamageDict { get; set; } = new Dictionary<string, FixedPoint2>();

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00033B04 File Offset: 0x00031D04
		[JsonIgnore]
		public FixedPoint2 Total
		{
			get
			{
				return this.DamageDict.Values.Sum();
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06001011 RID: 4113 RVA: 0x00033B16 File Offset: 0x00031D16
		[JsonIgnore]
		public bool Empty
		{
			get
			{
				return this.DamageDict.Count == 0;
			}
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x00033B26 File Offset: 0x00031D26
		public DamageSpecifier()
		{
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x00033B39 File Offset: 0x00031D39
		public DamageSpecifier(DamageSpecifier damageSpec)
		{
			this.DamageDict = new Dictionary<string, FixedPoint2>(damageSpec.DamageDict);
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x00033B5D File Offset: 0x00031D5D
		public DamageSpecifier(DamageTypePrototype type, FixedPoint2 value)
		{
			this.DamageDict = new Dictionary<string, FixedPoint2>
			{
				{
					type.ID,
					value
				}
			};
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x00033B88 File Offset: 0x00031D88
		public DamageSpecifier(DamageGroupPrototype group, FixedPoint2 value)
		{
			int remainingTypes = group.DamageTypes.Count;
			FixedPoint2 remainingDamage = value;
			foreach (string damageType in group.DamageTypes)
			{
				FixedPoint2 damage = remainingDamage / FixedPoint2.New(remainingTypes);
				this.DamageDict.Add(damageType, damage);
				remainingDamage -= damage;
				remainingTypes--;
			}
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00033C1C File Offset: 0x00031E1C
		public static DamageSpecifier ApplyModifierSet(DamageSpecifier damageSpec, DamageModifierSet modifierSet)
		{
			DamageSpecifier newDamage = new DamageSpecifier(damageSpec);
			foreach (KeyValuePair<string, FixedPoint2> entry in newDamage.DamageDict)
			{
				if (!(entry.Value <= 0))
				{
					float newValue = entry.Value.Float();
					float reduction;
					if (modifierSet.FlatReduction.TryGetValue(entry.Key, out reduction))
					{
						newValue -= reduction;
						if (newValue <= 0f)
						{
							newDamage.DamageDict[entry.Key] = FixedPoint2.Zero;
							continue;
						}
					}
					float coefficient;
					if (modifierSet.Coefficients.TryGetValue(entry.Key, out coefficient))
					{
						newValue *= coefficient;
					}
					newDamage.DamageDict[entry.Key] = FixedPoint2.New(newValue);
				}
			}
			newDamage.TrimZeros();
			return newDamage;
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x00033D10 File Offset: 0x00031F10
		public static DamageSpecifier ApplyModifierSets(DamageSpecifier damageSpec, IEnumerable<DamageModifierSet> modifierSets)
		{
			DamageSpecifier newDamage = new DamageSpecifier(damageSpec);
			foreach (DamageModifierSet set in modifierSets)
			{
				newDamage = DamageSpecifier.ApplyModifierSet(newDamage, set);
			}
			return newDamage;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00033D64 File Offset: 0x00031F64
		public void TrimZeros()
		{
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in this.DamageDict)
			{
				string text;
				FixedPoint2 a;
				keyValuePair.Deconstruct(out text, out a);
				string key = text;
				if (a == 0)
				{
					this.DamageDict.Remove(key);
				}
			}
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x00033DD4 File Offset: 0x00031FD4
		public void Clamp(FixedPoint2 minValue, FixedPoint2 maxValue)
		{
			this.ClampMax(maxValue);
			this.ClampMin(minValue);
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x00033DE4 File Offset: 0x00031FE4
		public void ClampMin(FixedPoint2 minValue)
		{
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in this.DamageDict)
			{
				string text;
				FixedPoint2 a;
				keyValuePair.Deconstruct(out text, out a);
				string key = text;
				if (a < minValue)
				{
					this.DamageDict[key] = minValue;
				}
			}
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x00033E54 File Offset: 0x00032054
		public void ClampMax(FixedPoint2 maxValue)
		{
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in this.DamageDict)
			{
				string text;
				FixedPoint2 a;
				keyValuePair.Deconstruct(out text, out a);
				string key = text;
				if (a > maxValue)
				{
					this.DamageDict[key] = maxValue;
				}
			}
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x00033EC4 File Offset: 0x000320C4
		public void ExclusiveAdd(DamageSpecifier other)
		{
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in other.DamageDict)
			{
				string text;
				FixedPoint2 fixedPoint;
				keyValuePair.Deconstruct(out text, out fixedPoint);
				string type = text;
				FixedPoint2 value = fixedPoint;
				if (this.DamageDict.ContainsKey(type))
				{
					Dictionary<string, FixedPoint2> damageDict = this.DamageDict;
					text = type;
					damageDict[text] += value;
				}
			}
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x00033F54 File Offset: 0x00032154
		public bool TryGetDamageInGroup(DamageGroupPrototype group, out FixedPoint2 total)
		{
			bool containsMemeber = false;
			total = FixedPoint2.Zero;
			foreach (string type in group.DamageTypes)
			{
				FixedPoint2 value;
				if (this.DamageDict.TryGetValue(type, out value))
				{
					total += value;
					containsMemeber = true;
				}
			}
			return containsMemeber;
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x00033FD4 File Offset: 0x000321D4
		public Dictionary<string, FixedPoint2> GetDamagePerGroup([Nullable(2)] IPrototypeManager protoManager = null)
		{
			IoCManager.Resolve<IPrototypeManager>(ref protoManager);
			Dictionary<string, FixedPoint2> damageGroupDict = new Dictionary<string, FixedPoint2>();
			foreach (DamageGroupPrototype group in protoManager.EnumeratePrototypes<DamageGroupPrototype>())
			{
				FixedPoint2 value;
				if (this.TryGetDamageInGroup(group, out value))
				{
					damageGroupDict.Add(group.ID, value);
				}
			}
			return damageGroupDict;
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x00034040 File Offset: 0x00032240
		public static DamageSpecifier operator *(DamageSpecifier damageSpec, FixedPoint2 factor)
		{
			DamageSpecifier newDamage = new DamageSpecifier();
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
			{
				newDamage.DamageDict.Add(entry.Key, entry.Value * factor);
			}
			return newDamage;
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000340B4 File Offset: 0x000322B4
		public static DamageSpecifier operator *(DamageSpecifier damageSpec, float factor)
		{
			DamageSpecifier newDamage = new DamageSpecifier();
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
			{
				newDamage.DamageDict.Add(entry.Key, entry.Value * factor);
			}
			return newDamage;
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x00034128 File Offset: 0x00032328
		public static DamageSpecifier operator /(DamageSpecifier damageSpec, FixedPoint2 factor)
		{
			DamageSpecifier newDamage = new DamageSpecifier();
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
			{
				newDamage.DamageDict.Add(entry.Key, entry.Value / factor);
			}
			return newDamage;
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x0003419C File Offset: 0x0003239C
		public static DamageSpecifier operator /(DamageSpecifier damageSpec, float factor)
		{
			DamageSpecifier newDamage = new DamageSpecifier();
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpec.DamageDict)
			{
				newDamage.DamageDict.Add(entry.Key, entry.Value / factor);
			}
			return newDamage;
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x00034210 File Offset: 0x00032410
		public static DamageSpecifier operator +(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
		{
			DamageSpecifier newDamage = new DamageSpecifier(damageSpecA);
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpecB.DamageDict)
			{
				if (!newDamage.DamageDict.TryAdd(entry.Key, entry.Value))
				{
					Dictionary<string, FixedPoint2> damageDict = newDamage.DamageDict;
					string key = entry.Key;
					damageDict[key] += entry.Value;
				}
			}
			return newDamage;
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x000342AC File Offset: 0x000324AC
		public static DamageSpecifier operator -(DamageSpecifier damageSpecA, DamageSpecifier damageSpecB)
		{
			DamageSpecifier newDamage = new DamageSpecifier(damageSpecA);
			foreach (KeyValuePair<string, FixedPoint2> entry in damageSpecB.DamageDict)
			{
				if (!newDamage.DamageDict.TryAdd(entry.Key, -entry.Value))
				{
					Dictionary<string, FixedPoint2> damageDict = newDamage.DamageDict;
					string key = entry.Key;
					damageDict[key] -= entry.Value;
				}
			}
			return newDamage;
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x0003434C File Offset: 0x0003254C
		public static DamageSpecifier operator +(DamageSpecifier damageSpec)
		{
			return damageSpec;
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x0003434F File Offset: 0x0003254F
		public static DamageSpecifier operator -(DamageSpecifier damageSpec)
		{
			return damageSpec * -1f;
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0003435C File Offset: 0x0003255C
		public static DamageSpecifier operator *(float factor, DamageSpecifier damageSpec)
		{
			return damageSpec * factor;
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00034365 File Offset: 0x00032565
		public static DamageSpecifier operator *(FixedPoint2 factor, DamageSpecifier damageSpec)
		{
			return damageSpec * factor;
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00034370 File Offset: 0x00032570
		[NullableContext(2)]
		public bool Equals(DamageSpecifier other)
		{
			if (other == null || this.DamageDict.Count != other.DamageDict.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, FixedPoint2> keyValuePair in this.DamageDict)
			{
				string text;
				FixedPoint2 fixedPoint;
				keyValuePair.Deconstruct(out text, out fixedPoint);
				string key = text;
				FixedPoint2 value = fixedPoint;
				FixedPoint2 otherValue;
				if (!other.DamageDict.TryGetValue(key, out otherValue) || value != otherValue)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000F3A RID: 3898
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("types")]
		[DataField("types", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>))]
		private readonly Dictionary<string, FixedPoint2> _damageTypeDictionary;

		// Token: 0x04000F3B RID: 3899
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("groups")]
		[DataField("groups", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>))]
		private readonly Dictionary<string, FixedPoint2> _damageGroupDictionary;
	}
}
