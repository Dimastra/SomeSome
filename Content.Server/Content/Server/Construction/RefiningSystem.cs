using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Server.Stack;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Construction
{
	// Token: 0x020005F3 RID: 1523
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RefiningSystem : EntitySystem
	{
		// Token: 0x060020AB RID: 8363 RVA: 0x000AC812 File Offset: 0x000AAA12
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WelderRefinableComponent, InteractUsingEvent>(new ComponentEventHandler<WelderRefinableComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x000AC830 File Offset: 0x000AAA30
		private void OnInteractUsing(EntityUid uid, WelderRefinableComponent component, InteractUsingEvent args)
		{
			RefiningSystem.<OnInteractUsing>d__3 <OnInteractUsing>d__;
			<OnInteractUsing>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnInteractUsing>d__.<>4__this = this;
			<OnInteractUsing>d__.uid = uid;
			<OnInteractUsing>d__.component = component;
			<OnInteractUsing>d__.args = args;
			<OnInteractUsing>d__.<>1__state = -1;
			<OnInteractUsing>d__.<>t__builder.Start<RefiningSystem.<OnInteractUsing>d__3>(ref <OnInteractUsing>d__);
		}

		// Token: 0x0400142E RID: 5166
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x0400142F RID: 5167
		[Dependency]
		private readonly StackSystem _stackSystem;
	}
}
