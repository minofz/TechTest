using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TechinicalTest
{
    public class cHttp : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected string vReponseHTML;

        protected async Task<string> donwloadHTML (string pURL)
        {
            vReponseHTML = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    vReponseHTML = await client.GetStringAsync(pURL);
                }
            }
            catch
            {
                vReponseHTML = "";
            }

            return vReponseHTML;
        }

        public async Task<QueryHMTL> queryURL (string pURL)
        {
            QueryHMTL vReturn = new QueryHMTL();

            vReturn.BaseURL = pURL;

            var teste = await donwloadHTML(pURL);

            if (vReponseHTML != "")
            {
                MatchCollection CollectionLink = Regex.Matches(vReponseHTML, @"<(a|link).*?href=(""|')(.+?)(""|').*?>");

                foreach (Match m in CollectionLink)
                {
                    string vClearURL = Regex.Match(m.ToString(), @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*").ToString();
                    if (vClearURL != "")
                    {
                        vReturn.ListURL += $"{vClearURL}\n";
                    }
                }
            }


            return vReturn;
        }

    }

    public partial class QueryHMTL
    {
        public string ListURL { get; set; }
        public string BaseURL { get; set; }
    }


}
