using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Fax;
using Content.Server.Paper;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Nuke
{
	// Token: 0x02000325 RID: 805
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeCodePaperSystem : EntitySystem
	{
		// Token: 0x0600109A RID: 4250 RVA: 0x000554B4 File Offset: 0x000536B4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NukeCodePaperComponent, MapInitEvent>(new ComponentEventHandler<NukeCodePaperComponent, MapInitEvent>(this.OnMapInit), null, new Type[]
			{
				typeof(NukeLabelSystem)
			});
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x000554E4 File Offset: 0x000536E4
		private void OnMapInit(EntityUid uid, NukeCodePaperComponent component, MapInitEvent args)
		{
			this.SetupPaper(uid, null);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00055504 File Offset: 0x00053704
		private void SetupPaper(EntityUid uid, EntityUid? station = null)
		{
			string paperContent;
			if (this.TryGetRelativeNukeCode(uid, out paperContent, station, null))
			{
				this._paper.SetContent(uid, paperContent, null);
			}
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0005552C File Offset: 0x0005372C
		public bool SendNukeCodes(EntityUid station)
		{
			if (!base.HasComp<StationDataComponent>(station))
			{
				return false;
			}
			IEnumerable<FaxMachineComponent> enumerable = this.EntityManager.EntityQuery<FaxMachineComponent>(false);
			bool wasSent = false;
			foreach (FaxMachineComponent fax in enumerable)
			{
				string paperContent;
				if (fax.ReceiveNukeCodes && this.TryGetRelativeNukeCode(fax.Owner, out paperContent, new EntityUid?(station), null))
				{
					FaxPrintout printout = new FaxPrintout(paperContent, Loc.GetString("nuke-codes-fax-paper-name"), null, "paper_stamp-cent", new List<string>
					{
						Loc.GetString("stamp-component-stamped-name-centcom")
					});
					this._faxSystem.Receive(fax.Owner, printout, null, fax);
					wasSent = true;
				}
			}
			if (wasSent)
			{
				string msg = Loc.GetString("nuke-component-announcement-send-codes");
				this._chatSystem.DispatchStationAnnouncement(station, msg, "Central Command", true, null, new Color?(Color.Red));
			}
			return wasSent;
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x00055618 File Offset: 0x00053818
		[NullableContext(2)]
		private bool TryGetRelativeNukeCode(EntityUid uid, [NotNullWhen(true)] out string nukeCode, EntityUid? station = null, TransformComponent transform = null)
		{
			nukeCode = null;
			if (!base.Resolve<TransformComponent>(uid, ref transform, true))
			{
				return false;
			}
			EntityUid? entityUid = station;
			EntityUid? owningStation = (entityUid != null) ? entityUid : this._station.GetOwningStation(uid, null);
			foreach (NukeComponent nuke in base.EntityQuery<NukeComponent>(false))
			{
				if (owningStation == null)
				{
					ValueTuple<MapId, EntityUid?>? originMapGrid = nuke.OriginMapGrid;
					MapId mapID = transform.MapID;
					entityUid = transform.GridUid;
					bool flag = originMapGrid != null;
					bool flag2;
					if (!flag)
					{
						flag2 = true;
					}
					else if (!flag)
					{
						flag2 = false;
					}
					else
					{
						ValueTuple<MapId, EntityUid?> valueOrDefault = originMapGrid.GetValueOrDefault();
						flag2 = (valueOrDefault.Item1 != mapID || valueOrDefault.Item2 != entityUid);
					}
					if (flag2)
					{
						continue;
					}
				}
				if (!(nuke.OriginStation != owningStation))
				{
					nukeCode = Loc.GetString("nuke-codes-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", base.MetaData(nuke.Owner).EntityName),
						new ValueTuple<string, object>("code", nuke.Code)
					});
					return true;
				}
			}
			return false;
		}

		// Token: 0x040009C1 RID: 2497
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x040009C2 RID: 2498
		[Dependency]
		private readonly StationSystem _station;

		// Token: 0x040009C3 RID: 2499
		[Dependency]
		private readonly PaperSystem _paper;

		// Token: 0x040009C4 RID: 2500
		[Dependency]
		private readonly FaxSystem _faxSystem;
	}
}
