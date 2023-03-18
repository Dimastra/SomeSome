using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000377 RID: 887
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedExpendableLightComponent : Component
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x00022351 File Offset: 0x00020551
		// (set) Token: 0x06000A4D RID: 2637 RVA: 0x00022359 File Offset: 0x00020559
		[ViewVariables]
		public ExpendableLightState CurrentState { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00022362 File Offset: 0x00020562
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x0002236A File Offset: 0x0002056A
		[DataField("turnOnBehaviourID", false, 1, false, false, null)]
		public string TurnOnBehaviourID { get; set; } = string.Empty;

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000A50 RID: 2640 RVA: 0x00022373 File Offset: 0x00020573
		// (set) Token: 0x06000A51 RID: 2641 RVA: 0x0002237B File Offset: 0x0002057B
		[DataField("fadeOutBehaviourID", false, 1, false, false, null)]
		public string FadeOutBehaviourID { get; set; } = string.Empty;

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x00022384 File Offset: 0x00020584
		// (set) Token: 0x06000A53 RID: 2643 RVA: 0x0002238C File Offset: 0x0002058C
		[DataField("glowDuration", false, 1, false, false, null)]
		public float GlowDuration { get; set; } = 900f;

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000A54 RID: 2644 RVA: 0x00022395 File Offset: 0x00020595
		// (set) Token: 0x06000A55 RID: 2645 RVA: 0x0002239D File Offset: 0x0002059D
		[DataField("fadeOutDuration", false, 1, false, false, null)]
		public float FadeOutDuration { get; set; } = 300f;

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x000223A6 File Offset: 0x000205A6
		// (set) Token: 0x06000A57 RID: 2647 RVA: 0x000223AE File Offset: 0x000205AE
		[DataField("spentDesc", false, 1, false, false, null)]
		public string SpentDesc { get; set; } = string.Empty;

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x000223B7 File Offset: 0x000205B7
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x000223BF File Offset: 0x000205BF
		[DataField("spentName", false, 1, false, false, null)]
		public string SpentName { get; set; } = string.Empty;

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x000223C8 File Offset: 0x000205C8
		// (set) Token: 0x06000A5B RID: 2651 RVA: 0x000223D0 File Offset: 0x000205D0
		[Nullable(2)]
		[DataField("litSound", false, 1, false, false, null)]
		public SoundSpecifier LitSound { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000A5C RID: 2652 RVA: 0x000223D9 File Offset: 0x000205D9
		// (set) Token: 0x06000A5D RID: 2653 RVA: 0x000223E1 File Offset: 0x000205E1
		[Nullable(2)]
		[DataField("loopedSound", false, 1, false, false, null)]
		public SoundSpecifier LoopedSound { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000A5E RID: 2654 RVA: 0x000223EA File Offset: 0x000205EA
		// (set) Token: 0x06000A5F RID: 2655 RVA: 0x000223F2 File Offset: 0x000205F2
		[Nullable(2)]
		[DataField("dieSound", false, 1, false, false, null)]
		public SoundSpecifier DieSound { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x04000A32 RID: 2610
		public static readonly AudioParams LoopedSoundParams = new AudioParams(0f, 1f, "Master", 62.5f, 1f, 1f, true, 0.3f, null);
	}
}
