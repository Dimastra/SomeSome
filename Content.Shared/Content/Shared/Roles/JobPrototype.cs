using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Access;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles
{
	// Token: 0x020001E4 RID: 484
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("job", 1)]
	public sealed class JobPrototype : IPrototype
	{
		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x00013CE2 File Offset: 0x00011EE2
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x00013CEA File Offset: 0x00011EEA
		[DataField("playTimeTracker", false, 1, true, false, typeof(PrototypeIdSerializer<PlayTimeTrackerPrototype>))]
		public string PlayTimeTracker { get; } = string.Empty;

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00013CF2 File Offset: 0x00011EF2
		[DataField("supervisors", false, 1, false, false, null)]
		public string Supervisors { get; } = "nobody";

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x00013CFA File Offset: 0x00011EFA
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00013D02 File Offset: 0x00011F02
		[ViewVariables]
		public string LocalizedName
		{
			get
			{
				return Loc.GetString(this.Name);
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x00013D0F File Offset: 0x00011F0F
		[Nullable(2)]
		[DataField("description", false, 1, false, false, null)]
		public string Description { [NullableContext(2)] get; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x00013D17 File Offset: 0x00011F17
		[Nullable(2)]
		[ViewVariables]
		public string LocalizedDescription
		{
			[NullableContext(2)]
			get
			{
				if (this.Description != null)
				{
					return Loc.GetString(this.Description);
				}
				return null;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x00013D2E File Offset: 0x00011F2E
		[DataField("joinNotifyCrew", false, 1, false, false, null)]
		public bool JoinNotifyCrew { get; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00013D36 File Offset: 0x00011F36
		[DataField("requireAdminNotify", false, 1, false, false, null)]
		public bool RequireAdminNotify { get; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x00013D3E File Offset: 0x00011F3E
		[DataField("setPreference", false, 1, false, false, null)]
		public bool SetPreference { get; } = 1;

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00013D46 File Offset: 0x00011F46
		[DataField("canBeAntag", false, 1, false, false, null)]
		public bool CanBeAntag { get; } = 1;

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x00013D4E File Offset: 0x00011F4E
		// (set) Token: 0x06000571 RID: 1393 RVA: 0x00013D56 File Offset: 0x00011F56
		[DataField("weight", false, 1, false, false, null)]
		public int Weight { get; private set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x00013D5F File Offset: 0x00011F5F
		// (set) Token: 0x06000573 RID: 1395 RVA: 0x00013D67 File Offset: 0x00011F67
		[Nullable(2)]
		[DataField("startingGear", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string StartingGear { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00013D70 File Offset: 0x00011F70
		[DataField("icon", false, 1, false, false, null)]
		public string Icon { get; } = string.Empty;

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x00013D78 File Offset: 0x00011F78
		// (set) Token: 0x06000576 RID: 1398 RVA: 0x00013D80 File Offset: 0x00011F80
		[DataField("special", false, 1, false, true, null)]
		public JobSpecial[] Special { get; private set; } = Array.Empty<JobSpecial>();

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x00013D89 File Offset: 0x00011F89
		[DataField("access", false, 1, false, false, typeof(PrototypeIdListSerializer<AccessLevelPrototype>))]
		public IReadOnlyCollection<string> Access { get; } = Array.Empty<string>();

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00013D91 File Offset: 0x00011F91
		[DataField("accessGroups", false, 1, false, false, typeof(PrototypeIdListSerializer<AccessGroupPrototype>))]
		public IReadOnlyCollection<string> AccessGroups { get; } = Array.Empty<string>();

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x00013D99 File Offset: 0x00011F99
		[DataField("extendedAccess", false, 1, false, false, typeof(PrototypeIdListSerializer<AccessLevelPrototype>))]
		public IReadOnlyCollection<string> ExtendedAccess { get; } = Array.Empty<string>();

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00013DA1 File Offset: 0x00011FA1
		[DataField("extendedAccessGroups", false, 1, false, false, typeof(PrototypeIdListSerializer<AccessGroupPrototype>))]
		public IReadOnlyCollection<string> ExtendedAccessGroups { get; } = Array.Empty<string>();

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x00013DA9 File Offset: 0x00011FA9
		[DataField("whitelistedSpecies", false, 1, false, false, typeof(PrototypeIdListSerializer<SpeciesPrototype>))]
		public IReadOnlyCollection<string> WhitelistedSpecies { get; } = Array.Empty<string>();

		// Token: 0x04000579 RID: 1401
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("requirements", false, 1, false, false, null)]
		public HashSet<JobRequirement> Requirements;

		// Token: 0x0400057F RID: 1407
		[DataField("antagAdvantage", false, 1, false, false, null)]
		public int AntagAdvantage;

		// Token: 0x04000581 RID: 1409
		[Nullable(2)]
		[DataField("jobEntity", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string JobEntity;
	}
}
