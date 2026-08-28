//データ構造を追記するときはここに

using CSV4Unity;
using CSV4Unity.Validation;

[CsvSchema]
public enum ScenarioFields
{
    [AllowedValues(
        "",
        "Text",
        "PlayBGM",
        "PlaySE",
        "StopBGM",
        "PauseBGM",
        "SetVol",
        "Goto",
        "Wait",
        "Button",
        "Bg",
        "HideBox",
        "#Bloom",
        "#FilmGrain",
        "#Chromatic",
        "#DepthF",
        "#Distortion",
        "#Vintage",
        "#Glitch",
        "#Shake",
        "#Fade")]
    Command,

    [Condition(1, ScenarioFields.Command, Compare.Equal, "Wait")]
    [NotNull(ConditionGroup = 1)]
    [TypeConstraint(typeof(int), ConditionGroup = 1)]
    [Condition(2, ScenarioFields.Command, Compare.Equal, "Bg")]
    [NotNull(ConditionGroup = 2)]
    [Condition(3, ScenarioFields.Command, Compare.Equal, "#Fade")]
    [NotNull(ConditionGroup = 3)]
    [TypeConstraint(typeof(float), ConditionGroup = 3)]
    [Condition(4, ScenarioFields.Command, Compare.Equal, "#Shake")]
    [NotNull(ConditionGroup = 4)]
    [TypeConstraint(typeof(int), ConditionGroup = 4)]
    Arg1,

    [Condition(1, ScenarioFields.Command, Compare.Equal, "Text")]
    [NotNull(ConditionGroup = 1)]
    [TypeConstraint(typeof(int), ConditionGroup = 1)]
    [Condition(2, ScenarioFields.Command, Compare.Equal, "Bg")]
    [NotNull(ConditionGroup = 2)]
    [TypeConstraint(typeof(float), ConditionGroup = 2)]
    [Condition(3, ScenarioFields.Command, Compare.Equal, "#Shake")]
    [NotNull(ConditionGroup = 3)]
    [TypeConstraint(typeof(int), ConditionGroup = 3)]
    Arg2,

    [Condition(ScenarioFields.Command, Compare.Equal, "Text")]
    [NotNull]
    [TypeConstraint(typeof(int))]
    Arg3,

    [Condition(ScenarioFields.Command, Compare.In, "#Bloom", "#Glitch")]
    [NotNull]
    [AllowedValues("enable", "disable")]
    Arg4,
    Arg5,
    Arg6,
    WaitType,
    [Condition(ScenarioFields.Command, Compare.Equal, "Text")]
    [NotNull]
    Text,

    [Condition(1, ScenarioFields.Command, Compare.Equal, "Goto")]
    [NotNull(ConditionGroup = 1)]
    [TypeConstraint(typeof(int), ConditionGroup = 1)]
    [Condition(2, ScenarioFields.Command, Compare.In, "Bg", "#Shake")]
    [AllowedValues("", "instant", ConditionGroup = 2)]
    PageCtrl,
    Voice,
    WindowType,
    English
}
