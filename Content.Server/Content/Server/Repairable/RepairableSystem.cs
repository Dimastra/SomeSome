using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Repairable
{
	// Token: 0x02000245 RID: 581
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RepairableSystem : EntitySystem
	{
		// Token: 0x06000BA6 RID: 2982 RVA: 0x0003D39B File Offset: 0x0003B59B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RepairableComponent, InteractUsingEvent>(new ComponentEventHandler<RepairableComponent, InteractUsingEvent>(this.Repair), null, null);
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0003D3B4 File Offset: 0x0003B5B4
		public void Repair(EntityUid uid, RepairableComponent component, InteractUsingEvent args)
		{
			RepairableSystem.<Repair>d__4 <Repair>d__;
			<Repair>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Repair>d__.<>4__this = this;
			<Repair>d__.uid = uid;
			<Repair>d__.component = component;
			<Repair>d__.args = args;
			<Repair>d__.<>1__state = -1;
			<Repair>d__.<>t__builder.Start<RepairableSystem.<Repair>d__4>(ref <Repair>d__);
		}

		// Token: 0x04000722 RID: 1826
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x04000723 RID: 1827
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04000724 RID: 1828
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
