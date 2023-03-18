using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Damage.Components;
using Content.Server.Tools.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C2 RID: 1474
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageOnToolInteractSystem : EntitySystem
	{
		// Token: 0x06001F7A RID: 8058 RVA: 0x000A5386 File Offset: 0x000A3586
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageOnToolInteractComponent, InteractUsingEvent>(new ComponentEventHandler<DamageOnToolInteractComponent, InteractUsingEvent>(this.OnInteracted), null, null);
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x000A53A4 File Offset: 0x000A35A4
		private void OnInteracted(EntityUid uid, DamageOnToolInteractComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			DamageSpecifier weldingDamage = component.WeldingDamage;
			WelderComponent welder;
			if (weldingDamage != null && this.EntityManager.TryGetComponent<WelderComponent>(args.Used, ref welder) && welder.Lit && !welder.TankSafe)
			{
				DamageSpecifier dmg = this._damageableSystem.TryChangeDamage(new EntityUid?(args.Target), weldingDamage, false, true, null, new EntityUid?(args.User));
				if (dmg != null)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Damaged;
					LogStringHandler logStringHandler = new LogStringHandler(38, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" used ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "used", "ToPrettyString(args.Used)");
					logStringHandler.AppendLiteral(" as a welder to deal ");
					logStringHandler.AppendFormatted<FixedPoint2>(dmg.Total, "damage", "dmg.Total");
					logStringHandler.AppendLiteral(" damage to ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
					adminLogger.Add(type, ref logStringHandler);
				}
				args.Handled = true;
				return;
			}
			DamageSpecifier damage = component.DefaultDamage;
			ToolComponent tool;
			if (damage != null && this.EntityManager.TryGetComponent<ToolComponent>(args.Used, ref tool) && tool.Qualities.ContainsAny(component.Tools))
			{
				DamageSpecifier dmg2 = this._damageableSystem.TryChangeDamage(new EntityUid?(args.Target), damage, false, true, null, new EntityUid?(args.User));
				if (dmg2 != null)
				{
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Damaged;
					LogStringHandler logStringHandler = new LogStringHandler(36, 4);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
					logStringHandler.AppendLiteral(" used ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Used), "used", "ToPrettyString(args.Used)");
					logStringHandler.AppendLiteral(" as a tool to deal ");
					logStringHandler.AppendFormatted<FixedPoint2>(dmg2.Total, "damage", "dmg.Total");
					logStringHandler.AppendLiteral(" damage to ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
					adminLogger2.Add(type2, ref logStringHandler);
				}
				args.Handled = true;
			}
		}

		// Token: 0x0400138E RID: 5006
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x0400138F RID: 5007
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
