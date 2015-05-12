using Newtonsoft.Json;
using NFCTrust.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace NFCTrust.Services
{
    public class NfcClient
    {
        public static async Task<Car> GetCar(string registration)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format("http://nfcrest.azurewebsites.net/api/car/get?registration={0}", registration));
            try
            {
                var response = await client.GetAsync(uri);
                Car car = JsonConvert.DeserializeObject<Car>(response.Content.ToString());
                return car;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task<Driver> GetDriver(string registration)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format("http://nfcrest.azurewebsites.net/api/driver/get?registration={0}", registration));
            try
            {
                var response = await client.GetAsync(uri);
                Driver driver = JsonConvert.DeserializeObject<Driver>(response.Content.ToString());
                return driver;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task<User> SaveUser(User user)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format("http://nfcrest.azurewebsites.net/api/driver/get?registration={0}", user));
            try
            {
                var response = await client.GetAsync(uri);
                User userResponse = JsonConvert.DeserializeObject<User>(response.Content.ToString());
                return userResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
