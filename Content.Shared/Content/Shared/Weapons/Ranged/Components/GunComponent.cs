using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000063 RID: 99
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Virtual]
	public class GunComponent : Component
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00006EB1 File Offset: 0x000050B1
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00006EB9 File Offset: 0x000050B9
		[ViewVariables]
		[DataField("soundGunshot", false, 1, false, false, null)]
		public SoundSpecifier SoundGunshot { get; set; } = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/smg.ogg", null);

		// Token: 0x0400012A RID: 298
		[ViewVariables]
		[DataField("soundEmpty", false, 1, false, false, null)]
		public SoundSpecifier SoundEmpty = new SoundPathSpecifier("/Audio/Weapons/Guns/Empty/empty.ogg", null);

		// Token: 0x0400012B RID: 299
		[ViewVariables]
		[DataField("soundMode", false, 1, false, false, null)]
		public SoundSpecifier SoundModeToggle = new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/selector.ogg", null);

		// Token: 0x0400012C RID: 300
		[DataField("lastFire", false, 1, false, false, null)]
		public TimeSpan LastFire = TimeSpan.Zero;

		// Token: 0x0400012D RID: 301
		[DataField("currentAngle", false, 1, false, false, null)]
		public Angle CurrentAngle;

		// Token: 0x0400012E RID: 302
		[ViewVariables]
		[DataField("angleIncrease", false, 1, false, false, null)]
		public Angle AngleIncrease = Angle.FromDegrees(0.5);

		// Token: 0x0400012F RID: 303
		[DataField("angleDecay", false, 1, false, false, null)]
		public Angle AngleDecay = Angle.FromDegrees(4.0);

		// Token: 0x04000130 RID: 304
		[ViewVariables]
		[DataField("maxAngle", false, 1, false, false, null)]
		public Angle MaxAngle = Angle.FromDegrees(2.0);

		// Token: 0x04000131 RID: 305
		[ViewVariables]
		[DataField("minAngle", false, 1, false, false, null)]
		public Angle MinAngle = Angle.FromDegrees(1.0);

		// Token: 0x04000132 RID: 306
		[ViewVariables]
		public EntityCoordinates? ShootCoordinates;

		// Token: 0x04000133 RID: 307
		[ViewVariables]
		public int ShotCounter;

		// Token: 0x04000134 RID: 308
		[ViewVariables]
		[DataField("fireRate", false, 1, false, false, null)]
		public float FireRate = 8f;

		// Token: 0x04000135 RID: 309
		[ViewVariables]
		[DataField("projectileSpeed", false, 1, false, false, null)]
		public float ProjectileSpeed = 25f;

		// Token: 0x04000136 RID: 310
		[DataField("nextFire", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextFire = TimeSpan.Zero;

		// Token: 0x04000137 RID: 311
		[ViewVariables]
		[DataField("availableModes", false, 1, false, false, null)]
		public SelectiveFire AvailableModes = SelectiveFire.SemiAuto;

		// Token: 0x04000138 RID: 312
		[ViewVariables]
		[DataField("selectedMode", false, 1, false, false, null)]
		public SelectiveFire SelectedMode = SelectiveFire.SemiAuto;

		// Token: 0x04000139 RID: 313
		[DataField("selectModeAction", false, 1, false, false, null)]
		public InstantAction SelectModeAction;

		// Token: 0x0400013A RID: 314
		[DataField("showExamineText", false, 1, false, false, null)]
		public bool ShowExamineText = true;
	}
}
