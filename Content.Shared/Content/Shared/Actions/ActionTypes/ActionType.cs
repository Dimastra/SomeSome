using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Actions.ActionTypes
{
	// Token: 0x02000767 RID: 1895
	[NullableContext(2)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	[NetSerializable]
	[Serializable]
	public abstract class ActionType : IEquatable<ActionType>, IComparable, ICloneable
	{
		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x0004BCA8 File Offset: 0x00049EA8
		// (set) Token: 0x06001753 RID: 5971 RVA: 0x0004BCCD File Offset: 0x00049ECD
		public EntityUid? EntityIcon
		{
			get
			{
				EntityUid? entityIcon = this._entityIcon;
				if (entityIcon == null)
				{
					return this.Provider;
				}
				return entityIcon;
			}
			set
			{
				this._entityIcon = value;
			}
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0004BCD8 File Offset: 0x00049ED8
		public virtual int CompareTo(object obj)
		{
			ActionType otherAction = obj as ActionType;
			if (otherAction == null)
			{
				return -1;
			}
			if (this.Priority != otherAction.Priority)
			{
				return otherAction.Priority - this.Priority;
			}
			string name = FormattedMessage.RemoveMarkup(Loc.GetString(this.DisplayName));
			string otherName = FormattedMessage.RemoveMarkup(Loc.GetString(otherAction.DisplayName));
			if (name != otherName)
			{
				return string.Compare(name, otherName, StringComparison.CurrentCulture);
			}
			if (!(this.Provider != otherAction.Provider))
			{
				return 0;
			}
			if (this.Provider == null)
			{
				return -1;
			}
			if (otherAction.Provider == null)
			{
				return 1;
			}
			return (int)this.Provider.Value - (int)otherAction.Provider.Value;
		}

		// Token: 0x06001755 RID: 5973
		[NullableContext(1)]
		public abstract object Clone();

		// Token: 0x06001756 RID: 5974 RVA: 0x0004BDC8 File Offset: 0x00049FC8
		[NullableContext(1)]
		public virtual void CopyFrom(object objectToClone)
		{
			ActionType toClone = objectToClone as ActionType;
			if (toClone == null)
			{
				return;
			}
			this.Priority = toClone.Priority;
			this.Icon = toClone.Icon;
			this.IconOn = toClone.IconOn;
			this.DisplayName = toClone.DisplayName;
			this.Description = toClone.Description;
			this.Provider = toClone.Provider;
			this.AttachedEntity = toClone.AttachedEntity;
			this.Enabled = toClone.Enabled;
			this.Toggled = toClone.Toggled;
			this.Cooldown = toClone.Cooldown;
			this.Charges = toClone.Charges;
			this.Keywords = new HashSet<string>(toClone.Keywords);
			this.AutoPopulate = toClone.AutoPopulate;
			this.AutoRemove = toClone.AutoRemove;
			this.ItemIconStyle = toClone.ItemIconStyle;
			this.CheckCanInteract = toClone.CheckCanInteract;
			this.Speech = toClone.Speech;
			this.UseDelay = toClone.UseDelay;
			this.Sound = toClone.Sound;
			this.AudioParams = toClone.AudioParams;
			this.UserPopup = toClone.UserPopup;
			this.Popup = toClone.Popup;
			this.PopupToggleSuffix = toClone.PopupToggleSuffix;
			this.ItemIconStyle = toClone.ItemIconStyle;
			this._entityIcon = toClone._entityIcon;
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x0004BF11 File Offset: 0x0004A111
		public bool Equals(ActionType other)
		{
			return this.CompareTo(other) == 0;
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x0004BF1D File Offset: 0x0004A11D
		public override int GetHashCode()
		{
			return (this.Priority.GetHashCode() * 397 ^ this.DisplayName.GetHashCode()) * 397 ^ this.Provider.GetHashCode();
		}

		// Token: 0x0400171A RID: 5914
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon;

		// Token: 0x0400171B RID: 5915
		[DataField("iconOn", false, 1, false, false, null)]
		public SpriteSpecifier IconOn;

		// Token: 0x0400171C RID: 5916
		[DataField("iconColor", false, 1, false, false, null)]
		public Color IconColor = Color.White;

		// Token: 0x0400171D RID: 5917
		[Nullable(1)]
		[DataField("name", false, 1, false, false, null)]
		public string DisplayName = string.Empty;

		// Token: 0x0400171E RID: 5918
		[Nullable(1)]
		[DataField("description", false, 1, false, false, null)]
		public string Description = string.Empty;

		// Token: 0x0400171F RID: 5919
		[Nullable(1)]
		[DataField("keywords", false, 1, false, false, null)]
		public HashSet<string> Keywords = new HashSet<string>();

		// Token: 0x04001720 RID: 5920
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x04001721 RID: 5921
		public bool Toggled;

		// Token: 0x04001722 RID: 5922
		[TupleElementNames(new string[]
		{
			"Start",
			"End"
		})]
		[Nullable(0)]
		public ValueTuple<TimeSpan, TimeSpan>? Cooldown;

		// Token: 0x04001723 RID: 5923
		[DataField("useDelay", false, 1, false, false, null)]
		public TimeSpan? UseDelay;

		// Token: 0x04001724 RID: 5924
		[DataField("charges", false, 1, false, false, null)]
		public int? Charges;

		// Token: 0x04001725 RID: 5925
		public EntityUid? Provider;

		// Token: 0x04001726 RID: 5926
		private EntityUid? _entityIcon;

		// Token: 0x04001727 RID: 5927
		[DataField("checkCanInteract", false, 1, false, false, null)]
		public bool CheckCanInteract = true;

		// Token: 0x04001728 RID: 5928
		[DataField("clientExclusive", false, 1, false, false, null)]
		public bool ClientExclusive;

		// Token: 0x04001729 RID: 5929
		[DataField("priority", false, 1, false, false, null)]
		public int Priority;

		// Token: 0x0400172A RID: 5930
		[ViewVariables]
		public EntityUid? AttachedEntity;

		// Token: 0x0400172B RID: 5931
		[DataField("autoPopulate", false, 1, false, false, null)]
		public bool AutoPopulate = true;

		// Token: 0x0400172C RID: 5932
		[DataField("autoRemove", false, 1, false, false, null)]
		public bool AutoRemove = true;

		// Token: 0x0400172D RID: 5933
		[DataField("temporary", false, 1, false, false, null)]
		public bool Temporary;

		// Token: 0x0400172E RID: 5934
		[DataField("itemIconStyle", false, 1, false, false, null)]
		public ItemActionIconStyle ItemIconStyle;

		// Token: 0x0400172F RID: 5935
		[DataField("speech", false, 1, false, false, null)]
		public string Speech;

		// Token: 0x04001730 RID: 5936
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound;

		// Token: 0x04001731 RID: 5937
		[DataField("audioParams", false, 1, false, false, null)]
		public AudioParams? AudioParams;

		// Token: 0x04001732 RID: 5938
		[DataField("userPopup", false, 1, false, false, null)]
		public string UserPopup;

		// Token: 0x04001733 RID: 5939
		[DataField("popup", false, 1, false, false, null)]
		public string Popup;

		// Token: 0x04001734 RID: 5940
		[DataField("popupToggleSuffix", false, 1, false, false, null)]
		public string PopupToggleSuffix;
	}
}
