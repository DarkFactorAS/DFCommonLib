using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DFCommonLib.Config
{
    public class CustomerConfiguration
    {
        public virtual IEnumerable<Customer> GetCustomer(string customerName)
        {
            return null;
        }

        public virtual Customer GetFirstCustomer()
        {
            return null;
        }
    }

    public class CustomerConfiguration<T> : CustomerConfiguration where T:Customer,new()
    {
        public IEnumerable<T> Customers { get; set; }

/*        public override IEnumerable<Customer> GetCustomer(string customerName)
        {
            return Customers.Where(x => x.Id == customerName).FirstOrDefault();
        }*/

        public override Customer GetFirstCustomer()
        {
            return Customers.FirstOrDefault();
        }
    }

    public class Customer
    {
        public string Id { get; set; }
        public IEnumerable<DatabaseConnection> DatabaseConnections { get; set; }

        public Customer()
        {            
        }

        public DatabaseConnection GetDbConnection(string connectionType)
        {
            return DatabaseConnections.Where(x => x.ConnectionType == connectionType).FirstOrDefault();
        }
    }

    public class DatabaseConnection
    {
        public string ConnectionType { get; set; }
        public string ConnectionString { get; set; }
    }
}
