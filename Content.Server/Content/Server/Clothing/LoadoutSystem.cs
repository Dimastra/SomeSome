using System;
using System.Runtime.CompilerServices;
using Content.Server.Clothing.Components;
using Content.Server.Station.Systems;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Clothing
{
	// Token: 0x02000636 RID: 1590
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LoadoutSystem : EntitySystem
	{
		// Token: 0x060021D3 RID: 8659 RVA: 0x000B0716 File Offset: 0x000AE916
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LoadoutComponent, ComponentStartup>(new ComponentEventHandler<LoadoutComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x000B0734 File Offset: 0x000AE934
		private void OnStartup(EntityUid uid, LoadoutComponent component, ComponentStartup args)
		{
			if (component.Prototypes == null)
			{
				return;
			}
			StartingGearPrototype proto = this._protoMan.Index<StartingGearPrototype>(RandomExtensions.Pick<string>(this._random, component.Prototypes));
			this._station.EquipStartingGear(uid, proto, null);
		}

		// Token: 0x040014BB RID: 5307
		[Dependency]
		private readonly StationSpawningSystem _station;

		// Token: 0x040014BC RID: 5308
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x040014BD RID: 5309
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
