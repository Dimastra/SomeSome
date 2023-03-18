using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Objectives.Requirements
{
	// Token: 0x020002F4 RID: 756
	[DataDefinition]
	public sealed class IncompatibleConditionsRequirement : IObjectiveRequirement
	{
		// Token: 0x06000F91 RID: 3985 RVA: 0x00050074 File Offset: 0x0004E274
		[NullableContext(1)]
		public bool CanBeAssigned(Mind mind)
		{
			foreach (Objective objective in mind.AllObjectives)
			{
				foreach (IObjectiveCondition condition in objective.Conditions)
				{
					using (List<string>.Enumerator enumerator3 = this._incompatibleConditions.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current == condition.GetType().Name)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		// Token: 0x04000922 RID: 2338
		[Nullable(1)]
		[DataField("conditions", false, 1, false, false, null)]
		private readonly List<string> _incompatibleConditions = new List<string>();
	}
}
