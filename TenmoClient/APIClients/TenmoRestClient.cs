using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient;
using TenmoClient.Data;
using TenmoClient.APIClients;

namespace TenmoClient.APIClients
{
    public class TenmoRestClient
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client;

        public TenmoRestClient()
        {
            this.client = new RestClient(API_BASE_URL);
        }

        public API_User GetUser()
        {
            RestRequest request = new RestRequest("/user");
            IRestResponse<API_User> response = client.Get<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to Server; Try again later!");

                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem getting Balance: " + response.StatusDescription);
                Console.WriteLine(response.Content);

                return null;
            }

            return response.Data;
        }
        public bool HasAuthToken
        {
            get
            {
                return token != null; // TODO: Return true if one is present
            }
        }

        public string token;
        public void UpdateToken(string jwt)
        {
            token = jwt;

            if (jwt == null)
            {
                client.Authenticator = null;
            }
            client.Authenticator = new JwtAuthenticator(jwt);
            //Any request with this client (this.client defined in the constructor)
            // in the future will automatically
            // contain the Authorization/Bearer token header
            // with this we technically don't need any use of
            // line 121 <request.AddHeader("Authorization", "Bearer " + jwt)>
        }

        public List<API_User> GetAllUsers()
        {
            RestRequest request = new RestRequest("user/all");
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not connect to server, try again!");
                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Issue getting users " + response.StatusDescription);
                Console.WriteLine(response.Content);
                return null;
            }
            return response.Data;
        }

        public API_Account GetAccountByUsername(string username)
        {
            RestRequest request = new RestRequest("user/account/" + username);

            IRestResponse<API_Account> response = client.Get<API_Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Whoopsie, somethings not right here.");
                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Yikes...at least you tried..." + response.StatusDescription);
                Console.WriteLine(response.Content);
                return null;
            }
            return response.Data;

        }
        public List<API_Transfer> GetUserTransferHistory()
        {
            RestRequest request = new RestRequest("/transfer");
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Unable to retrieve history from server at this time.");
                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("If at first you don't succeed....well you know what they say...");
                return null;
            }
            return response.Data;
        }

        public API_Transfer CreateTransfer(API_Transfer transfer)
        {
            RestRequest request = new RestRequest("/transfer");
            request.AddJsonBody(transfer);
            IRestResponse<API_Transfer> response = client.Post<API_Transfer>(request);


            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Are you sure you're doing this right?");

                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem making Transfer: " + response.StatusDescription);
                Console.WriteLine(response.Content);

                return null;
            }

            return response.Data;
        }

        public bool IncreaseBalance(int accountId, API_Transfer transfer)
        {
            RestRequest request = new RestRequest("/transfer/to/" + accountId);
            request.AddJsonBody(transfer);
            IRestResponse response = client.Put(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Couldn't connect to database, try again");
                return false;
            }
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem sending transfer: " + response.StatusDescription);
                Console.WriteLine(response.Content);
                return false;
            }
            return true;
        }

        public bool DecreaseBalance(int accountId, API_Transfer transfer)
        {
            RestRequest request = new RestRequest("/transfer/from/" + accountId);
            request.AddJsonBody(transfer);
            IRestResponse response = client.Put(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Couldn't connect to database, try again");
                return false;
            }
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem sending transfer: " + response.StatusDescription);
                Console.WriteLine(response.Content);
                return false;
            }
            return true;
        }


        public API_Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest("/transfer/" + transferId);
            IRestResponse<API_Transfer> response = client.Get<API_Transfer>(request);


            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Are you sure you're doing this right?");

                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Problem retrieving Transfer: " + response.StatusDescription);
                Console.WriteLine(response.Content);

                return null;
            }

            return response.Data;
        }

        public API_User GetUserFromAcctId(int acctId)
        {
            RestRequest request = new RestRequest("user/" + acctId);

            IRestResponse<API_User> response = client.Get<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Whoopsie, somethings not right here.");
                return null;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Yikes...at least you tried..." + response.StatusDescription);
                Console.WriteLine(response.Content);
                return null;
            }
            return response.Data;

        }

        public bool UpdateTransfer(API_Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/update");

            request.AddJsonBody(transfer);

            IRestResponse response = client.Put(request);

            if(response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Unable to complete update" + response.ErrorMessage);
                return false;
            }
            
            if(!response.IsSuccessful)
            {
                Console.WriteLine("Boo u whore" + response.StatusDescription);
                Console.WriteLine(response.Content);
                return false;
            }
            return true;
        }


    }
}
