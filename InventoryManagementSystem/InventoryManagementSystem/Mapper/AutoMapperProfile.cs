using AutoMapper;
using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSytem.Common.Enums;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            // UserDb to User mapping with navigation properties for Role and Status
            CreateMap<UserDb, User>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null));
            // User to UserDb mapping (for reverse operations)
            CreateMap<User, UserDb>();
            // User to UserDto mapping with RoleName and StatusName
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusName));
            CreateMap<RoleDb, Role>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<GenericStatusDb, GenericStatus>().ReverseMap();
            CreateMap<GenericStatus, StatusDto>().ReverseMap();
            CreateMap<UserKycDto, UserKyc>().ReverseMap();
            CreateMap<UserKyc, UserKycDb>().ReverseMap();
            CreateMap<WarehouseDto, Warehouse>().ReverseMap();
            // WarehouseDb to Warehouse mapping with navigation property for Manager
            CreateMap<WarehouseDb, Warehouse>()
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Name : null));
            // Warehouse to WarehouseDto mapping with ManagerName
            CreateMap<Warehouse, WarehouseDto>()
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.ManagerName));
            CreateMap<Warehouse, WarehouseDb>().ReverseMap();
            CreateMap<SupplierDto, Supplier>().ReverseMap();
            CreateMap<Supplier, SupplierDb>().ReverseMap();
            CreateMap<StoneDto, Stone>().ReverseMap();
            CreateMap<Stone, StoneDb>().ReverseMap();
            CreateMap<SaleOrderItemDto, SaleOrderItem>().ReverseMap();
            CreateMap<SaleOrderItem, SaleOrderItemDb>().ReverseMap();
            // Db to DTO mapping with navigation properties for SaleOrderItem
            CreateMap<SaleOrderItemDb, SaleOrderItemDto>()
                .ForMember(dest => dest.SaleOrderNumber, opt => opt.MapFrom(src => src.SaleOrder != null ? src.SaleOrder.OrderNumber : null));
            // Db to Common model mapping with navigation properties for SaleOrderItem
            CreateMap<SaleOrderItemDb, SaleOrderItem>()
                .ForMember(dest => dest.SaleOrderNumber, opt => opt.MapFrom(src => src.SaleOrder != null ? src.SaleOrder.OrderNumber : null));
            CreateMap<SaleOrderDto, SaleOrder>().ReverseMap();
            CreateMap<SaleOrder, SaleOrderDb>().ReverseMap();
            // Db to DTO mapping with navigation properties for SaleOrder
            CreateMap<SaleOrderDb, SaleOrderDto>()
                .ForMember(dest => dest.ExchangeOrderNumber, opt => opt.MapFrom(src => src.ExchangeOrder != null ? src.ExchangeOrder.OrderNumber : null));
            CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderDb>().ReverseMap();
            // Payment mappings
            CreateMap<PaymentDto, Payment>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()));
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => ParsePaymentMethod(src.PaymentMethod)));
            CreateMap<Payment, PaymentDb>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => ParsePaymentMethod(src.PaymentMethod)));
            // PaymentDb to Payment with navigation properties for Customer and SalesPerson names
            CreateMap<PaymentDb, Payment>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null));
            // PaymentDb to PaymentDto with navigation properties for Customer and SalesPerson names
            CreateMap<PaymentDb, PaymentDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null));
            CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>().ReverseMap();
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDb>().ReverseMap();
            CreateMap<PurityDto, Purity>().ReverseMap();
            CreateMap<Purity, PurityDb>().ReverseMap();
            CreateMap<MetalDto, Metal>().ReverseMap();
            CreateMap<Metal, MetalDb>().ReverseMap();
            CreateMap<JewelleryItemDto, JewelleryItem>().ReverseMap();
            CreateMap<JewelleryItem, JewelleryItemDb>().ReverseMap();
            // Db to DTO mapping with navigation properties
            CreateMap<JewelleryItemDb, JewelleryItemDto>()
                .ForMember(dest => dest.MetalName, opt => opt.MapFrom(src => src.Metal != null ? src.Metal.Name : null))
                .ForMember(dest => dest.PurityName, opt => opt.MapFrom(src => src.Purity != null ? src.Purity.Name : null));
            CreateMap<ItemStoneDto, ItemStone>().ReverseMap();
            CreateMap<ItemStone, ItemStoneDb>().ReverseMap();
            CreateMap<ItemStockDto, ItemStock>().ReverseMap();
            CreateMap<ItemStock, ItemStockDb>().ReverseMap();
            // Stone Rate History mappings
            CreateMap<StoneRateHistoryDb, StoneRateHistory>().ReverseMap();
            CreateMap<StoneRateHistoryDb, StoneRateDto>()
                .ForMember(dest => dest.StoneName, opt => opt.MapFrom(src => src.Stone != null ? src.Stone.Name : null))
                .ForMember(dest => dest.StoneUnit, opt => opt.MapFrom(src => src.Stone != null ? src.Stone.Unit : null))
                .ReverseMap();
            CreateMap<StoneRateHistory, StoneRateDto>()
                .ForMember(dest => dest.StoneName, opt => opt.MapFrom(src => src.Stone != null ? src.Stone.Name : null))
                .ForMember(dest => dest.StoneUnit, opt => opt.MapFrom(src => src.Stone != null ? src.Stone.Unit : null))
                .ReverseMap();
            CreateMap<StoneRateHistory, StoneRateCreateDto>().ReverseMap();
            // Metal Rate History mappings
            CreateMap<MetalRateHistoryDb, MetalRateHistory>().ReverseMap();

            // MetalRate DTO mappings from Common.Models.MetalRateHistory
            CreateMap<MetalRateCreateDto, MetalRateHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore());

            CreateMap<MetalRateUpdateDto, MetalRateHistory>()
                .ForMember(dest => dest.PurityId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore());

            CreateMap<MetalRateHistory, MetalRateDto>();
            CreateMap<MetalRateHistory, MetalRateResponseDto>();
            CreateMap<MetalRateHistory, MetalRateHistoryDto>();
            // Exchange Order mappings
            CreateMap<ExchangeOrderDb, ExchangeOrder>()
                .ForMember(dest => dest.ExchangeType, opt => opt.MapFrom(src => src.ExchangeType == "BUYBACK" ? 2 : 1))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ExchangeItems));
            CreateMap<ExchangeOrder, ExchangeOrderDb>()
                .ForMember(dest => dest.ExchangeType, opt => opt.MapFrom(src => src.ExchangeType == 2 ? "BUYBACK" : "EXCHANGE"))
                .ForMember(dest => dest.ExchangeItems, opt => opt.MapFrom(src => src.Items));
            CreateMap<ExchangeItemDb, ExchangeItem>().ReverseMap();

            // Exchange calculation mappings
            CreateMap<ExchangeItemInputDto, ExchangeItemInput>().ReverseMap();
            CreateMap<ExchangeItemInputDto, ExchangeItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.PurityPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.PureWeight, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentRatePerGram, opt => opt.Ignore())
                .ForMember(dest => dest.MarketValue, opt => opt.Ignore())
                .ForMember(dest => dest.DeductionAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CreditAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeOrder, opt => opt.Ignore());
            CreateMap<ExchangeItemCalculation, ExchangeItemResponseDto>().ReverseMap();
            CreateMap<ExchangeItemCalculation, ExchangeItemDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ItemDescription, opt => opt.Ignore());
            CreateMap<ExchangeItem, ExchangeItemDto>();
            CreateMap<ExchangeCalculationResult, ExchangeCalculateResponseDto>().ReverseMap();
            CreateMap<ExchangeOrder, ExchangeOrderDto>()
                .ForMember(dest => dest.ExchangeType, opt => opt.MapFrom(src => src.ExchangeType == 2 ? "BUYBACK" : "EXCHANGE"));

            // ExchangeOrderCreateDto to ExchangeOrder mapping
            CreateMap<ExchangeOrderCreateDto, ExchangeOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalGrossWeight, opt => opt.Ignore())
                .ForMember(dest => dest.TotalNetWeight, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPureWeight, opt => opt.Ignore())
                .ForMember(dest => dest.TotalMarketValue, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDeductionAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalCreditAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            // Invoice mappings
            CreateMap<InvoiceDb, Invoice>().ReverseMap();
            CreateMap<InvoiceItemDb, InvoiceItem>()
                .ForMember(dest => dest.MetalType, opt => opt.MapFrom(src => src.Metal != null ? src.Metal.Name : null))
                .ForMember(dest => dest.Purity, opt => opt.MapFrom(src => src.Purity != null ? src.Purity.Name : null))
                .ForMember(dest => dest.HallmarkDetails, opt => opt.MapFrom(src =>
                    src.IsHallmarked
                        ? string.Join(", ", new[]
                        {
                            src.BISCertificationNumber,
                            src.HallmarkCenterName,
                            src.HallmarkDate.HasValue ? src.HallmarkDate.Value.ToString("dd MMM yyyy") : null
                        }.Where(value => !string.IsNullOrWhiteSpace(value)))
                        : null))
                .ForMember(dest => dest.TotalMakingCharges, opt => opt.MapFrom(src => src.MakingCharges));
            CreateMap<InvoiceItem, InvoiceItemDb>()
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.Metal, opt => opt.Ignore())
                .ForMember(dest => dest.Purity, opt => opt.Ignore());
            CreateMap<InvoicePaymentDb, InvoicePayment>().ReverseMap();

            // Category mappings
            CreateMap<CategoryDb, Category>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryResponseDto>().ReverseMap();
        }

        private static PaymentMethod ParsePaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                return PaymentMethod.CASH;
            }

            if (Enum.TryParse<PaymentMethod>(paymentMethod, true, out var parsedMethod) &&
                Enum.IsDefined(typeof(PaymentMethod), parsedMethod))
            {
                return parsedMethod;
            }

            if (int.TryParse(paymentMethod, out var numericValue) &&
                Enum.IsDefined(typeof(PaymentMethod), numericValue))
            {
                return (PaymentMethod)numericValue;
            }

            return PaymentMethod.CASH;
        }
    }
}
