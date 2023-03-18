using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Guidebook.Richtext;
using Pidgin;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;
using Robust.Shared.Sandboxing;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook
{
	// Token: 0x020002E4 RID: 740
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DocumentParsingManager
	{
		// Token: 0x060012B2 RID: 4786 RVA: 0x0006F784 File Offset: 0x0006D984
		public void Initialize()
		{
			this._tagParser = DocumentParsingManager.TryOpeningTag.Assert(new Func<string, bool>(this._tagControlParsers.ContainsKey), (string tag) => "unknown tag: " + tag).Bind<Control>((string tag) => this._tagControlParsers[tag]);
			this._controlParser = Parser.OneOf<char, Control>(new Parser<char, Control>[]
			{
				this._tagParser,
				DocumentParsingManager.TryHeaderControl,
				DocumentParsingManager.ListControlParser,
				DocumentParsingManager.TextControlParser
			}).Before<Unit>(Parser.SkipWhitespaces);
			foreach (Type type in this._reflectionManager.GetAllChildren<IDocumentTag>(false))
			{
				this._tagControlParsers.Add(type.Name, this.CreateTagControlParser(type.Name, type, this._sandboxHelper));
			}
			this.ControlParser = Parser.SkipWhitespaces.Then<IEnumerable<Control>>(this._controlParser.Many());
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x0006F8A0 File Offset: 0x0006DAA0
		public bool TryAddMarkup(Control control, string text, bool log = true)
		{
			try
			{
				foreach (Control control2 in ParserExtensions.ParseOrThrow<IEnumerable<Control>>(this.ControlParser, text, null))
				{
					control.AddChild(control2);
				}
			}
			catch (Exception value)
			{
				if (log)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Encountered error while generating markup controls: ");
					defaultInterpolatedStringHandler.AppendFormatted<Exception>(value);
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				return false;
			}
			return true;
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x0006F93C File Offset: 0x0006DB3C
		private Parser<char, Control> CreateTagControlParser(string tagId, Type tagType, ISandboxHelper sandbox)
		{
			return Parser.Map<char, Dictionary<string, string>, IEnumerable<Control>, Control>(delegate(Dictionary<string, string> args, IEnumerable<Control> controls)
			{
				Control control;
				if (!((IDocumentTag)sandbox.CreateInstance(tagType)).TryParseTag(args, out control))
				{
					Logger.Error("Failed to parse " + tagId + " args");
					return new Control();
				}
				foreach (Control control2 in controls)
				{
					control.AddChild(control2);
				}
				return control;
			}, DocumentParsingManager.ParseTagArgs(tagId), this.TagContentParser(tagId)).Labelled(tagId + " control");
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x0006F9A4 File Offset: 0x0006DBA4
		private Parser<char, IEnumerable<Control>> TagContentParser(string tag)
		{
			return Parser.OneOf<char, IEnumerable<Control>>(new Parser<char, IEnumerable<Control>>[]
			{
				Parser.Try<char, Unit>(DocumentParsingManager.ImmediateTagEnd).ThenReturn<IEnumerable<Control>>(Enumerable.Empty<Control>()),
				DocumentParsingManager.TagEnd.Then<IEnumerable<Control>>(this._controlParser.Until<Unit>(DocumentParsingManager.TryTagTerminator(tag)).Labelled(tag + " children"))
			});
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x0006FA04 File Offset: 0x0006DC04
		private static Parser<char, Dictionary<string, string>> ParseTagArgs(string tag)
		{
			return (from x in DocumentParsingManager.TagArgsParser.Labelled(tag + " arguments")
			select x.ToDictionary((ValueTuple<string, string> y) => y.Item1, (ValueTuple<string, string> y) => y.Item2)).Before<Unit>(Parser.SkipWhitespaces);
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0006FA54 File Offset: 0x0006DC54
		private static Parser<char, Unit> TryTagTerminator(string tag)
		{
			return Parser.Try<char, string>(Parser.String("</")).Then<Unit>(Parser.SkipWhitespaces).Then<string>(Parser.String(tag)).Then<Unit>(Parser.SkipWhitespaces).Then<Unit>(DocumentParsingManager.TagEnd).Labelled("closing " + tag + " tag");
		}

		// Token: 0x04000941 RID: 2369
		[Dependency]
		private readonly IReflectionManager _reflectionManager;

		// Token: 0x04000942 RID: 2370
		[Dependency]
		private readonly ISandboxHelper _sandboxHelper;

		// Token: 0x04000943 RID: 2371
		private readonly Dictionary<string, Parser<char, Control>> _tagControlParsers = new Dictionary<string, Parser<char, Control>>();

		// Token: 0x04000944 RID: 2372
		private Parser<char, Control> _tagParser;

		// Token: 0x04000945 RID: 2373
		private Parser<char, Control> _controlParser;

		// Token: 0x04000946 RID: 2374
		public Parser<char, IEnumerable<Control>> ControlParser;

		// Token: 0x04000947 RID: 2375
		private const string ListBullet = "  › ";

		// Token: 0x04000948 RID: 2376
		private static readonly Parser<char, char> TryEscapedChar = Parser.Try<char, char>(Parser.Char('\\').Then<char>(Parser.OneOf<char, char>(new Parser<char, char>[]
		{
			Parser.Try<char, char>(Parser.Char('<')),
			Parser.Try<char, char>(Parser.Char('>')),
			Parser.Try<char, char>(Parser.Char('\\')),
			Parser.Try<char, char>(Parser.Char('-')),
			Parser.Try<char, char>(Parser.Char('=')),
			Parser.Try<char, char>(Parser.Char('"')),
			Parser.Try<char, char>(Parser.Char(' ')),
			Parser.Try<char, char>(Parser.Char('n')).ThenReturn<char>('\n'),
			Parser.Try<char, char>(Parser.Char('t')).ThenReturn<char>('\t')
		})));

		// Token: 0x04000949 RID: 2377
		private static readonly Parser<char, Unit> SkipNewline = Parser.Whitespace.SkipUntil<char>(Parser.Char('\n'));

		// Token: 0x0400094A RID: 2378
		private static readonly Parser<char, char> TrySingleNewlineToSpace = Parser.Try<char, Unit>(DocumentParsingManager.SkipNewline).Then<Unit>(Parser.SkipWhitespaces).ThenReturn<char>(' ');

		// Token: 0x0400094B RID: 2379
		private static readonly Parser<char, char> TextChar = Parser.OneOf<char, char>(new Parser<char, char>[]
		{
			DocumentParsingManager.TryEscapedChar,
			DocumentParsingManager.TrySingleNewlineToSpace,
			Parser<char>.Any
		});

		// Token: 0x0400094C RID: 2380
		private static readonly Parser<char, char> QuotedTextChar = Parser.OneOf<char, char>(new Parser<char, char>[]
		{
			DocumentParsingManager.TryEscapedChar,
			Parser<char>.Any
		});

		// Token: 0x0400094D RID: 2381
		private static readonly Parser<char, string> QuotedText = Parser.Char('"').Then<string>(DocumentParsingManager.QuotedTextChar.Until<char>(Parser.Try<char, char>(Parser.Char('"'))).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>))).Labelled("quoted text");

		// Token: 0x0400094E RID: 2382
		private static readonly Parser<char, Unit> TryStartList = Parser.Try<char, char>(DocumentParsingManager.SkipNewline.Then<Unit>(Parser.SkipWhitespaces).Then<char>(Parser.Char('-'))).Then<Unit>(Parser.SkipWhitespaces);

		// Token: 0x0400094F RID: 2383
		private static readonly Parser<char, Unit> TryStartTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces);

		// Token: 0x04000950 RID: 2384
		private static readonly Parser<char, Unit> TryStartParagraph = Parser.Try<char, Unit>(DocumentParsingManager.SkipNewline.Then<Unit>(DocumentParsingManager.SkipNewline)).Then<Unit>(Parser.SkipWhitespaces);

		// Token: 0x04000951 RID: 2385
		private static readonly Parser<char, Unit> TryLookTextEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[]
		{
			DocumentParsingManager.TryStartTag,
			DocumentParsingManager.TryStartList,
			DocumentParsingManager.TryStartParagraph,
			Parser.Try<char, Unit>(Parser.Whitespace.SkipUntil<Unit>(Parser<char>.End))
		}));

		// Token: 0x04000952 RID: 2386
		private static readonly Parser<char, string> TextParser = DocumentParsingManager.TextChar.AtLeastOnceUntil<Unit>(DocumentParsingManager.TryLookTextEnd).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>));

		// Token: 0x04000953 RID: 2387
		private static readonly Parser<char, Control> TextControlParser = Parser.Try<char, Control>(Parser.Map<char, string, RichTextLabel>(delegate(string text)
		{
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.HorizontalExpand = true;
			richTextLabel.Margin = new Thickness(0f, 0f, 0f, 15f);
			FormattedMessage formattedMessage = new FormattedMessage();
			formattedMessage.PushColor(Color.White);
			formattedMessage.AddMarkup(text);
			formattedMessage.Pop();
			richTextLabel.SetMessage(formattedMessage);
			return richTextLabel;
		}, DocumentParsingManager.TextParser).Cast<Control>()).Labelled("richtext");

		// Token: 0x04000954 RID: 2388
		private static readonly Parser<char, Control> HeaderControlParser = Parser.Try<char, char>(Parser.Char('#')).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((string text) => new Label
		{
			Text = text,
			StyleClasses = 
			{
				"LabelHeadingBigger"
			}
		}, Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[]
		{
			'\n'
		}))).Cast<Control>())).Labelled("header");

		// Token: 0x04000955 RID: 2389
		private static readonly Parser<char, Control> SubHeaderControlParser = Parser.Try<char, string>(Parser.String("##")).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((string text) => new Label
		{
			Text = text,
			StyleClasses = 
			{
				"LabelHeading"
			}
		}, Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[]
		{
			'\n'
		}))).Cast<Control>())).Labelled("subheader");

		// Token: 0x04000956 RID: 2390
		private static readonly Parser<char, Control> TryHeaderControl = Parser.OneOf<char, Control>(new Parser<char, Control>[]
		{
			DocumentParsingManager.SubHeaderControlParser,
			DocumentParsingManager.HeaderControlParser
		});

		// Token: 0x04000957 RID: 2391
		private static readonly Parser<char, Control> ListControlParser = Parser.Try<char, char>(Parser.Char('-')).Then<Unit>(Parser.SkipWhitespaces).Then<Control>(Parser.Map<char, Control, BoxContainer>(delegate(Control control)
		{
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Children.Add(new Label
			{
				Text = "  › ",
				VerticalAlignment = 1
			});
			boxContainer.Children.Add(control);
			boxContainer.Orientation = 0;
			return boxContainer;
		}, DocumentParsingManager.TextControlParser).Cast<Control>()).Labelled("list");

		// Token: 0x04000958 RID: 2392
		private static readonly Parser<char, Unit> TagEnd = Parser.Char('>').Then<Unit>(Parser.SkipWhitespaces);

		// Token: 0x04000959 RID: 2393
		private static readonly Parser<char, Unit> ImmediateTagEnd = Parser.String("/>").Then<Unit>(Parser.SkipWhitespaces);

		// Token: 0x0400095A RID: 2394
		private static readonly Parser<char, Unit> TryLookTagEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[]
		{
			Parser.Try<char, Unit>(DocumentParsingManager.TagEnd),
			Parser.Try<char, Unit>(DocumentParsingManager.ImmediateTagEnd)
		}));

		// Token: 0x0400095B RID: 2395
		private static readonly Parser<char, string> TagArgKey = Parser.LetterOrDigit.Until<char>(Parser.Char('=')).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>)).Labelled("tag argument key");

		// Token: 0x0400095C RID: 2396
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		private static readonly Parser<char, ValueTuple<string, string>> TagArgParser = Parser.Map<char, string, string, ValueTuple<string, string>>((string key, string value) => new ValueTuple<string, string>(key, value), DocumentParsingManager.TagArgKey, DocumentParsingManager.QuotedText).Before<Unit>(Parser.SkipWhitespaces);

		// Token: 0x0400095D RID: 2397
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1,
			1
		})]
		private static readonly Parser<char, IEnumerable<ValueTuple<string, string>>> TagArgsParser = DocumentParsingManager.TagArgParser.Until<Unit>(DocumentParsingManager.TryLookTagEnd);

		// Token: 0x0400095E RID: 2398
		private static readonly Parser<char, string> TryOpeningTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces).Then<IEnumerable<char>>(DocumentParsingManager.TextChar.Until<Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[]
		{
			Parser.Whitespace.SkipAtLeastOnce(),
			DocumentParsingManager.TryLookTagEnd
		}))).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>)).Labelled("opening tag");
	}
}
