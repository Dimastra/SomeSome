using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Holiday
{
	// Token: 0x02000461 RID: 1121
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HolidaySystem : EntitySystem
	{
		// Token: 0x060016A5 RID: 5797 RVA: 0x00077507 File Offset: 0x00075707
		public override void Initialize()
		{
			this._configManager.OnValueChanged<bool>(CCVars.HolidaysEnabled, new Action<bool>(this.OnHolidaysEnableChange), true);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRunLevelChanged), null, null);
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x0007753C File Offset: 0x0007573C
		public void RefreshCurrentHolidays()
		{
			this._currentHolidays.Clear();
			if (!this._enabled)
			{
				base.RaiseLocalEvent<HolidaysRefreshedEvent>(new HolidaysRefreshedEvent(Enumerable.Empty<HolidayPrototype>()));
				return;
			}
			DateTime now = DateTime.Now;
			foreach (HolidayPrototype holiday in this._prototypeManager.EnumeratePrototypes<HolidayPrototype>())
			{
				if (holiday.ShouldCelebrate(now))
				{
					this._currentHolidays.Add(holiday);
				}
			}
			base.RaiseLocalEvent<HolidaysRefreshedEvent>(new HolidaysRefreshedEvent(this._currentHolidays));
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x000775D8 File Offset: 0x000757D8
		public void DoGreet()
		{
			foreach (HolidayPrototype holiday in this._currentHolidays)
			{
				this._chatManager.DispatchServerAnnouncement(holiday.Greet(), null);
			}
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00077640 File Offset: 0x00075840
		public void DoCelebrate()
		{
			foreach (HolidayPrototype holidayPrototype in this._currentHolidays)
			{
				holidayPrototype.Celebrate();
			}
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00077690 File Offset: 0x00075890
		public IEnumerable<HolidayPrototype> GetCurrentHolidays()
		{
			return this._currentHolidays;
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x00077698 File Offset: 0x00075898
		public bool IsCurrentlyHoliday(string holiday)
		{
			HolidayPrototype prototype;
			return this._prototypeManager.TryIndex<HolidayPrototype>(holiday, ref prototype) && this._currentHolidays.Contains(prototype);
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x000776C3 File Offset: 0x000758C3
		private void OnHolidaysEnableChange(bool enabled)
		{
			this._enabled = enabled;
			this.RefreshCurrentHolidays();
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x000776D4 File Offset: 0x000758D4
		private void OnRunLevelChanged(GameRunLevelChangedEvent eventArgs)
		{
			if (!this._enabled)
			{
				return;
			}
			switch (eventArgs.New)
			{
			case GameRunLevel.PreRoundLobby:
				this.RefreshCurrentHolidays();
				return;
			case GameRunLevel.InRound:
				this.DoGreet();
				this.DoCelebrate();
				break;
			case GameRunLevel.PostRound:
				break;
			default:
				return;
			}
		}

		// Token: 0x04000E23 RID: 3619
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000E24 RID: 3620
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000E25 RID: 3621
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000E26 RID: 3622
		[ViewVariables]
		private readonly List<HolidayPrototype> _currentHolidays = new List<HolidayPrototype>();

		// Token: 0x04000E27 RID: 3623
		[ViewVariables]
		private bool _enabled = true;
	}
}
