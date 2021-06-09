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
            public Client(int code, string name, int accountNumber, double balance)
            {
                Code = code;
                Name = name;
                AccountNumber = accountNumber;
                Balance = balance;
            }

            public int Code;
            public string Name;
            public int AccountNumber;
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
                var tool = new ToolMenuAndMessages();

                int code;
                int accountNumber;
                
                string name;               

                Console.Clear();
                Console.Write("Ingrese nombre completo del cliente: ");
                name = Console.ReadLine();

                if (!string.IsNullOrEmpty(findClient(name)))
                {
                    Console.WriteLine("El cliente: [{name}] existe registrado en el sistema");
                    tool.operationCanceled();
                    
                }

                //generate code
                code = codeGenerator();

                //generate number account
                accountNumber = accountNumberGenerator();

                var client = new Client(code, name, accountNumber, 0);

                Clients.Add(client);
                tool.savedData();
            }

            public string findClient(string name)
            {
                //if(Clients == null)
                //{
                //    return string.Empty;
                //}

                return Clients.FirstOrDefault(t => t.Name.Equals(name)).Name;
            }

            private int accountNumberGenerator()
            {
                if (Clients.Count == 0)
                {
                    return 100;
                }

                int accountNumber = Clients.Max(t => t.AccountNumber);

                return accountNumber + 1;
            }

            private int codeGenerator()
            {
                if (Clients.Count == 0)
                {
                    return 1;
                }

                int code = Clients.Max(t => t.Code);

                return code + 1;
            }

            public List<Client> Clients;
            public List<Transaction> Transactions;

            
        }

        struct ToolMenuAndMessages
        {
            private const string Line = "===========================================================";
            private const string Greeting = "Bienvenido al sistema Banco Del Caribe";
            private const string OperationCanceled = "Operación cancelada.";
            private const string SavedData = "Datos guardados.";
            private const string Options = @"
1) Agregar Cliente
2) Agregar Transacción
3) Modificar Cliente
4) Eliminar Cliente
5) Mostrar lista de clientes con depósitos
6) Mostrar lista de clientes con retiros
7) Mostrar toda la lista de clientes ordenada por número de cuenta
8) Salir del programa
Seleccione una opción: ";



            public void printMenuHead()
            {
                Console.WriteLine(Line);
                Console.WriteLine(Greeting);
                Console.WriteLine(Line);
                Console.Write(Options);
            }

            public void operationCanceled()
            {
                Console.WriteLine(OperationCanceled);
                Console.Clear();
                printMenuHead();
            }

            public void savedData()
            {
                Console.WriteLine(SavedData);
                Console.Clear();
            }
        }


        static void Main(string[] args)
        {
            var menuHead = new ToolMenuAndMessages();
            var core = new CoreCaribeBank();

            core.Initializer();

            int option = 0;

            do
            {
                menuHead.printMenuHead();

                option = Convert.ToInt32(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        core.addClient();
                        break;
                }
                
                Console.ReadKey();

            } while (true);
        }
    }
}
