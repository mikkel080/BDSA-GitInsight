using Microsoft.AspNetCore.Components;

namespace GitInsight.Blazor.Pages
{
    public partial class InsightPage
    {
        [Parameter]
        public string org { get; set; } = "";

        [Parameter]
        public string proj { get; set; } = "";

        private string mode { get; set; } = "";


        private void fetchAnalysis()
        {
            if (org == "" || proj == "" || mode == "")
                return;

            //Fetch the data from the api.
        }
    }
}
