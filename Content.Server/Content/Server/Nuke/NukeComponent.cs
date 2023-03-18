using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Explosion;
using Content.Shared.Nuke;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nuke
{
	// Token: 0x02000326 RID: 806
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(NukeSystem)
	})]
	public sealed class NukeComponent : SharedNukeComponent
	{
		// Token: 0x040009C5 RID: 2501
		[DataField("timer", false, 1, false, false, null)]
		[ViewVariables]
		public int Timer = 300;

		// Token: 0x040009C6 RID: 2502
		[DataField("cooldown", false, 1, false, false, null)]
		public int Cooldown = 30;

		// Token: 0x040009C7 RID: 2503
		[DataField("diskSlot", false, 1, false, false, null)]
		public ItemSlot DiskSlot = new ItemSlot();

		// Token: 0x040009C8 RID: 2504
		[DataField("alertTime", false, 1, false, false, null)]
		public float AlertSoundTime = 10f;

		// Token: 0x040009C9 RID: 2505
		[DataField("disarmDoafterLength", false, 1, false, false, null)]
		public float DisarmDoafterLength = 30f;

		// Token: 0x040009CA RID: 2506
		[DataField("alertLevelOnActivate", false, 1, false, false, null)]
		public string AlertLevelOnActivate;

		// Token: 0x040009CB RID: 2507
		[DataField("alertLevelOnDeactivate", false, 1, false, false, null)]
		public string AlertLevelOnDeactivate;

		// Token: 0x040009CC RID: 2508
		public int LastPlayedKeypadSemitones;

		// Token: 0x040009CD RID: 2509
		[DataField("keypadPressSound", false, 1, false, false, null)]
		public SoundSpecifier KeypadPressSound = new SoundPathSpecifier("/Audio/Machines/Nuke/general_beep.ogg", null);

		// Token: 0x040009CE RID: 2510
		[DataField("accessGrantedSound", false, 1, false, false, null)]
		public SoundSpecifier AccessGrantedSound = new SoundPathSpecifier("/Audio/Machines/Nuke/confirm_beep.ogg", null);

		// Token: 0x040009CF RID: 2511
		[DataField("accessDeniedSound", false, 1, false, false, null)]
		public SoundSpecifier AccessDeniedSound = new SoundPathSpecifier("/Audio/Machines/Nuke/angry_beep.ogg", null);

		// Token: 0x040009D0 RID: 2512
		[DataField("alertSound", false, 1, false, false, null)]
		public SoundSpecifier AlertSound = new SoundPathSpecifier("/Audio/Machines/Nuke/nuke_alarm.ogg", null);

		// Token: 0x040009D1 RID: 2513
		[DataField("armSound", false, 1, false, false, null)]
		public SoundSpecifier ArmSound = new SoundPathSpecifier("/Audio/Misc/notice1.ogg", null);

		// Token: 0x040009D2 RID: 2514
		[DataField("disarmSound", false, 1, false, false, null)]
		public SoundSpecifier DisarmSound = new SoundPathSpecifier("/Audio/Misc/notice2.ogg", null);

		// Token: 0x040009D3 RID: 2515
		[DataField("armMusic", false, 1, false, false, null)]
		public SoundSpecifier ArmMusic = new SoundPathSpecifier("/Audio/StationEvents/countdown.ogg", null);

		// Token: 0x040009D4 RID: 2516
		[ViewVariables]
		[DataField("explosionType", false, 1, true, false, typeof(PrototypeIdSerializer<ExplosionPrototype>))]
		public string ExplosionType;

		// Token: 0x040009D5 RID: 2517
		[ViewVariables]
		[DataField("maxIntensity", false, 1, false, false, null)]
		public float MaxIntensity = 100f;

		// Token: 0x040009D6 RID: 2518
		[ViewVariables]
		[DataField("intensitySlope", false, 1, false, false, null)]
		public float IntensitySlope = 5f;

		// Token: 0x040009D7 RID: 2519
		[ViewVariables]
		[DataField("totalIntensity", false, 1, false, false, null)]
		public float TotalIntensity = 100000f;

		// Token: 0x040009D8 RID: 2520
		public bool Exploded;

		// Token: 0x040009D9 RID: 2521
		public EntityUid? OriginStation;

		// Token: 0x040009DA RID: 2522
		[Nullable(0)]
		public ValueTuple<MapId, EntityUid?>? OriginMapGrid;

		// Token: 0x040009DB RID: 2523
		[DataField("codeLength", false, 1, false, false, null)]
		public int CodeLength = 6;

		// Token: 0x040009DC RID: 2524
		[ViewVariables]
		public string Code = string.Empty;

		// Token: 0x040009DD RID: 2525
		[ViewVariables]
		public float RemainingTime;

		// Token: 0x040009DE RID: 2526
		[ViewVariables]
		public float CooldownTime;

		// Token: 0x040009DF RID: 2527
		[ViewVariables]
		public string EnteredCode = "";

		// Token: 0x040009E0 RID: 2528
		[ViewVariables]
		public NukeStatus Status;

		// Token: 0x040009E1 RID: 2529
		public bool PlayedNukeSong;

		// Token: 0x040009E2 RID: 2530
		public bool PlayedAlertSound;

		// Token: 0x040009E3 RID: 2531
		[Nullable(2)]
		public IPlayingAudioStream AlertAudioStream;
	}
}
