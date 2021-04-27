# Insecure Bank (.NET Core)
![Insecure-Bank](https://hdivsecurity.com/img/bank.png)
## Running the application locally

1. Clone the repository:

        $ git clone https://github.com/hdiv/insecure-bank-net.git

2. Build and run the application:

       $ dotnet build
       $ dotnet run

3. You can then access the bank application here: http://localhost:5000

## Running with Docker

Run the insecure-bank application with Docker.

        $ docker-compose build insecure-bank
        $ docker-compose up insecure-bank

Open the application here: http://localhost:5000

If you want to run the application with the agent enabled you must enter your credentials in the **.env** file:
* **NEXUS_USERNAME** username to access Hdiv's Nexus repository
* **NEXUS_PASSWORD** password for the previous user
* **HDIV_LICENSE_DATA** license for the agent as a base64 string
* **HDIV_AGENT_VERSION** version of the agent

Then run insecure-bank with Docker (remember to specify the windows or linux suffix depending on your OS).

        $ docker-compose build insecure-bank-agent-{windows|linux}
        $ docker-compose up insecure-bank-agent-{windows|linux}

## Login credentials
- Username: john
- Password: test