using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATAGY_Halak
{
    enum Species
    {
        Bluefish,
        Cobia,
        Grouper,
        Snapper,
        Permit,
        Redfish,
        Tarpon,
        Trout,
    }

    class Fish
    {
        private float _weight;
        private bool _weightIsSet = false;
        private int _top;
        private int _depth;

        public float Weight
        {
            get => _weight;
            set
            {
                if (value < .5F) throw new Exception("Hiba: Túl kicsi a hal súlya");
                if (value > 40F) throw new Exception("Hiba: Túl nagy a hal súlya");
                if (_weightIsSet && value > _weight * 1.1F) throw new Exception("Hiba: Ennyivel nem növekedhet egy hal súlya!");
                if (_weightIsSet && value < _weight * .9F) throw new Exception("Hiba: Ennyivel nem csökkenhet a hal súlya!");
                _weight = value;
                _weightIsSet = true;
            }
        }
        public bool Predator { get; private set; }

        public int Top
        {
            get => _top;
            set
            {
                if (value < 0) throw new Exception("Hiba: Nincsenek repülő halak");
                if (value > 400) throw new Exception("Hiba: Túl mélyen van a hal merülési mélységének teteje");
                _top = value;
            }
        }
        public int Depth
        {
            get => _depth;
            set
            {
                if (value < 10) throw new Exception("Hiba: Túl keskeny merülési sáv");
                if (value > 400) throw new Exception("Hiba: Túl széles merülési sáv");
                _depth = value;
            }
        }
        public Species Species { get; set; }

        public int Bottom => Top + Depth;

        public Fish(float weight, bool predator, int top, int depth, Species species)
        {
            Weight = weight;
            Predator = predator;
            Top = top;
            Depth = depth;
            Species = species;
        }
    }


    class Program
    {
        static List<Fish> to = new List<Fish>();
        static void Main()
        {
            InitTo();
            ToKiir();
            DbRagadozo();
            LegnagyobbHal();
            DbKepesUszni(melyseg: 1.1F);
            MainLoop();
            Report();
            Console.ReadKey(true);
        }

        private static void ToKiir()
        {
            foreach (var h in to)
            {
                Console.ForegroundColor = h.Predator ?
                    ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine("[{4,2}] {0,-8} {1,5:0.00} Kg [{2,3}-{3,3}] cm",
                    h.Species, h.Weight, h.Top, h.Bottom, to.IndexOf(h));
            }
            Console.ResetColor();
        }

        private static void Report()
        {
            ToKiir();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Megevett halak: {megevettHalak.Count} db ({megevettHalak.Sum(x => x.Weight)} Kg)");
        }

        static List<Fish> megevettHalak = new List<Fish>();

        private static void MainLoop()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(to.Count);
                int y = rnd.Next(to.Count);

                bool kulonbozo = to[x].Predator != to[y].Predator;
                bool harmincSzazalek = rnd.Next(100) < 30;
                bool beTudUszni =
                    to[x].Top <= to[y].Bottom
                    && to[y].Top <= to[x].Bottom;

                if (kulonbozo && harmincSzazalek && beTudUszni)
                {
                    Fish ragadozo, novenyevo;

                    if (to[x].Predator)
                    {
                        ragadozo = to[x];
                        novenyevo = to[y];
                    }
                    else
                    {
                        ragadozo = to[y];
                        novenyevo = to[x];
                    }

                    if (ragadozo.Weight <= 40F / 1.1F)
                    {
                        ragadozo.Weight *= 1.09F;
                        megevettHalak.Add(novenyevo);
                        to.Remove(novenyevo);
                    }
                }
            }
        }

        private static void DbKepesUszni(float melyseg)
        {
            float cm = melyseg * 100;
            int db = 0;

            foreach (var h in to)
                if (h.Top <= cm && cm <= h.Bottom) db++;

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"{melyseg} m mélységben {db} hal képes úszni");

        }

        private static void LegnagyobbHal()
        {
            int maxi = 0;

            for (int i = 1; i < to.Count; i++)
                if (to[i].Weight > to[maxi].Weight) maxi = i;

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"A legnagyobb hal ({maxi}. index) súlya: {to[maxi].Weight} Kg");
        }

        private static void DbRagadozo()
        {
            int db = 0;
            foreach (var h in to) if (h.Predator) db++;

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Ragadozók száma: {db} | Növényevők száma: {to.Count - db}");
        }

        static Random rnd = new Random();
        private static void InitTo()
        {
            for (int i = 0; i < 100; i++)
            {
                to.Add(new Fish(
                    weight: rnd.Next(1, 81) / 2F,
                    top: rnd.Next(401),
                    depth: rnd.Next(10, 401),
                    predator: rnd.Next(100) < 10,
                    species: (Species)rnd.Next(Enum.GetNames(typeof(Species)).Length)
                    ));
            }
        }
    }
}
