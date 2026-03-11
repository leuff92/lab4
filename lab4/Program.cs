using System;
using System.Collections.Generic;

namespace lab4
{
    public class Computer
    {
        public string CPU { get; set; }
        public int RAM { get; set; }
        public string GPU { get; set; }
        public List<string> AdditionalComponents { get; set; }

        public Computer()
        {
            CPU = "";
            RAM = 0;
            GPU = "";
            AdditionalComponents = new List<string>();
        }

        public void Display()
        {
            Console.WriteLine("Конфигурация компьютера:");
            Console.WriteLine("Процессор: " + CPU);
            Console.WriteLine("ОЗУ: " + RAM + " ГБ");
            Console.WriteLine("Видеокарта: " + GPU);
            Console.WriteLine("Дополнительные комплектующие:");

            if (AdditionalComponents.Count == 0)
            {
                Console.WriteLine("Нет");
            }
            else
            {
                foreach (string component in AdditionalComponents)
                {
                    Console.WriteLine("- " + component);
                }
            }

            Console.WriteLine("-----------------------------------");
        }

        public Computer ShallowCopy()
        {
            return (Computer)this.MemberwiseClone();
        }

        public Computer DeepCopy()
        {
            Computer copy = (Computer)this.MemberwiseClone();
            copy.AdditionalComponents = new List<string>(this.AdditionalComponents);
            return copy;
        }
    }

    public class ComputerBuilder
    {
        private Computer _computer;

        public ComputerBuilder()
        {
            _computer = new Computer();
        }

        public ComputerBuilder WithCPU(string cpu)
        {
            _computer.CPU = cpu;
            return this;
        }

        public ComputerBuilder WithRAM(int ram)
        {
            _computer.RAM = ram;
            return this;
        }

        public ComputerBuilder WithGPU(string gpu)
        {
            _computer.GPU = gpu;
            return this;
        }

        public ComputerBuilder WithComponent(string component)
        {
            _computer.AdditionalComponents.Add(component);
            return this;
        }

        public Computer Build()
        {
            return _computer;
        }
    }

    public interface IComputerFactory
    {
        Computer Construct();
    }

    public class OfficeComputerFactory : IComputerFactory
    {
        public Computer Construct()
        {
            return new ComputerBuilder()
                .WithCPU("Intel Core i3")
                .WithRAM(8)
                .WithGPU("Integrated Graphics")
                .WithComponent("SSD 256 GB")
                .WithComponent("Office Keyboard")
                .Build();
        }
    }

    public class GamingComputerFactory : IComputerFactory
    {
        public Computer Construct()
        {
            return new ComputerBuilder()
                .WithCPU("Intel Core i7")
                .WithRAM(32)
                .WithGPU("NVIDIA RTX 4070")
                .WithComponent("SSD 1 TB")
                .WithComponent("Liquid Cooling")
                .WithComponent("RGB Case")
                .Build();
        }
    }

    public class HomeComputerFactory : IComputerFactory
    {
        public Computer Construct()
        {
            return new ComputerBuilder()
                .WithCPU("AMD Ryzen 5")
                .WithRAM(16)
                .WithGPU("AMD Radeon RX 6600")
                .WithComponent("SSD 512 GB")
                .WithComponent("Wi-Fi Adapter")
                .Build();
        }
    }

    public sealed class PrototypeRegistry
    {
        private static readonly Lazy<PrototypeRegistry> _instance =
            new Lazy<PrototypeRegistry>(() => new PrototypeRegistry());

        private Dictionary<string, Computer> _prototypes;

        private PrototypeRegistry()
        {
            _prototypes = new Dictionary<string, Computer>();

            _prototypes["office"] = new OfficeComputerFactory().Construct();
            _prototypes["gaming"] = new GamingComputerFactory().Construct();
            _prototypes["home"] = new HomeComputerFactory().Construct();
        }

        public static PrototypeRegistry Instance
        {
            get { return _instance.Value; }
        }

        public Computer GetPrototype(string key)
        {
            if (_prototypes.ContainsKey(key))
            {
                return _prototypes[key].DeepCopy();
            }

            throw new ArgumentException("Прототип не найден");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Лабораторная работа №4");
            Console.WriteLine("Builder, Prototype, Singleton");
            Console.WriteLine();

            Console.WriteLine("1. Создание компьютеров через фабрики");
            Console.WriteLine();

            IComputerFactory officeFactory = new OfficeComputerFactory();
            IComputerFactory gamingFactory = new GamingComputerFactory();
            IComputerFactory homeFactory = new HomeComputerFactory();

            Computer officePc = officeFactory.Construct();
            Computer gamingPc = gamingFactory.Construct();
            Computer homePc = homeFactory.Construct();

            Console.WriteLine("Офисный ПК:");
            officePc.Display();

            Console.WriteLine("Игровой ПК:");
            gamingPc.Display();

            Console.WriteLine("Домашний ПК:");
            homePc.Display();

            Console.WriteLine("2. Демонстрация Prototype");
            Console.WriteLine();

            Computer original = new GamingComputerFactory().Construct();
            Computer shallowCopy = original.ShallowCopy();

            shallowCopy.AdditionalComponents.Add("Extra Fan");

            Console.WriteLine("Оригинал после поверхностной копии:");
            original.Display();

            Console.WriteLine("Поверхностная копия:");
            shallowCopy.Display();

            Computer original2 = new HomeComputerFactory().Construct();
            Computer deepCopy = original2.DeepCopy();

            deepCopy.AdditionalComponents.Add("Bluetooth Module");

            Console.WriteLine("Оригинал после глубокой копии:");
            original2.Display();

            Console.WriteLine("Глубокая копия:");
            deepCopy.Display();

            Console.WriteLine("3. Демонстрация Singleton");
            Console.WriteLine();

            PrototypeRegistry registry1 = PrototypeRegistry.Instance;
            PrototypeRegistry registry2 = PrototypeRegistry.Instance;

            Console.WriteLine("Один и тот же объект?");
            Console.WriteLine(ReferenceEquals(registry1, registry2));

            Console.WriteLine();

            Computer clonedGaming = registry1.GetPrototype("gaming");
            clonedGaming.RAM = 64;
            clonedGaming.AdditionalComponents.Add("Capture Card");

            Console.WriteLine("Изменённая копия из реестра:");
            clonedGaming.Display();

            Computer clonedGamingAgain = registry1.GetPrototype("gaming");
            Console.WriteLine("Повторный запрос из реестра:");
            clonedGamingAgain.Display();

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}