﻿using System.Net;
using System.Net.Http.Json;

using WebStore.Interfaces;
using WebStore.Interfaces.TestAPI;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Values
{
    public class ValuesClient : BaseClient, IValuesService
    {
        public ValuesClient(HttpClient Client) : base(Client, WebApiAddresses.V1.Values)
        {

        }        
        public IEnumerable<string> GetValues()
        {
            var response = Http.GetAsync(Address).Result;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<string>();

            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<IEnumerable<string>>().Result!;

            return Enumerable.Empty<string>();
        }
        public string? GetById(int Id)
        {
            var response = Http.GetAsync($"{Address}/{Id}").Result;

            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<string>().Result!;

            return null;
        }
        public void Add(string Value)
        {
            var response = Http.PostAsJsonAsync(Address, Value).Result;
            response.EnsureSuccessStatusCode();
        }
        public void Edit(int Id, string Value)
        {
            var response = Http.PutAsJsonAsync($"{Address}/{Id}", Value).Result;
            response.EnsureSuccessStatusCode();
        }
        public bool Delete(int Id)
        {
            var response = Http.DeleteAsync($"{Address}/{Id}").Result;
            if (response.IsSuccessStatusCode)
                return true;
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            throw new InvalidOperationException();
        }        
    }
}
