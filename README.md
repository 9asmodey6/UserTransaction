# Distributed User Registration Transaction

A microservice-based application demonstrating how independent services interact in a distributed architecture. The project showcases how to maintain data consistency across multiple independent services using the Saga orchestration pattern and compensating actions.

## Overview

When a user attempts to register, the system must validate the username, log the attempt, and persist the data across three separate services. If any step fails (e.g., the database service is unavailable, or a business rule is violated), the system automatically triggers compensating events (like releasing a reserved username) to roll back the transaction and maintain a consistent state.

## Tech Stack & Libraries

- **C# / .NET 8**
- **MassTransit** (Saga State Machine & Message Broker Integration)
- **RabbitMQ** (Asynchronous Messaging)
- **gRPC** (Synchronous Service-to-Service Communication)
- **Docker & Docker Compose**

## Architecture

- **Service A (Orchestrator & API):** Exposes the REST endpoint for the client and orchestrates the distributed transaction using a MassTransit State Machine.
- **Service B (Validation):** Operates asynchronously via RabbitMQ. Responsible for validating and temporarily reserving usernames, as well as providing compensation logic to release the username if the overall transaction fails.
- **Service C (Persistence):** Simulates a database service. Communicates synchronously with Service A via gRPC to finalize user creation.

## How to Run

1. Clone the repository to your local machine:
   ```bash
   git clone https://github.com/9asmodey6/UserTransaction.git
   cd UserTransaction
   ```
2. Open a terminal in the root directory of the project.
3. Run the following command to build and start the microservices along with RabbitMQ:
   ```bash
   docker compose up --build
   ```
4. Once the containers are running, navigate to `http://localhost:5100/swagger/` in your browser to manually invoke the endpoint and start the distributed transaction.

---
*Note: This project was developed as part of a technical assignment for a Backend Developer role.*
