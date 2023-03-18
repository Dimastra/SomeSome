using System;
using System.Runtime.CompilerServices;
using Content.Server.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001FA RID: 506
	public sealed class SpaceGarbageSystem : EntitySystem
	{
		// Token: 0x06000A3C RID: 2620 RVA: 0x00036023 File Offset: 0x00034223
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpaceGarbageComponent, StartCollideEvent>(new ComponentEventRefHandler<SpaceGarbageComponent, StartCollideEvent>(this.OnCollide), null, null);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00036040 File Offset: 0x00034240
		[NullableContext(1)]
		private void OnCollide(EntityUid uid, SpaceGarbageComponent component, ref StartCollideEvent args)
		{
			if (args.OtherFixture.Body.BodyType != 4)
			{
				return;
			}
			TransformComponent ourXform = base.Transform(args.OurFixture.Body.Owner);
			TransformComponent transformComponent = base.Transform(args.OtherFixture.Body.Owner);
			if (ourXform.GridUid == transformComponent.GridUid)
			{
				return;
			}
			base.QueueDel(uid);
		}
	}
}
