using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private static CustomerRepository _instance;
        private static readonly object _lock = new object();
        private List<Customer> _customers;

        private CustomerRepository()
        {
            InitializeData();
        }

        public static CustomerRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new CustomerRepository();
                    }
                }
                return _instance;
            }
        }

        private void InitializeData()
        {
            try
            {
                _customers = new List<Customer>
                {
                    new Customer
                    {
                        CustomerID = 1,
                        CustomerFullName = "Lê Thanh Lâm",
                        Telephone = "0123456789",
                        EmailAddress = "lamltde180684@fpt.edu.vn",
                        CustomerBirthday = new DateTime(2004, 09, 10),
                        CustomerStatus = 1,
                        Password = "123"
                    },
                    new Customer
                    {
                        CustomerID = 2,
                        CustomerFullName = "Abc",
                        Telephone = "0123401234",
                        EmailAddress = "abc@email.com",
                        CustomerBirthday = new DateTime(2001, 04, 05),
                        CustomerStatus = 1,
                        Password = "abc"
                    }
                };
            }
            catch (Exception)
            {
                _customers = new List<Customer>();
            }
        }

        public List<Customer> GetAllCustomers()
        {
            try
            {
                return _customers?.Where(c => c != null && c.CustomerStatus == 1).ToList() ?? new List<Customer>();
            }
            catch (Exception)
            {
                return new List<Customer>();
            }
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                return _customers?.FirstOrDefault(c => c != null && c.CustomerID == id && c.CustomerStatus == 1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Customer GetCustomerByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || _customers == null)
                    return null;

                return _customers.FirstOrDefault(c => c != null && !string.IsNullOrEmpty(c.EmailAddress) 
                && string.Equals(c.EmailAddress, email, StringComparison.OrdinalIgnoreCase) 
                && c.CustomerStatus == 1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddCustomer(Customer customer)
        {
            try
            {
                if (customer == null || _customers == null) return;

                customer.CustomerID = _customers.Count > 0 ? _customers.Max(c => c?.CustomerID ?? 0) + 1 : 1;
                customer.CustomerStatus = 1;
                _customers.Add(customer);
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            try
            {
                if (customer == null || _customers == null) return;

                var existingCustomer = _customers.FirstOrDefault(c => c != null && c.CustomerID == customer.CustomerID);
                if (existingCustomer != null)
                {
                    existingCustomer.CustomerFullName = customer.CustomerFullName ?? existingCustomer.CustomerFullName;
                    existingCustomer.Telephone = customer.Telephone ?? existingCustomer.Telephone;
                    existingCustomer.EmailAddress = customer.EmailAddress ?? existingCustomer.EmailAddress;
                    existingCustomer.CustomerBirthday = customer.CustomerBirthday;
                    existingCustomer.Password = customer.Password ?? existingCustomer.Password;
                }
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public void DeleteCustomer(int id)
        {
            try
            {
                if (_customers == null) return;

                var customer = _customers.FirstOrDefault(c => c != null && c.CustomerID == id);
                if (customer != null)
                {
                    customer.CustomerStatus = 2;
                }
            }
            catch (Exception)
            {
                // Log error or handle as needed
            }
        }

        public List<Customer> SearchCustomers(string searchTerm)
        {
            try
            {
                if (_customers == null)
                    return new List<Customer>();

                if (string.IsNullOrEmpty(searchTerm))
                    return GetAllCustomers();

                return _customers.Where(c => c != null && c.CustomerStatus == 1 &&
                    ((!string.IsNullOrEmpty(c.CustomerFullName) && c.CustomerFullName.ToLower().Contains(searchTerm.ToLower())) ||
                     (!string.IsNullOrEmpty(c.EmailAddress) && c.EmailAddress.ToLower().Contains(searchTerm.ToLower())) ||
                     (!string.IsNullOrEmpty(c.Telephone) && c.Telephone.Contains(searchTerm)))).ToList();
            }
            catch (Exception)
            {
                return new List<Customer>();
            }
        }
    }
}
