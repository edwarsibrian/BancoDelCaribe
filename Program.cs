using System;
using System.Collections.Generic;
using System.Linq;

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
                name = Console.ReadLine().ToUpper();

                if (string.IsNullOrEmpty(name))
                {
                    Tool.nameNull();
                    Tool.operationCanceled();
                    return;
                }

                if (countClientsEquals(name) > 0)
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
                Tool.ClientResume(client.Code, client.Name, client.AccountNumber, client.Balance, false);
                Tool.savedData();
            }

            public void addTrasaction()
            {
                int code;
                //int codeClient;
                int transactionType;

                double amount;

                bool validation;

                Console.Clear();

                var client = GetClientAndValidateClientCode();
                if (client == null) { return; }
                
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

                Console.Write("Ingrese cantidad a {0}: $", (TransactionType)transactionType == TransactionType.Withdrawal ? "retirar" : "depositar");
                if (!double.TryParse(Console.ReadLine(), out amount))
                {
                    Console.WriteLine("Valor no válido.");
                    Tool.operationCanceled();
                    return;
                }

                if ((TransactionType)transactionType == TransactionType.Withdrawal)
                {
                    if (client.Value.Balance < amount)
                    {
                        Console.WriteLine("Fondos insuficientes.");
                        Tool.operationCanceled();
                        return;
                    }
                }
                code = codeGenerator(true);

                var transaction = new Transaction(code, client.Value.Code, (TransactionType)transactionType, amount, DateTime.Now);

                Transactions.Add(transaction);
                updateBalanceOfClient(client.Value.Code, amount, (TransactionType)transactionType);
                Console.WriteLine(@"
Resumen Transacción
=====================================

Código:             {0}
Código de Cliente:  {1}
Tipo de transacción:{2}
Monto:             ${3}
Fecha:              {4}", 
transaction.Code, transaction.CodeClient, transaction.TType == TransactionType.Deposit ? "Depósito" : "Retiro", transaction.Amount, transaction.Date);
                Tool.savedData();
            }

            public void updateAndDeleteClient(bool update = true)
            {
                Console.Clear();

                var client = GetClientAndValidateClientCode();
                if (!client.HasValue) { return; }

                Tool.ClientResume(client.Value.Code, client.Value.Name, client.Value.AccountNumber, client.Value.Balance, update);
                Console.WriteLine("------------------------------------");

                if (update)
                {
                    Console.WriteLine("(*) valores que se pueden modificar.");
                }
                                
                Console.Write("Desea {0} el cliente [s/n]: ", update ? "actualizar" : "eliminar");
                string answer = Console.ReadLine();

                if (answer != "s" && answer != "n")
                {
                    Tool.invalidOption();
                    return;
                }

                if (answer == "n")
                {
                    Tool.operationCanceled();
                    return;
                }

                if (update)
                {
                    var clientUpdt = new Client(client.Value.Code, client.Value.Name, client.Value.AccountNumber, client.Value.Balance);

                    Console.Write("Ingrese nombre completo del cliente: ");
                    clientUpdt.Name = Console.ReadLine().ToUpper();

                    if (string.IsNullOrEmpty(clientUpdt.Name))
                    {
                        Tool.nameNull();
                        Tool.operationCanceled();
                        return;
                    }

                    deleteClient(clientUpdt.Code);

                    if (countClientsEquals(clientUpdt.Name) > 0)
                    {
                        Console.WriteLine($"Existe otro cliente: [{clientUpdt.Name}] registrado en el sistema");
                        Clients.Add(client.Value);
                        Tool.operationCanceled();
                        return;
                    }

                    double balance;

                    Console.Write("Ingrese nuevo balance: $");

                    if(!double.TryParse(Console.ReadLine(), out balance))
                    {
                        Console.WriteLine("Valor no válido.");
                        Clients.Add(client.Value);
                        Tool.operationCanceled();
                        return;
                    }

                    Clients.Add(clientUpdt);
                    Tool.ClientResume(clientUpdt.Code, clientUpdt.Name, clientUpdt.AccountNumber, clientUpdt.Balance, update);
                    Tool.savedData();
                }
                else
                {
                    deleteClient(client.Value.Code);
                    Console.WriteLine("El registro a sido eliminado.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            public void printAllClients()
            {
                var clientsOrdered = Clients.OrderBy(t => t.AccountNumber).ToList();

                Console.Clear();
                Console.WriteLine($"{"CÓDIGO",-6}   {"NOMBRE",-30}  {"No. CUENTA",-8}  {"BALANCE"}");
                Console.WriteLine("-----------------------------------------------------------------------------------");
                foreach(var client in clientsOrdered)
                {
                    Console.WriteLine($"{client.Code,-6}    {client.Name,-30}   {client.AccountNumber,-8}   ${client.Balance}");
                }
                               
                Console.WriteLine("\nFin de registro.");
                Tool.continueWithAnyKey();
            }

            public void printClientsWithTransactions(TransactionType type)
            {
                

                var clientsWithDeposit = Transactions.Where(t => t.TType == type).GroupBy(g => g.CodeClient)
                    .Select(s => new
                    {
                        CodeClient = s.Key,
                        Average = s.Sum(x => x.Amount) / s.Count(),
                        Total = s.Sum(x => x.Amount)
                    })
                    .Join(Clients,
                    txn => txn.CodeClient,
                    cl => cl.Code,
                    (txn, cl) => new { DepTxn = txn, DepCl = cl })
                    .Where(DepCl => DepCl.DepCl.Code == DepCl.DepTxn.CodeClient)
                    .Select(s => new
                    {
                        Code = s.DepCl.Code,
                        Name = s.DepCl.Name,
                        Average = s.DepTxn.Average,
                        Sum = s.DepTxn.Total
                    }).ToList();

                Console.Clear();
                Console.WriteLine("LISTA DE CLIENTES CON {0}\n", type == TransactionType.Deposit ? "DEPÓSITOS" : "RETIROS");
                Console.WriteLine($"{"CÓDIGO",-6}   {"NOMBRE",-30}  {"PROMEDIO",-8} {"TOTAL"}");
                Console.WriteLine("----------------------------------------------------------------------");

                foreach(var item in clientsWithDeposit)
                {
                    Console.WriteLine($"{item.Code,-6}  {item.Name,-30} {Math.Round(item.Average, 2),-8}  ${item.Sum}");
                }

                Console.WriteLine("\nFin de registro.");
                Tool.continueWithAnyKey();
            }

            private Client? GetClientAndValidateClientCode()
            {
                int codeClient;

                Console.Write("Ingrese código del cliente: ");
                if (!int.TryParse(Console.ReadLine(), out codeClient))
                {
                    Tool.integerValidation();
                    Tool.operationCanceled();
                    return null;
                }

                var client = getClientByCode(codeClient);

                if (client.Name == null)
                {
                    Tool.clientNotFounded();
                    Tool.operationCanceled();
                    return null;
                }

                return client;
            }

            private void updateBalanceOfClient(int clientCode, double amount, TransactionType tType)
            {
                int index = Clients.FindIndex(t => t.Code == clientCode);

                var client = Clients.Where(t => t.Code == clientCode)
                    .Select(t => { _ = tType == TransactionType.Deposit ? t.Balance += amount : t.Balance -= amount; return t; }).ToList();

                Clients.RemoveAt(index);
                Clients.AddRange(client);
            }

            private void deleteClient(int clientCode)
            {
                int index = Clients.FindIndex(t => t.Code == clientCode);

                Clients.RemoveAt(index);
            }

            private int countClientsEquals(string name)
            {
                return Clients.Where(t => t.Name.Equals(name)).ToList().Count();
            }

            private Client getClientByCode(int code)
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
            private const string NameNull = "Debe ingresar un nombre.";
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

            public void nameNull()
            {
                Console.WriteLine(NameNull);
            }

            public void continueWithAnyKey()
            {
                Console.WriteLine(ContinueWithAnyKey);
                Console.ReadKey();
                Console.Clear();
            }

            public void ClientResume(int code, string name, int accountNumber, double balance, bool update)
            {
                Console.WriteLine(@"
Resumen operación
==========================================================

    Código cliente:     {0}
{4} Nombre de cliente:  {1}
    Número de cuenta:   {2}
{4} Balance:           ${3}", code, name, accountNumber, balance, update ? "(*)" : "   ");
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
                        case 3:
                            core.updateAndDeleteClient();
                            break;
                        case 4:
                            core.updateAndDeleteClient(false);
                            break;
                        case 5:
                            core.printClientsWithTransactions(TransactionType.Deposit);
                            break;
                        case 6:
                            core.printClientsWithTransactions(TransactionType.Withdrawal);
                            break;
                        case 7:
                            core.printAllClients();
                            break;
                        case 8:
                            endProgram = true;
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
