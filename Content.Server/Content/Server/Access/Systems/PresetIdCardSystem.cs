using System;
using System.Runtime.CompilerServices;
using Content.Server.Access.Components;
using Content.Server.GameTicking;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Access.Systems;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Access.Systems
{
	// Token: 0x0200087E RID: 2174
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PresetIdCardSystem : EntitySystem
	{
		// Token: 0x06002F7C RID: 12156 RVA: 0x000F5F20 File Offset: 0x000F4120
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PresetIdCardComponent, MapInitEvent>(new ComponentEventHandler<PresetIdCardComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(new EntityEventHandler<RulePlayerJobsAssignedEvent>(this.PlayerJobsAssigned), null, null);
		}

		// Token: 0x06002F7D RID: 12157 RVA: 0x000F5F4C File Offset: 0x000F414C
		private void PlayerJobsAssigned(RulePlayerJobsAssignedEvent ev)
		{
			foreach (PresetIdCardComponent card in base.EntityQuery<PresetIdCardComponent>(false))
			{
				EntityUid? station = this._stationSystem.GetOwningStation(card.Owner, null);
				if (station == null || !base.Comp<StationJobsComponent>(station.Value).ExtendedAccess)
				{
					break;
				}
				this.SetupIdAccess(card.Owner, card, true);
			}
		}

		// Token: 0x06002F7E RID: 12158 RVA: 0x000F5FD4 File Offset: 0x000F41D4
		private void OnMapInit(EntityUid uid, PresetIdCardComponent id, MapInitEvent args)
		{
			EntityUid? station = this._stationSystem.GetOwningStation(id.Owner, null);
			bool extended = false;
			if (station != null)
			{
				extended = base.Comp<StationJobsComponent>(station.Value).ExtendedAccess;
			}
			this.SetupIdAccess(uid, id, extended);
		}

		// Token: 0x06002F7F RID: 12159 RVA: 0x000F601C File Offset: 0x000F421C
		private void SetupIdAccess(EntityUid uid, PresetIdCardComponent id, bool extended)
		{
			if (id.JobName == null)
			{
				return;
			}
			JobPrototype job;
			if (!this._prototypeManager.TryIndex<JobPrototype>(id.JobName, ref job))
			{
				Logger.ErrorS("access", "Invalid job id (" + id.JobName + ") for preset card");
				return;
			}
			this._accessSystem.SetAccessToJob(uid, job, extended, null);
			this._cardSystem.TryChangeJobTitle(uid, job.LocalizedName, null, null);
		}

		// Token: 0x04001C90 RID: 7312
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001C91 RID: 7313
		[Dependency]
		private readonly IdCardSystem _cardSystem;

		// Token: 0x04001C92 RID: 7314
		[Dependency]
		private readonly SharedAccessSystem _accessSystem;

		// Token: 0x04001C93 RID: 7315
		[Dependency]
		private readonly StationSystem _stationSystem;
	}
}
