using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Hands.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Holiday.Christmas
{
	// Token: 0x02000472 RID: 1138
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomGiftSystem : EntitySystem
	{
		// Token: 0x060016C8 RID: 5832 RVA: 0x00077DF0 File Offset: 0x00075FF0
		public override void Initialize()
		{
			this._prototype.PrototypesReloaded += this.OnPrototypesReloaded;
			base.SubscribeLocalEvent<RandomGiftComponent, MapInitEvent>(new ComponentEventHandler<RandomGiftComponent, MapInitEvent>(this.OnGiftMapInit), null, null);
			base.SubscribeLocalEvent<RandomGiftComponent, UseInHandEvent>(new ComponentEventHandler<RandomGiftComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<RandomGiftComponent, ExaminedEvent>(new ComponentEventHandler<RandomGiftComponent, ExaminedEvent>(this.OnExamined), null, null);
			this.BuildIndex();
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00077E58 File Offset: 0x00076058
		private void OnExamined(EntityUid uid, RandomGiftComponent component, ExaminedEvent args)
		{
			if (!component.ContentsViewers.IsValid(args.Examiner, this.EntityManager) || component.SelectedEntity == null)
			{
				return;
			}
			string name = this._prototype.Index<EntityPrototype>(component.SelectedEntity).Name;
			args.Message.PushNewline();
			args.Message.AddText(Loc.GetString("gift-packin-contains", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", name)
			}));
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x00077ED8 File Offset: 0x000760D8
		private void OnUseInHand(EntityUid uid, RandomGiftComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.SelectedEntity == null)
			{
				return;
			}
			EntityCoordinates coords = base.Transform(args.User).Coordinates;
			EntityUid handsEnt = base.Spawn(component.SelectedEntity, coords);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.EntitySpawn;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(21, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" used ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "ToPrettyString(component.Owner)");
			logStringHandler.AppendLiteral(" which spawned ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(handsEnt), "ToPrettyString(handsEnt)");
			adminLogger.Add(type, impact, ref logStringHandler);
			base.EnsureComp<ItemComponent>(handsEnt);
			if (component.Wrapper != null)
			{
				base.Spawn(component.Wrapper, coords);
			}
			args.Handled = true;
			this._audio.PlayPvs(component.Sound, args.User, null);
			base.Del(uid);
			this._hands.PickupOrDrop(new EntityUid?(args.User), handsEnt, true, false, null, null);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x00077FF4 File Offset: 0x000761F4
		private void OnGiftMapInit(EntityUid uid, RandomGiftComponent component, MapInitEvent args)
		{
			if (component.InsaneMode)
			{
				component.SelectedEntity = RandomExtensions.Pick<string>(this._random, this._possibleGiftsUnsafe);
				return;
			}
			component.SelectedEntity = RandomExtensions.Pick<string>(this._random, this._possibleGiftsSafe);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0007802D File Offset: 0x0007622D
		private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
		{
			this.BuildIndex();
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x00078038 File Offset: 0x00076238
		private void BuildIndex()
		{
			this._possibleGiftsSafe.Clear();
			this._possibleGiftsUnsafe.Clear();
			string itemCompName = this._componentFactory.GetComponentName(typeof(ItemComponent));
			string mapGridCompName = this._componentFactory.GetComponentName(typeof(MapGridComponent));
			string physicsCompName = this._componentFactory.GetComponentName(typeof(PhysicsComponent));
			foreach (EntityPrototype proto in this._prototype.EnumeratePrototypes<EntityPrototype>())
			{
				if (!proto.Abstract && !proto.NoSpawn && !proto.Components.ContainsKey(mapGridCompName) && proto.Components.ContainsKey(physicsCompName))
				{
					this._possibleGiftsUnsafe.Add(proto.ID);
					if (proto.Components.ContainsKey(itemCompName))
					{
						this._possibleGiftsSafe.Add(proto.ID);
					}
				}
			}
		}

		// Token: 0x04000E4A RID: 3658
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04000E4B RID: 3659
		[Dependency]
		private readonly HandsSystem _hands;

		// Token: 0x04000E4C RID: 3660
		[Dependency]
		private readonly IComponentFactory _componentFactory;

		// Token: 0x04000E4D RID: 3661
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000E4E RID: 3662
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000E4F RID: 3663
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000E50 RID: 3664
		private readonly List<string> _possibleGiftsSafe = new List<string>();

		// Token: 0x04000E51 RID: 3665
		private readonly List<string> _possibleGiftsUnsafe = new List<string>();
	}
}
