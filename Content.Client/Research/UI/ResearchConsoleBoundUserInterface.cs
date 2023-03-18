using System;
using System.Runtime.CompilerServices;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Research.UI
{
	// Token: 0x02000171 RID: 369
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResearchConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x00037C5D File Offset: 0x00035E5D
		// (set) Token: 0x06000999 RID: 2457 RVA: 0x00037C65 File Offset: 0x00035E65
		public int Points { get; private set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x00037C6E File Offset: 0x00035E6E
		// (set) Token: 0x0600099B RID: 2459 RVA: 0x00037C76 File Offset: 0x00035E76
		public int PointsPerSecond { get; private set; }

		// Token: 0x0600099C RID: 2460 RVA: 0x00037C7F File Offset: 0x00035E7F
		public ResearchConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			base.SendMessage(new ConsoleServerSyncMessage());
			this._entityManager = IoCManager.Resolve<IEntityManager>();
			this._research = this._entityManager.System<SharedResearchSystem>();
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00037CB0 File Offset: 0x00035EB0
		protected override void Open()
		{
			base.Open();
			if (!this._entityManager.TryGetComponent<TechnologyDatabaseComponent>(base.Owner.Owner, ref this._technologyDatabase))
			{
				return;
			}
			this._consoleMenu = new ResearchConsoleMenu(this);
			this._consoleMenu.OnClose += base.Close;
			this._consoleMenu.ServerSyncButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ConsoleServerSyncMessage());
			};
			this._consoleMenu.ServerSelectionButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ConsoleServerSelectionMessage());
			};
			this._consoleMenu.UnlockButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				if (this._consoleMenu.TechnologySelected != null)
				{
					base.SendMessage(new ConsoleUnlockTechnologyMessage(this._consoleMenu.TechnologySelected.ID));
				}
			};
			this._consoleMenu.OpenCentered();
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00037D64 File Offset: 0x00035F64
		public bool IsTechnologyUnlocked(TechnologyPrototype technology)
		{
			return this._technologyDatabase != null && this._research.IsTechnologyUnlocked(this._technologyDatabase.Owner, technology, this._technologyDatabase);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00037D8D File Offset: 0x00035F8D
		public bool CanUnlockTechnology(TechnologyPrototype technology)
		{
			return this._technologyDatabase != null && this._research.ArePrerequesitesUnlocked(this._technologyDatabase.Owner, technology, this._technologyDatabase);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00037DB8 File Offset: 0x00035FB8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ResearchConsoleBoundInterfaceState researchConsoleBoundInterfaceState = (ResearchConsoleBoundInterfaceState)state;
			this.Points = researchConsoleBoundInterfaceState.Points;
			this.PointsPerSecond = researchConsoleBoundInterfaceState.PointsPerSecond;
			ResearchConsoleMenu consoleMenu = this._consoleMenu;
			if (consoleMenu != null)
			{
				consoleMenu.PopulatePoints();
			}
			ResearchConsoleMenu consoleMenu2 = this._consoleMenu;
			if (consoleMenu2 == null)
			{
				return;
			}
			consoleMenu2.Populate();
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00037E0C File Offset: 0x0003600C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			ResearchConsoleMenu consoleMenu = this._consoleMenu;
			if (consoleMenu == null)
			{
				return;
			}
			consoleMenu.Dispose();
		}

		// Token: 0x040004CD RID: 1229
		[Nullable(2)]
		private ResearchConsoleMenu _consoleMenu;

		// Token: 0x040004CE RID: 1230
		[Nullable(2)]
		private TechnologyDatabaseComponent _technologyDatabase;

		// Token: 0x040004CF RID: 1231
		private readonly IEntityManager _entityManager;

		// Token: 0x040004D0 RID: 1232
		private readonly SharedResearchSystem _research;
	}
}
