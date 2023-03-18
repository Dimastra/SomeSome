using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Administration
{
	// Token: 0x02000804 RID: 2052
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class QuickDialogSystem : EntitySystem
	{
		// Token: 0x06002C6E RID: 11374 RVA: 0x000E79F8 File Offset: 0x000E5BF8
		public override void Initialize()
		{
			this._playerManager.PlayerStatusChanged += this.PlayerManagerOnPlayerStatusChanged;
			base.SubscribeNetworkEvent<QuickDialogResponseEvent>(new EntitySessionEventHandler<QuickDialogResponseEvent>(this.Handler), null, null);
		}

		// Token: 0x06002C6F RID: 11375 RVA: 0x000E7A25 File Offset: 0x000E5C25
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.PlayerManagerOnPlayerStatusChanged;
		}

		// Token: 0x06002C70 RID: 11376 RVA: 0x000E7A44 File Offset: 0x000E5C44
		private void Handler(QuickDialogResponseEvent msg, EntitySessionEventArgs args)
		{
			if (!this._openDialogs.ContainsKey(msg.DialogId) || !this._openDialogsByUser[args.SenderSession.UserId].Contains(msg.DialogId))
			{
				INetChannel connectedClient = args.SenderSession.ConnectedClient;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Replied with invalid quick dialog data with id ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(msg.DialogId);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				connectedClient.Disconnect(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			QuickDialogButtonFlag buttonPressed = msg.ButtonPressed;
			if (buttonPressed != QuickDialogButtonFlag.OkButton)
			{
				if (buttonPressed != QuickDialogButtonFlag.CancelButton)
				{
					throw new ArgumentOutOfRangeException();
				}
				this._openDialogs[msg.DialogId].Item2();
			}
			else
			{
				this._openDialogs[msg.DialogId].Item1(msg);
			}
			this._openDialogs.Remove(msg.DialogId);
			this._openDialogsByUser[args.SenderSession.UserId].Remove(msg.DialogId);
		}

		// Token: 0x06002C71 RID: 11377 RVA: 0x000E7B58 File Offset: 0x000E5D58
		private int GetDialogId()
		{
			int nextDialogId = this._nextDialogId;
			this._nextDialogId = nextDialogId + 1;
			return nextDialogId;
		}

		// Token: 0x06002C72 RID: 11378 RVA: 0x000E7B78 File Offset: 0x000E5D78
		private void PlayerManagerOnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus != 4 && e.NewStatus != null)
			{
				return;
			}
			NetUserId user = e.Session.UserId;
			if (!this._openDialogsByUser.ContainsKey(user))
			{
				return;
			}
			foreach (int dialogId in this._openDialogsByUser[user])
			{
				this._openDialogs[dialogId].Item2();
				this._openDialogs.Remove(dialogId);
			}
			this._openDialogsByUser.Remove(user);
		}

		// Token: 0x06002C73 RID: 11379 RVA: 0x000E7C28 File Offset: 0x000E5E28
		private void OpenDialogInternal(IPlayerSession session, string title, List<QuickDialogEntry> entries, QuickDialogButtonFlag buttons, Action<QuickDialogResponseEvent> okAction, Action cancelAction)
		{
			int did = this.GetDialogId();
			base.RaiseNetworkEvent(new QuickDialogOpenEvent(title, entries, did, buttons), session);
			this._openDialogs.Add(did, new ValueTuple<Action<QuickDialogResponseEvent>, Action>(okAction, cancelAction));
			if (!this._openDialogsByUser.ContainsKey(session.UserId))
			{
				this._openDialogsByUser.Add(session.UserId, new List<int>());
			}
			this._openDialogsByUser[session.UserId].Add(did);
		}

		// Token: 0x06002C74 RID: 11380 RVA: 0x000E7CA4 File Offset: 0x000E5EA4
		[NullableContext(2)]
		private bool TryParseQuickDialog<T>(QuickDialogEntryType entryType, [Nullable(1)] string input, [NotNullWhen(true)] out T output)
		{
			switch (entryType)
			{
			case QuickDialogEntryType.Integer:
			{
				int val;
				bool result = int.TryParse(input, out val);
				output = (T)((object)val);
				return result;
			}
			case QuickDialogEntryType.Float:
			{
				float val2;
				bool result2 = float.TryParse(input, out val2);
				output = (T)((object)val2);
				return result2;
			}
			case QuickDialogEntryType.ShortText:
				if (input.Length > 100)
				{
					output = default(T);
					return false;
				}
				output = (T)((object)input);
				return output != null;
			case QuickDialogEntryType.LongText:
			{
				if (input.Length > 2000)
				{
					output = default(T);
					return false;
				}
				LongString longString = (LongString)input;
				output = (T)((object)longString);
				return output != null;
			}
			default:
				throw new ArgumentOutOfRangeException("entryType", entryType, null);
			}
		}

		// Token: 0x06002C75 RID: 11381 RVA: 0x000E7D84 File Offset: 0x000E5F84
		private QuickDialogEntryType TypeToEntryType(Type T)
		{
			if (T == typeof(int) || T == typeof(uint) || T == typeof(long) || T == typeof(ulong))
			{
				return QuickDialogEntryType.Integer;
			}
			if (T == typeof(float) || T == typeof(double))
			{
				return QuickDialogEntryType.Float;
			}
			if (T == typeof(string))
			{
				return QuickDialogEntryType.ShortText;
			}
			if (T == typeof(LongString))
			{
				return QuickDialogEntryType.LongText;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Tried to open a dialog with unsupported type ");
			defaultInterpolatedStringHandler.AppendFormatted<Type>(T);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06002C76 RID: 11382 RVA: 0x000E7E60 File Offset: 0x000E6060
		public void OpenDialog<[Nullable(2)] T1>(IPlayerSession session, string title, string prompt, Action<T1> okAction, [Nullable(2)] Action cancelAction = null)
		{
			IPlayerSession session2 = session;
			List<QuickDialogEntry> list = new List<QuickDialogEntry>();
			list.Add(new QuickDialogEntry("1", this.TypeToEntryType(typeof(T1)), prompt));
			QuickDialogButtonFlag buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;
			Action<QuickDialogResponseEvent> okAction2 = delegate(QuickDialogResponseEvent ev)
			{
				T1 v;
				if (this.TryParseQuickDialog<T1>(this.TypeToEntryType(typeof(T1)), ev.Responses["1"], out v))
				{
					okAction(v);
					return;
				}
				session.ConnectedClient.Disconnect("Replied with invalid quick dialog data.");
				Action cancelAction3 = cancelAction;
				if (cancelAction3 == null)
				{
					return;
				}
				cancelAction3();
			};
			Action cancelAction2;
			if ((cancelAction2 = cancelAction) == null && (cancelAction2 = QuickDialogSystem.<>c__12<T1>.<>9__12_1) == null)
			{
				cancelAction2 = (QuickDialogSystem.<>c__12<T1>.<>9__12_1 = delegate()
				{
				});
			}
			this.OpenDialogInternal(session2, title, list, buttons, okAction2, cancelAction2);
		}

		// Token: 0x06002C77 RID: 11383 RVA: 0x000E7EFC File Offset: 0x000E60FC
		public void OpenDialog<[Nullable(2)] T1, [Nullable(2)] T2>(IPlayerSession session, string title, string prompt1, string prompt2, Action<T1, T2> okAction, [Nullable(2)] Action cancelAction = null)
		{
			IPlayerSession session2 = session;
			List<QuickDialogEntry> list = new List<QuickDialogEntry>();
			list.Add(new QuickDialogEntry("1", this.TypeToEntryType(typeof(T1)), prompt1));
			list.Add(new QuickDialogEntry("2", this.TypeToEntryType(typeof(T2)), prompt2));
			QuickDialogButtonFlag buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;
			Action<QuickDialogResponseEvent> okAction2 = delegate(QuickDialogResponseEvent ev)
			{
				T1 v;
				T2 v2;
				if (this.TryParseQuickDialog<T1>(this.TypeToEntryType(typeof(T1)), ev.Responses["1"], out v) && this.TryParseQuickDialog<T2>(this.TypeToEntryType(typeof(T2)), ev.Responses["2"], out v2))
				{
					okAction(v, v2);
					return;
				}
				session.ConnectedClient.Disconnect("Replied with invalid quick dialog data.");
				Action cancelAction3 = cancelAction;
				if (cancelAction3 == null)
				{
					return;
				}
				cancelAction3();
			};
			Action cancelAction2;
			if ((cancelAction2 = cancelAction) == null && (cancelAction2 = QuickDialogSystem.<>c__13<T1, T2>.<>9__13_1) == null)
			{
				cancelAction2 = (QuickDialogSystem.<>c__13<T1, T2>.<>9__13_1 = delegate()
				{
				});
			}
			this.OpenDialogInternal(session2, title, list, buttons, okAction2, cancelAction2);
		}

		// Token: 0x06002C78 RID: 11384 RVA: 0x000E7FB8 File Offset: 0x000E61B8
		public void OpenDialog<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(IPlayerSession session, string title, string prompt1, string prompt2, string prompt3, Action<T1, T2, T3> okAction, [Nullable(2)] Action cancelAction = null)
		{
			IPlayerSession session2 = session;
			List<QuickDialogEntry> list = new List<QuickDialogEntry>();
			list.Add(new QuickDialogEntry("1", this.TypeToEntryType(typeof(T1)), prompt1));
			list.Add(new QuickDialogEntry("2", this.TypeToEntryType(typeof(T2)), prompt2));
			list.Add(new QuickDialogEntry("3", this.TypeToEntryType(typeof(T3)), prompt3));
			QuickDialogButtonFlag buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;
			Action<QuickDialogResponseEvent> okAction2 = delegate(QuickDialogResponseEvent ev)
			{
				T1 v;
				T2 v2;
				T3 v3;
				if (this.TryParseQuickDialog<T1>(this.TypeToEntryType(typeof(T1)), ev.Responses["1"], out v) && this.TryParseQuickDialog<T2>(this.TypeToEntryType(typeof(T2)), ev.Responses["2"], out v2) && this.TryParseQuickDialog<T3>(this.TypeToEntryType(typeof(T3)), ev.Responses["3"], out v3))
				{
					okAction(v, v2, v3);
					return;
				}
				session.ConnectedClient.Disconnect("Replied with invalid quick dialog data.");
				Action cancelAction3 = cancelAction;
				if (cancelAction3 == null)
				{
					return;
				}
				cancelAction3();
			};
			Action cancelAction2;
			if ((cancelAction2 = cancelAction) == null && (cancelAction2 = QuickDialogSystem.<>c__14<T1, T2, T3>.<>9__14_1) == null)
			{
				cancelAction2 = (QuickDialogSystem.<>c__14<T1, T2, T3>.<>9__14_1 = delegate()
				{
				});
			}
			this.OpenDialogInternal(session2, title, list, buttons, okAction2, cancelAction2);
		}

		// Token: 0x06002C79 RID: 11385 RVA: 0x000E8098 File Offset: 0x000E6298
		public void OpenDialog<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(IPlayerSession session, string title, string prompt1, string prompt2, string prompt3, string prompt4, Action<T1, T2, T3, T4> okAction, [Nullable(2)] Action cancelAction = null)
		{
			IPlayerSession session2 = session;
			List<QuickDialogEntry> list = new List<QuickDialogEntry>();
			list.Add(new QuickDialogEntry("1", this.TypeToEntryType(typeof(T1)), prompt1));
			list.Add(new QuickDialogEntry("2", this.TypeToEntryType(typeof(T2)), prompt2));
			list.Add(new QuickDialogEntry("3", this.TypeToEntryType(typeof(T3)), prompt3));
			list.Add(new QuickDialogEntry("4", this.TypeToEntryType(typeof(T4)), prompt4));
			QuickDialogButtonFlag buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;
			Action<QuickDialogResponseEvent> okAction2 = delegate(QuickDialogResponseEvent ev)
			{
				T1 v;
				T2 v2;
				T3 v3;
				T4 v4;
				if (this.TryParseQuickDialog<T1>(this.TypeToEntryType(typeof(T1)), ev.Responses["1"], out v) && this.TryParseQuickDialog<T2>(this.TypeToEntryType(typeof(T2)), ev.Responses["2"], out v2) && this.TryParseQuickDialog<T3>(this.TypeToEntryType(typeof(T3)), ev.Responses["3"], out v3) && this.TryParseQuickDialog<T4>(this.TypeToEntryType(typeof(T4)), ev.Responses["4"], out v4))
				{
					okAction(v, v2, v3, v4);
					return;
				}
				session.ConnectedClient.Disconnect("Replied with invalid quick dialog data.");
				Action cancelAction3 = cancelAction;
				if (cancelAction3 == null)
				{
					return;
				}
				cancelAction3();
			};
			Action cancelAction2;
			if ((cancelAction2 = cancelAction) == null && (cancelAction2 = QuickDialogSystem.<>c__15<T1, T2, T3, T4>.<>9__15_1) == null)
			{
				cancelAction2 = (QuickDialogSystem.<>c__15<T1, T2, T3, T4>.<>9__15_1 = delegate()
				{
				});
			}
			this.OpenDialogInternal(session2, title, list, buttons, okAction2, cancelAction2);
		}

		// Token: 0x04001B7F RID: 7039
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B80 RID: 7040
		[TupleElementNames(new string[]
		{
			"okAction",
			"cancelAction"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1,
			1
		})]
		private readonly Dictionary<int, ValueTuple<Action<QuickDialogResponseEvent>, Action>> _openDialogs = new Dictionary<int, ValueTuple<Action<QuickDialogResponseEvent>, Action>>();

		// Token: 0x04001B81 RID: 7041
		private readonly Dictionary<NetUserId, List<int>> _openDialogsByUser = new Dictionary<NetUserId, List<int>>();

		// Token: 0x04001B82 RID: 7042
		private int _nextDialogId = 1;
	}
}
