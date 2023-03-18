using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.GameTicking.Presets
{
	// Token: 0x020004D0 RID: 1232
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gamePreset", 1)]
	public sealed class GamePresetPrototype : IPrototype
	{
		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x0600196B RID: 6507 RVA: 0x00086144 File Offset: 0x00084344
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x0600196C RID: 6508 RVA: 0x0008614C File Offset: 0x0008434C
		[DataField("alias", false, 1, false, false, null)]
		public string[] Alias { get; } = Array.Empty<string>();

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x0600196D RID: 6509 RVA: 0x00086154 File Offset: 0x00084354
		[DataField("name", false, 1, false, false, null)]
		public string ModeTitle { get; } = "????";

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x0600196E RID: 6510 RVA: 0x0008615C File Offset: 0x0008435C
		[DataField("description", false, 1, false, false, null)]
		public string Description { get; } = string.Empty;

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x0600196F RID: 6511 RVA: 0x00086164 File Offset: 0x00084364
		[DataField("showInVote", false, 1, false, false, null)]
		public bool ShowInVote { get; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06001970 RID: 6512 RVA: 0x0008616C File Offset: 0x0008436C
		[DataField("minPlayers", false, 1, false, false, null)]
		public int? MinPlayers { get; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x00086174 File Offset: 0x00084374
		[DataField("maxPlayers", false, 1, false, false, null)]
		public int? MaxPlayers { get; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06001972 RID: 6514 RVA: 0x0008617C File Offset: 0x0008437C
		[DataField("rules", false, 1, false, false, typeof(PrototypeIdListSerializer<GameRulePrototype>))]
		public IReadOnlyList<string> Rules { get; } = Array.Empty<string>();
	}
}
