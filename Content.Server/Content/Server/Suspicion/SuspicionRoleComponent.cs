using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Server.Suspicion.Roles;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Suspicion;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.Suspicion
{
	// Token: 0x02000136 RID: 310
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SuspicionRoleComponent : SharedSuspicionRoleComponent
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001BD66 File Offset: 0x00019F66
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x0001BD70 File Offset: 0x00019F70
		[Nullable(2)]
		[ViewVariables]
		public Role Role
		{
			[NullableContext(2)]
			get
			{
				return this._role;
			}
			[NullableContext(2)]
			set
			{
				if (this._role == value)
				{
					return;
				}
				this._role = value;
				base.Dirty(null);
				SuspicionRuleSystem sus = EntitySystem.Get<SuspicionRuleSystem>();
				if (value == null || !value.Antagonist)
				{
					this.ClearAllies();
					sus.RemoveTraitor(this);
					return;
				}
				if (value.Antagonist)
				{
					this.SetAllies(sus.Traitors);
					sus.AddTraitor(this);
				}
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0001BDCF File Offset: 0x00019FCF
		[ViewVariables]
		public bool KnowsAllies
		{
			get
			{
				return this.IsTraitor();
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001BDD8 File Offset: 0x00019FD8
		public bool IsDead()
		{
			MobStateComponent state;
			return this._entMan.TryGetComponent<MobStateComponent>(base.Owner, ref state) && this._entMan.EntitySysManager.GetEntitySystem<MobStateSystem>().IsDead(base.Owner, state);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0001BE18 File Offset: 0x0001A018
		public bool IsInnocent()
		{
			return !this.IsTraitor();
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0001BE23 File Offset: 0x0001A023
		public bool IsTraitor()
		{
			Role role = this.Role;
			return role != null && role.Antagonist;
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0001BE38 File Offset: 0x0001A038
		public void SyncRoles()
		{
			MindComponent mind;
			if (!this._entMan.TryGetComponent<MindComponent>(base.Owner, ref mind) || !mind.HasMind)
			{
				return;
			}
			this.Role = mind.Mind.AllRoles.First((Role role) => role is SuspicionRole);
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0001BE98 File Offset: 0x0001A098
		public void AddAlly(SuspicionRoleComponent ally)
		{
			if (ally == this)
			{
				return;
			}
			this._allies.Add(ally);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0001BEAC File Offset: 0x0001A0AC
		public bool RemoveAlly(SuspicionRoleComponent ally)
		{
			if (this._allies.Remove(ally))
			{
				base.Dirty(null);
				return true;
			}
			return false;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0001BEC6 File Offset: 0x0001A0C6
		public void SetAllies(IEnumerable<SuspicionRoleComponent> allies)
		{
			this._allies.Clear();
			this._allies.UnionWith(from a in allies
			where a != this
			select a);
			base.Dirty(null);
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0001BEF7 File Offset: 0x0001A0F7
		public void ClearAllies()
		{
			this._allies.Clear();
			base.Dirty(null);
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0001BF0C File Offset: 0x0001A10C
		public override ComponentState GetComponentState()
		{
			if (this.Role == null)
			{
				return new SuspicionRoleComponentState(null, null, Array.Empty<ValueTuple<string, EntityUid>>());
			}
			List<ValueTuple<string, EntityUid>> allies = new List<ValueTuple<string, EntityUid>>();
			foreach (SuspicionRoleComponent role in this._allies)
			{
				Role role2 = role.Role;
				if (((role2 != null) ? role2.Mind.CharacterName : null) != null)
				{
					allies.Add(new ValueTuple<string, EntityUid>(role.Role.Mind.CharacterName, role.Owner));
				}
			}
			Role role3 = this.Role;
			string role4 = (role3 != null) ? role3.Name : null;
			Role role5 = this.Role;
			return new SuspicionRoleComponentState(role4, (role5 != null) ? new bool?(role5.Antagonist) : null, allies.ToArray());
		}

		// Token: 0x04000361 RID: 865
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000362 RID: 866
		[Nullable(2)]
		private Role _role;

		// Token: 0x04000363 RID: 867
		[ViewVariables]
		private readonly HashSet<SuspicionRoleComponent> _allies = new HashSet<SuspicionRoleComponent>();
	}
}
