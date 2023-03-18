using System;
using System.Runtime.CompilerServices;
using Content.Shared.Random.Helpers;
using Robust.Shared.GameObjects;

namespace Content.Server.Coordinates
{
	// Token: 0x020005E3 RID: 1507
	public sealed class SpawnRandomOffsetSystem : EntitySystem
	{
		// Token: 0x06002033 RID: 8243 RVA: 0x000A7EE6 File Offset: 0x000A60E6
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpawnRandomOffsetComponent, MapInitEvent>(new ComponentEventHandler<SpawnRandomOffsetComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x000A7F02 File Offset: 0x000A6102
		[NullableContext(1)]
		private void OnMapInit(EntityUid uid, SpawnRandomOffsetComponent component, MapInitEvent args)
		{
			uid.RandomOffset(component.Offset);
			this.EntityManager.RemoveComponentDeferred(uid, component);
		}
	}
}
