using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Construction.Components;
using Content.Server.Power.Components;
using Content.Server.Stack;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Materials;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Materials
{
	// Token: 0x020003D1 RID: 977
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MaterialStorageSystem : SharedMaterialStorageSystem
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x00068192 File Offset: 0x00066392
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MaterialStorageComponent, MachineDeconstructedEvent>(new ComponentEventHandler<MaterialStorageComponent, MachineDeconstructedEvent>(this.OnDeconstructed), null, null);
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x000681B0 File Offset: 0x000663B0
		private void OnDeconstructed(EntityUid uid, MaterialStorageComponent component, MachineDeconstructedEvent args)
		{
			if (!component.DropOnDeconstruct)
			{
				return;
			}
			foreach (KeyValuePair<string, int> keyValuePair in component.Storage)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string material = text;
				int amount = num;
				this.SpawnMultipleFromMaterial(amount, material, base.Transform(uid).Coordinates);
			}
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0006822C File Offset: 0x0006642C
		[NullableContext(2)]
		public override bool TryInsertMaterialEntity(EntityUid user, EntityUid toInsert, EntityUid receiver, MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(receiver, ref component, true))
			{
				return false;
			}
			ApcPowerReceiverComponent power;
			if (base.TryComp<ApcPowerReceiverComponent>(receiver, ref power) && !power.Powered)
			{
				return false;
			}
			if (!base.TryInsertMaterialEntity(user, toInsert, receiver, component))
			{
				return false;
			}
			this._audio.PlayPvs(component.InsertingSound, component.Owner, null);
			this._popup.PopupEntity(Loc.GetString("machine-insert-item", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", user),
				new ValueTuple<string, object>("machine", component.Owner),
				new ValueTuple<string, object>("item", toInsert)
			}), component.Owner, PopupType.Small);
			base.QueueDel(toInsert);
			StackComponent stack;
			base.TryComp<StackComponent>(toInsert, ref stack);
			int count = (stack != null) ? stack.Count : 1;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" inserted ");
			logStringHandler.AppendFormatted<int>(count, "count");
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(toInsert), "inserted", "ToPrettyString(toInsert)");
			logStringHandler.AppendLiteral(" into ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(receiver), "receiver", "ToPrettyString(receiver)");
			adminLogger.Add(type, impact, ref logStringHandler);
			return true;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x000683B0 File Offset: 0x000665B0
		public List<EntityUid> SpawnMultipleFromMaterial(int amount, string material, EntityCoordinates coordinates)
		{
			MaterialPrototype stackType;
			if (!this._prototypeManager.TryIndex<MaterialPrototype>(material, ref stackType))
			{
				Logger.Error("Failed to index material prototype " + material);
				return new List<EntityUid>();
			}
			return this.SpawnMultipleFromMaterial(amount, stackType, coordinates);
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x000683EC File Offset: 0x000665EC
		public List<EntityUid> SpawnMultipleFromMaterial(int amount, MaterialPrototype materialProto, EntityCoordinates coordinates)
		{
			if (amount <= 0)
			{
				return new List<EntityUid>();
			}
			MaterialComponent material;
			if (!this._prototypeManager.Index<EntityPrototype>(materialProto.StackEntity).TryGetComponent<MaterialComponent>(ref material, null))
			{
				return new List<EntityUid>();
			}
			int materialPerStack = material.Materials[materialProto.ID];
			int amountToSpawn = amount / materialPerStack;
			return this._stackSystem.SpawnMultiple(materialProto.StackEntity, amountToSpawn, coordinates);
		}

		// Token: 0x04000C68 RID: 3176
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000C69 RID: 3177
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000C6A RID: 3178
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000C6B RID: 3179
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000C6C RID: 3180
		[Dependency]
		private readonly StackSystem _stackSystem;
	}
}
