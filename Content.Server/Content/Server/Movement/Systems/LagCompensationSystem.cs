using System;
using System.Runtime.CompilerServices;
using Content.Server.Movement.Components;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Movement.Systems
{
	// Token: 0x02000390 RID: 912
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LagCompensationSystem : EntitySystem
	{
		// Token: 0x060012B2 RID: 4786 RVA: 0x00060E48 File Offset: 0x0005F048
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill.Level = new LogLevel?(2);
			base.SubscribeLocalEvent<LagCompensationComponent, MoveEvent>(new ComponentEventRefHandler<LagCompensationComponent, MoveEvent>(this.OnLagMove), null, null);
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00060E78 File Offset: 0x0005F078
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			TimeSpan earliestTime = this._timing.CurTime - LagCompensationSystem.BufferTime;
			foreach (ValueTuple<ActiveLagCompensationComponent, LagCompensationComponent> valueTuple in base.EntityQuery<ActiveLagCompensationComponent, LagCompensationComponent>(true))
			{
				LagCompensationComponent comp = valueTuple.Item2;
				ValueTuple<TimeSpan, EntityCoordinates, Angle> pos;
				while (comp.Positions.TryPeek(out pos) && pos.Item1 < earliestTime)
				{
					comp.Positions.Dequeue();
				}
				if (comp.Positions.Count == 0)
				{
					base.RemComp<ActiveLagCompensationComponent>(comp.Owner);
				}
			}
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x00060F28 File Offset: 0x0005F128
		private void OnLagMove(EntityUid uid, LagCompensationComponent component, ref MoveEvent args)
		{
			if (!args.NewPosition.EntityId.IsValid())
			{
				return;
			}
			base.EnsureComp<ActiveLagCompensationComponent>(uid);
			component.Positions.Enqueue(new ValueTuple<TimeSpan, EntityCoordinates, Angle>(this._timing.CurTime, args.NewPosition, args.NewRotation));
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x00060F78 File Offset: 0x0005F178
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Coordinates",
			"Angle"
		})]
		public ValueTuple<EntityCoordinates, Angle> GetCoordinatesAngle(EntityUid uid, [Nullable(1)] IPlayerSession pSession, [Nullable(2)] TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return new ValueTuple<EntityCoordinates, Angle>(EntityCoordinates.Invalid, Angle.Zero);
			}
			LagCompensationComponent lag;
			if (!base.TryComp<LagCompensationComponent>(uid, ref lag) || lag.Positions.Count == 0)
			{
				return new ValueTuple<EntityCoordinates, Angle>(xform.Coordinates, xform.LocalRotation);
			}
			Angle angle = Angle.Zero;
			EntityCoordinates coordinates = EntityCoordinates.Invalid;
			short ping = pSession.Ping;
			TimeSpan sentTime = this._timing.CurTime - TimeSpan.FromMilliseconds((double)ping * 1.5);
			foreach (ValueTuple<TimeSpan, EntityCoordinates, Angle> valueTuple in lag.Positions)
			{
				coordinates = valueTuple.Item2;
				angle = valueTuple.Item3;
				if (valueTuple.Item1 >= sentTime)
				{
					break;
				}
			}
			if (coordinates == default(EntityCoordinates))
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No long comp coords found, using ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(xform.Coordinates);
				sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				coordinates = xform.Coordinates;
				angle = xform.LocalRotation;
			}
			else
			{
				ISawmill sawmill2 = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Actual coords is ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(xform.Coordinates);
				defaultInterpolatedStringHandler.AppendLiteral(" and got ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coordinates);
				sawmill2.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return new ValueTuple<EntityCoordinates, Angle>(coordinates, angle);
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x00061108 File Offset: 0x0005F308
		public Angle GetAngle(EntityUid uid, IPlayerSession pSession, [Nullable(2)] TransformComponent xform = null)
		{
			return this.GetCoordinatesAngle(uid, pSession, xform).Item2;
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x00061118 File Offset: 0x0005F318
		public EntityCoordinates GetCoordinates(EntityUid uid, IPlayerSession pSession, [Nullable(2)] TransformComponent xform = null)
		{
			return this.GetCoordinatesAngle(uid, pSession, xform).Item1;
		}

		// Token: 0x04000B73 RID: 2931
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000B74 RID: 2932
		public static readonly TimeSpan BufferTime = TimeSpan.FromMilliseconds(750.0);

		// Token: 0x04000B75 RID: 2933
		private ISawmill _sawmill = Logger.GetSawmill("lagcomp");
	}
}
