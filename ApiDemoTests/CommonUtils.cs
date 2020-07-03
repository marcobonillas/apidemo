using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ApiDemo.Contracts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ApiDemoTests
{
    public class CommonUtils
    {
        public static HttpResponseMessage CreateRandomUser(IHost server, string email = null)
        {
            var client = server.GetTestClient();
            var requestUrl = "/user";

            var randomString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

            email ??= $"theDude_{randomString}@apidemo.com";

            var newUser = new UserRequest
            {
                FirstName = $"The_{randomString}",
                LastName = $"Dude_{randomString}",
                MiddleName = $"mn_{randomString}",
                PhoneNumber = $"555-123-4567",
                EmailAddress = email
            };

            var jsonStringUser = JsonConvert.SerializeObject(newUser);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;
            return result;
        }

        public static HttpResponseMessage CreateTestUser(IHost server, UserRequest userRequest)
        {
            var client = server.GetTestClient();
            var requestUrl = "/user";

            var jsonStringUser = JsonConvert.SerializeObject(userRequest);

            var data = new StringContent(jsonStringUser, Encoding.UTF8, "application/json");

            var response = client.PostAsync(requestUrl, data);
            var result = response.Result;
            return result;
        }

        public static UserResponse GetUserByEmailAddress(string emailForUpdate, HttpClient client)
        {
            var requestGetUrl = $"/user?emailAddress={emailForUpdate}";

            var responseGet = client.GetAsync(requestGetUrl);
            var resultGet = responseGet.Result;
            var contentGet = resultGet.Content.ReadAsStringAsync().Result;

            var jsonResult = JsonConvert.DeserializeObject<UserResponse>(contentGet);
            return jsonResult;
        }
    }
}
