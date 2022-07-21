using System;
using TenmoClient.Data;
using TenmoClient.APIClients;
using System.Collections.Generic;
namespace TenmoClient
{
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly TenmoRestClient tenmoClient = new TenmoRestClient();
        public API_User CurrentUser;

        private bool quitRequested = false;

        public void Start()
        {
            while (!quitRequested)
            {
                while (!authService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();

            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");


                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    CurrentUser = tenmoClient.GetUser();
                    switch (menuSelection)
                    {
                        case 1: // View Balance

                            GetUserBalance(); ; // TODO: Implement me  

                            break;

                        case 2: // View Past Transfers
                            GetAllTransfers(); // TODO: Implement me
                            break;

                        case 3: // View Pending Requests
                            GetPendingTransfers();
                            break;

                        case 4: // Send TE Bucks
                            int sendingTypeID = 1001;
                            SendTransferProcess(sendingTypeID);

                            break;

                        case 5: // Request TE Bucks
                            int requestingTypeID = 1000;
                            //GetAllUsers();
                            RequestTransferProcess(requestingTypeID);

                            break;

                        case 6: // Log in as someone else

                            authService.ClearAuthenticator();

                            

                            return; // Leaves the menu and should return as someone else

                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            CurrentUser = null;
                            return;

                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        private void HandleUserLogin()
        {
            while (!authService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();

                // Log the user in
                API_User authenticatedUser = authService.Login(loginUser);

                if (authenticatedUser != null)
                {
                    string jwt = authenticatedUser.Token;     // Use this for the authentication story

                    // TODO: Do something with this JWT.
                    Console.WriteLine("Successfully logged in with JWT");
                    tenmoClient.UpdateToken(jwt);
                }
            }
        }

        private void GetUserBalance()
        {
            API_User displayUser = tenmoClient.GetUser();

            if (displayUser != null)
            {
                Console.WriteLine("Your current account balance is: " + displayUser.Balance.ToString("C"));
            }
            else
            {
                Console.WriteLine("Sorry you don't have access :(");
            }
        }
        private void GetAllTransfers()
        {
            List<API_Transfer> displaytransactions = tenmoClient.GetUserTransferHistory();

            if (displaytransactions != null)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Transfer Herstory:");
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                foreach (API_Transfer transfer in displaytransactions)
                {
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    Console.WriteLine();
                    Console.WriteLine();
                    API_User userTo = GetUserByAccountID(transfer.AccountTo);
                    API_User userfrom = GetUserByAccountID(transfer.AccountFrom);
                    Console.WriteLine(TranslateTypeIdToString(transfer.TransferTypeId));
                    Console.WriteLine("Recieving Transfer: " + userTo.Username);
                    Console.WriteLine("Sending Transfer: " + userfrom.Username);
                    Console.WriteLine("Amount of Transfer: " + transfer.Amount.ToString("C"));
                    Console.WriteLine(TranslateStatusIdToString(transfer.TransferStatusId));
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");

                }
            }
        }


        private void GetPendingTransfers()
        {
            List<API_Transfer> allTransfers = tenmoClient.GetUserTransferHistory();
            Console.WriteLine("Pending Transfers");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            foreach (API_Transfer transfer in allTransfers)
            {
                if (transfer.TransferStatusId == 2000)
                {

                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    Console.WriteLine();
                    Console.WriteLine();
                    API_User userTo = GetUserByAccountID(transfer.AccountTo);
                    API_User userfrom = GetUserByAccountID(transfer.AccountFrom);
                    Console.WriteLine(TranslateTypeIdToString(transfer.TransferTypeId));
                    Console.WriteLine("Recieving Transfer: " + userTo.Username);
                    Console.WriteLine("Sending Transfer: " + userfrom.Username);
                    Console.WriteLine("Amount of Transfer: " + transfer.Amount.ToString("C"));
                    Console.WriteLine(TranslateStatusIdToString(transfer.TransferStatusId));
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");

                }
            }
        }

        private void GetAllUsers()
        {
            List<API_User> displayUsers = tenmoClient.GetAllUsers();

            if (displayUsers != null)
            {
                foreach (API_User user in displayUsers)
                {
                    if(user.Username != CurrentUser.Username)
                    {
                        Console.WriteLine(user.Username);
                    }
                }
            }
            else
            {
                Console.WriteLine("No users to display :( ");
            }
        }
        private API_Account GetAccountFromUsername(string username)
        {
            API_Account retrievedAcct = tenmoClient.GetAccountByUsername(username);

            if (retrievedAcct != null)
            {
                return retrievedAcct;
            }
            else
            {
                return null;
            }
        }
        private API_Transfer GetTransferById(int transferId)
        {
            API_Transfer transfer = tenmoClient.GetTransferById(transferId);
            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                return null;
            }
        }
        private void CreateOrUpdateTransfer(API_Transfer newTransfer)
        {

            if (newTransfer.TransferId == 0)
            {
                API_Transfer createdTransfer = tenmoClient.CreateTransfer(newTransfer);
                if (createdTransfer != null)
                {
                    Console.WriteLine("Transaction has been saved.");
                    return;
                }
                Console.WriteLine("Unable to save transaction, please try again.");
            }
            else
            {
                bool updatedTransfer = tenmoClient.UpdateTransfer(newTransfer);
                if (updatedTransfer)
                {
                    Console.WriteLine("Transaction has been updated.");
                    return;
                }

                Console.WriteLine("Unable to updated transaction, please try again.");
            }
        }


        private void SendTransferProcess(int typeID) //typeID parameter allows us to pull ID from menu scope
        {
            GetAllUsers();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Which user would you like to transfer to?");
            string recievingUsername = Console.ReadLine();// gets user id for the account we're sending to

            Console.WriteLine();

            Console.WriteLine("How much money would you like to send?");
            decimal transferAmount = Convert.ToDecimal(Console.ReadLine()); // gets the amount of money we're sending

            API_Account accountFrom = tenmoClient.GetAccountByUsername(CurrentUser.Username);
            API_Account accountTo = tenmoClient.GetAccountByUsername(recievingUsername);
            int pendingStatusID = 2000;


            API_Transfer newTransfer = new API_Transfer(typeID, pendingStatusID, accountFrom.AccountId, accountTo.AccountId, transferAmount);
            UpdateBalances(accountTo, accountFrom, newTransfer);
            GetUserBalance();

        }
        private void RequestTransferProcess(int typeID) //typeID parameter allows us to pull ID from menu scope
        {
            Console.WriteLine();
            Console.WriteLine();
            GetAllUsers();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Which user would you like to request from?");
            string requestedUsername = Console.ReadLine();// gets user id for the account we're sending to

            Console.WriteLine();

            Console.WriteLine("How much money would you like to request?");
            decimal transferAmount = Convert.ToDecimal(Console.ReadLine()); // gets the amount of money we're sending

            API_Account accountFrom = tenmoClient.GetAccountByUsername(requestedUsername);
            API_Account accountTo = tenmoClient.GetAccountByUsername(CurrentUser.Username);
            int pendingStatusID = 2000;

            API_Transfer newTransfer = new API_Transfer(typeID, pendingStatusID, accountFrom.AccountId, accountTo.AccountId, transferAmount);
            Console.WriteLine();
            UpdateBalances(accountTo, accountFrom, newTransfer);
            Console.WriteLine();
            GetUserBalance();

        }

        private void UpdateBalances(API_Account accountTo, API_Account accountFrom, API_Transfer transfer)
        {
            if (accountFrom.Balance >= transfer.Amount)
            {
                CurrentUser.Balance -= transfer.Amount;
                bool updatedToBalance = tenmoClient.IncreaseBalance(accountTo.AccountId, transfer);
                bool updatedFromBalance = tenmoClient.DecreaseBalance(accountFrom.AccountId, transfer);
                if (updatedToBalance && updatedFromBalance)
                {

                    Console.WriteLine("Processing your request...");
                    if (transfer.TransferTypeId == 1001)
                    {
                        Console.WriteLine("Transfer successful");
                        transfer.TransferStatusId = 2001;
                        Console.WriteLine();
                        CreateOrUpdateTransfer(transfer);
                    }
                    else if (transfer.TransferTypeId == 1000)
                    {
                        //request has been sent and update status id to 2000(pending) createTransfer()
                        Console.WriteLine("Request has been sent");
                        transfer.TransferStatusId = 2000;
                        CreateOrUpdateTransfer(transfer);
                    }
                    else
                    {
                        Console.WriteLine("Missing valid transfer type ID");
                    }
                }
                else if (updatedToBalance && !updatedFromBalance)
                {
                    Console.WriteLine("Unable to complete transaction, check 'From Balance' ");
                    transfer.TransferStatusId = 2002;
                    Console.WriteLine();
                    CreateOrUpdateTransfer(transfer);
                }
                else if (!updatedToBalance && updatedFromBalance)
                {
                    Console.WriteLine("Unable to complete transaction, Check 'To Balance'");
                    transfer.TransferStatusId = 2002;
                    Console.WriteLine();
                    CreateOrUpdateTransfer(transfer);
                }
                else
                {
                    Console.WriteLine("Both updates are fucked");
                    transfer.TransferStatusId = 2002;
                    Console.WriteLine();
                    CreateOrUpdateTransfer(transfer);
                }

            }
            else
            {
                Console.WriteLine("You don't have a sufficient balance for this request.");
                Console.WriteLine("Get your bread up bitch");
                transfer.TransferStatusId = 2002;
                CreateOrUpdateTransfer(transfer);
            }
        }
        private API_User GetUserByAccountID(int accountId)
        {
            return tenmoClient.GetUserFromAcctId(accountId);
        }
        private string TranslateTypeIdToString(int typeId)
        {
            if (typeId == 1000)
            {
                return "Transfer Type: Request";
            }

            if (typeId == 1001)
            {
                return "Transfer Type: Send";
            }
            else
            {
                return "Check your type translation";
            }
        }
        private string TranslateStatusIdToString(int statusId)
        {
            if (statusId == 2000)
            {
                return "Transfer Status: Pending";
            }

            if (statusId == 2001)
            {
                return "Transfer Status: Approved";
            }

            if (statusId == 2002)
            {
                return "Transfer Status: Rejected";
            }
            return "Status translator needs fixin";
        }
    }
}
