using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Content.Shared.Foldable
{
	// Token: 0x0200047F RID: 1151
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedFoldableSystem : EntitySystem
	{
		// Token: 0x06000DE2 RID: 3554 RVA: 0x0002D46C File Offset: 0x0002B66C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FoldableComponent, ComponentGetState>(new ComponentEventRefHandler<FoldableComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<FoldableComponent, ComponentHandleState>(new ComponentEventRefHandler<FoldableComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<FoldableComponent, ComponentInit>(new ComponentEventHandler<FoldableComponent, ComponentInit>(this.OnFoldableInit), null, null);
			base.SubscribeLocalEvent<FoldableComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<FoldableComponent, ContainerGettingInsertedAttemptEvent>(this.OnInsertEvent), null, null);
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x0002D4CF File Offset: 0x0002B6CF
		private void OnGetState(EntityUid uid, FoldableComponent component, ref ComponentGetState args)
		{
			args.State = new FoldableComponentState(component.IsFolded);
		}

		// Token: 0x06000DE4 RID: 3556 RVA: 0x0002D4E4 File Offset: 0x0002B6E4
		private void OnHandleState(EntityUid uid, FoldableComponent component, ref ComponentHandleState args)
		{
			FoldableComponentState state = args.Current as FoldableComponentState;
			if (state == null)
			{
				return;
			}
			if (state.IsFolded != component.IsFolded)
			{
				this.SetFolded(uid, component, state.IsFolded);
			}
		}

		// Token: 0x06000DE5 RID: 3557 RVA: 0x0002D51D File Offset: 0x0002B71D
		private void OnFoldableInit(EntityUid uid, FoldableComponent component, ComponentInit args)
		{
			this.SetFolded(uid, component, component.IsFolded);
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x0002D52D File Offset: 0x0002B72D
		public virtual void SetFolded(EntityUid uid, FoldableComponent component, bool folded)
		{
			if (component.IsFolded == folded)
			{
				return;
			}
			component.IsFolded = folded;
			base.Dirty(component, null);
			this.Appearance.SetData(uid, SharedFoldableSystem.FoldedVisuals.State, folded, null);
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x0002D561 File Offset: 0x0002B761
		private void OnInsertEvent(EntityUid uid, FoldableComponent component, ContainerGettingInsertedAttemptEvent args)
		{
			if (!component.IsFolded)
			{
				args.Cancel();
			}
		}

		// Token: 0x04000D31 RID: 3377
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x0200080B RID: 2059
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public enum FoldedVisuals : byte
		{
			// Token: 0x040018B7 RID: 6327
			State
		}
	}
}
