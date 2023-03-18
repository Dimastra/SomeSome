using System;
using System.Runtime.CompilerServices;
using Content.Server.Morgue.Components;
using Content.Shared.Body.Components;
using Content.Shared.Standing;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Morgue
{
	// Token: 0x02000397 RID: 919
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntityStorageLayingDownOverrideSystem : EntitySystem
	{
		// Token: 0x060012D4 RID: 4820 RVA: 0x000618A0 File Offset: 0x0005FAA0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>(new ComponentEventRefHandler<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>(this.OnBeforeClose), null, null);
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x000618BC File Offset: 0x0005FABC
		private void OnBeforeClose(EntityUid uid, EntityStorageLayingDownOverrideComponent component, ref StorageBeforeCloseEvent args)
		{
			foreach (EntityUid ent in args.Contents)
			{
				if (base.HasComp<BodyComponent>(ent) && !this._standing.IsDown(ent, null))
				{
					args.Contents.Remove(ent);
				}
			}
		}

		// Token: 0x04000B83 RID: 2947
		[Dependency]
		private readonly StandingStateSystem _standing;
	}
}
