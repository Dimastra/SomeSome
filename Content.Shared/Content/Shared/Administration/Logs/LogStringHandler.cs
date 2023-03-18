using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.Administration.Logs
{
	// Token: 0x0200074F RID: 1871
	[NullableContext(2)]
	[Nullable(0)]
	[CompilerFeatureRequired("RefStructs")]
	[InterpolatedStringHandler]
	public ref struct LogStringHandler
	{
		// Token: 0x060016C5 RID: 5829 RVA: 0x0004A0C0 File Offset: 0x000482C0
		public LogStringHandler(int literalLength, int formattedCount)
		{
			this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
			this.Values = new Dictionary<string, object>();
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0004A0DA File Offset: 0x000482DA
		public LogStringHandler(int literalLength, int formattedCount, IFormatProvider provider)
		{
			this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider);
			this.Values = new Dictionary<string, object>();
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0004A0F5 File Offset: 0x000482F5
		[NullableContext(0)]
		public LogStringHandler(int literalLength, int formattedCount, [Nullable(2)] IFormatProvider provider, Span<char> initialBuffer)
		{
			this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider, initialBuffer);
			this.Values = new Dictionary<string, object>();
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0004A114 File Offset: 0x00048314
		private void AddFormat<T>(string format, [Nullable(1)] T value, string argument = null)
		{
			if (format == null)
			{
				if (argument == null)
				{
					return;
				}
				format = ((argument[0] == '@') ? argument.Substring(1, argument.Length - 1) : argument);
			}
			if (this.Values.TryAdd(format, value) || this.Values[format] == value)
			{
				return;
			}
			string originalFormat = format;
			int i = 2;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(originalFormat);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<int>(i);
			format = defaultInterpolatedStringHandler.ToStringAndClear();
			while (!this.Values.TryAdd(format, value))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(originalFormat);
				defaultInterpolatedStringHandler.AppendLiteral("_");
				defaultInterpolatedStringHandler.AppendFormatted<int>(i);
				format = defaultInterpolatedStringHandler.ToStringAndClear();
				i++;
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0004A1EC File Offset: 0x000483EC
		[NullableContext(1)]
		public void AppendLiteral(string value)
		{
			this._handler.AppendLiteral(value);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0004A1FA File Offset: 0x000483FA
		public void AppendFormatted<T>([Nullable(1)] T value, [CallerArgumentExpression("value")] string argument = null)
		{
			this.AddFormat<T>(null, value, argument);
			this._handler.AppendFormatted<T>(value);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0004A211 File Offset: 0x00048411
		public void AppendFormatted<T>([Nullable(1)] T value, string format, [CallerArgumentExpression("value")] string argument = null)
		{
			this.AddFormat<T>(format, value, argument);
			this._handler.AppendFormatted<T>(value, format);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0004A229 File Offset: 0x00048429
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, [CallerArgumentExpression("value")] string argument = null)
		{
			this.AddFormat<T>(null, value, argument);
			this._handler.AppendFormatted<T>(value, alignment);
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0004A241 File Offset: 0x00048441
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format, [CallerArgumentExpression("value")] string argument = null)
		{
			this.AddFormat<T>(format, value, argument);
			this._handler.AppendFormatted<T>(value, alignment, format);
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x0004A25B File Offset: 0x0004845B
		[NullableContext(0)]
		public void AppendFormatted(ReadOnlySpan<char> value)
		{
			this._handler.AppendFormatted(value);
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0004A269 File Offset: 0x00048469
		[NullableContext(0)]
		public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, [Nullable(2)] string format = null)
		{
			this.AddFormat<string>(format, value.ToString(), null);
			this._handler.AppendFormatted(value, alignment, format);
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0004A28E File Offset: 0x0004848E
		public void AppendFormatted(string value)
		{
			this._handler.AppendFormatted(value);
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0004A29C File Offset: 0x0004849C
		public void AppendFormatted(string value, int alignment = 0, string format = null)
		{
			this.AddFormat<string>(format, value, null);
			this._handler.AppendFormatted(value, alignment, format);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0004A2B5 File Offset: 0x000484B5
		public void AppendFormatted(object value, int alignment = 0, string format = null)
		{
			this.AddFormat<object>(null, value, format);
			this._handler.AppendFormatted(value, alignment, format);
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x0004A2CE File Offset: 0x000484CE
		[NullableContext(1)]
		public string ToStringAndClear()
		{
			this.Values.Clear();
			return this._handler.ToStringAndClear();
		}

		// Token: 0x040016EA RID: 5866
		private DefaultInterpolatedStringHandler _handler;

		// Token: 0x040016EB RID: 5867
		[Nullable(new byte[]
		{
			1,
			1,
			2
		})]
		public readonly Dictionary<string, object> Values;
	}
}
