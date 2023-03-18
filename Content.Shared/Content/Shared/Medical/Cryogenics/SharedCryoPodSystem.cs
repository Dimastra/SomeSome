using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Medical.Components;
using Content.Shared.Body.Components;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Medical.Cryogenics
{
	// Token: 0x02000310 RID: 784
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedCryoPodSystem : EntitySystem
	{
		// Token: 0x06000901 RID: 2305 RVA: 0x0001E42D File Offset: 0x0001C62D
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeInsideCryoPod();
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0001E43B File Offset: 0x0001C63B
		protected void OnCryoPodCanDropOn(EntityUid uid, SharedCryoPodComponent component, ref CanDropTargetEvent args)
		{
			args.CanDrop = (args.CanDrop && base.HasComp<BodyComponent>(args.Dragged));
			args.Handled = true;
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0001E461 File Offset: 0x0001C661
		protected void OnComponentInit(EntityUid uid, SharedCryoPodComponent cryoPodComponent, ComponentInit args)
		{
			cryoPodComponent.BodyContainer = this._containerSystem.EnsureContainer<ContainerSlot>(uid, "scanner-body", null);
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0001E47C File Offset: 0x0001C67C
		[NullableContext(2)]
		protected void UpdateAppearance(EntityUid uid, SharedCryoPodComponent cryoPod = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<SharedCryoPodComponent>(uid, ref cryoPod, true))
			{
				return;
			}
			bool cryoPodEnabled = base.HasComp<ActiveCryoPodComponent>(uid);
			SharedPointLightComponent light;
			if (base.TryComp<SharedPointLightComponent>(uid, ref light))
			{
				light.Enabled = (cryoPodEnabled && cryoPod.BodyContainer.ContainedEntity != null);
			}
			if (!base.Resolve<AppearanceComponent>(uid, ref appearance, true))
			{
				return;
			}
			this._appearanceSystem.SetData(uid, SharedCryoPodComponent.CryoPodVisuals.ContainsEntity, cryoPod.BodyContainer.ContainedEntity == null, appearance);
			this._appearanceSystem.SetData(uid, SharedCryoPodComponent.CryoPodVisuals.IsOn, cryoPodEnabled, appearance);
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0001E51C File Offset: 0x0001C71C
		public void InsertBody(EntityUid uid, EntityUid target, SharedCryoPodComponent cryoPodComponent)
		{
			if (cryoPodComponent.BodyContainer.ContainedEntity != null)
			{
				return;
			}
			if (!base.HasComp<MobStateComponent>(target))
			{
				return;
			}
			TransformComponent xform = base.Transform(target);
			cryoPodComponent.BodyContainer.Insert(target, null, xform, null, null, null);
			base.EnsureComp<InsideCryoPodComponent>(target);
			this._standingStateSystem.Stand(target, null, null, true);
			this.UpdateAppearance(uid, cryoPodComponent, null);
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0001E584 File Offset: 0x0001C784
		[NullableContext(2)]
		public void TryEjectBody(EntityUid uid, EntityUid userId, SharedCryoPodComponent cryoPodComponent)
		{
			if (!base.Resolve<SharedCryoPodComponent>(uid, ref cryoPodComponent, true))
			{
				return;
			}
			if (cryoPodComponent.Locked)
			{
				this._popupSystem.PopupEntity(Loc.GetString("cryo-pod-locked"), uid, userId, PopupType.Small);
				return;
			}
			this.EjectBody(uid, cryoPodComponent);
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0001E5BC File Offset: 0x0001C7BC
		[NullableContext(2)]
		public virtual void EjectBody(EntityUid uid, SharedCryoPodComponent cryoPodComponent)
		{
			if (!base.Resolve<SharedCryoPodComponent>(uid, ref cryoPodComponent, true))
			{
				return;
			}
			EntityUid? containedEntity = cryoPodComponent.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				if (contained.Valid)
				{
					cryoPodComponent.BodyContainer.Remove(contained, null, null, null, true, false, null, null);
					if (base.HasComp<KnockedDownComponent>(contained) || this._mobStateSystem.IsIncapacitated(contained, null))
					{
						this._standingStateSystem.Down(contained, true, true, null, null, null);
					}
					else
					{
						this._standingStateSystem.Stand(contained, null, null, false);
					}
					this.UpdateAppearance(uid, cryoPodComponent, null);
					return;
				}
			}
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0001E668 File Offset: 0x0001C868
		protected void AddAlternativeVerbs(EntityUid uid, SharedCryoPodComponent cryoPodComponent, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (cryoPodComponent.BodyContainer.ContainedEntity != null)
			{
				args.Verbs.Add(new AlternativeVerb
				{
					Text = Loc.GetString("cryo-pod-verb-noun-occupant"),
					Category = VerbCategory.Eject,
					Priority = 1,
					Act = delegate()
					{
						this.TryEjectBody(uid, args.User, cryoPodComponent);
					}
				});
			}
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0001E716 File Offset: 0x0001C916
		[NullableContext(2)]
		protected void OnEmagged(EntityUid uid, SharedCryoPodComponent cryoPodComponent, ref GotEmaggedEvent args)
		{
			if (!base.Resolve<SharedCryoPodComponent>(uid, ref cryoPodComponent, true))
			{
				return;
			}
			cryoPodComponent.PermaLocked = true;
			cryoPodComponent.Locked = true;
			args.Handled = true;
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0001E73A File Offset: 0x0001C93A
		protected void OnCryoPodPryFinished(EntityUid uid, SharedCryoPodComponent cryoPodComponent, SharedCryoPodSystem.CryoPodPryFinished args)
		{
			cryoPodComponent.IsPrying = false;
			this.EjectBody(uid, cryoPodComponent);
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0001E74B File Offset: 0x0001C94B
		protected void OnCryoPodPryInterrupted(EntityUid uid, SharedCryoPodComponent cryoPodComponent, SharedCryoPodSystem.CryoPodPryInterrupted args)
		{
			cryoPodComponent.IsPrying = false;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0001E754 File Offset: 0x0001C954
		public virtual void InitializeInsideCryoPod()
		{
			base.SubscribeLocalEvent<InsideCryoPodComponent, DownAttemptEvent>(new ComponentEventHandler<InsideCryoPodComponent, DownAttemptEvent>(this.HandleDown), null, null);
			base.SubscribeLocalEvent<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<InsideCryoPodComponent, EntGotRemovedFromContainerMessage>(this.OnEntGotRemovedFromContainer), null, null);
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0001E77E File Offset: 0x0001C97E
		private void HandleDown(EntityUid uid, InsideCryoPodComponent component, DownAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0001E786 File Offset: 0x0001C986
		private void OnEntGotRemovedFromContainer(EntityUid uid, InsideCryoPodComponent component, EntGotRemovedFromContainerMessage args)
		{
			if (base.Terminating(uid, null))
			{
				return;
			}
			base.RemComp<InsideCryoPodComponent>(uid);
		}

		// Token: 0x040008F9 RID: 2297
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x040008FA RID: 2298
		[Dependency]
		private readonly StandingStateSystem _standingStateSystem;

		// Token: 0x040008FB RID: 2299
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040008FC RID: 2300
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040008FD RID: 2301
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x020007DB RID: 2011
		[Nullable(0)]
		protected class CryoPodPryFinished : IEquatable<SharedCryoPodSystem.CryoPodPryFinished>
		{
			// Token: 0x170004FC RID: 1276
			// (get) Token: 0x0600183E RID: 6206 RVA: 0x0004DC29 File Offset: 0x0004BE29
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(SharedCryoPodSystem.CryoPodPryFinished);
				}
			}

			// Token: 0x0600183F RID: 6207 RVA: 0x0004DC38 File Offset: 0x0004BE38
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CryoPodPryFinished");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06001840 RID: 6208 RVA: 0x0004DC84 File Offset: 0x0004BE84
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				return false;
			}

			// Token: 0x06001841 RID: 6209 RVA: 0x0004DC87 File Offset: 0x0004BE87
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(SharedCryoPodSystem.CryoPodPryFinished left, SharedCryoPodSystem.CryoPodPryFinished right)
			{
				return !(left == right);
			}

			// Token: 0x06001842 RID: 6210 RVA: 0x0004DC93 File Offset: 0x0004BE93
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(SharedCryoPodSystem.CryoPodPryFinished left, SharedCryoPodSystem.CryoPodPryFinished right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x06001843 RID: 6211 RVA: 0x0004DCA7 File Offset: 0x0004BEA7
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract);
			}

			// Token: 0x06001844 RID: 6212 RVA: 0x0004DCB9 File Offset: 0x0004BEB9
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as SharedCryoPodSystem.CryoPodPryFinished);
			}

			// Token: 0x06001845 RID: 6213 RVA: 0x0004DCC7 File Offset: 0x0004BEC7
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(SharedCryoPodSystem.CryoPodPryFinished other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract);
			}

			// Token: 0x06001847 RID: 6215 RVA: 0x0004DCED File Offset: 0x0004BEED
			[CompilerGenerated]
			protected CryoPodPryFinished(SharedCryoPodSystem.CryoPodPryFinished original)
			{
			}

			// Token: 0x06001848 RID: 6216 RVA: 0x0004DCF5 File Offset: 0x0004BEF5
			public CryoPodPryFinished()
			{
			}
		}

		// Token: 0x020007DC RID: 2012
		[Nullable(0)]
		protected class CryoPodPryInterrupted : IEquatable<SharedCryoPodSystem.CryoPodPryInterrupted>
		{
			// Token: 0x170004FD RID: 1277
			// (get) Token: 0x06001849 RID: 6217 RVA: 0x0004DCFD File Offset: 0x0004BEFD
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(SharedCryoPodSystem.CryoPodPryInterrupted);
				}
			}

			// Token: 0x0600184A RID: 6218 RVA: 0x0004DD0C File Offset: 0x0004BF0C
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CryoPodPryInterrupted");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x0600184B RID: 6219 RVA: 0x0004DD58 File Offset: 0x0004BF58
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				return false;
			}

			// Token: 0x0600184C RID: 6220 RVA: 0x0004DD5B File Offset: 0x0004BF5B
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(SharedCryoPodSystem.CryoPodPryInterrupted left, SharedCryoPodSystem.CryoPodPryInterrupted right)
			{
				return !(left == right);
			}

			// Token: 0x0600184D RID: 6221 RVA: 0x0004DD67 File Offset: 0x0004BF67
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(SharedCryoPodSystem.CryoPodPryInterrupted left, SharedCryoPodSystem.CryoPodPryInterrupted right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x0600184E RID: 6222 RVA: 0x0004DD7B File Offset: 0x0004BF7B
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract);
			}

			// Token: 0x0600184F RID: 6223 RVA: 0x0004DD8D File Offset: 0x0004BF8D
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as SharedCryoPodSystem.CryoPodPryInterrupted);
			}

			// Token: 0x06001850 RID: 6224 RVA: 0x0004DD9B File Offset: 0x0004BF9B
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(SharedCryoPodSystem.CryoPodPryInterrupted other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract);
			}

			// Token: 0x06001852 RID: 6226 RVA: 0x0004DDC1 File Offset: 0x0004BFC1
			[CompilerGenerated]
			protected CryoPodPryInterrupted(SharedCryoPodSystem.CryoPodPryInterrupted original)
			{
			}

			// Token: 0x06001853 RID: 6227 RVA: 0x0004DDC9 File Offset: 0x0004BFC9
			public CryoPodPryInterrupted()
			{
			}
		}
	}
}
