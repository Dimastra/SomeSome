using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Spawners.Components;
using Content.Shared.Spawners.EntitySystems;
using Robust.Shared.GameObjects;

namespace Content.Server.Spawners.EntitySystems
{
	// Token: 0x020001D4 RID: 468
	public sealed class TimedDespawnSystem : SharedTimedDespawnSystem
	{
		// Token: 0x060008E9 RID: 2281 RVA: 0x0002D75C File Offset: 0x0002B95C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TimedSpawnerComponent, ComponentShutdown>(new ComponentEventHandler<TimedSpawnerComponent, ComponentShutdown>(this.OnTimedSpawnerShutdown), null, null);
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x0002D778 File Offset: 0x0002B978
		[NullableContext(1)]
		private void OnTimedSpawnerShutdown(EntityUid uid, TimedSpawnerComponent component, ComponentShutdown args)
		{
			CancellationTokenSource tokenSource = component.TokenSource;
			if (tokenSource == null)
			{
				return;
			}
			tokenSource.Cancel();
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x0002D78A File Offset: 0x0002B98A
		protected override bool CanDelete(EntityUid uid)
		{
			return true;
		}
	}
}
