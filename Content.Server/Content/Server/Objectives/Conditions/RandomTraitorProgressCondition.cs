using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Server.Roles;
using Content.Server.Traitor;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x02000301 RID: 769
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class RandomTraitorProgressCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>
	{
		// Token: 0x06000FD0 RID: 4048 RVA: 0x000509B8 File Offset: 0x0004EBB8
		public IObjectiveCondition GetAssigned(Mind mind)
		{
			List<TraitorRole> traitors = IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<TraitorRuleSystem>().GetOtherTraitorsAliveAndConnected(mind).ToList<TraitorRole>();
			List<TraitorRole> removeList = new List<TraitorRole>();
			foreach (TraitorRole traitor in traitors)
			{
				foreach (Objective objective in traitor.Mind.AllObjectives)
				{
					using (IEnumerator<IObjectiveCondition> enumerator3 = objective.Conditions.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.GetType() == typeof(RandomTraitorProgressCondition))
							{
								removeList.Add(traitor);
							}
						}
					}
				}
			}
			foreach (TraitorRole traitor2 in removeList)
			{
				traitors.Remove(traitor2);
			}
			if (traitors.Count == 0)
			{
				return new EscapeShuttleCondition();
			}
			return new RandomTraitorProgressCondition
			{
				_target = RandomExtensions.Pick<TraitorRole>(IoCManager.Resolve<IRobustRandom>(), traitors).Mind
			};
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x00050B24 File Offset: 0x0004ED24
		public string Title
		{
			get
			{
				string targetName = string.Empty;
				Mind target = this._target;
				string text;
				if (target == null)
				{
					text = null;
				}
				else
				{
					Job currentJob = target.CurrentJob;
					text = ((currentJob != null) ? currentJob.Name : null);
				}
				string jobName = text ?? "Unknown";
				if (this._target == null)
				{
					return Loc.GetString("objective-condition-other-traitor-progress-title", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("targetName", targetName),
						new ValueTuple<string, object>("job", jobName)
					});
				}
				EntityUid? ownedEntity = this._target.OwnedEntity;
				if (ownedEntity != null)
				{
					EntityUid owned = ownedEntity.GetValueOrDefault();
					if (owned.Valid)
					{
						targetName = IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(owned).EntityName;
					}
				}
				return Loc.GetString("objective-condition-other-traitor-progress-title", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", targetName),
					new ValueTuple<string, object>("job", jobName)
				});
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x00050C07 File Offset: 0x0004EE07
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-other-traitor-progress-description");
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x00050C13 File Offset: 0x0004EE13
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.Rsi(new ResourcePath("Objects/Misc/bureaucracy.rsi", "/"), "folder-white");
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x00050C30 File Offset: 0x0004EE30
		public float Progress
		{
			get
			{
				IoCManager.Resolve<IEntityManager>();
				float total = 0f;
				float max = 0f;
				if (this._target == null)
				{
					Logger.Error("Null target on RandomTraitorProgressCondition.");
					return 1f;
				}
				foreach (Objective objective in this._target.AllObjectives)
				{
					foreach (IObjectiveCondition condition in objective.Conditions)
					{
						max += 1f;
						total += condition.Progress;
					}
				}
				if (max == 0f)
				{
					Logger.Error("RandomTraitorProgressCondition assigned someone with no objectives to be helped.");
					return 1f;
				}
				float completion = total / max;
				if (completion >= 0.5f)
				{
					return 1f;
				}
				return completion / 0.5f;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00050D24 File Offset: 0x0004EF24
		public float Difficulty
		{
			get
			{
				return 2.5f;
			}
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x00050D2C File Offset: 0x0004EF2C
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			RandomTraitorProgressCondition kpc = other as RandomTraitorProgressCondition;
			return kpc != null && object.Equals(this._target, kpc._target);
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x00050D58 File Offset: 0x0004EF58
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			RandomTraitorProgressCondition alive = obj as RandomTraitorProgressCondition;
			return alive != null && alive.Equals(this);
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00050D83 File Offset: 0x0004EF83
		public override int GetHashCode()
		{
			Mind target = this._target;
			if (target == null)
			{
				return 0;
			}
			return target.GetHashCode();
		}

		// Token: 0x0400092A RID: 2346
		[Nullable(2)]
		private Mind _target;
	}
}
