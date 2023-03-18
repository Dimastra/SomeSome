using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Objectives.Requirements
{
	// Token: 0x020002F5 RID: 757
	[DataDefinition]
	public sealed class IncompatibleObjectivesRequirement : IObjectiveRequirement
	{
		// Token: 0x06000F93 RID: 3987 RVA: 0x00050158 File Offset: 0x0004E358
		[NullableContext(1)]
		public bool CanBeAssigned(Mind mind)
		{
			foreach (Objective objective in mind.AllObjectives)
			{
				using (List<string>.Enumerator enumerator2 = this._incompatibleObjectives.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == objective.Prototype.ID)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x04000923 RID: 2339
		[Nullable(1)]
		[DataField("objectives", false, 1, false, false, null)]
		private readonly List<string> _incompatibleObjectives = new List<string>();
	}
}
