using System;
using System.Collections.Generic;
namespace csv_read
{
    //S
    class Program
    {
        static string[] whose = { "A", "B", "C", "D", };

        static void Main(string[] args)
        {
            Dictionary<string, int> lbl 
            = new Dictionary<string, int> { { "no", 0 }, { "date", 1 }, { "whose", 2 }, { "shop", 3 }, { "item", 4 },{ "price", 5 } };

            try
            {
                //lbls lbl = new lbls();
                List<string>[] shopList = new List<string>[whose.Length];
                List<Dictionary<string, int>>[] outList = new List<Dictionary<string, int>>[whose.Length];
               

                for (int i = 0; i < whose.Length; i++)
                {
                
                    shopList[i] = new List<string>();
                    outList[i] = new List<Dictionary<string, int>>();

                }
                string DirectoryPath = "read_csv";
                string[] files = System.IO.Directory.GetFiles(DirectoryPath);

                for (int fi = 0; fi < files.Length; fi++)
                {
                    if (!files[fi].Split('.')[1].Equals("csv")) continue;
                    // csvファイルを開く
                    using (var sr = new System.IO.StreamReader(files[fi]))
                    {
                        Console.WriteLine("filename=" + files[fi]);
                        // ストリームの末尾まで繰り返す
                        while (!sr.EndOfStream)
                        {
                            // ファイルから一行読み込む
                            string line = sr.ReadLine();
                            // 読み込んだ一行をカンマ毎に分けて配列に格納する
                            string[] values = line.Split(',');

                            //who取得
                            int whoseIndex = Array.IndexOf(whose, values[lbl["whose"]]);
                            if (whoseIndex < 0) continue;//最初のラベル除く
                            int shopIndex = shopList[whoseIndex].IndexOf(values[lbl["shop"]]);

                            // 登録済みの場合
                            if (shopIndex > -1)
                            {
                                if (outList[whoseIndex][shopIndex].ContainsKey(values[lbl["item"]]))
                                    outList[whoseIndex][shopIndex][values[lbl["item"]]] += int.Parse(values[lbl["price"]]);
                                else
                                    outList[whoseIndex][shopIndex].Add(values[lbl["item"]], int.Parse(values[lbl["price"]]));
                                
                            }
                            else
                            {
                                shopList[whoseIndex].Insert(shopList[whoseIndex].Count, values[lbl["shop"]]);

                                outList[whoseIndex].Insert(outList[whoseIndex].Count,
                                                           new Dictionary<string, int> { { values[lbl["item"]], int.Parse(values[lbl["price"]]) } });

                            }


                        }
                    }
                }
           
                WriteCsv(outList, shopList);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
                                
         

        }

        private static void WriteCsv(List<Dictionary<string,int>>[] list, List<string>[] shopList)
        {
            try
            {
                // appendをtrueにすると，既存のファイルに追記
                // falseにすると，ファイルを新規作成する
                var append = false;
                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(@"out.csv", append))
                {
                    
                    sw.WriteLine("whose, shop, item, price");
                    bool ic, jc;
                    for (int i = 0; i < list.Length; ++i)
                    {
                        ic = true;
                        for (int j = 0; j < list[i].Count; ++j)
                        {
                            jc = true;
                            foreach (var dic in list[i][j])
                            {

                                sw.WriteLine(string.Format("{0}, {1}, {2}, {3}", ic ? whose[i]:"" , jc ? shopList[i][j] : "",dic.Key, dic.Value));
                                ic = false;
                                jc = false;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}