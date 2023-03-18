using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Audio;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tools
{
	// Token: 0x020000B1 RID: 177
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedToolSystem : EntitySystem
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000A2CC File Offset: 0x000084CC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MultipleToolComponent, ComponentStartup>(new ComponentEventHandler<MultipleToolComponent, ComponentStartup>(this.OnMultipleToolStartup), null, null);
			base.SubscribeLocalEvent<MultipleToolComponent, ActivateInWorldEvent>(new ComponentEventHandler<MultipleToolComponent, ActivateInWorldEvent>(this.OnMultipleToolActivated), null, null);
			base.SubscribeLocalEvent<MultipleToolComponent, ComponentGetState>(new ComponentEventRefHandler<MultipleToolComponent, ComponentGetState>(this.OnMultipleToolGetState), null, null);
			base.SubscribeLocalEvent<MultipleToolComponent, ComponentHandleState>(new ComponentEventRefHandler<MultipleToolComponent, ComponentHandleState>(this.OnMultipleToolHandleState), null, null);
			base.SubscribeLocalEvent<ToolComponent, DoAfterEvent<ToolEventData>>(new ComponentEventHandler<ToolComponent, DoAfterEvent<ToolEventData>>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<SharedToolSystem.ToolDoAfterComplete>(new EntityEventHandler<SharedToolSystem.ToolDoAfterComplete>(this.OnDoAfterComplete), null, null);
			base.SubscribeLocalEvent<SharedToolSystem.ToolDoAfterCancelled>(new EntityEventHandler<SharedToolSystem.ToolDoAfterCancelled>(this.OnDoAfterCancelled), null, null);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000A368 File Offset: 0x00008568
		private void OnDoAfter(EntityUid uid, ToolComponent component, DoAfterEvent<ToolEventData> args)
		{
			if (args.Handled || args.AdditionalData.Ev == null)
			{
				return;
			}
			if (args.Cancelled)
			{
				if (args.AdditionalData.CancelledEv != null)
				{
					if (args.AdditionalData.TargetEntity != null)
					{
						base.RaiseLocalEvent(args.AdditionalData.TargetEntity.Value, args.AdditionalData.CancelledEv, false);
					}
					else
					{
						base.RaiseLocalEvent(args.AdditionalData.CancelledEv);
					}
					args.Handled = true;
				}
				return;
			}
			if (this.ToolFinishUse(uid, args.Args.User, args.AdditionalData.Fuel, null))
			{
				if (args.AdditionalData.TargetEntity != null)
				{
					base.RaiseLocalEvent(args.AdditionalData.TargetEntity.Value, args.AdditionalData.Ev, false);
				}
				else
				{
					base.RaiseLocalEvent(args.AdditionalData.Ev);
				}
				args.Handled = true;
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000A460 File Offset: 0x00008660
		[NullableContext(2)]
		public bool UseTool(EntityUid tool, EntityUid user, EntityUid? target, float doAfterDelay, [Nullable(1)] IEnumerable<string> toolQualitiesNeeded, [Nullable(1)] ToolEventData toolEventData, float fuel = 0f, ToolComponent toolComponent = null, Func<bool> doAfterCheck = null, CancellationTokenSource cancelToken = null)
		{
			if (!base.Resolve<ToolComponent>(tool, ref toolComponent, false))
			{
				return false;
			}
			ToolUserAttemptUseEvent ev = new ToolUserAttemptUseEvent(user, target);
			base.RaiseLocalEvent<ToolUserAttemptUseEvent>(user, ref ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
			if (!this.ToolStartUse(tool, user, fuel, toolQualitiesNeeded, toolComponent))
			{
				return false;
			}
			if (doAfterDelay > 0f)
			{
				DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(user, doAfterDelay / toolComponent.SpeedModifier, (cancelToken != null) ? cancelToken.Token : default(CancellationToken), target, new EntityUid?(tool))
				{
					ExtraCheck = doAfterCheck,
					BreakOnDamage = true,
					BreakOnStun = true,
					BreakOnTargetMove = true,
					BreakOnUserMove = true,
					NeedHand = true
				};
				this._doAfterSystem.DoAfter<ToolEventData>(doAfterArgs, toolEventData);
				return true;
			}
			return this.ToolFinishUse(tool, user, fuel, toolComponent);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000A528 File Offset: 0x00008728
		public bool UseTool(EntityUid tool, EntityUid user, EntityUid? target, float doAfterDelay, string toolQualityNeeded, ToolEventData toolEventData, float fuel = 0f, [Nullable(2)] ToolComponent toolComponent = null, [Nullable(2)] Func<bool> doAfterCheck = null)
		{
			return this.UseTool(tool, user, target, doAfterDelay, new string[]
			{
				toolQualityNeeded
			}, toolEventData, fuel, toolComponent, doAfterCheck, null);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000A554 File Offset: 0x00008754
		private void OnMultipleToolHandleState(EntityUid uid, MultipleToolComponent component, ref ComponentHandleState args)
		{
			MultipleToolComponentState state = args.Current as MultipleToolComponentState;
			if (state == null)
			{
				return;
			}
			component.CurrentEntry = state.Selected;
			this.SetMultipleTool(uid, component, null, false, null);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000A590 File Offset: 0x00008790
		private void OnMultipleToolStartup(EntityUid uid, MultipleToolComponent multiple, ComponentStartup args)
		{
			ToolComponent tool;
			if (this.EntityManager.TryGetComponent<ToolComponent>(uid, ref tool))
			{
				this.SetMultipleTool(uid, multiple, tool, false, null);
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000A5C0 File Offset: 0x000087C0
		private void OnMultipleToolActivated(EntityUid uid, MultipleToolComponent multiple, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = this.CycleMultipleTool(uid, multiple, new EntityUid?(args.User));
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000A5E4 File Offset: 0x000087E4
		private void OnMultipleToolGetState(EntityUid uid, MultipleToolComponent multiple, ref ComponentGetState args)
		{
			args.State = new MultipleToolComponentState(multiple.CurrentEntry);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000A5F8 File Offset: 0x000087F8
		[NullableContext(2)]
		public bool CycleMultipleTool(EntityUid uid, MultipleToolComponent multiple = null, EntityUid? user = null)
		{
			if (!base.Resolve<MultipleToolComponent>(uid, ref multiple, true))
			{
				return false;
			}
			if (multiple.Entries.Length == 0)
			{
				return false;
			}
			multiple.CurrentEntry = (uint)((ulong)(multiple.CurrentEntry + 1U) % (ulong)((long)multiple.Entries.Length));
			this.SetMultipleTool(uid, multiple, null, true, user);
			return true;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000A644 File Offset: 0x00008844
		[NullableContext(2)]
		public virtual void SetMultipleTool(EntityUid uid, MultipleToolComponent multiple = null, ToolComponent tool = null, bool playSound = false, EntityUid? user = null)
		{
			if (!base.Resolve<MultipleToolComponent, ToolComponent>(uid, ref multiple, ref tool, true))
			{
				return;
			}
			base.Dirty(multiple, null);
			if ((long)multiple.Entries.Length <= (long)((ulong)multiple.CurrentEntry))
			{
				multiple.CurrentQualityName = Loc.GetString("multiple-tool-component-no-behavior");
				return;
			}
			MultipleToolComponent.ToolEntry current = multiple.Entries[(int)multiple.CurrentEntry];
			tool.UseSound = current.Sound;
			tool.Qualities = current.Behavior;
			if (playSound && current.ChangeSound != null)
			{
				this._audioSystem.PlayPredicted(current.ChangeSound, uid, user, null);
			}
			ToolQualityPrototype quality;
			if (this._protoMan.TryIndex<ToolQualityPrototype>(current.Behavior.First<string>(), ref quality))
			{
				multiple.CurrentQualityName = Loc.GetString(quality.Name);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000A706 File Offset: 0x00008906
		public bool HasQuality(EntityUid uid, string quality, [Nullable(2)] ToolComponent tool = null)
		{
			return base.Resolve<ToolComponent>(uid, ref tool, false) && tool.Qualities.Contains(quality);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000A722 File Offset: 0x00008922
		public bool HasAllQualities(EntityUid uid, IEnumerable<string> qualities, [Nullable(2)] ToolComponent tool = null)
		{
			return base.Resolve<ToolComponent>(uid, ref tool, false) && tool.Qualities.ContainsAll(qualities);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000A740 File Offset: 0x00008940
		private bool ToolStartUse(EntityUid tool, EntityUid user, float fuel, IEnumerable<string> toolQualitiesNeeded, [Nullable(2)] ToolComponent toolComponent = null)
		{
			if (!base.Resolve<ToolComponent>(tool, ref toolComponent, true))
			{
				return false;
			}
			if (!toolComponent.Qualities.ContainsAll(toolQualitiesNeeded))
			{
				return false;
			}
			ToolUseAttemptEvent beforeAttempt = new ToolUseAttemptEvent(fuel, user);
			base.RaiseLocalEvent<ToolUseAttemptEvent>(tool, beforeAttempt, false);
			return !beforeAttempt.Cancelled;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000A788 File Offset: 0x00008988
		[NullableContext(2)]
		private bool ToolFinishUse(EntityUid tool, EntityUid user, float fuel, ToolComponent toolComponent = null)
		{
			if (!base.Resolve<ToolComponent>(tool, ref toolComponent, true))
			{
				return false;
			}
			ToolUseFinishAttemptEvent afterAttempt = new ToolUseFinishAttemptEvent(fuel, user);
			base.RaiseLocalEvent<ToolUseFinishAttemptEvent>(tool, afterAttempt, false);
			if (afterAttempt.Cancelled)
			{
				return false;
			}
			if (toolComponent.UseSound != null)
			{
				this.PlayToolSound(tool, toolComponent);
			}
			return true;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000A7D4 File Offset: 0x000089D4
		[NullableContext(2)]
		public void PlayToolSound(EntityUid uid, ToolComponent tool = null)
		{
			if (!base.Resolve<ToolComponent>(uid, ref tool, true))
			{
				return;
			}
			SoundSpecifier sound = tool.UseSound;
			if (sound == null)
			{
				return;
			}
			SoundSystem.Play(sound.GetSound(null, null), Filter.Pvs(tool.Owner, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.175f).WithVolume(-5f)));
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000A838 File Offset: 0x00008A38
		private void OnDoAfterComplete(SharedToolSystem.ToolDoAfterComplete ev)
		{
			if (!this.ToolFinishUse(ev.Uid, ev.UserUid, ev.Fuel, null))
			{
				if (ev.CancelledEvent != null)
				{
					if (ev.EventTarget != null)
					{
						base.RaiseLocalEvent(ev.EventTarget.Value, ev.CancelledEvent, false);
						return;
					}
					base.RaiseLocalEvent(ev.CancelledEvent);
				}
				return;
			}
			if (ev.EventTarget != null)
			{
				base.RaiseLocalEvent(ev.EventTarget.Value, ev.CompletedEvent, false);
				return;
			}
			base.RaiseLocalEvent(ev.CompletedEvent);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000A8CD File Offset: 0x00008ACD
		private void OnDoAfterCancelled(SharedToolSystem.ToolDoAfterCancelled ev)
		{
			if (ev.EventTarget != null)
			{
				base.RaiseLocalEvent(ev.EventTarget.Value, ev.Event, false);
				return;
			}
			base.RaiseLocalEvent(ev.Event);
		}

		// Token: 0x0400026A RID: 618
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x0400026B RID: 619
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x0400026C RID: 620
		[Dependency]
		private readonly SharedDoAfterSystem _doAfterSystem;

		// Token: 0x02000791 RID: 1937
		[Nullable(0)]
		private sealed class ToolDoAfterComplete : EntityEventArgs
		{
			// Token: 0x060017C6 RID: 6086 RVA: 0x0004D116 File Offset: 0x0004B316
			public ToolDoAfterComplete(object completedEvent, [Nullable(2)] object cancelledEvent, EntityUid uid, EntityUid userUid, float fuel, EntityUid? eventTarget = null)
			{
				this.CompletedEvent = completedEvent;
				this.Uid = uid;
				this.UserUid = userUid;
				this.Fuel = fuel;
				this.CancelledEvent = cancelledEvent;
				this.EventTarget = eventTarget;
			}

			// Token: 0x04001797 RID: 6039
			public readonly object CompletedEvent;

			// Token: 0x04001798 RID: 6040
			[Nullable(2)]
			public readonly object CancelledEvent;

			// Token: 0x04001799 RID: 6041
			public readonly EntityUid Uid;

			// Token: 0x0400179A RID: 6042
			public readonly EntityUid UserUid;

			// Token: 0x0400179B RID: 6043
			public readonly float Fuel;

			// Token: 0x0400179C RID: 6044
			public readonly EntityUid? EventTarget;
		}

		// Token: 0x02000792 RID: 1938
		[Nullable(0)]
		private sealed class ToolDoAfterCancelled : EntityEventArgs
		{
			// Token: 0x060017C7 RID: 6087 RVA: 0x0004D14B File Offset: 0x0004B34B
			public ToolDoAfterCancelled(object @event, EntityUid? eventTarget = null)
			{
				this.Event = @event;
				this.EventTarget = eventTarget;
			}

			// Token: 0x0400179D RID: 6045
			public readonly object Event;

			// Token: 0x0400179E RID: 6046
			public readonly EntityUid? EventTarget;
		}
	}
}
