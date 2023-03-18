using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Lathe
{
	// Token: 0x02000380 RID: 896
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedLatheSystem : EntitySystem
	{
		// Token: 0x06000A68 RID: 2664 RVA: 0x00022560 File Offset: 0x00020760
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LatheComponent, ComponentGetState>(new ComponentEventRefHandler<LatheComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<LatheComponent, ComponentHandleState>(new ComponentEventRefHandler<LatheComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00022590 File Offset: 0x00020790
		private void OnGetState(EntityUid uid, LatheComponent component, ref ComponentGetState args)
		{
			args.State = new LatheComponentState(component.MaterialUseMultiplier);
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x000225A4 File Offset: 0x000207A4
		private void OnHandleState(EntityUid uid, LatheComponent component, ref ComponentHandleState args)
		{
			LatheComponentState state = args.Current as LatheComponentState;
			if (state == null)
			{
				return;
			}
			component.MaterialUseMultiplier = state.MaterialUseMultiplier;
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x000225D0 File Offset: 0x000207D0
		public bool CanProduce(EntityUid uid, string recipe, int amount = 1, [Nullable(2)] LatheComponent component = null)
		{
			LatheRecipePrototype proto;
			return this._proto.TryIndex<LatheRecipePrototype>(recipe, ref proto) && this.CanProduce(uid, proto, amount, component);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x000225FC File Offset: 0x000207FC
		public bool CanProduce(EntityUid uid, LatheRecipePrototype recipe, int amount = 1, [Nullable(2)] LatheComponent component = null)
		{
			if (!base.Resolve<LatheComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!this.HasRecipe(uid, recipe, component))
			{
				return false;
			}
			foreach (KeyValuePair<string, int> keyValuePair in recipe.RequiredMaterials)
			{
				string text;
				int original;
				keyValuePair.Deconstruct(out text, out original);
				string material = text;
				int adjustedAmount = SharedLatheSystem.AdjustMaterial(original, recipe.ApplyMaterialDiscount, component.MaterialUseMultiplier);
				if (this._materialStorage.GetMaterialAmount(component.Owner, material, null) < adjustedAmount * amount)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x000226AC File Offset: 0x000208AC
		public static int AdjustMaterial(int original, bool reduce, float multiplier)
		{
			if (!reduce)
			{
				return original;
			}
			return (int)MathF.Ceiling((float)original * multiplier);
		}

		// Token: 0x06000A6E RID: 2670
		protected abstract bool HasRecipe(EntityUid uid, LatheRecipePrototype recipe, LatheComponent component);

		// Token: 0x04000A58 RID: 2648
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000A59 RID: 2649
		[Dependency]
		private readonly SharedMaterialStorageSystem _materialStorage;
	}
}
