using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.CrewManifest;
using Content.Client.GameTicking.Managers;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.LateJoin
{
	// Token: 0x02000285 RID: 645
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LateJoinGui : DefaultWindow
	{
		// Token: 0x1400005F RID: 95
		// (add) Token: 0x0600107F RID: 4223 RVA: 0x00062A04 File Offset: 0x00060C04
		// (remove) Token: 0x06001080 RID: 4224 RVA: 0x00062A3C File Offset: 0x00060C3C
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public event Action<ValueTuple<EntityUid, string>> SelectedId;

		// Token: 0x06001081 RID: 4225 RVA: 0x00062A74 File Offset: 0x00060C74
		public LateJoinGui()
		{
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(360f, 560f));
			IoCManager.InjectDependencies<LateJoinGui>(this);
			this._sprites = this._entitySystem.GetEntitySystem<SpriteSystem>();
			this._crewManifest = this._entitySystem.GetEntitySystem<CrewManifestSystem>();
			this._gameTicker = this._entitySystem.GetEntitySystem<ClientGameTicker>();
			base.Title = Loc.GetString("late-join-gui-title");
			this._base = new BoxContainer
			{
				Orientation = 1,
				VerticalExpand = true
			};
			base.Contents.AddChild(this._base);
			this.RebuildUI();
			this.SelectedId += delegate([Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<EntityUid, string> x)
			{
				EntityUid item = x.Item1;
				string item2 = x.Item2;
				Logger.InfoS("latejoin", "Late joining as ID: " + item2);
				IConsoleHost consoleHost = this._consoleHost;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("joingame ");
				defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(item2));
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(item);
				consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
				this.Close();
			};
			this._gameTicker.LobbyJobsAvailableUpdated += this.JobsAvailableUpdated;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x00062B70 File Offset: 0x00060D70
		private void RebuildUI()
		{
			this._base.RemoveAllChildren();
			this._jobLists.Clear();
			this._jobButtons.Clear();
			this._jobCategories.Clear();
			if (!this._gameTicker.DisallowedLateJoin && this._gameTicker.StationNames.Count == 0)
			{
				Logger.Warning("No stations exist, nothing to display in late-join GUI");
			}
			using (IEnumerator<KeyValuePair<EntityUid, string>> enumerator = this._gameTicker.StationNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LateJoinGui.<>c__DisplayClass16_0 CS$<>8__locals1 = new LateJoinGui.<>c__DisplayClass16_0();
					CS$<>8__locals1.<>4__this = this;
					KeyValuePair<EntityUid, string> keyValuePair = enumerator.Current;
					EntityUid id;
					string text;
					keyValuePair.Deconstruct(out id, out text);
					CS$<>8__locals1.id = id;
					string text2 = text;
					LateJoinGui.<>c__DisplayClass16_1 CS$<>8__locals2 = new LateJoinGui.<>c__DisplayClass16_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					BoxContainer boxContainer = new BoxContainer
					{
						Orientation = 1,
						Margin = new Thickness(0f, 0f, 5f, 0f)
					};
					ContainerButton containerButton = new ContainerButton();
					containerButton.HorizontalAlignment = 3;
					containerButton.ToggleMode = true;
					containerButton.Children.Add(new TextureRect
					{
						StyleClasses = 
						{
							"optionTriangle"
						},
						Margin = new Thickness(8f, 0f),
						HorizontalAlignment = 2,
						VerticalAlignment = 2
					});
					ContainerButton containerButton2 = containerButton;
					Control @base = this._base;
					StripeBack stripeBack = new StripeBack();
					Control.OrderedChildCollection children = stripeBack.Children;
					PanelContainer panelContainer = new PanelContainer();
					panelContainer.Children.Add(new Label
					{
						StyleClasses = 
						{
							"LabelBig"
						},
						Text = text2,
						Align = 1
					});
					panelContainer.Children.Add(containerButton2);
					children.Add(panelContainer);
					@base.AddChild(stripeBack);
					if (this._configManager.GetCVar<bool>(CCVars.CrewManifestWithoutEntity))
					{
						Button button = new Button
						{
							Text = Loc.GetString("crew-manifest-button-label")
						};
						button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this._crewManifest.RequestCrewManifest(CS$<>8__locals2.CS$<>8__locals1.id);
						};
						this._base.AddChild(button);
					}
					LateJoinGui.<>c__DisplayClass16_1 CS$<>8__locals3 = CS$<>8__locals2;
					ScrollContainer scrollContainer = new ScrollContainer();
					scrollContainer.VerticalExpand = true;
					scrollContainer.Children.Add(boxContainer);
					scrollContainer.Visible = false;
					CS$<>8__locals3.jobListScroll = scrollContainer;
					if (this._jobLists.Count == 0)
					{
						CS$<>8__locals2.jobListScroll.Visible = true;
					}
					this._jobLists.Add(CS$<>8__locals2.jobListScroll);
					this._base.AddChild(CS$<>8__locals2.jobListScroll);
					containerButton2.OnToggled += delegate(BaseButton.ButtonToggledEventArgs _)
					{
						foreach (ScrollContainer scrollContainer2 in CS$<>8__locals2.CS$<>8__locals1.<>4__this._jobLists)
						{
							scrollContainer2.Visible = false;
						}
						CS$<>8__locals2.jobListScroll.Visible = true;
					};
					bool flag = true;
					foreach (DepartmentPrototype departmentPrototype in this._prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
					{
						string @string = Loc.GetString("department-" + departmentPrototype.ID);
						this._jobCategories[CS$<>8__locals2.CS$<>8__locals1.id] = new Dictionary<string, BoxContainer>();
						this._jobButtons[CS$<>8__locals2.CS$<>8__locals1.id] = new Dictionary<string, JobButton>();
						Dictionary<string, uint?> dictionary = this._gameTicker.JobsAvailable[CS$<>8__locals2.CS$<>8__locals1.id];
						List<JobPrototype> list = new List<JobPrototype>();
						foreach (string text3 in departmentPrototype.Roles)
						{
							if (dictionary.ContainsKey(text3))
							{
								list.Add(this._prototypeManager.Index<JobPrototype>(text3));
							}
						}
						list.Sort((JobPrototype x, JobPrototype y) => -string.Compare(x.LocalizedName, y.LocalizedName, StringComparison.CurrentCultureIgnoreCase));
						if (list.Count != 0)
						{
							BoxContainer boxContainer2 = new BoxContainer
							{
								Orientation = 1,
								Name = departmentPrototype.ID,
								ToolTip = Loc.GetString("late-join-gui-jobs-amount-in-department-tooltip", new ValueTuple<string, object>[]
								{
									new ValueTuple<string, object>("departmentName", @string)
								})
							};
							if (flag)
							{
								flag = false;
							}
							else
							{
								boxContainer2.AddChild(new Control
								{
									MinSize = new Vector2(0f, 23f)
								});
							}
							Control control = boxContainer2;
							PanelContainer panelContainer2 = new PanelContainer();
							panelContainer2.Children.Add(new Label
							{
								StyleClasses = 
								{
									"LabelBig"
								},
								Text = Loc.GetString("late-join-gui-department-jobs-label", new ValueTuple<string, object>[]
								{
									new ValueTuple<string, object>("departmentName", @string)
								})
							});
							control.AddChild(panelContainer2);
							this._jobCategories[CS$<>8__locals2.CS$<>8__locals1.id][departmentPrototype.ID] = boxContainer2;
							boxContainer.AddChild(boxContainer2);
							foreach (JobPrototype jobPrototype in list)
							{
								uint? num = dictionary[jobPrototype.ID];
								JobButton jobButton = new JobButton(jobPrototype.ID, num);
								BoxContainer boxContainer3 = new BoxContainer
								{
									Orientation = 0,
									HorizontalExpand = true
								};
								TextureRect textureRect = new TextureRect
								{
									TextureScale = new ValueTuple<float, float>(2f, 2f),
									Stretch = 4
								};
								SpriteSpecifier.Rsi rsi = new SpriteSpecifier.Rsi(new ResourcePath("/Textures/Interface/Misc/job_icons.rsi", "/"), jobPrototype.Icon);
								textureRect.Texture = this._sprites.Frame0(rsi);
								boxContainer3.AddChild(textureRect);
								Label label = new Label
								{
									Margin = new Thickness(5f, 0f, 0f, 0f),
									Text = ((num != null) ? Loc.GetString("late-join-gui-job-slot-capped", new ValueTuple<string, object>[]
									{
										new ValueTuple<string, object>("jobName", jobPrototype.LocalizedName),
										new ValueTuple<string, object>("amount", num)
									}) : Loc.GetString("late-join-gui-job-slot-uncapped", new ValueTuple<string, object>[]
									{
										new ValueTuple<string, object>("jobName", jobPrototype.LocalizedName)
									}))
								};
								boxContainer3.AddChild(label);
								jobButton.AddChild(boxContainer3);
								boxContainer2.AddChild(jobButton);
								jobButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
								{
									CS$<>8__locals2.CS$<>8__locals1.<>4__this.SelectedId(new ValueTuple<EntityUid, string>(CS$<>8__locals2.CS$<>8__locals1.id, jobButton.JobId));
								};
								string text4;
								if (!this._playTimeTracking.IsAllowed(jobPrototype, out text4))
								{
									jobButton.Disabled = true;
									if (!string.IsNullOrEmpty(text4))
									{
										jobButton.ToolTip = text4;
									}
									boxContainer3.AddChild(new TextureRect
									{
										TextureScale = new ValueTuple<float, float>(0.4f, 0.4f),
										Stretch = 4,
										Texture = this._sprites.Frame0(new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/Nano/lock.svg.192dpi.png", "/"))),
										HorizontalExpand = true,
										HorizontalAlignment = 3
									});
								}
								else
								{
									uint? num2 = num;
									uint num3 = 0U;
									if (num2.GetValueOrDefault() == num3 & num2 != null)
									{
										jobButton.Disabled = true;
									}
								}
								this._jobButtons[CS$<>8__locals2.CS$<>8__locals1.id][jobPrototype.ID] = jobButton;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x00063330 File Offset: 0x00061530
		private void JobsAvailableUpdated(IReadOnlyDictionary<EntityUid, Dictionary<string, uint?>> _)
		{
			this.RebuildUI();
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x00063338 File Offset: 0x00061538
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this._gameTicker.LobbyJobsAvailableUpdated -= this.JobsAvailableUpdated;
				this._jobButtons.Clear();
				this._jobCategories.Clear();
			}
		}

		// Token: 0x0400081C RID: 2076
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400081D RID: 2077
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x0400081E RID: 2078
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x0400081F RID: 2079
		[Dependency]
		private readonly IEntitySystemManager _entitySystem;

		// Token: 0x04000820 RID: 2080
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;

		// Token: 0x04000822 RID: 2082
		private readonly ClientGameTicker _gameTicker;

		// Token: 0x04000823 RID: 2083
		private readonly SpriteSystem _sprites;

		// Token: 0x04000824 RID: 2084
		private readonly CrewManifestSystem _crewManifest;

		// Token: 0x04000825 RID: 2085
		private readonly Dictionary<EntityUid, Dictionary<string, JobButton>> _jobButtons = new Dictionary<EntityUid, Dictionary<string, JobButton>>();

		// Token: 0x04000826 RID: 2086
		private readonly Dictionary<EntityUid, Dictionary<string, BoxContainer>> _jobCategories = new Dictionary<EntityUid, Dictionary<string, BoxContainer>>();

		// Token: 0x04000827 RID: 2087
		private readonly List<ScrollContainer> _jobLists = new List<ScrollContainer>();

		// Token: 0x04000828 RID: 2088
		private readonly Control _base;
	}
}
