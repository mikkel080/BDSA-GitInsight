using GitInsight.Blazor.Data;
using Microsoft.AspNetCore.Components;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitInsight.Blazor.Pages
{
    public partial class InsightPage
    {
        [Parameter]
        public string org { get; set; } = "";

        [Parameter]
        public string proj { get; set; } = "";
        [Inject]
        IHttpClientFactory ClientFactory { get; set; }

        private bool isFetchSuccesful { get; set; }

        private string mode { get; set; } = "";
        private Rootobject poco { get; set; }

        private bool doAPIFetch { get; set; } = false;


        private async void fetchAnalysis()
        {
            isFetchSuccesful = false;
            if (org == "" || proj == "" || mode == "")
                return;


            if (doAPIFetch)
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://localhost:7199/{org}/{proj}");
                /*request.Headers.Add("Accept", "application/vnd.github.v3+json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");*/

                var client = ClientFactory.CreateClient();

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    poco = await JsonSerializer.DeserializeAsync
                        <Rootobject>(responseStream);
                    isFetchSuccesful = true;
                }
                else
                {
                    isFetchSuccesful = false;
                }
            } else
            {
                //System.IO.FileNotFoundException: 'Could not find file 'C:\Users\User\source\repos\GitInsight\BDSA-GitInsight\GitInsight.Blazor\wwwroot\miinum98.json'.'
                var file = File.ReadAllText("wwwroot/miinim98.json");
                poco = JsonSerializer.Deserialize<Rootobject>(file);
                Console.WriteLine(poco);
            }
            //Fetch the data from the api.
        }
    }
}
