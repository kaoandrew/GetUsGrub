namespace CSULB.GetUsGrub.DataAccess.Migrations.UserManagement.UserContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seed1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "GetUsGrub.AuthenticationToken",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Username = c.String(),
                        ExpiresOn = c.DateTime(nullable: false),
                        Salt = c.String(),
                        TokenString = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserAccount", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.UserAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        IsActive = c.Boolean(),
                        IsFirstTimeUser = c.Boolean(),
                        RoleType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "GetUsGrub.PasswordSalt",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserAccount", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.SecurityQuestion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        Question = c.Int(nullable: false),
                        Answer = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserAccount", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "GetUsGrub.SecurityAnswerSalt",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.SecurityQuestion", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ClaimsJson = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserAccount", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.UserProfile",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DisplayPicture = c.String(),
                        DisplayName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserAccount", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.RestaurantProfile",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PhoneNumber = c.String(),
                        Address_Street1 = c.String(nullable: false),
                        Address_Street2 = c.String(),
                        Address_City = c.String(nullable: false),
                        Address_State = c.String(nullable: false),
                        Address_Zip = c.Int(nullable: false),
                        Details_AvgFoodPrice = c.Int(nullable: false),
                        Details_HasReservations = c.Boolean(),
                        Details_HasDelivery = c.Boolean(),
                        Details_HasTakeOut = c.Boolean(),
                        Details_AcceptCreditCards = c.Boolean(),
                        Details_Attire = c.String(),
                        Details_ServesAlcohol = c.Boolean(),
                        Details_HasOutdoorSeating = c.Boolean(),
                        Details_HasTv = c.Boolean(),
                        Details_HasDriveThru = c.Boolean(),
                        Details_Caters = c.Boolean(),
                        Details_AllowsPets = c.Boolean(),
                        Details_Category = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.UserProfile", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "GetUsGrub.BusinessHour",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RestaurantId = c.Int(),
                        Day = c.String(nullable: false),
                        OpenTime = c.String(nullable: false),
                        CloseTime = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.RestaurantProfile", t => t.RestaurantId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "GetUsGrub.RestaurantMenu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RestaurantId = c.Int(),
                        MenuName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.RestaurantProfile", t => t.RestaurantId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "GetUsGrub.RestaurantMenuItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MenuId = c.Int(),
                        ItemName = c.String(),
                        ItemPrice = c.Double(nullable: false),
                        ItemPicture = c.String(),
                        Tag = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("GetUsGrub.RestaurantMenu", t => t.MenuId)
                .Index(t => t.MenuId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("GetUsGrub.AuthenticationToken", "Id", "GetUsGrub.UserAccount");
            DropForeignKey("GetUsGrub.UserProfile", "Id", "GetUsGrub.UserAccount");
            DropForeignKey("GetUsGrub.RestaurantProfile", "Id", "GetUsGrub.UserProfile");
            DropForeignKey("GetUsGrub.RestaurantMenu", "RestaurantId", "GetUsGrub.RestaurantProfile");
            DropForeignKey("GetUsGrub.RestaurantMenuItem", "MenuId", "GetUsGrub.RestaurantMenu");
            DropForeignKey("GetUsGrub.BusinessHour", "RestaurantId", "GetUsGrub.RestaurantProfile");
            DropForeignKey("GetUsGrub.UserClaims", "Id", "GetUsGrub.UserAccount");
            DropForeignKey("GetUsGrub.SecurityQuestion", "UserId", "GetUsGrub.UserAccount");
            DropForeignKey("GetUsGrub.SecurityAnswerSalt", "Id", "GetUsGrub.SecurityQuestion");
            DropForeignKey("GetUsGrub.PasswordSalt", "Id", "GetUsGrub.UserAccount");
            DropIndex("GetUsGrub.RestaurantMenuItem", new[] { "MenuId" });
            DropIndex("GetUsGrub.RestaurantMenu", new[] { "RestaurantId" });
            DropIndex("GetUsGrub.BusinessHour", new[] { "RestaurantId" });
            DropIndex("GetUsGrub.RestaurantProfile", new[] { "Id" });
            DropIndex("GetUsGrub.UserProfile", new[] { "Id" });
            DropIndex("GetUsGrub.UserClaims", new[] { "Id" });
            DropIndex("GetUsGrub.SecurityAnswerSalt", new[] { "Id" });
            DropIndex("GetUsGrub.SecurityQuestion", new[] { "UserId" });
            DropIndex("GetUsGrub.PasswordSalt", new[] { "Id" });
            DropIndex("GetUsGrub.AuthenticationToken", new[] { "Id" });
            DropTable("GetUsGrub.RestaurantMenuItem");
            DropTable("GetUsGrub.RestaurantMenu");
            DropTable("GetUsGrub.BusinessHour");
            DropTable("GetUsGrub.RestaurantProfile");
            DropTable("GetUsGrub.UserProfile");
            DropTable("GetUsGrub.UserClaims");
            DropTable("GetUsGrub.SecurityAnswerSalt");
            DropTable("GetUsGrub.SecurityQuestion");
            DropTable("GetUsGrub.PasswordSalt");
            DropTable("GetUsGrub.UserAccount");
            DropTable("GetUsGrub.AuthenticationToken");
        }
    }
}