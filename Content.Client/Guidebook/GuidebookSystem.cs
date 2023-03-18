using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Guidebook.Controls;
using Content.Client.Light;
using Content.Client.Verbs;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Light.Component;
using Content.Shared.Speech;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook
{
	// Token: 0x020002E7 RID: 743
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GuidebookSystem : EntitySystem
	{
		// Token: 0x060012C8 RID: 4808 RVA: 0x0007019C File Offset: 0x0006E39C
		public override void Initialize()
		{
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenGuidebook, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.HandleOpenGuidebook), true, false)).Register<GuidebookSystem>();
			this._guideWindow = new GuidebookWindow();
			base.SubscribeLocalEvent<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<GuideHelpComponent, GetVerbsEvent<ExamineVerb>>(this.OnGetVerbs), null, null);
			base.SubscribeLocalEvent<GuidebookControlsTestComponent, InteractHandEvent>(new ComponentEventHandler<GuidebookControlsTestComponent, InteractHandEvent>(this.OnGuidebookControlsTestInteractHand), null, null);
			base.SubscribeLocalEvent<GuidebookControlsTestComponent, ActivateInWorldEvent>(new ComponentEventHandler<GuidebookControlsTestComponent, ActivateInWorldEvent>(this.OnGuidebookControlsTestActivateInWorld), null, null);
			base.SubscribeLocalEvent<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<GuidebookControlsTestComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGuidebookControlsTestGetAlternateVerbs), null, null);
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x0007022C File Offset: 0x0006E42C
		private void OnGetVerbs(EntityUid uid, GuideHelpComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (component.Guides.Count == 0 || this._tags.HasTag(uid, "GuideEmbeded"))
			{
				return;
			}
			args.Verbs.Add(new ExamineVerb
			{
				Text = Loc.GetString("guide-help-verb"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/information.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.OpenGuidebook(component.Guides, null, null, component.IncludeChildren, component.Guides[0]);
				},
				ClientExclusive = true,
				CloseMenu = new bool?(true)
			});
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x000702D4 File Offset: 0x0006E4D4
		private void OnGuidebookControlsTestGetAlternateVerbs(EntityUid uid, GuidebookControlsTestComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate()
				{
					if (this.Transform(uid).LocalRotation != Angle.Zero)
					{
						this.Transform(uid).LocalRotation -= Angle.FromDegrees(90.0);
					}
				},
				Text = Loc.GetString("guidebook-monkey-unspin"),
				Priority = -9999
			});
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate()
				{
					this.EnsureComp<PointLightComponent>(uid).Enabled = false;
					RgbLightControllerComponent rgb = this.EnsureComp<RgbLightControllerComponent>(uid);
					SpriteComponent spriteComponent = this.EnsureComp<SpriteComponent>(uid);
					List<int> list = new List<int>();
					for (int i = 0; i < spriteComponent.AllLayers.Count<ISpriteLayer>(); i++)
					{
						list.Add(i);
					}
					this._rgbLightControllerSystem.SetLayers(uid, list, rgb);
				},
				Text = Loc.GetString("guidebook-monkey-disco"),
				Priority = -9998
			});
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x00070371 File Offset: 0x0006E571
		private void OnGuidebookControlsTestActivateInWorld(EntityUid uid, GuidebookControlsTestComponent component, ActivateInWorldEvent args)
		{
			base.Transform(uid).LocalRotation += Angle.FromDegrees(90.0);
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x00070398 File Offset: 0x0006E598
		private void OnGuidebookControlsTestInteractHand(EntityUid uid, GuidebookControlsTestComponent component, InteractHandEvent args)
		{
			SpeechComponent speechComponent;
			if (!base.TryComp<SpeechComponent>(uid, ref speechComponent) || speechComponent.SpeechSounds == null)
			{
				return;
			}
			this._audioSystem.PlayGlobal(speechComponent.SpeechSounds, Filter.Local(), false, new AudioParams?(speechComponent.AudioParams));
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x000703DC File Offset: 0x0006E5DC
		public void FakeClientActivateInWorld(EntityUid activated)
		{
			EntityUid? controlledEntity = this._playerManager.LocalPlayer.ControlledEntity;
			if (controlledEntity == null)
			{
				return;
			}
			ActivateInWorldEvent activateInWorldEvent = new ActivateInWorldEvent(controlledEntity.Value, activated);
			base.RaiseLocalEvent<ActivateInWorldEvent>(activated, activateInWorldEvent, true);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x0007041C File Offset: 0x0006E61C
		public void FakeClientAltActivateInWorld(EntityUid activated)
		{
			EntityUid? controlledEntity = this._playerManager.LocalPlayer.ControlledEntity;
			if (controlledEntity == null)
			{
				return;
			}
			SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(activated, controlledEntity.Value, typeof(AlternativeVerb), false);
			if (!localVerbs.Any<Verb>())
			{
				return;
			}
			this._verbSystem.ExecuteVerb(localVerbs.First<Verb>(), controlledEntity.Value, activated, false);
		}

		// Token: 0x060012CF RID: 4815 RVA: 0x00070488 File Offset: 0x0006E688
		public void FakeClientUse(EntityUid activated)
		{
			InteractHandEvent interactHandEvent = new InteractHandEvent(this._playerManager.LocalPlayer.ControlledEntity ?? EntityUid.Invalid, activated);
			base.RaiseLocalEvent<InteractHandEvent>(activated, interactHandEvent, true);
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x000704CD File Offset: 0x0006E6CD
		private bool HandleOpenGuidebook(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (args.State != 1)
			{
				return false;
			}
			this.OpenGuidebook(null, null, null, true, null);
			return true;
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x000704E8 File Offset: 0x0006E6E8
		[NullableContext(2)]
		public bool OpenGuidebook([Nullable(new byte[]
		{
			2,
			1,
			1
		})] Dictionary<string, GuideEntry> guides = null, [Nullable(new byte[]
		{
			2,
			1
		})] List<string> rootEntries = null, string forceRoot = null, bool includeChildren = true, string selected = null)
		{
			this._guideWindow.OpenCenteredRight();
			if (guides == null)
			{
				GetGuidesEvent getGuidesEvent = new GetGuidesEvent();
				getGuidesEvent.Guides = this._prototypeManager.EnumeratePrototypes<GuideEntryPrototype>().ToDictionary((GuideEntryPrototype x) => x.ID, (GuideEntryPrototype x) => x);
				GetGuidesEvent getGuidesEvent2 = getGuidesEvent;
				base.RaiseLocalEvent<GetGuidesEvent>(getGuidesEvent2);
				guides = getGuidesEvent2.Guides;
			}
			else if (includeChildren)
			{
				Dictionary<string, GuideEntry> dictionary = guides;
				guides = new Dictionary<string, GuideEntry>(dictionary);
				foreach (GuideEntry guide in dictionary.Values)
				{
					this.RecursivelyAddChildren(guide, guides);
				}
			}
			this._guideWindow.UpdateGuides(guides, rootEntries, forceRoot, selected);
			return true;
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x000705D4 File Offset: 0x0006E7D4
		[NullableContext(2)]
		public bool OpenGuidebook([Nullable(1)] List<string> guideList, [Nullable(new byte[]
		{
			2,
			1
		})] List<string> rootEntries = null, string forceRoot = null, bool includeChildren = true, string selected = null)
		{
			Dictionary<string, GuideEntry> dictionary = new Dictionary<string, GuideEntry>();
			foreach (string text in guideList)
			{
				GuideEntryPrototype value;
				if (!this._prototypeManager.TryIndex<GuideEntryPrototype>(text, ref value))
				{
					Logger.Error("Encountered unknown guide prototype: " + text);
				}
				else
				{
					dictionary.Add(text, value);
				}
			}
			return this.OpenGuidebook(dictionary, rootEntries, forceRoot, includeChildren, selected);
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x00070658 File Offset: 0x0006E858
		private void RecursivelyAddChildren(GuideEntry guide, Dictionary<string, GuideEntry> guides)
		{
			foreach (string text in guide.Children)
			{
				if (!guides.ContainsKey(text))
				{
					GuideEntryPrototype guideEntryPrototype;
					if (!this._prototypeManager.TryIndex<GuideEntryPrototype>(text, ref guideEntryPrototype))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(116, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Encountered unknown guide prototype: ");
						defaultInterpolatedStringHandler.AppendFormatted(text);
						defaultInterpolatedStringHandler.AppendLiteral(" as a child of ");
						defaultInterpolatedStringHandler.AppendFormatted(guide.Id);
						defaultInterpolatedStringHandler.AppendLiteral(". If the child is not a prototype, it must be directly provided.");
						Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						guides.Add(text, guideEntryPrototype);
						this.RecursivelyAddChildren(guideEntryPrototype, guides);
					}
				}
			}
		}

		// Token: 0x04000967 RID: 2407
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000968 RID: 2408
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000969 RID: 2409
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x0400096A RID: 2410
		[Dependency]
		private readonly VerbSystem _verbSystem;

		// Token: 0x0400096B RID: 2411
		[Dependency]
		private readonly RgbLightControllerSystem _rgbLightControllerSystem;

		// Token: 0x0400096C RID: 2412
		[Dependency]
		private readonly TagSystem _tags;

		// Token: 0x0400096D RID: 2413
		private GuidebookWindow _guideWindow;

		// Token: 0x0400096E RID: 2414
		public const string GuideEmbedTag = "GuideEmbeded";
	}
}
