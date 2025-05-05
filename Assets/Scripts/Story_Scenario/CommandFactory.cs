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

            case "Bg":
                // 背景の変更
                tmp = new BgCommand();
                break;

            /// PostProcess

            case "Bloom":
                // ブルーム
                tmp = new BloomCommand();
                break;

            case "FilmGrain":
                // ノイズ
                tmp = new BgCommand();
                break;

            case "Chromatic":
                // 色収差
                tmp = new BgCommand();
                break;

            case "DepthF":
                // ガウスぼかし
                tmp = new BgCommand();
                break;

            case "Distortion":
                // カメラひずみ
                tmp = new BgCommand();
                break;

            case "Vintage":
                // 周囲を暗く
                tmp = new BgCommand();
                break;

            case "AllEffect":
                // すべてのポストプロセス
                tmp = new BgCommand();
                break;

            default:
                Debug.LogError("Command Was Not Found");
                break;
        }

        return tmp;
    }
}
