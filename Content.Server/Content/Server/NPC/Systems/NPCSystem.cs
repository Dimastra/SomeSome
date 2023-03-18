using System;
using System.Runtime.CompilerServices;
using Content.Server.NPC.Components;
using Content.Server.NPC.HTN;
using Content.Shared.CCVar;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.NPC.Systems
{
	// Token: 0x02000336 RID: 822
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCSystem : EntitySystem
	{
		// Token: 0x0600111E RID: 4382 RVA: 0x00058FE7 File Offset: 0x000571E7
		public void SetBlackboard(EntityUid uid, string key, object value, [Nullable(2)] NPCComponent component = null)
		{
			if (!base.Resolve<NPCComponent>(uid, ref component, false))
			{
				return;
			}
			component.Blackboard.SetValue(key, value);
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x0600111F RID: 4383 RVA: 0x00059004 File Offset: 0x00057204
		// (set) Token: 0x06001120 RID: 4384 RVA: 0x0005900C File Offset: 0x0005720C
		public bool Enabled { get; set; } = true;

		// Token: 0x06001121 RID: 4385 RVA: 0x00059018 File Offset: 0x00057218
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("npc");
			this._sawmill.Level = new LogLevel?(2);
			base.SubscribeLocalEvent<NPCComponent, MobStateChangedEvent>(new ComponentEventHandler<NPCComponent, MobStateChangedEvent>(this.OnMobStateChange), null, null);
			base.SubscribeLocalEvent<NPCComponent, MapInitEvent>(new ComponentEventHandler<NPCComponent, MapInitEvent>(this.OnNPCMapInit), null, null);
			base.SubscribeLocalEvent<NPCComponent, ComponentShutdown>(new ComponentEventHandler<NPCComponent, ComponentShutdown>(this.OnNPCShutdown), null, null);
			base.SubscribeLocalEvent<NPCComponent, PlayerAttachedEvent>(new ComponentEventHandler<NPCComponent, PlayerAttachedEvent>(this.OnPlayerNPCAttach), null, null);
			base.SubscribeLocalEvent<NPCComponent, PlayerDetachedEvent>(new ComponentEventHandler<NPCComponent, PlayerDetachedEvent>(this.OnPlayerNPCDetach), null, null);
			this._configurationManager.OnValueChanged<bool>(CCVars.NPCEnabled, new Action<bool>(this.SetEnabled), true);
			this._configurationManager.OnValueChanged<int>(CCVars.NPCMaxUpdates, new Action<int>(this.SetMaxUpdates), true);
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x000590EA File Offset: 0x000572EA
		private void OnPlayerNPCAttach(EntityUid uid, NPCComponent component, PlayerAttachedEvent args)
		{
			this.SleepNPC(uid, component);
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x000590F4 File Offset: 0x000572F4
		private void OnPlayerNPCDetach(EntityUid uid, NPCComponent component, PlayerDetachedEvent args)
		{
			if (this._mobState.IsIncapacitated(uid, null))
			{
				return;
			}
			this.WakeNPC(uid, component);
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0005910E File Offset: 0x0005730E
		private void SetMaxUpdates(int obj)
		{
			this._maxUpdates = obj;
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00059117 File Offset: 0x00057317
		private void SetEnabled(bool value)
		{
			this.Enabled = value;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x00059120 File Offset: 0x00057320
		public override void Shutdown()
		{
			base.Shutdown();
			this._configurationManager.UnsubValueChanged<bool>(CCVars.NPCEnabled, new Action<bool>(this.SetEnabled));
			this._configurationManager.UnsubValueChanged<int>(CCVars.NPCMaxUpdates, new Action<int>(this.SetMaxUpdates));
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00059160 File Offset: 0x00057360
		private void OnNPCMapInit(EntityUid uid, NPCComponent component, MapInitEvent args)
		{
			component.Blackboard.SetValue("Owner", uid);
			this.WakeNPC(uid, component);
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00059180 File Offset: 0x00057380
		private void OnNPCShutdown(EntityUid uid, NPCComponent component, ComponentShutdown args)
		{
			this.SleepNPC(uid, component);
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x0005918A File Offset: 0x0005738A
		public bool IsAwake(NPCComponent component, [Nullable(2)] ActiveNPCComponent active = null)
		{
			return base.Resolve<ActiveNPCComponent>(component.Owner, ref active, false);
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x0005919C File Offset: 0x0005739C
		[NullableContext(2)]
		public void WakeNPC(EntityUid uid, NPCComponent component = null)
		{
			if (!base.Resolve<NPCComponent>(uid, ref component, false))
			{
				return;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Waking ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			base.EnsureComp<ActiveNPCComponent>(component.Owner);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00059200 File Offset: 0x00057400
		[NullableContext(2)]
		public void SleepNPC(EntityUid uid, NPCComponent component = null)
		{
			if (!base.Resolve<NPCComponent>(uid, ref component, false))
			{
				return;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Sleeping ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			base.RemComp<ActiveNPCComponent>(component.Owner);
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00059262 File Offset: 0x00057462
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this.Enabled)
			{
				return;
			}
			this._count = 0;
			this._htn.UpdateNPC(ref this._count, this._maxUpdates, frameTime);
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00059294 File Offset: 0x00057494
		private void OnMobStateChange(EntityUid uid, NPCComponent component, MobStateChangedEvent args)
		{
			if (base.HasComp<ActorComponent>(uid))
			{
				return;
			}
			MobState newMobState = args.NewMobState;
			if (newMobState == MobState.Alive)
			{
				this.WakeNPC(uid, component);
				return;
			}
			if (newMobState - MobState.Critical > 1)
			{
				return;
			}
			this.SleepNPC(uid, component);
		}

		// Token: 0x04000A30 RID: 2608
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000A31 RID: 2609
		[Dependency]
		private readonly HTNSystem _htn;

		// Token: 0x04000A32 RID: 2610
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000A33 RID: 2611
		private ISawmill _sawmill;

		// Token: 0x04000A35 RID: 2613
		private int _maxUpdates;

		// Token: 0x04000A36 RID: 2614
		private int _count;
	}
}
