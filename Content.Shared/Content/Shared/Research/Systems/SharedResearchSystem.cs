using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Research.Systems
{
	// Token: 0x02000201 RID: 513
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedResearchSystem : EntitySystem
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x0001475C File Offset: 0x0001295C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ResearchServerComponent, ComponentGetState>(new ComponentEventRefHandler<ResearchServerComponent, ComponentGetState>(this.OnServerGetState), null, null);
			base.SubscribeLocalEvent<ResearchServerComponent, ComponentHandleState>(new ComponentEventRefHandler<ResearchServerComponent, ComponentHandleState>(this.OnServerHandleState), null, null);
			base.SubscribeLocalEvent<TechnologyDatabaseComponent, ComponentGetState>(new ComponentEventRefHandler<TechnologyDatabaseComponent, ComponentGetState>(this.OnTechnologyGetState), null, null);
			base.SubscribeLocalEvent<TechnologyDatabaseComponent, ComponentHandleState>(new ComponentEventRefHandler<TechnologyDatabaseComponent, ComponentHandleState>(this.OnTechnologyHandleState), null, null);
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x000147BF File Offset: 0x000129BF
		private void OnServerGetState(EntityUid uid, ResearchServerComponent component, ref ComponentGetState args)
		{
			args.State = new ResearchServerState(component.ServerName, component.Points, component.Id);
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x000147E0 File Offset: 0x000129E0
		private void OnServerHandleState(EntityUid uid, ResearchServerComponent component, ref ComponentHandleState args)
		{
			ResearchServerState state = args.Current as ResearchServerState;
			if (state == null)
			{
				return;
			}
			component.ServerName = state.ServerName;
			component.Points = state.Points;
			component.Id = state.Id;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00014824 File Offset: 0x00012A24
		private void OnTechnologyHandleState(EntityUid uid, TechnologyDatabaseComponent component, ref ComponentHandleState args)
		{
			TechnologyDatabaseState state = args.Current as TechnologyDatabaseState;
			if (state == null)
			{
				return;
			}
			component.TechnologyIds = new List<string>(state.Technologies);
			component.RecipeIds = new List<string>(state.Recipes);
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00014863 File Offset: 0x00012A63
		private void OnTechnologyGetState(EntityUid uid, TechnologyDatabaseComponent component, ref ComponentGetState args)
		{
			args.State = new TechnologyDatabaseState(component.TechnologyIds, component.RecipeIds);
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0001487C File Offset: 0x00012A7C
		public bool IsTechnologyUnlocked(EntityUid uid, TechnologyPrototype technology, [Nullable(2)] TechnologyDatabaseComponent component = null)
		{
			return base.Resolve<TechnologyDatabaseComponent>(uid, ref component, true) && this.IsTechnologyUnlocked(uid, technology.ID, component);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001489A File Offset: 0x00012A9A
		public bool IsTechnologyUnlocked(EntityUid uid, string technologyId, [Nullable(2)] TechnologyDatabaseComponent component = null)
		{
			return base.Resolve<TechnologyDatabaseComponent>(uid, ref component, false) && component.TechnologyIds.Contains(technologyId);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x000148B8 File Offset: 0x00012AB8
		public bool ArePrerequesitesUnlocked(EntityUid uid, TechnologyPrototype prototype, [Nullable(2)] TechnologyDatabaseComponent component = null)
		{
			if (!base.Resolve<TechnologyDatabaseComponent>(uid, ref component, true))
			{
				return false;
			}
			foreach (string technologyId in prototype.RequiredTechnologies)
			{
				if (!this.IsTechnologyUnlocked(uid, technologyId, component))
				{
					return false;
				}
			}
			return true;
		}
	}
}
