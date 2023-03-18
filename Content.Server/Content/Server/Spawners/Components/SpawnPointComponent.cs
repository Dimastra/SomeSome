using System;
using System.Runtime.CompilerServices;
using Content.Shared.Markers;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Spawners.Components
{
	// Token: 0x020001D7 RID: 471
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedSpawnPointComponent))]
	public sealed class SpawnPointComponent : SharedSpawnPointComponent
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0002D83C File Offset: 0x0002BA3C
		[ViewVariables]
		[DataField("spawn_type", false, 1, false, false, null)]
		public SpawnPointType SpawnType { get; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0002D844 File Offset: 0x0002BA44
		public JobPrototype Job
		{
			get
			{
				if (!string.IsNullOrEmpty(this._jobId))
				{
					return this._prototypeManager.Index<JobPrototype>(this._jobId);
				}
				return null;
			}
		}

		// Token: 0x0400056B RID: 1387
		[Nullable(1)]
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400056C RID: 1388
		[ViewVariables]
		[DataField("job_id", false, 1, false, false, null)]
		private string _jobId;
	}
}
