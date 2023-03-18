using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000776 RID: 1910
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAccessSystem : EntitySystem
	{
		// Token: 0x06001792 RID: 6034 RVA: 0x0004C928 File Offset: 0x0004AB28
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AccessComponent, MapInitEvent>(new ComponentEventHandler<AccessComponent, MapInitEvent>(this.OnAccessInit), null, null);
			base.SubscribeLocalEvent<AccessComponent, ComponentGetState>(new ComponentEventRefHandler<AccessComponent, ComponentGetState>(this.OnAccessGetState), null, null);
			base.SubscribeLocalEvent<AccessComponent, ComponentHandleState>(new ComponentEventRefHandler<AccessComponent, ComponentHandleState>(this.OnAccessHandleState), null, null);
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0004C978 File Offset: 0x0004AB78
		private void OnAccessHandleState(EntityUid uid, AccessComponent component, ref ComponentHandleState args)
		{
			SharedAccessSystem.AccessComponentState state = args.Current as SharedAccessSystem.AccessComponentState;
			if (state == null)
			{
				return;
			}
			component.Tags.Clear();
			component.Groups.Clear();
			component.Tags.UnionWith(state.Tags);
			component.Groups.UnionWith(state.Groups);
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0004C9CD File Offset: 0x0004ABCD
		private void OnAccessGetState(EntityUid uid, AccessComponent component, ref ComponentGetState args)
		{
			args.State = new SharedAccessSystem.AccessComponentState
			{
				Tags = component.Tags,
				Groups = component.Groups
			};
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x0004C9F4 File Offset: 0x0004ABF4
		private void OnAccessInit(EntityUid uid, AccessComponent component, MapInitEvent args)
		{
			foreach (string group in component.Groups)
			{
				AccessGroupPrototype proto;
				if (this._prototypeManager.TryIndex<AccessGroupPrototype>(group, ref proto))
				{
					component.Tags.UnionWith(proto.Tags);
				}
			}
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0004CA64 File Offset: 0x0004AC64
		public bool TrySetTags(EntityUid uid, IEnumerable<string> newTags, [Nullable(2)] AccessComponent access = null)
		{
			if (!base.Resolve<AccessComponent>(uid, ref access, true))
			{
				return false;
			}
			access.Tags.Clear();
			access.Tags.UnionWith(newTags);
			base.Dirty(access, null);
			return true;
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x0004CA94 File Offset: 0x0004AC94
		[NullableContext(2)]
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public IEnumerable<string> TryGetTags(EntityUid uid, AccessComponent access = null)
		{
			if (base.Resolve<AccessComponent>(uid, ref access, true))
			{
				return access.Tags;
			}
			return null;
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x0004CAAC File Offset: 0x0004ACAC
		public bool TryAddGroups(EntityUid uid, IEnumerable<string> newGroups, [Nullable(2)] AccessComponent access = null)
		{
			if (!base.Resolve<AccessComponent>(uid, ref access, true))
			{
				return false;
			}
			foreach (string group in newGroups)
			{
				AccessGroupPrototype proto;
				if (this._prototypeManager.TryIndex<AccessGroupPrototype>(group, ref proto))
				{
					access.Tags.UnionWith(proto.Tags);
				}
			}
			base.Dirty(access, null);
			return true;
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x0004CB28 File Offset: 0x0004AD28
		public void SetAccessToJob(EntityUid uid, JobPrototype prototype, bool extended, [Nullable(2)] AccessComponent access = null)
		{
			if (!base.Resolve<AccessComponent>(uid, ref access, true))
			{
				return;
			}
			access.Tags.Clear();
			access.Tags.UnionWith(prototype.Access);
			this.TryAddGroups(uid, prototype.AccessGroups, access);
			if (extended)
			{
				access.Tags.UnionWith(prototype.ExtendedAccess);
				this.TryAddGroups(uid, prototype.ExtendedAccessGroups, access);
			}
		}

		// Token: 0x04001753 RID: 5971
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x020008AB RID: 2219
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		private sealed class AccessComponentState : ComponentState
		{
			// Token: 0x04001AAD RID: 6829
			public HashSet<string> Tags = new HashSet<string>();

			// Token: 0x04001AAE RID: 6830
			public HashSet<string> Groups = new HashSet<string>();
		}
	}
}
