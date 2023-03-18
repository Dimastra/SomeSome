using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Smoking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x02000410 RID: 1040
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MatchboxSystem : EntitySystem
	{
		// Token: 0x06001522 RID: 5410 RVA: 0x0006ED51 File Offset: 0x0006CF51
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MatchboxComponent, InteractUsingEvent>(new ComponentEventHandler<MatchboxComponent, InteractUsingEvent>(this.OnInteractUsing), new Type[]
			{
				typeof(StorageSystem)
			}, null);
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x0006ED80 File Offset: 0x0006CF80
		private void OnInteractUsing(EntityUid uid, MatchboxComponent component, InteractUsingEvent args)
		{
			MatchstickComponent matchstick;
			if (!args.Handled && this.EntityManager.TryGetComponent<MatchstickComponent>(args.Used, ref matchstick) && matchstick.CurrentState == SmokableState.Unlit)
			{
				this._stickSystem.Ignite(args.Used, matchstick, args.User);
				args.Handled = true;
			}
		}

		// Token: 0x04000D10 RID: 3344
		[Dependency]
		private readonly MatchstickSystem _stickSystem;
	}
}
