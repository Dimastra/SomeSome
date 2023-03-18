using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Components;
using Content.Server.Kitchen.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Random.Helpers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Botany.Systems
{
	// Token: 0x020006FD RID: 1789
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LogSystem : EntitySystem
	{
		// Token: 0x06002577 RID: 9591 RVA: 0x000C4A26 File Offset: 0x000C2C26
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LogComponent, InteractUsingEvent>(new ComponentEventHandler<LogComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x000C4A44 File Offset: 0x000C2C44
		private void OnInteractUsing(EntityUid uid, LogComponent component, InteractUsingEvent args)
		{
			if (!base.HasComp<SharpComponent>(args.Used))
			{
				return;
			}
			bool inContainer = this._containerSystem.IsEntityInContainer(uid, null);
			EntityCoordinates pos = base.Transform(uid).Coordinates;
			for (int i = 0; i < component.SpawnCount; i++)
			{
				EntityUid plank = base.Spawn(component.SpawnedPrototype, pos);
				if (inContainer)
				{
					this._handsSystem.PickupOrDrop(new EntityUid?(args.User), plank, true, false, null, null);
				}
				else
				{
					TransformComponent xform = base.Transform(plank);
					this._containerSystem.AttachParentToContainerOrGrid(xform);
					xform.LocalRotation = 0f;
					plank.RandomOffset(0.25f);
				}
			}
			base.QueueDel(uid);
		}

		// Token: 0x0400171B RID: 5915
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x0400171C RID: 5916
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
