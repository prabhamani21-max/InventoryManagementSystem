namespace InventoryManagementSystem.Service.Interface
{
    public interface IInvoiceNumberService
    {
        Task<string> GenerateNextInvoiceNumberAsync();
    }
}
