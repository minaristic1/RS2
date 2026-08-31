using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionExactlyOneTargetCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Promotion_ExactlyOneTarget",
                table: "Promotions",
                sql: "(CASE WHEN MenuItemId IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN MenuCategoryId IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN MenuId IS NOT NULL THEN 1 ELSE 0 END) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Promotion_ExactlyOneTarget",
                table: "Promotions");
        }
    }
}
