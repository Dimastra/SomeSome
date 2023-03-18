using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Kitchen.Components;
using Content.Server.Popups;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Access.Systems
{
	// Token: 0x0200087C RID: 2172
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IdCardSystem : SharedIdCardSystem
	{
		// Token: 0x06002F70 RID: 12144 RVA: 0x000F57AF File Offset: 0x000F39AF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<IdCardComponent, MapInitEvent>(new ComponentEventHandler<IdCardComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<IdCardComponent, BeingMicrowavedEvent>(new ComponentEventHandler<IdCardComponent, BeingMicrowavedEvent>(this.OnMicrowaved), null, null);
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x000F57DF File Offset: 0x000F39DF
		private void OnMapInit(EntityUid uid, IdCardComponent id, MapInitEvent args)
		{
			this.UpdateEntityName(uid, id);
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x000F57EC File Offset: 0x000F39EC
		private void OnMicrowaved(EntityUid uid, IdCardComponent component, BeingMicrowavedEvent args)
		{
			AccessComponent access;
			if (base.TryComp<AccessComponent>(uid, ref access))
			{
				float randomPick = this._random.NextFloat();
				LogStringHandler logStringHandler;
				if (randomPick <= 0.15f)
				{
					TransformComponent transformComponent;
					base.TryComp<TransformComponent>(uid, ref transformComponent);
					if (transformComponent != null)
					{
						this._popupSystem.PopupCoordinates(Loc.GetString("id-card-component-microwave-burnt", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("id", uid)
						}), transformComponent.Coordinates, PopupType.Medium);
						this.EntityManager.SpawnEntity("FoodBadRecipe", transformComponent.Coordinates);
					}
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Medium;
					logStringHandler = new LogStringHandler(7, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Microwave), "ToPrettyString(args.Microwave)");
					logStringHandler.AppendLiteral(" burnt ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
					adminLogger.Add(type, impact, ref logStringHandler);
					this.EntityManager.QueueDeleteEntity(uid);
					return;
				}
				if (randomPick <= 0.25f)
				{
					this._popupSystem.PopupEntity(Loc.GetString("id-card-component-microwave-bricked", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("id", uid)
					}), uid, PopupType.Small);
					access.Tags.Clear();
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Action;
					LogImpact impact2 = LogImpact.Medium;
					logStringHandler = new LogStringHandler(19, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Microwave), "ToPrettyString(args.Microwave)");
					logStringHandler.AppendLiteral(" cleared access on ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
					adminLogger2.Add(type2, impact2, ref logStringHandler);
				}
				else
				{
					this._popupSystem.PopupEntity(Loc.GetString("id-card-component-microwave-safe", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("id", uid)
					}), uid, PopupType.Medium);
				}
				AccessLevelPrototype random = RandomExtensions.Pick<AccessLevelPrototype>(this._random, this._prototypeManager.EnumeratePrototypes<AccessLevelPrototype>().ToArray<AccessLevelPrototype>());
				access.Tags.Add(random.ID);
				ISharedAdminLogManager adminLogger3 = this._adminLogger;
				LogType type3 = LogType.Action;
				LogImpact impact3 = LogImpact.Medium;
				logStringHandler = new LogStringHandler(18, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Microwave), "ToPrettyString(args.Microwave)");
				logStringHandler.AppendLiteral(" added ");
				logStringHandler.AppendFormatted(random.ID);
				logStringHandler.AppendLiteral(" access to ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				adminLogger3.Add(type3, impact3, ref logStringHandler);
			}
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x000F5A50 File Offset: 0x000F3C50
		[NullableContext(2)]
		public bool TryChangeJobTitle(EntityUid uid, string jobTitle, IdCardComponent id = null, EntityUid? player = null)
		{
			if (!base.Resolve<IdCardComponent>(uid, ref id, true))
			{
				return false;
			}
			if (!string.IsNullOrWhiteSpace(jobTitle))
			{
				jobTitle = jobTitle.Trim();
				if (jobTitle.Length > 30)
				{
					jobTitle = jobTitle.Substring(0, 30);
				}
			}
			else
			{
				jobTitle = null;
			}
			if (id.JobTitle == jobTitle)
			{
				return true;
			}
			id.JobTitle = jobTitle;
			base.Dirty(id, null);
			this.UpdateEntityName(uid, id);
			if (player != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Identity;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(35, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player.Value), "player", "ToPrettyString(player.Value)");
				logStringHandler.AppendLiteral(" has changed the job title of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(id.Owner), "entity", "ToPrettyString(id.Owner)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted(jobTitle);
				logStringHandler.AppendLiteral(" ");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			return true;
		}

		// Token: 0x06002F74 RID: 12148 RVA: 0x000F5B4C File Offset: 0x000F3D4C
		[NullableContext(2)]
		public bool TryChangeFullName(EntityUid uid, string fullName, IdCardComponent id = null, EntityUid? player = null)
		{
			if (!base.Resolve<IdCardComponent>(uid, ref id, true))
			{
				return false;
			}
			if (!string.IsNullOrWhiteSpace(fullName))
			{
				fullName = fullName.Trim();
				if (fullName.Length > 30)
				{
					fullName = fullName.Substring(0, 30);
				}
			}
			else
			{
				fullName = null;
			}
			if (id.FullName == fullName)
			{
				return true;
			}
			id.FullName = fullName;
			base.Dirty(id, null);
			this.UpdateEntityName(uid, id);
			if (player != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Identity;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(30, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player.Value), "player", "ToPrettyString(player.Value)");
				logStringHandler.AppendLiteral(" has changed the name of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(id.Owner), "entity", "ToPrettyString(id.Owner)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted(fullName);
				logStringHandler.AppendLiteral(" ");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			return true;
		}

		// Token: 0x06002F75 RID: 12149 RVA: 0x000F5C48 File Offset: 0x000F3E48
		[NullableContext(2)]
		private void UpdateEntityName(EntityUid uid, IdCardComponent id = null)
		{
			if (!base.Resolve<IdCardComponent>(uid, ref id, true))
			{
				return;
			}
			string jobSuffix = string.IsNullOrWhiteSpace(id.JobTitle) ? string.Empty : (" (" + id.JobTitle + ")");
			string val = string.IsNullOrWhiteSpace(id.FullName) ? Loc.GetString("access-id-card-component-owner-name-job-title-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("jobSuffix", jobSuffix)
			}) : Loc.GetString("access-id-card-component-owner-full-name-job-title-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("fullName", id.FullName),
				new ValueTuple<string, object>("jobSuffix", jobSuffix)
			});
			this.EntityManager.GetComponent<MetaDataComponent>(id.Owner).EntityName = val;
		}

		// Token: 0x04001C8A RID: 7306
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001C8B RID: 7307
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001C8C RID: 7308
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001C8D RID: 7309
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
