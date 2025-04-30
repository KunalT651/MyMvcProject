using MySql.Data.MySqlClient;

namespace Cymulative1_KunalThakare.Models
{
    public class SchoolDbContext
    {
        private static string User { get { return "root"; } }
        private static string Password { get { return ""; } } //Default XAMPP has no password
        private static string Database { get { return "school"; } }
        private static string Server { get { return "localhost"; } }
        private static string Port { get { return "3306"; } } //Default XAMPP MySQL port Number

        //This string will be responsible to connect db
        protected static string ConnectionString
        {
            get
            {
                return "server = " + Server
                       + "; user = " + User
                       + "; database = " + Database
                       + "; port = " + Port
                       + "; password = " + Password
                       + "; convert zero datetime = True";
            }
        }

        public MySqlConnection AccessDatabase()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}