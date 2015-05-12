using Newtonsoft.Json;
using NFCTrust.Writer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace NFCTrust.Writer.Service
{
    public class NFCService
    {
        public async Task<Car> SaveCar(Car car)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://nfcrest.azurewebsites.net/api/car/Post");
            try
            {
                var response = await client.PostAsync(
                        uri,
                        new HttpStringContent(
                            JsonConvert.SerializeObject(car),
                            Windows.Storage.Streams.UnicodeEncoding.Utf8,
                            "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return car = JsonConvert.DeserializeObject<Car>(await response.Content.ReadAsStringAsync());
                }
                throw new ArgumentException(response.ReasonPhrase);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task<int> SaveCarDriver(int driverId, int carId)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(string.Format("http://nfcrest.azurewebsites.net/api/driver/AddDriverToCar?driverId={0}&carId={1}", driverId, carId));
            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
                throw new ArgumentException(response.ReasonPhrase);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<Driver> SaveDriver(Driver driver)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://nfcrest.azurewebsites.net/api/driver/Post");
            try
            {
                var serializedDriver = JsonConvert.SerializeObject(driver);
                var response = await client.PostAsync(
                        uri,
                        new HttpStringContent(
                            serializedDriver,
                            Windows.Storage.Streams.UnicodeEncoding.Utf8,
                            "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return driver = JsonConvert.DeserializeObject<Driver>(await response.Content.ReadAsStringAsync());
                }
                throw new ArgumentException(response.ReasonPhrase);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
