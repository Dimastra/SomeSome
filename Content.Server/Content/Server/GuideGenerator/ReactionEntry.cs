using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;

namespace Content.Server.GuideGenerator
{
	// Token: 0x0200047E RID: 1150
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReactionEntry
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x000790CE File Offset: 0x000772CE
		[JsonPropertyName("id")]
		public string Id { get; }

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x000790D6 File Offset: 0x000772D6
		[JsonPropertyName("name")]
		public string Name { get; }

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x000790DE File Offset: 0x000772DE
		[JsonPropertyName("reactants")]
		public Dictionary<string, ReactantEntry> Reactants { get; }

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x000790E6 File Offset: 0x000772E6
		[JsonPropertyName("products")]
		public Dictionary<string, float> Products { get; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060016FD RID: 5885 RVA: 0x000790EE File Offset: 0x000772EE
		[JsonPropertyName("effects")]
		public List<ReagentEffect> Effects { get; }

		// Token: 0x060016FE RID: 5886 RVA: 0x000790F8 File Offset: 0x000772F8
		public ReactionEntry(ReactionPrototype proto)
		{
			this.Id = proto.ID;
			this.Name = proto.Name;
			this.Reactants = (from x in proto.Reactants
			select KeyValuePair.Create<string, ReactantEntry>(x.Key, new ReactantEntry(x.Value.Amount.Float(), x.Value.Catalyst))).ToDictionary((KeyValuePair<string, ReactantEntry> x) => x.Key, (KeyValuePair<string, ReactantEntry> x) => x.Value);
			this.Products = (from x in proto.Products
			select KeyValuePair.Create<string, float>(x.Key, x.Value.Float())).ToDictionary((KeyValuePair<string, float> x) => x.Key, (KeyValuePair<string, float> x) => x.Value);
			this.Effects = proto.Effects;
		}
	}
}
