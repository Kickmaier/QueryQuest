using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace QueryQuest.Data
{
    internal class OpenTriviaDbApiConnection
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public async Task <List<TriviaData.Result>>GetQuestionFromApiAsync(string amount, string difficulty, string categoryId)
        {

            string url = $"https://opentdb.com/api.php?amount={amount}";
            if (!string.IsNullOrEmpty(difficulty))
            {
                url += $"&difficulty={difficulty}";
            }
            if (!string.IsNullOrEmpty(categoryId))
            {
                url += $"&category={categoryId}";
            }
            url += "&type=multiple";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TriviaData.Rootobject>(url);
                if (response != null && response.results.Length > 0)
                {
                    return response.results.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return new List<TriviaData.Result>();
        }
    }
}
