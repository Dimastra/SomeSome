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
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x02000300 RID: 768
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class RandomTraitorAliveCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>
	{
		// Token: 0x06000FC6 RID: 4038 RVA: 0x000507B8 File Offset: 0x0004E9B8
		public IObjectiveCondition GetAssigned(Mind mind)
		{
			List<TraitorRole> traitors = IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<TraitorRuleSystem>().GetOtherTraitorsAliveAndConnected(mind).ToList<TraitorRole>();
			if (traitors.Count == 0)
			{
				return new EscapeShuttleCondition();
			}
			return new RandomTraitorAliveCondition
			{
				_target = RandomExtensions.Pick<TraitorRole>(IoCManager.Resolve<IRobustRandom>(), traitors).Mind
			};
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x0005080C File Offset: 0x0004EA0C
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
					return Loc.GetString("objective-condition-other-traitor-alive-title", new ValueTuple<string, object>[]
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
				return Loc.GetString("objective-condition-other-traitor-alive-title", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", targetName),
					new ValueTuple<string, object>("job", jobName)
				});
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x000508EF File Offset: 0x0004EAEF
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-other-traitor-alive-description");
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x000508FB File Offset: 0x0004EAFB
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.Rsi(new ResourcePath("Objects/Misc/bureaucracy.rsi", "/"), "folder-white");
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000FCA RID: 4042 RVA: 0x00050916 File Offset: 0x0004EB16
		public float Progress
		{
			get
			{
				Mind target = this._target;
				if (target != null && target.CharacterDeadIC)
				{
					return 0f;
				}
				return 1f;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0005093A File Offset: 0x0004EB3A
		public float Difficulty
		{
			get
			{
				return 1.75f;
			}
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x00050944 File Offset: 0x0004EB44
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			RandomTraitorAliveCondition kpc = other as RandomTraitorAliveCondition;
			return kpc != null && object.Equals(this._target, kpc._target);
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x00050970 File Offset: 0x0004EB70
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
			RandomTraitorAliveCondition alive = obj as RandomTraitorAliveCondition;
			return alive != null && alive.Equals(this);
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0005099B File Offset: 0x0004EB9B
		public override int GetHashCode()
		{
			Mind target = this._target;
			if (target == null)
			{
				return 0;
			}
			return target.GetHashCode();
		}

		// Token: 0x04000929 RID: 2345
		[Nullable(2)]
		private Mind _target;
	}
}
