using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Data
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if data already exists
                if (await _context.Status.AnyAsync())
                {
                    _logger.LogInformation("Database already seeded. Skipping...");
                    return;
                }

                _logger.LogInformation("Starting database seeding...");

                // Disable triggers on the tables to bypass foreign key constraints
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE generic_status DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE roles DISABLE TRIGGER ALL");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE users DISABLE TRIGGER ALL");

                try
                {
                    // Step 1: Insert status records
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO generic_status (id, name, is_active, created_date, created_by) VALUES
                        (1, 'Active', true, NOW(), 1),
                        (2, 'Inactive', true, NOW(), 1),
                        (3, 'Deleted', true, NOW(), 1)
                    ");
                    _logger.LogInformation("Seeded 3 status records");

                    // Step 2: Insert role records
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO roles (id, name, status_id, created_date, created_by) VALUES
                        (1, 'SuperAdmin', 1, NOW(), 1),
                        (2, 'Manager', 1, NOW(), 1),
                        (3, 'Sales', 1, NOW(), 1),
                        (4, 'Customer', 1, NOW(), 1)
                    ");
                    _logger.LogInformation("Seeded 4 role records");

                    // Step 3: Insert the user
                    await _context.Database.ExecuteSqlRawAsync(@"
                        INSERT INTO users (id, name, email, password, contact_number, gender, address, dob, role_id, status_id, created_by, created_date, profile_image)
                        VALUES (1, 'Santosh', 'santosh@gmail.com', 'ff7bd97b1a7789ddd2775122fd6817f3173672da9f802ceec57f284325bf589f', '8762598315', 1, 'Bangalore', '1991-11-21', 1, 1, 1, NOW(), NULL)
                    ");
                    _logger.LogInformation("Seeded super admin user: santosh@gmail.com");
                }
                finally
                {
                    // Re-enable triggers
                    await _context.Database.ExecuteSqlRawAsync("ALTER TABLE generic_status ENABLE TRIGGER ALL");
                    await _context.Database.ExecuteSqlRawAsync("ALTER TABLE roles ENABLE TRIGGER ALL");
                    await _context.Database.ExecuteSqlRawAsync("ALTER TABLE users ENABLE TRIGGER ALL");
                }

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}
