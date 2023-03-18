using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Content.Server.Roles;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x020002FE RID: 766
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class KillPersonCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>
	{
		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x00050563 File Offset: 0x0004E763
		protected IEntityManager EntityManager
		{
			get
			{
				return IoCManager.Resolve<IEntityManager>();
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x0005056A File Offset: 0x0004E76A
		protected MobStateSystem MobStateSystem
		{
			get
			{
				return this.EntityManager.EntitySysManager.GetEntitySystem<MobStateSystem>();
			}
		}

		// Token: 0x06000FBA RID: 4026
		public abstract IObjectiveCondition GetAssigned(Mind mind);

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000FBB RID: 4027 RVA: 0x0005057C File Offset: 0x0004E77C
		public string Title
		{
			get
			{
				string targetName = string.Empty;
				Mind target = this.Target;
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
				if (this.Target == null)
				{
					return Loc.GetString("objective-condition-kill-person-title", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("targetName", targetName),
						new ValueTuple<string, object>("job", jobName)
					});
				}
				EntityUid? ownedEntity = this.Target.OwnedEntity;
				if (ownedEntity != null)
				{
					EntityUid owned = ownedEntity.GetValueOrDefault();
					if (owned.Valid)
					{
						targetName = this.EntityManager.GetComponent<MetaDataComponent>(owned).EntityName;
					}
				}
				return Loc.GetString("objective-condition-kill-person-title", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", targetName),
					new ValueTuple<string, object>("job", jobName)
				});
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000FBC RID: 4028 RVA: 0x00050660 File Offset: 0x0004E860
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-kill-person-description");
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000FBD RID: 4029 RVA: 0x0005066C File Offset: 0x0004E86C
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.Rsi(new ResourcePath("Objects/Weapons/Guns/Pistols/viper.rsi", "/"), "icon");
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x00050687 File Offset: 0x0004E887
		public float Progress
		{
			get
			{
				Mind target = this.Target;
				if (target != null && !target.CharacterDeadIC)
				{
					return 0f;
				}
				return 1f;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000FBF RID: 4031 RVA: 0x000506A8 File Offset: 0x0004E8A8
		public float Difficulty
		{
			get
			{
				return 2f;
			}
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x000506B0 File Offset: 0x0004E8B0
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			KillPersonCondition kpc = other as KillPersonCondition;
			return kpc != null && object.Equals(this.Target, kpc.Target);
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x000506DA File Offset: 0x0004E8DA
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((KillPersonCondition)obj)));
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00050708 File Offset: 0x0004E908
		public override int GetHashCode()
		{
			Mind target = this.Target;
			if (target == null)
			{
				return 0;
			}
			return target.GetHashCode();
		}

		// Token: 0x04000928 RID: 2344
		[Nullable(2)]
		protected Mind Target;
	}
}
