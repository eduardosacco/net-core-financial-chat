# net-core-financial-chat
A simple browser-based chat application using .NET made with love by [Eduardo Sacco](https://www.linkedin.com/in/esacco/).

<p align=center><img src="img/stonks.png" height="200">

## Introduction
Financial chat is composed by:
 - **FinancialChat:** webpage that hosts the chat room.
 - **StockBot:** a bot that can fetch stock prices.
 - **RabbitMQ Server:** Message broker that enables chat and bot to communicate.

## Running the whole thing
1. **Start RabbitMQ Server**
   
   Before starting FinancialChat make sure to install [Docker](https://docs.docker.com/get-docker/) and run RabbitMQ Server using:
   
   `docker run -d --hostname my-rabbit --name ecom-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management`

2. **Run FinancialChat web app**
   
   Open the **FinancialChat Solution** in Visual Studio and run the **FinancialChat Project** using IIS Express. This should be de default config if you just press the play button or F5.

3. **Run the StockBot console app**
   
   From the Solution explorer right click the **StockBot Project** and select **Debug>Start New Instance**

## StockBot
The console app can fetch Stock prices info from either [AlphaVantage](https://www.alphavantage.co/) or [Stooq](https://stooq.com/).

The app has been pre configured with a free test Alpha Vantage API key. This key has limitations and may expire.

>If AlphaVantage commands are not working you may need to [get a new free key](https://www.alphavantage.co/support/#api-key) and replace the value for "AlphaVantageApiKey" with the key you obtained in StockBot project appsettings.json config file.

### Commands
Simply write the desired command in the chat window to use StockBot.

- AlphaVantage: `/stockav=IBM` where `IBM` can be replaced for any ticker symbol supported by AlphaVantage, eg: `A`, `MSFT`, `AAPL`.
- Stooq: `/stock=AAPL.US` where `AAPL.US` can be replaced for any ticker symbol supported by Stooq, eg: `GOOGL.US`, `MSFT.US`, `TSLA.US`.
