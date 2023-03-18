using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Shared.Atmos.Visuals;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000796 RID: 1942
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosPlaqueSystem : EntitySystem
	{
		// Token: 0x06002A00 RID: 10752 RVA: 0x000DCC8A File Offset: 0x000DAE8A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AtmosPlaqueComponent, MapInitEvent>(new ComponentEventHandler<AtmosPlaqueComponent, MapInitEvent>(this.OnPlaqueMapInit), null, null);
		}

		// Token: 0x06002A01 RID: 10753 RVA: 0x000DCCA8 File Offset: 0x000DAEA8
		private void OnPlaqueMapInit(EntityUid uid, AtmosPlaqueComponent component, MapInitEvent args)
		{
			int rand = this._random.Next(100);
			if (rand == 0)
			{
				component.Type = PlaqueType.Zumos;
			}
			else if (rand <= 10)
			{
				component.Type = PlaqueType.Fea;
			}
			else if (rand <= 55)
			{
				component.Type = PlaqueType.Zas;
			}
			else
			{
				component.Type = PlaqueType.Linda;
			}
			this.UpdateSign(uid, component);
		}

		// Token: 0x06002A02 RID: 10754 RVA: 0x000DCCFC File Offset: 0x000DAEFC
		public void UpdateSign(EntityUid uid, AtmosPlaqueComponent component)
		{
			MetaDataComponent metaData = base.MetaData(component.Owner);
			string @string;
			switch (component.Type)
			{
			case PlaqueType.Unset:
				@string = Loc.GetString("atmos-plaque-component-desc-unset");
				break;
			case PlaqueType.Zumos:
				@string = Loc.GetString("atmos-plaque-component-desc-zum");
				break;
			case PlaqueType.Fea:
				@string = Loc.GetString("atmos-plaque-component-desc-fea");
				break;
			case PlaqueType.Linda:
				@string = Loc.GetString("atmos-plaque-component-desc-linda");
				break;
			case PlaqueType.Zas:
				@string = Loc.GetString("atmos-plaque-component-desc-zas");
				break;
			default:
				@string = Loc.GetString("atmos-plaque-component-desc-unset");
				break;
			}
			string val = @string;
			metaData.EntityDescription = val;
			switch (component.Type)
			{
			case PlaqueType.Unset:
				@string = Loc.GetString("atmos-plaque-component-name-unset");
				break;
			case PlaqueType.Zumos:
				@string = Loc.GetString("atmos-plaque-component-name-zum");
				break;
			case PlaqueType.Fea:
				@string = Loc.GetString("atmos-plaque-component-name-fea");
				break;
			case PlaqueType.Linda:
				@string = Loc.GetString("atmos-plaque-component-name-linda");
				break;
			case PlaqueType.Zas:
				@string = Loc.GetString("atmos-plaque-component-name-zas");
				break;
			default:
				@string = Loc.GetString("atmos-plaque-component-name-unset");
				break;
			}
			string val2 = @string;
			metaData.EntityName = val2;
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				string state = (component.Type == PlaqueType.Zumos) ? "zumosplaque" : "atmosplaque";
				this._appearance.SetData(uid, AtmosPlaqueVisuals.State, state, appearance);
			}
		}

		// Token: 0x040019EC RID: 6636
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040019ED RID: 6637
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
