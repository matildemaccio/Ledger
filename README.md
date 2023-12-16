# Ledger API
REST API for traditional double-entry accounting ledger, implemented in .NET 8.0.

## Prerequisites

### Download the Code

You can clone this repository using the following git command:

```bash
git clone https://github.com/matildemaccio/Ledger
```

### Install Docker
Make sure you have Docker installed on your machine. You can download it from [here](https://www.docker.com/products/docker-desktop/).

## Running the API
1. Start Docker.
2. Navigate to the root directory of the project and run the following command to build and start the Docker container:

```bash
docker-compose up
```

3. You can find out the port number on which the API is running by running the following command:
```bash
docker ps
```

The API will be accessible at ```http://localhost:PORT_NUMBER```.

## API Documentation
You can explore the API documentation by visiting ```http://localhost:PORT_NUMBER/swagger``` in your browser.
This page contains detailed information about the API endpoints, request and response models, and examples.
You can also try out the API endpoints directly from this page.

## Additional Considerations
### In-Memory Database Usage
The API uses an in-memory database to store the data. This means that the data will be lost when the API is stopped.

### Concurrency problem in ```POST /transaction``` endpoint
It is possible that two transactions are processed at the same time and the affected account balances are not updated correctly. In a real world scenario, this problem could solved in different ways:
- Implementing a **Distributed Lock** mechanism, such as using Redis, ensures that only one transaction is processed at a time, preventing conflicting updates to the balance.
- Utilizing **Row-Level Locks**, such as with a SQL Database and a ```SELECT FOR UPDATE``` statement, ensures exclusive access to the selected rows during the update, preventing concurrent modifications.
- Employing a SQL Database and an **UPDATE statement with a WHERE clause** that checks and updates the current balance atomically, ensures a consistent and atomic balance update operation.

### Accounts resource path
Even though the instructions specifies using ```/account``` (in singular) as the resource path for the account endpoints, I used the ```/accounts``` (in plural) instead, following the REST API naming conventions. Also, being consistent with the ```/transactions``` resource path.