using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Administration
{
	// Token: 0x02000485 RID: 1157
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class QuickDialogSystem : EntitySystem
	{
		// Token: 0x06001C7E RID: 7294 RVA: 0x000A5424 File Offset: 0x000A3624
		public override void Initialize()
		{
			base.SubscribeNetworkEvent<QuickDialogOpenEvent>(new EntityEventHandler<QuickDialogOpenEvent>(this.OpenDialog), null, null);
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x000A543C File Offset: 0x000A363C
		private void OpenDialog(QuickDialogOpenEvent ev)
		{
			FancyWindow window = new FancyWindow
			{
				Title = ev.Title
			};
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				Margin = new Thickness(8f)
			};
			Dictionary<string, LineEdit> promptsDict = new Dictionary<string, LineEdit>();
			foreach (QuickDialogEntry quickDialogEntry in ev.Prompts)
			{
				BoxContainer boxContainer2 = new BoxContainer
				{
					Orientation = 0
				};
				boxContainer2.AddChild(new Label
				{
					Text = quickDialogEntry.Prompt,
					HorizontalExpand = true,
					SizeFlagsStretchRatio = 0.5f
				});
				LineEdit lineEdit = new LineEdit
				{
					HorizontalExpand = true
				};
				boxContainer2.AddChild(lineEdit);
				switch (quickDialogEntry.Type)
				{
				case QuickDialogEntryType.Integer:
				{
					LineEdit lineEdit2 = lineEdit;
					lineEdit2.IsValid = (Func<string, bool>)Delegate.Combine(lineEdit2.IsValid, new Func<string, bool>(this.VerifyInt));
					lineEdit.PlaceHolder = "Integer..";
					break;
				}
				case QuickDialogEntryType.Float:
				{
					LineEdit lineEdit3 = lineEdit;
					lineEdit3.IsValid = (Func<string, bool>)Delegate.Combine(lineEdit3.IsValid, new Func<string, bool>(this.VerifyFloat));
					lineEdit.PlaceHolder = "Float..";
					break;
				}
				case QuickDialogEntryType.ShortText:
				{
					LineEdit lineEdit4 = lineEdit;
					lineEdit4.IsValid = (Func<string, bool>)Delegate.Combine(lineEdit4.IsValid, new Func<string, bool>(this.VerifyShortText));
					lineEdit.PlaceHolder = "Short text..";
					break;
				}
				case QuickDialogEntryType.LongText:
				{
					LineEdit lineEdit5 = lineEdit;
					lineEdit5.IsValid = (Func<string, bool>)Delegate.Combine(lineEdit5.IsValid, new Func<string, bool>(this.VerifyLongText));
					lineEdit.PlaceHolder = "Long text..";
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
				promptsDict.Add(quickDialogEntry.FieldId, lineEdit);
				boxContainer.AddChild(boxContainer2);
			}
			BoxContainer boxContainer3 = new BoxContainer
			{
				Orientation = 0,
				HorizontalAlignment = 2
			};
			bool alreadyReplied = false;
			if ((ev.Buttons & QuickDialogButtonFlag.OkButton) != (QuickDialogButtonFlag)0)
			{
				Button button = new Button
				{
					Text = "Ok"
				};
				button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.RaiseNetworkEvent(new QuickDialogResponseEvent(ev.DialogId, (from x in promptsDict
					select new ValueTuple<string, string>(x.Key, x.Value.Text)).ToDictionary(([TupleElementNames(new string[]
					{
						"Key",
						"Text"
					})] ValueTuple<string, string> x) => x.Item1, ([TupleElementNames(new string[]
					{
						"Key",
						"Text"
					})] ValueTuple<string, string> x) => x.Item2), QuickDialogButtonFlag.OkButton));
					alreadyReplied = true;
					window.Close();
				};
				boxContainer3.AddChild(button);
			}
			if ((ev.Buttons & QuickDialogButtonFlag.OkButton) != (QuickDialogButtonFlag)0)
			{
				Button button2 = new Button
				{
					Text = "Cancel"
				};
				button2.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.RaiseNetworkEvent(new QuickDialogResponseEvent(ev.DialogId, new Dictionary<string, string>(), QuickDialogButtonFlag.CancelButton));
					alreadyReplied = true;
					window.Close();
				};
				boxContainer3.AddChild(button2);
			}
			window.OnClose += delegate()
			{
				if (!alreadyReplied)
				{
					this.RaiseNetworkEvent(new QuickDialogResponseEvent(ev.DialogId, new Dictionary<string, string>(), QuickDialogButtonFlag.CancelButton));
				}
			};
			boxContainer.AddChild(boxContainer3);
			window.ContentsContainer.AddChild(boxContainer);
			window.MinWidth *= 2f;
			window.OpenCentered();
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x000A573C File Offset: 0x000A393C
		private bool VerifyInt(string input)
		{
			int num;
			return int.TryParse(input, out num);
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x000A5754 File Offset: 0x000A3954
		private bool VerifyFloat(string input)
		{
			float num;
			return float.TryParse(input, out num);
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x000A5769 File Offset: 0x000A3969
		private bool VerifyShortText(string input)
		{
			return input.Length <= 100;
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x000A5778 File Offset: 0x000A3978
		private bool VerifyLongText(string input)
		{
			return input.Length <= 2000;
		}
	}
}
