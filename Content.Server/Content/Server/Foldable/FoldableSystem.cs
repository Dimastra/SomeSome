using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Buckle.Systems;
using Content.Server.Storage.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Foldable;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Foldable
{
	// Token: 0x020004E9 RID: 1257
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FoldableSystem : SharedFoldableSystem
	{
		// Token: 0x060019DC RID: 6620 RVA: 0x000879F8 File Offset: 0x00085BF8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FoldableComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<FoldableComponent, StorageOpenAttemptEvent>(this.OnFoldableOpenAttempt), null, null);
			base.SubscribeLocalEvent<FoldableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<FoldableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddFoldVerb), null, null);
			base.SubscribeLocalEvent<FoldableComponent, StoreMobInItemContainerAttemptEvent>(new ComponentEventRefHandler<FoldableComponent, StoreMobInItemContainerAttemptEvent>(this.OnStoreThisAttempt), null, null);
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x00087A47 File Offset: 0x00085C47
		private void OnFoldableOpenAttempt(EntityUid uid, FoldableComponent component, ref StorageOpenAttemptEvent args)
		{
			if (component.IsFolded)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00087A58 File Offset: 0x00085C58
		public bool TryToggleFold(EntityUid uid, FoldableComponent comp)
		{
			return this.TrySetFolded(uid, comp, !comp.IsFolded);
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00087A6C File Offset: 0x00085C6C
		[NullableContext(2)]
		public bool CanToggleFold(EntityUid uid, FoldableComponent fold = null)
		{
			StrapComponent strap;
			EntityStorageComponent storage;
			return base.Resolve<FoldableComponent>(uid, ref fold, true) && !this._container.IsEntityInContainer(uid, null) && (!base.TryComp<StrapComponent>(uid, ref strap) || !strap.BuckledEntities.Any<EntityUid>()) && (!base.TryComp<EntityStorageComponent>(uid, ref storage) || (!storage.Open && !storage.Contents.ContainedEntities.Any<EntityUid>()));
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x00087ADC File Offset: 0x00085CDC
		public bool TrySetFolded(EntityUid uid, FoldableComponent comp, bool state)
		{
			if (state == comp.IsFolded)
			{
				return false;
			}
			if (!this.CanToggleFold(uid, comp))
			{
				return false;
			}
			this.SetFolded(uid, comp, state);
			return true;
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x00087AFF File Offset: 0x00085CFF
		public override void SetFolded(EntityUid uid, FoldableComponent component, bool folded)
		{
			base.SetFolded(uid, component, folded);
			this._buckle.StrapSetEnabled(uid, !component.IsFolded, null);
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x00087B20 File Offset: 0x00085D20
		public void OnStoreThisAttempt(EntityUid uid, FoldableComponent comp, ref StoreMobInItemContainerAttemptEvent args)
		{
			args.Handled = true;
			if (comp.IsFolded)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00087B38 File Offset: 0x00085D38
		private void AddFoldVerb(EntityUid uid, FoldableComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null || !this.CanToggleFold(uid, component))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.TryToggleFold(uid, component);
				},
				Text = (component.IsFolded ? Loc.GetString("unfold-verb") : Loc.GetString("fold-verb")),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png", "/")),
				Priority = (component.IsFolded ? 0 : 2)
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x04001047 RID: 4167
		[Dependency]
		private readonly BuckleSystem _buckle;

		// Token: 0x04001048 RID: 4168
		[Dependency]
		private readonly SharedContainerSystem _container;
	}
}
