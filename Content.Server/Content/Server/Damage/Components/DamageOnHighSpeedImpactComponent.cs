using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005C9 RID: 1481
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	internal sealed class DamageOnHighSpeedImpactComponent : Component
	{
		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001F97 RID: 8087 RVA: 0x000A5B23 File Offset: 0x000A3D23
		// (set) Token: 0x06001F98 RID: 8088 RVA: 0x000A5B2B File Offset: 0x000A3D2B
		[DataField("minimumSpeed", false, 1, false, false, null)]
		public float MinimumSpeed { get; set; } = 20f;

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001F99 RID: 8089 RVA: 0x000A5B34 File Offset: 0x000A3D34
		// (set) Token: 0x06001F9A RID: 8090 RVA: 0x000A5B3C File Offset: 0x000A3D3C
		[DataField("factor", false, 1, false, false, null)]
		public float Factor { get; set; } = 0.5f;

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001F9B RID: 8091 RVA: 0x000A5B45 File Offset: 0x000A3D45
		// (set) Token: 0x06001F9C RID: 8092 RVA: 0x000A5B4D File Offset: 0x000A3D4D
		[DataField("soundHit", false, 1, true, false, null)]
		public SoundSpecifier SoundHit { get; set; }

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06001F9D RID: 8093 RVA: 0x000A5B56 File Offset: 0x000A3D56
		// (set) Token: 0x06001F9E RID: 8094 RVA: 0x000A5B5E File Offset: 0x000A3D5E
		[DataField("stunChance", false, 1, false, false, null)]
		public float StunChance { get; set; } = 0.25f;

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06001F9F RID: 8095 RVA: 0x000A5B67 File Offset: 0x000A3D67
		// (set) Token: 0x06001FA0 RID: 8096 RVA: 0x000A5B6F File Offset: 0x000A3D6F
		[DataField("stunMinimumDamage", false, 1, false, false, null)]
		public int StunMinimumDamage { get; set; } = 10;

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06001FA1 RID: 8097 RVA: 0x000A5B78 File Offset: 0x000A3D78
		// (set) Token: 0x06001FA2 RID: 8098 RVA: 0x000A5B80 File Offset: 0x000A3D80
		[DataField("stunSeconds", false, 1, false, false, null)]
		public float StunSeconds { get; set; } = 1f;

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001FA3 RID: 8099 RVA: 0x000A5B89 File Offset: 0x000A3D89
		// (set) Token: 0x06001FA4 RID: 8100 RVA: 0x000A5B91 File Offset: 0x000A3D91
		[DataField("damageCooldown", false, 1, false, false, null)]
		public float DamageCooldown { get; set; } = 2f;

		// Token: 0x040013A0 RID: 5024
		internal TimeSpan LastHit = TimeSpan.Zero;

		// Token: 0x040013A1 RID: 5025
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
