using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.BarSign;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.BarSign.Systems
{
	// Token: 0x0200072C RID: 1836
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BarSignSystem : EntitySystem
	{
		// Token: 0x06002689 RID: 9865 RVA: 0x000CBE5A File Offset: 0x000CA05A
		public override void Initialize()
		{
			base.SubscribeLocalEvent<BarSignComponent, MapInitEvent>(new ComponentEventHandler<BarSignComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<BarSignComponent, ComponentGetState>(new ComponentEventRefHandler<BarSignComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x000CBE84 File Offset: 0x000CA084
		private void OnGetState(EntityUid uid, BarSignComponent component, ref ComponentGetState args)
		{
			args.State = new BarSignComponentState(component.CurrentSign);
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x000CBE98 File Offset: 0x000CA098
		private void OnMapInit(EntityUid uid, BarSignComponent component, MapInitEvent args)
		{
			if (component.CurrentSign != null)
			{
				return;
			}
			List<BarSignPrototype> prototypes = (from p in this._prototypeManager.EnumeratePrototypes<BarSignPrototype>()
			where !p.Hidden
			select p).ToList<BarSignPrototype>();
			BarSignPrototype newPrototype = RandomExtensions.Pick<BarSignPrototype>(this._random, prototypes);
			MetaDataComponent metaDataComponent = base.Comp<MetaDataComponent>(uid);
			string name = (newPrototype.Name != string.Empty) ? newPrototype.Name : "barsign-component-name";
			metaDataComponent.EntityName = Loc.GetString(name);
			metaDataComponent.EntityDescription = Loc.GetString(newPrototype.Description);
			component.CurrentSign = newPrototype.ID;
			base.Dirty(component, null);
		}

		// Token: 0x040017FA RID: 6138
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040017FB RID: 6139
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
