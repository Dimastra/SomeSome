using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Construction.Components;
using Content.Server.DoAfter;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Server.Wires;
using Content.Shared.Construction.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Construction
{
	// Token: 0x020005F2 RID: 1522
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PartExchangerSystem : EntitySystem
	{
		// Token: 0x060020A7 RID: 8359 RVA: 0x000AC3CA File Offset: 0x000AA5CA
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PartExchangerComponent, AfterInteractEvent>(new ComponentEventHandler<PartExchangerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<PartExchangerComponent, DoAfterEvent>(new ComponentEventHandler<PartExchangerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x000AC3F4 File Offset: 0x000AA5F4
		private void OnDoAfter(EntityUid uid, PartExchangerComponent component, DoAfterEvent args)
		{
			if (args.Cancelled || args.Handled || args.Args.Target == null)
			{
				IPlayingAudioStream audioStream = component.AudioStream;
				if (audioStream == null)
				{
					return;
				}
				audioStream.Stop();
				return;
			}
			else
			{
				IPlayingAudioStream audioStream2 = component.AudioStream;
				if (audioStream2 != null)
				{
					audioStream2.Stop();
				}
				MachineComponent machine;
				if (!base.TryComp<MachineComponent>(args.Args.Target.Value, ref machine))
				{
					return;
				}
				ServerStorageComponent storage;
				if (!base.TryComp<ServerStorageComponent>(uid, ref storage) || storage.Storage == null)
				{
					return;
				}
				EntityUid? board = Extensions.FirstOrNull<EntityUid>(machine.BoardContainer.ContainedEntities);
				MachineBoardComponent macBoardComp;
				if (board == null || !base.TryComp<MachineBoardComponent>(board, ref macBoardComp))
				{
					return;
				}
				List<MachinePartComponent> machineParts = new List<MachinePartComponent>();
				foreach (EntityUid ent in storage.Storage.ContainedEntities)
				{
					MachinePartComponent part;
					if (base.TryComp<MachinePartComponent>(ent, ref part))
					{
						machineParts.Add(part);
					}
				}
				foreach (EntityUid ent2 in new List<EntityUid>(machine.PartContainer.ContainedEntities))
				{
					MachinePartComponent part2;
					if (base.TryComp<MachinePartComponent>(ent2, ref part2))
					{
						machineParts.Add(part2);
						this._container.RemoveEntity(args.Args.Target.Value, ent2, null, null, null, true, false, null, null);
					}
				}
				machineParts = (from p in machineParts
				orderby p.Rating descending
				select p).ToList<MachinePartComponent>();
				List<MachinePartComponent> updatedParts = new List<MachinePartComponent>();
				foreach (KeyValuePair<string, int> keyValuePair in macBoardComp.Requirements)
				{
					string type2;
					int num;
					keyValuePair.Deconstruct(out type2, out num);
					string type = type2;
					int amount = num;
					IEnumerable<MachinePartComponent> target = (from p in machineParts
					where p.PartType == type
					select p).Take(amount);
					updatedParts.AddRange(target);
				}
				foreach (MachinePartComponent part3 in updatedParts)
				{
					machine.PartContainer.Insert(part3.Owner, this.EntityManager, null, null, null, null);
					machineParts.Remove(part3);
				}
				foreach (MachinePartComponent unused in machineParts)
				{
					storage.Storage.Insert(unused.Owner, null, null, null, null, null);
					this._storage.Insert(uid, unused.Owner, null, false);
				}
				this._construction.RefreshParts(machine);
				args.Handled = true;
				return;
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000AC71C File Offset: 0x000AA91C
		private void OnAfterInteract(EntityUid uid, PartExchangerComponent component, AfterInteractEvent args)
		{
			if (component.DoDistanceCheck && !args.CanReach)
			{
				return;
			}
			if (args.Target == null)
			{
				return;
			}
			if (!base.HasComp<MachineComponent>(args.Target))
			{
				return;
			}
			WiresComponent wires;
			if (base.TryComp<WiresComponent>(args.Target, ref wires) && !wires.IsPanelOpen)
			{
				this._popup.PopupEntity(Loc.GetString("construction-step-condition-wire-panel-open"), args.Target.Value, PopupType.Small);
				return;
			}
			component.AudioStream = this._audio.PlayPvs(component.ExchangeSound, uid, null);
			SharedDoAfterSystem doAfter = this._doAfter;
			EntityUid user = args.User;
			float exchangeDuration = component.ExchangeDuration;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(args.Used);
			doAfter.DoAfter(new DoAfterEventArgs(user, exchangeDuration, default(CancellationToken), target, used)
			{
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnUserMove = true
			});
		}

		// Token: 0x04001428 RID: 5160
		[Dependency]
		private readonly ConstructionSystem _construction;

		// Token: 0x04001429 RID: 5161
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x0400142A RID: 5162
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x0400142B RID: 5163
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x0400142C RID: 5164
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400142D RID: 5165
		[Dependency]
		private readonly StorageSystem _storage;
	}
}
