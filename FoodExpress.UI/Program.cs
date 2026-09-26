using FoodExpress.Infrastructure.Repositories;
using FoodExpress.Infrastructure.Services;
using FoodExpress.Service.Services;
using FoodExpress.UI.UI;

var fileManager = new FileManager();

var userRepository =
    new UserRepository(fileManager);

var menuRepository =
    new MenuRepository(fileManager);

var restaurantRepository =
    new RestaurantRepository(fileManager);


var orderRepository =
    new OrderRepository(fileManager);

var cartRepository =
    new CartRepository(fileManager);


var promoCodeRepository =
    new PromoCodeRepository(fileManager);

var userService =
    new UserService(userRepository);

var menuService =
    new MenuService(menuRepository);

var restaurantService =
    new RestaurantService(
        restaurantRepository);


var cartService =
    new CartService(
        cartRepository,
        menuService);



var promoCodeService =
    new PromoCodeService(
        promoCodeRepository);

var emailService =
    new EmailService();

var logger =
    new Logger(fileManager);

var orderService =
    new OrderService(
        orderRepository,
        userRepository,
        cartService,
        promoCodeService,
        emailService,
        logger);

var adminService =
    new AdminService(
        menuService,
        orderService,
        restaurantService);


var authService =
    new AuthService(
        userService,
        emailService);



var authMenu =
    new AuthMenu(
        authService);

var menuUI =
    new MenuUI(
        menuService,
        adminService,
        cartService);

var restaurantUI =
    new RestaurantUI(
        restaurantService);

var orderUI =
    new OrderUI(
        orderService,
        adminService);

var cartUI =
    new CartUI(
        cartService,
        menuService,
        orderService);

var promoCodeUI =
    new PromoCodeUI(
        promoCodeService);

var adminMenu =
    new AdminMenu(
        menuUI,
        restaurantUI,
        orderUI,
        promoCodeUI);

var customerMenu =
    new CustomerMenu(
        menuUI,
        cartUI,
        orderUI,
        restaurantService);

var mainMenu =
    new MainMenu(
        authMenu,
        adminMenu,
        customerMenu);

await mainMenu.ShowAsync();