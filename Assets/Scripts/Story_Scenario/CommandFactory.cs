using UnityEngine;
using IsogiYama.Commands;

public class CommandFactory
{
    public CommandBase CreateCommandInstance(string cmdRow)
    {
        CommandBase tmp = null;

        switch (cmdRow)
        {
            case "":

                break;

            case "Text":
                // Text表示
                tmp = new TextCommand();
                break;

            case "Character":
                // 立ち絵表示
                // tmp = new CharacterCommand();
                break;

            case "Hidden":
                // 立ち絵非表示
                // tmp = new HiddenCommand();
                break;

            case "PlayBGM":
                // BGM再生

                break;

            case "PlaySE":
                // SE再生

                break;

            case "StopBGM":
                // BGM停止

                break;

            case "PauseBGM":
                // BGM一時停止

                break;

            case "SetVol":
                // Volumeの設定

                break;

            case "Goto":
                // Indexのスキップ(Ifなどと併用することを想定)
                tmp = new GotoCommand();
                break;

            case "Wait":
                // ms秒待つ
                tmp = new DelayCommand();
                break;

            case "Button":
                // ボタンの生成
                // tmp = new ButtonCommand();
                break;

            default:
                Debug.LogError("Command Was Not Found");
                break;
        }

        return tmp;
    }
}
