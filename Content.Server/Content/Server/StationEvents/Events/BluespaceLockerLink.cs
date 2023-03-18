using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Resist;
using Content.Server.Station.Components;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Access.Components;
using Content.Shared.Coordinates;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000183 RID: 387
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BluespaceLockerLink : StationEventSystem
	{
		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x00025DB1 File Offset: 0x00023FB1
		public override string Prototype
		{
			get
			{
				return "BluespaceLockerLink";
			}
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00025DB8 File Offset: 0x00023FB8
		public override void Started()
		{
			base.Started();
			List<ValueTuple<EntityStorageComponent, ResistLockerComponent>> targets = base.EntityQuery<EntityStorageComponent, ResistLockerComponent>(false).ToList<ValueTuple<EntityStorageComponent, ResistLockerComponent>>();
			this._robustRandom.Shuffle<ValueTuple<EntityStorageComponent, ResistLockerComponent>>(targets);
			foreach (ValueTuple<EntityStorageComponent, ResistLockerComponent> valueTuple in targets)
			{
				EntityUid potentialLink = valueTuple.Item1.Owner;
				if (!base.HasComp<AccessReaderComponent>(potentialLink) && !base.HasComp<BluespaceLockerComponent>(potentialLink) && base.HasComp<StationMemberComponent>(potentialLink.ToCoordinates().GetGridUid(this.EntityManager)))
				{
					BluespaceLockerComponent comp = base.AddComp<BluespaceLockerComponent>(potentialLink);
					comp.PickLinksFromSameMap = true;
					comp.MinBluespaceLinks = 1U;
					comp.BehaviorProperties.BluespaceEffectOnTeleportSource = true;
					comp.AutoLinksBidirectional = true;
					comp.AutoLinksUseProperties = true;
					comp.AutoLinkProperties.BluespaceEffectOnInit = true;
					comp.AutoLinkProperties.BluespaceEffectOnTeleportSource = true;
					this._bluespaceLocker.GetTarget(potentialLink, comp, true);
					this._bluespaceLocker.BluespaceEffect(potentialLink, comp, comp, true);
					ISawmill sawmill = this.Sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Converted ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(potentialLink));
					defaultInterpolatedStringHandler.AppendLiteral(" to bluespace locker");
					sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					break;
				}
			}
		}

		// Token: 0x040004A3 RID: 1187
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040004A4 RID: 1188
		[Dependency]
		private readonly BluespaceLockerSystem _bluespaceLocker;
	}
}
