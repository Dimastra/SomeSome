using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Objectives
{
	// Token: 0x020002F2 RID: 754
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("objective", 1)]
	public sealed class ObjectivePrototype : IPrototype
	{
		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000F85 RID: 3973 RVA: 0x0004FE6D File Offset: 0x0004E06D
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0004FE75 File Offset: 0x0004E075
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0004FE7D File Offset: 0x0004E07D
		[DataField("issuer", false, 1, false, false, null)]
		public string Issuer { get; private set; } = "Unknown";

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x0004FE88 File Offset: 0x0004E088
		[ViewVariables]
		public float Difficulty
		{
			get
			{
				float? difficultyOverride = this._difficultyOverride;
				if (difficultyOverride == null)
				{
					return this._conditions.Sum((IObjectiveCondition c) => c.Difficulty);
				}
				return difficultyOverride.GetValueOrDefault();
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0004FED7 File Offset: 0x0004E0D7
		[ViewVariables]
		public IReadOnlyList<IObjectiveCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x0004FEDF File Offset: 0x0004E0DF
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0004FEE7 File Offset: 0x0004E0E7
		[DataField("canBeDuplicate", false, 1, false, false, null)]
		public bool CanBeDuplicateAssignment { get; private set; }

		// Token: 0x06000F8C RID: 3980 RVA: 0x0004FEF0 File Offset: 0x0004E0F0
		public bool CanBeAssigned(Mind mind)
		{
			using (List<IObjectiveRequirement>.Enumerator enumerator = this._requirements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.CanBeAssigned(mind))
					{
						return false;
					}
				}
			}
			if (!this.CanBeDuplicateAssignment)
			{
				using (IEnumerator<Objective> enumerator2 = mind.AllObjectives.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Prototype.ID == this.ID)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0004FFA4 File Offset: 0x0004E1A4
		public Objective GetObjective(Mind mind)
		{
			return new Objective(this, mind);
		}

		// Token: 0x0400091C RID: 2332
		[DataField("conditions", false, 1, false, false, null)]
		private List<IObjectiveCondition> _conditions = new List<IObjectiveCondition>();

		// Token: 0x0400091D RID: 2333
		[DataField("requirements", false, 1, false, false, null)]
		private List<IObjectiveRequirement> _requirements = new List<IObjectiveRequirement>();

		// Token: 0x0400091F RID: 2335
		[ViewVariables]
		[DataField("difficultyOverride", false, 1, false, false, null)]
		private float? _difficultyOverride;
	}
}
