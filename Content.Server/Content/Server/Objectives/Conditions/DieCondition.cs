using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Objectives.Interfaces;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Conditions
{
	// Token: 0x020002FC RID: 764
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class DieCondition : IObjectiveCondition, IEquatable<IObjectiveCondition>
	{
		// Token: 0x06000FA3 RID: 4003 RVA: 0x00050277 File Offset: 0x0004E477
		public IObjectiveCondition GetAssigned(Mind mind)
		{
			return new DieCondition
			{
				_mind = mind
			};
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x00050285 File Offset: 0x0004E485
		public string Title
		{
			get
			{
				return Loc.GetString("objective-condition-die-title");
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x00050291 File Offset: 0x0004E491
		public string Description
		{
			get
			{
				return Loc.GetString("objective-condition-die-description");
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x0005029D File Offset: 0x0004E49D
		public SpriteSpecifier Icon
		{
			get
			{
				return new SpriteSpecifier.Rsi(new ResourcePath("Mobs/Ghosts/ghost_human.rsi", "/"), "icon");
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x000502B8 File Offset: 0x0004E4B8
		public float Progress
		{
			get
			{
				Mind mind = this._mind;
				if (mind != null && !mind.CharacterDeadIC)
				{
					return 0f;
				}
				return 1f;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x000502D9 File Offset: 0x0004E4D9
		public float Difficulty
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x000502E0 File Offset: 0x0004E4E0
		[NullableContext(2)]
		public bool Equals(IObjectiveCondition other)
		{
			DieCondition condition = other as DieCondition;
			return condition != null && object.Equals(this._mind, condition._mind);
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0005030A File Offset: 0x0004E50A
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((DieCondition)obj)));
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00050338 File Offset: 0x0004E538
		public override int GetHashCode()
		{
			if (this._mind == null)
			{
				return 0;
			}
			return this._mind.GetHashCode();
		}

		// Token: 0x04000926 RID: 2342
		[Nullable(2)]
		private Mind _mind;
	}
}
