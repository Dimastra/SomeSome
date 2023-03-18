using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Shared.Traits;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Traits
{
	// Token: 0x02000102 RID: 258
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitSystem : EntitySystem
	{
		// Token: 0x060004B1 RID: 1201 RVA: 0x0001678C File Offset: 0x0001498C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawnComplete), null, null);
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x000167A8 File Offset: 0x000149A8
		private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent args)
		{
			foreach (string traitId in args.Profile.TraitPreferences)
			{
				TraitPrototype traitPrototype;
				if (!this._prototypeManager.TryIndex<TraitPrototype>(traitId, ref traitPrototype))
				{
					Logger.Warning("No trait found with ID " + traitId + "!");
					break;
				}
				if ((traitPrototype.Whitelist == null || traitPrototype.Whitelist.IsValid(args.Mob, null)) && (traitPrototype.Blacklist == null || !traitPrototype.Blacklist.IsValid(args.Mob, null)))
				{
					foreach (EntityPrototype.ComponentRegistryEntry entry in traitPrototype.Components.Values)
					{
						Component comp = (Component)this._serializationManager.CreateCopy<IComponent>(entry.Component, null, false, true);
						comp.Owner = args.Mob;
						this.EntityManager.AddComponent<Component>(args.Mob, comp, false);
					}
				}
			}
		}

		// Token: 0x040002B8 RID: 696
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040002B9 RID: 697
		[Dependency]
		private readonly ISerializationManager _serializationManager;
	}
}
