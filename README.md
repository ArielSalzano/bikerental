# bikerental

Rentals are stored in a database (ms sql server), along with the business rules (types of rentals, prices, and discounts for promo)

bikerental.sql is a script to create all db object (tables, stored procedures and user functions)

edit web.config to set up proper db string connection

BikeRental project can be compiled and run in a web server, or executed in visual studio 2017
Its a web service, exposing the following methods

call method  AddBikeRent  to register a bike rent
call method  BikeRentDetail to display a given bike rent detail
call method  PromotionChangeDiscount to modify promotion discount
call method  RentaltypeChangePrice to modify any rental price 

see BikeRental.png

edit web.config section applicationSettings/BikeRental.My.MySettings/setting name="promosetting" to modify min and max number of elements of promo


