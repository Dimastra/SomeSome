using System;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Suspicion
{
	// Token: 0x02000101 RID: 257
	internal sealed class SuspicionRoleSystem : EntitySystem
	{
		// Token: 0x0600073C RID: 1852 RVA: 0x0002604C File Offset: 0x0002424C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SuspicionRoleComponent, ComponentAdd>(delegate(EntityUid _, SuspicionRoleComponent component, ComponentAdd _)
			{
				component.AddUI();
			}, null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, ComponentRemove>(delegate(EntityUid _, SuspicionRoleComponent component, ComponentRemove _)
			{
				component.RemoveUI();
			}, null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, PlayerAttachedEvent>(delegate(EntityUid _, SuspicionRoleComponent component, PlayerAttachedEvent _)
			{
				component.AddUI();
			}, null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, PlayerDetachedEvent>(delegate(EntityUid _, SuspicionRoleComponent component, PlayerDetachedEvent _)
			{
				component.RemoveUI();
			}, null, null);
		}
	}
}
