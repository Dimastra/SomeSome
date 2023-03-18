using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.GuideGenerator
{
	// Token: 0x0200047D RID: 1149
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReagentEntry
	{
		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060016F0 RID: 5872 RVA: 0x00079011 File Offset: 0x00077211
		[JsonPropertyName("id")]
		public string Id { get; }

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x00079019 File Offset: 0x00077219
		[JsonPropertyName("name")]
		public string Name { get; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060016F2 RID: 5874 RVA: 0x00079021 File Offset: 0x00077221
		[JsonPropertyName("group")]
		public string Group { get; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060016F3 RID: 5875 RVA: 0x00079029 File Offset: 0x00077229
		[JsonPropertyName("desc")]
		public string Description { get; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060016F4 RID: 5876 RVA: 0x00079031 File Offset: 0x00077231
		[JsonPropertyName("physicalDesc")]
		public string PhysicalDescription { get; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060016F5 RID: 5877 RVA: 0x00079039 File Offset: 0x00077239
		[JsonPropertyName("color")]
		public string SubstanceColor { get; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060016F6 RID: 5878 RVA: 0x00079041 File Offset: 0x00077241
		[JsonPropertyName("recipes")]
		public List<string> Recipes { get; } = new List<string>();

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x00079049 File Offset: 0x00077249
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[JsonPropertyName("metabolisms")]
		public Dictionary<string, ReagentEffectsEntry> Metabolisms { [return: Nullable(new byte[]
		{
			2,
			1,
			1
		})] get; }

		// Token: 0x060016F8 RID: 5880 RVA: 0x00079054 File Offset: 0x00077254
		public ReagentEntry(ReagentPrototype proto)
		{
			this.Id = proto.ID;
			this.Name = proto.LocalizedName;
			this.Group = proto.Group;
			this.Description = proto.LocalizedDescription;
			this.PhysicalDescription = proto.LocalizedPhysicalDescription;
			this.SubstanceColor = proto.SubstanceColor.ToHex();
			this.Metabolisms = proto.Metabolisms;
		}
	}
}
