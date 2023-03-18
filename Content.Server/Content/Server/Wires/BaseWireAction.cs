using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Wires
{
	// Token: 0x0200006F RID: 111
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class BaseWireAction : IWireAction
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000149 RID: 329
		// (set) Token: 0x0600014A RID: 330
		[DataField("name", false, 1, false, false, null)]
		public abstract string Name { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600014B RID: 331
		// (set) Token: 0x0600014C RID: 332
		[DataField("color", false, 1, false, false, null)]
		public abstract Color Color { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00008201 File Offset: 0x00006401
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00008209 File Offset: 0x00006409
		[DataField("lightRequiresPower", false, 1, false, false, null)]
		public virtual bool LightRequiresPower { get; set; } = true;

		// Token: 0x0600014F RID: 335 RVA: 0x00008214 File Offset: 0x00006414
		public virtual StatusLightData? GetStatusLightData(Wire wire)
		{
			if (this.LightRequiresPower && !this.IsPowered(wire.Owner))
			{
				return new StatusLightData?(new StatusLightData(this.Color, StatusLightState.Off, Loc.GetString(this.Name)));
			}
			StatusLightState? state = this.GetLightState(wire);
			if (state != null)
			{
				return new StatusLightData?(new StatusLightData(this.Color, state.Value, Loc.GetString(this.Name)));
			}
			return null;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00008294 File Offset: 0x00006494
		public virtual StatusLightState? GetLightState(Wire wire)
		{
			return null;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000151 RID: 337
		[Nullable(2)]
		public abstract object StatusKey { [NullableContext(2)] get; }

		// Token: 0x06000152 RID: 338 RVA: 0x000082AA File Offset: 0x000064AA
		public virtual void Initialize()
		{
			this.EntityManager = IoCManager.Resolve<IEntityManager>();
			this._adminLogger = IoCManager.Resolve<ISharedAdminLogManager>();
			this.WiresSystem = this.EntityManager.EntitySysManager.GetEntitySystem<WiresSystem>();
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000082D8 File Offset: 0x000064D8
		public virtual bool AddWire(Wire wire, int count)
		{
			return count == 1;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000082DE File Offset: 0x000064DE
		public virtual bool Cut(EntityUid user, Wire wire)
		{
			return this.Log(user, wire, "cut");
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000082ED File Offset: 0x000064ED
		public virtual bool Mend(EntityUid user, Wire wire)
		{
			return this.Log(user, wire, "mended");
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000082FC File Offset: 0x000064FC
		public virtual void Pulse(EntityUid user, Wire wire)
		{
			this.Log(user, wire, "pulsed");
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000830C File Offset: 0x0000650C
		private bool Log(EntityUid user, Wire wire, string verb)
		{
			EntityStringRepresentation player = this.EntityManager.ToPrettyString(user);
			EntityStringRepresentation owner = this.EntityManager.ToPrettyString(wire.Owner);
			string name = Loc.GetString(this.Name);
			string color = wire.Color.Name();
			string action = base.GetType().Name;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.WireHacking;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(15, 6);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(player, "player");
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted(verb);
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted(color);
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted(name);
			logStringHandler.AppendLiteral(" wire (");
			logStringHandler.AppendFormatted(action);
			logStringHandler.AppendLiteral(") in ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(owner, "owner");
			adminLogger.Add(type, impact, ref logStringHandler);
			return true;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x000083EF File Offset: 0x000065EF
		public virtual void Update(Wire wire)
		{
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000083F1 File Offset: 0x000065F1
		protected bool IsPowered(EntityUid uid)
		{
			return this.WiresSystem.IsPowered(uid, this.EntityManager, null);
		}

		// Token: 0x04000114 RID: 276
		private ISharedAdminLogManager _adminLogger;

		// Token: 0x04000116 RID: 278
		public IEntityManager EntityManager;

		// Token: 0x04000117 RID: 279
		public WiresSystem WiresSystem;
	}
}
