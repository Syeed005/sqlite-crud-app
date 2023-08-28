# SQlite Crud App
Sample data loading to SQlite database by external C# based application. This is a test application of an online shopping system. There are four tables defined in SQLite database: - User, Product, Order, and OrderLine Table. Now an external application is created to load data into this table.

A reasonably big database is created with 1000 users, 2000 products, and 10,000 orders. Each order might have up to 10 products bought.

# How Data are loaded
## Sample User data loading
There is an xml file called User.xml in the bin directory where the source user data are recorder. where `SaveSampleUserData(sqliteConnection)` runs, it collects data from the xml and load data into sqlite User table.

## Sample Product data loading
Similarly, we have another xml called Product.xml which acts as source data. While running the `SaveSampleProductData(sqliteConnection)`, it read data from this xml and save into Product table.

## Sample OrderLine data loading
As we have to load atlesat 10,000 order, we are using a simple while loop and generating random number for product id and quantity and recording the same to Order line table. `SaveOrderLineData(sqliteConnection)` is doing the job.

## Sample Order data loading
Here another while loop is used to generate order id, also read data from order line table and sum the total cost and calculate tax and then insert into the DB. Run the `SaveOrderData(sqliteConnection)` function in the Main function to load the data.
