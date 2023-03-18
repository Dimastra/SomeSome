using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.DoAfter;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.DoAfter
{
	// Token: 0x02000348 RID: 840
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoAfterSystem : SharedDoAfterSystem
	{
		// Token: 0x060014D0 RID: 5328 RVA: 0x0007A484 File Offset: 0x00078684
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			base.SubscribeNetworkEvent<CancelledDoAfterMessage>(new EntityEventHandler<CancelledDoAfterMessage>(this.OnCancelledDoAfter), null, null);
			base.SubscribeLocalEvent<DoAfterComponent, ComponentHandleState>(new ComponentEventRefHandler<DoAfterComponent, ComponentHandleState>(this.OnDoAfterHandleState), null, null);
			this._overlay.AddOverlay(new DoAfterOverlay(this.EntityManager, this._prototype));
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0007A4E3 File Offset: 0x000786E3
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlay.RemoveOverlay<DoAfterOverlay>();
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0007A4F8 File Offset: 0x000786F8
		private void OnDoAfterHandleState(EntityUid uid, DoAfterComponent component, ref ComponentHandleState args)
		{
			DoAfterComponentState doAfterComponentState = args.Current as DoAfterComponentState;
			if (doAfterComponentState == null)
			{
				return;
			}
			foreach (KeyValuePair<byte, DoAfter> keyValuePair in doAfterComponentState.DoAfters)
			{
				byte b;
				DoAfter doAfter;
				keyValuePair.Deconstruct(out b, out doAfter);
				DoAfter doAfter2 = doAfter;
				if (!component.DoAfters.ContainsKey(doAfter2.ID))
				{
					component.DoAfters.Add(doAfter2.ID, doAfter2);
				}
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0007A588 File Offset: 0x00078788
		private void OnCancelledDoAfter(CancelledDoAfterMessage ev)
		{
			DoAfterComponent component;
			if (!base.TryComp<DoAfterComponent>(ev.Uid, ref component))
			{
				return;
			}
			this.Cancel(component, ev.ID);
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0007A5B3 File Offset: 0x000787B3
		public void Remove(DoAfterComponent component, DoAfter doAfter, bool found = false)
		{
			component.DoAfters.Remove(doAfter.ID);
			component.CancelledDoAfters.Remove(doAfter.ID);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0007A5DC File Offset: 0x000787DC
		public void Cancel(DoAfterComponent component, byte id)
		{
			if (component.CancelledDoAfters.ContainsKey(id))
			{
				return;
			}
			if (!component.DoAfters.ContainsKey(id))
			{
				return;
			}
			DoAfter doAfter = component.DoAfters[id];
			doAfter.Cancelled = true;
			doAfter.CancelledTime = this.GameTiming.CurTime;
			component.CancelledDoAfters.Add(id, doAfter);
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0007A63C File Offset: 0x0007883C
		public override void Update(float frameTime)
		{
			if (!this.GameTiming.IsFirstTimePredicted)
			{
				return;
			}
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			foreach (ValueTuple<DoAfterComponent, TransformComponent> valueTuple in base.EntityQuery<DoAfterComponent, TransformComponent>(false))
			{
				DoAfterComponent item = valueTuple.Item1;
				TransformComponent item2 = valueTuple.Item2;
				Dictionary<byte, DoAfter> doAfters = item.DoAfters;
				if (doAfters.Count != 0)
				{
					EntityCoordinates coordinates = item2.Coordinates;
					RemQueue<DoAfter> remQueue = default(RemQueue<DoAfter>);
					foreach (KeyValuePair<byte, DoAfter> keyValuePair in doAfters)
					{
						byte b;
						DoAfter doAfter;
						keyValuePair.Deconstruct(out b, out doAfter);
						DoAfter doAfter2 = doAfter;
						if ((float)doAfter2.Elapsed.TotalSeconds + (float)doAfter2.CancelledElapsed.TotalSeconds > doAfter2.Delay + 0.5f)
						{
							remQueue.Add(doAfter2);
						}
						else if (doAfter2.Cancelled)
						{
							doAfter2.CancelledElapsed = this.GameTiming.CurTime - doAfter2.CancelledTime;
						}
						else
						{
							doAfter2.Elapsed = this.GameTiming.CurTime - doAfter2.StartTime;
							if ((float)doAfter2.Elapsed.TotalSeconds <= doAfter2.Delay)
							{
								EntityUid owner = item.Owner;
								EntityUid? entityUid2 = entityUid;
								if (entityUid2 != null)
								{
									owner != entityUid2.GetValueOrDefault();
								}
							}
						}
					}
					foreach (DoAfter doAfter3 in remQueue)
					{
						this.Remove(item, doAfter3, false);
					}
					RemQueue<DoAfter> remQueue2 = default(RemQueue<DoAfter>);
					foreach (KeyValuePair<byte, DoAfter> keyValuePair in item.CancelledDoAfters)
					{
						byte b;
						DoAfter doAfter;
						keyValuePair.Deconstruct(out b, out doAfter);
						DoAfter doAfter4 = doAfter;
						if ((float)doAfter4.CancelledElapsed.TotalSeconds > 0.5f)
						{
							remQueue2.Add(doAfter4);
						}
					}
					foreach (DoAfter doAfter5 in remQueue2)
					{
						this.Remove(item, doAfter5, false);
					}
				}
			}
		}

		// Token: 0x04000ADA RID: 2778
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x04000ADB RID: 2779
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x04000ADC RID: 2780
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000ADD RID: 2781
		public const float ExcessTime = 0.5f;
	}
}
