using EligibilityScoring.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace EligibilityScoring.Infrastructure.ExternalServices
{
    public class CreditReportingClient(HttpClient httpClient, IConfiguration configuration) : ICreditReportingClient
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly string baseUrl = configuration["ExternalServices:CreditReportingApi"] ?? "https://creditservices-dhf2avgkhqdwdabq.canadacentral-01.azurewebsites.net";

        public async Task<int> GetCibilScoreAsync(int customerId, string panNo)
        {
            try
            {
                
                var getResponse = await httpClient.GetAsync($"{baseUrl}/api/v1/cibil/pan/{panNo}");
                if (getResponse.IsSuccessStatusCode)
                {
                    var result = await getResponse.Content.ReadFromJsonAsync<CibilApiResponse>();
                    return result?.Data?.CibilScore ?? 0;
                }

                var request = new { CustomerId = customerId, PanNo = panNo };
                var postResponse = await httpClient.PostAsJsonAsync($"{baseUrl}/api/v1/cibil/check", request);

                if (postResponse.IsSuccessStatusCode)
                {
                    var result = await postResponse.Content.ReadFromJsonAsync<CibilApiResponse>();
                    return result?.Data?.CibilScore ?? 0;
                }
                
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetCibilScoreByCustomerIdAsync(int customerId)
        {
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/v1/cibil/customer/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CibilApiResponse>();
                    return result?.Data?.CibilScore ?? 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

       
        private class CibilApiResponse        {
            public bool Success { get; set; }
            public CibilData Data { get; set; }
        }

        private class CibilData
        {
            public int CibilScore { get; set; }
        }
    }
}
