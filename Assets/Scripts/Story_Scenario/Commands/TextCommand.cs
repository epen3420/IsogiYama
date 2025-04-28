using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class TextCommand : CommandBase
    {
        TextWindows textWindows;

        public TextCommand()
        {
            textWindows = InstanceRegister.Get<TextWindows>();
        }

        public override async UniTask ExecuteAsync(ScenarioLineData lineData)
        {
            string names = lineData.GetData<string>(ScenarioFields.Arg1);
            string body = lineData.GetData<string>(ScenarioFields.Text);
            int interval = lineData.GetData<int>(ScenarioFields.Arg2);
            int threshold = lineData.GetData<int>(ScenarioFields.Arg3);

            await textWindows.DisplayTextAsync(
                    names,
                    body,
                    interval,
                    threshold
                );
        }
    }
}