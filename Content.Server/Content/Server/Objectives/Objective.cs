using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.ViewVariables;

namespace Content.Server.Objectives
{
	// Token: 0x020002F1 RID: 753
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class Objective : IEquatable<Objective>
	{
		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000F80 RID: 3968 RVA: 0x0004FD13 File Offset: 0x0004DF13
		[ViewVariables]
		public IReadOnlyList<IObjectiveCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0004FD1C File Offset: 0x0004DF1C
		public Objective(ObjectivePrototype prototype, Mind mind)
		{
			this.Prototype = prototype;
			this.Mind = mind;
			foreach (IObjectiveCondition condition in prototype.Conditions)
			{
				this._conditions.Add(condition.GetAssigned(mind));
			}
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0004FD94 File Offset: 0x0004DF94
		[NullableContext(2)]
		public bool Equals(Objective other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (!object.Equals(this.Mind, other.Mind) || !object.Equals(this.Prototype, other.Prototype))
			{
				return false;
			}
			if (this._conditions.Count != other._conditions.Count)
			{
				return false;
			}
			for (int i = 0; i < this._conditions.Count; i++)
			{
				if (!this._conditions[i].Equals(other._conditions[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0004FE26 File Offset: 0x0004E026
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((Objective)obj)));
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0004FE54 File Offset: 0x0004E054
		public override int GetHashCode()
		{
			return HashCode.Combine<Mind, ObjectivePrototype, List<IObjectiveCondition>>(this.Mind, this.Prototype, this._conditions);
		}

		// Token: 0x04000917 RID: 2327
		[ViewVariables]
		public readonly Mind Mind;

		// Token: 0x04000918 RID: 2328
		[ViewVariables]
		public readonly ObjectivePrototype Prototype;

		// Token: 0x04000919 RID: 2329
		private readonly List<IObjectiveCondition> _conditions = new List<IObjectiveCondition>();
	}
}
