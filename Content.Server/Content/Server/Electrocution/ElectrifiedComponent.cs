using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Electrocution
{
	// Token: 0x02000531 RID: 1329
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ElectrifiedComponent : Component
	{
		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001BA4 RID: 7076 RVA: 0x00093C0E File Offset: 0x00091E0E
		// (set) Token: 0x06001BA5 RID: 7077 RVA: 0x00093C16 File Offset: 0x00091E16
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06001BA6 RID: 7078 RVA: 0x00093C1F File Offset: 0x00091E1F
		// (set) Token: 0x06001BA7 RID: 7079 RVA: 0x00093C27 File Offset: 0x00091E27
		[DataField("onBump", false, 1, false, false, null)]
		public bool OnBump { get; set; } = true;

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001BA8 RID: 7080 RVA: 0x00093C30 File Offset: 0x00091E30
		// (set) Token: 0x06001BA9 RID: 7081 RVA: 0x00093C38 File Offset: 0x00091E38
		[DataField("onAttacked", false, 1, false, false, null)]
		public bool OnAttacked { get; set; } = true;

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06001BAA RID: 7082 RVA: 0x00093C41 File Offset: 0x00091E41
		// (set) Token: 0x06001BAB RID: 7083 RVA: 0x00093C49 File Offset: 0x00091E49
		[DataField("noWindowInTile", false, 1, false, false, null)]
		public bool NoWindowInTile { get; set; }

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06001BAC RID: 7084 RVA: 0x00093C52 File Offset: 0x00091E52
		// (set) Token: 0x06001BAD RID: 7085 RVA: 0x00093C5A File Offset: 0x00091E5A
		[DataField("onHandInteract", false, 1, false, false, null)]
		public bool OnHandInteract { get; set; } = true;

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06001BAE RID: 7086 RVA: 0x00093C63 File Offset: 0x00091E63
		// (set) Token: 0x06001BAF RID: 7087 RVA: 0x00093C6B File Offset: 0x00091E6B
		[DataField("onInteractUsing", false, 1, false, false, null)]
		public bool OnInteractUsing { get; set; } = true;

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x00093C74 File Offset: 0x00091E74
		[DataField("requirePower", false, 1, false, false, null)]
		public bool RequirePower { get; } = 1;

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x00093C7C File Offset: 0x00091E7C
		[DataField("usesApcPower", false, 1, false, false, null)]
		public bool UsesApcPower { get; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001BB2 RID: 7090 RVA: 0x00093C84 File Offset: 0x00091E84
		[DataField("highVoltageNode", false, 1, false, false, null)]
		public string HighVoltageNode { get; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x00093C8C File Offset: 0x00091E8C
		[DataField("mediumVoltageNode", false, 1, false, false, null)]
		public string MediumVoltageNode { get; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06001BB4 RID: 7092 RVA: 0x00093C94 File Offset: 0x00091E94
		[DataField("lowVoltageNode", false, 1, false, false, null)]
		public string LowVoltageNode { get; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06001BB5 RID: 7093 RVA: 0x00093C9C File Offset: 0x00091E9C
		[DataField("highVoltageDamageMultiplier", false, 1, false, false, null)]
		public float HighVoltageDamageMultiplier { get; } = 3f;

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x00093CA4 File Offset: 0x00091EA4
		[DataField("highVoltageTimeMultiplier", false, 1, false, false, null)]
		public float HighVoltageTimeMultiplier { get; } = 1.5f;

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x00093CAC File Offset: 0x00091EAC
		[DataField("mediumVoltageDamageMultiplier", false, 1, false, false, null)]
		public float MediumVoltageDamageMultiplier { get; } = 2f;

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x00093CB4 File Offset: 0x00091EB4
		[DataField("mediumVoltageTimeMultiplier", false, 1, false, false, null)]
		public float MediumVoltageTimeMultiplier { get; } = 1.25f;

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x00093CBC File Offset: 0x00091EBC
		[DataField("shockDamage", false, 1, false, false, null)]
		public int ShockDamage { get; } = 20;

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06001BBA RID: 7098 RVA: 0x00093CC4 File Offset: 0x00091EC4
		[DataField("shockTime", false, 1, false, false, null)]
		public float ShockTime { get; } = 8f;

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x00093CCC File Offset: 0x00091ECC
		[DataField("siemensCoefficient", false, 1, false, false, null)]
		public float SiemensCoefficient { get; } = 1f;

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x00093CD4 File Offset: 0x00091ED4
		[Nullable(1)]
		[DataField("shockNoises", false, 1, false, false, null)]
		public SoundSpecifier ShockNoises { [NullableContext(1)] get; } = new SoundCollectionSpecifier("sparks", null);

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x00093CDC File Offset: 0x00091EDC
		[DataField("playSoundOnShock", false, 1, false, false, null)]
		public bool PlaySoundOnShock { get; } = 1;

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06001BBE RID: 7102 RVA: 0x00093CE4 File Offset: 0x00091EE4
		[DataField("shockVolume", false, 1, false, false, null)]
		public float ShockVolume { get; } = 20f;
	}
}
