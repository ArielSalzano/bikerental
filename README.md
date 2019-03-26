# bikerental

Rentals are stored in a databa, along with the business rules (types of rentals, prices, and discounts for promo)

bikerental.sql is a script to create all db object (tables, stored procedures and user functions)

edit web.config to set up proper db string connection

call method  AddBikeRent  to register a bike rent
call method  BikeRentDetail to display a given bike rent detail
call method  PromotionChangeDiscount to modify promotion discount
call method  RentaltypeChangePrice to modify any rental price 

edit web.config section applicationSettings/BikeRental.My.MySettings/setting name="promosetting" to modify min and max number of elements of promo


