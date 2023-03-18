using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles
{
	// Token: 0x020001E9 RID: 489
	[NullableContext(1)]
	[Nullable(0)]
	public static class JobRequirements
	{
		// Token: 0x06000581 RID: 1409 RVA: 0x00013E64 File Offset: 0x00012064
		public static bool TryRequirementsMet(JobPrototype job, Dictionary<string, TimeSpan> playTimes, [Nullable(2)] [NotNullWhen(false)] out string reason, IPrototypeManager prototypes)
		{
			reason = null;
			if (job.Requirements == null)
			{
				return true;
			}
			using (HashSet<JobRequirement>.Enumerator enumerator = job.Requirements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!JobRequirements.TryRequirementMet(enumerator.Current, playTimes, out reason, prototypes))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x00013ED0 File Offset: 0x000120D0
		public static bool TryRequirementMet(JobRequirement requirement, Dictionary<string, TimeSpan> playTimes, [Nullable(2)] [NotNullWhen(false)] out string reason, IPrototypeManager prototypes)
		{
			reason = null;
			DepartmentTimeRequirement deptRequirement = requirement as DepartmentTimeRequirement;
			if (deptRequirement == null)
			{
				OverallPlaytimeRequirement overallRequirement = requirement as OverallPlaytimeRequirement;
				if (overallRequirement == null)
				{
					RoleTimeRequirement roleRequirement = requirement as RoleTimeRequirement;
					if (roleRequirement == null)
					{
						throw new NotImplementedException();
					}
					string proto = roleRequirement.Role;
					TimeSpan roleTime;
					playTimes.TryGetValue(proto, out roleTime);
					double roleDiff = roleRequirement.Time.TotalMinutes - roleTime.TotalMinutes;
					if (!roleRequirement.Inverted)
					{
						if (roleDiff <= 0.0)
						{
							return true;
						}
						reason = Loc.GetString("role-timer-role-insufficient", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("time", roleDiff),
							new ValueTuple<string, object>("job", Loc.GetString(proto))
						});
						return false;
					}
					else
					{
						if (roleDiff <= 0.0)
						{
							reason = Loc.GetString("role-timer-role-too-high", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("time", -roleDiff),
								new ValueTuple<string, object>("job", Loc.GetString(proto))
							});
							return false;
						}
						return true;
					}
				}
				else
				{
					TimeSpan overallTime = playTimes.GetValueOrDefault("Overall");
					double overallDiff = overallRequirement.Time.TotalMinutes - overallTime.TotalMinutes;
					if (!overallRequirement.Inverted)
					{
						if (overallDiff <= 0.0 || overallTime >= overallRequirement.Time)
						{
							return true;
						}
						reason = Loc.GetString("role-timer-overall-insufficient", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("time", overallDiff)
						});
						return false;
					}
					else
					{
						if (overallDiff <= 0.0 || overallTime >= overallRequirement.Time)
						{
							reason = Loc.GetString("role-timer-overall-too-high", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("time", -overallDiff)
							});
							return false;
						}
						return true;
					}
				}
			}
			else
			{
				TimeSpan playtime = TimeSpan.Zero;
				foreach (string other in prototypes.Index<DepartmentPrototype>(deptRequirement.Department).Roles)
				{
					string proto = prototypes.Index<JobPrototype>(other).PlayTimeTracker;
					TimeSpan otherTime;
					playTimes.TryGetValue(proto, out otherTime);
					playtime += otherTime;
				}
				double deptDiff = deptRequirement.Time.TotalMinutes - playtime.TotalMinutes;
				if (!deptRequirement.Inverted)
				{
					if (deptDiff <= 0.0)
					{
						return true;
					}
					reason = Loc.GetString("role-timer-department-insufficient", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("time", deptDiff),
						new ValueTuple<string, object>("department", Loc.GetString(deptRequirement.Department))
					});
					return false;
				}
				else
				{
					if (deptDiff <= 0.0)
					{
						reason = Loc.GetString("role-timer-department-too-high", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("time", -deptDiff),
							new ValueTuple<string, object>("department", Loc.GetString(deptRequirement.Department))
						});
						return false;
					}
					return true;
				}
			}
		}
	}
}
