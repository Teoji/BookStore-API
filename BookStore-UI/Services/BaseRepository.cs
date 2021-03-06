﻿using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class BaseRepository<T> : IBaseRepositorty<T> where T : class
    {
        private readonly IHttpClientFactory _client;

        public BaseRepository(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<bool> Create(string URL, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, URL);
            if (obj == null)
                return false;
            request.Content = new StringContent(JsonConvert.SerializeObject(obj));
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Created)
                return true;
            return false;
        }

        public async Task<bool> Delete(string URL, int id)
        {
            if (id < 1)
                return false;
            var request = new HttpRequestMessage(HttpMethod.Delete, URL + id);
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;
            return false;
        }

        public async Task<T> Get(string URL, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, URL + id);
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null;
        }

        public async Task<IList<T>> Get(string URL)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, URL);
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }
            return null;
        }

        public async Task<bool> Update(string URL, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, URL);
            if (obj == null)
                return false;
            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;
            return false;
        }
    }
}
