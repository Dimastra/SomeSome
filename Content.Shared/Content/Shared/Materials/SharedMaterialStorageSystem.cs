using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Materials
{
	// Token: 0x02000337 RID: 823
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMaterialStorageSystem : EntitySystem
	{
		// Token: 0x06000980 RID: 2432 RVA: 0x0001FA34 File Offset: 0x0001DC34
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MaterialStorageComponent, MapInitEvent>(new ComponentEventHandler<MaterialStorageComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<MaterialStorageComponent, InteractUsingEvent>(new ComponentEventHandler<MaterialStorageComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<MaterialStorageComponent, ComponentGetState>(new ComponentEventRefHandler<MaterialStorageComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<MaterialStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<MaterialStorageComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<InsertingMaterialStorageComponent, ComponentGetState>(new ComponentEventRefHandler<InsertingMaterialStorageComponent, ComponentGetState>(this.OnGetInsertingState), null, null);
			base.SubscribeLocalEvent<InsertingMaterialStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<InsertingMaterialStorageComponent, ComponentHandleState>(this.OnHandleInsertingState), null, null);
			base.SubscribeLocalEvent<InsertingMaterialStorageComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<InsertingMaterialStorageComponent, EntityUnpausedEvent>(this.OnUnpaused), null, null);
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0001FAD4 File Offset: 0x0001DCD4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (InsertingMaterialStorageComponent inserting in base.EntityQuery<InsertingMaterialStorageComponent>(false))
			{
				if (!(this._timing.CurTime < inserting.EndTime))
				{
					this._appearance.SetData(inserting.Owner, MaterialStorageVisuals.Inserting, false, null);
					base.RemComp(inserting.Owner, inserting);
				}
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0001FB68 File Offset: 0x0001DD68
		private void OnMapInit(EntityUid uid, MaterialStorageComponent component, MapInitEvent args)
		{
			this._appearance.SetData(uid, MaterialStorageVisuals.Inserting, false, null);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0001FB83 File Offset: 0x0001DD83
		private void OnGetState(EntityUid uid, MaterialStorageComponent component, ref ComponentGetState args)
		{
			args.State = new MaterialStorageComponentState(component.Storage, component.MaterialWhiteList);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0001FB9C File Offset: 0x0001DD9C
		private void OnHandleState(EntityUid uid, MaterialStorageComponent component, ref ComponentHandleState args)
		{
			MaterialStorageComponentState state = args.Current as MaterialStorageComponentState;
			if (state == null)
			{
				return;
			}
			component.Storage = new Dictionary<string, int>(state.Storage);
			if (state.MaterialWhitelist != null)
			{
				component.MaterialWhiteList = new List<string>(state.MaterialWhitelist);
			}
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0001FBE3 File Offset: 0x0001DDE3
		private void OnGetInsertingState(EntityUid uid, InsertingMaterialStorageComponent component, ref ComponentGetState args)
		{
			args.State = new InsertingMaterialStorageComponentState(component.EndTime, component.MaterialColor);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0001FBFC File Offset: 0x0001DDFC
		private void OnHandleInsertingState(EntityUid uid, InsertingMaterialStorageComponent component, ref ComponentHandleState args)
		{
			InsertingMaterialStorageComponentState state = args.Current as InsertingMaterialStorageComponentState;
			if (state == null)
			{
				return;
			}
			component.EndTime = state.EndTime;
			component.MaterialColor = state.MaterialColor;
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0001FC31 File Offset: 0x0001DE31
		private void OnUnpaused(EntityUid uid, InsertingMaterialStorageComponent component, ref EntityUnpausedEvent args)
		{
			component.EndTime += args.PausedTime;
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0001FC4A File Offset: 0x0001DE4A
		public int GetMaterialAmount(EntityUid uid, MaterialPrototype material, [Nullable(2)] MaterialStorageComponent component = null)
		{
			return this.GetMaterialAmount(uid, material.ID, component);
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0001FC5C File Offset: 0x0001DE5C
		public int GetMaterialAmount(EntityUid uid, string material, [Nullable(2)] MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(uid, ref component, true))
			{
				return 0;
			}
			int amount;
			if (component.Storage.TryGetValue(material, out amount))
			{
				return amount;
			}
			return 0;
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0001FC8A File Offset: 0x0001DE8A
		[NullableContext(2)]
		public int GetTotalMaterialAmount(EntityUid uid, MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(uid, ref component, true))
			{
				return 0;
			}
			return component.Storage.Values.Sum();
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0001FCAC File Offset: 0x0001DEAC
		[NullableContext(2)]
		public bool CanTakeVolume(EntityUid uid, int volume, MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.StorageLimit != null)
			{
				int num = this.GetTotalMaterialAmount(uid, component) + volume;
				int? storageLimit = component.StorageLimit;
				return num <= storageLimit.GetValueOrDefault() & storageLimit != null;
			}
			return true;
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0001FCFC File Offset: 0x0001DEFC
		public bool CanChangeMaterialAmount(EntityUid uid, string materialId, int volume, [Nullable(2)] MaterialStorageComponent component = null)
		{
			int amount;
			return base.Resolve<MaterialStorageComponent>(uid, ref component, true) && (this.CanTakeVolume(uid, volume, component) && (component.MaterialWhiteList == null || component.MaterialWhiteList.Contains(materialId))) && (!component.Storage.TryGetValue(materialId, out amount) || amount + volume >= 0);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0001FD5C File Offset: 0x0001DF5C
		public bool TryChangeMaterialAmount(EntityUid uid, string materialId, int volume, [Nullable(2)] MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!this.CanChangeMaterialAmount(uid, materialId, volume, component))
			{
				return false;
			}
			if (!component.Storage.ContainsKey(materialId))
			{
				component.Storage.Add(materialId, 0);
			}
			Dictionary<string, int> storage = component.Storage;
			storage[materialId] += volume;
			MaterialAmountChangedEvent ev = default(MaterialAmountChangedEvent);
			base.RaiseLocalEvent<MaterialAmountChangedEvent>(uid, ref ev, false);
			base.Dirty(component, null);
			return true;
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0001FDDC File Offset: 0x0001DFDC
		[NullableContext(2)]
		public virtual bool TryInsertMaterialEntity(EntityUid user, EntityUid toInsert, EntityUid receiver, MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(receiver, ref component, true))
			{
				return false;
			}
			MaterialComponent material;
			if (!base.TryComp<MaterialComponent>(toInsert, ref material))
			{
				return false;
			}
			EntityWhitelist entityWhitelist = component.EntityWhitelist;
			if (entityWhitelist != null && !entityWhitelist.IsValid(toInsert, null))
			{
				return false;
			}
			StackComponent stackComponent;
			int multiplier = base.TryComp<StackComponent>(toInsert, ref stackComponent) ? stackComponent.Count : 1;
			int totalVolume = 0;
			foreach (KeyValuePair<string, int> keyValuePair in material.Materials)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string mat = text;
				int vol = num;
				if (!this.CanChangeMaterialAmount(receiver, mat, vol * multiplier, component))
				{
					return false;
				}
				totalVolume += vol * multiplier;
			}
			if (!this.CanTakeVolume(receiver, totalVolume, component))
			{
				return false;
			}
			foreach (KeyValuePair<string, int> keyValuePair in material.Materials)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string mat2 = text;
				int vol2 = num;
				this.TryChangeMaterialAmount(receiver, mat2, vol2 * multiplier, component);
			}
			InsertingMaterialStorageComponent insertingComp = base.EnsureComp<InsertingMaterialStorageComponent>(receiver);
			insertingComp.EndTime = this._timing.CurTime + component.InsertionTime;
			if (!component.IgnoreColor)
			{
				MaterialPrototype lastMat;
				this._prototype.TryIndex<MaterialPrototype>(material.Materials.Keys.Last<string>(), ref lastMat);
				insertingComp.MaterialColor = ((lastMat != null) ? new Color?(lastMat.Color) : null);
			}
			this._appearance.SetData(receiver, MaterialStorageVisuals.Inserting, true, null);
			base.Dirty(insertingComp, null);
			MaterialEntityInsertedEvent ev = new MaterialEntityInsertedEvent(material);
			base.RaiseLocalEvent<MaterialEntityInsertedEvent>(component.Owner, ref ev, false);
			return true;
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0001FFC8 File Offset: 0x0001E1C8
		[NullableContext(2)]
		public void UpdateMaterialWhitelist(EntityUid uid, MaterialStorageComponent component = null)
		{
			if (!base.Resolve<MaterialStorageComponent>(uid, ref component, false))
			{
				return;
			}
			GetMaterialWhitelistEvent ev = new GetMaterialWhitelistEvent(uid);
			base.RaiseLocalEvent<GetMaterialWhitelistEvent>(uid, ref ev, false);
			component.MaterialWhiteList = ev.Whitelist;
			base.Dirty(component, null);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00020008 File Offset: 0x0001E208
		private void OnInteractUsing(EntityUid uid, MaterialStorageComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this.TryInsertMaterialEntity(args.User, args.Used, uid, component);
		}

		// Token: 0x0400095F RID: 2399
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000960 RID: 2400
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000961 RID: 2401
		[Dependency]
		private readonly IPrototypeManager _prototype;
	}
}
