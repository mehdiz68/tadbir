using DataLayer;
using Domain;
using Repository;
using Repository.Service;
using System;

namespace UnitOfWork
{

    public class UnitOfWorkClass : IDisposable
    {
        public ahmadiDbContext context = new ahmadiDbContext();
        private CreateOrderKeyService createOrderKeyService;
        private GenericRepository<ApplicationUser> userRepository;
        private GenericRepository<attachment> attachmentRepository;
        private GenericRepository<EventLog> eventLogRepository;
        private GenericRepository<Setting> settingRepository;
        private GenericRepository<ShoppingWorkTime> shoppingWorkTimeRepository;
        private GenericRepository<Tax> taxRepository;
        private GenericRepository<Folder> folderRepository;
        private GenericRepository<FileType> filetypeRepository;
        private GenericRepository<EmailSender> emailsenderRepository;
        private GenericRepository<SettingState> settingStateRepository;
        private GenericRepository<ProductRandomSetting> productRandomSettingRepository;
        private GenericRepository<SmsSender> smssenderRepository;
        private GenericRepository<AdministratorModule> administratorModuleRepository;
        private GenericRepository<AdministratorPermission> administratorPermissionRepository;
        private GenericRepository<NewsLetterEmail> newsLetterEmailRepository;
        private GenericRepository<Slider> sliderRepository;
        private GenericRepository<SliderImage> sliderImageRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<TicketCategory> ticketCategoryRepository;
        private GenericRepository<FormRequest> formRequestRepository;
        private GenericRepository<FormRequestCategory> formRequestCategoryRepository;
        private GenericRepository<StaticTextCategory> staticTextCategoryCategoryRepository;
        private GenericRepository<StaticTextContent> staticTextContentCategoryRepository;
        private GenericRepository<Comment> commentRepository;
        private GenericRepository<ContactUs> contactUsRepository;
        private GenericRepository<Content> contentRepository;
        private GenericRepository<Ticket> ticketRepository;
        private GenericRepository<MetaSeo> metaSeoRepository;
        private GenericRepository<ContentRating> contentRatingRepository;
        private GenericRepository<CourseRating> courseRatingRepository;
        private GenericRepository<Menu> menuRepository;
        private GenericRepository<OtherImage> otherImageRepository;
        private GenericRepository<SearchEngineElementType> searchEngineElementTypeRepository;
        private GenericRepository<SearchEngineFact> searchEngineFactRepository;
        private GenericRepository<Social> socialRepository;
        private GenericRepository<Source> sourceRepository;
        private GenericRepository<Tag> tagRepository;
        private ProductService productRepository;

        private GenericRepository<Law> lawRepository;
        private GenericRepository<SearchLog> searchLogRepository;
        private GenericRepository<ProductType> productType;
        private GenericRepository<ProductMultipeItem> productMultipeItem;
        private GenericRepository<ProductFileInfo> productFileInfo;
        private GenericRepository<ProductFileItem> productFileItem;

        private GenericRepository<ProductState> productState;
        private GenericRepository<ProductIcon> productIcon;
        private GenericRepository<ProductAccessory> productAccessory;
        private GenericRepository<ProductSendWay> productSendWay;
        private GenericRepository<ProductSendWayDetail> productSendWayDetail;
        private GenericRepository<ProductSendWaySelect> productSendWaySelect;
        private GenericRepository<ProductSendwayIrPostDetail> productSendwayIrPostDetails;

        private GenericRepository<ProductAttribute> productAttributeRepository;
        private GenericRepository<ProductAttributeGroup> productAttributeGroupRepository;
        private GenericRepository<ProductAttributeGroupSelect> productAttributeGroupSelectRepository;
        private GenericRepository<ProductAttributeItem> productAttributeItemRepository;
        private GenericRepository<ProductAttributeItemColor> productAttributeItemColorRepository;
        private ProductAttributeSelectService productAttributeSelectRepository;
        private GenericRepository<ProductAttributeTab> productAttributeTabRepository;
        private GenericRepository<Brand> brandRepository;
        private GenericRepository<ProductImage> productImageRepository;
        private ProductPriceService productPriceRepository;
        private GenericRepository<ProductSeller> productSellerRepository;
        private GenericRepository<ProductOffer> productOfferRepository;
        private GenericRepository<OfferProductCategory> offerProductCategoryRepository;
        private GenericRepository<OfferUserGroup> offerUserGroupRepository;
        private GenericRepository<ProductPriceGroupModification> productPriceGroupModificationRepository;

        private GenericRepository<ProductRank> productRankRepository;
        private GenericRepository<ProductRankGroup> productRankGroupRepository;
        private GenericRepository<ProductRankGroupSelect> productRankGroupSelectRepository;
        private GenericRepository<ProductRankSelect> productRankSelectRepository;
        private GenericRepository<ProductRankSelectValue> productRankSelectValueRepository;
        private GenericRepository<ProductCourse> productCourseRepository;
        private GenericRepository<ProductCourseLesson> productCourseLessonRepository;

        private GenericRepository<ProductAdvantage> productAdvantageRepository;
        private ProductCategoryService productCategoryRepository;
        private ProductCommentService productCommentRepository;
        private GenericRepository<ProductCommentAdvantage> productCommentAdvantageRepository;
        private GenericRepository<ProductCommentDisAdvantage> productCommentDisAdvantageRepository;
        private GenericRepository<ProductDisAdvantage> productDisAdvantageRepository;
        private ProductFavorateService productFavorateRepository;
        private ProductLetmeKnowService productLetmeknowRepository;
        private ProductQuestionService productQuestionRepository;
        private GenericRepository<HelpModule> helpModuleRepository;
        private GenericRepository<HelpModuleSection> helpModuleSectionRepository;
        private GenericRepository<HelpModuleSectionField> helpModuleSectionFieldRepository;
        private GenericRepository<GalleryCategory> galleryCategoryRepository;
        private GenericRepository<GalleryImage> galleryImageRepository;

        private GenericRepository<Seller> sellerRepository;
        private GenericRepository<Offer> offerRepository;

        private BankAccountService bankAccountRepository;
        private GenericRepository<BankAccountOnlineInfo> bankAccountOnlineInfoRepository;
        private GenericRepository<ForWhat> forWhatRepository;
        private GenericRepository<OrderRate> orderRateRepository;
        private GenericRepository<OrderRateItem> orderRateItemRepository;
        private OrderService orderRepository;
        private GenericRepository<OrderAttribute> orderAttributeRepository;
        private OrderAttributeOrderService orderAttributeOrderRepository;
        private GenericRepository<OrderRow> orderRowRepository;
        private OrderStateService orderStateRepository;
        private GenericRepository<OrderWallet> orderWalletRepository;
        private GenericRepository<OrderDelivery> orderDeliveryRepository;
        private UserAddressService userAddressRepository;
        private UserBonService userBonRepository;
        private GenericRepository<UserBonLog> userBonLogRepository;
        private GenericRepository<GeneralCodeGift> generalCodeGiftRepository;
        private UserCodeGiftService userCodeGiftRepository;
        private GenericRepository<UserCodeGiftLog> userCodeGiftLogRepository;
        private GenericRepository<GeneralCodeGiftLog> generalCodeGiftLogRepository;
        private GenericRepository<UserGroup> userGroupRepository;
        private GenericRepository<UserGroupSelect> userGroupSelectRepository;
        private GenericRepository<FreeSendOffer> freeSendOfferRepository;
        private GenericRepository<FreeSendOfferState> freeSendOfferStateRepository;
        private GenericRepository<UserMessage> userMessageRepository;
        private GenericRepository<UserOfferMessage> userOfferMessageRepository;
        private GenericRepository<UserOfferMessageMember> userOfferMessageMemberRepository;
        private GenericRepository<Wallet> walletRepository;
        private GenericRepository<WalletAttribute> walletAttributeRepository;
        private GenericRepository<WalletAttributeWallet> walletAttributeWalletRepository;

        private GenericRepository<Adveresting> adverestingRepository;
        private GenericRepository<AdverestingLog> adverestingLogRepository;

        private GenericRepository<SendwayBox> sendwayBoxRepository;
        private GenericRepository<ProductSendWayBox> productSendWayBoxRepository;
        private GenericRepository<ProductSendWayWorkTime> productSendWayWorkTimeRepository;

        private GenericRepository<City> cityRepository;
        private GenericRepository<Province> provinceRepository;

        private ProductCategoryAttributeService productCategoryAttributeRepository;
        private ProductAttributeGroupProductCategorysService productAttributeGroupProductCategorysRepository;

        private GenericRepository<labelIcon> labelIconRepository;
        private GenericRepository<Holiday> holidayRepository;
        public GenericRepository<Holiday> HolidayRepository
        {
            get
            {
                if (this.holidayRepository == null)
                {
                    this.holidayRepository = new GenericRepository<Holiday>(context);
                }
                return holidayRepository;
            }
        }
        public GenericRepository<labelIcon> LabelIconRepository
        {
            get
            {
                if (this.labelIconRepository == null)
                {
                    this.labelIconRepository = new GenericRepository<labelIcon>(context);
                }
                return labelIconRepository;
            }
        }
        public GenericRepository<City> CityRepository
        {
            get
            {
                if (this.cityRepository == null)
                {
                    this.cityRepository = new GenericRepository<City>(context);
                }
                return cityRepository;
            }
        }
        public CreateOrderKeyService CreateOrderKeyRepository
        {
            get
            {
                if (this.createOrderKeyService == null)
                {
                    this.createOrderKeyService = new CreateOrderKeyService(context);
                }
                return createOrderKeyService;
            }

        }
        public GenericRepository<Province> ProvinceRepository
        {
            get
            {
                if (this.provinceRepository == null)
                {
                    this.provinceRepository = new GenericRepository<Province>(context);
                }
                return provinceRepository;
            }
        }
        public GenericRepository<SendwayBox> SendwayBoxRepository
        {
            get
            {
                if (this.sendwayBoxRepository == null)
                {
                    this.sendwayBoxRepository = new GenericRepository<SendwayBox>(context);
                }
                return sendwayBoxRepository;
            }
        }
   
        public GenericRepository<ProductSendWayBox> ProductSendWayBoxRepository
        {
            get
            {
                if (this.productSendWayBoxRepository == null)
                {
                    this.productSendWayBoxRepository = new GenericRepository<ProductSendWayBox>(context);
                }
                return productSendWayBoxRepository;
            }
        }
        public GenericRepository<ProductSendWayWorkTime> ProductSendWayWorkTimeRepository
        {
            get
            {
                if (this.productSendWayWorkTimeRepository == null)
                {
                    this.productSendWayWorkTimeRepository = new GenericRepository<ProductSendWayWorkTime>(context);
                }
                return productSendWayWorkTimeRepository;
            }
        }


        public GenericRepository<AdverestingLog> AdverestingLogRepository
        {
            get
            {
                if (this.adverestingLogRepository == null)
                {
                    this.adverestingLogRepository = new GenericRepository<AdverestingLog>(context);
                }
                return adverestingLogRepository;
            }
        }
        public GenericRepository<Adveresting> AdverestingRepository
        {
            get
            {
                if (this.adverestingRepository == null)
                {
                    this.adverestingRepository = new GenericRepository<Adveresting>(context);
                }
                return adverestingRepository;
            }
        }
        public GenericRepository<Offer> OfferRepository
        {
            get
            {
                if (this.offerRepository == null)
                {
                    this.offerRepository = new GenericRepository<Offer>(context);
                }
                return offerRepository;
            }
        }

        public GenericRepository<ProductCourse> ProductCourseRepository
        {
            get
            {
                if (this.productCourseRepository == null)
                {
                    this.productCourseRepository = new GenericRepository<ProductCourse>(context);
                }
                return productCourseRepository;
            }
        }
        public GenericRepository<ProductCourseLesson> ProductCourseLessonRepository
        {
            get
            {
                if (this.productCourseLessonRepository == null)
                {
                    this.productCourseLessonRepository = new GenericRepository<ProductCourseLesson>(context);
                }
                return productCourseLessonRepository;
            }
        }
        public GenericRepository<Tax> TaxRepository
        {
            get
            {
                if (this.taxRepository == null)
                {
                    this.taxRepository = new GenericRepository<Tax>(context);
                }
                return taxRepository;
            }
        } 
        public GenericRepository<ProductSendwayIrPostDetail> ProductSendwayIrPostDetailRepository
        {
            get
            {
                if (this.productSendwayIrPostDetails == null)
                {
                    this.productSendwayIrPostDetails = new GenericRepository<ProductSendwayIrPostDetail>(context);
                }
                return productSendwayIrPostDetails;
            }
        }
        public GenericRepository<ProductSendWaySelect> ProductSendWaySelectRepository
        {
            get
            {
                if (this.productSendWaySelect == null)
                {
                    this.productSendWaySelect = new GenericRepository<ProductSendWaySelect>(context);
                }
                return productSendWaySelect;
            }
        }
        public GenericRepository<ProductSendWayDetail> ProductSendWayDetailRepository
        {
            get
            {
                if (this.productSendWayDetail == null)
                {
                    this.productSendWayDetail = new GenericRepository<ProductSendWayDetail>(context);
                }
                return productSendWayDetail;
            }
        }
        public GenericRepository<ProductSendWay> ProductSendWayRepository
        {
            get
            {
                if (this.productSendWay == null)
                {
                    this.productSendWay = new GenericRepository<ProductSendWay>(context);
                }
                return productSendWay;
            }
        }
        public GenericRepository<ProductAccessory> ProductAccessoryRepository
        {
            get
            {
                if (this.productAccessory == null)
                {
                    this.productAccessory = new GenericRepository<ProductAccessory>(context);
                }
                return productAccessory;
            }
        }
        public GenericRepository<ProductIcon> ProductIconRepository
        {
            get
            {
                if (this.productIcon == null)
                {
                    this.productIcon = new GenericRepository<ProductIcon>(context);
                }
                return productIcon;
            }
        }
        public GenericRepository<ProductState> ProductStateRepository
        {
            get
            {
                if (this.productState == null)
                {
                    this.productState = new GenericRepository<ProductState>(context);
                }
                return productState;
            }
        }
        public GenericRepository<ProductFileItem> ProductFileItemRepository
        {
            get
            {
                if (this.productFileItem == null)
                {
                    this.productFileItem = new GenericRepository<ProductFileItem>(context);
                }
                return productFileItem;
            }
        }
        public GenericRepository<ProductFileInfo> ProductFileInfoRepository
        {
            get
            {
                if (this.productFileInfo == null)
                {
                    this.productFileInfo = new GenericRepository<ProductFileInfo>(context);
                }
                return productFileInfo;
            }
        }
        public GenericRepository<ProductMultipeItem> ProductMultipeItemRepository
        {
            get
            {
                if (this.productMultipeItem == null)
                {
                    this.productMultipeItem = new GenericRepository<ProductMultipeItem>(context);
                }
                return productMultipeItem;
            }
        }
        public GenericRepository<ProductType> ProductTypeRepository
        {
            get
            {
                if (this.productType == null)
                {
                    this.productType = new GenericRepository<ProductType>(context);
                }
                return productType;
            }
        }
        public GenericRepository<Law> LawRepository
        {
            get
            {
                if (this.lawRepository == null)
                {
                    this.lawRepository = new GenericRepository<Law>(context);
                }
                return lawRepository;
            }
        }
        public GenericRepository<SearchLog> SearchLogRepository
        {
            get
            {
                if (this.searchLogRepository == null)
                {
                    this.searchLogRepository = new GenericRepository<SearchLog>(context);
                }
                return searchLogRepository;
            }
        }
        public GenericRepository<Seller> SellerRepository
        {
            get
            {
                if (this.sellerRepository == null)
                {
                    this.sellerRepository = new GenericRepository<Seller>(context);
                }
                return sellerRepository;
            }
        }
        public GenericRepository<ProductSeller> ProductSellerRepository
        {
            get
            {
                if (this.productSellerRepository == null)
                {
                    this.productSellerRepository = new GenericRepository<ProductSeller>(context);
                }
                return productSellerRepository;
            }
        }
        public GenericRepository<ProductOffer> ProductOfferRepository
        {
            get
            {
                if (this.productOfferRepository == null)
                {
                    this.productOfferRepository = new GenericRepository<ProductOffer>(context);
                }
                return productOfferRepository;
            }
        }
        public GenericRepository<OfferProductCategory> OfferProductCategoryRepository
        {
            get
            {
                if (this.offerProductCategoryRepository == null)
                {
                    this.offerProductCategoryRepository = new GenericRepository<OfferProductCategory>(context);
                }
                return offerProductCategoryRepository;
            }
        }
        public GenericRepository<OfferUserGroup> OfferUserGroupRepository
        {
            get
            {
                if (this.offerUserGroupRepository == null)
                {
                    this.offerUserGroupRepository = new GenericRepository<OfferUserGroup>(context);
                }
                return offerUserGroupRepository;
            }
        }
        public GenericRepository<ProductPriceGroupModification> ProductPriceGroupModificationRepository
        {
            get
            {
                if (this.productPriceGroupModificationRepository == null)
                {
                    this.productPriceGroupModificationRepository = new GenericRepository<ProductPriceGroupModification>(context);
                }
                return productPriceGroupModificationRepository;
            }
        }
        public ProductPriceService ProductPriceRepository
        {
            get
            {
                if (this.productPriceRepository == null)
                {
                    this.productPriceRepository = new ProductPriceService(context, ProductRepository);
                }
                return productPriceRepository;
            }
        }
        public GenericRepository<ProductImage> ProductImageRepository
        {
            get
            {
                if (this.productImageRepository == null)
                {
                    this.productImageRepository = new GenericRepository<ProductImage>(context);
                }
                return productImageRepository;
            }
        }
        public GenericRepository<Brand> BrandRepository
        {
            get
            {
                if (this.brandRepository == null)
                {
                    this.brandRepository = new GenericRepository<Brand>(context);
                }
                return brandRepository;
            }
        }
        public GenericRepository<ProductAttributeTab> ProductAttributeTabRepository
        {
            get
            {
                if (this.productAttributeTabRepository == null)
                {
                    this.productAttributeTabRepository = new GenericRepository<ProductAttributeTab>(context);
                }
                return productAttributeTabRepository;
            }
        }
        public ProductAttributeSelectService ProductAttributeSelectRepository
        {
            get
            {
                if (this.productAttributeSelectRepository == null)
                {
                    this.productAttributeSelectRepository = new ProductAttributeSelectService(context);
                }
                return productAttributeSelectRepository;
            }
        }
        public GenericRepository<ProductAttributeItem> ProductAttributeItemRepository
        {
            get
            {
                if (this.productAttributeItemRepository == null)
                {
                    this.productAttributeItemRepository = new GenericRepository<ProductAttributeItem>(context);
                }
                return productAttributeItemRepository;
            }
        }
        public GenericRepository<ProductAttributeItemColor> ProductAttributeItemColorRepository
        {
            get
            {
                if (this.productAttributeItemColorRepository == null)
                {
                    this.productAttributeItemColorRepository = new GenericRepository<ProductAttributeItemColor>(context);
                }
                return productAttributeItemColorRepository;
            }
        }
        public GenericRepository<ProductAttributeGroupSelect> ProductAttributeGroupSelectRepository
        {
            get
            {
                if (this.productAttributeGroupSelectRepository == null)
                {
                    this.productAttributeGroupSelectRepository = new GenericRepository<ProductAttributeGroupSelect>(context);
                }
                return productAttributeGroupSelectRepository;
            }
        }
        public GenericRepository<ProductAttributeGroup> ProductAttributeGroupRepository
        {
            get
            {
                if (this.productAttributeGroupRepository == null)
                {
                    this.productAttributeGroupRepository = new GenericRepository<ProductAttributeGroup>(context);
                }
                return productAttributeGroupRepository;
            }
        }
        public GenericRepository<ProductAttribute> ProductAttributeRepository
        {
            get
            {
                if (this.productAttributeRepository == null)
                {
                    this.productAttributeRepository = new GenericRepository<ProductAttribute>(context);
                }
                return productAttributeRepository;
            }
        }
        public GenericRepository<GalleryCategory> GalleryCategoryRepository
        {
            get
            {
                if (this.galleryCategoryRepository == null)
                {
                    this.galleryCategoryRepository = new GenericRepository<GalleryCategory>(context);
                }
                return galleryCategoryRepository;
            }
        }
        public GenericRepository<GalleryImage> GalleryImageRepository
        {
            get
            {
                if (this.galleryImageRepository == null)
                {
                    this.galleryImageRepository = new GenericRepository<GalleryImage>(context);
                }
                return galleryImageRepository;
            }
        }
        public UserBonService UserBonRepository
        {
            get
            {
                if (this.userBonRepository == null)
                {
                    this.userBonRepository = new UserBonService(context);
                }
                return userBonRepository;
            }
        }
        public GenericRepository<UserBonLog> UserBonLogRepository
        {
            get
            {
                if (this.userBonLogRepository == null)
                {
                    this.userBonLogRepository = new GenericRepository<UserBonLog>(context);
                }
                return userBonLogRepository;
            }
        } 
        public GenericRepository<GeneralCodeGift> GeneralCodeGiftRepository
        {
            get
            {
                if (this.generalCodeGiftRepository == null)
                {
                    this.generalCodeGiftRepository = new GenericRepository<GeneralCodeGift>(context);
                }
                return generalCodeGiftRepository;
            }
        }
        public UserCodeGiftService UserCodeGiftRepository
        {
            get
            {
                if (this.userCodeGiftRepository == null)
                {
                    this.userCodeGiftRepository = new UserCodeGiftService(context);
                }
                return userCodeGiftRepository;
            }
        }

        public GenericRepository<UserCodeGiftLog> UserCodeGiftLogRepository
        {
            get
            {
                if (this.userCodeGiftLogRepository == null)
                {
                    this.userCodeGiftLogRepository = new GenericRepository<UserCodeGiftLog>(context);
                }
                return userCodeGiftLogRepository;
            }
        }
        public GenericRepository<GeneralCodeGiftLog> GeneralCodeGiftLogRepository
        {
            get
            {
                if (this.generalCodeGiftLogRepository == null)
                {
                    this.generalCodeGiftLogRepository = new GenericRepository<GeneralCodeGiftLog>(context);
                }
                return generalCodeGiftLogRepository;
            }
        }

        public GenericRepository<UserGroup> UserGroupRepository
        {
            get
            {
                if (this.userGroupRepository == null)
                {
                    this.userGroupRepository = new GenericRepository<UserGroup>(context);
                }
                return userGroupRepository;
            }
        }

        public GenericRepository<UserGroupSelect> UserGroupSelectRepository
        {
            get
            {
                if (this.userGroupSelectRepository == null)
                {
                    this.userGroupSelectRepository = new GenericRepository<UserGroupSelect>(context);
                }
                return userGroupSelectRepository;
            }
        }
        public GenericRepository<FreeSendOffer> FreeSendOfferRepository
        {
            get
            {
                if (this.freeSendOfferRepository == null)
                {
                    this.freeSendOfferRepository = new GenericRepository<FreeSendOffer>(context);
                }
                return freeSendOfferRepository;
            }
        }
        public GenericRepository<FreeSendOfferState> FreeSendOfferStateRepository
        {
            get
            {
                if (this.freeSendOfferStateRepository == null)
                {
                    this.freeSendOfferStateRepository = new GenericRepository<FreeSendOfferState>(context);
                }
                return freeSendOfferStateRepository;
            }
        }

        public GenericRepository<WalletAttributeWallet> WalletAttributeWalletRepository
        {
            get
            {
                if (this.walletAttributeWalletRepository == null)
                {
                    this.walletAttributeWalletRepository = new GenericRepository<WalletAttributeWallet>(context);
                }
                return walletAttributeWalletRepository;
            }
        }
        public GenericRepository<WalletAttribute> WalletAttributeRepository
        {
            get
            {
                if (this.walletAttributeRepository == null)
                {
                    this.walletAttributeRepository = new GenericRepository<WalletAttribute>(context);
                }
                return walletAttributeRepository;
            }
        }
        public GenericRepository<Wallet> WalletRepository
        {
            get
            {
                if (this.walletRepository == null)
                {
                    this.walletRepository = new GenericRepository<Wallet>(context);
                }
                return walletRepository;
            }
        }
        public GenericRepository<UserMessage> UserMessageRepository
        {
            get
            {
                if (this.userMessageRepository == null)
                {
                    this.userMessageRepository = new GenericRepository<UserMessage>(context);
                }
                return userMessageRepository;
            }
        }
        public GenericRepository<UserOfferMessage> UserOfferMessageRepository
        {
            get
            {
                if (this.userOfferMessageRepository == null)
                {
                    this.userOfferMessageRepository = new GenericRepository<UserOfferMessage>(context);
                }
                return userOfferMessageRepository;
            }
        }
        public GenericRepository<UserOfferMessageMember> UserOfferMessageMemberRepository
        {
            get
            {
                if (this.userOfferMessageMemberRepository == null)
                {
                    this.userOfferMessageMemberRepository = new GenericRepository<UserOfferMessageMember>(context);
                }
                return userOfferMessageMemberRepository;
            }
        }
        public UserAddressService UserAddressRepository
        {
            get
            {
                if (this.userAddressRepository == null)
                {
                    this.userAddressRepository = new UserAddressService(context);
                }
                return userAddressRepository;
            }
        }
        public GenericRepository<OrderWallet> OrderWalletRepository
        {
            get
            {
                if (this.orderWalletRepository == null)
                {
                    this.orderWalletRepository = new GenericRepository<OrderWallet>(context);
                }
                return orderWalletRepository;
            }
        }
        public GenericRepository<OrderRate> OrderRateRepository
        {
            get
            {
                if (this.orderRateRepository == null)
                {
                    this.orderRateRepository = new GenericRepository<OrderRate>(context);
                }
                return orderRateRepository;
            }
        }
        public GenericRepository<OrderRateItem> OrderRateItemRepository
        {
            get
            {
                if (this.orderRateItemRepository == null)
                {
                    this.orderRateItemRepository = new GenericRepository<OrderRateItem>(context);
                }
                return orderRateItemRepository;
            }
        }

        public GenericRepository<OrderDelivery> OrderDeliveryRepository
        {
            get
            {
                if (this.orderDeliveryRepository == null)
                {
                    this.orderDeliveryRepository = new GenericRepository<OrderDelivery>(context);
                }
                return orderDeliveryRepository;
            }
        }
        public GenericRepository<OrderRow> OrderRowRepository
        {
            get
            {
                if (this.orderRowRepository == null)
                {
                    this.orderRowRepository = new GenericRepository<OrderRow>(context);
                }
                return orderRowRepository;
            }
        }
        public OrderStateService OrderStateRepository
        {
            get
            {
                if (this.orderStateRepository == null)
                {
                    this.orderStateRepository = new OrderStateService(context);
                }
                return orderStateRepository;
            }
        }
        public OrderAttributeOrderService OrderAttributeOrderRepository
        {
            get
            {
                if (this.orderAttributeOrderRepository == null)
                {
                    this.orderAttributeOrderRepository = new OrderAttributeOrderService(context);
                }
                return orderAttributeOrderRepository;
            }
        }
        public GenericRepository<OrderAttribute> OrderAttributeRepository
        {
            get
            {
                if (this.orderAttributeRepository == null)
                {
                    this.orderAttributeRepository = new GenericRepository<OrderAttribute>(context);
                }
                return orderAttributeRepository;
            }
        }
        public OrderService OrderRepository
        {
            get
            {
                if (this.orderRepository == null)
                {
                    this.orderRepository = new OrderService(context,OrderStateRepository, ProductPriceRepository,orderAttributeOrderRepository,ProductRepository, CreateOrderKeyRepository);
                }
                return orderRepository;
            }
        }
        public GenericRepository<ForWhat> ForWhatRepository
        {
            get
            {
                if (this.forWhatRepository == null)
                {
                    this.forWhatRepository = new GenericRepository<ForWhat>(context);
                }
                return forWhatRepository;
            }
        }

        public GenericRepository<BankAccountOnlineInfo> BankAccountOnlineInfoRepository
        {
            get
            {
                if (this.bankAccountOnlineInfoRepository == null)
                {
                    this.bankAccountOnlineInfoRepository = new GenericRepository<BankAccountOnlineInfo>(context);
                }
                return bankAccountOnlineInfoRepository;
            }
        }

        public BankAccountService BankAccountRepository
        {
            get
            {
                if (this.bankAccountRepository == null)
                {
                    this.bankAccountRepository = new BankAccountService(context,createOrderKeyService);
                }
                return bankAccountRepository;
            }
        }

        public GenericRepository<HelpModule> HelpModuleRepository
        {
            get
            {
                if (this.helpModuleRepository == null)
                {
                    this.helpModuleRepository = new GenericRepository<HelpModule>(context);
                }
                return helpModuleRepository;
            }
        }
        public GenericRepository<HelpModuleSection> HelpModuleSectionRepository
        {
            get
            {
                if (this.helpModuleSectionRepository == null)
                {
                    this.helpModuleSectionRepository = new GenericRepository<HelpModuleSection>(context);
                }
                return helpModuleSectionRepository;
            }
        }
        public GenericRepository<HelpModuleSectionField> HelpModuleSectionFieldRepository
        {
            get
            {
                if (this.helpModuleSectionFieldRepository == null)
                {
                    this.helpModuleSectionFieldRepository = new GenericRepository<HelpModuleSectionField>(context);
                }
                return helpModuleSectionFieldRepository;
            }
        }
        public GenericRepository<ProductRankGroupSelect> ProductRankGroupSelectRepository
        {
            get
            {
                if (this.productRankGroupSelectRepository == null)
                {
                    this.productRankGroupSelectRepository = new GenericRepository<ProductRankGroupSelect>(context);
                }
                return productRankGroupSelectRepository;
            }
        }
        public GenericRepository<ProductRankGroup> ProductRankGroupRepository
        {
            get
            {
                if (this.productRankGroupRepository == null)
                {
                    this.productRankGroupRepository = new GenericRepository<ProductRankGroup>(context);
                }
                return productRankGroupRepository;
            }
        }
        public GenericRepository<ProductRankSelect> ProductRankSelectRepository
        {
            get
            {
                if (this.productRankSelectRepository == null)
                {
                    this.productRankSelectRepository = new GenericRepository<ProductRankSelect>(context);
                }
                return productRankSelectRepository;
            }
        }
        public GenericRepository<ProductRankSelectValue> ProductRankSelectValueRepository
        {
            get
            {
                if (this.productRankSelectValueRepository == null)
                {
                    this.productRankSelectValueRepository = new GenericRepository<ProductRankSelectValue>(context);
                }
                return productRankSelectValueRepository;
            }
        }
        public GenericRepository<ProductRank> ProductRankRepository
        {
            get
            {
                if (this.productRankRepository == null)
                {
                    this.productRankRepository = new GenericRepository<ProductRank>(context);
                }
                return productRankRepository;
            }
        }
        public ProductQuestionService ProductQuestionRepository
        {
            get
            {
                if (this.productQuestionRepository == null)
                {
                    this.productQuestionRepository = new ProductQuestionService(context, ProductRepository);
                }
                return productQuestionRepository;
            }
        }
        public ProductLetmeKnowService ProductLetmeknowRepository
        {
            get
            {
                if (this.productLetmeknowRepository == null)
                {
                    this.productLetmeknowRepository = new ProductLetmeKnowService(context,ProductRepository);
                }
                return productLetmeknowRepository;
            }
        }
        public ProductFavorateService ProductFavorateRepository
        {
            get
            {
                if (this.productFavorateRepository == null)
                {
                    this.productFavorateRepository = new ProductFavorateService(context);
                }
                return productFavorateRepository;
            }
        }
        public GenericRepository<ProductDisAdvantage> ProductDisAdvantageRepository
        {
            get
            {
                if (this.productDisAdvantageRepository == null)
                {
                    this.productDisAdvantageRepository = new GenericRepository<ProductDisAdvantage>(context);
                }
                return productDisAdvantageRepository;
            }
        }
        public GenericRepository<ProductCommentDisAdvantage> ProductCommentDisAdvantageRepository
        {
            get
            {
                if (this.productCommentDisAdvantageRepository == null)
                {
                    this.productCommentDisAdvantageRepository = new GenericRepository<ProductCommentDisAdvantage>(context);
                }
                return productCommentDisAdvantageRepository;
            }
        }
        public GenericRepository<ProductCommentAdvantage> ProductCommentAdvantageRepository
        {
            get
            {
                if (this.productCommentAdvantageRepository == null)
                {
                    this.productCommentAdvantageRepository = new GenericRepository<ProductCommentAdvantage>(context);
                }
                return productCommentAdvantageRepository;
            }
        }
        public ProductCommentService ProductCommentRepository
        {
            get
            {
                if (this.productCommentRepository == null)
                {
                    this.productCommentRepository = new ProductCommentService(context, ProductRepository);
                }
                return productCommentRepository;
            }
        }
        public ProductCategoryService ProductCategoryRepository
        {
            get
            {
                if (this.productCategoryRepository == null)
                {
                    this.productCategoryRepository = new ProductCategoryService(context);
                }
                return productCategoryRepository;
            }
        }
        public ProductCategoryAttributeService ProductCategoryAttributeRepository
        {
            get
            {
                if (this.productCategoryAttributeRepository == null)
                {
                    this.productCategoryAttributeRepository = new ProductCategoryAttributeService(context);
                }
                return productCategoryAttributeRepository;
            }

        }

        public ProductAttributeGroupProductCategorysService ProductAttributeGroupProductCategorysRepository
        {
            get
            {
                if (this.productAttributeGroupProductCategorysRepository == null)
                {
                    this.productAttributeGroupProductCategorysRepository = new ProductAttributeGroupProductCategorysService(context);
                }
                return productAttributeGroupProductCategorysRepository;
            }

        }
        public GenericRepository<ProductAdvantage> ProductAdvantageRepository
        {
            get
            {
                if (this.productAdvantageRepository == null)
                {
                    this.productAdvantageRepository = new GenericRepository<ProductAdvantage>(context);
                }
                return productAdvantageRepository;
            }
        }
        public ProductService ProductRepository
        {
            get
            {
                if (this.productRepository == null)
                {
                    this.productRepository = new ProductService(context, ProductCategoryRepository);
                }
                return productRepository;
            }
        }
        public GenericRepository<Tag> TagRepository
        {
            get
            {
                if (this.tagRepository == null)
                {
                    this.tagRepository = new GenericRepository<Tag>(context);
                }
                return tagRepository;
            }
        }
        public GenericRepository<Source> SourceRepository
        {
            get
            {
                if (this.sourceRepository == null)
                {
                    this.sourceRepository = new GenericRepository<Source>(context);
                }
                return sourceRepository;
            }
        }
        public GenericRepository<Social> SocialRepository
        {
            get
            {
                if (this.socialRepository == null)
                {
                    this.socialRepository = new GenericRepository<Social>(context);
                }
                return socialRepository;
            }
        }
        public GenericRepository<SearchEngineFact> SearchEngineFactRepository
        {
            get
            {
                if (this.searchEngineFactRepository == null)
                {
                    this.searchEngineFactRepository = new GenericRepository<SearchEngineFact>(context);
                }
                return searchEngineFactRepository;
            }
        }
        public GenericRepository<SearchEngineElementType> SearchEngineElementTypeRepository
        {
            get
            {
                if (this.searchEngineElementTypeRepository == null)
                {
                    this.searchEngineElementTypeRepository = new GenericRepository<SearchEngineElementType>(context);
                }
                return searchEngineElementTypeRepository;
            }
        }
        public GenericRepository<OtherImage> OtherImageRepository
        {
            get
            {
                if (this.otherImageRepository == null)
                {
                    this.otherImageRepository = new GenericRepository<OtherImage>(context);
                }
                return otherImageRepository;
            }
        }
        public GenericRepository<Menu> MenuRepository
        {
            get
            {
                if (this.menuRepository == null)
                {
                    this.menuRepository = new GenericRepository<Menu>(context);
                }
                return menuRepository;
            }
        }
        public GenericRepository<ContentRating> ContentRatingRepository
        {
            get
            {
                if (this.contentRatingRepository == null)
                {
                    this.contentRatingRepository = new GenericRepository<ContentRating>(context);
                }
                return contentRatingRepository;
            }
        }
        public GenericRepository<MetaSeo> MetaSeoRatingRepository
        {
            get
            {
                if (this.metaSeoRepository == null)
                {
                    this.metaSeoRepository = new GenericRepository<MetaSeo>(context);
                }
                return metaSeoRepository;
            }
        }
        public GenericRepository<CourseRating> CourseRatingRepository
        {
            get
            {
                if (this.courseRatingRepository == null)
                {
                    this.courseRatingRepository = new GenericRepository<CourseRating>(context);
                }
                return courseRatingRepository;
            }
        }
        public GenericRepository<Content> ContentRepository
        {
            get
            {
                if (this.contentRepository == null)
                {
                    this.contentRepository = new GenericRepository<Content>(context);
                }
                return contentRepository;
            }
        }
        public GenericRepository<Ticket> TicketRepository
        {
            get
            {
                if (this.ticketRepository == null)
                {
                    this.ticketRepository = new GenericRepository<Ticket>(context);
                }
                return ticketRepository;
            }
        }
        public GenericRepository<TicketCategory> TicketCategoryRepository
        {
            get
            {
                if (this.ticketCategoryRepository == null)
                {
                    this.ticketCategoryRepository = new GenericRepository<TicketCategory>(context);
                }
                return ticketCategoryRepository;
            }
        }

        public GenericRepository<FormRequest> FormRequestRepository
        {
            get
            {
                if (this.formRequestRepository == null)
                {
                    this.formRequestRepository = new GenericRepository<FormRequest>(context);
                }
                return formRequestRepository;
            }
        }


        public GenericRepository<FormRequestCategory> FormRequestCategoryRepository
        {
            get
            {
                if (this.formRequestCategoryRepository == null)
                {
                    this.formRequestCategoryRepository = new GenericRepository<FormRequestCategory>(context);
                }
                return formRequestCategoryRepository;
            }
        }

        public GenericRepository<StaticTextCategory> StaticTextCategoryRepository
        {
            get
            {
                if (this.staticTextCategoryCategoryRepository == null)
                {
                    this.staticTextCategoryCategoryRepository = new GenericRepository<StaticTextCategory>(context);
                }
                return staticTextCategoryCategoryRepository;
            }
        }
        public GenericRepository<StaticTextContent> StaticTextContentRepository
        {
            get
            {
                if (this.staticTextContentCategoryRepository == null)
                {
                    this.staticTextContentCategoryRepository = new GenericRepository<StaticTextContent>(context);
                }
                return staticTextContentCategoryRepository;
            }
        }
        public GenericRepository<ContactUs> ContactUsRepository
        {
            get
            {
                if (this.contactUsRepository == null)
                {
                    this.contactUsRepository = new GenericRepository<ContactUs>(context);
                }
                return contactUsRepository;
            }
        }
        public GenericRepository<Comment> CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new GenericRepository<Comment>(context);
                }
                return commentRepository;
            }
        }
        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(context);
                }
                return categoryRepository;
            }
        }
        public GenericRepository<EventLog> EventLogRepository
        {
            get
            {
                if (this.eventLogRepository == null)
                {
                    this.eventLogRepository = new GenericRepository<EventLog>(context);
                }
                return eventLogRepository;
            }
        }
        public GenericRepository<ApplicationUser> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<ApplicationUser>(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<attachment> AttachmentRepository
        {
            get
            {
                if (this.attachmentRepository == null)
                {
                    this.attachmentRepository = new GenericRepository<attachment>(context);
                }
                return attachmentRepository;
            }
        }
        public GenericRepository<Setting> SettingRepository
        {
            get
            {
                if (this.settingRepository == null)
                {
                    this.settingRepository = new GenericRepository<Setting>(context);
                }
                return settingRepository;
            }
        }
        public GenericRepository<ShoppingWorkTime> ShoppingWorkTimeRepository
        {
            get
            {
                if (this.shoppingWorkTimeRepository == null)
                {
                    this.shoppingWorkTimeRepository = new GenericRepository<ShoppingWorkTime>(context);
                }
                return shoppingWorkTimeRepository;
            }
        }
        public GenericRepository<Folder> FolderRepository
        {
            get
            {
                if (this.folderRepository == null)
                {
                    this.folderRepository = new GenericRepository<Folder>(context);
                }
                return folderRepository;
            }
        }
        public GenericRepository<FileType> FiletypeRepository
        {
            get
            {
                if (this.filetypeRepository == null)
                {
                    this.filetypeRepository = new GenericRepository<FileType>(context);
                }
                return filetypeRepository;
            }
        }
        public GenericRepository<SettingState> SettingStateRepository
        {
            get
            {
                if (this.settingStateRepository == null)
                {
                    this.settingStateRepository = new GenericRepository<SettingState>(context);
                }
                return settingStateRepository;
            }
        }
        public GenericRepository<ProductRandomSetting> ProductRandomSettingRepository
        {
            get
            {
                if (this.productRandomSettingRepository == null)
                {
                    this.productRandomSettingRepository = new GenericRepository<ProductRandomSetting>(context);
                }
                return productRandomSettingRepository;
            }
        }

        public GenericRepository<EmailSender> EmailsenderRepository
        {
            get
            {
                if (this.emailsenderRepository == null)
                {
                    this.emailsenderRepository = new GenericRepository<EmailSender>(context);
                }
                return emailsenderRepository;
            }
        }
        public GenericRepository<SmsSender> SmssenderRepository
        {
            get
            {
                if (this.smssenderRepository == null)
                {
                    this.smssenderRepository = new GenericRepository<SmsSender>(context);
                }
                return smssenderRepository;
            }
        }

        public GenericRepository<AdministratorModule> AdministratorModuleRepository
        {
            get
            {
                if (this.administratorModuleRepository == null)
                {
                    this.administratorModuleRepository = new GenericRepository<AdministratorModule>(context);
                }
                return administratorModuleRepository;
            }
        }
        public GenericRepository<AdministratorPermission> AdministratorPermissionRepository
        {
            get
            {
                if (this.administratorPermissionRepository == null)
                {
                    this.administratorPermissionRepository = new GenericRepository<AdministratorPermission>(context);
                }
                return administratorPermissionRepository;
            }
        }
        public GenericRepository<NewsLetterEmail> NewsLetterEmailRepository
        {
            get
            {
                if (this.newsLetterEmailRepository == null)
                {
                    this.newsLetterEmailRepository = new GenericRepository<NewsLetterEmail>(context);
                }
                return newsLetterEmailRepository;
            }
        }
        public GenericRepository<Slider> SliderRepository
        {
            get
            {
                if (this.sliderRepository == null)
                {
                    this.sliderRepository = new GenericRepository<Slider>(context);
                }
                return sliderRepository;
            }
        }
        public GenericRepository<SliderImage> SliderImageRepository
        {
            get
            {
                if (this.sliderImageRepository == null)
                {
                    this.sliderImageRepository = new GenericRepository<SliderImage>(context);
                }
                return sliderImageRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
        public async void SaveAsync()
        {
           await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
