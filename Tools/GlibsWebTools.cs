using System.Text;
using System.Web;

namespace Glibs.WebTools
{
    internal class GlibsWebTools
    {
    }
    public class UrlBuilder
    {
        string? url;
        string? address;
        string? path;
        List<string>? queryPair;
        public UrlBuilder(string _address) => this.address = _address;
        public static UrlBuilder Create(string _address) => new UrlBuilder(_address);
        public UrlBuilder NewUrl(string _path)
        {
            this.path = _path;
            this.queryPair = new List<string>();
            return this;
        }
        public UrlBuilder AddOption(string _param, string _value)
        {
            this.queryPair?.Add($"{HttpUtility.UrlEncode(_param)}={HttpUtility.UrlEncode(_value)}");
            return this;
        }
        public string Build()
        {
            if (string.IsNullOrWhiteSpace(this.address)) 
                throw new ArgumentNullException("Address is empty!");
            StringBuilder sb = new StringBuilder(this.address);
            if (!string.IsNullOrWhiteSpace(this.path))
                sb.Append($"/{this.path}");
            if (this.queryPair?.Count > 0)
                sb.Append($"?{string.Join("&", this.queryPair)}");
            return sb.ToString();
        }
        public static string AddOption(string _url, string _param, string _value)
        {
            return $"{_url}&{HttpUtility.UrlEncode(_param)}={HttpUtility.UrlEncode(_value)}";
        }
    }
}
