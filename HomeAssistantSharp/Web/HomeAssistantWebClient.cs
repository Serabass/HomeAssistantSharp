namespace HomeAssistantSharp.Web;
using Newtonsoft.Json;
using RestSharp;

public class HomeAssistantWebClient(string baseUrl, string token)
{
  private readonly string _token = token;
  readonly RestClient client = new(baseUrl);
}
