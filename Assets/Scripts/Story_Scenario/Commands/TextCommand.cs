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

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData)
        {
            string names = lineData.Get<string>(ScenarioFields.Arg1);
            string body = lineData.Get<string>(ScenarioFields.Text);
            int interval = lineData.Get<int>(ScenarioFields.Arg2);
            int threshold = lineData.Get<int>(ScenarioFields.Arg3);

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
