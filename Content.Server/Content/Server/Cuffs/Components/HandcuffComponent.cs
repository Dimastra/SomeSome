using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Shared.Cuffs.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Cuffs.Components
{
	// Token: 0x020005D6 RID: 1494
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedHandcuffComponent))]
	public sealed class HandcuffComponent : SharedHandcuffComponent
	{
		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06001FCD RID: 8141 RVA: 0x000A6C72 File Offset: 0x000A4E72
		// (set) Token: 0x06001FCE RID: 8142 RVA: 0x000A6C7A File Offset: 0x000A4E7A
		[DataField("cuffTime", false, 1, false, false, null)]
		public float CuffTime { get; set; } = 3.5f;

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06001FCF RID: 8143 RVA: 0x000A6C83 File Offset: 0x000A4E83
		// (set) Token: 0x06001FD0 RID: 8144 RVA: 0x000A6C8B File Offset: 0x000A4E8B
		[DataField("uncuffTime", false, 1, false, false, null)]
		public float UncuffTime { get; set; } = 3.5f;

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x000A6C94 File Offset: 0x000A4E94
		// (set) Token: 0x06001FD2 RID: 8146 RVA: 0x000A6C9C File Offset: 0x000A4E9C
		[DataField("breakoutTime", false, 1, false, false, null)]
		public float BreakoutTime { get; set; } = 30f;

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001FD3 RID: 8147 RVA: 0x000A6CA5 File Offset: 0x000A4EA5
		// (set) Token: 0x06001FD4 RID: 8148 RVA: 0x000A6CAD File Offset: 0x000A4EAD
		[DataField("stunBonus", false, 1, false, false, null)]
		public float StunBonus { get; set; } = 2f;

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06001FD5 RID: 8149 RVA: 0x000A6CB6 File Offset: 0x000A4EB6
		// (set) Token: 0x06001FD6 RID: 8150 RVA: 0x000A6CBE File Offset: 0x000A4EBE
		[DataField("breakOnRemove", false, 1, false, false, null)]
		public bool BreakOnRemove { get; set; }

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06001FD7 RID: 8151 RVA: 0x000A6CC7 File Offset: 0x000A4EC7
		// (set) Token: 0x06001FD8 RID: 8152 RVA: 0x000A6CCF File Offset: 0x000A4ECF
		[Nullable(2)]
		[DataField("brokenPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BrokenPrototype { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06001FD9 RID: 8153 RVA: 0x000A6CD8 File Offset: 0x000A4ED8
		// (set) Token: 0x06001FDA RID: 8154 RVA: 0x000A6CE0 File Offset: 0x000A4EE0
		[Nullable(2)]
		[DataField("cuffedRSI", false, 1, false, false, null)]
		public string CuffedRSI { [NullableContext(2)] get; [NullableContext(2)] set; } = "Objects/Misc/handcuffs.rsi";

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06001FDB RID: 8155 RVA: 0x000A6CE9 File Offset: 0x000A4EE9
		// (set) Token: 0x06001FDC RID: 8156 RVA: 0x000A6CF1 File Offset: 0x000A4EF1
		[Nullable(2)]
		[DataField("bodyIconState", false, 1, false, false, null)]
		public string OverlayIconState { [NullableContext(2)] get; [NullableContext(2)] set; } = "body-overlay";

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001FDD RID: 8157 RVA: 0x000A6CFA File Offset: 0x000A4EFA
		// (set) Token: 0x06001FDE RID: 8158 RVA: 0x000A6D02 File Offset: 0x000A4F02
		[DataField("startCuffSound", false, 1, false, false, null)]
		public SoundSpecifier StartCuffSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_start.ogg", null);

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001FDF RID: 8159 RVA: 0x000A6D0B File Offset: 0x000A4F0B
		// (set) Token: 0x06001FE0 RID: 8160 RVA: 0x000A6D13 File Offset: 0x000A4F13
		[DataField("endCuffSound", false, 1, false, false, null)]
		public SoundSpecifier EndCuffSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_end.ogg", null);

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06001FE1 RID: 8161 RVA: 0x000A6D1C File Offset: 0x000A4F1C
		// (set) Token: 0x06001FE2 RID: 8162 RVA: 0x000A6D24 File Offset: 0x000A4F24
		[DataField("startBreakoutSound", false, 1, false, false, null)]
		public SoundSpecifier StartBreakoutSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_breakout_start.ogg", null);

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06001FE3 RID: 8163 RVA: 0x000A6D2D File Offset: 0x000A4F2D
		// (set) Token: 0x06001FE4 RID: 8164 RVA: 0x000A6D35 File Offset: 0x000A4F35
		[DataField("startUncuffSound", false, 1, false, false, null)]
		public SoundSpecifier StartUncuffSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_takeoff_start.ogg", null);

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06001FE5 RID: 8165 RVA: 0x000A6D3E File Offset: 0x000A4F3E
		// (set) Token: 0x06001FE6 RID: 8166 RVA: 0x000A6D46 File Offset: 0x000A4F46
		[DataField("endUncuffSound", false, 1, false, false, null)]
		public SoundSpecifier EndUncuffSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Handcuffs/cuff_takeoff_end.ogg", null);

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06001FE7 RID: 8167 RVA: 0x000A6D4F File Offset: 0x000A4F4F
		// (set) Token: 0x06001FE8 RID: 8168 RVA: 0x000A6D57 File Offset: 0x000A4F57
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; set; } = Color.White;

		// Token: 0x06001FE9 RID: 8169 RVA: 0x000A6D60 File Offset: 0x000A4F60
		public void TryUpdateCuff(EntityUid user, EntityUid target, CuffableComponent cuffs)
		{
			HandcuffComponent.<TryUpdateCuff>d__59 <TryUpdateCuff>d__;
			<TryUpdateCuff>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TryUpdateCuff>d__.<>4__this = this;
			<TryUpdateCuff>d__.user = user;
			<TryUpdateCuff>d__.target = target;
			<TryUpdateCuff>d__.cuffs = cuffs;
			<TryUpdateCuff>d__.<>1__state = -1;
			<TryUpdateCuff>d__.<>t__builder.Start<HandcuffComponent.<TryUpdateCuff>d__59>(ref <TryUpdateCuff>d__);
		}

		// Token: 0x040013C0 RID: 5056
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x040013C1 RID: 5057
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040013D0 RID: 5072
		public bool Cuffing;
	}
}
