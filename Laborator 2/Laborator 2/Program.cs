using System;

namespace CalculSalariu
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Write("Introduceți tariful pe oră: ");
            double tarifPeOra = Convert.ToDouble(Console.ReadLine());

            Console.Write("Introduceți numărul de ore lucrate: ");
            int oreLucrate = Convert.ToInt32(Console.ReadLine());

            double salariu = tarifPeOra * oreLucrate;

            Console.WriteLine($"\nSalariul calculat este: {salariu} lei.");

            if (salariu > 3000)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Salariu mare");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ați lucrat prea puține ore pentru a avea un salariu mare!");
            }

            Console.ResetColor();

            Console.WriteLine("\nApăsați orice tastă pentru a ieși...");
            Console.ReadKey();
        } 
    } 
} 