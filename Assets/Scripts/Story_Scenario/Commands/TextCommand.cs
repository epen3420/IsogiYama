using CSV4Unity;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace IsogiYama.Commands
{
    public class TextCommand : CommandBase
    {
        TextWindows textWindows;
        private readonly CancellationToken lifetimeToken;

        public TextCommand()
        {
            textWindows = InstanceRegister.Get<TextWindows>();

            lifetimeToken = textWindows.GetCancellationTokenOnDestroy();
        }

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            string names = lineData[ScenarioFields.Arg1].Get<string>();
            string body = lineData[ScenarioFields.Text].Get<string>();
            int interval = lineData[ScenarioFields.Arg2].Get<int>();
            int threshold = lineData[ScenarioFields.Arg3].Get<int>();

            await textWindows.DisplayTextAsync(
                    names,
                    body,
                    interval,
                    threshold,
                    lifetimeToken
                );
        }
    }
}
