using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Server.Teleportation
{
	// Token: 0x02000129 RID: 297
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandTeleporterSystem : EntitySystem
	{
		// Token: 0x0600055D RID: 1373 RVA: 0x0001A3E8 File Offset: 0x000185E8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<HandTeleporterComponent, UseInHandEvent>(new ComponentEventHandler<HandTeleporterComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<HandTeleporterComponent, DoAfterEvent>(new ComponentEventHandler<HandTeleporterComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x0001A412 File Offset: 0x00018612
		private void OnDoAfter(EntityUid uid, HandTeleporterComponent component, DoAfterEvent args)
		{
			if (args.Cancelled || args.Handled)
			{
				return;
			}
			this.HandlePortalUpdating(uid, component, args.Args.User);
			args.Handled = true;
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001A440 File Offset: 0x00018640
		private void OnUseInHand(EntityUid uid, HandTeleporterComponent component, UseInHandEvent args)
		{
			if (base.Deleted(component.FirstPortal))
			{
				component.FirstPortal = null;
			}
			if (base.Deleted(component.SecondPortal))
			{
				component.SecondPortal = null;
			}
			if (component.FirstPortal != null && component.SecondPortal != null)
			{
				this.HandlePortalUpdating(uid, component, args.User);
				return;
			}
			TransformComponent transformComponent = base.Transform(args.User);
			EntityUid parentUid = transformComponent.ParentUid;
			EntityUid? gridUid = transformComponent.GridUid;
			if (parentUid != gridUid)
			{
				return;
			}
			EntityUid user = args.User;
			float portalCreationDelay = component.PortalCreationDelay;
			gridUid = new EntityUid?(uid);
			DoAfterEventArgs doafterArgs = new DoAfterEventArgs(user, portalCreationDelay, default(CancellationToken), null, gridUid)
			{
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnUserMove = true,
				MovementThreshold = 0.5f
			};
			this._doafter.DoAfter(doafterArgs);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001A53C File Offset: 0x0001873C
		private void HandlePortalUpdating(EntityUid uid, HandTeleporterComponent component, EntityUid user)
		{
			if (base.Deleted(user, null))
			{
				return;
			}
			TransformComponent xform = base.Transform(user);
			if (component.FirstPortal == null && component.SecondPortal == null)
			{
				if (xform.ParentUid != xform.GridUid)
				{
					return;
				}
				base.EnsureComp<PortalTimeoutComponent>(user).EnteredPortal = null;
				component.FirstPortal = new EntityUid?(base.Spawn(component.FirstPortalPrototype, base.Transform(user).Coordinates));
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.EntitySpawn;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(19, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" opened ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.FirstPortal.Value), "ToPrettyString(component.FirstPortal.Value)");
				logStringHandler.AppendLiteral(" at ");
				logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(component.FirstPortal.Value).Coordinates, "Transform(component.FirstPortal.Value).Coordinates");
				logStringHandler.AppendLiteral(" using ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
				this._audio.PlayPvs(component.NewPortalSound, uid, null);
				return;
			}
			else
			{
				if (component.SecondPortal == null)
				{
					base.EnsureComp<PortalTimeoutComponent>(user).EnteredPortal = null;
					component.SecondPortal = new EntityUid?(base.Spawn(component.SecondPortalPrototype, base.Transform(user).Coordinates));
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.EntitySpawn;
					LogImpact impact2 = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(30, 5);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" opened ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.SecondPortal.Value), "ToPrettyString(component.SecondPortal.Value)");
					logStringHandler.AppendLiteral(" at ");
					logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(component.SecondPortal.Value).Coordinates, "Transform(component.SecondPortal.Value).Coordinates");
					logStringHandler.AppendLiteral(" linked to ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.FirstPortal.Value), "ToPrettyString(component.FirstPortal!.Value)");
					logStringHandler.AppendLiteral(" using ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
					adminLogger2.Add(type2, impact2, ref logStringHandler);
					this._link.TryLink(component.FirstPortal.Value, component.SecondPortal.Value, true);
					this._audio.PlayPvs(component.NewPortalSound, uid, null);
					return;
				}
				string portalStrings = "";
				portalStrings += base.ToPrettyString(component.FirstPortal.Value);
				if (portalStrings != "")
				{
					portalStrings += " and ";
				}
				portalStrings += base.ToPrettyString(component.SecondPortal.Value);
				if (portalStrings != "")
				{
					ISharedAdminLogManager adminLogger3 = this._adminLogger;
					LogType type3 = LogType.EntityDelete;
					LogImpact impact3 = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(14, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
					logStringHandler.AppendLiteral(" closed ");
					logStringHandler.AppendFormatted(portalStrings);
					logStringHandler.AppendLiteral(" with ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
					adminLogger3.Add(type3, impact3, ref logStringHandler);
				}
				base.QueueDel(component.FirstPortal.Value);
				base.QueueDel(component.SecondPortal.Value);
				component.FirstPortal = null;
				component.SecondPortal = null;
				this._audio.PlayPvs(component.ClearPortalsSound, uid, null);
				return;
			}
		}

		// Token: 0x04000343 RID: 835
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000344 RID: 836
		[Dependency]
		private readonly LinkedEntitySystem _link;

		// Token: 0x04000345 RID: 837
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04000346 RID: 838
		[Dependency]
		private readonly DoAfterSystem _doafter;
	}
}
