using CSV4Unity;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace IsogiYama.Commands
{
    public class HideBoxCommand : CommandBase
    {
        TextWindows textWindows;

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData)
        {
            textWindows = InstanceRegister.Get<TextWindows>();
            textWindows.HideBubble();

            await UniTask.Yield();
        }
    }
}
