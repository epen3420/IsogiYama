namespace IsogiYama.Logger
{/// <summary>
 /// スプシのURLを含めたラッパークラス
 /// </summary>
 /// <typeparam name="T"></typeparam>
    public class SendLogClass<T>
    {
        public SendLogClass(string sheetURL, T datas)
        {
            this.sheetURL = sheetURL;
            this.datas = datas;
        }

        public string sheetURL;
        public T datas;
    }
}
