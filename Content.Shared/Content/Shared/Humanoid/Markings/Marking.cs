using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041A RID: 1050
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class Marking : IEquatable<Marking>, IComparable<Marking>, IComparable<string>
	{
		// Token: 0x06000C74 RID: 3188 RVA: 0x00028D32 File Offset: 0x00026F32
		private Marking(string markingId, List<Color> markingColors)
		{
			this.MarkingId = markingId;
			this._markingColors = markingColors;
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x00028D5A File Offset: 0x00026F5A
		public Marking(string markingId, IReadOnlyList<Color> markingColors) : this(markingId, new List<Color>(markingColors))
		{
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00028D6C File Offset: 0x00026F6C
		public Marking(string markingId, int colorCount)
		{
			this.MarkingId = markingId;
			List<Color> colors = new List<Color>();
			for (int i = 0; i < colorCount; i++)
			{
				colors.Add(Color.White);
			}
			this._markingColors = colors;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00028DBC File Offset: 0x00026FBC
		public Marking(Marking other)
		{
			this.MarkingId = other.MarkingId;
			this._markingColors = new List<Color>(other.MarkingColors);
			this.Visible = other.Visible;
			this.Forced = other.Forced;
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x00028E16 File Offset: 0x00027016
		[DataField("markingId", false, 1, false, false, null)]
		public string MarkingId { get; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000C79 RID: 3193 RVA: 0x00028E1E File Offset: 0x0002701E
		[ViewVariables]
		public IReadOnlyList<Color> MarkingColors
		{
			get
			{
				return this._markingColors;
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x00028E26 File Offset: 0x00027026
		public void SetColor(int colorIndex, Color color)
		{
			this._markingColors[colorIndex] = color;
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x00028E35 File Offset: 0x00027035
		[NullableContext(2)]
		public int CompareTo(Marking marking)
		{
			if (marking == null)
			{
				return 1;
			}
			return string.Compare(this.MarkingId, marking.MarkingId, StringComparison.Ordinal);
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00028E4E File Offset: 0x0002704E
		[NullableContext(2)]
		public int CompareTo(string markingId)
		{
			if (markingId == null)
			{
				return 1;
			}
			return string.Compare(this.MarkingId, markingId, StringComparison.Ordinal);
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x00028E64 File Offset: 0x00027064
		[NullableContext(2)]
		public bool Equals(Marking other)
		{
			return other != null && (this.MarkingId.Equals(other.MarkingId) && this._markingColors.SequenceEqual(other._markingColors) && this.Visible.Equals(other.Visible)) && this.Forced.Equals(other.Forced);
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x00028EC4 File Offset: 0x000270C4
		public new string ToString()
		{
			string sanitizedName = this.MarkingId.Replace('@', '_');
			List<string> colorStringList = new List<string>();
			foreach (Color color in this._markingColors)
			{
				colorStringList.Add(color.ToHex());
			}
			return sanitizedName + "@" + string.Join<string>(',', colorStringList);
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00028F48 File Offset: 0x00027148
		[return: Nullable(2)]
		public static Marking ParseFromDbString(string input)
		{
			if (input.Length == 0)
			{
				return null;
			}
			string[] split = input.Split('@', StringSplitOptions.None);
			if (split.Length != 2)
			{
				return null;
			}
			List<Color> colorList = new List<Color>();
			foreach (string color in split[1].Split(',', StringSplitOptions.None))
			{
				colorList.Add(Color.FromHex(color, null));
			}
			return new Marking(split[0], colorList);
		}

		// Token: 0x04000C69 RID: 3177
		[DataField("markingColor", false, 1, false, false, null)]
		private List<Color> _markingColors = new List<Color>();

		// Token: 0x04000C6B RID: 3179
		[DataField("visible", false, 1, false, false, null)]
		public bool Visible = true;

		// Token: 0x04000C6C RID: 3180
		[ViewVariables]
		public bool Forced;
	}
}
