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
        }

        public enum TransactionType
        {
            Withdrawal = 0,
            Deposit = 1
        }

        public struct CoreCaribeBank
        {
            public void Initializer()
            {
                Clients = new List<Client>();
                Transactions = new List<Transaction>();
                Tool = new ToolMenuAndMessages();
            }

            public void addClient()
            {
                int code;
                int accountNumber;
                
                string name;               

                Console.Clear();
                Console.Write("Ingrese nombre completo del cliente: ");
                name = Console.ReadLine();

                if (!string.IsNullOrEmpty(findClientByName(name)))
                {
                    Console.WriteLine($"El cliente: [{name}] existe registrado en el sistema");
                    Tool.operationCanceled();
                    return;
                }

                //generate code
                code = codeGenerator();

                //generate number account
                accountNumber = accountNumberGenerator();

                var client = new Client(code, name, accountNumber, 0);

                Clients.Add(client);
                Tool.savedData();
            }

            public void addTrasaction()
            {
                int code;
                int codeClient;
                int transactionType;

                double amount;

                bool validation;

                Console.Clear();
                Console.Write("Ingrese código del cliente: ");
                if(!int.TryParse(Console.ReadLine(), out codeClient))
                {
                    Tool.integerValidation();
                    Tool.operationCanceled();
                    return;
                }

                var client = getClientByCode(codeClient);

                if (client.Name == null)
                {
                    Tool.clientNotFounded();
                    Tool.operationCanceled();
                    return;
                }

                Console.Write("Tipo de transacción 0-Retiro / 1-Depósito: ");
                if(!int.TryParse(Console.ReadLine(), out transactionType))
                {
                    Tool.integerValidation();
                    Tool.operationCanceled();
                    return;
                }

                if(transactionType != 0 && transactionType != 1)
                {
                    Tool.invalidOption();
                    Tool.operationCanceled();
                    return;
                }

                Console.Write("Ingrese cantidad a {0}: ", (TransactionType)transactionType == TransactionType.Withdrawal ? "retirar" : "depositar");
                if (!double.TryParse(Console.ReadLine(), out amount))
                {
                    Console.WriteLine("Valor no válido.");
                    Tool.operationCanceled();
                    return;
                }

                if ((TransactionType)transactionType == TransactionType.Withdrawal)
                {
                    if (client.Balance < amount)
                    {
                        Console.WriteLine("Fondos insuficientes.");
                        Tool.operationCanceled();
                        return;
                    }
                }
                code = codeGenerator(true);

                var transaction = new Transaction(code, codeClient, (TransactionType)transactionType, amount, DateTime.Now);

                Transactions.Add(transaction);
                updateBalanceOfClient(codeClient, amount, (TransactionType)transactionType);
                Tool.savedData();
            }

            public void updateBalanceOfClient(int clientCode, double amount, TransactionType tType)
            {
                int index = Clients.FindIndex(t => t.Code == clientCode);

                var client = Clients.Where(t => t.Code == clientCode)
                    .Select(t => { _ = tType == TransactionType.Deposit ? t.Balance += amount : t.Balance -= amount; return t; }).ToList();

                Clients.RemoveAt(index);
                Clients.AddRange(client);
            }

            public string findClientByName(string name)
            {
                return Clients.FirstOrDefault(t => t.Name.Equals(name)).Name;
            }

            public Client getClientByCode(int code)
            {
                return Clients.FirstOrDefault(t => t.Code == code);
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

            private int codeGenerator(bool transaction = false)
            {
                int code;

                if (transaction)
                {
                    if (Transactions.Count == 0)
                    {
                        return 1;
                    }

                    code = Transactions.Max(t => t.Code);
                    return code + 1;
                }

                if (Clients.Count == 0)
                {
                    return 1;
                }

                code = Clients.Max(t => t.Code);
                return code + 1;
            }

            public List<Client> Clients;
            public List<Transaction> Transactions;

            private ToolMenuAndMessages Tool;

            
        }

        struct ToolMenuAndMessages
        {
            private const string ContinueWithAnyKey = "Presione cualquier tecla para continuar.";
            private const string Line = "===========================================================";
            private const string Greeting = "Bienvenido al sistema Banco Del Caribe";
            private const string OperationCanceled = "Operación cancelada.";
            private const string SavedData = "Datos guardados.";
            private const string InvalidOption = "Ha ingresado una opción no válida.";
            private const string IntegerValidation = "Error de captura. Sólo se permiten valores enteros.";
            private const string ClientNotFounded = "El cliente no fue encontrado.";
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
                Console.WriteLine($"{OperationCanceled} \n{ContinueWithAnyKey}");
                Console.ReadKey();
                Console.Clear();
            }

            public void savedData()
            {
                Console.WriteLine($"{SavedData} \n{ContinueWithAnyKey}");
                Console.ReadKey();
                Console.Clear();
            }

            public void invalidOption()
            {
                Console.WriteLine($"{InvalidOption} \n{ContinueWithAnyKey}");
                Console.ReadKey();
                Console.Clear();
            }

            public void integerValidation()
            {
                Console.WriteLine($"{IntegerValidation}");
                //Console.ReadKey();
                //Console.Clear();
            }

            public void clientNotFounded()
            {
                Console.WriteLine(ClientNotFounded);
            }
        }


        static void Main(string[] args)
        {
            var tool = new ToolMenuAndMessages();
            var core = new CoreCaribeBank();

            core.Initializer();

            int option = 0;

            bool endProgram = false;

            do
            {
                tool.printMenuHead();

                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    tool.integerValidation();
                }
                else
                {
                    switch (option)
                    {
                        case 1:
                            core.addClient();
                            break;
                        case 2:
                            core.addTrasaction();
                            break;
                        default:
                            tool.invalidOption();
                            break;
                    }
                }                
            } while (!endProgram);
        }
    }
}
