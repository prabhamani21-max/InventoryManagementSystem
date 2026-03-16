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
                .Include(p => p.Customer)
                .Include(p => p.SalesPerson)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Payment>(paymentDb);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var paymentsDb = await _context.Payments
                .Include(p => p.Status)
                .Include(p => p.Customer)
                .Include(p => p.SalesPerson)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Payment>>(paymentsDb);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            var entity = _mapper.Map<PaymentDb>(payment);
            // Set the appropriate foreign key based on OrderType
            if (payment.OrderType == TransactionType.SALE.ToString())
            {
                entity.OrderId = payment.OrderId;
            }
         
            await _context.Payments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Payment>(entity);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            var entity = await _context.Payments.FindAsync(payment.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            // Set the appropriate foreign key based on OrderType
            if (payment.OrderType == TransactionType.SALE.ToString())
            {
                entity.OrderId = payment.OrderId;
            }
            //else if (payment.OrderType == TransactionType.PURCHASE.ToString())
            //{
            //    entity.PurchaseOrderId = payment.OrderId;
            //}
            entity.OrderType = Enum.Parse<TransactionType>(payment.OrderType);
            entity.CustomerId = (int?)payment.CustomerId;
            entity.SalesPersonId = (int?)payment.SalesPersonId;
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

        /// <summary>
        /// Gets payments by order ID and order type
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="orderType">The order type (SALE, PURCHASE, etc.)</param>
        /// <returns>List of payment DB models</returns>
        public async Task<List<PaymentDb>> GetPaymentsByOrderIdAndTypeAsync(long orderId, TransactionType orderType)
        {
            if (orderType == TransactionType.SALE)
            {
                return await _context.Payments
                    .Where(p => p.OrderId == orderId && p.OrderType == orderType)
                    .ToListAsync();
            }
            //else if (orderType == TransactionType.PURCHASE)
            //{
            //    return await _context.Payments
            //        .Where(p => p.PurchaseOrderId == orderId && p.OrderType == orderType)
            //        .ToListAsync();
            //}
            else
            {
                // For other order types, we don't have a direct foreign key, but we can return empty list or handle as needed.
                // Since the original method used OrderId, and we've split it, we assume only SALE and PURCHASE are used.
                return new List<PaymentDb>();
            }
        }
    }
}
