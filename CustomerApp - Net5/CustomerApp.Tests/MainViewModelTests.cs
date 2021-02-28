using System;
using System.Collections.Generic;
using System.Windows.Data;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Times = Moq.Times;

using CustomerApp.ViewModel;
using CustomerLib;

namespace CustomerApp.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void Constructor_NullRepository_ShouldThrow()
        {
            Action action = () =>
            {
                var _ = new MainViewModel(null);
            };

            action.Should().Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("customerRepository"));
        }
        
        [TestMethod]
        public void Constructor_Customers_ShouldHaveValue()
        {
            var repository = Mock.Of<ICustomerRepository>();
            IEnumerable<Customer> customers = new List<Customer>();
            
            var vm =  new MainViewModel(repository);
            
            vm.Customers.Should().BeEquivalentTo(customers);
        }
        
        [TestMethod]
        public void Constructor_SelectedCustomer_ShouldBeNull()
        {
            var repository = Mock.Of<ICustomerRepository>();
           
            var vm = new MainViewModel(repository);
            
            vm.SelectedCustomer.Should().BeNull();
        }
        
        [TestMethod]
        public void AddCommand_ShouldAddInRepository()
        {
            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(repository => repository.Add(It.IsAny<Customer>())).Returns(true);
            var vm = new MainViewModel(repositoryMock.Object);
            
            vm.AddCommand.Execute(null);
            
            repositoryMock.Verify(repository => repository.Add(It.IsAny<Customer>()));
        }
        
        [TestMethod]
        public void AddCommand_SelectedCustomer_ShouldNotBeNull()
        {
            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(repository => repository.Add(It.IsAny<Customer>())).Returns(true);
            var vm = new MainViewModel(repositoryMock.Object);
            
            vm.AddCommand.Execute(null);
            
            vm.SelectedCustomer.Should().NotBeNull();
        }
        
        [TestMethod]
        public void AddCommand_ShouldNotifyCustomers()
        {
            var repository = Mock.Of<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            var wasNotified = false;
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == "Customers")
                    wasNotified = true;
            };

            vm.AddCommand.Execute(null);
            
            wasNotified.Should().BeTrue();
        }
        
        [TestMethod]
        public void RemoveCommand_SelectedCustomerNull_ShouldNotRemoveInRepository()
        {
            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(repository => repository.Remove(It.IsAny<Customer>()));
            var vm = new MainViewModel(repositoryMock.Object);
            
            vm.RemoveCommand.Execute(null);
            
            repositoryMock.Verify(repository => repository.Remove(It.IsAny<Customer>()), Times.Never);
        }
        
        [TestMethod]
        public void RemoveCommand_SelectedCustomerNotNull_ShouldRemoveInRepository()
        {
            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(repository => repository.Remove(It.IsAny<Customer>()));
            var vm = new MainViewModel(repositoryMock.Object) {SelectedCustomer = new Customer()};

            vm.RemoveCommand.Execute(null);
            
            repositoryMock.Verify(repository => repository.Remove(It.IsAny<Customer>()));
        }
        
        [TestMethod]
        public void RemoveCommand_SelectedCustomer_ShouldBeNull()
        {
            var repository = Mock.Of<ICustomerRepository>();
            var vm = new MainViewModel(repository);
            vm.SelectedCustomer = new Customer();
            
            vm.RemoveCommand.Execute(null);

            vm.SelectedCustomer.Should().BeNull();
        }
        
        [TestMethod]
        public void RemoveCommand_ShouldNotifyCustomers()
        {
            var repository = Mock.Of<ICustomerRepository>();
            var vm = new MainViewModel(repository) {SelectedCustomer = new Customer()};
            var wasNotified = false;
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == "Customers")
                    wasNotified = true;
            };

            vm.RemoveCommand.Execute(null);
            
            wasNotified.Should().BeTrue();
        }
        
        [TestMethod]
        public void SaveCommand_ShouldCommitInRepository()
        {
            var repositoryMock = new Mock<ICustomerRepository>();
            repositoryMock.Setup(repository => repository.Commit());
            var vm = new MainViewModel(repositoryMock.Object) {SelectedCustomer = new Customer()};

            vm.SaveCommand.Execute(null);
            
            repositoryMock.Verify(repository => repository.Commit());
        }
        
        [TestMethod]
        public void SearchCommand_WithText_ShouldSetFilter()
        {
            var repository = Mock.Of<ICustomerRepository>();
            var vm = new MainViewModel(repository);

            vm.SearchCommand.Execute("text");
            var collection = CollectionViewSource.GetDefaultView(vm.Customers);
            
            collection.Filter.Should().NotBeNull();
        }
        
        [TestMethod]
        public void SearchCommand_WithoutText_ShouldSetFilter()
        {
            var repository = Mock.Of<ICustomerRepository>();
            var vm = new MainViewModel(repository);

            vm.SearchCommand.Execute("");
            var collection = CollectionViewSource.GetDefaultView(vm.Customers);
            
            collection.Filter.Should().BeNull();
        }
    }
}
