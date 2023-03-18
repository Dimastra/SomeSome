using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Database;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Verbs
{
	// Token: 0x02000087 RID: 135
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class Verb : IComparable
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000923F File Offset: 0x0000743F
		public virtual int TypePriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00009242 File Offset: 0x00007442
		public virtual bool CloseMenuDefault
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00009245 File Offset: 0x00007445
		public virtual bool DefaultDoContactInteraction
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00009248 File Offset: 0x00007448
		public int CompareTo(object obj)
		{
			Verb otherVerb = obj as Verb;
			if (otherVerb == null)
			{
				return -1;
			}
			if (this.TypePriority != otherVerb.TypePriority)
			{
				return otherVerb.TypePriority - this.TypePriority;
			}
			if (this.Priority != otherVerb.Priority)
			{
				return otherVerb.Priority - this.Priority;
			}
			VerbCategory category = this.Category;
			string a = (category != null) ? category.Text : null;
			VerbCategory category2 = otherVerb.Category;
			if (a != ((category2 != null) ? category2.Text : null))
			{
				VerbCategory category3 = this.Category;
				string strA = (category3 != null) ? category3.Text : null;
				VerbCategory category4 = otherVerb.Category;
				return string.Compare(strA, (category4 != null) ? category4.Text : null, StringComparison.CurrentCulture);
			}
			if (this.Text != otherVerb.Text)
			{
				return string.Compare(this.Text, otherVerb.Text, StringComparison.CurrentCulture);
			}
			if (!(this.IconEntity != otherVerb.IconEntity))
			{
				SpriteSpecifier icon = this.Icon;
				string strA2 = (icon != null) ? icon.ToString() : null;
				SpriteSpecifier icon2 = otherVerb.Icon;
				return string.Compare(strA2, (icon2 != null) ? icon2.ToString() : null, StringComparison.CurrentCulture);
			}
			if (this.IconEntity == null)
			{
				return -1;
			}
			if (otherVerb.IconEntity == null)
			{
				return 1;
			}
			return this.IconEntity.Value.CompareTo(otherVerb.IconEntity.Value);
		}

		// Token: 0x040001BC RID: 444
		[Nullable(1)]
		public static string DefaultTextStyleClass = "Verb";

		// Token: 0x040001BD RID: 445
		[Nullable(1)]
		public string TextStyleClass = Verb.DefaultTextStyleClass;

		// Token: 0x040001BE RID: 446
		[NonSerialized]
		public Action Act;

		// Token: 0x040001BF RID: 447
		[NonSerialized]
		public object ExecutionEventArgs;

		// Token: 0x040001C0 RID: 448
		[NonSerialized]
		public EntityUid EventTarget = EntityUid.Invalid;

		// Token: 0x040001C1 RID: 449
		[NonSerialized]
		public bool ClientExclusive;

		// Token: 0x040001C2 RID: 450
		[Nullable(1)]
		public string Text = string.Empty;

		// Token: 0x040001C3 RID: 451
		public SpriteSpecifier Icon;

		// Token: 0x040001C4 RID: 452
		public VerbCategory Category;

		// Token: 0x040001C5 RID: 453
		public bool Disabled;

		// Token: 0x040001C6 RID: 454
		public string Message;

		// Token: 0x040001C7 RID: 455
		public int Priority;

		// Token: 0x040001C8 RID: 456
		public EntityUid? IconEntity;

		// Token: 0x040001C9 RID: 457
		public bool? CloseMenu;

		// Token: 0x040001CA RID: 458
		public LogImpact Impact = LogImpact.Low;

		// Token: 0x040001CB RID: 459
		public bool ConfirmationPopup;

		// Token: 0x040001CC RID: 460
		public bool? DoContactInteraction;

		// Token: 0x040001CD RID: 461
		[Nullable(1)]
		public static List<Type> VerbTypes = new List<Type>
		{
			typeof(Verb),
			typeof(InteractionVerb),
			typeof(UtilityVerb),
			typeof(InnateVerb),
			typeof(AlternativeVerb),
			typeof(ActivationVerb),
			typeof(ExamineVerb)
		};
	}
}
