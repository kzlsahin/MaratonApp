using System.Globalization;

namespace MaratonApp
{

    class MaratonMeter
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Bu uygulama girdiğiniz koşu süreleri ve adım mesafenize göre koştuğunuz mesafeyi hesaplar");
            Console.WriteLine("Maraton koşunuz farklı aşamalardan oluşuyorsa her bir aşamayı ayrı koşu olarak ekleyebilirsiniz.");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            List<Jogging> joggings = new List<Jogging>();

            double duration = 0; // magic number!
            double totalRange = 0;
            int answer;

            do
            {
                AddJogging(joggings);
            } while (RequestYesNo("Koşu eklemek ister misiniz?", 'y', 'n'));

            foreach (Jogging jogging in joggings)
            {
                totalRange += jogging.range;
                duration += jogging.durationSeconds;
            }

            Console.Clear();
            Console.WriteLine($"\n=========== Sonuc ===================\n");
            Console.WriteLine($"Toplam koşu süreniz: {TimeSpan.FromSeconds(duration)}");
            Console.WriteLine($"Toplam koştuğunuz mesafe: {totalRange:F1} metre");
            Console.WriteLine($"\n=========== Sonuc ===================\n");

            if (RequestYesNo("Sonuçları kaydetmek ister misiniz", 'y', 'n'))
            {
                Console.Clear();
                Console.WriteLine("1. Sadece Sonucu .txt olarak kaydet");
                Console.WriteLine("2. Koşu bilgisini .csv olarak kaydet");
                answer = RequestEntry("yukarıdaki seçeneklerden birini seçiniz", new int[] { 1, 2 });
                switch (answer)
                {
                    case 2:
                        SaveCsvFile(joggings);
                        break;
                    case 1:
                        SaveResultsToTxt(duration, totalRange);
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("Süreç Tamamlandı. Teşekkürler...");
            Console.Read();

            //switch (joggingTest.Status)
            //{
            //    case Status.Active:
            //        SaveCsvFile(joggings);
            //        break;
            //    case Status.Pending:
            //        SaveResultsToTxt(duration, totalRange);
            //        break;
            //}
        }

        static void AddJogging(List<Jogging> joggings)
        {
            TimeSpan joggingTime = new TimeSpan(0, 0, 0);
            double paceLength;
            double pacePerMinute;

            RequestTime("Koşu sürenizi giriniz", joggingTime);
            RequestEntry("Adım boyunuzu metre olarak giriniz. \n( ondalık ayraç olarak nokta kullanınız)", out paceLength);
            RequestEntry("Dakikada attığınız adım sayısını giriniz", out pacePerMinute);

            joggings.Add(new Jogging(joggingTime.TotalSeconds, paceLength, pacePerMinute));
        }

        static void RequestEntry(string message, out double userInput)
        {
            Console.WriteLine(message);
            Console.WriteLine("sayısal bir değer giriniz");

            while (!double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, null, out userInput))
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

            while (!int.TryParse(Console.ReadLine(), out userInput) || !cases.Contains(userInput))
            {
                Console.Write("lütfen");
                foreach (int i in cases) Console.Write(", " + i + " ");
                Console.WriteLine("seçeneklerinden birini giriniz ");
            }

            Console.WriteLine(userInput);
            return userInput;
        }

        static void RequestTime(string messege, TimeSpan time)
        {
            Console.WriteLine(messege);
            Console.WriteLine("[saat,dakika,saniye] / [dakika,saniye] / [saniye]");

            string inputLine = Console.ReadLine();
            string[] entries = inputLine.Split(',');

            if (entries.Length > 3)
            {
                Console.WriteLine("Girdiğiniz format tanımlanamadı. Tekrar deneyiniz.");
                RequestTime(messege, time);
                return;
            }

            try
            {
                int zeroIndex = int.Parse(entries[0]);
                var timeTest = new TimeSpan(0,0,0,0,0);
                switch (entries.Length)
                {
                    case 3:
                        time = new TimeSpan(zeroIndex, int.Parse(entries[1]), int.Parse(entries[2]));
                        break;
                    case 2:
                        time = new TimeSpan(0, zeroIndex, int.Parse(entries[1]));
                        break;
                    case 1:
                        time = new TimeSpan(0, 0, zeroIndex, 0, 0);
                        break;
                }//Dynamic Dispatch - if - switch

                Console.WriteLine(time.TotalSeconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                RequestTime(messege, time);
            }

            return;
        }

        static bool RequestYesNo(string messege, char yes, char no)
        {
            Console.WriteLine(messege + $"({yes}/{no})");

            char ans = Console.ReadKey().KeyChar;//Read Key değişekene atanmalı

            while (ans != yes && ans != no)
            {
                Console.WriteLine($"Girdiniz yorumlanamadı. Lütfen {yes}/{no} ile cevap veriniz.");
                ans = Console.ReadKey().KeyChar;
            }

            //if (ans == yes) return true;
            //return false;

            return ans == yes;
        }

        static void SaveCsvFile(in List<Jogging> jogginList, string fileName = "results.csv")
        {
            string path = Directory.GetCurrentDirectory() + @"\Data\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path += fileName;
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("duration;adım boyu;adım/saniye;mesafe");
                sw.WriteLine("(saniye);(metre);(1/dakika);(metre)");

                //Immutable objects -- string -- stringbuilder

                foreach (Jogging jogging in jogginList)
                {
                    string line = $"{jogging.durationSeconds}; {jogging.paceLength}; {jogging.pacePerSec}; {jogging.range}";
                    sw.WriteLine(line);
                }
            }
        }

        static void SaveResultsToTxt(double duration, double range, string fileName = "results.txt")
        {

            string path = Directory.GetCurrentDirectory() + @"\Data\";

            if (!Directory.Exists(path)) 
            {
                Directory.CreateDirectory(path);
            }
            
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

        public Jogging(double durationSec, double paceLength, double pacePerMin)
        {
            this.pacePerSec = pacePerMin / 60;
            this.paceLength = paceLength;
            this.durationSeconds = durationSec;
            this.range = durationSeconds * paceLength * pacePerSec; // to convert to meter per second
        }
    }

    class JoggingTest
    {
        public double Range
        {
            get
            {
                return _durationSec * _paceLength * PacePerSec;
            }
        }

        public double DurationSeconds
        {
            get
            {
                return _durationSec;
            }
        }

        public double PaceLength
        {
            get
            {
                return _paceLength;
            }
        }

        public double PacePerSec 
        { 
            get 
            {
                return _pacePerMin / 60;
            }
        }

        public Status Status { get; set; }

        private readonly double _durationSec;

        private readonly double _paceLength;

        private readonly double _pacePerMin;

        public JoggingTest(double durationSec, double paceLength, double pacePerMin)
        {
            _durationSec = durationSec;
            _paceLength = paceLength;
            _pacePerMin = pacePerMin;
        }
    }

    public enum Status
    {
        Active = 1,
        Pending = 2,
        Initial = 3,
        Passive = 4
    }
}


