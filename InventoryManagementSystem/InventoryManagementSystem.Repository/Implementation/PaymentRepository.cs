using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            var paymentDb = await _context.Payments
                .Include(p => p.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Payment>(paymentDb);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var paymentsDb = await _context.Payments
                .Include(p => p.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Payment>>(paymentsDb);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            var entity = _mapper.Map<PaymentDb>(payment);
            await _context.Payments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Payment>(entity);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            var entity = await _context.Payments.FindAsync(payment.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.OrderId = (int?)payment.OrderId;
            entity.OrderType = Enum.Parse<TransactionType>(payment.OrderType);
            entity.CustomerId = null; // Payment model doesn't have CustomerId
            entity.SalesPersonId = null; // Payment model doesn't have SalesPersonId
            entity.Amount = payment.Amount;
            entity.PaymentMethod = Enum.Parse<PaymentMethod>(payment.PaymentMethod);
            entity.PaymentDate = payment.PaymentDate;
            entity.ReferenceNumber = payment.ReferenceNumber;
            entity.StatusId = payment.StatusId;
            entity.UpdatedBy = payment.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Payment>(entity);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var entity = await _context.Payments.FindAsync(id);
            if (entity == null) return false;
            _context.Payments.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}