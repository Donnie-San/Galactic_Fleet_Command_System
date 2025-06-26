using System;
using System.Collections.Generic;
using System.Threading;

namespace Galactic_Fleet_Command_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ship[] ships = {
                new FighterShip("Oni", 6.78, 3),
                new FreighterShip("Kuruma", 2.45, 130),
                new MedicalShip("Ryouin", 4.54, 10)
            };

            foreach (Ship ship in ships)
            {
                if (ship is IMissionReady readyShip)
                {
                    readyShip.PrepareForMission();
                }
            }

            Fleet<Ship> fleet = new Fleet<Ship>();
            fleet.AddShips(ships);

            fleet.LaunchAllMissions();

            Console.ReadKey();
        }
    }

    enum ShipType { Fighter, Freighter, Medical }
    enum MissionStatus { Pending, InProgress, Completed }

    interface IMissionReady
    {
        void PrepareForMission();
    }

    abstract class Ship
    {
        private static readonly object consoleLock = new object();
        private double speed;

        public string Name { get; set; }
        public ShipType ShipType { get; protected set; }
        public MissionStatus Status { get; protected set; } = MissionStatus.Pending;

        public double Speed
        {
            get => speed;
            set => speed = Math.Min(value, 500);
        }

        public Ship(string name, double speed)
        {
            Name = name;
            Speed = speed;
        }

        public abstract void ExecuteMission();

        public override string ToString()
        {
            return $"[{ShipType}] {Name} - Speed: {Speed} MGLT - Status: {Status}";
        }

        protected void SafeConsoleWrite(string message)
        {
            lock (consoleLock)
            {
                Console.WriteLine(message);
            }
        }
    }

    class FighterShip : Ship, IMissionReady
    {
        public int WeaponCount { get; set; }

        public FighterShip(string name, double speed, int weaponCount)
            : base(name, speed)
        {
            WeaponCount = weaponCount;
            ShipType = ShipType.Fighter;
        }

        public void PrepareForMission()
        {
            SafeConsoleWrite($"{Name} loading {WeaponCount} weapons...");
        }

        public override void ExecuteMission()
        {
            Status = MissionStatus.InProgress;
            SafeConsoleWrite($"{Name} engaging in tactical maneuvers...");
            Thread.Sleep(2000);
            Status = MissionStatus.Completed;
            SafeConsoleWrite($"{Name} completed attack mission.");
        }
    }

    class FreighterShip : Ship, IMissionReady
    {
        public int CargoCapacity { get; set; }

        public FreighterShip(string name, double speed, int cargoCapacity)
            : base(name, speed)
        {
            CargoCapacity = cargoCapacity;
            ShipType = ShipType.Freighter;
        }

        public void PrepareForMission()
        {
            SafeConsoleWrite($"{Name} loading {CargoCapacity} tons of cargo...");
        }

        public override void ExecuteMission()
        {
            Status = MissionStatus.InProgress;
            SafeConsoleWrite($"{Name} is delivering cargo...");
            Thread.Sleep(3000);
            Status = MissionStatus.Completed;
            SafeConsoleWrite($"{Name} completed delivery mission.");
        }
    }

    class MedicalShip : Ship, IMissionReady
    {
        public int MedBaySize { get; set; }

        public MedicalShip(string name, double speed, int medBaySize)
            : base(name, speed)
        {
            MedBaySize = medBaySize;
            ShipType = ShipType.Medical;
        }

        public void PrepareForMission()
        {
            SafeConsoleWrite($"{Name} sterilizing medbay with {MedBaySize} units...");
        }

        public override void ExecuteMission()
        {
            Status = MissionStatus.InProgress;
            SafeConsoleWrite($"{Name} is performing medical support...");
            Thread.Sleep(2500);
            Status = MissionStatus.Completed;
            SafeConsoleWrite($"{Name} completed support mission.");
        }
    }

    class Fleet<T> where T : Ship
    {
        private List<T> ships = new List<T>();

        public void AddShips(params T[] ships)
        {
            this.ships.AddRange(ships);
        }

        public void LaunchAllMissions()
        {
            List<Thread> threads = new List<Thread>();

            foreach (T ship in ships)
            {
                Thread t = new Thread(() =>
                {
                    ship.ExecuteMission();
                    Console.WriteLine(ship.ToString());
                });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            Console.WriteLine("All missions have been completed.");
        }
    }
}