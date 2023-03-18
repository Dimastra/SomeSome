using System;
using System.Runtime.CompilerServices;
using Content.Server.DoAfter;
using Content.Server.Engineering.Components;
using Content.Server.Stack;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Engineering.EntitySystems
{
	// Token: 0x0200052C RID: 1324
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpawnAfterInteractSystem : EntitySystem
	{
		// Token: 0x06001B95 RID: 7061 RVA: 0x00093A78 File Offset: 0x00091C78
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpawnAfterInteractComponent, AfterInteractEvent>(new ComponentEventHandler<SpawnAfterInteractComponent, AfterInteractEvent>(this.HandleAfterInteract), null, null);
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x00093A94 File Offset: 0x00091C94
		private void HandleAfterInteract(EntityUid uid, SpawnAfterInteractComponent component, AfterInteractEvent args)
		{
			SpawnAfterInteractSystem.<HandleAfterInteract>d__4 <HandleAfterInteract>d__;
			<HandleAfterInteract>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleAfterInteract>d__.<>4__this = this;
			<HandleAfterInteract>d__.uid = uid;
			<HandleAfterInteract>d__.component = component;
			<HandleAfterInteract>d__.args = args;
			<HandleAfterInteract>d__.<>1__state = -1;
			<HandleAfterInteract>d__.<>t__builder.Start<SpawnAfterInteractSystem.<HandleAfterInteract>d__4>(ref <HandleAfterInteract>d__);
		}

		// Token: 0x040011AC RID: 4524
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040011AD RID: 4525
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040011AE RID: 4526
		[Dependency]
		private readonly StackSystem _stackSystem;
	}
}
