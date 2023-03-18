using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Buckle.Components
{
	// Token: 0x02000646 RID: 1606
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StrapComponent : Component
	{
		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06001367 RID: 4967 RVA: 0x000408A6 File Offset: 0x0003EAA6
		// (set) Token: 0x06001368 RID: 4968 RVA: 0x000408AE File Offset: 0x0003EAAE
		[DataField("position", false, 1, false, false, null)]
		public StrapPosition Position { get; set; }

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06001369 RID: 4969 RVA: 0x000408B7 File Offset: 0x0003EAB7
		public Vector2 BuckleOffset
		{
			get
			{
				return Vector2.Clamp(this.BuckleOffsetUnclamped, Vector2.One * -this.MaxBuckleDistance, Vector2.One * this.MaxBuckleDistance);
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x0600136A RID: 4970 RVA: 0x000408E5 File Offset: 0x0003EAE5
		// (set) Token: 0x0600136B RID: 4971 RVA: 0x000408ED File Offset: 0x0003EAED
		[DataField("rotation", false, 1, false, false, null)]
		public int Rotation { get; set; }

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x000408F6 File Offset: 0x0003EAF6
		// (set) Token: 0x0600136D RID: 4973 RVA: 0x000408FE File Offset: 0x0003EAFE
		[DataField("size", false, 1, false, false, null)]
		public int Size { get; set; } = 100;

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x00040907 File Offset: 0x0003EB07
		// (set) Token: 0x0600136F RID: 4975 RVA: 0x0004090F File Offset: 0x0003EB0F
		public bool Enabled { get; set; } = true;

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x00040918 File Offset: 0x0003EB18
		[DataField("buckleSound", false, 1, false, false, null)]
		public SoundSpecifier BuckleSound { get; } = new SoundPathSpecifier("/Audio/Effects/buckle.ogg", null);

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x00040920 File Offset: 0x0003EB20
		[DataField("unbuckleSound", false, 1, false, false, null)]
		public SoundSpecifier UnbuckleSound { get; } = new SoundPathSpecifier("/Audio/Effects/unbuckle.ogg", null);

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06001372 RID: 4978 RVA: 0x00040928 File Offset: 0x0003EB28
		[DataField("buckledAlertType", false, 1, false, false, null)]
		public AlertType BuckledAlertType { get; } = 11;

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x00040930 File Offset: 0x0003EB30
		// (set) Token: 0x06001374 RID: 4980 RVA: 0x00040938 File Offset: 0x0003EB38
		public int OccupiedSize { get; set; }

		// Token: 0x0400135B RID: 4955
		public readonly HashSet<EntityUid> BuckledEntities = new HashSet<EntityUid>();

		// Token: 0x0400135C RID: 4956
		[DataField("maxBuckleDistance", false, 1, false, false, null)]
		public float MaxBuckleDistance = 0.1f;

		// Token: 0x0400135D RID: 4957
		[DataField("buckleOffset", false, 1, false, false, null)]
		[Access]
		public Vector2 BuckleOffsetUnclamped = Vector2.Zero;

		// Token: 0x04001361 RID: 4961
		[DataField("unbuckleOffset", false, 1, false, false, null)]
		public Vector2 UnbuckleOffset = Vector2.Zero;
	}
}
