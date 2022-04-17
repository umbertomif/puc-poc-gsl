using System.Text;

namespace POC.GSL.Data.Mongo
{
    public class ConnStringBuilder
    {
        private StringBuilder _connString;

        public ConnStringBuilder()
        {
            _connString = new StringBuilder().Append("${connectionString}");
        }

        public ConnStringBuilder WithconnectionString(string connectionString)
        {
            _connString = _connString.Replace("${connectionString}", connectionString);

            return this;
        }

        public ConnStringBuilder WithDatabase(string database)
        {
            _connString = _connString.Replace("${database}", database);

            return this;
        }
        public string Get()
        {
            return _connString.ToString();
        }
    }
}