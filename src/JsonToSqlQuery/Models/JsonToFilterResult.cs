namespace JsonToSqlQuery.Models
{
    public class JsonToFilterResult
    {
        public Filter Filter { get; set; }

        public string Error { get; set; }

        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Error); }
        }
    }
}
