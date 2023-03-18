using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A8 RID: 1960
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasTankComponent : Component, IGasMixtureHolder
	{
		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06002A79 RID: 10873 RVA: 0x000DFC41 File Offset: 0x000DDE41
		public bool IsLowPressure
		{
			get
			{
				GasMixture air = this.Air;
				return ((air != null) ? air.Pressure : 0f) <= this.TankLowPressure;
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06002A7A RID: 10874 RVA: 0x000DFC64 File Offset: 0x000DDE64
		// (set) Token: 0x06002A7B RID: 10875 RVA: 0x000DFC6C File Offset: 0x000DDE6C
		[DataField("air", false, 1, false, false, null)]
		public GasMixture Air { get; set; } = new GasMixture();

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06002A7C RID: 10876 RVA: 0x000DFC75 File Offset: 0x000DDE75
		// (set) Token: 0x06002A7D RID: 10877 RVA: 0x000DFC7D File Offset: 0x000DDE7D
		[DataField("tankLowPressure", false, 1, false, false, null)]
		public float TankLowPressure { get; set; }

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06002A7E RID: 10878 RVA: 0x000DFC86 File Offset: 0x000DDE86
		// (set) Token: 0x06002A7F RID: 10879 RVA: 0x000DFC8E File Offset: 0x000DDE8E
		[DataField("outputPressure", false, 1, false, false, null)]
		public float OutputPressure { get; set; } = 101.325f;

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06002A80 RID: 10880 RVA: 0x000DFC97 File Offset: 0x000DDE97
		[ViewVariables]
		public bool IsConnected
		{
			get
			{
				return this.User != null;
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06002A81 RID: 10881 RVA: 0x000DFCA4 File Offset: 0x000DDEA4
		// (set) Token: 0x06002A82 RID: 10882 RVA: 0x000DFCAC File Offset: 0x000DDEAC
		[DataField("tankLeakPressure", false, 1, false, false, null)]
		public float TankLeakPressure { get; set; } = 3039.75f;

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06002A83 RID: 10883 RVA: 0x000DFCB5 File Offset: 0x000DDEB5
		// (set) Token: 0x06002A84 RID: 10884 RVA: 0x000DFCBD File Offset: 0x000DDEBD
		[DataField("tankRupturePressure", false, 1, false, false, null)]
		public float TankRupturePressure { get; set; } = 4053f;

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06002A85 RID: 10885 RVA: 0x000DFCC6 File Offset: 0x000DDEC6
		// (set) Token: 0x06002A86 RID: 10886 RVA: 0x000DFCCE File Offset: 0x000DDECE
		[DataField("tankFragmentPressure", false, 1, false, false, null)]
		public float TankFragmentPressure { get; set; } = 5066.25f;

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06002A87 RID: 10887 RVA: 0x000DFCD7 File Offset: 0x000DDED7
		// (set) Token: 0x06002A88 RID: 10888 RVA: 0x000DFCDF File Offset: 0x000DDEDF
		[DataField("tankFragmentScale", false, 1, false, false, null)]
		public float TankFragmentScale { get; set; } = 1013.25f;

		// Token: 0x04001A49 RID: 6729
		public const float MaxExplosionRange = 80f;

		// Token: 0x04001A4A RID: 6730
		private const float DefaultLowPressure = 0f;

		// Token: 0x04001A4B RID: 6731
		private const float DefaultOutputPressure = 101.325f;

		// Token: 0x04001A4C RID: 6732
		public int Integrity = 3;

		// Token: 0x04001A4D RID: 6733
		[ViewVariables]
		[DataField("ruptureSound", false, 1, false, false, null)]
		public SoundSpecifier RuptureSound = new SoundPathSpecifier("/Audio/Effects/spray.ogg", null);

		// Token: 0x04001A4E RID: 6734
		[Nullable(2)]
		[ViewVariables]
		[DataField("connectSound", false, 1, false, false, null)]
		public SoundSpecifier ConnectSound = new SoundPathSpecifier("/Audio/Effects/internals.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(5f)
		};

		// Token: 0x04001A4F RID: 6735
		[Nullable(2)]
		[ViewVariables]
		[DataField("disconnectSound", false, 1, false, false, null)]
		public SoundSpecifier DisconnectSound;

		// Token: 0x04001A50 RID: 6736
		[Nullable(2)]
		public IPlayingAudioStream ConnectStream;

		// Token: 0x04001A51 RID: 6737
		[Nullable(2)]
		public IPlayingAudioStream DisconnectStream;

		// Token: 0x04001A55 RID: 6741
		[ViewVariables]
		public EntityUid? User;

		// Token: 0x04001A56 RID: 6742
		[ViewVariables]
		public bool CheckUser;

		// Token: 0x04001A5B RID: 6747
		[DataField("toggleAction", false, 1, true, false, null)]
		public InstantAction ToggleAction = new InstantAction();
	}
}
