using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Threading.Tasks;
using DataLayer.Migrations;
using Domain;

namespace DataLayer
{

    public class ahmadiDbContext : IdentityDbContext<ApplicationUser>
    {
        public ahmadiDbContext()
            : base("ahmadiConnectionString", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ahmadiDbContext, Configuration>());
            //Database.SetInitializer<TfShopDbContext>(null);
        }

        public static ahmadiDbContext Create()
        {
            return new ahmadiDbContext();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<CreateOrderKey> CreateOrderKeies { get; set; }
        public DbSet<Holiday> Holidais { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ProductSendWayBox> ProductSendWayBoxes { get; set; }
        public DbSet<ProductSendwayIrPostDetail> ProductSendwayIrPostDetails { get; set; }
        public DbSet<SendwayBox> SendwayBoxes { get; set; }
        public DbSet<ProductSendWayWorkTime> ProductSendWayWorkTimes { get; set; }
        public DbSet<AdministratorModule> AdministratorModules { get; set; }
        public DbSet<AdministratorPermission> AdministratorPermissions { get; set; }
        public DbSet<attachment> attachments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FormRequestCategory> FormRequestCategories { get; set; }
        public DbSet<StaticTextContent> StaticTextContents { get; set; }
        public DbSet<StaticTextCategory> StaticTextCategories { get; set; }
        public DbSet<Law> Laws { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<FormRequest> FormRequests { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<MetaSeo> MetaSeos { get; set; }
        public DbSet<ContentRating> ContentRatings { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<EmailSender> EmailSenders { get; set; }
        public DbSet<SettingState> SettingStates { get; set; }
        public DbSet<ProductRandomSetting> ProductRandomSettings { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<FileType> FileTypes { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<NewsLetterEmail> NewsLetterEmails { get; set; }
        public DbSet<OtherImage> OtherImages { get; set; }
        public DbSet<SearchEngineElementType> SearchEngineElementTypes { get; set; }
        public DbSet<SearchEngineFact> SearchEngineFacts { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ShoppingWorkTime> ShoppingWorkTimes { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<SmsSender> SmsSenders { get; set; }
        public DbSet<Social> Socials { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductMultipeItem> ProductMultipeItems { get; set; }
        public DbSet<ProductFileInfo> ProductFileInfos { get; set; }
        public DbSet<ProductFileItem> ProductFileItems { get; set; }
        public DbSet<ProductCourseLesson> ProductCourseLessons { get; set; }
        public DbSet<ProductCourse> ProductCourses { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeGroup> ProductAttributeGroups { get; set; }
        public DbSet<ProductAttributeGroupSelect> ProductAttributeGroupSelects { get; set; }
        public DbSet<ProductAttributeItem> ProductAttributeItems { get; set; }
        public DbSet<ProductAttributeItemColor> ProductAttributeItemColors { get; set; }
        public DbSet<ProductAttributeSelect> ProductAttributeSelects { get; set; }
        public DbSet<ProductAttributeTab> ProductAttributeTabs { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductPriceGroupModification> ProductPriceGroupModifications { get; set; }
        public DbSet<ProductAttributeGroupProductCategory> ProductAttributeGroupProductCategory { get; set; }
        public DbSet<ProductSeller> ProductSellers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<ProductAdvantage> ProductAdvantages { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<ProductCommentAdvantage> ProductCommentAdvantages { get; set; }
        public DbSet<ProductCommentDisAdvantage> ProductCommentDisAdvantages { get; set; }
        public DbSet<ProductDisAdvantage> ProductDisAdvantages { get; set; }
        public DbSet<ProductFavorate> ProductFavorates { get; set; }
        public DbSet<ProductLetmeknow> ProductLetmeknows { get; set; }
        public DbSet<ProductQuestion> ProductQuestions { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductCategoryAttribute> ProductCategoryAttributes { get; set; }
        public DbSet<ProductRank> ProductRanks { get; set; }
        public DbSet<ProductRankSelect> ProductRankSelects { get; set; }
        public DbSet<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        public DbSet<ProductRankGroup> ProductRankGroups { get; set; }
        public DbSet<ProductRankGroupSelect> ProductRankGroupSelects { get; set; }
        public DbSet<ProductState> ProductStates { get; set; }
        public DbSet<ProductIcon> ProductIcons { get; set; }
        public DbSet<labelIcon> labelIcons { get; set; }
        public DbSet<ProductAccessory> ProductAccessories { get; set; }
        public DbSet<ProductSendWay> ProductSendWays { get; set; }
        public DbSet<ProductSendWayDetail> ProductSendWayDetails { get; set; }
        public DbSet<ProductSendWaySelect> ProductSendWaySelects { get; set; }
        public DbSet<HelpModule> HelpModules { get; set; }
        public DbSet<HelpModuleSection> HelpModuleSections { get; set; }
        public DbSet<HelpModuleSectionField> HelpModuleSectionFields { get; set; }
        public DbSet<GalleryCategory> GalleryCategories { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankAccountOnlineInfo> BankAccountOnlineInfos { get; set; }
        public DbSet<ForWhat> ForWhats { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderAttribute> OrderAttributes { get; set; }
        public DbSet<OrderAttributeOrder> OrderAttributeOrders { get; set; }
        public DbSet<OrderRow> OrderRows { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }
        public DbSet<OrderWallet> OrderWallets { get; set; }
        public DbSet<OrderRate> OrderRates { get; set; }
        public DbSet<OrderRateItem> OrderRateItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserOfferMessage> UserOfferMessages { get; set; }
        public DbSet<UserOfferMessageMember> UserOfferMessageMembers { get; set; }
        public DbSet<UserBon> UserBons { get; set; }
        public DbSet<UserBonLog> UserBonLog { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletAttribute> WalletAttributes { get; set; }
        public DbSet<WalletAttributeWallet> WalletAttributeWallets { get; set; }
        public DbSet<ProductOffer> ProductOffers { get; set; }
        public DbSet<OfferProductCategory> OfferProductCategories { get; set; }
        public DbSet<OfferUserGroup> offerUserGroups { get; set; }
        public DbSet<UserCodeGift> UserCodeGifts { get; set; }
        public DbSet<GeneralCodeGift> GeneralCodeGift { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupSelect> UserGroupSelects { get; set; }
        public DbSet<UserCodeGiftLog> UserCodeGiftLogs { get; set; }
        public DbSet<GeneralCodeGiftLog> GeneralCodeGiftLogs { get; set; }
        public DbSet<FreeSendOffer> FreeSendOffers { get; set; }
        public DbSet<FreeSendOfferState> FreeSendOfferStates { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OrderDelivery> OrderDeliveries { get; set; }


        public DbSet<Adveresting> Adverestings { get; set; }
        public DbSet<AdverestingLog> AdverestingLogs { get; set; }
        public DbSet<Tax> Taxes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
           .HasMany(e => e.OrderAttributeSelects)
           .WithRequired(e => e.Order)
           .HasForeignKey(e => e.OrderId);

            modelBuilder.Entity<OrderAttribute>()
                .HasMany(e => e.OrderAttributeSelects)
                .WithRequired(e => e.OrderAttribute)
                .HasForeignKey(e => e.AttributeId);


            modelBuilder.Entity<Wallet>()
            .HasMany(e => e.WalletAttributeWallets)
            .WithRequired(e => e.Wallet)
            .HasForeignKey(e => e.WalletId);

            modelBuilder.Entity<WalletAttribute>()
                .HasMany(e => e.WalletAttributeWallets)
                .WithRequired(e => e.WalletAttribute)
                .HasForeignKey(e => e.WalletAttributeId);

            modelBuilder.Entity<Order>()
          .HasMany(e => e.OrderWallets)
          .WithOptional(e => e.Order)
          .HasForeignKey(e => e.OrderId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Wallet>()
                .HasMany(e => e.OrderWallets)
                .WithOptional(e => e.Wallet)
                .HasForeignKey(e => e.WalletId).WillCascadeOnDelete(false);



            modelBuilder.Configurations.Add(new Law.Configuration());
            modelBuilder.Configurations.Add(new SearchLog.Configuration());
            modelBuilder.Configurations.Add(new BankAccount.Configuration());
            modelBuilder.Configurations.Add(new Ticket.Configuration());
            modelBuilder.Configurations.Add(new TicketCategory.Configuration());
            modelBuilder.Configurations.Add(new FormRequest.Configuration());
            modelBuilder.Configurations.Add(new StaticTextContent.Configuration());
            modelBuilder.Configurations.Add(new StaticTextCategory.Configuration());
            modelBuilder.Configurations.Add(new ProductAttributeGroupProductCategory.Configuration());
            modelBuilder.Configurations.Add(new ApplicationUser.Configuration());
            modelBuilder.Configurations.Add(new OrderAttributeOrder.Configuration());
            modelBuilder.Configurations.Add(new OrderDelivery.Configuration());
            modelBuilder.Configurations.Add(new ProductSendWayBox.Configuration());
            modelBuilder.Configurations.Add(new ProductSendWayWorkTime.Configuration());
            modelBuilder.Configurations.Add(new City.Configuration());
            modelBuilder.Configurations.Add(new Folder.Configuration());
            modelBuilder.Configurations.Add(new attachment.Configuration());
            modelBuilder.Configurations.Add(new Setting.Configuration());
            modelBuilder.Configurations.Add(new ShoppingWorkTime.Configuration());
            modelBuilder.Configurations.Add(new EmailSender.Configuration());
            modelBuilder.Configurations.Add(new SettingState.Configuration());
            modelBuilder.Configurations.Add(new ProductRandomSetting.Configuration());
            modelBuilder.Configurations.Add(new SmsSender.Configuration());
            modelBuilder.Configurations.Add(new AdministratorModule.Configuration());
            modelBuilder.Configurations.Add(new AdministratorPermission.Configuration());
            modelBuilder.Configurations.Add(new Tag.Configuration());
            modelBuilder.Configurations.Add(new Category.Configuration());
            modelBuilder.Configurations.Add(new Content.Configuration());
            modelBuilder.Configurations.Add(new MetaSeo.Configuration());
            modelBuilder.Configurations.Add(new Source.Configuration());
            modelBuilder.Configurations.Add(new OtherImage.Configuration());
            modelBuilder.Configurations.Add(new Comment.Configuration());
            modelBuilder.Configurations.Add(new SliderImage.Configuration());
            modelBuilder.Configurations.Add(new Social.Configuration());
            modelBuilder.Configurations.Add(new Menu.Configuration());
            modelBuilder.Configurations.Add(new ContactUs.Configuration());
            modelBuilder.Configurations.Add(new EventLog.Configuration());
            modelBuilder.Configurations.Add(new SearchEngineElementType.Configuration());
            modelBuilder.Configurations.Add(new SearchEngineFact.Configuration());
            modelBuilder.Configurations.Add(new ContentRating.Configuration());
            modelBuilder.Configurations.Add(new CourseRating.Configuration());
            modelBuilder.Configurations.Add(new Wallet.Configuration());
            modelBuilder.Configurations.Add(new WalletAttributeWallet.Configuration());
            modelBuilder.Configurations.Add(new Order.Configuration());
            modelBuilder.Configurations.Add(new OrderRow.Configuration());
            modelBuilder.Configurations.Add(new OrderRate.Configuration());
            //modelBuilder.Configurations.Add(new Entity.Phonebook.Configuration());
            //modelBuilder.Configurations.Add(new Entity.Adveresting.Configuration());
            //modelBuilder.Configurations.Add(new Entity.AdverestingLog.Configuration());
            modelBuilder.Configurations.Add(new HelpModuleSection.Configuration());
            modelBuilder.Configurations.Add(new HelpModuleSectionField.Configuration());
            ////Product
            modelBuilder.Configurations.Add(new ProductCategory.Configuration());
            modelBuilder.Configurations.Add(new ProductCategoryAttribute.Configuration());
            modelBuilder.Configurations.Add(new Product.Configuration());
            modelBuilder.Configurations.Add(new ProductState.Configuration());
            modelBuilder.Configurations.Add(new ProductIcon.Configuration());
            modelBuilder.Configurations.Add(new labelIcon.Configuration());
            modelBuilder.Configurations.Add(new ProductAccessory.Configuration());
            modelBuilder.Configurations.Add(new ProductSendWay.Configuration());
            modelBuilder.Configurations.Add(new ProductSendWayDetail.Configuration());
            modelBuilder.Configurations.Add(new ProductSendWaySelect.Configuration());
            modelBuilder.Configurations.Add(new ProductMultipeItem.Configuration());
            modelBuilder.Configurations.Add(new ProductFileInfo.Configuration());
            modelBuilder.Configurations.Add(new ProductFileItem.Configuration());
            modelBuilder.Configurations.Add(new ProductCourse.Configuration());
            modelBuilder.Configurations.Add(new ProductCourseLesson.Configuration());
            modelBuilder.Configurations.Add(new ProductQuestion.Configuration());
            modelBuilder.Configurations.Add(new ProductComment.Configuration());
            modelBuilder.Configurations.Add(new ProductCommentAdvantage.Configuration());
            modelBuilder.Configurations.Add(new ProductCommentDisAdvantage.Configuration());
            modelBuilder.Configurations.Add(new ProductRankSelect.Configuration());
            modelBuilder.Configurations.Add(new ProductRankSelectValue.Configuration());
            modelBuilder.Configurations.Add(new ProductRankGroupSelect.Configuration());
            modelBuilder.Configurations.Add(new ProductRankGroup.Configuration());

            modelBuilder.Configurations.Add(new ProductAttributeItem.Configuration());
            modelBuilder.Configurations.Add(new ProductAttributeItemColor.Configuration());
            modelBuilder.Configurations.Add(new ProductAttributeSelect.Configuration());
            modelBuilder.Configurations.Add(new ProductAttributeGroupSelect.Configuration());
            modelBuilder.Configurations.Add(new ProductAttributeGroup.Configuration());
            modelBuilder.Configurations.Add(new Brand.Configuration());
            modelBuilder.Configurations.Add(new ProductImage.Configuration());
            modelBuilder.Configurations.Add(new ProductPrice.Configuration());
            modelBuilder.Configurations.Add(new ProductPriceGroupModification.Configuration());
            modelBuilder.Configurations.Add(new ProductSeller.Configuration());
            modelBuilder.Configurations.Add(new Seller.Configuration());

            modelBuilder.Configurations.Add(new OrderState.Configuration());
            modelBuilder.Configurations.Add(new ProductLetmeknow.Configuration());
            modelBuilder.Configurations.Add(new ProductFavorate.Configuration());
            modelBuilder.Configurations.Add(new GalleryImage.Configuration());
            modelBuilder.Configurations.Add(new GalleryCategory.Configuration());
            modelBuilder.Configurations.Add(new UserAddress.Configuration());
            modelBuilder.Configurations.Add(new UserOfferMessage.Configuration());
            modelBuilder.Configurations.Add(new UserOfferMessageMember.Configuration());
            modelBuilder.Configurations.Add(new UserMessage.Configuration());
            modelBuilder.Configurations.Add(new UserBon.Configuration());
            modelBuilder.Configurations.Add(new UserBonLog.Configuration());
            modelBuilder.Configurations.Add(new ProductAdvantage.Configuration());
            modelBuilder.Configurations.Add(new ProductDisAdvantage.Configuration());
            modelBuilder.Configurations.Add(new ProductOffer.Configuration());
            modelBuilder.Configurations.Add(new OfferProductCategory.Configuration());
            modelBuilder.Configurations.Add(new OfferUserGroup.Configuration());
            modelBuilder.Configurations.Add(new UserCodeGift.Configuration());
            modelBuilder.Configurations.Add(new GeneralCodeGift.Configuration());
            modelBuilder.Configurations.Add(new UserCodeGiftLog.Configuration());
            modelBuilder.Configurations.Add(new GeneralCodeGiftLog.Configuration());
            modelBuilder.Configurations.Add(new UserGroupSelect.Configuration());
            modelBuilder.Configurations.Add(new Offer.Configuration());
            modelBuilder.Configurations.Add(new FreeSendOffer.Configuration());
            modelBuilder.Configurations.Add(new FreeSendOfferState.Configuration());
            //modelBuilder.Configurations.Add(new Entity.ProductAttributeCategorySelect.Configuration());
            //modelBuilder.Configurations.Add(new Entity.Update.Configuration());


            modelBuilder.Configurations.Add(new Adveresting.Configuration());
            modelBuilder.Configurations.Add(new AdverestingLog.Configuration());
        }
    }
}
