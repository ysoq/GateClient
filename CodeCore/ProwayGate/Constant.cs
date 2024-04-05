using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCore.ProwayGate
{
    public class Constant
    {

        //可以进入人数
        public static volatile int comeInCount = 0;

        //可以出去人数
        public static volatile int comeOutCount = 0;

        //刷开方向 1：左手刷开(进、出相反) 2：右手刷开(进、出相同)
        public static volatile int readCardDirect = 2;

        //通行模式
        public static volatile int passModel = 4;

        //消防是否打开
        public static volatile bool fireModel = false;

        //是否打开一键常开
        public static volatile bool isNormalOpen = false;

        //是否停检
        public static volatile bool isStopCheck = false;

        public static void loadFile()
        {
            string fileName = Directory.GetCurrentDirectory() + "\\readCardDirect.ini";
            if (!File.Exists(fileName))
            {
                //没有则创建这个文件
                FileStream fs1 = new FileStream(fileName, FileMode.Create, FileAccess.Write);//创建写入文件                //设置文件属性为隐藏
                File.SetAttributes(fileName, FileAttributes.Hidden);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine("readCardDirect:2");//开始写入值
                sw.Close();
                fs1.Close();

            }
            else
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                File.SetAttributes(fileName, FileAttributes.Hidden);
                StreamReader reader = new StreamReader(fs);
                string res = reader.ReadLine();

                string[] arr = res.Split(':');
                if (arr[0] == "readCardDirect" && arr.Length == 2)
                {
                    readCardDirect = Convert.ToInt32(arr[1]);
                }

                reader.Close();
                fs.Close();
            }
        }

        public static void reWrite(string result)
        {
            string fileName = Directory.GetCurrentDirectory() + "\\readCardDirect.ini";
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            File.SetAttributes(fileName, FileAttributes.Hidden);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine(result);
            sr.Close();
            fs.Close();
        }
    }
}
