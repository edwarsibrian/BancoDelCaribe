using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoDelCaribe
{
    class Program
    {
        public struct Client
        {
            public Client(int code, string name, int numberAccount, double balance)
            {
                Code = code;
                Name = name;
                NumberAccount = numberAccount;
                Balance = balance;
            }

            public int Code;
            public string Name;
            public int NumberAccount;
            public double Balance;            
        }

        public struct Transaction
        {
            public Transaction(int code, int codeClient, TransactionType tType, double amount, DateTime date)
            {
                Code = code;
                CodeClient = codeClient;
                TType = tType;
                Amount = amount;
                Date = date;
            }

            public int Code { get; set; }
            public int CodeClient { get; set; }
            public TransactionType TType { get; set; }
            public double Amount { get; set; }
            public DateTime Date { get; set; }

            public enum TransactionType
            {
                Withdrawal = 0,
                Deposit = 1
            }
        }

        public struct CoreCaribeBank
        {
            public void Initializer()
            {
                Clients = new List<Client>();
                Transactions = new List<Transaction>();
            }

            public void addClient()
            {
                int code;
                string name;
                int numberAccount;

                Console.Clear();
                Console.Write("Ingrese nombre completo del cliente: ");
                name = Console.ReadLine();

                if (!string.IsNullOrEmpty(findClient(name)))
                {
                    Console.Write($"El cliente: {name} existe registrado en el sistema");
                }
            }

            public string findClient(string name)
            {
                return Clients.FirstOrDefault(t => t.Name.Equals(name)).Name;
            }

            public List<Client> Clients;
            public List<Transaction> Transactions;

            
        }

        struct ToolMenu
        {
            private const string Line = "===========================================================";
            private const string Greeting = "Bienvenido al sistema Banco Del Caribe";
            private const string Options = @"
1) Agregar Cliente
2) Agregar Transacción
3) Modificar Cliente
4) Eliminar Cliente
5) Mostrar lista de clientes con depósitos
6) Mostrar lista de clientes con retiros
7) Mostrar toda la lista de clientes ordenada por número de cuenta
8) Salir del programa";



            public void printMenuHead()
            {
                Console.WriteLine(Line);
                Console.WriteLine(Greeting);
                Console.WriteLine(Line);
                Console.WriteLine(Options);
            }
        }


        static void Main(string[] args)
        {
            var menuHead = new ToolMenu();

            do
            {
                menuHead.printMenuHead();
                Console.ReadKey();

            } while (true);
        }
    }
}
