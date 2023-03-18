using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Chat.Managers;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Emotions.Windows;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.UserInterface.Systems.Emotions
{
	// Token: 0x02000098 RID: 152
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmotionsUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600039D RID: 925 RVA: 0x00015886 File Offset: 0x00013A86
		[Nullable(2)]
		private MenuButton EmotionsButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.EmotionsButton;
			}
		}

		// Token: 0x0600039E RID: 926 RVA: 0x000158A0 File Offset: 0x00013AA0
		public void OnStateEntered(GameplayState state)
		{
			this._window = this.UIManager.CreateWindow<EmotionsWindow>();
			this._window.OnOpen += this.OnWindowOpened;
			this._window.OnClose += this.OnWindowClosed;
			List<EmotePrototype> list = this._prototypeManager.EnumeratePrototypes<EmotePrototype>().ToList<EmotePrototype>();
			list.Sort((EmotePrototype a, EmotePrototype b) => string.Compare(a.ButtonText, b.ButtonText.ToString(), StringComparison.Ordinal));
			using (List<EmotePrototype>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EmotePrototype emote = enumerator.Current;
					Button button = new Button();
					button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
					{
						this.UseEmote(RandomExtensions.Pick<string>(this._random, emote.ChatMessages));
					};
					button.Text = emote.ButtonText;
					button.HorizontalExpand = true;
					button.VerticalExpand = true;
					button.MaxWidth = 250f;
					button.MaxHeight = 50f;
					this._window.EmotionsContainer.AddChild(button);
				}
			}
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenEmotionsMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true)).Register<EmotionsUIController>();
		}

		// Token: 0x0600039F RID: 927 RVA: 0x000159F8 File Offset: 0x00013BF8
		public void UnloadButton()
		{
			if (this.EmotionsButton == null)
			{
				return;
			}
			this.EmotionsButton.OnPressed -= this.EmotionsButtonPressed;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00015A1C File Offset: 0x00013C1C
		private void UseEmote(string emote)
		{
			if ((DateTime.Now - this._lastEmotionTimeUse).TotalSeconds < (double)this._emoteCooldown)
			{
				return;
			}
			this._lastEmotionTimeUse = DateTime.Now;
			this._chatManager.SendMessage(emote, ChatSelectChannel.Emotes);
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00015A67 File Offset: 0x00013C67
		public void LoadButton()
		{
			if (this.EmotionsButton == null)
			{
				return;
			}
			this.EmotionsButton.OnPressed += this.EmotionsButtonPressed;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00015A89 File Offset: 0x00013C89
		private void OnWindowOpened()
		{
			if (this.EmotionsButton != null)
			{
				this.EmotionsButton.Pressed = true;
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00015A9F File Offset: 0x00013C9F
		private void OnWindowClosed()
		{
			if (this.EmotionsButton != null)
			{
				this.EmotionsButton.Pressed = false;
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00015AB8 File Offset: 0x00013CB8
		public void OnStateExited(GameplayState state)
		{
			if (this._window != null)
			{
				this._window.OnOpen -= this.OnWindowOpened;
				this._window.OnClose -= this.OnWindowClosed;
				this._window.Dispose();
				this._window = null;
			}
			CommandBinds.Unregister<EmotionsUIController>();
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00015B12 File Offset: 0x00013D12
		private void EmotionsButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.ToggleWindow();
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00015B1A File Offset: 0x00013D1A
		private void ToggleWindow()
		{
			if (this._window == null)
			{
				return;
			}
			if (this._window.IsOpen)
			{
				this._window.Close();
				return;
			}
			this._window.Open();
		}

		// Token: 0x040001B5 RID: 437
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001B6 RID: 438
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040001B7 RID: 439
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040001B8 RID: 440
		[Nullable(2)]
		private EmotionsWindow _window;

		// Token: 0x040001B9 RID: 441
		private DateTime _lastEmotionTimeUse = DateTime.Now;

		// Token: 0x040001BA RID: 442
		private float _emoteCooldown = 1.5f;
	}
}
