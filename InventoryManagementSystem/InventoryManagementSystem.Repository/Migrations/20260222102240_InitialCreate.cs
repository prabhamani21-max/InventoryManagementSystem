using System;
using InventoryManagementSytem.Common.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryManagementSystem.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:making_charge_type", "per_gram,percentage,fixed");

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.id);
                    table.ForeignKey(
                        name: "FK_category_category_parent_id",
                        column: x => x.parent_id,
                        principalTable: "category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "exchange_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exchange_order_id = table.Column<long>(type: "bigint", nullable: false),
                    metal_id = table.Column<int>(type: "integer", nullable: false),
                    purity_id = table.Column<int>(type: "integer", nullable: false),
                    gross_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    net_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    purity_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    pure_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    current_rate_per_gram = table.Column<decimal>(type: "numeric", nullable: false),
                    market_value = table.Column<decimal>(type: "numeric", nullable: false),
                    making_charge_deduction_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    wastage_deduction_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    total_deduction_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    deduction_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    credit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    item_description = table.Column<string>(type: "text", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exchange_order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    exchange_type = table.Column<string>(type: "text", nullable: false),
                    total_gross_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    total_net_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    total_pure_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    total_market_value = table.Column<decimal>(type: "numeric", nullable: false),
                    total_deduction_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_credit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    new_purchase_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    balance_refund = table.Column<decimal>(type: "numeric", nullable: true),
                    cash_payment = table.Column<decimal>(type: "numeric", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    exchange_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "generic_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generic_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    contact_number = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    dob = table.Column<DateOnly>(type: "date", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    profile_image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "metal",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metal", x => x.id);
                    table.ForeignKey(
                        name: "FK_metal_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metal_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metal_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: true),
                    order_type = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: true),
                    sales_person_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_method = table.Column<int>(type: "integer", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reference_number = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_payment_users_sales_person_id",
                        column: x => x.sales_person_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_payment_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sale_order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    is_exchange_sale = table.Column<bool>(type: "boolean", nullable: false),
                    exchange_order_id = table.Column<long>(type: "bigint", nullable: true),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_order_exchange_order_exchange_order_id",
                        column: x => x.exchange_order_id,
                        principalTable: "exchange_order",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_sale_order_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "stone",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stone", x => x.id);
                    table.ForeignKey(
                        name: "FK_stone_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stone_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stone_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "supplier",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    contact_person = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: false),
                    gst_number = table.Column<string>(type: "text", nullable: false),
                    tan_number = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplier", x => x.id);
                    table.ForeignKey(
                        name: "FK_supplier_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplier_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplier_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_kyc",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    pan_card_number = table.Column<string>(type: "text", nullable: true),
                    aadhaar_card_number = table.Column<string>(type: "text", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_kyc", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_kyc_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_kyc_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_kyc_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_kyc_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "warehouse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    manager_id = table.Column<long>(type: "bigint", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse", x => x.id);
                    table.ForeignKey(
                        name: "FK_warehouse_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_warehouse_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_warehouse_users_manager_id",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_warehouse_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    metal_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purity", x => x.id);
                    table.ForeignKey(
                        name: "FK_purity_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purity_metal_metal_id",
                        column: x => x.metal_id,
                        principalTable: "metal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purity_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purity_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "stone_rate_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stone_id = table.Column<int>(type: "integer", nullable: false),
                    carat = table.Column<decimal>(type: "numeric", nullable: true),
                    cut = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: true),
                    clarity = table.Column<string>(type: "text", nullable: true),
                    grade = table.Column<string>(type: "text", nullable: true),
                    rate_per_unit = table.Column<decimal>(type: "numeric", nullable: false),
                    effective_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stone_rate_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_stone_rate_history_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stone_rate_history_stone_stone_id",
                        column: x => x.stone_id,
                        principalTable: "stone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stone_rate_history_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stone_rate_history_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    expected_delivery_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_order_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_order_supplier_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_order_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_order_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "jewellery_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    has_stone = table.Column<bool>(type: "boolean", nullable: false),
                    stone_id = table.Column<int>(type: "integer", nullable: true),
                    metal_id = table.Column<int>(type: "integer", nullable: false),
                    purity_id = table.Column<int>(type: "integer", nullable: false),
                    gross_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    net_metal_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    making_charge_type = table.Column<MakingChargeType>(type: "making_charge_type", nullable: false),
                    making_charge_value = table.Column<decimal>(type: "numeric", nullable: false),
                    wastage_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    is_hallmarked = table.Column<bool>(type: "boolean", nullable: false),
                    huid = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    bis_certification_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    hallmark_center_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hallmark_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jewellery_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_jewellery_item_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jewellery_item_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jewellery_item_metal_metal_id",
                        column: x => x.metal_id,
                        principalTable: "metal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jewellery_item_purity_purity_id",
                        column: x => x.purity_id,
                        principalTable: "purity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jewellery_item_stone_stone_id",
                        column: x => x.stone_id,
                        principalTable: "stone",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_jewellery_item_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jewellery_item_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "metal_rate_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    purity_id = table.Column<int>(type: "integer", nullable: false),
                    rate_per_gram = table.Column<decimal>(type: "numeric", nullable: false),
                    effective_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metal_rate_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_metal_rate_history_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metal_rate_history_purity_purity_id",
                        column: x => x.purity_id,
                        principalTable: "purity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metal_rate_history_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_metal_rate_history_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_number = table.Column<string>(type: "text", nullable: false),
                    invoice_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    invoice_type = table.Column<int>(type: "integer", nullable: false),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    company_address = table.Column<string>(type: "text", nullable: true),
                    company_phone = table.Column<string>(type: "text", nullable: true),
                    company_email = table.Column<string>(type: "text", nullable: true),
                    company_gstin = table.Column<string>(type: "text", nullable: true),
                    company_pan = table.Column<string>(type: "text", nullable: true),
                    company_hallmark_license = table.Column<string>(type: "text", nullable: true),
                    party_id = table.Column<long>(type: "bigint", nullable: false),
                    party_type = table.Column<int>(type: "integer", nullable: false),
                    party_name = table.Column<string>(type: "text", nullable: false),
                    party_address = table.Column<string>(type: "text", nullable: true),
                    party_phone = table.Column<string>(type: "text", nullable: true),
                    party_email = table.Column<string>(type: "text", nullable: true),
                    party_gstin = table.Column<string>(type: "text", nullable: true),
                    party_pan = table.Column<string>(type: "text", nullable: true),
                    sale_order_id = table.Column<long>(type: "bigint", nullable: true),
                    purchase_order_id = table.Column<long>(type: "bigint", nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    taxable_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cgst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    sgst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    igst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_gst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    round_off = table.Column<decimal>(type: "numeric", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric", nullable: false),
                    grand_total_in_words = table.Column<string>(type: "text", nullable: true),
                    total_paid = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_due = table.Column<decimal>(type: "numeric", nullable: false),
                    total_gold_weight = table.Column<decimal>(type: "numeric", nullable: true),
                    total_stone_weight = table.Column<decimal>(type: "numeric", nullable: true),
                    total_pieces = table.Column<int>(type: "integer", nullable: true),
                    terms_and_conditions = table.Column<string>(type: "text", nullable: true),
                    return_policy = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    declaration = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    irn = table.Column<string>(type: "text", nullable: true),
                    irn_generated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    qr_code = table.Column<string>(type: "text", nullable: true),
                    einvoice_status = table.Column<string>(type: "text", nullable: true),
                    einvoice_cancelled_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    einvoice_cancel_reason = table.Column<string>(type: "text", nullable: true),
                    acknowledgement_number = table.Column<string>(type: "text", nullable: true),
                    acknowledgement_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_purchase_order_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_order",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_invoice_sale_order_sale_order_id",
                        column: x => x.sale_order_id,
                        principalTable: "sale_order",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_invoice_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "inventory_transaction",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jewellery_item_id = table.Column<long>(type: "bigint", nullable: false),
                    warehouse_id = table.Column<int>(type: "integer", nullable: false),
                    transaction_type = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    reference_type = table.Column<string>(type: "text", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_transaction_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_transaction_jewellery_item_jewellery_item_id",
                        column: x => x.jewellery_item_id,
                        principalTable: "jewellery_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_transaction_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_transaction_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inventory_transaction_warehouse_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_stock",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jewellery_item_id = table.Column<long>(type: "bigint", nullable: false),
                    warehouse_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reserved_quantity = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_stock", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_stock_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stock_jewellery_item_jewellery_item_id",
                        column: x => x.jewellery_item_id,
                        principalTable: "jewellery_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stock_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stock_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_item_stock_warehouse_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_stone",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    stone_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    weight = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_stone", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_stone_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stone_jewellery_item_item_id",
                        column: x => x.item_id,
                        principalTable: "jewellery_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stone_stone_stone_id",
                        column: x => x.stone_id,
                        principalTable: "stone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stone_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_stone_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItemDB",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    purchase_order_id = table.Column<long>(type: "bigint", nullable: false),
                    jewellery_item_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItemDB", x => x.id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemDB_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemDB_jewellery_item_jewellery_item_id",
                        column: x => x.jewellery_item_id,
                        principalTable: "jewellery_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemDB_purchase_order_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemDB_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemDB_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sale_order_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sale_order_id = table.Column<long>(type: "bigint", nullable: false),
                    jewellery_item_id = table.Column<long>(type: "bigint", nullable: false),
                    item_code = table.Column<string>(type: "text", nullable: true),
                    item_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    metal_id = table.Column<int>(type: "integer", nullable: false),
                    purity_id = table.Column<int>(type: "integer", nullable: false),
                    gross_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    net_metal_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    metal_rate_per_gram = table.Column<decimal>(type: "numeric", nullable: false),
                    metal_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    making_charge_type = table.Column<MakingChargeType>(type: "making_charge_type", nullable: false),
                    making_charge_value = table.Column<decimal>(type: "numeric", nullable: false),
                    total_making_charges = table.Column<decimal>(type: "numeric", nullable: false),
                    wastage_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    wastage_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    wastage_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    has_stone = table.Column<bool>(type: "boolean", nullable: false),
                    stone_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    item_subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    taxable_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    gst_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    gst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    is_hallmarked = table.Column<bool>(type: "boolean", nullable: false),
                    huid = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    bis_certification_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    hallmark_center_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hallmark_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_order_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_order_item_generic_status_status_id",
                        column: x => x.status_id,
                        principalTable: "generic_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_jewellery_item_jewellery_item_id",
                        column: x => x.jewellery_item_id,
                        principalTable: "jewellery_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_metal_metal_id",
                        column: x => x.metal_id,
                        principalTable: "metal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_purity_purity_id",
                        column: x => x.purity_id,
                        principalTable: "purity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_sale_order_sale_order_id",
                        column: x => x.sale_order_id,
                        principalTable: "sale_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sale_order_item_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "invoice_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<long>(type: "bigint", nullable: false),
                    reference_item_id = table.Column<long>(type: "bigint", nullable: true),
                    item_name = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    metal_id = table.Column<int>(type: "integer", nullable: false),
                    purity_id = table.Column<int>(type: "integer", nullable: false),
                    net_metal_weight = table.Column<decimal>(type: "numeric", nullable: true),
                    metal_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    stone_id = table.Column<int>(type: "integer", nullable: true),
                    stone_weight = table.Column<decimal>(type: "numeric", nullable: true),
                    stone_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    stone_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    making_charges = table.Column<decimal>(type: "numeric", nullable: true),
                    wastage_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    item_subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    discount = table.Column<decimal>(type: "numeric", nullable: false),
                    taxable_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cgst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    sgst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    igst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    gst_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    is_hallmarked = table.Column<bool>(type: "boolean", nullable: false),
                    huid = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    bis_certification_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    hallmark_center_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hallmark_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_item_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_item_metal_metal_id",
                        column: x => x.metal_id,
                        principalTable: "metal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_item_purity_purity_id",
                        column: x => x.purity_id,
                        principalTable: "purity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_payment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_id = table.Column<long>(type: "bigint", nullable: false),
                    allocated_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_payment_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_payment_payment_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tcs_transactions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    financial_year = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    pan_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    sale_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    cumulative_sale_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    tcs_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    tcs_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    tcs_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_exempted = table.Column<bool>(type: "boolean", nullable: false),
                    exemption_reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    transaction_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    quarter = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false),
                    updated_by = table.Column<long>(type: "bigint", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcs_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_tcs_transactions_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tcs_transactions_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tcs_transactions_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tcs_transactions_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_category_created_by",
                table: "category",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_category_parent_id",
                table: "category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_status_id",
                table: "category",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_updated_by",
                table: "category",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_created_by",
                table: "exchange_item",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_exchange_order_id",
                table: "exchange_item",
                column: "exchange_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_metal_id",
                table: "exchange_item",
                column: "metal_id");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_purity_id",
                table: "exchange_item",
                column: "purity_id");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_status_id",
                table: "exchange_item",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_item_updated_by",
                table: "exchange_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_order_CreatedByUserId",
                table: "exchange_order",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_order_customer_id",
                table: "exchange_order",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_order_status_id",
                table: "exchange_order",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_generic_status_created_by",
                table: "generic_status",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_generic_status_updated_by",
                table: "generic_status",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transaction_created_by",
                table: "inventory_transaction",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transaction_jewellery_item_id",
                table: "inventory_transaction",
                column: "jewellery_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transaction_status_id",
                table: "inventory_transaction",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transaction_updated_by",
                table: "inventory_transaction",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transaction_warehouse_id",
                table: "inventory_transaction",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_created_by",
                table: "invoice",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_purchase_order_id",
                table: "invoice",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_sale_order_id",
                table: "invoice",
                column: "sale_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_status_id",
                table: "invoice",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_updated_by",
                table: "invoice",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_invoice_id",
                table: "invoice_item",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_metal_id",
                table: "invoice_item",
                column: "metal_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_purity_id",
                table: "invoice_item",
                column: "purity_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_payment_invoice_id",
                table: "invoice_payment",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_payment_payment_id",
                table: "invoice_payment",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stock_created_by",
                table: "item_stock",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_stock_jewellery_item_id",
                table: "item_stock",
                column: "jewellery_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stock_status_id",
                table: "item_stock",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stock_updated_by",
                table: "item_stock",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_stock_warehouse_id",
                table: "item_stock",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stone_created_by",
                table: "item_stone",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_stone_item_id",
                table: "item_stone",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stone_status_id",
                table: "item_stone",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stone_stone_id",
                table: "item_stone",
                column: "stone_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_stone_updated_by",
                table: "item_stone",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_category_id",
                table: "jewellery_item",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_created_by",
                table: "jewellery_item",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_metal_id",
                table: "jewellery_item",
                column: "metal_id");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_purity_id",
                table: "jewellery_item",
                column: "purity_id");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_status_id",
                table: "jewellery_item",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_stone_id",
                table: "jewellery_item",
                column: "stone_id");

            migrationBuilder.CreateIndex(
                name: "IX_jewellery_item_updated_by",
                table: "jewellery_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_metal_created_by",
                table: "metal",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_metal_status_id",
                table: "metal",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_metal_updated_by",
                table: "metal",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_metal_rate_history_created_by",
                table: "metal_rate_history",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_metal_rate_history_purity_id",
                table: "metal_rate_history",
                column: "purity_id");

            migrationBuilder.CreateIndex(
                name: "IX_metal_rate_history_status_id",
                table: "metal_rate_history",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_metal_rate_history_updated_by",
                table: "metal_rate_history",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_created_by",
                table: "payment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_payment_customer_id",
                table: "payment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_sales_person_id",
                table: "payment",
                column: "sales_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_status_id",
                table: "payment",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_updated_by",
                table: "payment",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_created_by",
                table: "purchase_order",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_status_id",
                table: "purchase_order",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_supplier_id",
                table: "purchase_order",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_updated_by",
                table: "purchase_order",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemDB_created_by",
                table: "PurchaseOrderItemDB",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemDB_jewellery_item_id",
                table: "PurchaseOrderItemDB",
                column: "jewellery_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemDB_purchase_order_id",
                table: "PurchaseOrderItemDB",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemDB_status_id",
                table: "PurchaseOrderItemDB",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemDB_updated_by",
                table: "PurchaseOrderItemDB",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_purity_created_by",
                table: "purity",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_purity_metal_id",
                table: "purity",
                column: "metal_id");

            migrationBuilder.CreateIndex(
                name: "IX_purity_status_id",
                table: "purity",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_purity_updated_by",
                table: "purity",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_roles_status_id",
                table: "roles",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_created_by",
                table: "sale_order",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_customer_id",
                table: "sale_order",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_exchange_order_id",
                table: "sale_order",
                column: "exchange_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_status_id",
                table: "sale_order",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_updated_by",
                table: "sale_order",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_created_by",
                table: "sale_order_item",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_jewellery_item_id",
                table: "sale_order_item",
                column: "jewellery_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_metal_id",
                table: "sale_order_item",
                column: "metal_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_purity_id",
                table: "sale_order_item",
                column: "purity_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_sale_order_id",
                table: "sale_order_item",
                column: "sale_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_status_id",
                table: "sale_order_item",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_order_item_updated_by",
                table: "sale_order_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_stone_created_by",
                table: "stone",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_stone_status_id",
                table: "stone",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_stone_updated_by",
                table: "stone",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_stone_rate_history_created_by",
                table: "stone_rate_history",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_stone_rate_history_status_id",
                table: "stone_rate_history",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_stone_rate_history_stone_id",
                table: "stone_rate_history",
                column: "stone_id");

            migrationBuilder.CreateIndex(
                name: "IX_stone_rate_history_updated_by",
                table: "stone_rate_history",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_created_by",
                table: "supplier",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_status_id",
                table: "supplier",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_updated_by",
                table: "supplier",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_tcs_transactions_created_by",
                table: "tcs_transactions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_tcs_transactions_customer_id",
                table: "tcs_transactions",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tcs_transactions_invoice_id",
                table: "tcs_transactions",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_tcs_transactions_updated_by",
                table: "tcs_transactions",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_kyc_created_by",
                table: "user_kyc",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_kyc_status_id",
                table: "user_kyc",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_kyc_updated_by",
                table: "user_kyc",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_kyc_user_id",
                table: "user_kyc",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_by",
                table: "users",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_status_id",
                table: "users",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_updated_by",
                table: "users",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_created_by",
                table: "warehouse",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_manager_id",
                table: "warehouse",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_status_id",
                table: "warehouse",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_updated_by",
                table: "warehouse",
                column: "updated_by");

            migrationBuilder.AddForeignKey(
                name: "FK_category_generic_status_status_id",
                table: "category",
                column: "status_id",
                principalTable: "generic_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_category_users_created_by",
                table: "category",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_category_users_updated_by",
                table: "category",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_exchange_order_exchange_order_id",
                table: "exchange_item",
                column: "exchange_order_id",
                principalTable: "exchange_order",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_generic_status_status_id",
                table: "exchange_item",
                column: "status_id",
                principalTable: "generic_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_metal_metal_id",
                table: "exchange_item",
                column: "metal_id",
                principalTable: "metal",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_purity_purity_id",
                table: "exchange_item",
                column: "purity_id",
                principalTable: "purity",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_users_created_by",
                table: "exchange_item",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_item_users_updated_by",
                table: "exchange_item",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_order_generic_status_status_id",
                table: "exchange_order",
                column: "status_id",
                principalTable: "generic_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_order_users_CreatedByUserId",
                table: "exchange_order",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exchange_order_users_customer_id",
                table: "exchange_order",
                column: "customer_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_generic_status_users_created_by",
                table: "generic_status",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_generic_status_users_updated_by",
                table: "generic_status",
                column: "updated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_generic_status_status_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_generic_status_status_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "exchange_item");

            migrationBuilder.DropTable(
                name: "inventory_transaction");

            migrationBuilder.DropTable(
                name: "invoice_item");

            migrationBuilder.DropTable(
                name: "invoice_payment");

            migrationBuilder.DropTable(
                name: "item_stock");

            migrationBuilder.DropTable(
                name: "item_stone");

            migrationBuilder.DropTable(
                name: "metal_rate_history");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItemDB");

            migrationBuilder.DropTable(
                name: "sale_order_item");

            migrationBuilder.DropTable(
                name: "stone_rate_history");

            migrationBuilder.DropTable(
                name: "tcs_transactions");

            migrationBuilder.DropTable(
                name: "user_kyc");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "warehouse");

            migrationBuilder.DropTable(
                name: "jewellery_item");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "purity");

            migrationBuilder.DropTable(
                name: "stone");

            migrationBuilder.DropTable(
                name: "purchase_order");

            migrationBuilder.DropTable(
                name: "sale_order");

            migrationBuilder.DropTable(
                name: "metal");

            migrationBuilder.DropTable(
                name: "supplier");

            migrationBuilder.DropTable(
                name: "exchange_order");

            migrationBuilder.DropTable(
                name: "generic_status");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
