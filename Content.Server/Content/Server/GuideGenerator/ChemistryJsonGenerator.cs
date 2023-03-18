using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.GuideGenerator
{
	// Token: 0x0200047B RID: 1147
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemistryJsonGenerator
	{
		// Token: 0x060016EC RID: 5868 RVA: 0x00078D94 File Offset: 0x00076F94
		public static void PublishJson(StreamWriter file)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			Dictionary<string, ReagentEntry> prototypes = (from x in prototypeManager.EnumeratePrototypes<ReagentPrototype>()
			where !x.Abstract
			select new ReagentEntry(x)).ToDictionary((ReagentEntry x) => x.Id, (ReagentEntry x) => x);
			foreach (ReactionPrototype reaction in from x in prototypeManager.EnumeratePrototypes<ReactionPrototype>()
			where x.Products.Count != 0
			select x)
			{
				foreach (string product in reaction.Products.Keys)
				{
					prototypes[product].Recipes.Add(reaction.ID);
				}
			}
			JsonSerializerOptions serializeOptions = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = 
				{
					new UniversalJsonConverter<ReagentEffect>(),
					new UniversalJsonConverter<ReagentEffectCondition>(),
					new UniversalJsonConverter<ReagentEffectsEntry>(),
					new UniversalJsonConverter<DamageSpecifier>(),
					new ChemistryJsonGenerator.FixedPointJsonConverter()
				}
			};
			file.Write(JsonSerializer.Serialize<Dictionary<string, ReagentEntry>>(prototypes, serializeOptions));
		}

		// Token: 0x020009D3 RID: 2515
		[Nullable(0)]
		public sealed class FixedPointJsonConverter : JsonConverter<FixedPoint2>
		{
			// Token: 0x0600337A RID: 13178 RVA: 0x00107ADA File Offset: 0x00105CDA
			public override void Write(Utf8JsonWriter writer, FixedPoint2 value, JsonSerializerOptions options)
			{
				writer.WriteNumberValue(value.Float());
			}

			// Token: 0x0600337B RID: 13179 RVA: 0x00107AE9 File Offset: 0x00105CE9
			public override FixedPoint2 Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
			{
				throw new NotSupportedException();
			}
		}
	}
}
