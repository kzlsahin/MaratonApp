using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MaratonApp
{

    class MaratonMeter
    {
        
    public static void Main(string[] Args)
    {
        Console.WriteLine("Bu uygulama girdiğiniz koşu süreleri ve adım mesafenize göre koştuğunuz mesafeyi hesaplar");
        Console.WriteLine("Maraton koşunuz farklı aşamalardan oluşuyorsa her bir aşamayı ayrı koşu olarak ekleyebilirsiniz.");
        //Git deneme comment
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        List<Jogging> joggingList = new List<Jogging>();
        Double durationSec = 0;
        Double totalRange = 0;
        int ans;

        do
        {

            addJogging(ref joggingList);

        } while (RequestYesNo("Koşu eklemek ister misiniz?", 'y', 'n'));

        foreach (Jogging jogging in joggingList)
        {
            totalRange += jogging.range;
            durationSec += jogging.durationSeconds;
        }
        Console.Clear();
        Console.WriteLine($"\n=========== Sonuc ===================\n");
        Console.WriteLine($"Toplam koşu süreniz: {TimeSpan.FromSeconds(durationSec)}");
        Console.WriteLine($"Toplam koştuğunuz mesafe: {totalRange:F1} metre");
        Console.WriteLine($"\n=========== Sonuc ===================\n");

        if (RequestYesNo("Sonuçları kaydetmek ister misiniz", 'y', 'n'))
        {
            Console.Clear();
            Console.WriteLine("1. Sadece Sonucu .txt olarak kaydet");
            Console.WriteLine("2. Koşu bilgisini .csv olarak kaydet");
            ans = RequestEntry("yukarıdaki seçeneklerden birini seçiniz", new int[] { 1, 2 });
                switch (ans)
                {
                    case 2:
                        SaveCsvFile(joggingList);
                        break;
                    case 1:
                        SaveResultsToTxt(durationSec, totalRange);
                        break;
                }
        }

        Console.Clear();
        Console.WriteLine("Süreç Tamamlandı. Teşekkürler...");
        Console.Read();


    }

    static void addJogging(ref List<Jogging> joggingList)
    {
        TimeSpan joggingTime = new TimeSpan(0, 0, 0);
        double paceLength;
        double pacePerMinute;

        RequestTime("Koşu sürenizi giriniz", ref joggingTime);
        RequestEntry("Adım boyunuzu metre olarak giriniz. \n( ondalık ayraç olarak nokta kullanınız)", out paceLength);
        RequestEntry("Dakikada attığınız adım sayısını giriniz", out pacePerMinute);
            
        joggingList.Add(new Jogging(joggingTime.TotalSeconds, paceLength, pacePerMinute));

    }

    static void RequestEntry(string messege, out double userInput)
    {
        Console.WriteLine(messege);
        Console.WriteLine("sayısal bir değer giriniz");

        while (!Double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, null, out userInput))
        {
            Console.WriteLine("Girdiniz sayısal bir değer değil. sayısal bir değer giriniz (!! ondalık ayraç olarak nokta kulanın !!)");
        }
            Console.WriteLine(userInput);
        }

    static int RequestEntry(string messege, int[] cases)
    {
        Console.WriteLine(messege);
        Console.WriteLine("sayısal bir değer giriniz (!! ondalık ayraç olarak nokta kulanın !!)");
        int userInput;

        while (!int.TryParse(Console.ReadLine(),  out userInput) || !cases.Contains(userInput))
        {
            Console.Write("lütfen");
            foreach (int i in cases) Console.Write(", " + i + " ");
            Console.WriteLine("seçeneklerinden birini giriniz ");
        }

            Console.WriteLine(userInput);
        return userInput;


    }

    static void RequestTime(string messege, ref TimeSpan time)
    {

        Console.WriteLine(messege);
        Console.WriteLine("[saat,dakika,saniye] / [dakika,saniye] / [saniye]");
        string[] entries = Console.ReadLine().Split(',');

        if (entries.Length > 3)
        {
            Console.WriteLine("Girdiğiniz format tanımlanamadı. Tekrar deneyiniz.");
            RequestTime(messege, ref time);
            return;
        }

        try
        {
            switch (entries.Length)
            {
                case 3:
                    time = new TimeSpan(int.Parse(entries[0]), int.Parse(entries[1]), int.Parse(entries[2]));
                    break;
                case 2:
                    time = new TimeSpan(0, int.Parse(entries[0]), int.Parse(entries[1]));
                    break;
                case 1:
                    time = new TimeSpan(0, 0, int.Parse(entries[0]), 0, 0);
                    break;

            }
                Console.WriteLine(time.TotalSeconds);
            }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            RequestTime(messege, ref time);

        }

        return;
    }

    static bool RequestYesNo(string messege, char yes, char no)
    {
        Console.WriteLine(messege + $"({yes}/{no})");

        char ans = Console.ReadKey().KeyChar;

        while (ans != yes && ans != no)
        {
            Console.WriteLine($"Girdiniz yorumlanamadı. Lütfen {yes}/{no} ile cevap veriniz.");
            ans = Console.ReadKey().KeyChar;
        }

        if (ans == yes) return true;
        return false;
    }

    static void SaveCsvFile(in List<Jogging> jogginList, string fileName = "results.csv")
    {
        string path = Directory.GetCurrentDirectory() + @"\Data\";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path += fileName;

        string line = "";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("duration;adım boyu;adım/saniye;mesafe");
                sw.WriteLine("(saniye);(metre);(1/dakika);(metre)");


                foreach (Jogging jogging in jogginList)
                {
                    line = $"{jogging.durationSeconds}; {jogging.paceLength}; {jogging.pacePerSec}; {jogging.range}";
                    sw.WriteLine(line);
                }
            }
    }

        static void SaveResultsToTxt(double duration, double range, string fileName = "results.txt")
        {

            string path = Directory.GetCurrentDirectory() + @"\Data\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += fileName;

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine($"toplam katedilen mesafe : {range}");
                sw.WriteLine($"toplam koşulan süre : {duration}");
                sw.WriteLine($"ortalama hız (m/s) : {range / duration:F3}");
            }
        }
    }


    class Jogging
    {
        public readonly double range;
        public readonly double durationSeconds;
        public readonly double paceLength;
        public readonly double pacePerSec;
        public  Jogging(Double durationSec, double paceLength, double pacePerMin )
        {
            this.pacePerSec = pacePerMin / 60;
            this.paceLength = paceLength;
            this.durationSeconds = durationSec;
            this.range = durationSeconds * paceLength * pacePerSec; // to convert to meter per second
        }
        
    }
}


