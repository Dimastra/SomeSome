using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Shared.Roles;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Objectives.Requirements
{
	// Token: 0x020002F7 RID: 759
	[DataDefinition]
	public sealed class NotRoleRequirement : IObjectiveRequirement
	{
		// Token: 0x06000F97 RID: 3991 RVA: 0x0005022D File Offset: 0x0004E42D
		[NullableContext(1)]
		public bool CanBeAssigned(Mind mind)
		{
			return mind.CurrentJob == null || mind.CurrentJob.Prototype.ID != this.roleId;
		}

		// Token: 0x04000925 RID: 2341
		[Nullable(1)]
		[DataField("roleId", false, 1, false, false, typeof(PrototypeIdSerializer<JobPrototype>))]
		private string roleId = "";
	}
}
