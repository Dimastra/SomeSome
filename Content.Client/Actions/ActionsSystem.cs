using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Popups;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Popups;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Client.Actions
{
	// Token: 0x020004F1 RID: 1265
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ActionsSystem : SharedActionsSystem
	{
		// Token: 0x140000C6 RID: 198
		// (add) Token: 0x06002015 RID: 8213 RVA: 0x000BA5AC File Offset: 0x000B87AC
		// (remove) Token: 0x06002016 RID: 8214 RVA: 0x000BA5E4 File Offset: 0x000B87E4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ActionType> ActionAdded;

		// Token: 0x140000C7 RID: 199
		// (add) Token: 0x06002017 RID: 8215 RVA: 0x000BA61C File Offset: 0x000B881C
		// (remove) Token: 0x06002018 RID: 8216 RVA: 0x000BA654 File Offset: 0x000B8854
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ActionType> ActionRemoved;

		// Token: 0x140000C8 RID: 200
		// (add) Token: 0x06002019 RID: 8217 RVA: 0x000BA68C File Offset: 0x000B888C
		// (remove) Token: 0x0600201A RID: 8218 RVA: 0x000BA6C4 File Offset: 0x000B88C4
		public event ActionsSystem.OnActionReplaced ActionReplaced;

		// Token: 0x140000C9 RID: 201
		// (add) Token: 0x0600201B RID: 8219 RVA: 0x000BA6FC File Offset: 0x000B88FC
		// (remove) Token: 0x0600201C RID: 8220 RVA: 0x000BA734 File Offset: 0x000B8934
		public event Action ActionsUpdated;

		// Token: 0x140000CA RID: 202
		// (add) Token: 0x0600201D RID: 8221 RVA: 0x000BA76C File Offset: 0x000B896C
		// (remove) Token: 0x0600201E RID: 8222 RVA: 0x000BA7A4 File Offset: 0x000B89A4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ActionsComponent> LinkActions;

		// Token: 0x140000CB RID: 203
		// (add) Token: 0x0600201F RID: 8223 RVA: 0x000BA7DC File Offset: 0x000B89DC
		// (remove) Token: 0x06002020 RID: 8224 RVA: 0x000BA814 File Offset: 0x000B8A14
		public event Action UnlinkActions;

		// Token: 0x140000CC RID: 204
		// (add) Token: 0x06002021 RID: 8225 RVA: 0x000BA84C File Offset: 0x000B8A4C
		// (remove) Token: 0x06002022 RID: 8226 RVA: 0x000BA884 File Offset: 0x000B8A84
		public event Action ClearAssignments;

		// Token: 0x140000CD RID: 205
		// (add) Token: 0x06002023 RID: 8227 RVA: 0x000BA8BC File Offset: 0x000B8ABC
		// (remove) Token: 0x06002024 RID: 8228 RVA: 0x000BA8F4 File Offset: 0x000B8AF4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<List<ActionsSystem.SlotAssignment>> AssignSlot;

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x000BA929 File Offset: 0x000B8B29
		// (set) Token: 0x06002026 RID: 8230 RVA: 0x000BA931 File Offset: 0x000B8B31
		public ActionsComponent PlayerActions { get; private set; }

		// Token: 0x06002027 RID: 8231 RVA: 0x000BA93C File Offset: 0x000B8B3C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActionsComponent, PlayerAttachedEvent>(new ComponentEventHandler<ActionsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<ActionsComponent, PlayerDetachedEvent>(new ComponentEventHandler<ActionsComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<ActionsComponent, ComponentHandleState>(new ComponentEventRefHandler<ActionsComponent, ComponentHandleState>(this.HandleComponentState), null, null);
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x000BA98C File Offset: 0x000B8B8C
		[NullableContext(1)]
		private void HandleComponentState(EntityUid uid, ActionsComponent component, ref ComponentHandleState args)
		{
			ActionsComponentState actionsComponentState = args.Current as ActionsComponentState;
			if (actionsComponentState == null)
			{
				return;
			}
			SortedSet<ActionType> sortedSet = new SortedSet<ActionType>(actionsComponentState.Actions);
			List<ActionType> list = new List<ActionType>();
			foreach (ActionType actionType in component.Actions.ToList<ActionType>())
			{
				if (!actionType.ClientExclusive)
				{
					ActionType actionType2;
					if (!sortedSet.TryGetValue(actionType, out actionType2))
					{
						component.Actions.Remove(actionType);
						if (actionType.AutoRemove)
						{
							list.Add(actionType);
						}
					}
					else
					{
						actionType.CopyFrom(actionType2);
						sortedSet.Remove(actionType2);
					}
				}
			}
			List<ActionType> list2 = new List<ActionType>();
			foreach (ActionType actionType3 in sortedSet)
			{
				ActionType item = (ActionType)actionType3.Clone();
				component.Actions.Add(item);
				list2.Add(item);
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			foreach (ActionType obj in list)
			{
				Action<ActionType> actionRemoved = this.ActionRemoved;
				if (actionRemoved != null)
				{
					actionRemoved(obj);
				}
			}
			foreach (ActionType obj2 in list2)
			{
				Action<ActionType> actionAdded = this.ActionAdded;
				if (actionAdded != null)
				{
					actionAdded(obj2);
				}
			}
			Action actionsUpdated = this.ActionsUpdated;
			if (actionsUpdated == null)
			{
				return;
			}
			actionsUpdated();
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x000BAB94 File Offset: 0x000B8D94
		[NullableContext(1)]
		protected override void AddActionInternal(ActionsComponent comp, ActionType action)
		{
			ActionType actionType;
			if (comp.Actions.TryGetValue(action, out actionType))
			{
				comp.Actions.Remove(actionType);
				ActionsSystem.OnActionReplaced actionReplaced = this.ActionReplaced;
				if (actionReplaced != null)
				{
					actionReplaced(actionType, action);
				}
			}
			comp.Actions.Add(action);
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000BABE0 File Offset: 0x000B8DE0
		[NullableContext(1)]
		public override void AddAction(EntityUid uid, ActionType action, EntityUid? provider, [Nullable(2)] ActionsComponent comp = null, bool dirty = true)
		{
			if (!base.Resolve<ActionsComponent>(uid, ref comp, false))
			{
				return;
			}
			base.AddAction(uid, action, provider, comp, dirty);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid == ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				Action<ActionType> actionAdded = this.ActionAdded;
				if (actionAdded == null)
				{
					return;
				}
				actionAdded(action);
			}
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000BAC58 File Offset: 0x000B8E58
		[NullableContext(1)]
		public override void RemoveActions(EntityUid uid, IEnumerable<ActionType> actions, [Nullable(2)] ActionsComponent comp = null, bool dirty = true)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			if (!base.Resolve<ActionsComponent>(uid, ref comp, false))
			{
				return;
			}
			List<ActionType> list = actions.ToList<ActionType>();
			base.RemoveActions(uid, list, comp, dirty);
			foreach (ActionType actionType in list)
			{
				if (actionType.AutoRemove)
				{
					Action<ActionType> actionRemoved = this.ActionRemoved;
					if (actionRemoved != null)
					{
						actionRemoved(actionType);
					}
				}
			}
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000BAD1C File Offset: 0x000B8F1C
		[NullableContext(1)]
		protected override bool PerformBasicActions(EntityUid user, ActionType action, bool predicted)
		{
			bool result = action.Sound != null || !string.IsNullOrWhiteSpace(action.UserPopup) || !string.IsNullOrWhiteSpace(action.Popup);
			if (!this.GameTiming.IsFirstTimePredicted)
			{
				return result;
			}
			if (!string.IsNullOrWhiteSpace(action.UserPopup))
			{
				string message = (!action.Toggled || string.IsNullOrWhiteSpace(action.PopupToggleSuffix)) ? Loc.GetString(action.UserPopup) : Loc.GetString(action.UserPopup + action.PopupToggleSuffix);
				this._popupSystem.PopupEntity(message, user, PopupType.Small);
			}
			else if (!string.IsNullOrWhiteSpace(action.Popup))
			{
				string message2 = (!action.Toggled || string.IsNullOrWhiteSpace(action.PopupToggleSuffix)) ? Loc.GetString(action.Popup) : Loc.GetString(action.Popup + action.PopupToggleSuffix);
				this._popupSystem.PopupEntity(message2, user, PopupType.Small);
			}
			if (action.Sound != null)
			{
				SoundSystem.Play(action.Sound.GetSound(null, null), Filter.Local(), user, action.AudioParams);
			}
			return result;
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000BAE31 File Offset: 0x000B9031
		[NullableContext(1)]
		private void OnPlayerAttached(EntityUid uid, ActionsComponent component, PlayerAttachedEvent args)
		{
			this.LinkAllActions(component);
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000BAE3A File Offset: 0x000B903A
		[NullableContext(1)]
		private void OnPlayerDetached(EntityUid uid, ActionsComponent component, [Nullable(2)] PlayerDetachedEvent args = null)
		{
			this.UnlinkAllActions();
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000BAE42 File Offset: 0x000B9042
		public void UnlinkAllActions()
		{
			this.PlayerActions = null;
			Action unlinkActions = this.UnlinkActions;
			if (unlinkActions == null)
			{
				return;
			}
			unlinkActions();
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x000BAE5C File Offset: 0x000B905C
		public void LinkAllActions(ActionsComponent actions = null)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null || !base.Resolve<ActionsComponent>(entityUid.Value, ref actions, true))
			{
				return;
			}
			Action<ActionsComponent> linkActions = this.LinkActions;
			if (linkActions != null)
			{
				linkActions(actions);
			}
			this.PlayerActions = actions;
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x000BAEBE File Offset: 0x000B90BE
		public override void Shutdown()
		{
			base.Shutdown();
			CommandBinds.Unregister<ActionsSystem>();
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x000BAECC File Offset: 0x000B90CC
		public void TriggerAction(ActionType action)
		{
			if (this.PlayerActions != null && action != null)
			{
				LocalPlayer localPlayer = this._playerManager.LocalPlayer;
				EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
				if (entityUid != null)
				{
					EntityUid valueOrDefault = entityUid.GetValueOrDefault();
					if (valueOrDefault.Valid)
					{
						if (action.Provider != null && base.Deleted(action.Provider))
						{
							return;
						}
						InstantAction instantAction = action as InstantAction;
						if (instantAction == null)
						{
							return;
						}
						if (action.ClientExclusive)
						{
							if (instantAction.Event != null)
							{
								instantAction.Event.Performer = valueOrDefault;
							}
							base.PerformAction(valueOrDefault, this.PlayerActions, instantAction, instantAction.Event, this.GameTiming.CurTime, true);
							return;
						}
						RequestPerformActionEvent requestPerformActionEvent = new RequestPerformActionEvent(instantAction);
						this.EntityManager.RaisePredictiveEvent<RequestPerformActionEvent>(requestPerformActionEvent);
						return;
					}
				}
			}
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x000BAF98 File Offset: 0x000B9198
		[NullableContext(1)]
		public void LoadActionAssignments(string path, bool userData)
		{
			if (this.PlayerActions == null)
			{
				return;
			}
			ResourcePath resourcePath = new ResourcePath(path, "/").ToRootedPath();
			TextReader textReader = userData ? WritableDirProviderExt.OpenText(this._resources.UserData, resourcePath) : this._resources.ContentFileReadText(resourcePath);
			YamlStream yamlStream = new YamlStream();
			yamlStream.Load(textReader);
			SequenceDataNode sequenceDataNode = YamlNodeHelpers.ToDataNode(yamlStream.Documents[0].RootNode) as SequenceDataNode;
			if (sequenceDataNode == null)
			{
				return;
			}
			Action clearAssignments = this.ClearAssignments;
			if (clearAssignments != null)
			{
				clearAssignments();
			}
			List<ActionsSystem.SlotAssignment> list = new List<ActionsSystem.SlotAssignment>();
			foreach (DataNode dataNode in sequenceDataNode.Sequence)
			{
				MappingDataNode mappingDataNode = dataNode as MappingDataNode;
				DataNode dataNode2;
				if (mappingDataNode != null && mappingDataNode.TryGet("action", ref dataNode2))
				{
					ActionType actionType = this._serialization.Read<ActionType>(dataNode2, null, false, null, true);
					ActionType actionType2;
					if (this.PlayerActions.Actions.TryGetValue(actionType, out actionType2))
					{
						actionType2.CopyFrom(actionType);
						actionType = actionType2;
					}
					else
					{
						this.PlayerActions.Actions.Add(actionType);
					}
					DataNode dataNode3;
					if (mappingDataNode.TryGet("assignments", ref dataNode3))
					{
						foreach (ValueTuple<byte, byte> valueTuple in this._serialization.Read<List<ValueTuple<byte, byte>>>(dataNode3, null, false, null, true))
						{
							ActionsSystem.SlotAssignment item = new ActionsSystem.SlotAssignment(valueTuple.Item1, valueTuple.Item2, actionType);
							list.Add(item);
						}
					}
				}
			}
			Action<List<ActionsSystem.SlotAssignment>> assignSlot = this.AssignSlot;
			if (assignSlot == null)
			{
				return;
			}
			assignSlot(list);
		}

		// Token: 0x04000F56 RID: 3926
		[Nullable(1)]
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000F57 RID: 3927
		[Nullable(1)]
		[Dependency]
		private readonly IResourceManager _resources;

		// Token: 0x04000F58 RID: 3928
		[Nullable(1)]
		[Dependency]
		private readonly ISerializationManager _serialization;

		// Token: 0x04000F59 RID: 3929
		[Nullable(1)]
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x020004F2 RID: 1266
		// (Invoke) Token: 0x06002036 RID: 8246
		[NullableContext(0)]
		public delegate void OnActionReplaced(ActionType existing, ActionType action);

		// Token: 0x020004F3 RID: 1267
		[NullableContext(1)]
		[Nullable(0)]
		public struct SlotAssignment : IEquatable<ActionsSystem.SlotAssignment>
		{
			// Token: 0x06002039 RID: 8249 RVA: 0x000BB164 File Offset: 0x000B9364
			public SlotAssignment(byte Hotbar, byte Slot, ActionType Action)
			{
				this.Hotbar = Hotbar;
				this.Slot = Slot;
				this.Action = Action;
			}

			// Token: 0x170006FB RID: 1787
			// (get) Token: 0x0600203A RID: 8250 RVA: 0x000BB17B File Offset: 0x000B937B
			// (set) Token: 0x0600203B RID: 8251 RVA: 0x000BB183 File Offset: 0x000B9383
			public byte Hotbar { readonly get; set; }

			// Token: 0x170006FC RID: 1788
			// (get) Token: 0x0600203C RID: 8252 RVA: 0x000BB18C File Offset: 0x000B938C
			// (set) Token: 0x0600203D RID: 8253 RVA: 0x000BB194 File Offset: 0x000B9394
			public byte Slot { readonly get; set; }

			// Token: 0x170006FD RID: 1789
			// (get) Token: 0x0600203E RID: 8254 RVA: 0x000BB19D File Offset: 0x000B939D
			// (set) Token: 0x0600203F RID: 8255 RVA: 0x000BB1A5 File Offset: 0x000B93A5
			public ActionType Action { readonly get; set; }

			// Token: 0x06002040 RID: 8256 RVA: 0x000BB1B0 File Offset: 0x000B93B0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("SlotAssignment");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06002041 RID: 8257 RVA: 0x000BB1FC File Offset: 0x000B93FC
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Hotbar = ");
				builder.Append(this.Hotbar.ToString());
				builder.Append(", Slot = ");
				builder.Append(this.Slot.ToString());
				builder.Append(", Action = ");
				builder.Append(this.Action);
				return true;
			}

			// Token: 0x06002042 RID: 8258 RVA: 0x000BB271 File Offset: 0x000B9471
			[CompilerGenerated]
			public static bool operator !=(ActionsSystem.SlotAssignment left, ActionsSystem.SlotAssignment right)
			{
				return !(left == right);
			}

			// Token: 0x06002043 RID: 8259 RVA: 0x000BB27D File Offset: 0x000B947D
			[CompilerGenerated]
			public static bool operator ==(ActionsSystem.SlotAssignment left, ActionsSystem.SlotAssignment right)
			{
				return left.Equals(right);
			}

			// Token: 0x06002044 RID: 8260 RVA: 0x000BB287 File Offset: 0x000B9487
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<byte>.Default.GetHashCode(this.<Hotbar>k__BackingField) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(this.<Slot>k__BackingField)) * -1521134295 + EqualityComparer<ActionType>.Default.GetHashCode(this.<Action>k__BackingField);
			}

			// Token: 0x06002045 RID: 8261 RVA: 0x000BB2C7 File Offset: 0x000B94C7
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is ActionsSystem.SlotAssignment && this.Equals((ActionsSystem.SlotAssignment)obj);
			}

			// Token: 0x06002046 RID: 8262 RVA: 0x000BB2E0 File Offset: 0x000B94E0
			[CompilerGenerated]
			public readonly bool Equals(ActionsSystem.SlotAssignment other)
			{
				return EqualityComparer<byte>.Default.Equals(this.<Hotbar>k__BackingField, other.<Hotbar>k__BackingField) && EqualityComparer<byte>.Default.Equals(this.<Slot>k__BackingField, other.<Slot>k__BackingField) && EqualityComparer<ActionType>.Default.Equals(this.<Action>k__BackingField, other.<Action>k__BackingField);
			}

			// Token: 0x06002047 RID: 8263 RVA: 0x000BB335 File Offset: 0x000B9535
			[CompilerGenerated]
			public readonly void Deconstruct(out byte Hotbar, out byte Slot, out ActionType Action)
			{
				Hotbar = this.Hotbar;
				Slot = this.Slot;
				Action = this.Action;
			}
		}
	}
}
